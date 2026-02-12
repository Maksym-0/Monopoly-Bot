namespace MonopolyBot.Telegram.Interfaces.Callback
{
    internal interface ICallbackRoomHandler
    {
        Task HandleCallbackJoinRoom(long chatId, string data);
        Task HandleCallbackCreateRoom(long chatId, string data);
        Task HandleCallbackLeaveRoom(long chatId);
    }
}
