namespace MonopolyBot.Core.Models.Api.DTO.Games
{
    public class LeaveGameDto
    {
        public Guid PlayerId { get; set; }
        public string PlayerName { get; set; }
        
        public int RemainingPlayers { get; set; }
        public PlayerDto? Winner { get; set; }

        public bool IsGameOver { get; set; }
    }
}
