using MonopolyBot.Core.Interfaces.Clients;
using MonopolyBot.Core.Interfaces.DataBase.UnitOfWork;
using MonopolyBot.Core.Interfaces.Services;
using MonopolyBot.Core.Models.Api.DTO.Accounts;
using MonopolyBot.Core.Models.Api.Requests;
using MonopolyBot.Core.Models.Api.Responses;
using MonopolyBot.Core.Models.Bot;
using MonopolyBot.Core.Models.Services;

namespace MonopolyBot.Application.Service
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountClient _accountClient;
        private readonly IAuthorizationService _authorization;

        public AccountService(IUnitOfWork unitOfWork, IAccountClient accountClient, IAuthorizationService authorization)
        {
            _unitOfWork = unitOfWork;
            _accountClient = accountClient;
            _authorization = authorization;
        }

        public async Task<AccServiceResponse> GetMyDataAsync(long chatId)
        {
            User user = await _authorization.GetAuthorizedUserAsync(chatId);

            return new AccServiceResponse()
            {
                Success = true,
                Message = "Ви авторизовані",
                Name = user.Name,
                Id = user.Id,
                AccountId = user.AccountId
            };
        }
        public async Task<AccountDto> RegisterAsync(string name, string password)
        {
            AccountRequest account = new AccountRequest()
            {
                Name = name,
                Password = password
            };
            ApiResponse<AccountDto> data = await _accountClient.RegisterAsync(account);

            if (!data.Success)
                throw new Exception(data.Message);
            return data.Data;
        }
        public async Task<AccountDto> LoginAsync(long chatId, string name, string password)
        {
            AccountRequest account = new AccountRequest()
            {
                Name = name,
                Password = password
            };
            ApiResponse<LoginDto> data = await _accountClient.LoginAndReturnJWTAsync(account);

            if (!data.Success)
            {
                throw new Exception(data.Message);
            }

            User? user = await _unitOfWork.Users.GetByChatId(chatId);
            if (user == null)
            {
                User response = new User()
                {
                    ChatId = chatId,
                    Id = data.Data.Account.Id,
                    AccountId = data.Data.Account.Id,
                    RoomId = null,
                    GameId = null,
                    Name = name,
                    JWT = data.Data.Token,
                    CreatedAt = data.Data.CreatedAt,
                    ExpiresAt = data.Data.ExpiresAt
                };

                await _unitOfWork.Users.Add(response);
            }
            else
            {
                user.AccountId = data.Data.Account.Id;
                user.Name = name;
                user.JWT = data.Data.Token;
                user.CreatedAt = data.Data.CreatedAt;
                user.ExpiresAt = data.Data.ExpiresAt;

                await _unitOfWork.Users.Update(user);
            }

            await _unitOfWork.SaveChangesAsync();
            return data.Data.Account;
        }
        public async Task<DeleteAccountDto> DeleteAccountAsync(long chatId, string name, string password)
        {
            AccountRequest account = new AccountRequest()
            {
                Name = name,
                Password = password
            };
            ApiResponse<DeleteAccountDto> data = await _accountClient.DeleteAccount(account);

            if (data.Success)
            {
                await _unitOfWork.Users.DeleteByChatId(chatId);
                await _unitOfWork.SaveChangesAsync();
                return data.Data;
            }
            else
            {
                throw new Exception(data.Message);
            }
        }
    }
}
