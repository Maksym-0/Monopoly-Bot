using MonopolyBot.Core.Enums;
using MonopolyBot.Core.Interfaces.Services;
using MonopolyBot.Core.Models.Api.DTO.Rooms;
using MonopolyBot.Core.Models.Services;
using MonopolyBot.Telegram.Interfaces.Command;
using MonopolyBot.Telegram.Interfaces.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MonopolyBot.Telegram.Handlers.Command
{
    internal class RoomCommandHandler : IRoomCommandHandler
    {
        private readonly ITelegramBotClient _botClient;

        private readonly IContextService _contextService;
        private readonly IMessageFormatter _messageFormatter;

        private readonly IRoomService _roomService;
        private readonly IAccountService _accService;

        public RoomCommandHandler(ITelegramBotClient botClient, 
            IContextService contextService, IMessageFormatter messageFormatter, 
            IRoomService roomService, IAccountService accService)
        {
            _botClient = botClient;
            _contextService = contextService;
            _messageFormatter = messageFormatter;
            _roomService = roomService;
            _accService = accService;
        }

        public async Task HandleGetRooms(Message message)
        {
            try
            {
                ServiceResponse<List<RoomDto>> response = await _roomService.GetRoomsAsync(message.Chat.Id);
                if (!response.Success)
                {
                    await _contextService.SetStateAsync(message.Chat.Id, BotState.None);
                    await _botClient.SendMessage(message.Chat.Id, response.Message);

                    if(response.ErrorType == ErrorType.Unauthorized)
                    {
                        await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
                    }
                    return;
                }

                List<RoomDto> rooms = response.Data;

                if (rooms.Count == 0)
                {
                    await _botClient.SendMessage(message.Chat.Id, "Немає доступних кімнат.");
                }
                else
                {
                    await _botClient.SendMessage(message.Chat.Id, "Доступні кімнати:");

                    ServiceResponse<ProfileInfo> userResponse = await _accService.GetMyDataAsync(message.Chat.Id);
                    if (!userResponse.Success)
                    {
                        await _contextService.SetStateAsync(message.Chat.Id, BotState.None);
                        await _botClient.SendMessage(message.Chat.Id, userResponse.Message);

                        if(userResponse.ErrorType == ErrorType.Unauthorized)
                        {
                            await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
                        }
                        return;
                    }

                    foreach (var room in rooms)
                    {
                        bool playerInside = false;
                        InlineKeyboardMarkup keyboardMarkup;
                        foreach (var player in room.Players)
                        {
                            if (player.AccountId == userResponse.Data.AccountId)
                                playerInside = true;
                        }
                        if (playerInside && room.InGame)
                        {
                            keyboardMarkup = new
                            (
                                InlineKeyboardButton.WithCallbackData("Return To Game", $"ReturnToGame:{room.GameId}"),
                                InlineKeyboardButton.WithCallbackData("Leave", $"LeaveRoom:{room.RoomId}")
                            );
                        }
                        else if (playerInside)
                        {
                            keyboardMarkup = new
                            (
                                InlineKeyboardButton.WithCallbackData("Leave", $"LeaveRoom:{room.RoomId}")
                            );
                        }
                        else if (room.InGame)
                        {
                            keyboardMarkup = new
                            (
                                InlineKeyboardButton.WithCallbackData("Watch Game", $"WatchGame:{room.RoomId}")
                            );
                        }
                        else
                        {
                            keyboardMarkup = new
                            (
                                InlineKeyboardButton.WithCallbackData("Join", $"JoinRoom:{room.RoomId}:{room.HavePassword}")
                            );
                        }
                        string text = _messageFormatter.BuildRoomMessage(room);
                        await _botClient.SendMessage(message.Chat.Id, text, replyMarkup: keyboardMarkup);
                        await Task.Delay(100);
                    }
                }

            }
            catch (Exception ex)
            {
                await _contextService.SetStateAsync(message.Chat.Id, BotState.None);
                await _botClient.SendMessage(message.Chat.Id, $"Помилка при отриманні кімнат: {ex.Message}");
                return;
            }
        }

        public async Task HandleCreateRoom(Message message)
        {
            await _contextService.SetStateAsync(message.Chat.Id, BotState.AwaitingCreateRoom);
            await _botClient.SendMessage(message.Chat.Id, "Створення кімнати розпочато. Введіть максимальну кількість гравців (2-4):");
        }

        public async Task HandleAccountsMenu(Message message)
        {
            await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
        }
    }
}
