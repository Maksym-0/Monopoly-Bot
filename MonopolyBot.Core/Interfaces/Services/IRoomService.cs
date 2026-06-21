using MonopolyBot.Core.Models.Api.DTO.Rooms;
using MonopolyBot.Core.Models.Services;

namespace MonopolyBot.Core.Interfaces.Services
{
    public interface IRoomService
    {
        Task<ServiceResponse<List<RoomDto>>> GetRoomsAsync(long chatId);
        Task<ServiceResponse<RoomDto>> CreateRoomAsync(long chatId, int maxNumberOfPlayers, string? password);
        Task<ServiceResponse<JoinRoomDto>> JoinRoomAsync(long chatId, Guid roomId, string? password);
        Task<ServiceResponse<QuitRoomDto>> QuitRoomAsync(long chatId);

        Task<ServiceResponse<List<long>>> GetChatIdsInRoomAsync(long playerChatId);
        Task<ServiceResponse<List<long>>> GetChatIdsByRoomIdAsync(Guid roomId);
    }
}
