using Telegram.Bot.Types;
using Telegram.Bot;
using MonopolyBot.Telegram.Interfaces.Status;
using MonopolyBot.Core.Models.Api.DTO.Accounts;
using MonopolyBot.Core.Models.Bot;
using MonopolyBot.Core.Interfaces.Services;

namespace MonopolyBot.Telegram.Handlers.Status
{
    internal class AccountStatusHandler : IAccountStatusHandler
    {
        private readonly ITelegramBotClient _botClient;

        private readonly IContextService _contextService;

        private readonly IAccountService _accService;

        public AccountStatusHandler(ITelegramBotClient botClient, IContextService contextService, IAccountService accService)
        {
            _botClient = botClient;
            _contextService = contextService;
            _accService = accService;
        }

        public async Task HandleLoginStatus(Message message, ChatStatus status)
        {
            if (status.AccountName == null)
            {
                status.AccountName = message.Text;
                await _contextService.UpdateContextDataAsync(status);
                await _botClient.SendMessage(message.Chat.Id, "Введіть пароль для входу:");
            }
            else
            {
                try
                {
                    AccountDto loginData = await _accService.LoginAsync(message.Chat.Id, status.AccountName, message.Text);
                    await _botClient.SendMessage(message.Chat.Id, $"Ви увійшли в систему під ім'ям {loginData.Name}.");
                    await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.roomsKeyboardMarkup);
                }
                catch (Exception ex)
                {
                    await _botClient.SendMessage(message.Chat.Id, $"Помилка при вході в систему: {ex.Message}");
                    return;
                }
                finally
                {
                    await _contextService.ClearContextAsync(message.Chat.Id);
                }
            }
        }
        public async Task HandleRegisterStatus(Message message, ChatStatus status)
        {
            if (status.AccountName == null)
            {
                status.AccountName = message.Text;
                await _contextService.UpdateContextDataAsync(status);
                await _botClient.SendMessage(message.Chat.Id, "Введіть пароль для реєстрації:");
            }
            else
            {
                try
                {
                    AccountDto registerResult = await _accService.RegisterAsync(status.AccountName, message.Text);
                    await _botClient.SendMessage(message.Chat.Id, $"Аккаунт {registerResult.Name} створено");
                }
                catch (Exception ex)
                {
                    await _botClient.SendMessage(message.Chat.Id, $"Помилка при реєстрації: {ex.Message}");
                    return;
                }
                finally
                {
                    await _contextService.ClearContextAsync(message.Chat.Id);
                }
            }
        }
        public async Task HandleDeleteAccountStatus(Message message, ChatStatus status)
        {
            if (status.AccountName == null)
            {
                status.AccountName = message.Text;
                await _contextService.UpdateContextDataAsync(status);
                await _botClient.SendMessage(message.Chat.Id, "Введіть пароль для видалення акаунту:");
            }
            else
            {
                try
                {
                    DeleteAccountDto deleteResult = await _accService.DeleteAccountAsync(message.Chat.Id, status.AccountName, message.Text);
                    await _botClient.SendMessage(message.Chat.Id, $"Аккаунт {deleteResult.Name} успішно видалено");
                }
                catch (Exception)
                {
                    await _botClient.SendMessage(message.Chat.Id, "Помилка при видаленні акаунту. Перевірте правильність введених даних.");
                    return;
                }
                finally
                {
                    await _contextService.ClearContextAsync(message.Chat.Id);
                }
            }
        }
    }
}
