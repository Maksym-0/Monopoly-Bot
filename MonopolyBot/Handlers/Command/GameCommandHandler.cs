using MonopolyBot.Core.Enums;
using MonopolyBot.Core.Interfaces.Services;
using MonopolyBot.Core.Models.Api.DTO.Games;
using MonopolyBot.Telegram.Interfaces.Command;
using MonopolyBot.Telegram.Interfaces.Services;
using Telegram.Bot;
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

        public async Task HandleAllGameStatus(Message message)
        {
            try
            {
                GameStateDto gameState = await _gameService.GameStatusAsync(message.Chat.Id);
                List<string> gameStatusMessages = _messageFormatter.BuildAllGameStatusMessages(gameState);

                foreach (var msg in gameStatusMessages)
                {
                    await _botClient.SendMessage(message.Chat.Id, msg, parseMode: ParseMode.Html);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                await _botClient.SendMessage(message.Chat.Id, ex.Message);
                await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", parseMode: ParseMode.Html, replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(message.Chat.Id, $"Помилка при отриманні статусу гри: {ex.Message}");
            }
        }
        public async Task HandleGameStatus(Message message)
        {
            try
            {
                GameStateDto game = await _gameService.GameStatusAsync(message.Chat.Id);
                
                string boardStatusMessage = _messageFormatter.BuildBoardStatusMessage(game);
                string playersStatusMessage = _messageFormatter.BuildPlayersStatusMessage(game);

                await _botClient.SendMessage(message.Chat.Id, boardStatusMessage, parseMode: ParseMode.Html);
                await _botClient.SendMessage(message.Chat.Id, playersStatusMessage, parseMode: ParseMode.Html);
            }
            catch (UnauthorizedAccessException ex)
            {
                await _botClient.SendMessage(message.Chat.Id, ex.Message);
                await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(message.Chat.Id, $"Помилка при отриманні статусу гри: {ex.Message}");
            }
        }
        public async Task HandleRollDice(Message message)
        {
            try
            {
                MoveDto result = await _gameService.RollDiceAsync(message.Chat.Id);

                string selfMessage;
                string othersMessage;

                string selfDublResult;
                string othersDublResult;

                if (result.Player.Dices.Dubl && result.Player.Dices.CountOfDubles == 3)
                {
                    selfDublResult = "\n💥 Це був ваш третій дубль підряд! Ви відправляєтесь до тюрми.";
                    othersDublResult = "\n💥 Це був його третій дубль підряд! Він відправляється до тюрми.";

                    selfMessage =
                        $"🎲 Ви кинули кубики: {result.Player.Dices.Dice1} + {result.Player.Dices.Dice2} = {result.Player.Dices.DiceSum}.{selfDublResult}\n\n" +
                        "Перевірте статус гри для деталей.";

                    othersMessage =
                        $"🎲 {result.Player.Name} кинув кубики: {result.Player.Dices.Dice1} + {result.Player.Dices.Dice2} = {result.Player.Dices.DiceSum}.{othersDublResult}\n\n" +
                        "Перевірте статус гри для деталей.";
                }
                else
                {
                    selfDublResult = result.Player.Dices.Dubl ? $"\n🔥 Ви викинули дубль №{result.Player.Dices.CountOfDubles}! Маєте додатковий хід" : "";
                    othersDublResult = result.Player.Dices.Dubl ? $"\n🔥 Викинуто дубль №{result.Player.Dices.CountOfDubles}! Гравець має додатковий хід" : "";

                    selfMessage =
                        $"🎲 Ви кинули кубики: {result.Player.Dices.Dice1} + {result.Player.Dices.Dice2} = {result.Player.Dices.DiceSum}.{selfDublResult}\n" +
                        $"Ви пересунулись на клітинку *{result.Cell.Name}* (#{result.Cell.Number}).\n" +
                        $"{result.CellMessage}\n\n" +
                        "Перевірте статус гри для деталей.";

                    othersMessage =
                        $"🎲 {result.Player.Name} кинув кубики: {result.Player.Dices.Dice1} + {result.Player.Dices.Dice2} = {result.Player.Dices.DiceSum}.{othersDublResult}\n" +
                        $"Перейшов на клітинку *{result.Cell.Name}* (#{result.Cell.Number}).\n" +
                        $"{result.CellMessage}\n\n" +
                        "Перевірте статус гри для деталей.";
                }

                List<long> chatIdsInGame = await _gameService.GetChatIdsInGameAsync(message.Chat.Id);
                await _broadcastService.SendPersonalizedMessageAsync(message.Chat.Id, selfMessage, chatIdsInGame, othersMessage);
            }
            catch (UnauthorizedAccessException ex)
            {
                await _botClient.SendMessage(message.Chat.Id, ex.Message);
                await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(message.Chat.Id, $"Помилка при кидку кубиків: {ex.Message}");
            }
        }
        public async Task HandleBuy(Message message)
        {
            try
            {
                BuyDto result = await _gameService.BuyCellAsync(message.Chat.Id);

                string selfMessage =
                    $"Ви купили клітину №{result.CellNumber} ({result.CellName}) [{result.CellMonopolyType}] за {result.Price}.\n" +
                    $"Баланс: {result.OldBalance} → {result.NewBalance}.";

                string othersMessage =
                    $"{result.PlayerName} придбав клітину №{result.CellNumber} ({result.CellName}) [{result.CellMonopolyType}] за {result.Price}. " +
                    $"Баланс: {result.OldBalance} → {result.NewBalance}.";
                
                if(result.HasMonopoly != null)
                {
                    selfMessage += $"\n{((bool)result.HasMonopoly ? "🎉 У вас тепер монополія!" : "Монополії ще немає.")}";
                    othersMessage += $"\n{((bool)result.HasMonopoly ? "💥💥💥Тепер у нього монополія!💥💥💥" : "")}";
                }

                List<long> chatIdsInGame = await _gameService.GetChatIdsInGameAsync(message.Chat.Id);
                await _broadcastService.SendPersonalizedMessageAsync(message.Chat.Id, selfMessage, chatIdsInGame, othersMessage);
            }
            catch (UnauthorizedAccessException ex)
            {
                await _botClient.SendMessage(message.Chat.Id, ex.Message);
                await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(message.Chat.Id, $"Помилка при купівлі клітини: {ex.Message}");
            }
        }
        public async Task HandlePayRent(Message message)
        {
            try
            {
                PayDto result = await _gameService.PayRentAsync(message.Chat.Id);

                string selfMessage = BuildSelfPayMessage(result);
                string othersMessage = BuildOthersPayMessage(result);

                List<long> chatIdsInGame = await _gameService.GetChatIdsInGameAsync(message.Chat.Id);
                await _broadcastService.SendPersonalizedMessageAsync(message.Chat.Id, selfMessage, chatIdsInGame, othersMessage);
            }
            catch (UnauthorizedAccessException ex)
            {
                await _botClient.SendMessage(message.Chat.Id, ex.Message);
                await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(message.Chat.Id, $"Помилка при оплаті: {ex.Message}");
            }
        }
        public async Task HandlePayToLeavePrison(Message message)
        {
            try
            {
                PayDto result = await _gameService.PayToLeavePrisonAsync(message.Chat.Id);

                string selfMessage = BuildSelfPayMessage(result);
                string othersMessage = BuildOthersPayMessage(result);

                List<long> chatIdsInGame = await _gameService.GetChatIdsInGameAsync(message.Chat.Id);
                await _broadcastService.SendPersonalizedMessageAsync(message.Chat.Id, selfMessage, chatIdsInGame, othersMessage);
            }
            catch (UnauthorizedAccessException ex)
            {
                await _botClient.SendMessage(message.Chat.Id, ex.Message);
                await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(message.Chat.Id, $"Помилка при оплаті: {ex.Message}");
            }
        }
        public async Task HandleLevelUp(Message message)
        {
            try
            {
                await _contextService.SetStateAsync(message.Chat.Id, BotState.AwaitingLevelUpCell);
                await _botClient.SendMessage(message.Chat.Id, "Підвищення рівня клітини розпочато. Введіть номер клітини для підвищення рівня:");
            }
            catch (UnauthorizedAccessException ex)
            {
                await _botClient.SendMessage(message.Chat.Id, ex.Message);
                await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(message.Chat.Id, $"Помилка при підвищенні рівня клітини: {ex.Message}");
            }
        }
        public async Task HandleLevelDown(Message message)
        {
            try
            {
                await _contextService.SetStateAsync(message.Chat.Id, BotState.AwaitingLevelDownCell);
                await _botClient.SendMessage(message.Chat.Id, "Зниження рівня клітини розпочато. Введіть номер клітини для зниження рівня:");
            }
            catch (UnauthorizedAccessException ex)
            {
                await _botClient.SendMessage(message.Chat.Id, ex.Message);
                await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(message.Chat.Id, $"Помилка при зниженні рівня клітини: {ex.Message}");
            }
        }
        public async Task HandleEndAction(Message message)
        {
            try
            {
                NextActionDto result = await _gameService.EndActionAsync(message.Chat.Id);

                string selfMessage =
                    $"Ваша дія завершена. Наступним ходить {result.NewPlayerName}. Перевірте статус гри";

                string othersMessage =
                    $"{result.PlayerName} завершив свою дію. Наступним ходить {result.NewPlayerName}. Перевірте статус гри";

                List<long> chatIdsInGame = await _gameService.GetChatIdsInGameAsync(message.Chat.Id);
                await _broadcastService.SendPersonalizedMessageAsync(message.Chat.Id, selfMessage, chatIdsInGame, othersMessage);
            }
            catch (UnauthorizedAccessException ex)
            {
                await _botClient.SendMessage(message.Chat.Id, ex.Message);
                await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(message.Chat.Id, $"Помилка при завершенні дії: {ex.Message}");
            }
        }
        public async Task HandleLeaveGame(Message message)
        {
            try
            {
                List<long> chatIdsInGame = await _gameService.GetChatIdsInGameAsync(message.Chat.Id);
                LeaveGameDto result = await _gameService.LeaveGameAsync(message.Chat.Id);
                await _broadcastService.SendPlayerLeaveAsync(chatIdsInGame, message.Chat.Id, result);

            }
            catch (UnauthorizedAccessException ex)
            {
                await _botClient.SendMessage(message.Chat.Id, ex.Message);
                await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(message.Chat.Id, $"Помилка при виході з гри: {ex.Message}");
            }
        }
        public async Task HandleEndWatchGame(Message message)
        {
            try
            {
                await _gameService.LeaveWatchGameAsync(message.Chat.Id);
                await _contextService.SetStateAsync(message.Chat.Id, BotState.None);
                await _botClient.SendMessage(message.Chat.Id, "Ви припинили спостереження за грою.", replyMarkup: KeyboardMarkups.roomsKeyboardMarkup);
            }
            catch (UnauthorizedAccessException ex)
            {
                await _botClient.SendMessage(message.Chat.Id, ex.Message);
                await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(message.Chat.Id, $"Помилка при припиненні спостереження за грою: {ex.Message}");
            }
        }

        private string BuildSelfPayMessage(PayDto result)
        {
            string selfMessage = $"Оплата {result.Amount}$ здійснена. Ваш баланс: {result.NewPlayerBalance}$";

            if (result.ReceiverId != null)
            {
                selfMessage = $"Оплата {result.Amount}$ на рахунок {result.ReceiverName} здійснена.\n" +
                    $"Ваш баланс: {result.NewPlayerBalance}$\n" +
                    $"Баланс {result.ReceiverName}: {result.NewReceiverBalance}$";
            }
            else
            {
                selfMessage = $"Оплата {result.Amount}$ за вихід з тюрми здійснена. Ваш баланс: {result.NewPlayerBalance}$";
            }

            return selfMessage;
        }
        private string BuildOthersPayMessage(PayDto result)
        {
            string othersMessage;

            if (result.ReceiverId != null)
            {
                othersMessage = $"{result.PlayerName} сплатив {result.Amount}$ гравцю {result.ReceiverName}.\n" +
                    $"{result.PlayerName} баланс: {result.NewPlayerBalance}$\n" +
                    $"{result.ReceiverName} баланс: {result.NewReceiverBalance}$";
            }
            else
            {
                othersMessage = $"{result.PlayerName} сплатив за вихід з тюрми {result.Amount}$. " +
                    $"{result.PlayerName} баланс: {result.NewPlayerBalance}$";
            }

            return othersMessage;
        }
    }
}
