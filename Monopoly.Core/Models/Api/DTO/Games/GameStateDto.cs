namespace MonopolyBot.Core.Models.Api.DTO.Games
{
    public class GameStateDto
    {
        public Guid GameId { get; set; }

        public Guid RoomId { get; set; }

        public List<PlayerDto> Players { get; set; } = new();
        public BoardDto Board { get; set; }
        public TurnStateDto TurnState { get; set; }
        public TradeOfferDto? CurrentTradeOffer { get; set; }
    }
}
