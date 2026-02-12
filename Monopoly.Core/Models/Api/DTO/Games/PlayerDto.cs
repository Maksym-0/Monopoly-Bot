namespace MonopolyBot.Core.Models.Api.DTO.Games
{
    public class PlayerDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public int Index { get; set; }

        public int Balance { get; set; } 
        public int Location { get; set; }
        public int CantAction { get; set; }
        public int ReverseMove { get; set; } 

        public bool InGame { get; set; }
        public bool IsPrisoner { get; set; }

        public DiceDto Dices { get; set; }
    }
}
