using Telegram.Bot.Types;
using Telegram.Bot;
using MonopolyBot.Telegram.Interfaces.Status;
using MonopolyBot.Core.Interfaces.Services;
using MonopolyBot.Core.Models.Bot;
using MonopolyBot.Core.Models.Api.DTO.Rooms;
using MonopolyBot.Telegram.Interfaces.Services;
using MonopolyBot.Core.Enums;

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

        public async Task HandleJoinRoomStatus(Message message, ChatStatus status)
        {
            try
            {
                JoinRoomDto roomResponse = await _roomService.JoinRoomAsync(message.Chat.Id, status.RoomId.Value, message.Text);
                await _botClient.SendMessage(message.Chat.Id, $"Ви приєдналися до кімнати {roomResponse.Room.RoomId}.");
                if (roomResponse.IsGameStarted)
                {
                    List<long> chatIds = await _roomService.GetChatIdsInRoomAsync(message.Chat.Id);
                    await _broadcastService.SendGameStartAsync(chatIds, roomResponse.Room.RoomId);
                }
                status.Status = BotState.InRoom;
            }
            catch (UnauthorizedAccessException ex)
            {
                status.Status = BotState.None;
                await _botClient.SendMessage(message.Chat.Id, ex.Message);
                await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
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
        public async Task HandleCreateRoomStatus(Message message, ChatStatus status)
        {
            if (status.MaxNumberOfPlayers == null)
            {
                try
                {
                    int maxNumberOfPlayers = Convert.ToInt32(message.Text);
                    if (maxNumberOfPlayers > 4 || maxNumberOfPlayers < 2)
                    {
                        await _botClient.SendMessage(message.Chat.Id, "❌ Кількість гравців повинна бути від 2 до 4. Спробуйте ще раз:");
                        return;
                    }

                    status.MaxNumberOfPlayers = maxNumberOfPlayers;
                    await _contextService.UpdateContextDataAsync(status);

                    await _botClient.SendMessage(message.Chat.Id, "Оберіть тип кімнати:", replyMarkup: KeyboardMarkups.createRoomInlineMarkup);
                }
                catch (FormatException)
                {
                    await _botClient.SendMessage(message.Chat.Id, "⚠️ Введіть коректне число для максимальної кількості гравців:");
                    return;
                }
            }
            else
            {
                await _botClient.SendMessage(message.Chat.Id, "Оберіть тип кімнати:", replyMarkup: KeyboardMarkups.createRoomInlineMarkup);
            }
        }
        public async Task HandleCreateRoomPasswordStatus(Message message, ChatStatus status)
        {
            try
            {
                RoomDto roomResponse = await _roomService.CreateRoomAsync(message.Chat.Id, status.MaxNumberOfPlayers.Value, message.Text);
                await _botClient.SendMessage(message.Chat.Id, $"Кімната {roomResponse.RoomId} створена.");
                status.Status = BotState.InRoom;
            }
            catch (UnauthorizedAccessException ex)
            {
                await _botClient.SendMessage(message.Chat.Id, ex.Message);
                await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
                status.Status = BotState.None;
                return;
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
