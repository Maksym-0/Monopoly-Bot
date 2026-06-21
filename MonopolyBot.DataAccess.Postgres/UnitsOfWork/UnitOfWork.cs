using MonopolyBot.Core.Interfaces.DataBase.Repository;
using MonopolyBot.Core.Interfaces.DataBase.UnitOfWork;

namespace MonopolyBot.DataAccess.Postgres.UnitsOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BotDbContext _context;

        public UnitOfWork(BotDbContext context,
            IChatStatusRepository chatStatusRepository,
            IUserRepository userRepository)
        {
            _context = context;

            ChatStatuses = chatStatusRepository;
            Users = userRepository;
        }

        public IChatStatusRepository ChatStatuses { get; }
        public IUserRepository Users { get; }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
