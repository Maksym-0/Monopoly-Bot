using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MonopolyBot.DataAccess.Postgres;

namespace MonopolyBot.Telegram
{
    public class BotDbContextFactory : IDesignTimeDbContextFactory<BotDbContext>
    {
        public BotDbContext CreateDbContext(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddUserSecrets<Program>()
                .AddEnvironmentVariables();

            var configuration = builder.Build();

            var connectionString = configuration["DbConnectionStrings:DefaultConnection"];

            var optionsBuilder = new DbContextOptionsBuilder<BotDbContext>();

            optionsBuilder.UseNpgsql(connectionString, b =>
            {
                b.MigrationsAssembly("MonopolyBot.DataAccess.Postgres");
            });

            return new BotDbContext(optionsBuilder.Options);
        }
    }
}
