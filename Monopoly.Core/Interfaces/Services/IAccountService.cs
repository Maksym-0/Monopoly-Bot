using MonopolyBot.Core.Models.Api.DTO.Accounts;
using MonopolyBot.Core.Models.Services;

namespace MonopolyBot.Core.Interfaces.Services
{
    public interface IAccountService
    {
        Task<ServiceResponse<ProfileInfo>> GetMyDataAsync(long chatId);
        Task<ServiceResponse<AccountDto>> RegisterAsync(string name, string password);
        Task<ServiceResponse<AccountDto>> LoginAsync(long chatId, string name, string password);
        Task<ServiceResponse<DeleteAccountDto>> DeleteAccountAsync(long chatId, string name, string password);
    }
}
