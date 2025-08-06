using System.Text.Json.Serialization;

namespace Chatbot.Models
{
    public class InteractiveTemplate
    {
        public string Type { get; set; } = "template";
        public TemplateContent Interactive { get; set; }

        public InteractiveTemplate(string name, Language language, List<Component> components)
        {
            Interactive = new TemplateContent
            {
                name = name,
                language = language,
                components = components
            };
        }

        public class TemplateContent
        {
            [JsonPropertyName("name")]
            public string name { get; set; }
            [JsonPropertyName("language")]
            public Language language { get; set; }
            [JsonPropertyName("components")]
            public List<Component> components { get; set; }
        }

        public class Language
        {
            [JsonPropertyName("code")]
            public string code { get; set; }
        }

        public class Component
        {
            [JsonPropertyName("type")]
            public string type { get; set; }
            [JsonPropertyName("parameters")]
            public List<Parameter> parameters { get; set; }
        }

        public class Parameter
        {
            [JsonPropertyName("type")]
            public string type { get; set; } = "text";
            [JsonPropertyName("text")]
            public string text { get; set; }
        }
    }
}
