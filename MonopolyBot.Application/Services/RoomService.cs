using MonopolyBot.Core.Enums;
using MonopolyBot.Core.Interfaces.Clients;
using MonopolyBot.Core.Interfaces.DataBase.UnitOfWork;
using MonopolyBot.Core.Interfaces.Services;
using MonopolyBot.Core.Models.Api.DTO.Rooms;
using MonopolyBot.Core.Models.Api.Requests;
using MonopolyBot.Core.Models.Api.Responses;
using MonopolyBot.Core.Models.Bot;
using MonopolyBot.Core.Models.Services;

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

        public async Task<ServiceResponse<List<RoomDto>>> GetRoomsAsync(long chatId)
        {
            ServiceResponse<User> userResponse = await _authorization.GetAuthorizedUserAsync(chatId);
            if(!userResponse.Success)
                return new ServiceResponse<List<RoomDto>>
                {
                    Success = false,
                    Message = userResponse.Message,
                    Data = null,
                    ErrorType = userResponse.ErrorType
                };
            User user = userResponse.Data;

            ApiResponse<List<RoomDto>> response = await _roomClient.GetRoomsAsync(user.JWT);
            if (!response.Success)
                return new ServiceResponse<List<RoomDto>>
                {
                    Success = false,
                    Message = response.Message,
                    Data = null,
                    ErrorType = ErrorType.ApiError
                };

            return new ServiceResponse<List<RoomDto>>
            {
                Success = true,
                Message = "Список кімнат отримано успішно",
                Data = response.Data
            };
        }
        public async Task<ServiceResponse<RoomDto>> CreateRoomAsync(long chatId, int maxNumberOfPlayers, string? password)
        {
            ServiceResponse<User> userResponse = await _authorization.GetAuthorizedUserAsync(chatId);
            if (!userResponse.Success)
            {
                return new ServiceResponse<RoomDto>()
                {
                    Success = false,
                    Message = userResponse.Message,
                    Data = null,
                    ErrorType = userResponse.ErrorType
                };
            }
            User user = userResponse.Data;

            ApiResponse<RoomDto> response = await _roomClient.CreateRoomAsync(user.JWT, 
                new CreateRoomRequest
            {
                MaxNumberOfPlayers = maxNumberOfPlayers,
                Password = password
            });

            if (!response.Success)
                return new ServiceResponse<RoomDto>()
                {
                    Success = false,
                    Message = response.Message,
                    Data = null,
                    ErrorType = ErrorType.ApiError
                };

            user.RoomId = response.Data.RoomId;

            await _unitOfWork.SaveChangesAsync();

            return new ServiceResponse<RoomDto>
            {
                Success = true,
                Message = "Кімната створена успішно",
                Data = response.Data
            };
        }
        public async Task<ServiceResponse<JoinRoomDto>> JoinRoomAsync(long chatId, Guid roomId, string? password)
        {
            ServiceResponse<User> userResponse = await _authorization.GetAuthorizedUserAsync(chatId);
            if(!userResponse.Success)
            {
                return new ServiceResponse<JoinRoomDto>()
                {
                    Success = false,
                    Message = userResponse.Message,
                    Data = null,
                    ErrorType = userResponse.ErrorType
                };
            }
            User user = userResponse.Data;

            ApiResponse<JoinRoomDto> response = await _roomClient.JoinRoomAsync(user.JWT, 
                new JoinRoomRequest
            {
                RoomId = roomId,
                Password = password
            });

            if (!response.Success)
                return new ServiceResponse<JoinRoomDto>()
                {
                    Success = false,
                    Message = response.Message,
                    Data = null,
                    ErrorType = ErrorType.ApiError
                };
            
            user.RoomId = roomId;
            user.GameId = response.Data.Room.GameId;

            if (response.Data.IsGameStarted)
            {
                List<User> usersInRoom = await _unitOfWork.Users.GetListByRoomIdAsync(roomId);

                foreach (var player in usersInRoom)
                {
                    player.GameId = response.Data.Room.GameId;
                }
            }

            await _unitOfWork.SaveChangesAsync();

            return new ServiceResponse<JoinRoomDto>
            {
                Success = true,
                Message = "Ви приєднались до кімнати",
                Data = response.Data
            };
        }
        public async Task<ServiceResponse<QuitRoomDto>> QuitRoomAsync(long chatId)
        {
            ServiceResponse<User> userResponse = await _authorization.GetAuthorizedUserAsync(chatId);
            if(!userResponse.Success)
            {
                return new ServiceResponse<QuitRoomDto>()
                {
                    Success = false,
                    Message = userResponse.Message,
                    Data = null,
                    ErrorType = ErrorType.Unauthorized
                };
            }
            User user = userResponse.Data;

            ApiResponse<QuitRoomDto> response = await _roomClient.QuitRoomAsync(user.JWT);

            if (!response.Success)
                return new ServiceResponse<QuitRoomDto>()
                {
                    Success = false,
                    Message = response.Message,
                    Data = null,
                    ErrorType = ErrorType.ApiError
                };

            user.RoomId = null;
            user.GameId = null;
            await _unitOfWork.SaveChangesAsync();
            
            return new ServiceResponse<QuitRoomDto>
            {
                Success = true,
                Message = "Ви покинули кімнату",
                Data= response.Data
            };
        }

        public async Task<ServiceResponse<List<long>>> GetChatIdsInRoomAsync(long playerChatId)
        {
            User? user = await _unitOfWork.Users.GetByChatIdAsync(playerChatId);
            if (user == null)
                return new ServiceResponse<List<long>>()
                {
                    Success = false,
                    Message = "Користувач не знайдений",
                    Data = null,
                    ErrorType = ErrorType.ServiceError
                };
            if (user.RoomId == null)
                return new ServiceResponse<List<long>>()
                {
                    Success = false,
                    Message = "Користувач не перебуває в кімнаті",
                    Data = null,
                    ErrorType = ErrorType.ServiceError
                };

            List<User>? usersInRoom = await _unitOfWork.Users.GetListByRoomIdAsync(user.RoomId.Value);

            if (usersInRoom == null || usersInRoom.Count == 0)
                return new ServiceResponse<List<long>>()
                {
                    Success = false,
                    Message = "Користувачі в кімнаті не знайдені",
                    Data = null,
                    ErrorType = ErrorType.ServiceError
                };

            List<long> chatIds = usersInRoom.Select(u => u.ChatId).ToList();

            return new ServiceResponse<List<long>>()
            {
                Success = true,
                Message = "Id гравців в кімнаті отримано",
                Data = chatIds
            };
        }
        public async Task<ServiceResponse<List<long>>> GetChatIdsByRoomIdAsync(Guid roomId)
        {
            List<User>? usersInRoom = await _unitOfWork.Users.GetListByRoomIdAsync(roomId);

            if (usersInRoom == null || usersInRoom.Count == 0)
            {
                return new ServiceResponse<List<long>>()
                {
                    Success = false,
                    Message = "Користувачі в кімнаті не знайдені",
                    Data = null,
                    ErrorType = ErrorType.ServiceError
                };
            }

            List<long> chatIds = usersInRoom.Select(u => u.ChatId).ToList();

            return new ServiceResponse<List<long>>()
            {
                Success = true,
                Message = "Id гравців в кімнаті отримано",
                Data = chatIds
            };
        }
    }
}
