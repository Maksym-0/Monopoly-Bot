namespace MonopolyBot.Core.Models.Api.Requests
{
    public class CreateRoomRequest
    {
        public int MaxNumberOfPlayers { get; set; }
        public string? Password { get; set; }
    }
}
