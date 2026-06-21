using MonopolyBot.Core.Models.Api.DTO.Games;

namespace MonopolyBot.Core.Models.Api.DTO.Rooms
{
    public class QuitRoomDto
    {
        public bool IsRoomDeleted { get; set; }
       
        public RoomDto? RoomDto { get; set; }

        public LeaveGameDto? LeaveGameDto { get; set; }
    }
}
