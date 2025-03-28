using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using NutriDiet.Repository.Interface;

namespace NutriDiet.Service.Utilities
{
    public class FirebaseService
    {
        private static FirebaseApp? _firebaseApp;
        private readonly IUnitOfWork _unitOfWork;

        public FirebaseService(IUnitOfWork unitOfWork)
        {
            if (_firebaseApp == null)
            {
                string? firebase = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS_JSON");
                _firebaseApp = FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromJson(firebase)
                });
            }
            _unitOfWork = unitOfWork;
        }

        public async Task SendNotification(string fcmToken, string? title, string body)
        {
            var message = new Message()
            {
                Token = fcmToken,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                }
            };

            var userId = await GetUserIdByFcmToken(fcmToken);

            if (userId == 0)
            {
                Console.WriteLine("User not found for FCM token: " + fcmToken);
                return;
            }

            try
            {
                Console.WriteLine("Sending notification to: " + fcmToken);
                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                Console.WriteLine("Notification sent successfully: " + response);

                var notification = new Repository.Models.Notification
                {
                    UserId = userId,
                    Title = title,
                    Description = body,
                    Status = "Sent",
                    Date = DateTime.UtcNow
                };
                await _unitOfWork.NotificationRepository.AddAsync(notification);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending notification: " + ex.Message);
                // Lưu thông báo lỗi nếu gửi thất bại
                var notification = new Repository.Models.Notification
                {
                    UserId = userId,
                    Title = title,
                    Description = body,
                    Status = "Failed",
                    Date = DateTime.UtcNow
                };
                await _unitOfWork.NotificationRepository.AddAsync(notification);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<int> GetUserIdByFcmToken(string fcmToken)
        {
            try
            {
                var user = await _unitOfWork.UserRepository
                    .GetByWhere(u => u.FcmToken == fcmToken).FirstOrDefaultAsync();

                return user.UserId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving UserId by FCM token: {ex.Message}");
                return 0;
            }
        }

        public async Task<(bool Success, string Message)> EnableReminderAsync(string mealType, string fcmToken)
        {
            var userId = await GetUserIdByFcmToken(fcmToken);
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return (false, "User not found");
            }

            if (string.IsNullOrEmpty(fcmToken))
            {
                throw new ArgumentException("FCM token is required to enable reminders.");
            }

            if (string.IsNullOrEmpty(user.FcmToken))
            {
                user.FcmToken = fcmToken;
                await _unitOfWork.UserRepository.UpdateAsync(user);
            }

            // Kiểm tra xem đã có bản ghi Pending cho mealType này chưa
            var existingReminder = await _unitOfWork.NotificationRepository
                .GetByWhere(n => n.UserId == userId && n.Title == mealType.ToLower() && n.Status == "Pending")
                .FirstOrDefaultAsync();

            if (existingReminder == null)
            {
                // Nếu chưa có, tạo mới bản ghi Pending
                var notification = new Repository.Models.Notification
                {
                    UserId = userId,
                    Title = mealType.ToLower(),
                    Description = $"Reminder enabled for {mealType}",
                    Status = "Pending",
                    Date = DateTime.UtcNow
                };
                await _unitOfWork.NotificationRepository.AddAsync(notification);
                await _unitOfWork.SaveChangesAsync();
                return (true, $"Reminder enabled for {mealType}");
            }
            else
            {
                // Nếu đã có, xóa bản ghi để hủy nhắc nhở
                await _unitOfWork.NotificationRepository.DeleteAsync(existingReminder);
                await _unitOfWork.SaveChangesAsync();
                return (true, $"Reminder disabled for {mealType}");
            }
        }
    }
}