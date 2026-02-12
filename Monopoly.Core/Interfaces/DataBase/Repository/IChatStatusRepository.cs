using MonopolyBot.Core.Models.Bot;

namespace MonopolyBot.Core.Interfaces.DataBase.Repository
{
    public interface IChatStatusRepository
    {
        Task<ChatStatus?> GetByChatId(long chatId);
        Task Add(ChatStatus chatStatus);
        Task Update(ChatStatus chatStatus);
        Task DeleteByChatId(long chatId);
    }
}
