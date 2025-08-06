namespace Chatbot.Models;
public class WhatsAppWebhookRequest
{
    public string Object { get; set; }
    public List<Entry> Entry { get; set; }

}
