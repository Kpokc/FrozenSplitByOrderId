using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace FrozenSplitByOrderId
{
    internal class SendEmailNotification : Log
    {
        public static void SendEmail(string subject, string body, string? smtpClientAddrs)
        {
            if (string.IsNullOrEmpty(smtpClientAddrs))
            {
                // Log the email subject and body to a log file for tracking purposes
                Log.Write_Log($"{subject}\n{body}");
                Log.Write_Log("Please provide SMTP server address!");
                return;
            }
                
            // Log the email subject and body to a log file for tracking purposes
            Log.Write_Log($"{subject}\n{body}");

            // Initialize the SMTP client using the provided SMTP server address
            var smtpClient = new SmtpClient(smtpClientAddrs)
            {
                Port = 25, // Set the SMTP port to 25, which is a common port for sending emails
                           // Credentials can be set here if required; currently commented out for security reasons
                           // Credentials = new NetworkCredential("your-email@example.com", "your-password"),
                EnableSsl = true, // Enable SSL to ensure secure email transmission
            };

            // Create the email message with the specified subject and body
            var mailMessage = new MailMessage
            {
                From = new MailAddress("Frozen_Split_Orders@rhenus.com"), // Set the sender's email address
                Subject = subject, // Set the subject of the email
                Body = body, // Set the body content of the email
                IsBodyHtml = false, // Set to false to indicate that the body is plain text
            };

            // Retrieve the list of email recipients from an external file
            List<string> recipients = GetEmailRecipientsFromFile();
            foreach (var recipient in recipients)
            {
                // Add each recipient to the email's "To" field
                mailMessage.To.Add(recipient);
            }

            try
            {
                // Attempt to send the email
                smtpClient.Send(mailMessage);
                Log.Write_Log("Email Sent"); // Log a success message if the email is sent successfully
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during the email sending process
                Log.Write_Log($"Error sending email: {ex.Message}");
            }
        }

        private static List<string> GetEmailRecipientsFromFile()
        {
            // Initialize a list to hold the email addresses of the recipients
            List<string> recipients = new List<string>();

            try
            {
                // Define the path to the file that contains the list of email recipients
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "emailRecipients.txt");

                if (File.Exists(filePath))
                {
                    // If the file exists, read all lines (each line is an email address) and add them to the list
                    string[] lines = File.ReadAllLines(filePath);
                    recipients.AddRange(lines);
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur while reading the file
                Log.Write_Log($"Error reading email recipients from file: {ex.Message}");
            }

            // Return the list of recipients (could be empty if there was an error or no file)
            return recipients;
        }

    }
}
