using MonopolyBot.Core.Models.Bot;
using Telegram.Bot.Types;

namespace MonopolyBot.Telegram.Interfaces.Status
{
    internal interface IAccountStatusHandler
    {
        Task HandleLoginStatus(Message message, ChatStatus status);
        Task HandleRegisterStatus(Message message, ChatStatus status);
        Task HandleDeleteAccountStatus(Message message, ChatStatus status);
    }
}
