namespace MonopolyBot.Core.Models.Api.DTO.Games
{
    public class NextActionDto
    {
        public Guid PlayerId { get; set; }
        public string PlayerName { get; set; }

        public Guid NewPlayerId { get; set; }
        public string NewPlayerName { get; set; }
    }
}
