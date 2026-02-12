namespace MonopolyBot.Core.Models.Api.DTO.Rooms
{
    public class PlayerInRoomDto
    {
        public Guid Id { get; set; }

        public Guid AccountId { get; set; }

        public string Name { get; set; }
        public int Index { get; set; }

        public Guid RoomId { get; set; }
    }
}
