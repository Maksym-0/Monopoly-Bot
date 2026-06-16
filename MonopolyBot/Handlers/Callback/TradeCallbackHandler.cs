using MonopolyBot.Core.Enums;
using MonopolyBot.Core.Interfaces.Services;
using MonopolyBot.Core.Models.Api.DTO.Games;
using MonopolyBot.Core.Models.Bot;
using MonopolyBot.Core.Models.Services;
using MonopolyBot.Telegram.Interfaces.Callback;
using MonopolyBot.Telegram.Interfaces.Services;
using Telegram.Bot;

namespace MonopolyBot.Telegram.Handlers.Callback
{
    internal class TradeCallbackHandler : ITradeCallbackHandler
    {
        private readonly ITelegramBotClient _botClient;

        private readonly IContextService _contextService;
        private readonly IBroadcastService _broadcastService;

        private readonly IGameService _gameService;
        private readonly ITradingService _tradingService;

        public TradeCallbackHandler(ITelegramBotClient botClient, IContextService contextService, IBroadcastService broadcastService, IGameService gameService, ITradingService tradingService)
        {
            _botClient = botClient;
            _contextService = contextService;
            _broadcastService = broadcastService;
            _gameService = gameService;
            _tradingService = tradingService;
        }

        public async Task HandleSetOffereeIdAsync(long chatId, string data)
        {
            try
            {
                string offereeName = data.Split(":")[1];
                Guid offereeId = Guid.Parse(data.Split(':')[2]);

                ChatStatus? chatStatus = await _contextService.GetStatusAsync(chatId);
                if (chatStatus == null)
                {
                    await _botClient.SendMessage(chatId, "Стан чату не знайдено");
                    return;
                }

                if (chatStatus.Status != BotState.AwaitingOfferee)
                {
                    await _botClient.SendMessage(chatId, "Ви не перебуваєте в стані вибору людини, якій пропонується угода");
                    return;
                }

                chatStatus.TradeOffereeId = offereeId;
                chatStatus.TradeOffereeName = offereeName;
                chatStatus.Status = BotState.AwaitingGiveMoney; 
                await _contextService.UpdateContextDataAsync(chatStatus);

                await _botClient.SendMessage(chatId, "Введіть суму грошей, що Ви хочете віддати в межах угоди:");
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(chatId, $"Помилка під час вибору гравця для торгівлі: {ex.Message}");
                return;
            }
        }
        public async Task HandleTradeConfirmAsync(long chatId, string data)
        {
            try
            {
                string command = data.Split(":")[1];

                ChatStatus? chatStatus = await _contextService.GetStatusAsync(chatId);
                if (chatStatus == null || chatStatus.Status != BotState.AwaitingConfirmation)
                {
                    await _botClient.SendMessage(chatId, "Ця дія більше не є актуальною");
                    return;
                }

                if (command == "Send")
                {
                    ServiceResponse<TradeOfferDto> response = await _tradingService.ProposeTradeAsync(chatId, chatStatus.TradeGiveMoney.Value, chatStatus.TradeGiveCells, 
                        chatStatus.TradeOffereeId.Value, chatStatus.TradeWantedMoney.Value, chatStatus.TradeWantedCells);
                    if (!response.Success)
                    {
                        await _botClient.SendMessage(chatId, response.Message);
                        if (response.ErrorType == ErrorType.Unauthorized)
                        {
                            await _botClient.SendMessage(chatId, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.gameKeyboardMarkup);
                        }
                        return;
                    }

                    CancelTrade(chatStatus);
                    await _contextService.UpdateContextDataAsync(chatStatus);

                    await _botClient.SendMessage(chatId, "Пропозицію угоди надіслано");

                    ServiceResponse<List<long>> chatIdsInGame = await _gameService.GetChatIdsInGameAsync(chatId);
                    if (!chatIdsInGame.Success)
                    {
                        await _botClient.SendMessage(chatId, response.Message);
                        if (response.ErrorType == ErrorType.Unauthorized)
                        {
                            await _botClient.SendMessage(chatId, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.gameKeyboardMarkup);
                        }
                        return;
                    }

                    await _broadcastService.SendPersonalizedMessageAsync(chatId, "Очікуйте на відповідь іншого гравця", chatIdsInGame.Data, $"В грі з'явилась нова пропозиція. Перевірте статус гри для отримання деталей");
                }
                if (command == "Cancel")
                {
                    CancelTrade(chatStatus);
                    await _contextService.UpdateContextDataAsync(chatStatus);
                    await _botClient.SendMessage(chatId, "Створення пропозиції скасовано", replyMarkup: KeyboardMarkups.gameKeyboardMarkup);
                }
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(chatId, $"Помилка під час підтвердження/скасування торгівлі: {ex.Message}");
                return;
            }
        }

        private void CancelTrade(ChatStatus chatStatus)
        {
            chatStatus.Status = BotState.InGame;
            chatStatus.TradeOffereeId = null;
            chatStatus.TradeOffereeName = null;
            chatStatus.TradeGiveMoney = null;
            chatStatus.TradeGiveCells = null;
            chatStatus.TradeWantedMoney = null;
            chatStatus.TradeWantedCells = null;
        }
    }
}
