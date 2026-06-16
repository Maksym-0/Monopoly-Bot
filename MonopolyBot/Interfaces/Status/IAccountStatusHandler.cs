using MonopolyBot.Core.Models.Bot;
using Telegram.Bot.Types;

namespace MonopolyBot.Telegram.Interfaces.Status
{
    internal interface IAccountStatusHandler
    {
        Task HandleLoginAsync(Message message, ChatStatus status);
        Task HandleRegisterAsync(Message message, ChatStatus status);
        Task HandleDeleteAsync(Message message, ChatStatus status);
    }
}
