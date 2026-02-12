namespace MonopolyBot.Core.Models.Api.DTO.Games
{
    public class MoveDto
    {
        public PlayerDto Player { get; set; }
        public TurnStateDto TurnState { get; set; }
        public CellDto Cell { get; set; }

        public string CellMessage { get; set; }
    }
}
