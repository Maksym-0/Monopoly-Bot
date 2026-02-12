using MonopolyBot.Core.Enums;

namespace MonopolyBot.Core.Models.Bot
{
    public class ChatStatus
    {
        public long ChatId { get; set; }

        public BotState Status { get; set; } = BotState.None;

        public string? AccountName { get; set; } = null;
        public Guid? RoomId { get; set; } = null;
        public int? MaxNumberOfPlayers { get; set; } = null;

        public ChatStatus(long chatId, BotState botState)
        {
            ChatId = chatId;
            Status = botState;
        }
        public ChatStatus() { }
    }
}
