using MonopolyBot.Core.Interfaces.Clients;
using MonopolyBot.Core.Interfaces.DataBase.UnitOfWork;
using MonopolyBot.Core.Interfaces.Services;
using MonopolyBot.Core.Models.Api.DTO.Rooms;
using MonopolyBot.Core.Models.Api.Requests;
using MonopolyBot.Core.Models.Api.Responses;
using MonopolyBot.Core.Models.Bot;

namespace MonopolyBot.Application.Service
{
    public class RoomService : IRoomService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoomClient _roomClient;
        private readonly IAuthorizationService _authorization;

        public RoomService(IUnitOfWork unitOfWork, IAuthorizationService authorization, IRoomClient roomClient)
        {
            _unitOfWork = unitOfWork;
            _authorization = authorization;
            _roomClient = roomClient;
        }

        public async Task<List<RoomDto>> GetRoomsAsync(long chatId)
        {
            User? user = await _authorization.GetAuthorizedUserAsync(chatId);

            ApiResponse<List<RoomDto>> response = await _roomClient.GetRoomsAsync(user.JWT);
            if (!response.Success)
                throw new Exception(response.Message);

            return response.Data;
        }
        public async Task<RoomDto> CreateRoomAsync(long chatId, int maxNumberOfPlayers, string? password)
        {
            User user = await _authorization.GetAuthorizedUserAsync(chatId);

            ApiResponse<RoomDto> response = await _roomClient.CreateRoomAsync(user.JWT, 
                new CreateRoomRequest
            {
                MaxNumberOfPlayers = maxNumberOfPlayers,
                Password = password
            });

            if (!response.Success)
                throw new Exception(response.Message);

            user.RoomId = response.Data.RoomId;

            await _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return response.Data;
        }
        public async Task<JoinRoomDto> JoinRoomAsync(long chatId, Guid roomId, string? password)
        {
            User user = await _authorization.GetAuthorizedUserAsync(chatId);

            ApiResponse<JoinRoomDto> response = await _roomClient.JoinRoomAsync(user.JWT, 
                new JoinRoomRequest
            {
                RoomId = roomId,
                Password = password
            });

            if (!response.Success)
                throw new Exception(response.Message);
            
            user.RoomId = roomId;
            user.GameId = response.Data.Room.GameId;

            if (response.Data.IsGameStarted)
            {
                List<User> usersInRoom = await _unitOfWork.Users.GetListByRoomId(roomId);

                foreach (var player in usersInRoom)
                {
                    player.GameId = response.Data.Room.GameId;
                }
            }

            await _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return response.Data;
        }
        public async Task<QuitRoomDto> QuitRoomAsync(long chatId)
        {
            User user = await _authorization.GetAuthorizedUserAsync(chatId);

            ApiResponse<QuitRoomDto> response = await _roomClient.QuitRoomAsync(user.JWT);

            if (!response.Success)
                throw new Exception(response.Message);

            user.RoomId = null;
            user.GameId = null;
            await _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
            
            return response.Data;
        }

        public async Task<List<long>> GetChatIdsInRoomAsync(long playerChatId)
        {
            User? user = await _unitOfWork.Users.GetByChatId(playerChatId);
            if(user == null)
                throw new Exception("Користувач не знайдений");
            if (user.RoomId == null)
                throw new Exception("Користувач не перебуває в кімнату");

            List<User> usersInRoom = await _unitOfWork.Users.GetListByRoomId(user.RoomId.Value);

            if(usersInRoom == null || usersInRoom.Count == 0)
                throw new Exception("Користувачі в кімнаті не знайдені");

            return usersInRoom.Select(u => u.ChatId).ToList();
        }
    }
}
