namespace MonopolyBot.Telegram.Interfaces.Callback
{
    internal interface IRoomCallbackHandler
    {
        Task HandleJoinRoomAsync(long chatId, string data);
        Task HandleCreateRoomAsync(long chatId, string data);
        Task HandleLeaveRoomAsync(long chatId);
    }
}
