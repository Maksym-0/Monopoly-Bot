namespace MonopolyBot.Telegram.Interfaces.Callback
{
    internal interface ITradeCallbackHandler
    {
        Task HandleSetOffereeIdAsync(long chatId, string data);
        Task HandleTradeConfirmAsync(long chatId, string data);
    }
}
