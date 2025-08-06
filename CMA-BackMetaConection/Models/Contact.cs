using System.Text.Json.Serialization;
using static Chatbot.Models.WhatsAppWebhookRequest;

namespace Chatbot.Models
{
    public class Contact
    {
        [JsonPropertyName("wa_id")]
        public string? Wa_Id { get; set; }
        [JsonPropertyName("user_id")]
        public string? User_id { get; set; }
        [JsonPropertyName("profile")]
        public Profile? Profile { get; set; }
    }
}
