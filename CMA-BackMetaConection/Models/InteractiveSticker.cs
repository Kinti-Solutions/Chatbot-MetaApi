using System.Text.Json.Serialization;

namespace Chatbot.Models
{
    public class InteractiveSticker
    {
        public string Type { get; set; } = "sticker";
        public StickerContent Interactive { get; set; }

        public InteractiveSticker(string id)
        {
            Interactive = new StickerContent
            {
                Id = id
            };
        }

        public class StickerContent
        {
            [JsonPropertyName("id")]
            public string Id { get; set; }
        }

    }
}
