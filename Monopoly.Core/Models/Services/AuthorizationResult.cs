using MonopolyBot.Core.Models.Bot;

namespace MonopolyBot.Core.Models.Services
{
    public class AuthorizationResult
    {
        public bool IsAuthorized { get; set; }
        public string Message { get; set; }
        public User? User { get; set; }
    }
}
