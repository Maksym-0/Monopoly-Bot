namespace MonopolyBot.Core.Models.Bot
{
    public class User
    {
        public Guid Id { get; set; }
        public long ChatId { get; set; }

        public Guid AccountId { get; set; }

        public Guid? RoomId { get; set; }
        public Guid? GameId { get; set; }

        public string Name { get; set; }

        public string JWT { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
