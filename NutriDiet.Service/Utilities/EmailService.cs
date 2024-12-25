using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace NutriDiet.Service.Utilities
{
    public class EmailService
    {
        private readonly IMemoryCache _cache;
        public EmailService()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
        }
        public async Task SendEmail(string email, string subject, string body)
        {
            try
            {
                var emailSender = Environment.GetEnvironmentVariable("EMAIL_SENDER");
                var emailSenderPassword = Environment.GetEnvironmentVariable("EMAIL_SENDER_PASSWROD");
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("no-reply@yourdomain.com", "NutriDiet Support Team");
                mail.To.Add(email);
                mail.Subject = subject;
                mail.Body = body ?? "No content available";
                mail.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.UseDefaultCredentials = false;
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                smtp.Credentials = new NetworkCredential(emailSender, emailSenderPassword);

                await smtp.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                throw new Exception("Error when sending email" + ex.Message);
            }
        }

        public async Task SendEmailWithOTP(string email, string subject)
        {
            try
            {
                var otp = GenerateOtp();

                _cache.Set(email, otp, TimeSpan.FromMinutes(5));

                var emailContent = $"<br/><br/>Your OTP Code is: <strong>{otp}</strong><br/>This code will expire in 5 minutes.";

                var emailSender = Environment.GetEnvironmentVariable("EMAIL_SENDER");
                var emailSenderPassword = Environment.GetEnvironmentVariable("EMAIL_SENDER_PASSWORD");

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress("no-reply@yourdomain.com", "NutriDiet Support Team"),
                    Subject = subject,
                    Body = emailContent,
                    IsBodyHtml = true
                };
                mail.To.Add(email);

                // 5. Gửi email qua SMTP
                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Credentials = new NetworkCredential(emailSender, emailSenderPassword);

                    await smtp.SendMailAsync(mail);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error when sending email: " + ex.Message);
            }
        }


        private string GenerateOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

    }
}
