using Telegram.Bot.Types;
using Telegram.Bot;
using MonopolyBot.Telegram.Interfaces.Command;
using MonopolyBot.Core.Interfaces.Services;

namespace MonopolyBot.Telegram.Handlers.Command
{
    internal class StartCommandHandler : IStartCommandHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IContextService _contextService;

        public StartCommandHandler(ITelegramBotClient botClient, IContextService contextService)
        {
            _botClient = botClient;
            _contextService = contextService;
        }

        public async Task HandleStartAsync(Message message)
        {
            await _contextService.SetStateAsync(message.Chat.Id, Core.Enums.BotState.None);
            await _botClient.SendMessage(message.Chat.Id, "Вітаємо! Це бот для гри в Монополію. Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
        }
    }
}
