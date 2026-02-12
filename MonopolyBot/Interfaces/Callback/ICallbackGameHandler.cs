namespace MonopolyBot.Telegram.Interfaces.Callback
{
    public interface ICallbackGameHandler
    {
        Task HandleCallbackGameStatus(long chatId, string data);
        Task HandleCallbackReturnToGame(long chatId, string data);
        Task HandleCallbackWatchGame(long chatId, string data);
    }
}
