using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Reflection;
using Chatbot.Models;

namespace Chatbot.Utils
{
    public class DynamicPropertyNameConverter<T> : JsonConverter<WhatsAppMessage<T>>
    {
        public override WhatsAppMessage<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException("Deserialization not implemented for this converter.");
        }

        public override void Write(Utf8JsonWriter writer, WhatsAppMessage<T> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString("messaging_product", value.MessagingProduct);
            writer.WriteString("recipient_type", value.RecipientType);
            writer.WriteString("to", value.To);
            writer.WriteString("type", value.Type);

            string contentPropertyName = value.Type;
            writer.WritePropertyName(contentPropertyName);
            JsonSerializer.Serialize(writer, value.Content, options);

            writer.WriteEndObject();
        }
    }
}
