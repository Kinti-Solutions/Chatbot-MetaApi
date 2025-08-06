using System.Text.Json.Serialization;

namespace Chatbot.Models
{
    public class InteractiveButton
    {
        public string Type { get; set; } = "interactive";
        public InteractiveButtonContent Interactive { get; set; }

        public InteractiveButton(string bodyText, List<Button> buttons)
        {
            Interactive = new InteractiveButtonContent
            {
                Type = "button",
                Body = new InteractiveBody { Text = bodyText },
                Action = new ButtonAction { Buttons = buttons }
            };
        }
    }

    public class InteractiveButtonContent
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("body")]
        public InteractiveBody Body { get; set; }
        [JsonPropertyName("action")]
        public ButtonAction Action { get; set; }
    }

    public class ButtonAction
    {
        [JsonPropertyName("buttons")]
        public List<Button> Buttons { get; set; }
    }

    public class Button
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "reply";
        [JsonPropertyName("reply")]
        public Reply Reply { get; set; }
    }

    public class Reply
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
    }
}
