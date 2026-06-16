using MonopolyBot.Core.Enums;
using MonopolyBot.Core.Interfaces.DataBase.UnitOfWork;
using MonopolyBot.Core.Interfaces.Services;
using MonopolyBot.Core.Models.Bot;
using MonopolyBot.Core.Models.Services;
using System.Net;

namespace MonopolyBot.Application.Service
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthorizationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResponse<User>> GetAuthorizedUserAsync(long chatId)
        {
            User? user = await _unitOfWork.Users.GetByChatIdAsync(chatId);

            if (user == null)
            {
                return new ServiceResponse<User>
                {
                    Success = false,
                    Message = "Користувач не авторизований. Увійдіть в систему",
                    Data = null,
                    ErrorType = Core.Enums.ErrorType.Unauthorized
                };
            }

            if (user.ExpiresAt < DateTime.Now)
            {
                await _unitOfWork.Users.DeleteByChatIdAsync(chatId);
                await _unitOfWork.SaveChangesAsync();

                return new ServiceResponse<User>
                {
                    Success = false,
                    Message = "Термін дії сесії закінчився. Увійдіть в систему знову",
                    Data = null,
                    ErrorType = Core.Enums.ErrorType.Unauthorized
                };
            }
            else
            {
                return new ServiceResponse<User>
                {
                    Success = true,
                    Message = "Користувач авторизований",
                    Data = user
                };
            }
        }
        public async Task<ServiceResponse<User>> GetPlayerInGameAsync(long chatId)
        {
            ServiceResponse<User> authResult = await GetAuthorizedUserAsync(chatId);
            if (!authResult.Success)
            {
                return authResult;
            }
            User user = authResult.Data;

            if (user.GameId == null)
                return new ServiceResponse<User>
                {
                    Success = false,
                    Message = "Користувач не знаходиться в грі",
                    Data = null,
                    ErrorType = ErrorType.ServiceError
                };
            return new ServiceResponse<User>
            {
                Success = true,
                Message = "Успішно отримано користувача в грі",
                Data = user
            };
        }
    }
}
