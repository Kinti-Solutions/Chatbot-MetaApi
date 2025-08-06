using Chatbot.Utils;
using System.Text.Json.Serialization;

namespace Chatbot.Models
{
    public class WhatsAppMessage<T>
    {
        [JsonPropertyName("messaging_product")]
        public string MessagingProduct { get; set; } = "whatsapp";

        [JsonPropertyName("recipient_type")]
        public string RecipientType { get; set; } = "individual";

        [JsonPropertyName("to")]
        public string To { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        public T Content { get; set; }

        public WhatsAppMessage(string to, string type, T content)
        {
            To = to;
            Type = type;
            Content = content;
        }
    }
}
