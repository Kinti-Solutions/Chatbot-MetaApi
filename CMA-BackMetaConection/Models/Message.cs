using Microsoft.AspNetCore.Diagnostics;
using System.Text.Json.Serialization;

namespace Chatbot.Models
{
    public class Message
    {
        [JsonPropertyName("from")]
        public string? From { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("timestamp")]
        public string? Timestamp { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("text")]
        public TextContentRequest? Text { get; set; }

        [JsonPropertyName("audio")]
        public AudioContentRequest? Audio { get; set; }

        [JsonPropertyName("button")]
        public ButtonContentRequest? Button { get; set; }

        [JsonPropertyName("context")]
        public ContextContentRequest? Context { get; set; }

        [JsonPropertyName("document")]
        public DocumentContentRequest? Document { get; set; }

        [JsonPropertyName("errors")]
        public List<ErrorContentRequest>? Errors { get; set; }

        [JsonPropertyName("image")]
        public ImageContentRequest? Image { get; set; }

        [JsonPropertyName("interactive")]
        public InteractiveContentRequest? Interactive { get; set; }

        [JsonPropertyName("order")]
        public OrderContentRequest? Order { get; set; }

        [JsonPropertyName("referral")]
        public ReferralContentRequest? Referral { get; set; }

        [JsonPropertyName("sticker")]
        public StickerContentRequest? Sticker { get; set; }

        [JsonPropertyName("system")]
        public SystemContentRequest? System { get; set; }

        [JsonPropertyName("video")]
        public VideoContentRequest? Video { get; set; }
    }

    public class TextContentRequest
    {
        [JsonPropertyName("body")]
        public string? Body { get; set; }
    }

    public class AudioContentRequest
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("mime_type")]
        public string? MimeType { get; set; }
    }

    public class ButtonContentRequest
    {
        [JsonPropertyName("payload")]
        public string? Payload { get; set; }

        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }

    public class ContextContentRequest
    {
        [JsonPropertyName("forwarded")]
        public bool? Forwarded { get; set; }

        [JsonPropertyName("frequently_forwarded")]
        public bool? FrequentlyForwarded { get; set; }

        [JsonPropertyName("from")]
        public string? From { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("referred_product")]
        public ReferredProductRequest? ReferredProduct { get; set; }
    }

    public class ReferredProductRequest
    {
        [JsonPropertyName("catalog_id")]
        public string? CatalogId { get; set; }

        [JsonPropertyName("product_retailer_id")]
        public string? ProductRetailerId { get; set; }
    }

    public class DocumentContentRequest
    {
        [JsonPropertyName("caption")]
        public string? Caption { get; set; }

        [JsonPropertyName("filename")]
        public string? Filename { get; set; }

        [JsonPropertyName("sha256")]
        public string? Sha256 { get; set; }

        [JsonPropertyName("mime_type")]
        public string? MimeType { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }
    }

    public class ErrorContentRequest
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("error_data")]
        public ErrorDataRequest? ErrorData { get; set; }
    }

    public class ErrorDataRequest
    {
        [JsonPropertyName("details")]
        public string? Details { get; set; }
    }

    public class ImageContentRequest
    {
        [JsonPropertyName("caption")]
        public string? Caption { get; set; }

        [JsonPropertyName("sha256")]
        public string? Sha256 { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("mime_type")]
        public string? MimeType { get; set; }
    }

    public class InteractiveContentRequest
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("button_reply")]
        public ButtonReplyRequest? ButtonReply { get; set; }

        [JsonPropertyName("list_reply")]
        public ListReplyRequest? ListReply { get; set; }
    }

    public class ButtonReplyRequest
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }
    }

    public class ListReplyRequest
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }

    public class OrderContentRequest
    {
        [JsonPropertyName("catalog_id")]
        public string? CatalogId { get; set; }

        [JsonPropertyName("text")]
        public string? Text { get; set; }

        [JsonPropertyName("product_items")]
        public List<ProductItemRequest>? ProductItems { get; set; }
    }

    public class ProductItemRequest
    {
        [JsonPropertyName("product_retailer_id")]
        public string? ProductRetailerId { get; set; }

        [JsonPropertyName("quantity")]
        public string? Quantity { get; set; }

        [JsonPropertyName("item_price")]
        public string? ItemPrice { get; set; }

        [JsonPropertyName("currency")]
        public string? Currency { get; set; }
    }

    public class ReferralContentRequest
    {
        [JsonPropertyName("source_url")]
        public string? SourceUrl { get; set; }

        [JsonPropertyName("source_type")]
        public string? SourceType { get; set; }

        [JsonPropertyName("source_id")]
        public string? SourceId { get; set; }

        [JsonPropertyName("headline")]
        public string? Headline { get; set; }

        [JsonPropertyName("body")]
        public string? Body { get; set; }

        [JsonPropertyName("media_type")]
        public string? MediaType { get; set; }

        [JsonPropertyName("image_url")]
        public string? ImageUrl { get; set; }

        [JsonPropertyName("video_url")]
        public string? VideoUrl { get; set; }

        [JsonPropertyName("thumbnail_url")]
        public string? ThumbnailUrl { get; set; }

        [JsonPropertyName("ctwa_clid")]
        public string? CtwaClid { get; set; }
    }

    public class StickerContentRequest
    {
        [JsonPropertyName("mime_type")]
        public string? MimeType { get; set; }

        [JsonPropertyName("sha256")]
        public string? Sha256 { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("animated")]
        public bool? Animated { get; set; }
    }

    public class SystemContentRequest
    {
        [JsonPropertyName("body")]
        public string? Body { get; set; }

        [JsonPropertyName("identity")]
        public string? Identity { get; set; }

        [JsonPropertyName("new_wa_id")]
        public string? NewWaId { get; set; }

        [JsonPropertyName("wa_id")]
        public string? WaId { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("customer")]
        public string? Customer { get; set; }
    }

    public class VideoContentRequest
    {
        [JsonPropertyName("caption")]
        public string? Caption { get; set; }

        [JsonPropertyName("sha256")]
        public string? Sha256 { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("mime_type")]
        public string? MimeType { get; set; }
    }
}
