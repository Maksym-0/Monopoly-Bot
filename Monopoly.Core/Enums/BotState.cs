namespace MonopolyBot.Core.Enums
{
    public enum BotState
    {
        None = 0,
        
        AwaitingLogin,
        AwaitingRegister,
        AwaitingDeleteAccount,
        
        InRoom,
        AwaitingJoinRoom,
        AwaitingCreateRoom,
        AwaitingCreateRoomPassword,

        InGame,
        AwaitingLevelUpCell,
        AwaitingLevelDownCell,

        WatchingGame
    }
}
