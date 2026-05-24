using MonopolyBot.Core.Models.Api.DTO.Games;
using MonopolyBot.Core.Models.Services;

namespace MonopolyBot.Core.Interfaces.Services
{
    public interface IGameService
    {
        Task<ServiceResponse<GameStateDto>> GameStatusAsync(long chatId);
        Task<ServiceResponse<MoveDto>> RollDiceAsync(long chatId);
        Task<ServiceResponse<PayDto>> PayRentAsync(long chatId);
        Task<ServiceResponse<PayDto>> PayToLeavePrisonAsync(long chatId);
        Task<ServiceResponse<BuyDto>> BuyCellAsync(long chatId);
        Task<ServiceResponse<LevelChangeDto>> LevelUpCellAsync(long chatId, int cellNumber);
        Task<ServiceResponse<LevelChangeDto>> LevelDownCellAsync(long chatId, int cellNumber);
        Task<ServiceResponse<NextActionDto>> EndActionAsync(long chatId);
        Task<ServiceResponse<LeaveGameDto>> LeaveGameAsync(long chatId);
        Task<ServiceResponse<GameStateDto>> ReturnToGameAsync(long chatId, Guid gameId);

        Task<ServiceResponse<GameStateDto>> JoinWatchGameAsync(long chatId, Guid gameId);
        Task<ServiceResponse<bool>> LeaveWatchGameAsync(long chatId);

        Task<ServiceResponse<List<long>>> GetChatIdsInGameAsync(long palyerChatId);
    }
}