namespace Application.Models
{
    public class SendMailRequest
    {
        public required string Receiver { get; set; }
        public required string Subject { get; set; }
        public required string Content { get; set; }
        public bool IsHtml { get; set; }
    }
}
