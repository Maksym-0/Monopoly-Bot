using Telegram.Bot.Types;
using Telegram.Bot;
using MonopolyBot.Telegram.Interfaces.Status;
using MonopolyBot.Core.Models.Bot;
using MonopolyBot.Core.Models.Api.DTO.Games;
using MonopolyBot.Core.Interfaces.Services;
using MonopolyBot.Telegram.Interfaces.Services;
using MonopolyBot.Core.Enums;
using MonopolyBot.Core.Models.Services;

namespace MonopolyBot.Telegram.Handlers.Status
{
    internal class GameStatusHandler : IGameStatusHandler
    {
        ITelegramBotClient _botClient;

        IContextService _contextService;
        IMessageFormatter _messageFormatter;
        IBroadcastService _broadcastService;

        IGameService _gameService;

        public GameStatusHandler(ITelegramBotClient botClient, IContextService contextService, IMessageFormatter messageFormatter, IBroadcastService broadcastService, IGameService gameService)
        {
            _botClient = botClient;
            _contextService = contextService;
            _messageFormatter = messageFormatter;
            _broadcastService = broadcastService;
            _gameService = gameService;
        }

        public async Task HandleLevelUpStatus(Message message, ChatStatus status)
        {
            try
            {
                if (!int.TryParse(message.Text, out int cellNumber))
                {
                    await _botClient.SendMessage(message.Chat.Id, "⚠️ Будь ласка, введіть коректний номер клітини.");
                    return;
                }

                ServiceResponse<LevelChangeDto> response = await _gameService.LevelUpCellAsync(message.Chat.Id, cellNumber);
                if (!response.Success)
                {
                    await _botClient.SendMessage(message.Chat.Id, response.Message);

                    if(response.ErrorType == ErrorType.Unauthorized)
                    {
                        await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
                    }
                    return;
                }

                LevelChangeDto result = response.Data;

                string selfMessage =_messageFormatter.BuildSelfLevelChangeMessage(result);
                string othersMessage = _messageFormatter.BuildOthersLevelChangeMessage(result);
                
                ServiceResponse<List<long>> chatIdsInGame = await _gameService.GetChatIdsInGameAsync(message.Chat.Id);
                if(!chatIdsInGame.Success)
                {
                    await _botClient.SendMessage(message.Chat.Id, chatIdsInGame.Message);

                    if(chatIdsInGame.ErrorType == ErrorType.Unauthorized)
                    {
                        await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
                    }
                    return;
                }

                await _broadcastService.SendPersonalizedMessageAsync(message.Chat.Id, selfMessage, chatIdsInGame.Data, othersMessage);
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(message.Chat.Id, $"Помилка при підвищенні рівня клітини: {ex.Message}");
            }
            finally
            {
                status.Status = BotState.InGame;
                await _contextService.UpdateContextDataAsync(status);
            }
        }
        public async Task HandleLevelDownStatus(Message message, ChatStatus status)
        {
            try
            {
                if (!int.TryParse(message.Text, out int cellNumber))
                {
                    await _botClient.SendMessage(message.Chat.Id, "⚠️ Будь ласка, введіть коректний номер клітини.");
                    return;
                }

                ServiceResponse<LevelChangeDto> response = await _gameService.LevelDownCellAsync(message.Chat.Id, cellNumber);
                if (!response.Success)
                {
                    await _botClient.SendMessage(message.Chat.Id, response.Message);

                    if(response.ErrorType == ErrorType.Unauthorized)
                    {
                        await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
                    }
                    return;
                }

                LevelChangeDto result = response.Data;

                string selfMessage = _messageFormatter.BuildSelfLevelChangeMessage(result);
                string othersMessage = _messageFormatter.BuildOthersLevelChangeMessage(result);

                ServiceResponse<List<long>> chatIdsInGame = await _gameService.GetChatIdsInGameAsync(message.Chat.Id);
                if(!chatIdsInGame.Success)
                {
                    await _botClient.SendMessage(message.Chat.Id, chatIdsInGame.Message);

                    if(chatIdsInGame.ErrorType == ErrorType.Unauthorized)
                    {
                        await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
                    }
                    return;
                }

                await _broadcastService.SendPersonalizedMessageAsync(message.Chat.Id, selfMessage, chatIdsInGame.Data, othersMessage);
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(message.Chat.Id, $"Помилка при зниженні рівня клітини: {ex.Message}");
            }
            finally
            {
                status.Status = BotState.InGame;
                await _contextService.UpdateContextDataAsync(status);
            }
        }
    }
}
