using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Http;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.IO;

namespace NutriDiet.Service.Utilities
{
    public class GoogleService
    {
        private readonly IMemoryCache _cache;

        public GoogleService(IMemoryCache cache)
        {
            _cache = cache;

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

        public async Task<bool> VerifyOtp(string email, string otp)
        {
            var cachedOtp = _cache.Get(email) as string;

            Console.WriteLine($"Cached OTP: {cachedOtp} for email: {email}");
            Console.WriteLine($"Provided OTP: {otp}");

            if (!string.IsNullOrEmpty(cachedOtp) && cachedOtp.Equals(otp))
            {
                _cache.Remove(email);
                return true;
            }

            return false;
        }


        private string GenerateOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        public async Task<string> UploadImageGGDrive(IFormFile file)
        {
            // Lấy dữ liệu JSON từ biến môi trường
            var credentialsJson = Environment.GetEnvironmentVariable("GOOGLE_CREDENTIALS_JSON");

            if (string.IsNullOrEmpty(credentialsJson))
            {
                throw new Exception("Google credentials JSON not found in environment variables.");
            }

            GoogleCredential credential;

            try
            {
                credential = GoogleCredential.FromJson(credentialsJson)
                    .CreateScoped(new[] { DriveService.ScopeConstants.DriveFile });

                var service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "NutriDiet Upload App"
                });

                var fileMetaData = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = file.FileName,
                    Parents = new List<string> { "1ut6fCcK_V8x9G81AXov98ob37_1dtv-U" }
                };

                FilesResource.CreateMediaUpload request;

                using (var streamFile = file.OpenReadStream())
                {
                    request = service.Files.Create(fileMetaData, streamFile, file.ContentType);
                    request.Fields = "id";
                    var progress = await request.UploadAsync();

                    if (progress.Status == UploadStatus.Failed)
                    {
                        throw new Exception($"File upload failed: {progress.Exception.Message}");
                    }

                    var uploadedFile = request.ResponseBody;
                    var fileUrl = $"https://drive.google.com/thumbnail?id={uploadedFile.Id}";
                    return fileUrl;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during file upload: {ex.Message}");
                return null;
            }
        }

        public async Task<string> UploadImageWithCloudDinary(IFormFile file)
        {
            Cloudinary cloudinary = new Cloudinary(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));
            cloudinary.Api.Secure = true;
            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, stream), // Đặt tên tệp và nội dung
                    Folder = "images",                      // Thay bằng tên folder bạn muốn lưu trên Cloudinary
                    PublicId = Path.GetFileNameWithoutExtension(file.FileName), // Đặt PublicId dựa trên tên file
                    Overwrite = true                                 // Ghi đè nếu đã có file trùng PublicId
                };

                // Upload file và nhận kết quả
                var uploadResult = await cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return uploadResult.SecureUrl.ToString(); // Trả về URL ảnh đã upload
                }
                else
                {
                    throw new Exception($"Cloudinary upload failed: {uploadResult.Error.Message}");
                }
            }
        }
    }
}
