using MonopolyBot.Core.Models.Services;
using MonopolyBot.Core.Models.Bot;

namespace MonopolyBot.Core.Interfaces.Services
{
    public interface IAuthorizationService
    {
        Task<AuthorizationResult> GetAuthorizationResultAsync(long chatId);
        Task<User> GetAuthorizedUserAsync(long chatId);
    }
}
