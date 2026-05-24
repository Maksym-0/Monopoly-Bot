using MonopolyBot.Core.Enums;
using MonopolyBot.Core.Interfaces.Clients;
using MonopolyBot.Core.Interfaces.DataBase.UnitOfWork;
using MonopolyBot.Core.Interfaces.Services;
using MonopolyBot.Core.Models.Api.DTO.Games;
using MonopolyBot.Core.Models.Api.Responses;
using MonopolyBot.Core.Models.Bot;
using MonopolyBot.Core.Models.Services;
using Telegram.Bot.Types.Payments;

namespace MonopolyBot.Application.Service
{
    public class GameService : IGameService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGameClient _gameClient;
        private readonly IAuthorizationService _authorization;

        public GameService(IUnitOfWork unitOfWork, IGameClient gameClient, IAuthorizationService authorization)
        {
            _unitOfWork = unitOfWork;
            _gameClient = gameClient;
            _authorization = authorization;
        }

        public async Task<ServiceResponse<GameStateDto>> GameStatusAsync(long chatId)
        {
            ServiceResponse<User> userResponse = await GetValidUserAsync(chatId);
            if (!userResponse.Success)
            {
                return new ServiceResponse<GameStateDto>
                {
                    Success = false,
                    Message = userResponse.Message,
                    Data = null,
                    ErrorType = userResponse.ErrorType
                };
            }
            User user = userResponse.Data;

            var response = await _gameClient.GetGameStatusAsync(user.JWT, user.GameId.Value);
            if(!response.Success)
            {
                return new ServiceResponse<GameStateDto>
                {
                    Success = false,
                    Message = response.Message,
                    Data = null,
                    ErrorType = ErrorType.ApiError
                };
            }
            return new ServiceResponse<GameStateDto>
            {
                Success = true,
                Message = "Успішно отримано статус гри",
                Data = response.Data
            };
        }
        public async Task<ServiceResponse<MoveDto>> RollDiceAsync(long chatId)
        {
            ServiceResponse<User> userResponse = await GetValidUserAsync(chatId);
            if (!userResponse.Success)
            {
                return new ServiceResponse<MoveDto>
                {
                    Success = false,
                    Message = userResponse.Message,
                    Data = null,
                    ErrorType = userResponse.ErrorType
                };
            }
            User user = userResponse.Data;

            var response = await _gameClient.RollTheDiceAsync(user.JWT, user.GameId.Value);
            if (!response.Success)
            {
                return new ServiceResponse<MoveDto>
                {
                    Success = false,
                    Message = response.Message,
                    Data = null,
                    ErrorType = ErrorType.ApiError
                };
            }
            return new ServiceResponse<MoveDto>
            {
                Success = true,
                Message = "Успішно виконано кидок кубика",
                Data = response.Data
            };
        }
        public async Task<ServiceResponse<PayDto>> PayRentAsync(long chatId)
        {
            ServiceResponse<User> userResponse = await GetValidUserAsync(chatId);
            if (!userResponse.Success)
            {
                return new ServiceResponse<PayDto>
                {
                    Success = false,
                    Message = userResponse.Message,
                    Data = null,
                    ErrorType = userResponse.ErrorType
                };
            }
            User user = userResponse.Data;

            var response = await _gameClient.PayRentAsync(user.JWT, user.GameId.Value);
            if (!response.Success)
            {
                return new ServiceResponse<PayDto>
                {
                    Success = false,
                    Message = response.Message,
                    Data = null,
                    ErrorType = ErrorType.ApiError
                };
            }
            return new ServiceResponse<PayDto>
            {
                Success = true,
                Message = "Успішно сплачено оренду",
                Data = response.Data
            };
        }
        public async Task<ServiceResponse<PayDto>> PayToLeavePrisonAsync(long chatId)
        {
            ServiceResponse<User> userResponse = await GetValidUserAsync(chatId);
            if (!userResponse.Success)
            {
                return new ServiceResponse<PayDto>
                {
                    Success = false,
                    Message = userResponse.Message,
                    Data = null,
                    ErrorType = userResponse.ErrorType
                };
            }
            User user = userResponse.Data;

            var response = await _gameClient.PayToLeavePrisonAsync(user.JWT, user.GameId.Value);
            if (!response.Success)
            {
                return new ServiceResponse<PayDto>
                {
                    Success = false,
                    Message = response.Message,
                    Data = null,
                    ErrorType = ErrorType.ApiError
                };
            }
            return new ServiceResponse<PayDto>
            {
                Success = true,
                Message = "Успішно сплачено за вихід з в'язниці",
                Data = response.Data
            };
        }
        public async Task<ServiceResponse<BuyDto>> BuyCellAsync(long chatId)
        {
            ServiceResponse<User> userResponse = await GetValidUserAsync(chatId);
            if (!userResponse.Success)
            {
                return new ServiceResponse<BuyDto>
                {
                    Success = false,
                    Message = userResponse.Message,
                    Data = null,
                    ErrorType = userResponse.ErrorType
                };
            }
            User user = userResponse.Data;

            var response = await _gameClient.BuyCellAsync(user.JWT, user.GameId.Value);
            if (!response.Success)
            {
                return new ServiceResponse<BuyDto>
                {
                    Success = false,
                    Message = response.Message,
                    Data = null,
                    ErrorType = ErrorType.ApiError
                };
            }
            return new ServiceResponse<BuyDto>
            {
                Success = true,
                Message = "Успішно куплено клітинку",
                Data = response.Data
            };
        }
        public async Task<ServiceResponse<LevelChangeDto>> LevelUpCellAsync(long chatId, int cellNumber)
        {
            ServiceResponse<User> userResponse = await GetValidUserAsync(chatId);
            if (!userResponse.Success)
            {
                return new ServiceResponse<LevelChangeDto>
                {
                    Success = false,
                    Message = userResponse.Message,
                    Data = null,
                    ErrorType = userResponse.ErrorType
                };
            }
            User user = userResponse.Data;

            var response = await _gameClient.LevelUpCellAsync(user.JWT, user.GameId.Value, cellNumber);
            if (!response.Success)
            {
                return new ServiceResponse<LevelChangeDto>
                {
                    Success = false,
                    Message = response.Message,
                    Data = null,
                    ErrorType = ErrorType.ApiError
                };
            }
            return new ServiceResponse<LevelChangeDto>
            {
                Success = true,
                Message = "Успішно підвищено рівень клітинки",
                Data = response.Data
            };
        }
        public async Task<ServiceResponse<LevelChangeDto>> LevelDownCellAsync(long chatId, int cellNumber)
        {
            ServiceResponse<User> userResponse = await GetValidUserAsync(chatId);
            if (!userResponse.Success)
            {
                return new ServiceResponse<LevelChangeDto>
                {
                    Success = false,
                    Message = userResponse.Message,
                    Data = null,
                    ErrorType = userResponse.ErrorType
                };
            }
            User user = userResponse.Data;

            var response = await _gameClient.LevelDownCellAsync(user.JWT, user.GameId.Value, cellNumber);
            if (!response.Success)
            {
                return new ServiceResponse<LevelChangeDto>
                {
                    Success = false,
                    Message = response.Message,
                    Data = null,
                    ErrorType = ErrorType.ApiError
                };
            }
            return new ServiceResponse<LevelChangeDto>
            {
                Success = true,
                Message = "Успішно знижено рівень клітинки",
                Data = response.Data
            };
        }
        public async Task<ServiceResponse<NextActionDto>> EndActionAsync(long chatId)
        {
            ServiceResponse<User> userResponse = await GetValidUserAsync(chatId);
            if (!userResponse.Success)
            {
                return new ServiceResponse<NextActionDto>
                {
                    Success = false,
                    Message = userResponse.Message,
                    Data = null,
                    ErrorType = userResponse.ErrorType
                };
            }
            User user = userResponse.Data;

            var response = await _gameClient.EndActionAsync(user.JWT, user.GameId.Value);
            if (!response.Success)
            {
                return new ServiceResponse<NextActionDto>
                {
                    Success = false,
                    Message = response.Message,
                    Data = null,
                    ErrorType = ErrorType.ApiError
                };
            }
            return new ServiceResponse<NextActionDto>
            {
                Success = true,
                Message = "Успішно завершено дію",
                Data = response.Data
            };
        }
        public async Task<ServiceResponse<LeaveGameDto>> LeaveGameAsync(long chatId)
        {
            ServiceResponse<User> userResponse = await GetValidUserAsync(chatId);
            if (!userResponse.Success)
            {
                return new ServiceResponse<LeaveGameDto>
                {
                    Success = false,
                    Message = userResponse.Message,
                    Data = null,
                    ErrorType = userResponse.ErrorType
                };
            }
            User user = userResponse.Data;

            ApiResponse<LeaveGameDto> response = await _gameClient.LeaveGameAsync(user.JWT, user.GameId.Value);
            if (!response.Success)
            {
                return new ServiceResponse<LeaveGameDto>
                {
                    Success = false,
                    Message = response.Message,
                    Data = null,
                    ErrorType = ErrorType.ApiError
                };
            }

            user.GameId = null;
            user.RoomId = null;

            if (response.Data.IsGameOver)
            {
                List<User> usersInGame = await _unitOfWork.Users.GetListByGameId(user.GameId.Value);
                
                foreach (var player in usersInGame)
                {
                    if(player.Id != user.Id)
                    {
                        player.GameId = null;
                        player.RoomId = null;
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();

            return new ServiceResponse<LeaveGameDto>
            {
                Success = true,
                Message = "Успішно покинуто гру",
                Data = response.Data
            };
        }
        public async Task<ServiceResponse<GameStateDto>> ReturnToGameAsync(long chatId, Guid gameId)
        {
            AuthorizationResult authResult = await _authorization.GetAuthorizationResultAsync(chatId);
            if(!authResult.IsAuthorized)
            {
                return new ServiceResponse<GameStateDto>
                {
                    Success = false,
                    Message = authResult.Message,
                    Data = null,
                    ErrorType = ErrorType.Unauthorized
                };
            }
            User user = authResult.User;

            ApiResponse<GameStateDto> response = await _gameClient.GetGameStatusAsync(user.JWT, gameId);
            if (!response.Success)
            {
                return new ServiceResponse<GameStateDto>
                {
                    Success = false,
                    Message = response.Message,
                    Data = null,
                    ErrorType = ErrorType.ApiError
                };
            }

            if (user.GameId != gameId)
            {
                user.RoomId = response.Data.RoomId;
                user.GameId = gameId;
                await _unitOfWork.SaveChangesAsync();
            }

            return new ServiceResponse<GameStateDto>
            {
                Success = true,
                Message = "Ви повернулись до гри",
                Data = response.Data
            };
        }

        public async Task<ServiceResponse<GameStateDto>> JoinWatchGameAsync(long chatId, Guid gameId)
        {
            AuthorizationResult authResult = await _authorization.GetAuthorizationResultAsync(chatId);
            if(!authResult.IsAuthorized)
            {
                return new ServiceResponse<GameStateDto>
                {
                    Success = false,
                    Message = authResult.Message,
                    Data = null,
                    ErrorType = ErrorType.ApiError
                };
            }
            User user = authResult.User;

            ServiceResponse<GameStateDto> gameStatusResponse = await GameStatusAsync(chatId);
            if(!gameStatusResponse.Success)
            {
                return new ServiceResponse<GameStateDto>
                {
                    Success = false,
                    Message = gameStatusResponse.Message,
                    Data = null,
                    ErrorType = ErrorType.ApiError
                };
            }

            user.GameId = gameId;
            await _unitOfWork.SaveChangesAsync();

            return new ServiceResponse<GameStateDto>
            {
                Success = true,
                Message = "Ви приєдналися до перегляду гри",
                Data = gameStatusResponse.Data
            };
        }
        public async Task<ServiceResponse<bool>> LeaveWatchGameAsync(long chatId)
        {
            AuthorizationResult authResult = await _authorization.GetAuthorizationResultAsync(chatId);
            if(!authResult.IsAuthorized)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = authResult.Message,
                    Data = false,
                    ErrorType = ErrorType.Unauthorized
                };
            }
            User user = authResult.User;
            user.GameId = null;
            await _unitOfWork.SaveChangesAsync();
            return new ServiceResponse<bool>
            {
                Success = true,
                Message = "Ви покинули перегляд гри",
                Data = true
            };
        }

        public async Task<ServiceResponse<List<long>>> GetChatIdsInGameAsync(long palyerChatId)
        {
            ServiceResponse<User> userResponse = await GetValidUserAsync(palyerChatId);
            if (!userResponse.Success)
            {
                return new ServiceResponse<List<long>>
                {
                    Success = false,
                    Message = userResponse.Message,
                    Data = null,
                    ErrorType = userResponse.ErrorType
                };
            }
            User user = userResponse.Data;
            
            List<User> usersInGame = await _unitOfWork.Users.GetListByGameId(user.GameId.Value);
            List<long> chatIds = usersInGame.Select(u => u.ChatId).ToList();

            return new ServiceResponse<List<long>>
            {
                Success = true,
                Message = "Успішно отримано id чату гравців в грі",
                Data = chatIds
            };
        }

        private async Task<ServiceResponse<User>> GetValidUserAsync(long chatId)
        {
            AuthorizationResult authResult = await _authorization.GetAuthorizationResultAsync(chatId);
            if (!authResult.IsAuthorized)
            {
                return new ServiceResponse<User>
                {
                    Success = false,
                    Message = authResult.Message,
                    Data = null,
                    ErrorType = ErrorType.Unauthorized
                };
            }
            User user = authResult.User;

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
                Message = "Успішно отримано користувача",
                Data = user
            };
        }
    }
}
