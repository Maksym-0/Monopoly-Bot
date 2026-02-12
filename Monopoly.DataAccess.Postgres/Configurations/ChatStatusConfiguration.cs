using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MonopolyBot.Core.Models.Bot;

namespace MonopolyBot.DataAccess.Postgres.Configurations
{
    internal class ChatStatusConfiguration : IEntityTypeConfiguration<ChatStatus>
    {
        public void Configure(EntityTypeBuilder<ChatStatus> builder)
        {
            builder.HasKey(cs => cs.ChatId);
        }
    }
}
