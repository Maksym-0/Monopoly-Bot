using Telegram.Bot.Types;

namespace MonopolyBot.Telegram.Interfaces.Command
{
    internal interface IAccountCommandHandler
    {
        Task HandleStartRegisterAsync(Message message);
        Task HandleStartLoginAsync(Message message);
        Task HandleStartDeleteAccountAsync(Message message);
        Task HandleRoomsMenuAsync(Message message);

        Task HandleMeAsync(Message message);
    }
}
