using Telegram.Bot.Types;
using Telegram.Bot;
using MonopolyBot.Telegram.Interfaces.Status;
using MonopolyBot.Core.Interfaces.Services;
using MonopolyBot.Core.Models.Bot;
using MonopolyBot.Core.Models.Api.DTO.Rooms;
using MonopolyBot.Telegram.Interfaces.Services;
using MonopolyBot.Core.Enums;
using MonopolyBot.Core.Models.Services;

namespace MonopolyBot.Telegram.Handlers.Status
{
    internal class RoomStatusHandler : IRoomStatusHandler
    {
        private readonly ITelegramBotClient _botClient;

        private readonly IContextService _contextService;
        private readonly IBroadcastService _broadcastService;

        private readonly IRoomService _roomService;

        public RoomStatusHandler(ITelegramBotClient botClient, IContextService contextService, IBroadcastService broadcastService, IRoomService roomService)
        {
            _botClient = botClient;
            _contextService = contextService;
            _broadcastService = broadcastService;
            _roomService = roomService;
        }

        public async Task HandleJoinAsync(Message message, ChatStatus status)
        {
            try
            {
                ServiceResponse<JoinRoomDto> response = await _roomService.JoinRoomAsync(message.Chat.Id, status.RoomId.Value, message.Text);
                if(!response.Success)
                {
                    status.Status = BotState.None;
                    await _botClient.SendMessage(message.Chat.Id, response.Message);

                    if(response.ErrorType == ErrorType.Unauthorized)
                    {
                        await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
                    }
                    return;
                }

                await _botClient.SendMessage(message.Chat.Id, $"Ви приєдналися до кімнати {response.Data.Room.RoomId}.");
                if (response.Data.IsGameStarted)
                {
                    ServiceResponse<List<long>> chatIds = await _roomService.GetChatIdsInRoomAsync(message.Chat.Id);
                    if(!chatIds.Success)
                    {
                        status.Status = BotState.None;
                        await _botClient.SendMessage(message.Chat.Id, chatIds.Message);

                        if(chatIds.ErrorType == ErrorType.Unauthorized)
                        {
                            await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
                        }
                        return;
                    }
                    await _broadcastService.SendGameStartAsync(chatIds.Data, response.Data.Room.RoomId);
                }
                status.Status = BotState.InRoom;
            }
            catch (Exception ex)
            {
                status.Status = BotState.None;
                await _botClient.SendMessage(message.Chat.Id, $"Помилка при приєднанні до кімнати: {ex.Message}");
            }
            finally
            {
                await _contextService.UpdateContextDataAsync(status);
            }
        }
        public async Task HandleCreateAsync(Message message, ChatStatus status)
        {
            try
            {
                if (status.MaxNumberOfPlayers == null)
                {
                    if (!int.TryParse(message.Text, out int maxNumberOfPlayers))
                    {
                        await _botClient.SendMessage(message.Chat.Id, "⚠️ Введіть коректное число для максимальної кількости гравців:");
                        return;
                    }

                    if (maxNumberOfPlayers > 4 || maxNumberOfPlayers < 2)
                    {
                        await _botClient.SendMessage(message.Chat.Id, "❌ Кількість гравців повинна бути від 2 до 4. Спробуйте ще раз:");
                        return;
                    }

                    status.MaxNumberOfPlayers = maxNumberOfPlayers;
                    await _contextService.UpdateContextDataAsync(status);

                    await _botClient.SendMessage(message.Chat.Id, "Оберіть тип кімнати:", replyMarkup: KeyboardMarkups.createRoomInlineMarkup);
                }
                else
                {
                    await _botClient.SendMessage(message.Chat.Id, "Оберіть тип кімнати:", replyMarkup: KeyboardMarkups.createRoomInlineMarkup);
                }
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(message.Chat.Id, $"Помилка при створенні кімнати: {ex.Message}");
                return;
            }
        }
        public async Task HandleCreatePasswordAsync(Message message, ChatStatus status)
        {
            try
            {
                ServiceResponse<RoomDto> response = await _roomService.CreateRoomAsync(message.Chat.Id, status.MaxNumberOfPlayers.Value, message.Text);
                if(!response.Success)
                {
                    status.Status = BotState.None;
                    await _botClient.SendMessage(message.Chat.Id, response.Message);

                    if(response.ErrorType == ErrorType.Unauthorized)
                    {
                        await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
                    }
                    return;
                }

                await _botClient.SendMessage(message.Chat.Id, $"Кімната {response.Data.RoomId} створена.");
                status.Status = BotState.InRoom;
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(message.Chat.Id, $"Помилка при створенні кімнати: {ex.Message}");
                status.Status = BotState.None;
                return;
            }
            finally
            {
                await _contextService.UpdateContextDataAsync(status);
            }
        }
    }
}
