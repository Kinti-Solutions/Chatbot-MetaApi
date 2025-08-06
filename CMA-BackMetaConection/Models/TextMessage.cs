using System.Text.Json.Serialization;

namespace Chatbot.Models
{
    public class TextMessage
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "text";

        [JsonPropertyName("text")]
        public TextContent Text { get; set; }

        public TextMessage(string body, bool previewUrl = false)
        {
            Text = new TextContent { Body = body, PreviewUrl = previewUrl };
        }
    }

    public class TextContent
    {

        // Propiedad para habilitar o deshabilitar la previsualización de URLs
        [JsonPropertyName("preview_url")]
        public bool PreviewUrl { get; set; }

        // El cuerpo del mensaje de texto
        [JsonPropertyName("body")]
        public string Body { get; set; }
    }
}
