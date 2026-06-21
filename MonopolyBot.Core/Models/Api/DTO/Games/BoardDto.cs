namespace MonopolyBot.Core.Models.Api.DTO.Games
{
    public class BoardDto
    {
        public Guid Id { get; set; }
        public List<CellDto> Cells { get; set; } = new();
        public List<MonopolyDto> Monopolies { get; set; } = new();
    }
}
