using MonopolyBot.Core.Models.Bot;
using Telegram.Bot.Types;

namespace MonopolyBot.Telegram.Interfaces.Status
{
    internal interface IGameStatusHandler
    {
        Task HandleLevelUpAsync(Message message, ChatStatus status);
        Task HandleLevelDownAsync(Message message, ChatStatus status);
    }
}
