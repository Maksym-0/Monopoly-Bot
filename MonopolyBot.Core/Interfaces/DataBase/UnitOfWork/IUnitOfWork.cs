using MonopolyBot.Core.Interfaces.DataBase.Repository;

namespace MonopolyBot.Core.Interfaces.DataBase.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IChatStatusRepository ChatStatuses { get; }
        IUserRepository Users { get; }

        Task<int> SaveChangesAsync();
    }
}
