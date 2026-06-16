using Microsoft.EntityFrameworkCore;
using MonopolyBot.Core.Interfaces.DataBase.Repository;
using MonopolyBot.Core.Models.Bot;

namespace MonopolyBot.DataAccess.Postgres.Repositories
{
    public class ChatStatusRepository : IChatStatusRepository
    {
        private readonly BotDbContext _context;

        public ChatStatusRepository(BotDbContext context)
        {
            _context = context;
        }

        public async Task<ChatStatus?> GetByChatIdAsync(long chatId)
        {
            return await _context.ChatStatuses
                .FirstOrDefaultAsync(cs => cs.ChatId == chatId);
        }

        public async Task AddAsync(ChatStatus chatStatus)
        {
            await _context.ChatStatuses.AddAsync(chatStatus);
        }
        
        public async Task UpdateAsync(ChatStatus chatStatus)
        {
            _context.ChatStatuses.Update(chatStatus);
        }

        public async Task DeleteByChatIdAsync(long chatId)
        {
            ChatStatus? status = await GetByChatIdAsync(chatId);

            if(status != null)
            {
                _context.ChatStatuses.Remove(status);
            }
        }
    }
}
