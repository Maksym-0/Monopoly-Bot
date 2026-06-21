using MonopolyBot.Core.Models.Api.DTO.Games;
using MonopolyBot.Core.Models.Api.Requests;
using MonopolyBot.Core.Models.Api.Responses;

namespace MonopolyBot.Core.Interfaces.Clients
{
    public interface ITradingClient
    {
        Task<ApiResponse<TradeOfferDto>> GetCurrentTradeOfferAsync(string jwt, Guid gameId);
        Task<ApiResponse<TradeOfferDto>> CreateTradeOfferAsync(string jwt, Guid gameId, CreateTradeOfferRequest dto);
        Task<ApiResponse<AcceptTradeDto>> AcceptTradeOfferAsync(string jwt, Guid gameId);
        Task<ApiResponse<CancelTradeDto>> DeclineTradeOfferAsync(string jwt, Guid gameId);
    }
}
