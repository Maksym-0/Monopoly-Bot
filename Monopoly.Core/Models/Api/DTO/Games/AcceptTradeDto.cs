namespace MonopolyBot.Core.Models.Api.DTO.Games
{
    public class AcceptTradeDto
    {
        public Guid OffererId { get; set; }
        public string OffererName { get; set; }
        public int NewOffererBalance { get; set; }
        public List<int> NewOffererCells { get; set; } = new List<int>();

        public Guid OffereeId { get; set; }
        public string OffereeName { get; set; }
        public int NewOffereeBalance { get; set; }
        public List<int> NewOffereeCells { get; set; } = new List<int>();
    }
}
