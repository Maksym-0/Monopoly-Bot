using MonopolyBot.Core.Interfaces.DataBase.UnitOfWork;
using MonopolyBot.Core.Interfaces.Services;
using MonopolyBot.Core.Models.Bot;
using MonopolyBot.Core.Models.Services;

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
            User? user = await _unitOfWork.Users.GetByChatId(chatId);

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
                await _unitOfWork.Users.DeleteByChatId(chatId);
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
    }
}
