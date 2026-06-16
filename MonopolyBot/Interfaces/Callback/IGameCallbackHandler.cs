namespace MonopolyBot.Telegram.Interfaces.Callback
{
    public interface IGameCallbackHandler
    {
        Task HandleGameStatusAsync(long chatId, string data);
        Task HandleReturnToGameAsync(long chatId, string data);
        Task HandleWatchGameAsync(long chatId, string data);
    }
}
