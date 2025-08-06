using static Chatbot.Models.WhatsAppWebhookRequest;

namespace Chatbot.Models
{
    public class Entry
    {
        public string Id { get; set; }
        public List<Change> Changes { get; set; }
    }
}
