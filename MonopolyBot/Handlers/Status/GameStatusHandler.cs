using Telegram.Bot.Types;
using Telegram.Bot;
using MonopolyBot.Telegram.Interfaces.Status;
using MonopolyBot.Core.Models.Bot;
using MonopolyBot.Core.Models.Api.DTO.Games;
using MonopolyBot.Core.Interfaces.Services;
using MonopolyBot.Telegram.Interfaces.Services;
using MonopolyBot.Core.Enums;

namespace MonopolyBot.Telegram.Handlers.Status
{
    internal class GameStatusHandler : IGameStatusHandler
    {
        ITelegramBotClient _botClient;

        IContextService _contextService;
        IBroadcastService _broadcastService;

        IGameService _gameService;

        public GameStatusHandler(ITelegramBotClient botClient, IContextService contextService, IBroadcastService broadcastService, IGameService gameService)
        {
            _botClient = botClient;
            _contextService = contextService;
            _broadcastService = broadcastService;
            _gameService = gameService;
        }

        public async Task HandleLevelUpStatus(Message message, ChatStatus status)
        {
            try
            {
                int cellNumber = Convert.ToInt32(message.Text);
                LevelChangeDto result = await _gameService.LevelUpCellAsync(message.Chat.Id, cellNumber);

                string selfMessage =
                    $"✅ Ви підвищили рівень клітини №{result.CellNumber} ({result.CellName}) " +
                    $"з {result.OldLevel} до {result.NewLevel}.\n" +
                    $"Ваш баланс: {result.OldPlayerBalance} → {result.NewPlayerBalance}.";

                string othersMessage =
                    $"🔼 {result.PlayerName} підвищив рівень клітини №{result.CellNumber} ({result.CellName}) " +
                    $"з {result.OldLevel} до {result.NewLevel}.\n" +
                    $"Його баланс: {result.OldPlayerBalance} → {result.NewPlayerBalance}.";
                
                List<long> chatIdsInGame = await _gameService.GetChatIdsInGameAsync(message.Chat.Id);
                await _broadcastService.SendPersonalizedMessageAsync(message.Chat.Id, selfMessage, chatIdsInGame, othersMessage);
            }
            catch (UnauthorizedAccessException ex)
            {
                await _botClient.SendMessage(message.Chat.Id, ex.Message);
                await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
            }
            catch (FormatException)
            {
                await _botClient.SendMessage(message.Chat.Id, "Будь ласка, введіть коректний номер клітини.");
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(message.Chat.Id, $"Помилка при підвищенні рівня клітини: {ex.Message}");
            }
            finally
            {
                status.Status = BotState.InGame;
                await _contextService.UpdateContextDataAsync(status);
            }
        }
        public async Task HandleLevelDownStatus(Message message, ChatStatus status)
        {
            try
            {
                int cellNumber = Convert.ToInt32(message.Text);
                LevelChangeDto result = await _gameService.LevelDownCellAsync(message.Chat.Id, cellNumber);

                string selfMessage =
                    $"Клітина №{result.CellNumber} ({result.CellName}) знижена з рівня {result.OldLevel} до {result.NewLevel}.\n" +
                    $"Ваш баланс: {result.OldPlayerBalance} → {result.NewPlayerBalance}";

                string othersMessage =
                    $"{result.PlayerName} знизив рівень клітини №{result.CellNumber} ({result.CellName}) " +
                    $"з {result.OldLevel} до {result.NewLevel}.";

                List<long> chatIdsInGame = await _gameService.GetChatIdsInGameAsync(message.Chat.Id);
                await _broadcastService.SendPersonalizedMessageAsync(message.Chat.Id, selfMessage, chatIdsInGame, othersMessage);
            }
            catch (UnauthorizedAccessException ex)
            {
                await _botClient.SendMessage(message.Chat.Id, ex.Message);
                await _botClient.SendMessage(message.Chat.Id, "Виберіть пункт меню:", replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
            }
            catch (FormatException)
            {
                await _botClient.SendMessage(message.Chat.Id, "Будь ласка, введіть коректний номер клітини.");
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(message.Chat.Id, $"Помилка при зниженні рівня клітини: {ex.Message}");
            }
            finally
            {
                status.Status = BotState.InGame;
                await _contextService.UpdateContextDataAsync(status);
            }
        }
    }
}
