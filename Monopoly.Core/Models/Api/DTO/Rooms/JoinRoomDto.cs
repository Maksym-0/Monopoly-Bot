namespace MonopolyBot.Core.Models.Api.DTO.Rooms
{
    public class JoinRoomDto
    {
        public bool Success { get; set; }
        public bool IsGameStarted { get; set; }

        public RoomDto Room { get; set; }
    }
}
