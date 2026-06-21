using MonopolyBot.Core.Models.Bot;
using Telegram.Bot.Types;

namespace MonopolyBot.Telegram.Interfaces.Status
{
    internal interface IRoomStatusHandler
    {
        Task HandleJoinAsync(Message message, ChatStatus status);
        Task HandleCreateAsync(Message message, ChatStatus status);
        Task HandleCreatePasswordAsync(Message message, ChatStatus status);
    }
}
