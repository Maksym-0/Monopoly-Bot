using Telegram.Bot.Types;

namespace MonopolyBot.Telegram.Interfaces.Command
{
    internal interface IGameCommandHandler
    {
        Task HandleAllGameStatus(Message message);
        Task HandleGameStatus(Message message);
        Task HandleRollDices(Message message);
        Task HandleBuy(Message message);
        Task HandlePayRent(Message message);
        Task HandlePayToLeavePrison(Message message);
        Task HandleLevelUp(Message message);
        Task HandleLevelDown(Message message);
        Task HandleEndAction(Message message);
        Task HandleLeaveGame(Message message);
        Task HandleEndWatchGame(Message message);
    }
}
