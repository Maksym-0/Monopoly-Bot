using Microsoft.EntityFrameworkCore;
using MonopolyBot.Core.Models.Bot;
using MonopolyBot.DataAccess.Postgres.Configurations;

namespace MonopolyBot.DataAccess.Postgres
{
    public class BotDbContext : DbContext
    {
        public BotDbContext(DbContextOptions<BotDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<ChatStatus> ChatStatuses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ChatStatusConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }
    }
}
