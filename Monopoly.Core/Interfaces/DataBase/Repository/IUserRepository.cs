using MonopolyBot.Core.Models.Bot;

namespace MonopolyBot.Core.Interfaces.DataBase.Repository
{
    public interface IUserRepository
    {
        Task<User?> GetByUserIdAsync(Guid userId);
        Task<User?> GetByChatIdAsync(long chatId);
        Task<List<User>?> GetListByRoomIdAsync(Guid roomId);
        Task<List<User>?> GetListByGameIdAsync(Guid gameId);

        Task AddAsync(User user);

        Task UpdateAsync(User user);

        Task DeleteByUserIdAsync(Guid userId);
        Task DeleteByChatIdAsync(long chatId);
    }
}
