using MonopolyBot.Core.Models.Api.DTO.Games;
using MonopolyBot.Core.Models.Api.DTO.Rooms;

namespace MonopolyBot.Telegram.Interfaces.Services
{
    internal interface IMessageFormatter
    {
        string BuildRoomMessage(RoomDto room);

        List<string> BuildAllGameStatusMessages(GameStateDto game);

        string BuildBoardStatusMessage(GameStateDto game);
        string BuildPlayersStatusMessage(GameStateDto game);
    }
}
