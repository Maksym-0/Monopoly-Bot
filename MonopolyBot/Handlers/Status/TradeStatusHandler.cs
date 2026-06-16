using MonopolyBot.Core.Enums;
using MonopolyBot.Core.Interfaces.Services;
using MonopolyBot.Core.Models.Bot;
using MonopolyBot.Telegram.Interfaces.Services;
using MonopolyBot.Telegram.Interfaces.Status;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace MonopolyBot.Telegram.Handlers.Status
{
    internal class TradeStatusHandler : ITradeStatusHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IMessageFormatter _messageFormatter;
        private readonly IContextService _contextService;

        public TradeStatusHandler(ITelegramBotClient botClient, IMessageFormatter messageFormatter, IContextService contextService)
        {
            _botClient = botClient;
            _messageFormatter = messageFormatter;
            _contextService = contextService;
        }

        public async Task HandleAwaitingOffereeAsync(Message message)
        {
            await _botClient.SendMessage(message.Chat.Id, "Будь ласка, скористайтеся кнопками під попереднім повідомленням.");
        }
        public async Task HandleAwaitingConfirmationAsync(Message message)
        {
            await _botClient.SendMessage(message.Chat.Id, "Будь ласка, скористайтеся кнопками під попереднім повідомленням.");
        }
        public async Task HandleGiveMoneyAsync(Message message, ChatStatus chatStatus)
        {
            try
            {
                if(!int.TryParse(message.Text, out int money) || money < 0)
                {
                    await _botClient.SendMessage(message.Chat.Id, "⚠️ Введіть коректне числове значення");
                    return;
                }

                chatStatus.TradeGiveMoney = money;
                chatStatus.Status = BotState.AwaitingGiveCells;
                await _contextService.UpdateContextDataAsync(chatStatus);

                await _botClient.SendMessage(message.Chat.Id, "Введіть номери клітин, що Ви хочете віддати (наприклад: 1, 2, 5, 17) або 0");
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(message.Chat.Id, ex.Message);
            }
        }
        public async Task HandleGiveCellsAsync(Message message, ChatStatus chatStatus)
        {
            try
            {
                List<int>? cells = ParseCellsInput(message.Text);

                if (cells == null)
                {
                    await _botClient.SendMessage(message.Chat.Id, "⚠️ Неправильний формат. Введіть номери клітин через кому (априклад: 1, 2, 5, 14) або 0");
                    return;
                }

                chatStatus.TradeGiveCells = cells;
                chatStatus.Status = BotState.AwaitingWantedMoney;
                await _contextService.UpdateContextDataAsync(chatStatus);

                await _botClient.SendMessage(message.Chat.Id, "Введіть суму грошей, що Ви хочете отримати в межах угоди:");
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(message.Chat.Id, ex.Message);
            }
        }
        public async Task HandleWantedMoneyAsync(Message message, ChatStatus chatStatus)
        {
            try
            {
                if (!int.TryParse(message.Text, out int money) || money < 0)
                {
                    await _botClient.SendMessage(message.Chat.Id, "⚠️ Введіть коректне числове значення");
                    return;
                }

                chatStatus.TradeWantedMoney = money;
                chatStatus.Status = BotState.AwaitingWantedCells;
                await _contextService.UpdateContextDataAsync(chatStatus);

                await _botClient.SendMessage(message.Chat.Id, "Введіть номери клітин, що Ви хочете отримати (наприклад: 1, 2, 5, 17) або 0");
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(message.Chat.Id, ex.Message);
            }
        }
        public async Task HandleWantedCellsAsync(Message message, ChatStatus chatStatus)
        {
            try
            {
                List<int>? cells = ParseCellsInput(message.Text);

                if (cells == null)
                {
                    await _botClient.SendMessage(message.Chat.Id, "⚠️ Неправильний формат. Введіть номери клітин через кому (априклад: 1, 2, 5, 14) або 0");
                    return;
                }

                chatStatus.TradeWantedCells = cells;
                chatStatus.Status = BotState.AwaitingConfirmation;
                await _contextService.UpdateContextDataAsync(chatStatus);

                string text = _messageFormatter.BuildTradeMessage(chatStatus);

                InlineKeyboardMarkup confirmKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new[] { InlineKeyboardButton.WithCallbackData("✅ Відправити", "TradeConfirm:Send") },
                    new[] { InlineKeyboardButton.WithCallbackData("❌ Скасувати", "TradeConfirm:Cancel") }
                });

                await _botClient.SendMessage(message.Chat.Id, text, parseMode: ParseMode.Html, replyMarkup: confirmKeyboard);
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(message.Chat.Id, ex.Message);
            }
        }

        private List<int>? ParseCellsInput(string? input)
        {
            if (string.IsNullOrEmpty(input)) return null;
            
            input = input.Trim();

            if (input == "0") return new List<int>();

            List<int> cells = new List<int>();

            string[] parts = input.Split(',');
            foreach(string part in parts)
            {
                if (int.TryParse(part.Trim(), out int CellNumber))
                {
                    if (CellNumber < 0) 
                        return null;
                    else 
                        cells.Add(CellNumber);
                }
                else
                {
                    return null;
                }
            }

            return cells;
        }
    }
}
