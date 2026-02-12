using MonopolyBot.Core.Interfaces.Clients;
using MonopolyBot.Core.Interfaces.DataBase.UnitOfWork;
using MonopolyBot.Core.Interfaces.Services;
using MonopolyBot.Core.Models.Api.DTO.Games;
using MonopolyBot.Core.Models.Api.Responses;
using MonopolyBot.Core.Models.Bot;

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

        public async Task<GameStateDto> GameStatusAsync(long chatId)
        {
            User user = await GetValidUserAsync(chatId);

            var response = await _gameClient.GetGameStatusAsync(user.JWT, user.GameId.Value);
            if(!response.Success)
            {
                throw new Exception($"Не вдалося отримати статус гри: {response.Message}");
            }
            return response.Data;
        }
        public async Task<MoveDto> RollDiceAsync(long chatId)
        {
            User user = await GetValidUserAsync(chatId);

            var response = await _gameClient.RollTheDiceAsync(user.JWT, user.GameId.Value);
            if (!response.Success)
            {
                throw new Exception($"Не вдалося кинути кубики: {response.Message}");
            }
            return response.Data;
        }
        public async Task<PayDto> PayRentAsync(long chatId)
        {
            User user = await GetValidUserAsync(chatId);

            var response = await _gameClient.PayRentAsync(user.JWT, user.GameId.Value);
            if (!response.Success)
            {
                throw new Exception($"Не вдалося здійснити платіж: {response.Message}");
            }
            return response.Data;
        }
        public async Task<PayDto> PayToLeavePrisonAsync(long chatId)
        {
            User user = await GetValidUserAsync(chatId);

            var response = await _gameClient.PayToLeavePrisonAsync(user.JWT, user.GameId.Value);
            if (!response.Success)
            {
                throw new Exception($"Не вдалося сплатити за вихід з в'язниці: {response.Message}");
            }
            return response.Data;
        }
        public async Task<BuyDto> BuyCellAsync(long chatId)
        {
            User user = await GetValidUserAsync(chatId);

            var response = await _gameClient.BuyCellAsync(user.JWT, user.GameId.Value);
            if (!response.Success)
            {
                throw new Exception($"Не вдалося купити клітинку: {response.Message}");
            }
            return response.Data;
        }
        public async Task<LevelChangeDto> LevelUpCellAsync(long chatId, int cellNumber)
        {
            User user = await GetValidUserAsync(chatId);

            var response = await _gameClient.LevelUpCellAsync(user.JWT, user.GameId.Value, cellNumber);
            if (!response.Success)
            {
                throw new Exception($"Не вдалося підвищити рівень клітинки: {response.Message}");
            }
            return response.Data;
        }
        public async Task<LevelChangeDto> LevelDownCellAsync(long chatId, int cellNumber)
        {
            User user = await GetValidUserAsync(chatId);

            var response = await _gameClient.LevelDownCellAsync(user.JWT, user.GameId.Value, cellNumber);
            if (!response.Success)
            {
                throw new Exception($"Не вдалося знизити рівень клітинки: {response.Message}");
            }
            return response.Data;
        }
        public async Task<NextActionDto> EndActionAsync(long chatId)
        {
            User user = await GetValidUserAsync(chatId);

            var response = await _gameClient.EndActionAsync(user.JWT, user.GameId.Value);
            if (!response.Success)
            {
                throw new Exception($"Не вдалося завершити дію: {response.Message}");
            }
            return response.Data;
        }
        public async Task<LeaveGameDto> LeaveGameAsync(long chatId)
        {
            User user = await GetValidUserAsync(chatId);

            Guid gameId = user.GameId.Value;

            ApiResponse<LeaveGameDto> response = await _gameClient.LeaveGameAsync(user.JWT, user.GameId.Value);
            if (!response.Success)
            {
                throw new Exception($"Не вдалося вийти з гри: {response.Message}");
            }

            user.GameId = null;
            user.RoomId = null;

            await _unitOfWork.Users.Update(user);

            if (response.Data.IsGameOver)
            {
                List<User> usersInGame = await _unitOfWork.Users.GetListByGameId(gameId);
                
                foreach (var player in usersInGame)
                {
                    if(player.Id != user.Id)
                    {
                        player.GameId = null;
                        player.RoomId = null;
                        await _unitOfWork.Users.Update(player);
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();

            return response.Data;
        }

        public async Task<GameStateDto> TryReturnToGameAsync(long chatId, Guid gameId)
        {
            User user = await _authorization.GetAuthorizedUserAsync(chatId);

            ApiResponse<GameStateDto> response = await _gameClient.GetGameStatusAsync(user.JWT, gameId);

            if (!response.Success)
            {
                throw new Exception($"Не вдалося повернутися до гри: {response.Message}");
            }

            if (user.GameId != gameId)
            {
                user.RoomId = response.Data.RoomId;
                user.GameId = gameId;
                await _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();
            }

            return response.Data;
        }

        public async Task JoinWatchGameAsync(long chatId, Guid gameId)
        {
            User user = await _authorization.GetAuthorizedUserAsync(chatId);
            user.GameId = gameId;
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task LeaveWatchGameAsync(long chatId)
        {
            User user = await GetValidUserAsync(chatId);
            user.GameId = null;
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<long>> GetChatIdsInGameAsync(long palyerChatId)
        {
            User user = await GetValidUserAsync(palyerChatId);
            if (user.GameId == null)
                throw new Exception("Користувач не знаходиться в грі")
                    ;
            List<User> usersInGame = await _unitOfWork.Users.GetListByGameId(user.GameId.Value);

            return usersInGame.Select(u => u.ChatId).ToList();
        }

        private async Task<User> GetValidUserAsync(long chatId)
        {
            User user = await _authorization.GetAuthorizedUserAsync(chatId);
            if (user.GameId == null)
                throw new Exception("Користувач не знаходиться в грі");
            return user;
        }
    }
}
