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

        public async Task<AuthorizationResult> GetAuthorizationResultAsync(long chatId)
        {
            User? user = await _unitOfWork.Users.GetByChatId(chatId);

            if (user == null)
            {
                return new AuthorizationResult()
                {
                    IsAuthorized = false,
                    Message = "Користувач не знайдений. Увійдіть в систему"
                };
            }

            if (user.ExpiresAt < DateTime.Now)
            {
                await _unitOfWork.Users.DeleteByChatId(chatId);
                await _unitOfWork.SaveChangesAsync();

                return new AuthorizationResult()
                {
                    IsAuthorized = false,
                    Message = "Час авторизації вичерпано. Увійдіть в систему"
                };
            }
            else
                return new AuthorizationResult()
                {
                    IsAuthorized = true,
                    Message = "Авторизація успішна",
                    User = user
                };
            
        }
        public async Task<User> GetAuthorizedUserAsync(long chatId)
        {
            var data = await GetAuthorizationResultAsync(chatId);
            if (!data.IsAuthorized)
                throw new UnauthorizedAccessException(data.Message);
            return data.User;
        }
    }
}
