namespace MonopolyBot.Core.Models.Api.DTO.Rooms
{
    public class RoomDto
    {
        public Guid RoomId { get; set; }

        public int MaxNumberOfPlayers { get; set; }
        public int NumberOfPlayers { get; set; }

        public List<PlayerInRoomDto> Players { get; set; }

        public Guid? GameId { get; set; }

        public bool HavePassword { get; set; }
        public bool InGame { get; set; }
    }
}
