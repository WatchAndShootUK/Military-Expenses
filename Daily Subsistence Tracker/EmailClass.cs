using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;


namespace Daily_Subsistence_Tracker
{
    public class EmailClass
    {
        public static async Task SendEmail(string subject, string body, List<string> recipients, List<EmailAttachment> attachments)
        {
            try
            {
                var message = new EmailMessage
                {
                    Subject = subject,
                    Body = body,
                    To = recipients,
                    Attachments = attachments
                };


                await Email.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException fbsEx)
            {
                // Email is not supported on this device
            }
            catch (Exception ex)
            {
                // Some other exception occurred
            }
        }
    }
}