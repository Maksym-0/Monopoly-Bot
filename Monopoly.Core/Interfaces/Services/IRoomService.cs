using MonopolyBot.Core.Models.Api.DTO.Rooms;
using MonopolyBot.Core.Models.Services;

namespace MonopolyBot.Core.Interfaces.Services
{
    public interface IRoomService
    {
        public Task<ServiceResponse<List<RoomDto>>> GetRoomsAsync(long chatId);
        public Task<ServiceResponse<RoomDto>> CreateRoomAsync(long chatId, int maxNumberOfPlayers, string? password);
        public Task<ServiceResponse<JoinRoomDto>> JoinRoomAsync(long chatId, Guid roomId, string? password);
        public Task<ServiceResponse<QuitRoomDto>> QuitRoomAsync(long chatId);

        public Task<ServiceResponse<List<long>>> GetChatIdsInRoomAsync(long playerChatId);
    }
}
