using Chatbot.Models;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Chatbot.Services
{
    public class ConversationStateService
    {
        private readonly IDistributedCache _cache;

        public ConversationStateService(IDistributedCache cache)
        {
            _cache = cache;
        }

        // Obtener el estado de la conversación
        public async Task<ConversationState> GetConversationStateAsync(string phoneNumber)
        {
            var cachedState = await _cache.GetStringAsync(phoneNumber);

            if (cachedState == null)
            {
                return null; // No hay estado guardado
            }

            return JsonSerializer.Deserialize<ConversationState>(cachedState);
        }

        // Guardar el estado de la conversación
        public async Task SaveConversationStateAsync(string phoneNumber, ConversationState state)
        {
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) // El estado expira en 30 minutos
            };

            var serializedState = JsonSerializer.Serialize(state);
            await _cache.SetStringAsync(phoneNumber, serializedState, cacheOptions);
        }

        // Eliminar el estado de la conversación
        public async Task RemoveConversationStateAsync(string phoneNumber)
        {
            await _cache.RemoveAsync(phoneNumber);
        }
    }
}
