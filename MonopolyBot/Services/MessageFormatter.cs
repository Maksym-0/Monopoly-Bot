using MonopolyBot.Core.Models.Api.DTO.Games;
using MonopolyBot.Core.Models.Api.DTO.Rooms;
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

            return cellMessages.Concat(playerMessages).ToList();
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
