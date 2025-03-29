using Microsoft.EntityFrameworkCore;
using NutriDiet.Repository.Interface;
using NutriDiet.Service.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NutriDiet.Repository.Models;

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
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }

        }

        private async Task SendMealReminders()
        {
            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var firebaseService = scope.ServiceProvider.GetRequiredService<FirebaseService>();

            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); // UTC+7
            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
            
            var hour = now.Hour;
            var minute = now.Minute;

            if (!((hour == 8 && minute == 0) || (hour == 12 && minute == 0) ||
                  (hour == 15 && minute == 0) || (hour == 19 && minute == 0)))
            {
                return;
            }

            const string title = "Giờ ăn đến rồi!";
            string body = "Đã đến giờ ăn trong ngày, hãy kiểm tra nhật ký ăn uống.\nBỏ qua thông báo nếu bạn đã ăn hoặc không có món ăn trong bữa ăn hôm nay!";

            if (hour == 8 && minute == 0)
            {
                body = "Đã đến giờ ăn sáng, hãy kiểm tra nhật ký ăn uống.\nBỏ qua thông báo nếu bạn đã ăn hoặc không có món ăn trong bữa sáng hôm nay!";
            }

            else if (hour == 12 && minute == 0)
            {
                body = "Đã đến giờ ăn trưa, hãy kiểm tra nhật ký ăn uống.\nBỏ qua thông báo nếu bạn đã ăn hoặc không có món ăn trong bữa trưa hôm nay!";
            }

            else if (hour == 15 && minute == 0)
            {
                body = "Đã đến giờ ăn phụ, hãy kiểm tra nhật ký ăn uống.\nBỏ qua thông báo nếu bạn đã ăn hoặc không có món ăn trong bữa phụ hôm nay!";
            }

            else if (hour == 19 && minute == 0)
            {
                body = "Đã đến giờ ăn tối, hãy kiểm tra nhật ký ăn uống.\nBỏ qua thông báo nếu bạn đã ăn hoặc không có món ăn trong bữa tối hôm nay!";
            }

            var users = await unitOfWork.UserRepository
                .GetByWhere(u => u.EnableReminder == true && !string.IsNullOrEmpty(u.FcmToken))
                .ToListAsync();

            foreach (var user in users)
            {
                var mealLogs = await unitOfWork.MealLogRepository
                    .GetByWhere(x => x.UserId == user.UserId && x.LogDate.HasValue && x.LogDate.Value.Date == now.Date)
                    .Include(x => x.MealLogDetails)
                    .ToListAsync();

                // check có tồn tại meallog ko

                if (mealLogs.Any(x=>x.MealLogDetails.Any()))
                {
                    await firebaseService.SendNotification(user.FcmToken, title, body);
                    Console.WriteLine($"Đã gửi thông báo tới user {user.UserId} lúc {now}");
                }
            }
        }
    }
}