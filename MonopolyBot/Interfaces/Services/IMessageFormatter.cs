using MonopolyBot.Core.Models.Api.DTO.Games;
using MonopolyBot.Core.Models.Api.DTO.Rooms;
using MonopolyBot.Core.Models.Bot;

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

        string BuildActiveTradeMessage(TradeOfferDto? tradeOfferDto);
        string BuildTradeMessage(ChatStatus chatStatus);

        string BuildSelfAcceptTradeMessage(AcceptTradeDto acceptTradeDto);
        string BuildOthersAcceptTradeMessage(AcceptTradeDto acceptTradeDto);
    }
}
