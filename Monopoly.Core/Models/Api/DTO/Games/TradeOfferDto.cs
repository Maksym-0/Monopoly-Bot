namespace MonopolyBot.Core.Models.Api.DTO.Games
{
    public class TradeOfferDto
    {
        public Guid OffererAccountId { get; set; }
        public string OffererName { get; set; }
        public PropositionDto OffererProposition { get; set; }

        public Guid OffereeAccountId { get; set; }
        public string OffereeName { get; set; }
        public PropositionDto OffereeProposition { get; set; }

    }
}
