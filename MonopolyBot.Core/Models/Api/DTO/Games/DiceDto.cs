namespace MonopolyBot.Core.Models.Api.DTO.Games
{
    public class DiceDto
    {
        public int Dice1 { get; set; }
        public int Dice2 { get; set; }
        public int DiceSum { get; set; }

        public bool Dubl { get; set; }
        public int CountOfDubles { get; set; }
    }
}
