namespace MonopolyBot.Core.Models.Api.DTO.Games
{
    public class BuyDto
    {
        public Guid PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int OldBalance { get; set; }
        public int NewBalance { get; set; }

        public int CellNumber { get; set; }
        public string CellName { get; set; }
        public int Price { get; set; }

        public string? CellMonopolyType { get; set; }
        public bool? HasMonopoly { get; set; } 
    }
}
