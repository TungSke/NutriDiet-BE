using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

namespace NutriDiet.Service.Utilities
{
    public class FirebaseService
    {
        private static FirebaseApp? _firebaseApp;

        public FirebaseService()
        {
            if (_firebaseApp == null)
            {
                string? firebase = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS_JSON");
                _firebaseApp = FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromJson(firebase)
                });
            }
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

            try
            {
                Console.WriteLine("Sending notification to: " + fcmToken);
                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                Console.WriteLine("Notification sent successfully: " + response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending notification: " + ex.Message);
            }
        }
    }
}