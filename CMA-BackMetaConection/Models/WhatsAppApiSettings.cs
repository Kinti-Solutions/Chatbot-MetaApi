namespace Chatbot.Models
{
    public class WhatsAppApiSettings
    {
        public string GraphApiToken { get; set; }
        public string WebhookVerifyToken { get; set; }
        public string UrlApi { get; set; }
        public string BusinessPhoneNumberId { get; set; }
        public string WriteOnFile { get; set; }
        public bool EnableWriteOnFile { get; set; }
    }
}
