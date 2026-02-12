using Telegram.Bot.Types;

namespace MonopolyBot.Telegram.Interfaces.Command
{
    internal interface IStartCommandHandler
    {
        Task HandleStart(Message message);
    }
}
