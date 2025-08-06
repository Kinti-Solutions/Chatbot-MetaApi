using System.Text.Json.Serialization;

namespace Chatbot.Models
{
    public class InteractiveContact
    {
        public string Type { get; set; } = "contacts";
        public List<ContactContent> Interactive { get; set; }

        public InteractiveContact(ContactName name, List<ContactPhone> phones)
        {
            Interactive = new List <ContactContent> 
            { 
                new ContactContent
                {
                    Name = name,
                    Phones = phones
                } 
            };
        }

        public class ContactContent
        {
            [JsonPropertyName("name")]
            public ContactName Name { get; set; }
            [JsonPropertyName("phones")]
            public List<ContactPhone> Phones { get; set; }
        }

        public class ContactName
        {
            [JsonPropertyName("formatted_name")]
            public string formatted_name { get; set; }
            [JsonPropertyName("first_name")]
            public string first_name { get; set; }
            [JsonPropertyName("last_name")]
            public string last_name { get; set; }
            [JsonPropertyName("middle_name")]
            public string middle_name { get; set; }
            [JsonPropertyName("suffix")]
            public string suffix { get; set; }
            [JsonPropertyName("prefix")]
            public string prefix { get; set; }
        }

        public class ContactPhone
        {
            [JsonPropertyName("phone")]
            public string phone { get; set; }
            [JsonPropertyName("wa_id")]
            public string wa_id { get; set; }
            [JsonPropertyName("type")]
            public string type { get; set; }
        }
    }
}
