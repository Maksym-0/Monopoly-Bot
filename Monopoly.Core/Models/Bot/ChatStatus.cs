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

        public bool IsAwaitingState()
        {
            return Status == BotState.AwaitingLogin ||
                Status == BotState.AwaitingRegister ||
                Status == BotState.AwaitingDeleteAccount ||
                Status == BotState.AwaitingCreateRoom ||
                Status == BotState.AwaitingCreateRoomPassword ||
                Status == BotState.AwaitingJoinRoom ||
                Status == BotState.AwaitingLevelUpCell ||
                Status == BotState.AwaitingLevelDownCell ||
                Status == BotState.AwaitingOfferee ||
                Status == BotState.AwaitingGiveMoney ||
                Status == BotState.AwaitingGiveCells ||
                Status == BotState.AwaitingWantedMoney ||
                Status == BotState.AwaitingWantedCells ||
                Status == BotState.AwaitingConfirmation;
        }
        public bool IsTradeInProgress()
        {
            return Status == BotState.AwaitingOfferee ||
                   Status == BotState.AwaitingGiveMoney ||
                   Status == BotState.AwaitingGiveCells ||
                   Status == BotState.AwaitingWantedMoney ||
                   Status == BotState.AwaitingWantedCells ||
                   Status == BotState.AwaitingConfirmation;
        }
        public void ClearTrade()
        {
            Status = BotState.InGame;
            TradeOffereeId = null;
            TradeOffereeName = null;
            TradeGiveMoney = null;
            TradeGiveCells = null;
            TradeWantedMoney = null;
            TradeWantedCells = null;
        }
    }
}
