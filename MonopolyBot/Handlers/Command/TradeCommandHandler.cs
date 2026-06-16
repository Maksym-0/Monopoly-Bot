using MonopolyBot.Core.Enums;
using MonopolyBot.Core.Interfaces.Services;
using MonopolyBot.Core.Models.Api.DTO.Games;
using MonopolyBot.Core.Models.Bot;
using MonopolyBot.Core.Models.Services;
using MonopolyBot.Telegram.Interfaces.Command;
using MonopolyBot.Telegram.Interfaces.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MonopolyBot.Telegram.Handlers.Command
{
    internal class TradeCommandHandler : ITradeCommandHandler
    {
        private readonly ITelegramBotClient _botClient;

        private readonly IContextService _contextService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IMessageFormatter _messageFormatter;
        private readonly IBroadcastService _broadcastService;

        private readonly IGameService _gameService;
        private readonly ITradingService _tradingService;

        public TradeCommandHandler(ITelegramBotClient telegramBotClient, IContextService contextService, IAuthorizationService authorizationService, IGameService gameService, ITradingService tradingService)
        {
            _botClient = telegramBotClient;
            _contextService = contextService;
            _authorizationService = authorizationService;
            _gameService = gameService;
            _tradingService = tradingService;
        }

        public async Task HandleStartTradeAsync(Message message)
        {
            try
            {
                ServiceResponse<Core.Models.Bot.User> userResponse = await _authorizationService.GetPlayerInGameAsync(message.Chat.Id);
                if (!userResponse.Success)
                {
                    await _botClient.SendMessage(message.Chat.Id, "Ви повинні бути в грі, щоб почати торгівлю.");
                    await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
                    return;
                }
                string userName = userResponse.Data.Name;

                ServiceResponse<GameStateDto> response = await _gameService.GameStatusAsync(message.Chat.Id);
                if (!response.Success)
                {
                    await _botClient.SendMessage(message.Chat.Id, response.Message);

                    if (response.ErrorType == ErrorType.Unauthorized)
                    {
                        await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
                    }
                    return;
                }

                List<InlineKeyboardButton[]> buttons = new List<InlineKeyboardButton[]>();

                foreach (PlayerDto player in response.Data.Players)
                {
                    if (player.InGame && player.Name != userName)
                    {
                        InlineKeyboardButton button = InlineKeyboardButton.WithCallbackData(player.Name, $"Trade:{player.Name}:{player.Id}");
                        buttons.Add(new[] { button });
                    }
                }

                InlineKeyboardMarkup keyboardMarkup = new InlineKeyboardMarkup(buttons);

                await _contextService.SetStateAsync(message.Chat.Id, BotState.AwaitingOfferee);

                await _botClient.SendMessage(message.Chat.Id, "Будь ласка, оберіть ім'я гравця, з яким ви хочете почати торгівлю.", replyMarkup: keyboardMarkup);
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(message.Chat.Id, $"Помилка під час початку торгівлі: {ex.Message}");
            }
        }
        public async Task HandleAcceptTradeAsync(Message message)
        {
            try
            {
                ServiceResponse<AcceptTradeDto> response = await _tradingService.AcceptTradeAsync(message.Chat.Id);
                if (!response.Success)
                {
                    await _botClient.SendMessage(message.Chat.Id, response.Message);
                    if (response.ErrorType == ErrorType.Unauthorized)
                    {
                        await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
                    }
                    return;
                }

                string selfMessage = _messageFormatter.BuildSelfAcceptTradeMessage(response.Data);
                string othersMessage = _messageFormatter.BuildOthersAcceptTradeMessage(response.Data);

                ServiceResponse<List<long>> chatIdsInGame = await _gameService.GetChatIdsInGameAsync(message.Chat.Id);
                if (!chatIdsInGame.Success)
                {
                    await _botClient.SendMessage(message.Chat.Id, chatIdsInGame.Message);

                    if (chatIdsInGame.ErrorType == ErrorType.Unauthorized)
                    {
                        await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
                    }
                    return;
                }
                await _broadcastService.SendPersonalizedMessageAsync(message.Chat.Id, selfMessage, chatIdsInGame.Data, othersMessage);
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(message.Chat.Id, $"Помилка під час підтвердження угоди: {ex.Message}");
            }
        }
        public async Task HandleCancelTradeAsync(Message message)
        {
            try
            {
                ChatStatus? chatStatus = await _contextService.GetStatusAsync(message.Chat.Id);

                ServiceResponse<CancelTradeDto> response = await _tradingService.CancelTradeAsync(message.Chat.Id);
                if (response.Success)
                {
                    await _botClient.SendMessage(message.Chat.Id, "Торгівля успішно скасована.");
                }
                else if (!response.Success && IsTradeInProgress(chatStatus))
                {
                    await _botClient.SendMessage(message.Chat.Id, "Процес створення торгівлі припинено");
                }
                else
                {
                    await _botClient.SendMessage(message.Chat.Id, response.Message);

                    if (response.ErrorType == ErrorType.Unauthorized)
                    {
                        await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
                    }
                }

                if (chatStatus != null)
                {
                    chatStatus.Status = BotState.InGame;
                    chatStatus.TradeOffereeId = null;
                    chatStatus.TradeGiveMoney = null;
                    chatStatus.TradeGiveCells = null;
                    chatStatus.TradeWantedMoney = null;
                    chatStatus.TradeWantedCells = null;

                    await _contextService.UpdateContextDataAsync(chatStatus);
                }
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(message.Chat.Id, $"Помилка під час скасування торгівлі: {ex.Message}");
            }
        }

        private bool IsTradeInProgress(ChatStatus? chatStatus)
        {
            return chatStatus != null &&
                (chatStatus.Status == BotState.AwaitingOfferee ||
                chatStatus.Status == BotState.AwaitingGiveMoney ||
                chatStatus.Status == BotState.AwaitingGiveCells ||
                chatStatus.Status == BotState.AwaitingWantedMoney ||
                chatStatus.Status == BotState.AwaitingWantedCells ||
                chatStatus.Status == BotState.AwaitingConfirmation);
        }
    }
}