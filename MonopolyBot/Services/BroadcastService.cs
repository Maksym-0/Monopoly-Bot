using MonopolyBot.Core.Models.Api.DTO.Games;
using MonopolyBot.Telegram.Interfaces.Services;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace MonopolyBot.Telegram.Services
{
    internal class BroadcastService : IBroadcastService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IMessageFormatter _messageFormatter;

        public BroadcastService(ITelegramBotClient botClient, IMessageFormatter messageFormatter)
        {
            _botClient = botClient;
            _messageFormatter = messageFormatter;
        }

        public async Task SendGameStartAsync(List<long> chatIds, Guid roomId)
        {
            List<Task> tasks = new List<Task>();

            InlineKeyboardMarkup keyboardMarkup = new
                    (
                        InlineKeyboardButton.WithCallbackData("Game Status", $"GameStatus:{roomId}")
                    );

            foreach (long id in chatIds)
            {
                Task task = _botClient.SendMessage(id, "Гру в Вашій кімнаті розпочато." +
                    "\nНатисніть кнопку нижче, щоб перейти до гри:", parseMode: ParseMode.Html, replyMarkup: keyboardMarkup);
                tasks.Add(task);
            }
            await Task.WhenAll(tasks);
        }
        public async Task SendPlayerLeaveAsync(List<long> chatIds, long selfChatId, LeaveGameDto leaveInfo)
        {
            List<Task> tasks = new List<Task>();

            string selfMessage;
            string othersMessage;

            if (leaveInfo.IsGameOver)
            {
                selfMessage = $"Ви вийшли з гри.\nГру завершено. Переможець: {leaveInfo.Winner.Name}.";
                othersMessage = $"{leaveInfo.PlayerName} вийшов з гри.\nГру завершено. Переможець: {leaveInfo.Winner.Name}.";

                Task task;
                foreach (long id in chatIds)
                {
                    if (id == selfChatId)
                    {
                        task = _botClient.SendMessage(id, selfMessage, parseMode: ParseMode.Html, replyMarkup: KeyboardMarkups.roomsKeyboardMarkup);
                    }
                    else
                    {
                        task = _botClient.SendMessage(id, othersMessage, parseMode: ParseMode.Html, replyMarkup: KeyboardMarkups.roomsKeyboardMarkup);
                    }

                    tasks.Add(task);
                }
            }
            else
            {
                selfMessage = $"Ви вийшли з гри.\nЗалишилось гравців: {leaveInfo.RemainingPlayers}.";
                othersMessage = $"{leaveInfo.PlayerName} вийшов з гри.\nЗалишилось гравців: {leaveInfo.RemainingPlayers}.";

                Task task;
                foreach (long id in chatIds)
                {
                    if (id == selfChatId)
                    {
                        task = _botClient.SendMessage(id, selfMessage, parseMode: ParseMode.Html, replyMarkup: KeyboardMarkups.roomsKeyboardMarkup);
                    }
                    else
                    {
                        task = _botClient.SendMessage(id, othersMessage, parseMode: ParseMode.Html, replyMarkup: KeyboardMarkups.roomsKeyboardMarkup);
                    }

                    tasks.Add(task);
                }
            }

            await Task.WhenAll(tasks);
        }
        public async Task SendGameStatusAsync(List<long> chatIds, GameStateDto game)
        {
            List<Task> tasks = new List<Task>();

            string firstMessage = "Потомчний статус гри:\n";
            string cellMessage = _messageFormatter.BuildBoardStatusMessage(game);
            string playersMessage = _messageFormatter.BuildPlayersStatusMessage(game);

            foreach (long id in chatIds)
            {
                Task task = _botClient.SendMessage(id, firstMessage, parseMode: ParseMode.Html, replyMarkup: KeyboardMarkups.gameKeyboardMarkup);
                tasks.Add(task);
            }
            await Task.WhenAll(tasks);
            tasks.Clear();

            foreach (long id in chatIds)
            {
                Task task = _botClient.SendMessage(id, cellMessage, parseMode: ParseMode.Html, replyMarkup: KeyboardMarkups.gameKeyboardMarkup);
                tasks.Add(task);
            }
            await Task.WhenAll(tasks);
            tasks.Clear();

            foreach (long id in chatIds)
            {
                Task task = _botClient.SendMessage(id, playersMessage, parseMode: ParseMode.Html, replyMarkup: KeyboardMarkups.gameKeyboardMarkup);
                tasks.Add(task);
            }
            await Task.WhenAll(tasks);
        }

        public async Task SendMessageAsync(List<long> chatIds, string message)
        {
            List<Task> tasks = new List<Task>();

            foreach( long id in chatIds)
            {
                Task task = _botClient.SendMessage(id, message, parseMode: ParseMode.Html);
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
        }

        public async Task SendPersonalizedMessageAsync(long personalChatId, string personalMessage, List<long> chatIds, string publicMessage)
        {
            List<Task> tasks = new List<Task>();

            Task personalTask = _botClient.SendMessage(personalChatId, personalMessage, parseMode: ParseMode.Html);
            tasks.Add(personalTask);

            foreach (long id in chatIds)
            {
                if(id == personalChatId)
                    continue;

                Task task = _botClient.SendMessage(id, publicMessage, parseMode: ParseMode.Html);
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
        }
    }
}
