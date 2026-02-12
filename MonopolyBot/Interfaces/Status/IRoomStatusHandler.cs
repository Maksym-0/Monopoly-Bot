using MonopolyBot.Core.Models.Bot;
using Telegram.Bot.Types;

namespace MonopolyBot.Telegram.Interfaces.Status
{
    internal interface IRoomStatusHandler
    {
        Task HandleJoinRoomStatus(Message message, ChatStatus status);
        Task HandleCreateRoomStatus(Message message, ChatStatus status);
        Task HandleCreateRoomPasswordStatus(Message message, ChatStatus status);
    }
}
