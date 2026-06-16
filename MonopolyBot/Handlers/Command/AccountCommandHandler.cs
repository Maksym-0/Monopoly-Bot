using Telegram.Bot.Types;
using Telegram.Bot;
using MonopolyBot.Telegram.Interfaces.Command;
using MonopolyBot.Core.Interfaces.Services;
using MonopolyBot.Core.Enums;
using MonopolyBot.Core.Models.Services;

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

        public async Task HandleStartRegisterAsync(Message message)
        {
            await _contextService.SetStateAsync(message.Chat.Id, BotState.AwaitingRegister);
            await _botClient.SendMessage(message.Chat.Id, "Реєстрацію розпочато. Введіть ім'я:");
        }
        public async Task HandleStartLoginAsync(Message message)
        {
            await _contextService.SetStateAsync(message.Chat.Id, BotState.AwaitingLogin);
            await _botClient.SendMessage(message.Chat.Id, "Вхід в обліковий запис розпочато. Введіть ім'я:");
        }
        public async Task HandleStartDeleteAccountAsync(Message message)
        {
            await _contextService.SetStateAsync(message.Chat.Id, BotState.AwaitingDeleteAccount);
            await _botClient.SendMessage(message.Chat.Id, "Видалення аккаунту розпочато. Введіть ім'я.");
        }
        public async Task HandleRoomsMenuAsync(Message message)
        {
            await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.roomsKeyboardMarkup);
        }

        public async Task HandleMeAsync(Message message)
        {
            try
            {
                ServiceResponse<ProfileInfo> response = await _accService.GetMyDataAsync(message.Chat.Id);
                if (!response.Success)
                {
                    await _contextService.ClearContextAsync(message.Chat.Id);
                    await _botClient.SendMessage(message.Chat.Id, response.Message);

                    if(response.ErrorType == ErrorType.Unauthorized)
                    {
                        await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
                    }
                    return;
                }

                await _botClient.SendMessage(message.Chat.Id,
                    $"Ваше ID: {response.Data.UserId}\n" +
                    $"Ваше ім'я: {response.Data.Name}");
            }
            catch (Exception ex)
            {
                await _contextService.ClearContextAsync(message.Chat.Id);
                await _botClient.SendMessage(message.Chat.Id, $"Помилка при отриманні даних: {ex.Message}");
            }
        }
    }
}
