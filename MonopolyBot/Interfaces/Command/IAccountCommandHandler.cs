using Telegram.Bot.Types;

namespace MonopolyBot.Telegram.Interfaces.Command
{
    internal interface IAccountCommandHandler
    {
        Task HandleStartRegister(Message message);
        Task HandleStartLogin(Message message);
        Task HandleStartDeleteAccount(Message message);
        Task HandleRoomsMenu(Message message);

        Task HandleMe(Message message);
    }
}
