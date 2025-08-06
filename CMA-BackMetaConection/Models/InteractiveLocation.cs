using System.Text.Json.Serialization;

namespace Chatbot.Models
{
    public class InteractiveLocation
    {
        public string Type { get; set; } = "location";
        public LocationContent Interactive { get; set; }

        public InteractiveLocation(string longitud, string latitud, string nombre, string direccion)
        {
            Interactive = new LocationContent
            {
                Latitude = latitud,
                Longitude = longitud,
                Name = nombre,
                Address = direccion
            };
        }

        public class LocationContent
        {
            [JsonPropertyName("latitude")]
            public string Latitude { get; set; }
            [JsonPropertyName("longitude")]
            public string Longitude { get; set; }
            [JsonPropertyName("name")]
            public string Name { get; set; }
            [JsonPropertyName("address")]
            public string Address { get; set; }
        }

    }
}
