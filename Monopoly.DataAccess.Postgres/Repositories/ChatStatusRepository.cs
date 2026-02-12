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

        public async Task<ChatStatus?> GetByChatId(long chatId)
        {
            return await _context.ChatStatuses
                .FirstOrDefaultAsync(cs => cs.ChatId == chatId);
        }

        public async Task Add(ChatStatus chatStatus)
        {
            await _context.ChatStatuses.AddAsync(chatStatus);
        }
        
        public async Task Update(ChatStatus chatStatus)
        {
            _context.ChatStatuses.Update(chatStatus);
        }

        public async Task DeleteByChatId(long chatId)
        {
            ChatStatus? status = await GetByChatId(chatId);

            if(status != null)
            {
                _context.ChatStatuses.Remove(status);
            }
        }
    }
}
