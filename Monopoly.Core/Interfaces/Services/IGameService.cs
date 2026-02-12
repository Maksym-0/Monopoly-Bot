using MonopolyBot.Core.Models.Api.DTO.Games;

namespace MonopolyBot.Core.Interfaces.Services
{
    public interface IGameService
    {
        Task<GameStateDto> GameStatusAsync(long chatId);
        Task<MoveDto> RollDiceAsync(long chatId);
        Task<PayDto> PayRentAsync(long chatId);
        Task<PayDto> PayToLeavePrisonAsync(long chatId);
        Task<BuyDto> BuyCellAsync(long chatId);
        Task<LevelChangeDto> LevelUpCellAsync(long chatId, int cellNumber);
        Task<LevelChangeDto> LevelDownCellAsync(long chatId, int cellNumber);
        Task<NextActionDto> EndActionAsync(long chatId);
        Task<LeaveGameDto> LeaveGameAsync(long chatId);

        Task<GameStateDto> TryReturnToGameAsync(long chatId, Guid gameId);


        Task JoinWatchGameAsync(long chatId, Guid gameId);
        Task LeaveWatchGameAsync(long chatId);

        Task<List<long>> GetChatIdsInGameAsync(long palyerChatId);
    }
}