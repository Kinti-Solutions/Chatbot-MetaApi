using System.Text.Json.Serialization;

namespace Chatbot.Models
{
    public class InteractiveList
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "interactive";
        public InteractiveContent Interactive { get; set; }

        public InteractiveList(string headerText, string bodyText, string footerText, string buttonText, List<Section> sections)
        {
            Interactive = new InteractiveContent
            {
                Type = "list",
                Header = new InteractiveHeader { Type = "text", Text = headerText },
                Body = new InteractiveBody { Text = bodyText },
                Footer = new InteractiveFooter { Text = footerText },
                Action = new InteractiveAction { Button = buttonText, Sections = sections }
            };
        }
    }

    public class InteractiveContent
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("header")]
        public InteractiveHeader Header { get; set; }
        [JsonPropertyName("body")]
        public InteractiveBody Body { get; set; }
        [JsonPropertyName("footer")]
        public InteractiveFooter Footer { get; set; }
        [JsonPropertyName("action")]
        public InteractiveAction Action { get; set; }
    }

    public class InteractiveHeader
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }

    public class InteractiveBody
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }

    public class InteractiveFooter
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }

    public class InteractiveAction
    {
        [JsonPropertyName("button")]
        public string Button { get; set; }
        [JsonPropertyName("sections")]
        public List<Section> Sections { get; set; }
    }

    public class Section
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("rows")]
        public List<Row> Rows { get; set; }
    }

    public class Row
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}
