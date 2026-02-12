namespace MonopolyBot.Core.Models.Services
{
    public class AccServiceResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Guid AccountId { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
