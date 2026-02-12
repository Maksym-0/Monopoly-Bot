using MonopolyBot.Core.Models.Api.DTO.Rooms;

namespace MonopolyBot.Core.Interfaces.Services
{
    public interface IRoomService
    {
        public Task<List<RoomDto>> GetRoomsAsync(long chatId);
        public Task<RoomDto> CreateRoomAsync(long chatId, int maxNumberOfPlayers, string? password);
        public Task<JoinRoomDto> JoinRoomAsync(long chatId, Guid roomId, string? password);
        public Task<QuitRoomDto> QuitRoomAsync(long chatId);

        public Task<List<long>> GetChatIdsInRoomAsync(long playerChatId);
    }
}
