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

        public Guid? TradeOffereeId { get; set; }
        public string? TradeOffereeName { get; set; }
        public int? TradeGiveMoney { get; set; }
        public List<int>? TradeGiveCells { get; set; }
        public int? TradeWantedMoney { get; set; }
        public List<int>? TradeWantedCells { get; set; }

        public ChatStatus(long chatId, BotState botState)
        {
            ChatId = chatId;
            Status = botState;
        }
        public ChatStatus() { }
    }
}
