using Microsoft.EntityFrameworkCore;
using NutriDiet.Repository.Interface;
using NutriDiet.Service.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace NutriDiet.Service.BackgroundServices
{
    public class NotificationBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public NotificationBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await SendMealReminders();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task SendMealReminders()
        {
            // Tạo scope mới cho mỗi lần chạy
            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var firebaseService = scope.ServiceProvider.GetRequiredService<FirebaseService>();

            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); // UTC+7
            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
            var users = await unitOfWork.UserRepository.GetAll().ToListAsync();

            foreach (var user in users)
            {
                if (string.IsNullOrEmpty(user.FcmToken))
                    continue;

                var reminders = await unitOfWork.NotificationRepository
                    .GetByWhere(n => n.UserId == user.UserId && n.Status == "Pending")
                    .ToListAsync();

                foreach (var reminder in reminders)
                {
                    var mealType = reminder.Title;
                    var mealLogs = await unitOfWork.MealLogRepository
                        .GetByWhere(ml => ml.UserId == user.UserId && ml.LogDate.HasValue && ml.LogDate.Value.Date == now.Date)
                        .Include(ml => ml.MealLogDetails)
                        .ToListAsync();

                    foreach (var mealLog in mealLogs)
                    {
                        var details = mealLog.MealLogDetails
                            .Where(d => d.MealType.ToLower() == mealType)
                            .ToList();

                        if (details.Any())
                        {
                            var (title, body, shouldSend) = GetNotificationDetails(mealType, now);
                            if (shouldSend)
                            {
                                await firebaseService.SendNotification(user.FcmToken, title, body);
                                reminder.Status = "Sent";
                                reminder.Date = DateTime.UtcNow;
                                await unitOfWork.NotificationRepository.UpdateAsync(reminder);
                                await unitOfWork.SaveChangesAsync();
                            }
                        }
                    }
                }
            }
        }

        private (string Title, string Body, bool ShouldSend) GetNotificationDetails(string mealType, DateTime now)
        {
            var hour = now.Hour;
            var minute = now.Minute;

            return mealType.ToLower() switch
            {
                "breakfast" => (
                    "Đến giờ ăn rồi!",
                    "Hãy ăn bữa sáng của bạn. Bỏ qua nếu bạn đã ăn sáng",
                    hour == 6 && minute >= 30 || (hour == 7) || (hour == 8 && minute <= 0)
                ),
                "lunch" => (
                    "Đến giờ ăn rồi!",
                    "Hãy ăn bữa trưa của bạn. Bỏ qua nếu bạn đã ăn trưa",
                    hour == 11 && minute >= 30 || (hour == 12) || (hour == 13 && minute <= 30)
                ),
                "dinner" => (
                    "Đến giờ ăn rồi!",
                    "Hãy ăn bữa tối của bạn. Bỏ qua nếu bạn đã ăn tối",
                    hour == 18 || (hour == 19 && minute <= 30)
                ),
                "snacks" => (
                    "Đến giờ ăn rồi!",
                    "Hãy ăn bữa phụ của bạn. Bỏ qua nếu bạn đã ăn",
                    hour == 15 || (hour == 16 && minute <= 0)
                ),
                _ => ("", "", false)
            };
        }
    }
}