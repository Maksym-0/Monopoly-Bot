using MonopolyBot.Core.Enums;
using MonopolyBot.Core.Models.Bot;

namespace MonopolyBot.Core.Interfaces.Services
{
    public interface IContextService
    {
        Task<ChatStatus?> GetStatusAsync(long chatId);

        Task SetStateAsync(long chatId, BotState state);

        Task UpdateContextDataAsync(ChatStatus chatStatus);

        Task ClearContextAsync(long chatId);
        Task DeleteContextAsync(long chatId);
    }
}
