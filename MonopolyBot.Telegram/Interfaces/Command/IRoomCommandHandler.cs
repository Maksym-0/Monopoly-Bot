using Telegram.Bot.Types;

namespace MonopolyBot.Telegram.Interfaces.Command
{
    internal interface IRoomCommandHandler
    {
        Task HandleCreateRoomAsync(Message message);
        Task HandleGetRoomsAsync(Message message);
        Task HandleAccountsMenuAsync(Message message);
    }
}
