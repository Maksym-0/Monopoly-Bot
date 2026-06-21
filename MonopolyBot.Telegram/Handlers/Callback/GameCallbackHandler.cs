using MonopolyBot.Core.Enums;
using MonopolyBot.Core.Interfaces.Services;
using MonopolyBot.Core.Models.Api.DTO.Games;
using MonopolyBot.Core.Models.Services;
using MonopolyBot.Telegram.Interfaces.Callback;
using MonopolyBot.Telegram.Interfaces.Services;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace MonopolyBot.Telegram.Handlers.Callback
{
    internal class GameCallbackHandler : IGameCallbackHandler
    {
        private readonly ITelegramBotClient _botClient;
        
        private readonly IContextService _contextService;
        private readonly IMessageFormatter _messageFormatter;

        private readonly IGameService _gameService;

        public GameCallbackHandler(ITelegramBotClient botClient, IGameService gameService, IContextService contextService, 
            IMessageFormatter messageFormatter)
        {
            _botClient = botClient;
            _gameService = gameService;
            _contextService = contextService;
            _messageFormatter = messageFormatter;
        }

        public async Task HandleGameStatusAsync(long chatId, string data)
        {
            try
            {
                ServiceResponse<GameStateDto> response = await _gameService.GameStatusAsync(chatId);
                if (!response.Success)
                {
                    _botClient.SendMessage(chatId, response.Message);

                    if (response.ErrorType == ErrorType.Unauthorized)
                    {
                        await _botClient.SendMessage(chatId, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
                    }
                    return;
                }

                await _botClient.SendMessage(chatId, "Ви приєднались до гри", replyMarkup: KeyboardMarkups.gameKeyboardMarkup);

                await SendGameStatusMessageAsync(chatId, response.Data);
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(chatId, $"Помилка при отриманні статусу гри: {ex.Message}");
            }
        }
        public async Task HandleReturnToGameAsync(long chatId, string data)
        {
            try
            {
                Guid id = Guid.Parse(data.Split(':')[1]);

                ServiceResponse<GameStateDto> response = await _gameService.ReturnToGameAsync(chatId, id);
                if (!response.Success)
                {
                    await _botClient.SendMessage(chatId, response.Message);

                    if(response.ErrorType == ErrorType.Unauthorized)
                    {
                        await _botClient.SendMessage(chatId, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
                    }
                    return;
                }

                await _contextService.SetStateAsync(chatId, BotState.InGame);
                await _botClient.SendMessage(chatId, "Ви повернулись до гри", replyMarkup: KeyboardMarkups.gameKeyboardMarkup);

                await SendGameStatusMessageAsync(chatId, response.Data);
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(chatId, $"Помилка при поверненні до гри: {ex.Message}");
            }
        }
        public async Task HandleWatchGameAsync(long chatId, string data)
        {
            try
            {
                Guid id = Guid.Parse(data.Split(':')[1]);
                ServiceResponse<GameStateDto> response = await _gameService.JoinWatchGameAsync(chatId, id);
                if (!response.Success)
                {
                    await _contextService.ClearContextAsync(chatId);
                    await _botClient.SendMessage(chatId, response.Message);
                    if(response.ErrorType == ErrorType.Unauthorized)
                    {
                        await _botClient.SendMessage(chatId, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
                    }
                    return;
                }

                await _botClient.SendMessage(chatId, "Ви спостерігаєте за грою", replyMarkup: KeyboardMarkups.watchGameKeyboardMarkup);
                await SendGameStatusMessageAsync(chatId, response.Data);
            }
            catch (Exception ex)
            {
                await _contextService.ClearContextAsync(chatId);
                await _botClient.SendMessage(chatId, $"Помилка при спостереженні за грою: {ex.Message}");
            }
        }

        private async Task SendGameStatusMessageAsync(long chatId, GameStateDto gameState)
        {
            string boardStatus = _messageFormatter.BuildBoardStatusMessage(gameState);
            string playersStatus = _messageFormatter.BuildPlayersStatusMessage(gameState);
            await _botClient.SendMessage(chatId, boardStatus, parseMode: ParseMode.Html);
            await _botClient.SendMessage(chatId, playersStatus, parseMode: ParseMode.Html);
        }
    }
}
