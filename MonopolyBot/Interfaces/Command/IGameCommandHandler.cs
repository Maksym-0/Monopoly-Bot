using Telegram.Bot.Types;

namespace MonopolyBot.Telegram.Interfaces.Command
{
    internal interface IGameCommandHandler
    {
        Task HandleAllGameStatusAsync(Message message);
        Task HandleGameStatusAsync(Message message);
        Task HandleRollDicesAsync(Message message);
        Task HandleBuyAsync(Message message);
        Task HandlePayRentAsync(Message message);
        Task HandlePayToLeavePrisonAsync(Message message);
        Task HandleLevelUpAsync(Message message);
        Task HandleLevelDownAsync(Message message);
        Task HandleEndActionAsync(Message message);
        Task HandleLeaveGameAsync(Message message);
        Task HandleEndWatchGameAsync(Message message);
    }
}
