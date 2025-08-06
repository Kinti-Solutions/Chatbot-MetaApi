using System.Text.Json.Serialization;
using static Chatbot.Models.WhatsAppWebhookRequest;

namespace Chatbot.Models
{
    public class Value
    {
        [JsonPropertyName("messaging_product")]
        public string? Messaging_Product { get; set; }
        [JsonPropertyName("metadata")]
        public Metadata? Metadata { get; set; }
        [JsonPropertyName("contacts")]
        public List<Contact>? Contacts { get; set; }
        [JsonPropertyName("messages")]
        public List<Message>? Messages { get; set; }
        [JsonPropertyName("statuses")]
        public List<Status>? Statuses { get; set; }
    }
}
