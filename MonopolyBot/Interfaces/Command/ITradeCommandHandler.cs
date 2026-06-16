using Telegram.Bot.Types;

namespace MonopolyBot.Telegram.Interfaces.Command
{
    internal interface ITradeCommandHandler
    {
        Task HandleStartTradeAsync(Message message);
        Task HandleAcceptTradeAsync(Message message);
        Task HandleCancelTradeAsync(Message message);
    }
}
