namespace MonopolyBot.Core.Models.Api.DTO.Games
{
    public class TurnStateDto
    {
        public int CurrentPlayerIndex { get; set; }

        public bool NeedPay { get; set; }
        public bool CanRollDices { get; set; }
        public bool CanBuyCell { get; set; } 
        public bool CanLevelUpCell { get; set; }
    }
}
