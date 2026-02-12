using Telegram.Bot.Types;

namespace MonopolyBot.Telegram.Interfaces.Command
{
    internal interface IRoomCommandHandler
    {
        Task HandleCreateRoom(Message message);
        Task HandleGetRooms(Message message);
        Task HandleAccountsMenu(Message message);
    }
}
