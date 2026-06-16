using MonopolyBot.Core.Models.Bot;
using Telegram.Bot.Types;

namespace MonopolyBot.Telegram.Interfaces.Status
{
    internal interface ITradeStatusHandler
    {
        Task HandleAwaitingOffereeAsync(Message message);
        Task HandleGiveMoneyAsync(Message message, ChatStatus chatStatus);
        Task HandleGiveCellsAsync(Message message, ChatStatus chatStatus);
        Task HandleWantedMoneyAsync(Message message, ChatStatus chatStatus);
        Task HandleWantedCellsAsync(Message message, ChatStatus chatStatus);
        Task HandleAwaitingConfirmationAsync(Message message);
    }
}
