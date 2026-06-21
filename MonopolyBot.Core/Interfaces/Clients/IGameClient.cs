using MonopolyBot.Core.Models.Api.Responses;
using MonopolyBot.Core.Models.Api.DTO.Games;

namespace MonopolyBot.Core.Interfaces.Clients
{
    public interface IGameClient
    {
        public Task<ApiResponse<GameStateDto>> GetGameStatusAsync(string jwt, Guid gameId);
        public Task<ApiResponse<MoveDto>> RollTheDiceAsync(string jwt, Guid gameId);
        public Task<ApiResponse<PayDto>> PayRentAsync(string jwt, Guid gameId);
        public Task<ApiResponse<PayDto>> PayToLeavePrisonAsync(string jwt, Guid gameId);
        public Task<ApiResponse<BuyDto>> BuyCellAsync(string jwt, Guid gameId);
        public Task<ApiResponse<LevelChangeDto>> LevelUpCellAsync(string jwt, Guid gameId, int cellNumber);
        public Task<ApiResponse<LevelChangeDto>> LevelDownCellAsync(string jwt, Guid gameId, int cellNumber);
        public Task<ApiResponse<NextActionDto>> EndActionAsync(string jwt, Guid gameId);
        public Task<ApiResponse<LeaveGameDto>> LeaveGameAsync(string jwt, Guid gameId);
    }
}
