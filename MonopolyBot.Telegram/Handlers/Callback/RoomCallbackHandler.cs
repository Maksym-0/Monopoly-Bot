using MonopolyBot.Core.Enums;
using MonopolyBot.Core.Interfaces.Services;
using MonopolyBot.Core.Models.Api.DTO.Rooms;
using MonopolyBot.Core.Models.Bot;
using MonopolyBot.Core.Models.Services;
using MonopolyBot.Telegram.Interfaces.Callback;
using MonopolyBot.Telegram.Interfaces.Services;
using Telegram.Bot;

namespace MonopolyBot.Telegram.Handlers.Callback
{
    internal class RoomCallbackHandler : IRoomCallbackHandler
    {
        ITelegramBotClient _botClient;

        IContextService _contextService;
        IBroadcastService _broadcastService;

        IRoomService _roomService;
        IGameService _gameService;

        public RoomCallbackHandler(ITelegramBotClient botClient, IContextService contextService, IBroadcastService broadcastService, 
            IRoomService roomService, IGameService gameService)
        {
            _botClient = botClient;
            _contextService = contextService;
            _broadcastService = broadcastService;
            _roomService = roomService;
            _gameService = gameService;
        }

        public async Task HandleJoinRoomAsync(long chatId, string data)
        {
            Guid id = Guid.Parse(data.Split(':')[1]);
            string passwordStatus = data.Split(':')[2];

            if (passwordStatus == "True")
            {
                ChatStatus status = new ChatStatus(chatId, BotState.AwaitingJoinRoom) { RoomId = id};
                
                await _contextService.UpdateContextDataAsync(status);
                await _botClient.SendMessage(chatId, "Введіть пароль для приєднання до кімнати:");
            }
            else
            {
                try
                {
                    ServiceResponse<JoinRoomDto> joinRoomResponse = await _roomService.JoinRoomAsync(chatId, id, null);
                    if (!joinRoomResponse.Success)
                    {
                        await _botClient.SendMessage(chatId, joinRoomResponse.Message);

                        if(joinRoomResponse.ErrorType == ErrorType.Unauthorized)
                        {
                            await _botClient.SendMessage(chatId, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
                        }
                        return;
                    }
                    
                    await _botClient.SendMessage(chatId, $"Ви приєдналися до кімнати {joinRoomResponse.Data.Room.RoomId}.");

                    ChatStatus status;
                    if (joinRoomResponse.Data.IsGameStarted)
                    {
                        status = new ChatStatus(chatId, BotState.InGame);
                        ServiceResponse<List<long>> chatIdsResponse = await _gameService.GetChatIdsInGameAsync(chatId);
                        if (!chatIdsResponse.Success)
                        {
                            await _botClient.SendMessage(chatId, chatIdsResponse.Message);

                            if(chatIdsResponse.ErrorType == ErrorType.Unauthorized)
                            {
                                await _botClient.SendMessage(chatId, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
                            }
                            return;
                        }

                        await _broadcastService.SendGameStartAsync(chatIdsResponse.Data, joinRoomResponse.Data.Room.RoomId);
                    }
                    else
                    {
                        status = new ChatStatus(chatId, BotState.InRoom);
                    }
                    await _contextService.UpdateContextDataAsync(status);
                }
                catch (Exception ex)
                {
                    await _botClient.SendMessage(chatId, $"Помилка при приєднанні до кімнати: {ex.Message}");
                }
            }
        }
        public async Task HandleCreateRoomAsync(long chatId, string data)
        {
            string passwordStatus = data.Split(':')[1];
            string? password;
            ChatStatus? status = await _contextService.GetStatusAsync(chatId);
            if (status == null)
            {
                await _botClient.SendMessage(chatId, "Ви не перебуваєте у процесі створення кімнати");
                await _contextService.SetStateAsync(chatId, BotState.None);
                return;
            }
            if (status.MaxNumberOfPlayers == null)
            {
                await _botClient.SendMessage(chatId, "Невизначена кількість гравців для кімнати. Будь ласка, спробуйте створити кімнату знову");
                await _contextService.SetStateAsync(chatId, BotState.None);
                return;
            }

            try
            {
                if (passwordStatus == "set")
                {
                    status.Status = BotState.AwaitingCreateRoomPassword;
                    await _contextService.UpdateContextDataAsync(status);
                    await _botClient.SendMessage(chatId, "Введіть пароль для кімнати:");
                }
                else
                if (passwordStatus == "null")
                {
                    password = null;

                    ServiceResponse<RoomDto> roomResponse = await _roomService.CreateRoomAsync(chatId, status.MaxNumberOfPlayers.Value, password);
                    if(!roomResponse.Success)
                    {
                        await _contextService.SetStateAsync(chatId, BotState.None);
                        await _botClient.SendMessage(chatId, roomResponse.Message);

                        if(roomResponse.ErrorType == ErrorType.Unauthorized)
                        {
                            await _botClient.SendMessage(chatId, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
                        }
                        return;
                    }

                        status.ClearRoomState();
                    status.Status = BotState.InRoom;
                    await _contextService.UpdateContextDataAsync(status);

                    await _botClient.SendMessage(chatId, $"Кімната {roomResponse.Data.RoomId} створена.");
                }
            }
            catch (Exception ex)
            {
                await _contextService.SetStateAsync(chatId, BotState.None);
                await _botClient.SendMessage(chatId, $"Помилка при створенні кімнати: {ex.Message}");
                return;
            }
        }
        public async Task HandleLeaveRoomAsync(long chatId, string data)
        {
            try
            {
                Guid roomId = Guid.Parse(data.Split(":")[1]);
                ChatStatus? userStatus = await _contextService.GetStatusAsync(chatId);

                List<long> playersToNotify = new List<long>();
                
                ServiceResponse<List<long>> roomChatIdsResponse = await _roomService.GetChatIdsByRoomIdAsync(roomId);
                if (roomChatIdsResponse.Success)
                {
                    playersToNotify.AddRange(roomChatIdsResponse.Data);
                }
                else if (roomChatIdsResponse.ErrorType == ErrorType.Unauthorized)
                {
                    await _botClient.SendMessage(chatId, roomChatIdsResponse.Message);
                    await _botClient.SendMessage(chatId, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
                    return;
                }

                ServiceResponse<List<long>> gameChatIdsResponse = await _gameService.GetChatIdsInGameAsync(chatId);
                if (gameChatIdsResponse.Success)
                {
                    playersToNotify.AddRange(gameChatIdsResponse.Data);
                }
                else if (gameChatIdsResponse.ErrorType == ErrorType.Unauthorized)
                {
                    await _botClient.SendMessage(chatId, gameChatIdsResponse.Message);
                    await _botClient.SendMessage(chatId, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
                    return;
                }

                playersToNotify = playersToNotify.Distinct().ToList();

                ServiceResponse<QuitRoomDto> result = await _roomService.QuitRoomAsync(chatId);
                if (!result.Success)
                {
                    await _botClient.SendMessage(chatId, result.Message);

                    if (result.ErrorType == ErrorType.Unauthorized)
                    {
                        await _botClient.SendMessage(chatId, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
                    }
                    return;
                }

                if (result.Data.LeaveGameDto != null)
                {
                    if(playersToNotify.Count > 0)
                        await _broadcastService.SendPlayerLeaveAsync(playersToNotify, chatId, result.Data.LeaveGameDto);
                }
                else
                {
                    if (result.Data.IsRoomDeleted)
                    {
                        await _botClient.SendMessage(chatId, "Ви вийшли з кімнати. Кімната була видалена, оскільки ви були останнім гравцем.");
                    }
                    else
                    {
                        await _botClient.SendMessage(chatId, "Ви вийшли з кімнати.");
                    }
                }

                if (userStatus != null)
                {
                    userStatus.ClearRoomState();
                    await _contextService.UpdateContextDataAsync(userStatus);
                }
                else
                {
                    await _contextService.SetStateAsync(chatId, BotState.None);
                }
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(chatId, $"Помилка при виході з кімнати: {ex.Message}");
            }
        }
    }
}
