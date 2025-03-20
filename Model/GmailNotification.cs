namespace ExpenseTracker.Model
{
    public class GmailNotification
    {
        public string Subject { get; set; }
        public string Sender { get; set; }
        public DateTime DateReceived { get; set; }
        public string Body { get; set; }
    }
}
