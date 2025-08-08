using Chatbot.Models;
using Microsoft.AspNetCore.Mvc;
using Chatbot.Services;
using System.Text;
using System.Text.Json;

namespace Chatbot.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WhatsappController : ControllerBase
    {
        private readonly WhatsAppService _whatsAppService;

        public WhatsappController(WhatsAppService whatsAppService)
        {
            _whatsAppService = whatsAppService;
        }

        // Endpoint para la validación del webhook
        [HttpGet("webhooks")]
        public IActionResult Verify([FromQuery(Name = "hub.mode")] string mode,
                                [FromQuery(Name = "hub.verify_token")] string token,
                                [FromQuery(Name = "hub.challenge")] string challenge)
        {
            var verifyToken = _whatsAppService.GetVerifyToken();

            if (mode == "subscribe" && token == verifyToken)
            {
                return Ok(challenge);
            }

            return Forbid();
        }

        [HttpPost("webhooks")]
        public async Task<IActionResult> ReceiveMessage([FromBody] WhatsAppWebhookRequest request)
        {
            var message = request.Entry?.FirstOrDefault()?.Changes?.FirstOrDefault()?.Value?.Messages?.FirstOrDefault();

            //Escribir en el path local
            await _whatsAppService.WriteOnFile(message.From, request);

            if (message?.Type == "text")
            {

                await _whatsAppService.reciveMessage(message.From, message.Text.Body);

            }
            else if (message?.Type == "interactive")
            {
                if (message.Interactive.ListReply != null)
                    await _whatsAppService.reciveMessage(message.From, message.Interactive.ListReply.Title);
                else if (message.Interactive.ButtonReply != null)
                    await _whatsAppService.reciveMessage(message.From, message.Interactive.ButtonReply.Title);
                else
                    await _whatsAppService.SendComprehensionErrorMessageAsync(message.From);
            }
            else
            {
                await _whatsAppService.SendComprehensionErrorMessageAsync(message.From);
            }
            return Ok();
        }


        [HttpGet("Saludo")]
        public string Saludo()
        {
            return "Hola";
        }

    }
}
