using MonopolyBot.Core.Models.Api.DTO.Games;

namespace MonopolyBot.Telegram.Interfaces.Services
{
    internal interface IBroadcastService
    {
        Task SendGameStartAsync(List<long> chatIds, Guid roomId);
        Task SendPlayerLeaveAsync(List<long> chatIds, long selfChatId, LeaveGameDto leaveInfo);
        Task SendGameStatusAsync(List<long> chatIds, GameStateDto game);

        Task SendMessageAsync(List<long> chatIds, string message);

        Task SendPersonalizedMessageAsync(long personalChatId, string personalMessage, List<long> chatIds, string publicMessage);
    }
}
