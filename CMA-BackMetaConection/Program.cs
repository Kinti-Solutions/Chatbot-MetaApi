using Chatbot.Filters;
using Chatbot.Models;
using Chatbot.Services;
using ServiciosExternos.Controllers;
using ServiciosExternos.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("texts.json", optional: false, reloadOnChange: true);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.Configure<WhatsAppApiSettings>(builder.Configuration.GetSection("WhatsAppApi"));
builder.Services.Configure<GlobalConfigurations>(builder.Configuration.GetSection("GlobalConfigurations"));

builder.Services.Configure<ChatbotTexts>(builder.Configuration.GetSection("ChatbotTexts"));
builder.Services.AddTransient<ValidacionService>();
builder.Services.AddTransient<WhatsAppService>();
builder.Services.AddTransient<EncriptDecript>();
builder.Services.AddSingleton<HttpClient>();

// Configuración del caché distribuido en memoria
builder.Services.AddDistributedMemoryCache();
builder.Services.AddTransient<ConversationStateService>();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>(); // Añade el filtro global de excepciones
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
