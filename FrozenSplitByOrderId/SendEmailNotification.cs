using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSplitByOrderId
{
    internal class SendEmailNotification
    {
        public static void SendEmail(string subject, string body, string? smtpClientAddrs)
        {
            //Log.Write_Log("Sending email...");

            var smtpClient = new SmtpClient(smtpClientAddrs)
            {
                Port = 25,
                //Credentials = new NetworkCredential("your-email@example.com", "your-password"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("Frozen_Split_Orders@rhenus.com"),
                Subject = subject,
                Body = body,
                IsBodyHtml = false,
            };

            // Retrieve email addresses from the file
            List<string> recipients = GetEmailRecipientsFromFile();
            foreach (var recipient in recipients)
            {
                mailMessage.To.Add(recipient);
            }

            try
            {
                smtpClient.Send(mailMessage);
                //Log.Write_Log("Email Sent");
            }
            catch (Exception ex)
            {
                //Log.Write_Log($"Error sending email: {ex.Message}");
            }
        }

        private static List<string> GetEmailRecipientsFromFile()
        {
            List<string> recipients = new List<string>();

            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "emailRecipients.txt");

                if (File.Exists(filePath))
                {
                    // Read email addresses from the file
                    string[] lines = File.ReadAllLines(filePath);
                    recipients.AddRange(lines);
                }
            }
            catch (Exception ex)
            {
                //Log.Write_Log($"Error reading email recipients from file: {ex.Message}");
            }

            return recipients;
        }
    }
}
