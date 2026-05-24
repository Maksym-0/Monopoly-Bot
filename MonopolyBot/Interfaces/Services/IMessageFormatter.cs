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

        string BuildSelfMoveMessage(MoveDto moveDto);
        string BuildOthersMoveMessage(MoveDto moveDto);

        string BuildSelfBuyMessage(BuyDto buyDto);
        string BuildOthersBuyMessage(BuyDto butDto);

        string BuildSelfPayMessage(PayDto result);
        string BuildOthersPayMessage(PayDto result);

        string BuildSelfLevelChangeMessage(LevelChangeDto levelChangeDto);
        string BuildOthersLevelChangeMessage(LevelChangeDto levelChangeDto);

    }
}
