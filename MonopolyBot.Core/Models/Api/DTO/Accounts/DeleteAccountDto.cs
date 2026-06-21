namespace MonopolyBot.Core.Models.Api.DTO.Accounts
{
    public class DeleteAccountDto
    {
        public Guid AccountId { get; set; }
        public string Name { get; set; }

        public bool IsDeleted { get; set; }
    }
}
