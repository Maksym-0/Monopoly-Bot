using MonopolyBot.Core.Models.Bot;

namespace MonopolyBot.Core.Interfaces.DataBase.Repository
{
    public interface IUserRepository
    {
        Task<User?> GetByUserId(Guid userId);
        Task<User?> GetByChatId(long chatId);
        Task<List<User>?> GetListByRoomId(Guid roomId);
        Task<List<User>?> GetListByGameId(Guid gameId);

        Task Add(User user);

        Task Update(User user);

        Task DeleteByUserId(Guid userId);
        Task DeleteByChatId(long chatId);
    }
}
