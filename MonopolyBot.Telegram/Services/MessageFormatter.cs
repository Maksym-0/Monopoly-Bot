using MonopolyBot.Core.Models.Api.DTO.Games;
using MonopolyBot.Core.Models.Api.DTO.Rooms;
using MonopolyBot.Core.Models.Bot;
using MonopolyBot.Telegram.Interfaces.Services;

namespace MonopolyBot.Telegram.Services
{
    internal class MessageFormatter : IMessageFormatter
    {
        public string BuildRoomMessage(RoomDto room)
        {
            string players; 
            if(room.Players.Count > 0)
                players = string.Join(", ", room.Players.Select(p => p.Name));
            else
                players = "Відсутні";

            string text = $"Кімната {room.RoomId}\n" +
                $"Максимальна кількість гравців - {room.MaxNumberOfPlayers}\n";
            text += $"Гравці в кімнаті: {players}\n";
            if (room.HavePassword) text += "Пароль: Встановлено\n";
            else text += "Пароль: Не встановлено\n";
            if (room.InGame) text += "Гра в кімнаті: Так\n";
            else text += "Гра в кімнаті: Ні\n";

            return text;
        }
        
        public List<string> BuildAllGameStatusMessages(GameStateDto game)
        {
            const int maxMessageLength = 4000;

            List<string> cellMessages = new List<string>();
            List<string> playerMessages = new List<string>();

            string cellBlock = "";
            List<PlayerDto> playersOnCell;
            foreach (var cell in game.Board.Cells)
            {
                playersOnCell = new List<PlayerDto>();
                foreach (var player in game.Players)
                {
                    if (player.Location == cell.Number)
                        playersOnCell.Add(player);
                }

                string cellInfo = BuildCellStatusMessage(game, cell, playersOnCell);

                cellBlock += "\n";

                if (cellBlock.Length + cellInfo.Length > maxMessageLength)
                {
                    cellMessages.Add(cellBlock);
                    cellBlock = cellInfo;
                }
                else
                    cellBlock += cellInfo;
            }
            if (!string.IsNullOrEmpty(cellBlock))
            {
                cellMessages.Add(cellBlock);
            }

            string playerBlock = "";
            foreach (var player in game.Players)
            {
                string playerInfo = BuildPlayerStatusMessage(game, player);
                playerInfo +=
                    "\n";

                if (playerBlock.Length + playerInfo.Length >= maxMessageLength)
                {
                    playerMessages.Add(playerBlock);
                    playerBlock = playerInfo;
                }
                else
                    playerBlock += playerInfo;
            }
            if (!string.IsNullOrEmpty(playerBlock))
            {
                playerMessages.Add(playerBlock);
            }

            string tradeBlock = BuildActiveTradeMessage(game.CurrentTradeOffer);

            List<string> finalMessages = cellMessages.Concat(playerMessages).ToList();

            if (!string.IsNullOrEmpty(tradeBlock))
            {
                finalMessages.Add(tradeBlock);
            }

            return finalMessages;
        }
        
        public string BuildBoardStatusMessage(GameStateDto game)
        {
            string boardStatus = "";

            List<PlayerDto> playersOnCell;
            foreach (var cell in game.Board.Cells)
            {
                playersOnCell = new List<PlayerDto>();

                foreach (var player in game.Players)
                {
                    if (player.Location == cell.Number)
                        playersOnCell.Add(player);
                }

                if(playersOnCell.Count == 0)
                {
                    continue;
                }

                boardStatus += BuildCellStatusMessage(game, cell, playersOnCell);
            }

            return boardStatus;
        }
        public string BuildPlayersStatusMessage(GameStateDto game)
        {
            string playersStatus = "";

            foreach (PlayerDto player in game.Players)
            {
                playersStatus += BuildPlayerStatusMessage(game, player) + "\n";
            }

            return playersStatus;
        }
        
        public string BuildSelfMoveMessage(MoveDto moveDto)
        {
            string selfMessage;
            string selfDublResult;

            if (moveDto.Player.Dices.Dubl && moveDto.Player.Dices.CountOfDubles == 3)
            {
                selfDublResult = "\n💥 Це був ваш третій дубль підряд! Ви відправляєтесь до тюрми.";

                selfMessage =
                    $"🎲 Ви кинули кубики: {moveDto.Player.Dices.Dice1} + {moveDto.Player.Dices.Dice2} = {moveDto.Player.Dices.DiceSum}.{selfDublResult}\n\n" +
                    "Перевірте статус гри для деталей.";
            }
            else
            {
                selfDublResult = moveDto.Player.Dices.Dubl ? $"\n🔥 Ви викинули дубль №{moveDto.Player.Dices.CountOfDubles}! Маєте додатковий хід" : "";

                selfMessage =
                    $"🎲 Ви кинули кубики: {moveDto.Player.Dices.Dice1} + {moveDto.Player.Dices.Dice2} = {moveDto.Player.Dices.DiceSum}.{selfDublResult}\n" +
                    $"Ви пересунулись на клітинку *{moveDto.Cell.Name}* (#{moveDto.Cell.Number}).\n" +
                    $"{moveDto.CellMessage}\n\n" +
                    "Перевірте статус гри для деталей.";
            }

            return selfMessage;
        }
        public string BuildOthersMoveMessage(MoveDto moveDto)
        {
            string othersMessage;
            string othersDublResult;

            if (moveDto.Player.Dices.Dubl && moveDto.Player.Dices.CountOfDubles == 3)
            {
                othersDublResult = "\n💥 Це був його третій дубль підряд! Він відправляється до тюрми.";

                othersMessage =
                    $"🎲 {moveDto.Player.Name} кинув кубики: {moveDto.Player.Dices.Dice1} + {moveDto.Player.Dices.Dice2} = {moveDto.Player.Dices.DiceSum}.{othersDublResult}\n\n" +
                    "Перевірте статус гри для деталей.";
            }
            else
            {
                othersDublResult = moveDto.Player.Dices.Dubl ? $"\n🔥 Викинуто дубль №{moveDto.Player.Dices.CountOfDubles}! Гравець має додатковий хід" : "";

                othersMessage =
                    $"🎲 {moveDto.Player.Name} кинув кубики: {moveDto.Player.Dices.Dice1} + {moveDto.Player.Dices.Dice2} = {moveDto.Player.Dices.DiceSum}.{othersDublResult}\n" +
                    $"Перейшов на клітинку *{moveDto.Cell.Name}* (#{moveDto.Cell.Number}).\n" +
                    $"{moveDto.CellMessage}\n\n" +
                    "Перевірте статус гри для деталей.";
            }

            return othersMessage;
        }

        public string BuildSelfBuyMessage(BuyDto buyDto)
        {
            string selfMessage =
                    $"Ви купили клітину №{buyDto.CellNumber} ({buyDto.CellName}) [{buyDto.CellMonopolyType}] за {buyDto.Price}.\n" +
                    $"Баланс: {buyDto.OldBalance} → {buyDto.NewBalance}.";

            if (buyDto.HasMonopoly != null)
            {
                selfMessage += $"\n{((bool)buyDto.HasMonopoly ? "🎉 У вас тепер монополія!" : "Монополії ще немає.")}";
            }

            return selfMessage;
        }
        public string BuildOthersBuyMessage(BuyDto butDto)
        {
            string othersMessage =
                    $"{butDto.PlayerName} придбав клітину №{butDto.CellNumber} ({butDto.CellName}) [{butDto.CellMonopolyType}] за {butDto.Price}. " +
                    $"Баланс: {butDto.OldBalance} → {butDto.NewBalance}.";

            if (butDto.HasMonopoly != null)
            {
                othersMessage += $"\n{((bool)butDto.HasMonopoly ? "💥💥💥Тепер у нього монополія!💥💥💥" : "")}";
            }

            return othersMessage;
        }

        public string BuildSelfPayMessage(PayDto result)
        {
            string selfMessage = $"Оплата {result.Amount}$ здійснена. Ваш баланс: {result.NewPlayerBalance}$";

            if (result.ReceiverId != null)
            {
                selfMessage = $"Оплата {result.Amount}$ на рахунок {result.ReceiverName} здійснена.\n" +
                    $"Ваш баланс: {result.NewPlayerBalance}$\n" +
                    $"Баланс {result.ReceiverName}: {result.NewReceiverBalance}$";
            }

            if (result.IsPrisonPay)
            {
                selfMessage = $"Оплата {result.Amount}$ за вихід з тюрми здійснена. Ваш баланс: {result.NewPlayerBalance}$";
            }

            return selfMessage;
        }
        public string BuildOthersPayMessage(PayDto result)
        {
            string othersMessage = $"{result.PlayerName} сплатив {result.Amount}$ ";

            if (result.ReceiverId != null)
            {
                othersMessage += $"гравцю {result.ReceiverName}.\n" +
                    $"{result.PlayerName} баланс: {result.NewPlayerBalance}$\n" +
                    $"{result.ReceiverName} баланс: {result.NewReceiverBalance}$";
            }
            
            if (result.IsPrisonPay)
            {
                othersMessage += $"за вихід з тюрми. " +
                    $"{result.PlayerName} баланс: {result.NewPlayerBalance}$";
            }

            return othersMessage;
        }

        public string BuildSelfLevelChangeMessage(LevelChangeDto levelChangeDto)
        {
            string selfMessage;

            if(levelChangeDto.NewLevel > levelChangeDto.OldLevel)
            {
                selfMessage =
                    $"🔼 Ви підвищили рівень клітини №{levelChangeDto.CellNumber} ({levelChangeDto.CellName}) " +
                    $"з {levelChangeDto.OldLevel} до {levelChangeDto.NewLevel}.\n" +
                    $"Ваш баланс: {levelChangeDto.OldPlayerBalance} → {levelChangeDto.NewPlayerBalance}.";
            }
            else
            {
                selfMessage =
                    $"🔽 Ви знизили рівень клітини №{levelChangeDto.CellNumber} ({levelChangeDto.CellName}) " +
                    $"з {levelChangeDto.OldLevel} до {levelChangeDto.NewLevel}.\n" +
                    $"Ваш баланс: {levelChangeDto.OldPlayerBalance} → {levelChangeDto.NewPlayerBalance}.";
            }

            return selfMessage;
        }
        public string BuildOthersLevelChangeMessage(LevelChangeDto levelChangeDto)
        {
            string othersMessage;
            
            if(levelChangeDto.NewLevel > levelChangeDto.OldLevel)
            {
                othersMessage =
                    $"🔼 {levelChangeDto.PlayerName} підвищив рівень клітини №{levelChangeDto.CellNumber} ({levelChangeDto.CellName}) " +
                    $"з {levelChangeDto.OldLevel} до {levelChangeDto.NewLevel}.\n" +
                    $"Його баланс: {levelChangeDto.OldPlayerBalance} → {levelChangeDto.NewPlayerBalance}.";
            }
            else
            {
                othersMessage =
                    $"🔽{levelChangeDto.PlayerName} знизив рівень клітини №{levelChangeDto.CellNumber} ({levelChangeDto.CellName}) " +
                    $"з {levelChangeDto.OldLevel} до {levelChangeDto.NewLevel}.\n" +
                    $"Його баланс: {levelChangeDto.OldPlayerBalance} → {levelChangeDto.NewPlayerBalance}.";
            }

            return othersMessage;
        }

        public string BuildActiveTradeMessage(TradeOfferDto? tradeOfferDto)
        {
            if (tradeOfferDto == null)
                return "";

            string offererCells = tradeOfferDto.OffererProposition.CellNumbers.Count > 0 ?
                string.Join(", ", tradeOfferDto.OffererProposition.CellNumbers) : "Нічого";

            string offereeCells = tradeOfferDto.OffereeProposition.CellNumbers.Count > 0 ?
                string.Join(", ", tradeOfferDto.OffereeProposition.CellNumbers) : "Нічого";

            return $"\n\n<b>Активна торгова пропозиція мій гравцями {tradeOfferDto.OffererName} та {tradeOfferDto.OffereeName}:</b>\n" +
           $"{tradeOfferDto.OffererName} віддає:\n" +
           $"💰 Гроші: {tradeOfferDto.OffererProposition.Money}$\n" +
           $"🏢 Клітини [{offererCells}]\n\n" +
           $"{tradeOfferDto.OffereeName} віддає:\n" +
           $"💰 Гроші: {tradeOfferDto.OffereeProposition.Money}$\n" +
           $"🏢 Клітини [{offereeCells}]";
        }
        public string BuildTradeMessage(ChatStatus chatStatus)
        {
            string giveCellsText = chatStatus.TradeGiveCells != null && chatStatus.TradeGiveCells.Count > 0
                ? string.Join(", ", chatStatus.TradeGiveCells)
                : "Нічого";

            string wantedCellsText = chatStatus.TradeWantedCells != null && chatStatus.TradeWantedCells.Count > 0
                ? string.Join(", ", chatStatus.TradeWantedCells)
                : "Нічого";

            return "📝 <b>Ваша пропозиція обміну:</b>\n\n" +
                   $"<b>Гравець:</b> {chatStatus.TradeOffereeName}\n\n" +
                   "<b>Ви віддаєте:</b>\n" +
                   $"💰 Гроші: {chatStatus.TradeGiveMoney}$\n" +
                   $"🏢 Клітини: {giveCellsText}\n\n" +
                   "<b>Ви хочете отримати:</b>\n" +
                   $"💰 Гроші: {chatStatus.TradeWantedMoney}$\n" +
                   $"🏢 Клітини: {wantedCellsText}\n\n" +
                   "Відправити цю пропозицію?";
        }

        public string BuildSelfAcceptTradeMessage(AcceptTradeDto acceptTradeDto)
        {
            string receivedCells = acceptTradeDto.NewOffereeCells.Count > 0
                ? string.Join(", ", acceptTradeDto.NewOffereeCells)
                : "Нічого";

            return $"✅ <b>Угоду успішно укладено!</b>\n" +
                   $"Ви прийняли пропозицію від гравця <b>{acceptTradeDto.OffererName}</b>.\n\n" +
                   $"Ваш новий баланс: <b>{acceptTradeDto.NewOffereeBalance}$</b>\n" +
                   $"Ви отримали клітинки: [{receivedCells}]";

        }
        public string BuildOthersAcceptTradeMessage(AcceptTradeDto acceptTradeDto)
        {
            string offererReceivedCells = acceptTradeDto.NewOffererCells.Count > 0
                ? string.Join(", ", acceptTradeDto.NewOffererCells)
                : "Нічого";

            string offereeReceivedCells = acceptTradeDto.NewOffereeCells.Count > 0
                ? string.Join(", ", acceptTradeDto.NewOffereeCells)
                : "Нічого";

            return $"🤝 <b>Торгову угоду укладено!</b>\n" +
                   $"<b>{acceptTradeDto.OffereeName}</b> прийняв пропозицію від <b>{acceptTradeDto.OffererName}</b>.\n\n" +
                   $"<b>Нові статуси учасників:</b>\n" +
                   $"👤 {acceptTradeDto.OffererName} ➔ Баланс: {acceptTradeDto.NewOffererBalance}$, Отримав клітинки: [{offererReceivedCells}]\n" +
                   $"👤 {acceptTradeDto.OffereeName} ➔ Баланс: {acceptTradeDto.NewOffereeBalance}$, Отримав клітинки: [{offereeReceivedCells}]";
        }

        public string BuildSelfCancelTradeMessage(CancelTradeDto cancelTradeDto)
        {
            return $"Торгівлю успішно скасовано";
        }
        public string BuildOthersCancelTradeMessage(CancelTradeDto cancelTradeDto)
        {
            return $"Гравець <b>{cancelTradeDto.CancelerName}</b> скасував торгову угоду";
        }

        private string BuildCellStatusMessage(GameStateDto game, CellDto cell, List<PlayerDto> playersOnCell)
        {
            string cellInfo = "";
            if (cell.Special)
            {
                cellInfo = $"<b>{cell.Number}: {cell.Name}</b> - 🌀 <i>Особлива клітина</i>\n";
                if (playersOnCell.Count > 0)
                    cellInfo += $"👥 <i>Гравці на клітині:</i> {string.Join(", ", playersOnCell.Select(p => p.Name))}\n";
                else
                    cellInfo += "👥 Немає гравців\n";
            }
            else
            {
                if (cell.CellType == "CompanyCell")
                {
                    MonopolyDto? monopoly = game.Board.Monopolies.FirstOrDefault(m => m.Index == cell.MonopolyIndex);

                    cellInfo = $"<b>{cell.Number}: {cell.Name}</b>\n";

                    string ownerName = "Нікому";
                    if (cell.OwnerId != null)
                    {
                        var owner = game.Players.Find(p => p.Id == cell.OwnerId);
                        if (owner != null)
                            ownerName = owner.Name;
                    }
                    cellInfo += $"Власник: {ownerName}\n";
                    if (monopoly != null)
                    {
                        if (monopoly.IsMonopoly)
                            cellInfo += $"💠 Монополія: {monopoly.Type} (активна)\n";
                        else
                            cellInfo += $"💠 Монополія: {monopoly.Type} (неактивна)\n";
                    }
                    if (cell.OwnerId == null)
                        cellInfo += $"💰 Купівля: <b>{cell.Price}$</b>. Рента: <b>{cell.Rent}$</b>\n";
                    else
                        cellInfo += $"💸 Рента: <b>{cell.Rent}$</b>\n";
                    if (playersOnCell.Count > 0)
                        cellInfo += $"👥 <i>Гравці:</i> {string.Join(", ", playersOnCell.Select(p => p.Name))}\n";
                    else
                        cellInfo += "👥 Немає гравців\n";

                    if (monopoly != null && cell.OwnerId != null)
                    {
                        cellInfo += $"📈 Рівень: {cell.Level}\n";
                    }
                }
            }

            return cellInfo;
        }
        private string BuildPlayerStatusMessage(GameStateDto game, PlayerDto player)
        {
            string playerInfo =
                    $"<b>{player.Name}</b> — 💵 <b>{player.Balance}$</b>\n" +
                    $"📍 Клітина: {player.Location} ({game.Board.Cells.FirstOrDefault(c => c.Number == player.Location)?.Name ?? "Невідома позиція"})\n" +
                    (player.Dices != null ? $"🎲 Кубики: {player.Dices.Dice1}+{player.Dices.Dice2} = {player.Dices.DiceSum}" +
                    (player.Dices.Dubl ? " (Дубль!)" : "") + "\n" : "") +
                    (player.IsPrisoner ? "🚔 У в’язниці\n" : "") +
                    (player.CantAction > 0 ? $"⏳ Пропускає {player.CantAction} ходів\n" : "") +
                    (player.ReverseMove > 0 ? $"↩️ Рух назад на {player.ReverseMove}\n" : "");

            if (player.Index == game.TurnState.CurrentPlayerIndex)
            {
                playerInfo +=
                    "\n" +
                    "➡️ Зараз його хід\n" +
                    (game.TurnState.CanRollDices ? "🎲 Необхідно кинути кубики\n" : "") +
                    (game.TurnState.NeedPay ? "💸 Треба оплатити борг\n" : "") +
                    (game.TurnState.CanBuyCell ? "🛒 Може купити клітину\n" : "") +
                    (game.TurnState.CanLevelUpCell ? "⬆️ Може прокачати клітину\n" : "");

            }

            return playerInfo;
        }
    }
}
