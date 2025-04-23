using ExpenseTracker.Model;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
using MailKit;

namespace ExpenseTracker.Services
{
    public class GmailImapService
    {
        public async Task<List<GmailNotification>> GetUserGmailNotificationsAsync(string email, string appPassword)
        {
            var notifications = new List<GmailNotification>();

            try
            {
                using (var client = new ImapClient())
                {
                    await client.ConnectAsync("imap.gmail.com", 993, SecureSocketOptions.SslOnConnect);
                    Console.WriteLine("[DEBUG] Connected to Gmail IMAP server.");

                    await client.AuthenticateAsync(email, appPassword);
                    Console.WriteLine($"[DEBUG] Authenticated successfully with email: {email}");

                    var inbox = client.Inbox;
                    await inbox.OpenAsync(FolderAccess.ReadOnly);

                    // **Fixed Query**: Removed 'FromContains(email)', as the user is the recipient.
                    var query = SearchQuery.SubjectContains("budget alert")
                                           .And(SearchQuery.DeliveredAfter(DateTime.Now.AddDays(-7)));

                    var results = await inbox.SearchAsync(query);
                    //Console.WriteLine($"[DEBUG] Found {results.Count} matching emails.");

                    foreach (var uid in results)
                    {
                        var message = await inbox.GetMessageAsync(uid);
                        var messageBody = message.HtmlBody ?? message.TextBody; // Ensure we get the body

                        if (!string.IsNullOrEmpty(messageBody) && messageBody.Contains("Expense Management System"))
                        {
                            notifications.Add(new GmailNotification
                            {
                                Subject = message.Subject,
                                Sender = message.From.ToString(),
                                DateReceived = message.Date.DateTime
                            });

                            Console.WriteLine($"[DEBUG] Email Found - Subject: {message.Subject}, From: {message.From}, Date: {message.Date}");
                        }
                    }

                    await client.DisconnectAsync(true);
                    Console.WriteLine("[DEBUG] Disconnected from Gmail IMAP server.");
                }
            }
            catch (AuthenticationException ex)
            {
                Console.WriteLine($"[ERROR] Authentication failed: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error fetching notifications: {ex.Message}");
                throw;
            }

            return notifications;
        }

    }
}
