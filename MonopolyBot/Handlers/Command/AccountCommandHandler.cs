using Telegram.Bot.Types;
using Telegram.Bot;
using MonopolyBot.Telegram.Interfaces.Command;
using MonopolyBot.Core.Interfaces.Services;
using MonopolyBot.Core.Enums;

namespace MonopolyBot.Telegram.Handlers.Command
{
    internal class AccountCommandHandler : IAccountCommandHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IContextService _contextService;
        private readonly IAccountService _accService;

        public AccountCommandHandler(ITelegramBotClient botClient, IContextService contextService, IAccountService accService)
        {
            _botClient = botClient;
            _contextService = contextService;
            _accService = accService;
        }

        public async Task HandleStartRegister(Message message)
        {
            await _contextService.SetStateAsync(message.Chat.Id, BotState.AwaitingRegister);
            await _botClient.SendMessage(message.Chat.Id, "Реєстрацію розпочато. Введіть ім'я:");
        }
        public async Task HandleStartLogin(Message message)
        {
            await _contextService.SetStateAsync(message.Chat.Id, BotState.AwaitingLogin);
            await _botClient.SendMessage(message.Chat.Id, "Вхід в обліковий запис розпочато. Введіть ім'я:");
        }
        public async Task HandleStartDeleteAccount(Message message)
        {
            try
            {
                await _contextService.SetStateAsync(message.Chat.Id, BotState.AwaitingDeleteAccount);
                await _botClient.SendMessage(message.Chat.Id, "Видалення аккаунту розпочато. Введіть ім'я.");
            }
            catch (UnauthorizedAccessException ex)
            {
                await _contextService.ClearContextAsync(message.Chat.Id);
                await _botClient.SendMessage(message.Chat.Id, ex.Message);
                await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
                return;
            }
            catch (Exception ex)
            {
                await _contextService.ClearContextAsync(message.Chat.Id);
                await _botClient.SendMessage(message.Chat.Id, $"Помилка при видаленні акаунту: {ex.Message}");
                return;
            }
        }
        public async Task HandleRoomsMenu(Message message)
        {
            await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.roomsKeyboardMarkup);
        }

        public async Task HandleMe(Message message)
        {
            try
            {
                var data = await _accService.GetMyDataAsync(message.Chat.Id);
                await _botClient.SendMessage(message.Chat.Id,
                    $"Ваше ID: {data.Id}\n" +
                    $"Ваше ім'я: {data.Name}");
            }
            catch (UnauthorizedAccessException ex)
            {
                await _contextService.ClearContextAsync(message.Chat.Id);
                await _botClient.SendMessage(message.Chat.Id, ex.Message);
                await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
            }
            catch (Exception ex)
            {
                await _contextService.ClearContextAsync(message.Chat.Id);
                await _botClient.SendMessage(message.Chat.Id, $"Помилка при отриманні даних: {ex.Message}");
            }
        }
    }
}
