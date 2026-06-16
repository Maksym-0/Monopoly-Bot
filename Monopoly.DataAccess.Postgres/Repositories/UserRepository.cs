using Microsoft.EntityFrameworkCore;
using MonopolyBot.Core.Interfaces.DataBase.Repository;
using MonopolyBot.Core.Models.Bot;

namespace MonopolyBot.DataAccess.Postgres.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly BotDbContext _context;

        public UserRepository(BotDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByChatIdAsync(long chatId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.ChatId == chatId);
        }
        public async Task<User?> GetByUserIdAsync(Guid userId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
        public async Task<List<User>?> GetListByRoomIdAsync(Guid roomId)
        {
            return await _context.Users
                .Where(u => u.RoomId == roomId)
                .ToListAsync();
        }
        public async Task<List<User>?> GetListByGameIdAsync(Guid gameId)
        {
            return await _context.Users
                .Where(u => u.GameId == gameId)
                .ToListAsync();
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
        }

        public async Task DeleteByChatIdAsync(long chatId)
        {
            User? user = await GetByChatIdAsync(chatId);
            
            if (user != null)
            {
                _context.Users.Remove(user);
            }
        }
        public async Task DeleteByUserIdAsync(Guid userId)
        {
            User? user = await GetByUserIdAsync(userId);

            if (user != null)
            {
                _context.Users.Remove(user);
            }
        }
    }
}
