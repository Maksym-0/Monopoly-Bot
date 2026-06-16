using MonopolyBot.Core.Enums;
using MonopolyBot.Core.Interfaces.DataBase.UnitOfWork;
using MonopolyBot.Core.Interfaces.Services;
using MonopolyBot.Core.Models.Bot;

namespace MonopolyBot.Application.Services
{
    public class ContextService : IContextService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ContextService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ChatStatus?> GetStatusAsync(long chatId)
        {
            return await _unitOfWork.ChatStatuses.GetByChatIdAsync(chatId);
        }

        public async Task SetStateAsync(long chatId, BotState state)
        {
            ChatStatus? chatStatus = await _unitOfWork.ChatStatuses.GetByChatIdAsync(chatId);

            if (chatStatus != null)
            {
                if (state == BotState.None)
                {
                    chatStatus.AccountName = null;
                    chatStatus.RoomId = null;
                    chatStatus.MaxNumberOfPlayers = null;

                    chatStatus.TradeOffereeId = null;
                    chatStatus.TradeGiveMoney = null;
                    chatStatus.TradeGiveCells = null;
                    chatStatus.TradeWantedMoney = null;
                    chatStatus.TradeWantedCells = null;
                }
                chatStatus.Status = state;
            }
            else
            {
                chatStatus = new ChatStatus(chatId, state);
                await _unitOfWork.ChatStatuses.AddAsync(chatStatus);
            }
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateContextDataAsync(ChatStatus chatStatus)
        {
            ChatStatus? existingStatus = await _unitOfWork.ChatStatuses.GetByChatIdAsync(chatStatus.ChatId);
            
            if (existingStatus != null)
            {
                existingStatus.Status = chatStatus.Status;
                existingStatus.AccountName = chatStatus.AccountName;
                existingStatus.RoomId = chatStatus.RoomId;
                existingStatus.MaxNumberOfPlayers = chatStatus.MaxNumberOfPlayers;

                existingStatus.TradeOffereeId = chatStatus.TradeOffereeId;
                existingStatus.TradeGiveMoney = chatStatus.TradeGiveMoney;
                existingStatus.TradeGiveCells = chatStatus.TradeGiveCells;
                existingStatus.TradeWantedMoney = chatStatus.TradeWantedMoney;
                existingStatus.TradeWantedCells = chatStatus.TradeWantedCells;
            }
            else
            {
                await _unitOfWork.ChatStatuses.AddAsync(chatStatus);
            }
            
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ClearContextAsync(long chatId)
        {
            await SetStateAsync(chatId, BotState.None);
        }
        public async Task DeleteContextAsync(long chatId)
        {
            await _unitOfWork.ChatStatuses.DeleteByChatIdAsync(chatId);
        }
    }
}
