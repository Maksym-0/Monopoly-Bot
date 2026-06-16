using Microsoft.Extensions.DependencyInjection;
using MonopolyBot.Core.Enums;
using MonopolyBot.Core.Interfaces.Services;
using MonopolyBot.Core.Models.Bot;
using MonopolyBot.Core.Models.Services;
using MonopolyBot.Telegram.Interfaces.Callback;
using MonopolyBot.Telegram.Interfaces.Command;
using MonopolyBot.Telegram.Interfaces.Status;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MonopolyBot
{
    internal class MonopolyBot
    {
        ITelegramBotClient _botClient;
        CancellationToken _cancellationToken = new CancellationToken();
        ReceiverOptions _receiverOptions = new ReceiverOptions();

        IServiceProvider _serviceProvider;

        public MonopolyBot(ITelegramBotClient botClient, IServiceProvider serviceProvider)
        {
            _botClient = botClient;

            _serviceProvider = serviceProvider;
        }

        internal async Task StartAsync()
        {
            _botClient.StartReceiving(HandlerUpdateAsync, HandlerErrorAsync, _receiverOptions, _cancellationToken);
            var botMe = await _botClient.GetMe();
            Console.WriteLine($"Бот {botMe.Username} почав працювати");
            await Task.Delay(-1);
        }
        private Task HandlerErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellation)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Помилка в телеграм бот АПІ:\n {apiRequestException.ErrorCode}" +
                $"\n{apiRequestException.Message}",
                _ => exception.ToString()
            };
            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
        private async Task HandlerUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellation)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;

                if (update.Type == UpdateType.Message && update.Message.Text != null)
                {
                    await HandlerMessageAsync(botClient, update.Message, scopedServices);
                }
                else if (update.Type == UpdateType.CallbackQuery)
                {
                    await HandlerCallbackAsync(botClient, update.CallbackQuery, scopedServices);
                }
            }
        }

        private async Task HandlerCallbackAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, IServiceProvider serviceProvider)
        {
            var callbackRoomHandler = serviceProvider.GetRequiredService<IRoomCallbackHandler>();
            var callbackGameHandler = serviceProvider.GetRequiredService<IGameCallbackHandler>();
            var callbackTradeHandler = serviceProvider.GetRequiredService<ITradeCallbackHandler>();

            var chatId = callbackQuery.Message.Chat.Id;
            var data = callbackQuery.Data;

            if (data.StartsWith("JoinRoom:"))
            {
                await callbackRoomHandler.HandleJoinRoomAsync(chatId, data);
            }
            else
            if (data.StartsWith("LeaveRoom:"))
            {
                await callbackRoomHandler.HandleLeaveRoomAsync(chatId);
            }
            else
            if (data.StartsWith("CreateRoom:"))
            {
                await callbackRoomHandler.HandleCreateRoomAsync(chatId, data);
            }
            else
            if (data.StartsWith("GameStatus:"))
            {
                await callbackGameHandler.HandleGameStatusAsync(chatId, data);
            }
            else
            if (data.StartsWith("ReturnToGame:"))
            {
                await callbackGameHandler.HandleReturnToGameAsync(chatId, data);
            }
            else
            if (data.StartsWith("WatchGame:"))
            {
                await callbackGameHandler.HandleWatchGameAsync(chatId, data);
            }
            else
            if (data.StartsWith("Trade:"))
            {
                await callbackTradeHandler.HandleSetOffereeIdAsync(chatId, data);
            }
            else
            if (data.StartsWith("TradeConfirm:"))
            {
                await callbackTradeHandler.HandleTradeConfirmAsync(chatId, data);
            }
        }

        private async Task HandlerMessageAsync(ITelegramBotClient botClient, Message message, IServiceProvider serviceProvider)
        {
            var accountMessageHandler = serviceProvider.GetRequiredService<IAccountCommandHandler>();

            var contextService = serviceProvider.GetRequiredService<IContextService>();
            var authorizationService = serviceProvider.GetRequiredService<IAuthorizationService>();

            bool result = await HandleBasicCommandsAsync(message, serviceProvider);
            if (result)
                return;

            ChatStatus? status = await contextService.GetStatusAsync(message.Chat.Id);
            if (status != null && IsAwaitingState(status.Status))
            {
                await HandleStatusCommandsAsync(message, serviceProvider, status);
                return;
            }

            ServiceResponse<Core.Models.Bot.User> authResult = await authorizationService.GetAuthorizedUserAsync(message.Chat.Id);
            if (!authResult.Success)
            {
                await contextService.ClearContextAsync(message.Chat.Id);
                await botClient.SendMessage(message.Chat.Id, authResult.Message, replyMarkup: KeyboardMarkups.loginKeyboardMarkup);
                return;
            }
            Core.Models.Bot.User user = authResult.Data;

            if (user.GameId != null)
            {
                if (user.RoomId != null)
                {
                    await HandleGameCommandsAsync(message, serviceProvider);
                }
                else
                {
                    await HandleWatchGameCommandsAsync(message, serviceProvider);
                }
                return;
            }
            else
            {
                await HandleMenuCommandsAsync(message, serviceProvider);
                return;
            }
        }

        private async Task<bool> HandleBasicCommandsAsync(Message message, IServiceProvider serviceProvider)
        {
            var startMessageHandler = serviceProvider.GetRequiredService<IStartCommandHandler>();
            var accountMessageHandler = serviceProvider.GetRequiredService<IAccountCommandHandler>();

            switch (message.Text)
            {
                case "/start":
                    await startMessageHandler.HandleStartAsync(message);
                    return true;
                case "Profile":
                    await accountMessageHandler.HandleMeAsync(message);
                    return true;
                case "Register":
                    await accountMessageHandler.HandleStartRegisterAsync(message);
                    return true;
                case "Login":
                    await accountMessageHandler.HandleStartLoginAsync(message);
                    return true;
            }
            return false;
        }
        private async Task HandleMenuCommandsAsync(Message message, IServiceProvider serviceProvider)
        {
            var accountMessageHandler = serviceProvider.GetRequiredService<IAccountCommandHandler>();
            var roomMessageHandler = serviceProvider.GetRequiredService<IRoomCommandHandler>();

            switch (message.Text)
            {
                case "Delete Account":
                    await accountMessageHandler.HandleStartDeleteAccountAsync(message);
                    return;
                case "Rooms Menu":
                    await accountMessageHandler.HandleRoomsMenuAsync(message);
                    return;

                case "Create Room":
                    await roomMessageHandler.HandleCreateRoomAsync(message);
                    return;
                case "View Rooms":
                    await roomMessageHandler.HandleGetRoomsAsync(message);
                    return;
                case "Accounts menu":
                    await roomMessageHandler.HandleAccountsMenuAsync(message);
                    return;

                default:
                    await _botClient.SendMessage(message.Chat.Id, "Невідома команда. Спробуйте ще раз.");
                    return;
            }
        }
        private async Task HandleGameCommandsAsync(Message message, IServiceProvider serviceProvider)
        {
            var gameCommandHandler = serviceProvider.GetRequiredService<IGameCommandHandler>();
            var tradeCommandHandler = serviceProvider.GetRequiredService<ITradeCommandHandler>();

            switch (message.Text)
            {
                case "All Game Status":
                    await gameCommandHandler.HandleAllGameStatusAsync(message);
                    return;
                case "Game Status":
                    await gameCommandHandler.HandleGameStatusAsync(message);
                    return;
                case "Roll Dice":
                    await gameCommandHandler.HandleRollDicesAsync(message);
                    return;
                case "Buy":
                    await gameCommandHandler.HandleBuyAsync(message);
                    return;
                case "Pay Rent":
                    await gameCommandHandler.HandlePayRentAsync(message);
                    return;
                case "Pay to Leave Prison":
                    await gameCommandHandler.HandlePayToLeavePrisonAsync(message);
                    return;
                case "Trade":
                    await tradeCommandHandler.HandleStartTradeAsync(message);
                    return;
                case "Accept Trade":
                    await tradeCommandHandler.HandleAcceptTradeAsync(message);
                    return;
                case "Cancel/Decline Trade":
                    await tradeCommandHandler.HandleCancelTradeAsync(message);
                    return;
                case "Level Up":
                    await gameCommandHandler.HandleLevelUpAsync(message);
                    return;
                case "Level Down":
                    await gameCommandHandler.HandleLevelDownAsync(message);
                    return;
                case "End Action":
                    await gameCommandHandler.HandleEndActionAsync(message);
                    return;
                case "Leave Game":
                    await gameCommandHandler.HandleLeaveGameAsync(message);
                    return;

                default:
                    await _botClient.SendMessage(message.Chat.Id, "Невідома команда. Використайте команду з меню гри", replyMarkup: KeyboardMarkups.gameKeyboardMarkup);
                    return;
            }
        }
        private async Task HandleWatchGameCommandsAsync(Message message, IServiceProvider serviceProvider)
        {
            var gameMessageHandler = serviceProvider.GetRequiredService<IGameCommandHandler>();

            switch (message.Text)
            {
                case "All Game Status":
                    await gameMessageHandler.HandleAllGameStatusAsync(message);
                    return;
                case "Game Status":
                    await gameMessageHandler.HandleGameStatusAsync(message);
                    return;
                case "End Watch":
                    await gameMessageHandler.HandleEndWatchGameAsync(message);
                    return;

                default:
                    await _botClient.SendMessage(message.Chat.Id, "Невідома команда. Використайте команду з меню гри", replyMarkup: KeyboardMarkups.watchGameKeyboardMarkup);
                    return;
            }
        }
        private async Task HandleStatusCommandsAsync(Message message, IServiceProvider serviceProvider, ChatStatus status)
        {
            var accountStatusHandler = serviceProvider.GetRequiredService<IAccountStatusHandler>();
            var roomStatusHandler = serviceProvider.GetRequiredService<IRoomStatusHandler>();
            var gameStatusHandler = serviceProvider.GetRequiredService<IGameStatusHandler>();
            var tradeStatusHandler = serviceProvider.GetRequiredService<ITradeStatusHandler>();

            switch (status.Status)
            {
                case BotState.AwaitingLogin:
                    await accountStatusHandler.HandleLoginAsync(message, status);
                    return;
                case BotState.AwaitingRegister:
                    await accountStatusHandler.HandleRegisterAsync(message, status);
                    return;
                case BotState.AwaitingDeleteAccount:
                    await accountStatusHandler.HandleDeleteAsync(message, status);
                    return;

                case BotState.AwaitingCreateRoom:
                    await roomStatusHandler.HandleCreateAsync(message, status);
                    return;
                case BotState.AwaitingCreateRoomPassword:
                    await roomStatusHandler.HandleCreatePasswordAsync(message, status);
                    return;
                case BotState.AwaitingJoinRoom:
                    await roomStatusHandler.HandleJoinAsync(message, status);
                    return;

                case BotState.AwaitingLevelUpCell:
                    await gameStatusHandler.HandleLevelUpAsync(message, status);
                    return;
                case BotState.AwaitingLevelDownCell:
                    await gameStatusHandler.HandleLevelDownAsync(message, status);
                    return;

                case BotState.AwaitingOfferee:
                    await tradeStatusHandler.HandleAwaitingOffereeAsync(message);
                    return;
                case BotState.AwaitingGiveMoney:
                    await tradeStatusHandler.HandleGiveMoneyAsync(message, status);
                    return;
                case BotState.AwaitingGiveCells:
                    await tradeStatusHandler.HandleGiveCellsAsync(message, status);
                    return;
                case BotState.AwaitingWantedMoney:
                    await tradeStatusHandler.HandleWantedMoneyAsync(message, status);
                    return;
                case BotState.AwaitingWantedCells:
                    await tradeStatusHandler.HandleWantedCellsAsync(message, status);
                    return;
                case BotState.AwaitingConfirmation:
                    await tradeStatusHandler.HandleAwaitingConfirmationAsync(message);
                    return;
            }
        }

        private bool IsAwaitingState(BotState state)
        {
            return state == BotState.AwaitingLogin ||
                state == BotState.AwaitingRegister ||
                state == BotState.AwaitingDeleteAccount ||
                state == BotState.AwaitingCreateRoom ||
                state == BotState.AwaitingCreateRoomPassword ||
                state == BotState.AwaitingJoinRoom ||
                state == BotState.AwaitingLevelUpCell ||
                state == BotState.AwaitingLevelDownCell ||
                state == BotState.AwaitingOfferee ||
                state == BotState.AwaitingGiveMoney ||
                state == BotState.AwaitingGiveCells ||
                state == BotState.AwaitingWantedMoney ||
                state == BotState.AwaitingWantedCells ||
                state == BotState.AwaitingConfirmation;
        }
    }
}