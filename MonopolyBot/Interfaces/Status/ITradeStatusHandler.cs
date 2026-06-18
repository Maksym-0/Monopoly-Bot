using MonopolyBot.Core.Models.Bot;
using Telegram.Bot.Types;

namespace MonopolyBot.Telegram.Interfaces.Status
{
    internal interface ITradeStatusHandler
    {
        Task HandleInvalidOffereeInputAsync(Message message);
        Task HandleGiveMoneyAsync(Message message, ChatStatus chatStatus);
        Task HandleGiveCellsAsync(Message message, ChatStatus chatStatus);
        Task HandleWantedMoneyAsync(Message message, ChatStatus chatStatus);
        Task HandleWantedCellsAsync(Message message, ChatStatus chatStatus);
        Task HandleInvalidConfirmationInputAsync(Message message);
    }
}
