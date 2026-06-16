using MonopolyBot.Core.Models.Api.DTO.Games;
using MonopolyBot.Core.Models.Services;

namespace MonopolyBot.Core.Interfaces.Services
{
    public interface ITradingService
    {
        Task<ServiceResponse<TradeOfferDto>> GetTradeOfferAsync(long chatId);
        Task<ServiceResponse<TradeOfferDto>> ProposeTradeAsync(long chatId, int offererMoneyProposition, List<int> OffererCellNumbers,
            Guid offereePlayerId, int offereeMoneyProposition, List<int> ofereeCellNumbers);
        Task<ServiceResponse<AcceptTradeDto>> AcceptTradeAsync(long chatId);
        Task<ServiceResponse<CancelTradeDto>> CancelTradeAsync(long chatId);
    }
}
