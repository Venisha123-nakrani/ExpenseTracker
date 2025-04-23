using MailKit.Net.Imap;
using MailKit;

namespace ExpenseTracker.Services
{
    public class GmailService
    {


        private readonly IConfiguration _configuration;

        public GmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<EmailModel> GetRecentEmails(int maxEmails = 5)
        {
            var emailList = new List<EmailModel>();
            var gmailSettings = _configuration.GetSection("GmailSettings");

            using (var client = new ImapClient())
            {
                try
                {
                    // Connect to Gmail IMAP Server
                    client.Connect(gmailSettings["ImapHost"], int.Parse(gmailSettings["ImapPort"]), true);

                    // Authenticate using Gmail App Password
                    client.Authenticate(gmailSettings["Email"], gmailSettings["AppPassword"]);

                    // Open Inbox folder
                    var inbox = client.Inbox;
                    inbox.Open(FolderAccess.ReadOnly);

                    // Fetch the latest 'maxEmails' messages
                    for (int i = inbox.Count - 1; i >= Math.Max(inbox.Count - maxEmails, 0); i--)
                    {
                        var message = inbox.GetMessage(i);

                        emailList.Add(new EmailModel
                        {
                            Subject = message.Subject,
                            From = message.From.ToString(),
                            Date = message.Date.DateTime.ToString("dd/MM/yyyy"),
                            Body = message.TextBody ?? "No Content"
                        });
                    }

                    client.Disconnect(true);
                }
                catch (Exception ex)
                {
                    emailList.Add(new EmailModel
                    {
                        Subject = "Error",
                        From = "System",
                        Date = DateTime.Now.ToString("dd/MM/yyyy"),
                        Body = "Error fetching Gmail notifications: " + ex.Message
                    });
                }
            }

            return emailList;
        }
    }
    public class EmailModel
    {
        public string Subject { get; set; }
        public string From { get; set; }
        public string Date { get; set; }
        public string Body { get; set; }
    }

}

