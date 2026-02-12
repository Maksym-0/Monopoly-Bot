namespace MonopolyBot.Core.Models.Api.DTO.Games
{
    public class CellDto
    {
        public Guid Id { get; set; }
        public int Number {  get; set; }
        public string Name { get; set; }
        public string CellType { get; set; }
        public bool Special {  get; set; }

        public int? Price { get; set; }
        public int? Rent { get; set; }
        public int? Level { get; set; }
        public Guid? OwnerId { get; set; }
        public int? MonopolyIndex { get; set; }
    }
}
