namespace MonopolyBot.Core.Models.Api.Requests
{
    public class JoinRoomRequest
    {
        public Guid RoomId { get; set; }
        public string? Password { get; set; }
    }
}
