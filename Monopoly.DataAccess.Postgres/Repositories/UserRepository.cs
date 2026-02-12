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

        public async Task<User?> GetByChatId(long chatId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.ChatId == chatId);
        }
        public async Task<User?> GetByUserId(Guid userId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
        public async Task<List<User>?> GetListByRoomId(Guid roomId)
        {
            return await _context.Users
                .Where(u => u.RoomId == roomId)
                .ToListAsync();
        }
        public async Task<List<User>?> GetListByGameId(Guid gameId)
        {
            return await _context.Users
                .Where(u => u.GameId == gameId)
                .ToListAsync();
        }

        public async Task Add(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task Update(User user)
        {
            _context.Users.Update(user);
        }

        public async Task DeleteByChatId(long chatId)
        {
            User? user = await GetByChatId(chatId);
            
            if (user != null)
            {
                _context.Users.Remove(user);
            }
        }
        public async Task DeleteByUserId(Guid userId)
        {
            User? user = await GetByUserId(userId);

            if (user != null)
            {
                _context.Users.Remove(user);
            }
        }
    }
}
