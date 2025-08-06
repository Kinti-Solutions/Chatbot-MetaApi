using System.Text.Json.Serialization;

namespace Chatbot.Models
{
    public class Metadata
    {
        [JsonPropertyName("display_phone_number")]
        public string? Display_Phone_Number { get; set; }
        [JsonPropertyName("phone_number_id")]
        public string? Phone_Number_Id { get; set; }
    }
}

