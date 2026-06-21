using MonopolyBot.Core.Enums;
using MonopolyBot.Core.Interfaces.Services;
using MonopolyBot.Core.Models.Api.DTO.Games;
using MonopolyBot.Core.Models.Services;
using MonopolyBot.Telegram.Interfaces.Command;
using MonopolyBot.Telegram.Interfaces.Services;
using Npgsql.Internal;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MonopolyBot.Telegram.Handlers.Command
{
    internal class GameCommandHandler : IGameCommandHandler
    {
        private readonly ITelegramBotClient _botClient;

        private readonly IBroadcastService  _broadcastService;
        private readonly IContextService _contextService;
        private readonly IMessageFormatter _messageFormatter;

        private readonly IGameService _gameService;

        public GameCommandHandler(ITelegramBotClient botClient, 
            IBroadcastService broadcastService, IContextService contextService, IMessageFormatter messageFormatter, 
            IGameService gameService)
        {
            _botClient = botClient;
            
            _broadcastService = broadcastService;
            _contextService = contextService;
            _messageFormatter = messageFormatter;

            _gameService = gameService;
        }

        public async Task HandleAllGameStatusAsync(Message message)
        {
            try
            {
                ServiceResponse<GameStateDto> response = await _gameService.GameStatusAsync(message.Chat.Id);
                if (!response.Success)
                {
                    await _botClient.SendMessage(message.Chat.Id, response.Message);

                    if(response.ErrorType == ErrorType.Unauthorized)
                    {
                        await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
                    }
                    return;
                }

                List<string> gameStatusMessages = _messageFormatter.BuildAllGameStatusMessages(response.Data);

                foreach (var msg in gameStatusMessages)
                {
                    await _botClient.SendMessage(message.Chat.Id, msg, parseMode: ParseMode.Html);
                }
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(message.Chat.Id, $"Помилка при отриманні статусу гри: {ex.Message}");
            }
        }
        public async Task HandleGameStatusAsync(Message message)
        {
            try
            {
                ServiceResponse<GameStateDto> response = await _gameService.GameStatusAsync(message.Chat.Id);
                if (!response.Success)
                {
                    await _botClient.SendMessage(message.Chat.Id, response.Message);

                    if(response.ErrorType == ErrorType.Unauthorized)
                    {
                        await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
                    }
                    return;
                }

                string boardStatusMessage = _messageFormatter.BuildBoardStatusMessage(response.Data);
                string playersStatusMessage = _messageFormatter.BuildPlayersStatusMessage(response.Data);
                string tradeMessage = _messageFormatter.BuildActiveTradeMessage(response.Data.CurrentTradeOffer);

                await _botClient.SendMessage(message.Chat.Id, boardStatusMessage, parseMode: ParseMode.Html);
                await _botClient.SendMessage(message.Chat.Id, playersStatusMessage, parseMode: ParseMode.Html);
                if (!string.IsNullOrEmpty(tradeMessage))
                {
                    await _botClient.SendMessage(message.Chat.Id, tradeMessage, parseMode: ParseMode.Html);
                }
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(message.Chat.Id, $"Помилка при отриманні статусу гри: {ex.Message}");
            }
        }
        public async Task HandleRollDicesAsync(Message message)
        {
            try
            {
                ServiceResponse<MoveDto> response = await _gameService.RollDiceAsync(message.Chat.Id);
                if (!response.Success)
                {
                    await _botClient.SendMessage(message.Chat.Id, response.Message);

                    if(response.ErrorType == ErrorType.Unauthorized)
                    {
                        await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
                    }
                    return;
                }

                MoveDto result = response.Data;

                string selfMessage = _messageFormatter.BuildSelfMoveMessage(result);
                string othersMessage = _messageFormatter.BuildOthersMoveMessage(result);

                ServiceResponse<List<long>> chatIdsInGame = await _gameService.GetChatIdsInGameAsync(message.Chat.Id);
                if (!chatIdsInGame.Success)
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
                await _botClient.SendMessage(message.Chat.Id, $"Помилка при кидку кубиків: {ex.Message}");
            }
        }
        public async Task HandleBuyAsync(Message message)
        {
            try
            {
                ServiceResponse<BuyDto> response = await _gameService.BuyCellAsync(message.Chat.Id);
                if (!response.Success)
                {
                    await _botClient.SendMessage(message.Chat.Id, response.Message);

                    if(response.ErrorType == ErrorType.Unauthorized)
                    {
                        await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
                    }
                    return;
                }

                BuyDto result = response.Data;

                string selfMessage = _messageFormatter.BuildSelfBuyMessage(result);

                string othersMessage = _messageFormatter.BuildOthersBuyMessage(result);

                ServiceResponse<List<long>> chatIdsInGame = await _gameService.GetChatIdsInGameAsync(message.Chat.Id);
                if (!chatIdsInGame.Success)
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
                await _botClient.SendMessage(message.Chat.Id, $"Помилка при купівлі клітини: {ex.Message}");
            }
        }
        public async Task HandlePayRentAsync(Message message)
        {
            try
            {
                ServiceResponse<PayDto> response = await _gameService.PayRentAsync(message.Chat.Id);
                if(!response.Success)
                {
                    await _botClient.SendMessage(message.Chat.Id, response.Message);

                    if(response.ErrorType == ErrorType.Unauthorized)
                    {
                        await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
                    }
                    return;
                }

                PayDto result = response.Data;

                string selfMessage = _messageFormatter.BuildSelfPayMessage(result);
                string othersMessage = _messageFormatter.BuildOthersPayMessage(result);

                ServiceResponse<List<long>> chatIdsInGame = await _gameService.GetChatIdsInGameAsync(message.Chat.Id);
                if (!chatIdsInGame.Success)
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
                await _botClient.SendMessage(message.Chat.Id, $"Помилка при оплаті: {ex.Message}");
            }
        }
        public async Task HandlePayToLeavePrisonAsync(Message message)
        {
            try
            {
                ServiceResponse<PayDto> response = await _gameService.PayToLeavePrisonAsync(message.Chat.Id);
                if(!response.Success)
                {
                    await _botClient.SendMessage(message.Chat.Id, response.Message);

                    if(response.ErrorType == ErrorType.Unauthorized)
                    {
                        await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
                    }
                    return;
                }

                PayDto result = response.Data;

                string selfMessage = _messageFormatter.BuildSelfPayMessage(result);
                string othersMessage = _messageFormatter.BuildOthersPayMessage(result);

                ServiceResponse<List<long>> chatIdsInGame = await _gameService.GetChatIdsInGameAsync(message.Chat.Id);

                await _broadcastService.SendPersonalizedMessageAsync(message.Chat.Id, selfMessage, chatIdsInGame.Data, othersMessage);
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(message.Chat.Id, $"Помилка при оплаті: {ex.Message}");
            }
        }
        public async Task HandleLevelUpAsync(Message message)
        {
            try
            {
                await _contextService.SetStateAsync(message.Chat.Id, BotState.AwaitingLevelUpCell);
                await _botClient.SendMessage(message.Chat.Id, "Підвищення рівня клітини розпочато. Введіть номер клітини для підвищення рівня:");
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(message.Chat.Id, $"Помилка при підвищенні рівня клітини: {ex.Message}");
            }
        }
        public async Task HandleLevelDownAsync(Message message)
        {
            try
            {
                await _contextService.SetStateAsync(message.Chat.Id, BotState.AwaitingLevelDownCell);
                await _botClient.SendMessage(message.Chat.Id, "Зниження рівня клітини розпочато. Введіть номер клітини для зниження рівня:");
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(message.Chat.Id, $"Помилка при зниженні рівня клітини: {ex.Message}");
            }
        }
        public async Task HandleEndActionAsync(Message message)
        {
            try
            {
                ServiceResponse<NextActionDto> response = await _gameService.EndActionAsync(message.Chat.Id);
                if (!response.Success)
                {
                    await _botClient.SendMessage(message.Chat.Id, response.Message);

                    if(response.ErrorType == ErrorType.Unauthorized)
                    {
                        await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
                    }
                    return;
                }

                NextActionDto result = response.Data;

                string selfMessage =
                    $"Ваша дія завершена. Наступним ходить {result.NewPlayerName}. Перевірте статус гри";

                string othersMessage =
                    $"{result.PlayerName} завершив свою дію. Наступним ходить {result.NewPlayerName}. Перевірте статус гри";

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
                await _botClient.SendMessage(message.Chat.Id, $"Помилка при завершенні дії: {ex.Message}");
            }
        }
        public async Task HandleLeaveGameAsync(Message message)
        {
            try
            {
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

                ServiceResponse<LeaveGameDto> result = await _gameService.LeaveGameAsync(message.Chat.Id);
                if(!chatIdsInGame.Success)
                {
                    await _botClient.SendMessage(message.Chat.Id, result.Message);

                    if(result.ErrorType == ErrorType.Unauthorized)
                    {
                        await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
                    }
                    return;
                }

                await _broadcastService.SendPlayerLeaveAsync(chatIdsInGame.Data, message.Chat.Id, result.Data);

            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(message.Chat.Id, $"Помилка при виході з гри: {ex.Message}");
            }
        }
        public async Task HandleEndWatchGameAsync(Message message)
        {
            try
            {
                ServiceResponse<bool> result = await _gameService.LeaveWatchGameAsync(message.Chat.Id);
                if (!result.Success)
                {
                    await _botClient.SendMessage(message.Chat.Id, result.Message);
                    
                    if (result.ErrorType == ErrorType.Unauthorized)
                    {
                        await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
                    }
                    return;
                }

                await _contextService.SetStateAsync(message.Chat.Id, BotState.None);
                await _botClient.SendMessage(message.Chat.Id, "Ви припинили спостереження за грою.", replyMarkup: KeyboardMarkups.roomsKeyboardMarkup);
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(message.Chat.Id, $"Помилка при припиненні спостереження за грою: {ex.Message}");
            }
        }
    }
}
