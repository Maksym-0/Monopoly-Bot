using MonopolyBot.Core.Interfaces.Clients;
using MonopolyBot.Core.Models.Api.Requests;
using MonopolyBot.Core.Models.Api.Responses;
using MonopolyBot.Core.Models.Api.DTO.Accounts;
using MonopolyBot.Core.Models.Api.DTO.Rooms;
using MonopolyBot.Core.Models.Api.DTO.Games;
using System.Text;
using Newtonsoft.Json;
using MonopolyBot.Core;

namespace MonopolyBot.DataAccess.ApiClients.Clients
{
    public class MonopolyClient : IAccountClient, IRoomClient, IGameClient
    {
        private HttpClient _httpClient = new HttpClient();

        public async Task<ApiResponse<AccountDto>> MeAsync(string jwt)
        {
            string responseJson = await SendAccountRequestAsync(jwt, null, "/me", HttpMethod.Get);
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<AccountDto>>(responseJson);
            return apiResponse;
        }
        public async Task<ApiResponse<AccountDto>> RegisterAsync(AccountRequest account)
        {
            string responseJson = await SendAccountRequestAsync(null, account, "/register", HttpMethod.Post);
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<AccountDto>>(responseJson);
            return apiResponse;
        }
        public async Task<ApiResponse<LoginDto>> LoginAndReturnJWTAsync(AccountRequest account)
        {
            string responseJson = await SendAccountRequestAsync(null, account, "/login", HttpMethod.Post);
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<LoginDto>>(responseJson);
            return apiResponse;
        }
        public async Task<ApiResponse<DeleteAccountDto>> DeleteAccount(AccountRequest account)
        {
            string responseJson = await SendAccountRequestAsync(null, account, "/delete", HttpMethod.Delete);
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<DeleteAccountDto>>(responseJson);
            return apiResponse;
        }

        public async Task<ApiResponse<List<RoomDto>>> GetRoomsAsync(string jwt)
        {
            string responseJson = await SendRoomRequestAsync(jwt, null, HttpMethod.Get);
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<RoomDto>>>(responseJson);
            return apiResponse;
        }
        public async Task<ApiResponse<RoomDto>> CreateRoomAsync(string jwt, CreateRoomRequest dto)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{Constants.ApiAddress}{Constants.ApiRoomHost}/create");
            request.Headers.Add("Authorization", $"Bearer {jwt}");
            request.Content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            string responseJson = await response.Content.ReadAsStringAsync();

            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<RoomDto>>(responseJson);

            return apiResponse;
        }
        public async Task<ApiResponse<JoinRoomDto>> JoinRoomAsync(string jwt, JoinRoomRequest dto)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, $"{Constants.ApiAddress}{Constants.ApiRoomHost}/{dto.RoomId}/join");
            request.Headers.Add("Authorization", $"Bearer {jwt}");
            request.Content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            string responseJson = await response.Content.ReadAsStringAsync();

            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<JoinRoomDto>>(responseJson);

            return apiResponse;
        }
        public async Task<ApiResponse<QuitRoomDto>> QuitRoomAsync(string jwt)
        {
            string responseJson = await SendRoomRequestAsync(jwt, "/quit", HttpMethod.Put);
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<QuitRoomDto>>(responseJson);
            return apiResponse;
        }

        public async Task<ApiResponse<GameStateDto>> GetGameStatusAsync(string jwt, Guid gameId)
        {
            string responseJson = await SendGameRequestAsync(jwt, gameId, null, HttpMethod.Get);
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<GameStateDto>>(responseJson);
            return apiResponse;
        }
        public async Task<ApiResponse<MoveDto>> RollTheDiceAsync(string jwt, Guid gameId)
        {
            string responseJson = await SendGameRequestAsync(jwt, gameId, "/move", HttpMethod.Put);
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<MoveDto>>(responseJson);
            return apiResponse;
        }
        public async Task<ApiResponse<PayDto>> PayRentAsync(string jwt, Guid gameId)
        {
            string responseJson = await SendGameRequestAsync(jwt, gameId, "/pay/rent", HttpMethod.Put);
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<PayDto>>(responseJson);
            return apiResponse;
        }
        public async Task<ApiResponse<PayDto>> PayToLeavePrisonAsync(string jwt, Guid gameId)
        {
            string responseJson = await SendGameRequestAsync(jwt, gameId, "/pay/prison", HttpMethod.Put);
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<PayDto>>(responseJson);
            return apiResponse;
        }
        public async Task<ApiResponse<BuyDto>> BuyCellAsync(string jwt, Guid gameId)
        {
            string responseJson = await SendGameRequestAsync(jwt, gameId, "/cells/buy", HttpMethod.Put);
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<BuyDto>>(responseJson);
            return apiResponse;
        }
        public async Task<ApiResponse<LevelChangeDto>> LevelUpCellAsync(string jwt, Guid gameId, int cellNumber)
        {
            string responseJson = await SendGameRequestAsync(jwt, gameId, $"/cells/{cellNumber}/levelup", HttpMethod.Put);
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<LevelChangeDto>>(responseJson);
            return apiResponse;
        }
        public async Task<ApiResponse<LevelChangeDto>> LevelDownCellAsync(string jwt, Guid gameId, int cellNumber)
        {
            string responseJson = await SendGameRequestAsync(jwt, gameId, $"/cells/{cellNumber}/leveldown", HttpMethod.Put);
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<LevelChangeDto>>(responseJson);
            return apiResponse;
        }
        public async Task<ApiResponse<NextActionDto>> EndActionAsync(string jwt, Guid gameId)
        {
            string responseJson = await SendGameRequestAsync(jwt, gameId, "/endaction", HttpMethod.Put);
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<NextActionDto>>(responseJson);
            return apiResponse;
        }
        public async Task<ApiResponse<LeaveGameDto>> LeaveGameAsync(string jwt, Guid gameId)
        {
            string responseJson = await SendGameRequestAsync(jwt, gameId, "/leave", HttpMethod.Put);
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<LeaveGameDto>>(responseJson);
            return apiResponse;
        }

        private async Task<string> SendAccountRequestAsync(string? jwt, AccountRequest? account, string? command, HttpMethod method)
        {
            HttpRequestMessage request;
            if(command != null)
                request = new HttpRequestMessage(method, $"{Constants.ApiAddress}{Constants.ApiAccouuntHost}{command}");
            else
                request = new HttpRequestMessage(method, $"{Constants.ApiAddress}{Constants.ApiAccouuntHost}");
            
            if(jwt != null)
                request.Headers.Add("Authorization", $"Bearer {jwt}");
            if (account != null)
                request.Content = new StringContent(JsonConvert.SerializeObject(account), Encoding.UTF8, "application/json");
            
            HttpResponseMessage response = await _httpClient.SendAsync(request);
            string responseJson = await response.Content.ReadAsStringAsync();
            return responseJson;
        }
        private async Task<string> SendRoomRequestAsync(string jwt, string? command, HttpMethod method)
        {
            HttpRequestMessage request;
            if (command != null)
                request = new HttpRequestMessage(method, $"{Constants.ApiAddress}{Constants.ApiRoomHost}{command}");
            else
                request = new HttpRequestMessage(method, $"{Constants.ApiAddress}{Constants.ApiRoomHost}");
            
            request.Headers.Add("Authorization", $"Bearer {jwt}");

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            string responseJson = await response.Content.ReadAsStringAsync();
            return responseJson;
        }
        private async Task<string> SendGameRequestAsync(string jwt, Guid gameId, string? command, HttpMethod method)
        {
            HttpRequestMessage request;
            if (command != null) 
                request = new HttpRequestMessage(method, $"{Constants.ApiAddress}{Constants.ApiGameHost}/{gameId}{command}");
            else
                request = new HttpRequestMessage(method, $"{Constants.ApiAddress}{Constants.ApiGameHost}/{gameId}");
            
            request.Headers.Add("Authorization", $"Bearer {jwt}");

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            string responseJson = await response.Content.ReadAsStringAsync();
            return responseJson;
        }
    }
}
