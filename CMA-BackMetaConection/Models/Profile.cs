using System.Text.Json.Serialization;

namespace Chatbot.Models
{
    public class Profile
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}
