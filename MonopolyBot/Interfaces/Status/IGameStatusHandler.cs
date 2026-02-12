using MonopolyBot.Core.Models.Bot;
using Telegram.Bot.Types;

namespace MonopolyBot.Telegram.Interfaces.Status
{
    internal interface IGameStatusHandler
    {
        Task HandleLevelUpStatus(Message message, ChatStatus status);
        Task HandleLevelDownStatus(Message message, ChatStatus status);
    }
}
