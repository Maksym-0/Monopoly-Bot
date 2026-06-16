using MonopolyBot.Core.Models.Bot;

namespace MonopolyBot.Core.Interfaces.DataBase.Repository
{
    public interface IChatStatusRepository
    {
        Task<ChatStatus?> GetByChatIdAsync(long chatId);
        Task AddAsync(ChatStatus chatStatus);
        Task UpdateAsync(ChatStatus chatStatus);
        Task DeleteByChatIdAsync(long chatId);
    }
}
