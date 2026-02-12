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
            return await _unitOfWork.ChatStatuses.GetByChatId(chatId);
        }

        public async Task SetStateAsync(long chatId, BotState state)
        {
            ChatStatus? chatStatus = await _unitOfWork.ChatStatuses.GetByChatId(chatId);

            if (chatStatus != null)
            {
                if (state == BotState.None)
                {
                    chatStatus.AccountName = null;
                    chatStatus.RoomId = null;
                    chatStatus.MaxNumberOfPlayers = null;
                }
                chatStatus.Status = state;
                await _unitOfWork.ChatStatuses.Update(chatStatus);
            }
            else
            {
                chatStatus = new ChatStatus(chatId, state);
                await _unitOfWork.ChatStatuses.Add(chatStatus);
            }
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateContextDataAsync(ChatStatus chatStatus)
        {
            ChatStatus? existingStatus = await _unitOfWork.ChatStatuses.GetByChatId(chatStatus.ChatId);
            
            if (existingStatus != null)
            {
                existingStatus.Status = chatStatus.Status;
                existingStatus.AccountName = chatStatus.AccountName;
                existingStatus.RoomId = chatStatus.RoomId;
                existingStatus.MaxNumberOfPlayers = chatStatus.MaxNumberOfPlayers;

                await _unitOfWork.ChatStatuses.Update(existingStatus);
            }
            else
            {
                await _unitOfWork.ChatStatuses.Add(chatStatus);
            }
            
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ClearContextAsync(long chatId)
        {
            await SetStateAsync(chatId, BotState.None);
        }
        public async Task DeleteContextAsync(long chatId)
        {
            await _unitOfWork.ChatStatuses.DeleteByChatId(chatId);
        }
    }
}
