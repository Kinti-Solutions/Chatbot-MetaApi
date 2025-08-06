using System.Text.Json.Serialization;

namespace Chatbot.Models
{
    public class ImageMessage
    {
        public string Type { get; set; } = "image";
        public ImageContent Image { get; set; }

        public ImageMessage(string link)
        {
            Image = new ImageContent { Link = link };
        }
    }

    public class ImageContent
    {
        [JsonPropertyName("link")]
        public string Link { get; set; }
    }
}
