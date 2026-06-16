namespace MonopolyBot.Core.Models.Api.DTO.Games
{
    public class PayDto
    {
        public Guid PlayerId { get; set; }
        public string PlayerName { get; set; }

        public Guid? ReceiverId { get; set; }
        public string? ReceiverName { get; set; }
        
        public int NewPlayerBalance { get; set; }
        public int? NewReceiverBalance { get; set; }

        public bool IsPrisonPay { get; set; }

        public int Amount { get; set; }
    }
}
