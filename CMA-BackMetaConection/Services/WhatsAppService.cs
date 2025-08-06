using Chatbot.Models;
using Chatbot.Utils;
using Microsoft.Extensions.Options;
using ServiciosExternos.Controllers;
using ServiciosExternos.Models;
using System.Net.Http.Headers;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using static Chatbot.Models.InteractiveContact;
using static Chatbot.Models.InteractiveLocation;
using static Chatbot.Models.InteractiveSticker;
using static Chatbot.Models.InteractiveTemplate;

namespace Chatbot.Services
{
    public class WhatsAppService
    {
        #region Declaraciones
        private readonly WhatsAppApiSettings _settings;
        private readonly HttpClient _httpClient;
        private readonly ValidacionService _validacionService;
        private readonly ConversationStateService _conversationStateService;
        private readonly ChatbotTexts _chatbotTexts;
        private readonly GlobalConfigurations _globalConfigurations;
        private readonly ILogger<WhatsAppService> _logger;
        private readonly EncriptDecript _encriptDecript;
        #endregion

        #region Constructor
        public WhatsAppService(IOptions<WhatsAppApiSettings> settings,
                                HttpClient httpClient,
                                ValidacionService validacionService,
                                ConversationStateService conversationStateService,
                                IOptions<ChatbotTexts> chatbotTexts,
                                IOptions<GlobalConfigurations> globalConfigurations,
                                EncriptDecript encriptDecript,
                                ILogger<WhatsAppService> logger)
        {
            _settings = settings.Value;
            _httpClient = httpClient;
            _validacionService = validacionService;
            _conversationStateService = conversationStateService;
            _chatbotTexts = chatbotTexts.Value;
            _logger = logger;
            _globalConfigurations = globalConfigurations.Value;
            _encriptDecript = encriptDecript;
        }
        #endregion

        /// <summary>
        /// Retorna el token de verificación
        /// </summary>
        /// <returns></returns>
        public string GetVerifyToken()
        {
            return _settings.WebhookVerifyToken;
        }

        public async Task MarkMessageAsReadAsync(string messageId)
        {
            var url = $"{_settings.UrlApi}{_settings.BusinessPhoneNumberId}/messages";
            var data = new
            {
                messaging_product = "whatsapp",
                status = "read",
                to = messageId
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.GraphApiToken);
            requestMessage.Content = new StringContent(JsonSerializer.Serialize(data), System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();
        }



        // Método para recibir el mensaje y tomar una decisión basada en el estado de la conversación.
        public async Task reciveMessage(string from, string messageBody)
        {
            try
            {
                messageBody = await SanitizeMessageBody(from, messageBody);
                // se comprueba si no es asesor
                //var asesores = await GetAsesoresAsync(new EncriptedData { data = "", key = "" });

                if (messageBody == "error")
                {
                    await SendTextMessageAsync(from, _chatbotTexts.SelectionError);
                    return;
                }

                //if (!asesores.Any(a => a.Celular == from))
                if(true==true)
                {
                    // no es un asesor de credito
                    var conversationState = await _conversationStateService.GetConversationStateAsync(from);

                    if (conversationState == null)
                    {
                        await SendTextMessageAsync(from, _chatbotTexts.WelcomeMessage);
                        await Task.Delay(500);
                        await SendTextMessageAsync(from, _chatbotTexts.ReminderMessage);
                        await Task.Delay(500);
                        await SendListaServiciosAsync(from);

                        conversationState = new ConversationState { State = "servicio" };
                        await _conversationStateService.SaveConversationStateAsync(from, conversationState);
                        return;
                    }

                    var textoUsuario = messageBody.Trim().ToLower();

                    switch (conversationState.State)
                    {
                        case "servicio":
                            await HandleServicio(from, textoUsuario);
                            break;

                        case "cedula_ruc":
                            await HandleCedulaRuc(from, textoUsuario);
                            break;

                        case "cuentas":
                            await HandleCuentas(from, textoUsuario);
                            break;

                        case "autorizacion":
                            await HandleAutorizacion(from, textoUsuario);
                            break;

                        case "autorizado":
                            await HandleDestinoCredito(from, textoUsuario);
                            break;

                        case "destinoCred":
                            await HandleMontoCredito(from, textoUsuario);
                            break;

                        case "inversion":
                            await HandleInversionAutorizacion(from, textoUsuario);
                            break;

                        case "continuar":
                            await HandleContinuarConversación(from, textoUsuario);
                            break;

                        default:
                            await SendTextMessageAsync(from, _chatbotTexts.SelectionError);
                            break;
                    }
                }
                else
                {
                    // es un asesor, solo valida las cédulas
                    var cedula = messageBody.Trim().ToLower();

                    await HandleActualizarSeguimiento(from, cedula);
                }
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Failed to send message through WhatsApp API.");
                throw;
            }
        }


        // Enviar una lista de servicios
        private async Task SendListaServiciosAsync(string to, string errorMessage = null)
        {
            var sections = new List<Section>
            {
                new Section
                {
                    Title = _chatbotTexts.ServicesButtonListHeader,
                    Rows = new List<Row>
                    {
                        new Row { Id = "1_cred", Title = _chatbotTexts.ServicesCreditListHeader, Description = _chatbotTexts.ServicesCreditListDescription },
                        new Row { Id = "1_aper", Title = _chatbotTexts.ServicesAccountListHeader, Description = _chatbotTexts.ServicesAccountListDescription },
                        new Row { Id = "1_inve", Title = _chatbotTexts.ServicesInvestmentsListHeader, Description = _chatbotTexts.ServicesInvestmentsListDescription }
                    }
                },
                new Section
                {
                    Title = _chatbotTexts.HelpListHeader,
                    Rows = new List<Row>
                    {
                        new Row { Id = "1_agen", Title = _chatbotTexts.HelpAgenciesListHeader, Description = _chatbotTexts.HelpAgenciesListDescription},
                        //new Row { Id = "1_mov", Title = _chatbotTexts.HelpKintiListHeader, Description = _chatbotTexts.HelpProvimovilListDescription }
                    }
                }
            };

            var listaServicios = new WhatsAppMessage<InteractiveContent>(to, "interactive", new InteractiveContent
            {
                Type = "list",
                Header = new InteractiveHeader { Type = "text", Text = _chatbotTexts.ServicesListHeader },
                Body = new InteractiveBody { Text = _chatbotTexts.ServicesListBody },
                Footer = new InteractiveFooter { Text = _chatbotTexts.ServicesListFooter },
                Action = new InteractiveAction { Button = _chatbotTexts.ServicesListButton, Sections = sections }
            });

            if (!string.IsNullOrEmpty(errorMessage))
            {
                await SendTextMessageAsync(to, _chatbotTexts.ServicesError);
            }
            await SendMessageAsync(listaServicios);
        }

        private async Task SendErrorAsync(string to, string text)
        {
            var stickerMessage = new WhatsAppMessage<StickerContent>(to, "sticker", new StickerContent
            {
                Id = _chatbotTexts.StickerFixingIssueID
            });

            await SendMessageAsync(stickerMessage);

            await SendTextMessageAsync(to, text);
        }

        private async Task SendUbicacionesAsync(string to)
        {
            await SendTextMessageAsync(to, _chatbotTexts.LocationHeader);

            var ubicacionMatrizMessage = new WhatsAppMessage<LocationContent>(to, "location", new LocationContent
            {
                Latitude = _chatbotTexts.LocationMatrizLat,
                Longitude = _chatbotTexts.LocationMatrizLong,
                Name = _chatbotTexts.LocationMatrizName,
                Address = _chatbotTexts.LocationMatrizAdress
            });

            await SendMessageAsync(ubicacionMatrizMessage);

            var ubicacionPatamarcaMessage = new WhatsAppMessage<LocationContent>(to, "location", new LocationContent
            {
                Latitude = _chatbotTexts.LocationPatLat,
                Longitude = _chatbotTexts.LocationPatLong,
                Name = _chatbotTexts.LocationPatName,
                Address = _chatbotTexts.LocationPatAdress
            });

            await SendMessageAsync(ubicacionPatamarcaMessage);

        }


        // Enviar opciones de autorización
        private async Task SendAutorizacionConsultaAsync(string to, string errorMessage = null)
        {
            var buttons = new List<Button>
            {
                new Button { Reply = new Reply { Id = "1", Title = "Sí" } },
                new Button { Reply = new Reply { Id = "2", Title = "No" } }
            };

            var buttonMessage = new WhatsAppMessage<InteractiveButtonContent>(to, "interactive", new InteractiveButtonContent
            {
                Type = "button",
                Body = new InteractiveBody { Text = _chatbotTexts.AuthorizationPrompt },
                Action = new ButtonAction { Buttons = buttons }
            });

            if (!string.IsNullOrEmpty(errorMessage))
            {
                await SendTextMessageAsync(to, errorMessage);
            }
            await SendMessageAsync(buttonMessage);
        }


        // Enviar opciones de autorización
        private async Task SendInversionConsultaAsync(string to, string errorMessage = null)
        {
            var buttons = new List<Button>
            {
                new Button { Reply = new Reply { Id = "1", Title = "Sí" } },
                new Button { Reply = new Reply { Id = "2", Title = "No" } }
            };

            var buttonMessage = new WhatsAppMessage<InteractiveButtonContent>(to, "interactive", new InteractiveButtonContent
            {
                Type = "button",
                Body = new InteractiveBody { Text = _chatbotTexts.InvestingQuestion },
                Action = new ButtonAction { Buttons = buttons }
            });

            if (!string.IsNullOrEmpty(errorMessage))
            {
                await SendTextMessageAsync(to, errorMessage);
            }
            await SendMessageAsync(buttonMessage);
        }



        // Enviar opciones de continuar con el servicio
        private async Task SendContinuarConsultaAsync(string to, string errorMessage = null)
        {
            var buttons = new List<Button>
            {
                new Button { Reply = new Reply { Id = "1", Title = "Sí" } },
                new Button { Reply = new Reply { Id = "2", Title = "No" } }
            };

            var buttonMessage = new WhatsAppMessage<InteractiveButtonContent>(to, "interactive", new InteractiveButtonContent
            {
                Type = "button",
                Body = new InteractiveBody { Text = _chatbotTexts.ContinueQuestion },
                Action = new ButtonAction { Buttons = buttons }
            });

            if (!string.IsNullOrEmpty(errorMessage))
            {
                await SendTextMessageAsync(to, errorMessage);
            }
            await SendMessageAsync(buttonMessage);
        }

        // Enviar una lista de cuentas
        private async Task SendListaCuentasAsync(string to)
        {
            var sections = new List<Section>
            {
                new Section
                {
                    Title = _chatbotTexts.AccountsButtonListHeader,
                    Rows = new List<Row>
                    {
                        new Row { Id = "1_ahorros", Title = _chatbotTexts.AccountsSavingsListHeader, Description = _chatbotTexts.AccountsSavingsListDescription },
                        new Row { Id = "1_infantil", Title = _chatbotTexts.AccountsKidsListHeader, Description = _chatbotTexts.AccountsKidsListDescription }
                    }
                }
            };

            var listaCuentas = new WhatsAppMessage<InteractiveContent>(to, "interactive", new InteractiveContent
            {
                Type = "list",
                Header = new InteractiveHeader { Type = "text", Text = _chatbotTexts.AccountsListHeader },
                Body = new InteractiveBody { Text = _chatbotTexts.AccountsListBody },
                Footer = new InteractiveFooter { Text = _chatbotTexts.AccountsListFooter },
                Action = new InteractiveAction { Button = _chatbotTexts.AccountsListButton, Sections = sections }
            });

            await SendMessageAsync(listaCuentas);
        }

        private async Task SendListaCuentasAsync(string to, string error)
        {
            await SendTextMessageAsync(to, error);
            await Task.Delay(1000);

            var sections = new List<Section>
            {
                new Section
                {
                    Title = "Cuentas",
                    Rows = new List<Row>
                    {
                        new Row { Id = "1_ahorros", Title = _chatbotTexts.AccountsSavingsListHeader, Description = _chatbotTexts.AccountsSavingsListDescription },
                        new Row { Id = "1_infantil", Title = _chatbotTexts.AccountsKidsListHeader, Description = _chatbotTexts.AccountsKidsListDescription }
                    }
                }
            };

            var listaCuentas = new WhatsAppMessage<InteractiveContent>(to, "interactive", new InteractiveContent
            {
                Type = "list",
                Header = new InteractiveHeader { Type = "text", Text = _chatbotTexts.AccountsListHeader },
                Body = new InteractiveBody { Text = _chatbotTexts.AccountsListBody },
                Footer = new InteractiveFooter { Text = _chatbotTexts.AccountsListFooter },
                Action = new InteractiveAction { Button = _chatbotTexts.AccountsListButton, Sections = sections }
            });

            await SendMessageAsync(listaCuentas);
        }

        private async Task HandleCuentas(string from, string cuenta)
        {
            if (cuenta.Contains("ahorros vista"))
            {
                // Flujo para Ahorros Vista
                await SendTextMessageAsync(from, _chatbotTexts.SavingsAccountDescription);
                await Task.Delay(500);
                await SendTextMessageAsync(from, _chatbotTexts.SavingsAccountBenefits);
                await Task.Delay(500);
                await SendTextMessageAsync(from, _chatbotTexts.SavingsAccountRequirements);
                await Task.Delay(500);
                await SendTextMessageAsync(from, _chatbotTexts.SavingsAccountFooter);

                await Task.Delay(1000);
                await SendContinuarConsultaAsync(from);

                var conversationState = await _conversationStateService.GetConversationStateAsync(from);

                conversationState.State = "continuar"; // Cambiar al nuevo estado
                await _conversationStateService.SaveConversationStateAsync(from, conversationState);
            }
            else if (cuenta.Contains("ahorro infantil"))
            {
                // Flujo para Ahorro Infantil
                await SendTextMessageAsync(from, _chatbotTexts.KidsAccountDescription);
                await Task.Delay(500);
                await SendTextMessageAsync(from, _chatbotTexts.KidsAccountBenefits);
                await Task.Delay(500);
                await SendTextMessageAsync(from, _chatbotTexts.KidsAccountRequirements);
                await Task.Delay(500);
                await SendTextMessageAsync(from, _chatbotTexts.KidsAccountFooter);

                await Task.Delay(1000);
                await SendContinuarConsultaAsync(from);

                var conversationState = await _conversationStateService.GetConversationStateAsync(from);

                conversationState.State = "continuar"; // Cambiar al nuevo estado
                await _conversationStateService.SaveConversationStateAsync(from, conversationState);
            }
            else
            {
                // Si la opción no es válida, enviar nuevamente la lista de cuentas
                await SendListaCuentasAsync(from, _chatbotTexts.AccountsChoosenError);
            }
        }

        // Método para enviar mensajes de texto
        private async Task SendTextMessageAsync(string to, string body)
        {
            var textMessage = new WhatsAppMessage<TextContent>(to, "text", new TextContent { Body = body, PreviewUrl = false });
            await SendMessageAsync(textMessage);
        }

        public async Task SendComprehensionErrorMessageAsync(string to)
        {
            await SendTextMessageAsync(to, _chatbotTexts.ComprehensionErrorResponse);
        }

        // Método genérico para enviar mensajes a través de la API de WhatsApp
        public async Task SendMessageAsync<T>(WhatsAppMessage<T> message)
        {
            var handler = new HttpClientHandler()
            {
                SslProtocols = System.Security.Authentication.SslProtocols.Tls12
            };

            using var httpClient = new HttpClient(handler);

            var url = $"{_settings.UrlApi}{_settings.BusinessPhoneNumberId}/messages";

            var options = new JsonSerializerOptions();
            options.Converters.Add(new DynamicPropertyNameConverter<T>());

            var content = JsonSerializer.Serialize(message, options);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Headers =
                {
                    Authorization = new AuthenticationHeaderValue("Bearer", _settings.GraphApiToken)
                },
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            };

            try
            {
                var response = await httpClient.SendAsync(requestMessage);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                // Captura la excepción y muestra el error exacto
                Console.WriteLine($"Request error: {e.Message}");
            }
        }

        // -------------------------------------------------------
        // HANDLERS
        // -------------------------------------------------------

        // Método para manejar la entrada de cédula o RUC
        private async Task HandleCedulaRuc(string from, string cedulaRuc)
        {
            if (_validacionService.IsValidCedulaRuc(cedulaRuc))
            {
                // Guardar el estado actualizado
                var conversationState = await _conversationStateService.GetConversationStateAsync(from);
                conversationState.CedulaRuc = cedulaRuc;// Cambiar al nuevo estado
                await _conversationStateService.SaveConversationStateAsync(from, conversationState);

                var consulta = new AutorizationRequest
                {
                    identification = conversationState.CedulaRuc
                };

                // Se consulta API para ver si ya ha autorizado:
                var cifrado = _encriptDecript.encryptData(JsonSerializer.Serialize(consulta));
                //var autorizado = await GetAutorizacionAsync(cifrado);

                //if (autorizado.Autorizado)
                //{
                //    conversationState.State = "autorizado"; // Cambiar al nuevo estado
                //    await _conversationStateService.SaveConversationStateAsync(from, conversationState);

                //    await SendTextMessageAsync(from, _chatbotTexts.CreditDestinationPrompt);
                //}
                //else
                //{
                //    conversationState.State = "autorizacion";
                //    await _conversationStateService.SaveConversationStateAsync(from, conversationState);
                //    await SendAutorizacionConsultaAsync(from);
                //}
            }
            else
            {
                await SendTextMessageAsync(from, _chatbotTexts.InvalidId);
                await Task.Delay(500);
                await SendTextMessageAsync(from, _chatbotTexts.IdPrompt);
            }
        }

        // Método para manejar la selección de servicios
        private async Task HandleServicio(string from, string servicio)
        {
            if (servicio.Contains("crédito") || servicio.Contains("credito"))
            {
                // Guardar el estado actualizado
                var conversationState = await _conversationStateService.GetConversationStateAsync(from);

                conversationState.State = "cedula_ruc"; // Cambiar el estado a solicitar cédula o RUC
                await _conversationStateService.SaveConversationStateAsync(from, conversationState);

                await SendTextMessageAsync(from, _chatbotTexts.IdPrompt);
            }
            else if (servicio.Contains("cuenta"))
            {
                // Guardar el estado actualizado
                var conversationState = await _conversationStateService.GetConversationStateAsync(from);

                conversationState.State = "cuentas"; // Cambiar al nuevo estado
                await _conversationStateService.SaveConversationStateAsync(from, conversationState);

                await SendListaCuentasAsync(from);
            }
            else if (servicio.Contains("inversiones") || servicio.Contains("pólizas") || servicio.Contains("polizas"))
            {
                var conversationState = await _conversationStateService.GetConversationStateAsync(from);
                conversationState.State = "inversion";
                await _conversationStateService.SaveConversationStateAsync(from, conversationState);

                SendInvestmentsTexts(from);
            }
            else if (servicio.Contains("agencias") || servicio.Contains("agencia"))
            {
                await SendUbicacionesAsync(from);

                await Task.Delay(1000);
                await SendContinuarConsultaAsync(from);

                var conversationState = await _conversationStateService.GetConversationStateAsync(from);

                conversationState.State = "continuar"; // Cambiar al nuevo estado
                await _conversationStateService.SaveConversationStateAsync(from, conversationState);
            }
            else if (servicio.Contains("provimovil"))
            {
                await SendErrorAsync(from, _chatbotTexts.IssueUs);

                await Task.Delay(1000);
                await SendContinuarConsultaAsync(from);

                var conversationState = await _conversationStateService.GetConversationStateAsync(from);

                conversationState.State = "continuar"; // Cambiar al nuevo estado
                await _conversationStateService.SaveConversationStateAsync(from, conversationState);
            }
            else
            {
                await SendListaServiciosAsync(from, _chatbotTexts.InvalidServiceSelection);
            }
        }

        private async void SendInvestmentsTexts(string from)
        {
            // Flujo para Ahorros Vista
            await SendTextMessageAsync(from, _chatbotTexts.InvestingDescription);
            await Task.Delay(500);
            await SendTextMessageAsync(from, _chatbotTexts.InvestingBenefits);
            await Task.Delay(500);
            await SendInversionConsultaAsync(from);
        }

        // Método para manejar la autorización
        private async Task HandleAutorizacion(string from, string respuesta)
        {
            if (respuesta == "1" || respuesta.Contains("sí"))
            {
                // Guardar el estado actualizado
                var conversationState = await _conversationStateService.GetConversationStateAsync(from);

                conversationState.State = "autorizado"; // Cambiar al nuevo estado
                await _conversationStateService.SaveConversationStateAsync(from, conversationState);

                await SendTextMessageAsync(from, _chatbotTexts.CreditDestinationPrompt);

                var consulta = new AutorizationRequest
                {
                    identification = conversationState.CedulaRuc
                };

                // Se actualiza la aceptación de la política:
                var cifrado = _encriptDecript.encryptData(JsonSerializer.Serialize(consulta));
               // await UpdateAutorizacionAsync(cifrado);
            }
            else if (respuesta == "2" || respuesta.Contains("no"))
            {
                await SendTextMessageAsync(from, _chatbotTexts.NotAuthorized);
                await Task.Delay(1000);
                await SendContinuarConsultaAsync(from);

                var conversationState = await _conversationStateService.GetConversationStateAsync(from);

                conversationState.State = "continuar"; // Cambiar al nuevo estado
                await _conversationStateService.SaveConversationStateAsync(from, conversationState);
            }
            else
            {
                await SendAutorizacionConsultaAsync(from, _chatbotTexts.AuthorizationReminder);
            }
        }


        // Método para manejar la autorización
        private async Task HandleInversionAutorizacion(string from, string respuesta)
        {
            if (respuesta == "1" || respuesta.Contains("sí") || respuesta.Contains("si"))
            {

                //enviar a API
                await SendTextMessageAsync(from, _chatbotTexts.AcceptedInvesting);
                await Task.Delay(1000);
                await SendContinuarConsultaAsync(from);

                var conversationState = await _conversationStateService.GetConversationStateAsync(from);

                conversationState.State = "continuar"; // Cambiar al nuevo estado
                await _conversationStateService.SaveConversationStateAsync(from, conversationState);

                //await SetAsesorInversion(from);
            }
            else if (respuesta == "2" || respuesta.Contains("no"))
            {
                await SendTextMessageAsync(from, _chatbotTexts.NotAcceptedInvesting);
                await Task.Delay(1000);
                await SendContinuarConsultaAsync(from);

                var conversationState = await _conversationStateService.GetConversationStateAsync(from);

                conversationState.State = "continuar"; // Cambiar al nuevo estado
                await _conversationStateService.SaveConversationStateAsync(from, conversationState);
            }
            else
            {
                await SendAutorizacionConsultaAsync(from, _chatbotTexts.AuthorizationReminder);
            }
        }

        // Método para manejar la autorización
        private async Task HandleContinuarConversación(string from, string respuesta)
        {
            if (respuesta == "1" || respuesta.Contains("sí") || respuesta.Contains("si"))
            {

                var conversationState = await _conversationStateService.GetConversationStateAsync(from);

                conversationState.State = "servicio"; // Cambiar al nuevo estado
                await _conversationStateService.SaveConversationStateAsync(from, conversationState);

                await SendListaServiciosAsync(from);
            }
            else if (respuesta == "2" || respuesta.Contains("no"))
            {
                await SendTextMessageAsync(from, _chatbotTexts.ComunicationEnded);
                await _conversationStateService.RemoveConversationStateAsync(from);
            }
            else
            {
                await SendAutorizacionConsultaAsync(from, _chatbotTexts.AuthorizationReminder);
            }
        }

        // Método para manejar el destino del crédito
        private async Task HandleDestinoCredito(string from, string destino)
        {
            if (!string.IsNullOrEmpty(destino))
            {
                // Guardar el estado actualizado
                var conversationState = await _conversationStateService.GetConversationStateAsync(from);

                conversationState.DestinoCredito = destino;
                conversationState.State = "destinoCred"; // Cambiar al nuevo estado
                await _conversationStateService.SaveConversationStateAsync(from, conversationState);

                await SendTextMessageAsync(from, _chatbotTexts.CreditAmountPrompt);
            }
            else
            {
                await SendTextMessageAsync(from, _chatbotTexts.CreditDestinationErrorPrompt);
            }
        }

        // Método para manejar el monto del crédito
        private async Task HandleMontoCredito(string from, string monto)
        {
            if (!string.IsNullOrEmpty(monto) && monto.All(char.IsDigit))
            {

                await SendTextMessageAsync(from, _chatbotTexts.CreditProcesed);

                // se guarda el monto del crédito
                // Guardar el estado actualizado
                var conversationState = await _conversationStateService.GetConversationStateAsync(from);

                conversationState.Monto = monto;
                conversationState.Timestamp = DateTime.UtcNow;

                await _conversationStateService.SaveConversationStateAsync(from, conversationState);
                // Configurar enviar plantilla al asesor con la información

                await Task.Delay(1000);
                await SendContinuarConsultaAsync(from);

                conversationState.State = "continuar"; // Cambiar al nuevo estado
                await _conversationStateService.SaveConversationStateAsync(from, conversationState);

                // enviar a API
                // se consulta a asesores
               // await SetAsesorSolicitudCredito(from, conversationState.CedulaRuc, conversationState.Monto, conversationState.DestinoCredito);

            }
            else
            {
                await SendTextMessageAsync(from, _chatbotTexts.CreditAmountErrorPrompt);
            }
        }

        private async Task HandleActualizarSeguimiento(string from, string cedulaSeguimiento)
        {
            try
            {

                var propuesta = new PropuestaRequest
                {
                    IdentificacionProp = cedulaSeguimiento,
                    Usuario = from
                };

                var cifrado = _encriptDecript.encryptData(JsonSerializer.Serialize(propuesta));

                var url = $"{_globalConfigurations.ApiExternosUrl}/Chatbot/actualizarAsignacionPropuesta";

                // Serializar el objeto EncriptedData
                var content = new StringContent(JsonSerializer.Serialize(cifrado), Encoding.UTF8, "application/json");

                // Crear la solicitud HTTP POST
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = content
                };

                // Enviar la solicitud y obtener la respuesta
                var response = await _httpClient.SendAsync(requestMessage);

                // Verificar si la solicitud fue exitosa
                if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
                {
                    await SendTextMessageAsync(from, _chatbotTexts.UpdateProposal+ cedulaSeguimiento);
                }
                else
                {
                    await SendTextMessageAsync(from, _chatbotTexts.UpdateProposalError);
                }
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error al actualizar la propuesta de credito");
                throw;
            }
        }

        //private async Task<List<Asesor>> GetAsesoresAsync(EncriptedData encriptedData)
        //{
        //    try
        //    {
        //        // Construir la URL del endpoint
        //        var url = $"{_globalConfigurations.ApiExternosUrl}/Chatbot/ObtenerAsesores";

        //        // Serializar el objeto EncriptedData
        //        var content = new StringContent(JsonSerializer.Serialize(encriptedData), Encoding.UTF8, "application/json");

        //        // Crear la solicitud HTTP POST
        //        var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
        //        {
        //            Content = content
        //        };

        //        // Enviar la solicitud y obtener la respuesta
        //        var response = await _httpClient.SendAsync(requestMessage);

        //        // Verificar si la solicitud fue exitosa
        //        response.EnsureSuccessStatusCode();

        //        // Deserializar la respuesta en una lista de Asesores
        //        var responseContent = await response.Content.ReadAsStringAsync();
        //        var jsonresponse = JsonSerializer.Deserialize<EncriptedData>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        //        var retorno = JsonSerializer.Deserialize<List<Asesor>>(jsonresponse.data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        //        return retorno;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error al obtener asesores desde el API externo.");
        //        throw;
        //    }
        //}

        //private async Task<PoliticaProteccionDato> GetAutorizacionAsync(EncriptedData encriptedData)
        //{
        //    try
        //    {
        //        // Construir la URL del endpoint
        //        var url = $"{_globalConfigurations.ApiExternosUrl}/Chatbot/consultarPersona";

        //        // Serializar el objeto EncriptedData
        //        var content = new StringContent(JsonSerializer.Serialize(encriptedData), Encoding.UTF8, "application/json");

        //        // Crear la solicitud HTTP POST
        //        var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
        //        {
        //            Content = content
        //        };

        //        // Enviar la solicitud y obtener la respuesta
        //        var response = await _httpClient.SendAsync(requestMessage);

        //        // Verificar si la solicitud fue exitosa
        //        response.EnsureSuccessStatusCode();

        //        // Deserializar la respuesta en una lista de Asesores
        //        var responseContent = await response.Content.ReadAsStringAsync();
        //        var jsonresponse = JsonSerializer.Deserialize<EncriptedData>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        //        var desencriptado = _encriptDecript.decrypData(jsonresponse);
        //        string normalizedJson = JsonSerializer.Deserialize<string>(desencriptado);
        //        var retorno = JsonSerializer.Deserialize<PoliticaProteccionDato>(
        //                normalizedJson,
        //                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        //            );
        //        return retorno;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error al consultar la autorización de la política de aceptación.");
        //        throw;
        //    }
        //}

        //private async Task UpdateAutorizacionAsync(EncriptedData encriptedData)
        //{
        //    try
        //    {
        //        // Construir la URL del endpoint
        //        var url = $"{_globalConfigurations.ApiExternosUrl}/Chatbot/actualizarPolitica";

        //        // Serializar el objeto EncriptedData
        //        var content = new StringContent(JsonSerializer.Serialize(encriptedData), Encoding.UTF8, "application/json");

        //        // Crear la solicitud HTTP POST
        //        var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
        //        {
        //            Content = content
        //        };

        //        // Enviar la solicitud y obtener la respuesta
        //        var response = await _httpClient.SendAsync(requestMessage);

        //        // Verificar si la solicitud fue exitosa
        //        response.EnsureSuccessStatusCode();

        //        await response.Content.ReadAsStringAsync();

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error al actualizar la política de aceptación de manejo de datos.");
        //        throw;
        //    }
        //}

        //private async Task SetAsesorSolicitudCredito(string from, string identificacion, string monto, string destino)
        //{
        //    try
        //    {
        //        var Asesores = await GetAsesoresAsync(new EncriptedData { data = "", key = "" });

        //        // se elige un asesor de forma aleatoria
        //        Random r = new Random();
        //        int rInt = r.Next(0, Asesores.Count - 1);

        //        Asesor asesor = Asesores[rInt];

        //        var templateMessage = new WhatsAppMessage<TemplateContent>(asesor.Celular, "template", new TemplateContent {
        //            name = "propuesta_credito",
        //            language = new Language
        //            {
        //                code = "es_AR"
        //            },
        //            components = new List<Component> {
        //                new Component
        //                {
        //                    type = "body",
        //                    parameters = new List<Parameter> { 
        //                        new Parameter {
        //                            type = "text",
        //                            text = identificacion
        //                        },
        //                        new Parameter {
        //                            type = "text",
        //                            text = destino
        //                        },
        //                        new Parameter {
        //                            type = "text",
        //                            text = monto
        //                        },
        //                        new Parameter {
        //                            type = "text",
        //                            text = "+"+ from
        //                        }
        //                    }
        //                }
        //            }
        //        });

        //        await SendMessageAsync(templateMessage);

        //        var propuesta = new PropuestaRequest
        //        {
        //            IdentificacionProp = identificacion,
        //            Usuario = asesor.Celular
        //        };

        //        var cifrado = _encriptDecript.encryptData(JsonSerializer.Serialize(propuesta));

        //        // Construir la URL del endpoint
        //        var url = $"{_globalConfigurations.ApiExternosUrl}/Chatbot/crearAsignacionPropuesta";

        //        // Serializar el objeto EncriptedData
        //        var content = new StringContent(JsonSerializer.Serialize(cifrado), Encoding.UTF8, "application/json");

        //        // Crear la solicitud HTTP POST
        //        var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
        //        {
        //            Content = content
        //        };

        //        // Enviar la solicitud y obtener la respuesta
        //        var response = await _httpClient.SendAsync(requestMessage);

        //        // Verificar si la solicitud fue exitosa
        //        response.EnsureSuccessStatusCode();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error al colocar la propuesta de asignación en la base.");
        //        throw;
        //    }
        //}



        //private async Task SetAsesorInversion(string from)
        //{
        //    try
        //    {
        //        var Asesores = await GetAsesoresAsync(new EncriptedData { data = "", key = "" });

        //        // se elige un asesor de forma aleatoria
        //        Random r = new Random();
        //        int rInt = r.Next(0, Asesores.Count - 1);

        //        Asesor asesor = Asesores[rInt];

        //        var templateMessage = new WhatsAppMessage<TemplateContent>(asesor.Celular, "template", new TemplateContent
        //        {
        //            name = "propuesta_inversion",
        //            language = new Language
        //            {
        //                code = "es_AR"
        //            },
        //            components = new List<Component> {
        //                new Component
        //                {
        //                    type = "body",
        //                    parameters = new List<Parameter> {
        //                        new Parameter {
        //                            type = "text",
        //                            text = "+"+ from
        //                        }
        //                    }
        //                }
        //            }
        //        });

        //        await SendMessageAsync(templateMessage);

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error al colocar la propuesta de asignación en la base.");
        //        throw;
        //    }
        //}

        private async Task<string> SanitizeMessageBody(string from, string messageBody)
        {
            char[] invalidChars = { '"', '<', '>', '+', '=', '-', '(', ')', '\'', '&'};

            if (messageBody.IndexOfAny(invalidChars) != -1)
                messageBody = "error";

            return messageBody;
        }

        public async Task WriteOnFile(string from, WhatsAppWebhookRequest request)
        {
            try
            {
                // Validar si la escritura está habilitada (en el app setings ta la validacion)
                if (!_settings.EnableWriteOnFile)
                {
                    return;
                }

                string text = JsonSerializer.Serialize(request);

                string filePath = _settings.WriteOnFile;

                string directoryPath = Path.GetDirectoryName(filePath);

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                await File.AppendAllTextAsync(filePath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - FROM: {from} - TEXT: {text}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al escribir en el archivo: {ex.Message}");
            }
        }

        public async Task WriteOnFileGenerico(object request)
        {
            try
            {
                // Validar si la escritura está habilitada (en el app setings ta la validacion)
                if (!_settings.EnableWriteOnFile)
                {
                    return;
                }

                string text = JsonSerializer.Serialize(request);

                string filePath = _settings.WriteOnFile;

                string directoryPath = Path.GetDirectoryName(filePath);

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                await File.AppendAllTextAsync(filePath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - TEXT: {text}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al escribir en el archivo: {ex.Message}");
            }
        }
    }
}
