using Telegram.Bot.Types;
using Telegram.Bot;
using MonopolyBot.Telegram.Interfaces.Status;
using MonopolyBot.Core.Models.Api.DTO.Accounts;
using MonopolyBot.Core.Models.Bot;
using MonopolyBot.Core.Interfaces.Services;
using MonopolyBot.Core.Models.Services;

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

        public async Task HandleLoginAsync(Message message, ChatStatus status)
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
                    ServiceResponse<AccountDto> response = await _accService.LoginAsync(message.Chat.Id, status.AccountName, message.Text);
                    if (!response.Success)
                    {
                        await _botClient.SendMessage(message.Chat.Id, response.Message);
                        return;
                    }

                    await _botClient.SendMessage(message.Chat.Id, $"Ви увійшли в систему під ім'ям {response.Data.Name}.");
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
        public async Task HandleRegisterAsync(Message message, ChatStatus status)
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
                    ServiceResponse<AccountDto> response = await _accService.RegisterAsync(status.AccountName, message.Text);
                    if (!response.Success)
                    {
                        await _botClient.SendMessage(message.Chat.Id, response.Message);
                        return;
                    }

                    await _botClient.SendMessage(message.Chat.Id, $"Аккаунт {response.Data.Name} створено");
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
        public async Task HandleDeleteAsync(Message message, ChatStatus status)
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
                    ServiceResponse<DeleteAccountDto> response = await _accService.DeleteAccountAsync(message.Chat.Id, status.AccountName, message.Text);
                    if (!response.Success)
                    {
                        await _botClient.SendMessage(message.Chat.Id, response.Message);
                        return;
                    }

                    await _botClient.SendMessage(message.Chat.Id, $"Аккаунт {response.Data.Name} успішно видалено");
                }
                catch (Exception ex)
                {
                    await _botClient.SendMessage(message.Chat.Id, $"Помилка при видаленні акаунту: {ex.Message}");
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
