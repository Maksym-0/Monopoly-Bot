using MonopolyBot.Core.Models.Api.Requests;
using MonopolyBot.Core.Models.Api.Responses;
using MonopolyBot.Core.Models.Api.DTO.Rooms;

namespace MonopolyBot.Core.Interfaces.Clients
{
    public interface IRoomClient
    {
        public Task<ApiResponse<List<RoomDto>>> GetRoomsAsync(string jwt);
        public Task<ApiResponse<RoomDto>> CreateRoomAsync(string jwt, CreateRoomRequest dto);
        public Task<ApiResponse<JoinRoomDto>> JoinRoomAsync(string jwt, JoinRoomRequest dto);
        public Task<ApiResponse<QuitRoomDto>> QuitRoomAsync(string jwt);
    }
}
