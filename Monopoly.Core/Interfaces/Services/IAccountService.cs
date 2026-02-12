using MonopolyBot.Core.Models.Api.DTO.Accounts;
using MonopolyBot.Core.Models.Services;

namespace MonopolyBot.Core.Interfaces.Services
{
    public interface IAccountService
    {
        Task<AccServiceResponse> GetMyDataAsync(long chatId);
        Task<AccountDto> RegisterAsync(string name, string password);
        Task<AccountDto> LoginAsync(long chatId, string name, string password);
        Task<DeleteAccountDto> DeleteAccountAsync(long chatId, string name, string password);
    }
}
