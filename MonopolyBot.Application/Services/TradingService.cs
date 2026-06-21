using MonopolyBot.Core.Enums;
using MonopolyBot.Core.Interfaces.Clients;
using MonopolyBot.Core.Interfaces.DataBase.UnitOfWork;
using MonopolyBot.Core.Interfaces.Services;
using MonopolyBot.Core.Models.Api.DTO.Games;
using MonopolyBot.Core.Models.Api.Requests;
using MonopolyBot.Core.Models.Api.Responses;
using MonopolyBot.Core.Models.Bot;
using MonopolyBot.Core.Models.Services;

namespace MonopolyBot.Application.Services
{
    public class TradingService : ITradingService
    {
        ITradingClient _tradingClient;
        IAuthorizationService _authorization;

        public TradingService(ITradingClient tradingClient, IAuthorizationService authorizationService)
        {
            _tradingClient = tradingClient;
            _authorization = authorizationService;
        }

        public async Task<ServiceResponse<TradeOfferDto>> GetTradeOfferAsync(long chatId)
        {
            ServiceResponse<User> userResponse = await _authorization.GetPlayerInGameAsync(chatId);
            if (!userResponse.Success)
            {
                return new ServiceResponse<TradeOfferDto>
                {
                    Success = false,
                    Message = userResponse.Message,
                    Data = null,
                    ErrorType = userResponse.ErrorType
                };
            }
            User user = userResponse.Data;

            ApiResponse<TradeOfferDto> apiResponse = await _tradingClient.GetCurrentTradeOfferAsync(user.JWT, user.GameId.Value);
            if (!apiResponse.Success)
            {
                return new ServiceResponse<TradeOfferDto>
                {
                    Success = false,
                    Message = apiResponse.Message,
                    Data = null,
                    ErrorType = ErrorType.ApiError
                };
            }
            return new ServiceResponse<TradeOfferDto>
            {
                Success = true,
                Message = "Успішно отримано пропозицію обміну",
                Data = apiResponse.Data
            };
        }
        public async Task<ServiceResponse<TradeOfferDto>> ProposeTradeAsync(long chatId, int offererMoneyProposition, List<int> OffererCellNumbers,
            Guid offereePlayerId, int offereeMoneyProposition, List<int> ofereeCellNumbers)
        {
            ServiceResponse<User> userResponse = await _authorization.GetPlayerInGameAsync(chatId);
            if (!userResponse.Success)
            {
                return new ServiceResponse<TradeOfferDto>
                {
                    Success = false,
                    Message = userResponse.Message,
                    Data = null,
                    ErrorType = userResponse.ErrorType
                };
            }
            User user = userResponse.Data;

            CreateTradeOfferRequest request = new CreateTradeOfferRequest
            {
                OffereePlayerId = offereePlayerId,
                OffereeMoneyProposition = offereeMoneyProposition,
                OffererMoneyProposition = offererMoneyProposition,
                OffereeCellNumbers = ofereeCellNumbers,
                OffererCellNumbers = OffererCellNumbers
            };

            ApiResponse<TradeOfferDto> apiResponse = await _tradingClient.CreateTradeOfferAsync(user.JWT, user.GameId.Value , request);
            if (!apiResponse.Success)
            {
                return new ServiceResponse<TradeOfferDto>
                {
                    Success = false,
                    Message = apiResponse.Message,
                    Data = null,
                    ErrorType = ErrorType.ApiError
                };
            }
            return new ServiceResponse<TradeOfferDto>
            {
                Success = true,
                Message = "Успішно створено пропозицію обміну",
                Data = apiResponse.Data
            };
        }
        public async Task<ServiceResponse<AcceptTradeDto>> AcceptTradeAsync(long chatId)
        {
            ServiceResponse<User> userResponse = await _authorization.GetPlayerInGameAsync(chatId);
            if (!userResponse.Success)
            {
                return new ServiceResponse<AcceptTradeDto>
                {
                    Success = false,
                    Message = userResponse.Message,
                    Data = null,
                    ErrorType = userResponse.ErrorType
                };
            }
            User user = userResponse.Data;

            ApiResponse<AcceptTradeDto> apiResponse = await _tradingClient.AcceptTradeOfferAsync(user.JWT, user.GameId.Value);
            if (!apiResponse.Success)
            {
                return new ServiceResponse<AcceptTradeDto>
                {
                    Success = false,
                    Message = apiResponse.Message,
                    Data = null,
                    ErrorType = ErrorType.ApiError
                };
            }
            return new ServiceResponse<AcceptTradeDto>
            {
                Success = true,
                Message = "Успішно прийнято пропозицію обміну",
                Data = apiResponse.Data
            };
        }
        public async Task<ServiceResponse<CancelTradeDto>> CancelTradeAsync(long chatId)
        {
            ServiceResponse<User> userResponse = await _authorization.GetPlayerInGameAsync(chatId);
            if (!userResponse.Success)
            {
                return new ServiceResponse<CancelTradeDto>
                {
                    Success = false,
                    Message = userResponse.Message,
                    Data = null,
                    ErrorType = userResponse.ErrorType
                };
            }
            User user = userResponse.Data;

            ApiResponse<CancelTradeDto> apiResponse = await _tradingClient.DeclineTradeOfferAsync(user.JWT, user.GameId.Value);
            if (!apiResponse.Success)
            {
                return new ServiceResponse<CancelTradeDto>
                {
                    Success = false,
                    Message = apiResponse.Message,
                    Data = null,
                    ErrorType = ErrorType.ApiError
                };
            }
            return new ServiceResponse<CancelTradeDto>
            {
                Success = true,
                Message = "Успішно скасовано пропозицію обміну",
                Data = apiResponse.Data
            };
        }
    }
}