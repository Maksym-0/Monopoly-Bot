using MonopolyBot.Core.Enums;
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

        public async Task<ServiceResponse<ProfileInfo>> GetMyDataAsync(long chatId)
        {
            ServiceResponse<User> userResponse = await _authorization.GetAuthorizedUserAsync(chatId);

            if (!userResponse.Success)
            {
                return new ServiceResponse<ProfileInfo>()
                {
                    Success = false,
                    Message = userResponse.Message,
                    Data = null,
                    ErrorType = userResponse.ErrorType
                };
            }
            User user = userResponse.Data;

            ProfileInfo profile = new ProfileInfo()
            {
                AccountId = user.AccountId,
                UserId = user.Id,
                Name = user.Name
            };

            return new ServiceResponse<ProfileInfo>()
            {
                Success = true,
                Message = userResponse.Message,
                Data = profile
            };
        }
        public async Task<ServiceResponse<AccountDto>> RegisterAsync(string name, string password)
        {
            AccountRequest account = new AccountRequest()
            {
                Name = name,
                Password = password
            };
            ApiResponse<AccountDto> data = await _accountClient.RegisterAsync(account);

            if (!data.Success)
                return new ServiceResponse<AccountDto>()
                {
                    Success = false,
                    Message = data.Message,
                    Data = null,
                    ErrorType = ErrorType.ApiError
                };
            return new ServiceResponse<AccountDto>()
            {
                Success = true,
                Message = "Реєстрація успішна",
                Data = data.Data
            };
        }
        public async Task<ServiceResponse<AccountDto>> LoginAsync(long chatId, string name, string password)
        {
            AccountRequest account = new AccountRequest()
            {
                Name = name,
                Password = password
            };
            ApiResponse<LoginDto> data = await _accountClient.LoginAndReturnJWTAsync(account);

            if (!data.Success)
            {
                return new ServiceResponse<AccountDto>()
                {
                    Success = false,
                    Message = data.Message,
                    Data = null,
                    ErrorType = ErrorType.ApiError
                };
            }

            User? user = await _unitOfWork.Users.GetByChatIdAsync(chatId);
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

                await _unitOfWork.Users.AddAsync(response);
            }
            else
            {
                user.AccountId = data.Data.Account.Id;
                user.Name = name;
                user.JWT = data.Data.Token;
                user.CreatedAt = data.Data.CreatedAt;
                user.ExpiresAt = data.Data.ExpiresAt;
            }

            await _unitOfWork.SaveChangesAsync();
            
            return new ServiceResponse<AccountDto>()
            {
                Success = true,
                Message = "Авторизація успішна",
                Data = data.Data.Account
            };
        }
        public async Task<ServiceResponse<DeleteAccountDto>> DeleteAccountAsync(long chatId, string name, string password)
        {
            AccountRequest account = new AccountRequest()
            {
                Name = name,
                Password = password
            };
            ApiResponse<DeleteAccountDto> data = await _accountClient.DeleteAccountAsync(account);

            if (data.Success)
            {
                await _unitOfWork.Users.DeleteByChatIdAsync(chatId);
                await _unitOfWork.SaveChangesAsync();
                return new ServiceResponse<DeleteAccountDto>()
                {
                    Success = true,
                    Message = "Акаунт успішно видалено",
                    Data = data.Data
                };
            }
            else
            {
                return new ServiceResponse<DeleteAccountDto>()
                {
                    Success = false,
                    Message = data.Message,
                    Data = null,
                    ErrorType = ErrorType.ApiError
                };
            }
        }
    }
}
