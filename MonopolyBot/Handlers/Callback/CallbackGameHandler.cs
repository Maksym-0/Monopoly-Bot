using MonopolyBot.Application.Service;
using MonopolyBot.Core.Enums;
using MonopolyBot.Core.Interfaces.Services;
using MonopolyBot.Core.Models.Api.DTO.Games;
using MonopolyBot.Telegram.Interfaces.Callback;
using MonopolyBot.Telegram.Interfaces.Services;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace MonopolyBot.Telegram.Handlers.Callback
{
    internal class CallbackGameHandler : ICallbackGameHandler
    {
        private readonly ITelegramBotClient _botClient;
        
        private readonly IContextService _contextService;
        private readonly IMessageFormatter _messageFormatter;

        private readonly IGameService _gameService;

        public CallbackGameHandler(ITelegramBotClient botClient, IGameService gameService, IContextService contextService, 
            IMessageFormatter messageFormatter)
        {
            _botClient = botClient;
            _gameService = gameService;
            _contextService = contextService;
            _messageFormatter = messageFormatter;
        }

        public async Task HandleCallbackGameStatus(long chatId, string data)
        {
            try
            {
                GameStateDto gameResponse = await _gameService.GameStatusAsync(chatId);
                await _botClient.SendMessage(chatId, "Ви приєднались до гри", replyMarkup: KeyboardMarkups.gameKeyboardMarkup);

                await SendGameStatusMessage(chatId, gameResponse);
            }
            catch (UnauthorizedAccessException ex)
            {
                await _botClient.SendMessage(chatId, ex.Message);
                await _botClient.SendMessage(chatId, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(chatId, $"Помилка при отриманні статусу гри: {ex.Message}");
            }
        }
        public async Task HandleCallbackReturnToGame(long chatId, string data)
        {
            try
            {
                Guid id = Guid.Parse(data.Split(':')[1]);

                GameStateDto gameResponse = await _gameService.TryReturnToGameAsync(chatId, id);

                await _contextService.SetStateAsync(chatId, BotState.InGame);
                await _botClient.SendMessage(chatId, "Ви повернулись до гри", replyMarkup: KeyboardMarkups.gameKeyboardMarkup);

                await SendGameStatusMessage(chatId, gameResponse);
            }
            catch (UnauthorizedAccessException ex)
            {
                await _botClient.SendMessage(chatId, ex.Message);
                await _botClient.SendMessage(chatId, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(chatId, $"Помилка при поверненні до гри: {ex.Message}");
            }
        }
        public async Task HandleCallbackWatchGame(long chatId, string data)
        {
            try
            {
                Guid id = Guid.Parse(data.Split(':')[1]);
                await _gameService.JoinWatchGameAsync(chatId, id);

                GameStateDto gameResponse = await _gameService.GameStatusAsync(chatId);
                await _contextService.SetStateAsync(chatId, BotState.WatchingGame);

                await _botClient.SendMessage(chatId, "Ви спостерігаєте за грою", replyMarkup: KeyboardMarkups.watchGameKeyboardMarkup);
                await SendGameStatusMessage(chatId, gameResponse);
            }
            catch (UnauthorizedAccessException ex)
            {
                await _gameService.LeaveWatchGameAsync(chatId);
                await _contextService.ClearContextAsync(chatId);
                await _botClient.SendMessage(chatId, ex.Message);
                await _botClient.SendMessage(chatId, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
            }
            catch (Exception ex)
            {
                await _gameService.LeaveWatchGameAsync(chatId);
                await _contextService.ClearContextAsync(chatId);
                await _botClient.SendMessage(chatId, $"Помилка при спостереженні за грою: {ex.Message}");
            }
        }

        private async Task SendGameStatusMessage(long chatId, GameStateDto gameResponse)
        {
            string boardStatus = _messageFormatter.BuildBoardStatusMessage(gameResponse);
            string playersStatus = _messageFormatter.BuildPlayersStatusMessage(gameResponse);
            await _botClient.SendMessage(chatId, boardStatus, parseMode: ParseMode.Html);
            await _botClient.SendMessage(chatId, playersStatus, parseMode: ParseMode.Html);
        }
    }
}
