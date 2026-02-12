using MonopolyBot.Core.Models.Api.Requests;
using MonopolyBot.Core.Models.Api.Responses;
using MonopolyBot.Core.Models.Api.DTO.Accounts;

namespace MonopolyBot.Core.Interfaces.Clients
{
    public interface IAccountClient
    {
        public Task<ApiResponse<AccountDto>> MeAsync(string jwt);
        public Task<ApiResponse<AccountDto>> RegisterAsync(AccountRequest account);
        public Task<ApiResponse<LoginDto>> LoginAndReturnJWTAsync(AccountRequest account);
        public Task<ApiResponse<DeleteAccountDto>> DeleteAccount(AccountRequest account);
    }
}