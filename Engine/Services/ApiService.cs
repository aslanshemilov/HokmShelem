using Newtonsoft.Json;

namespace Engine.Services
{
    public class ApiService : IApiService
    {
        private readonly IHttpClientFactory _clientFactory;

        public ApiService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
        public async Task<PlayerDto> GetPlayerInfoAsync(bool isGuestUser)
        {
            var client = _clientFactory.CreateClient("Api");
            string url = isGuestUser ? "/api/guest/info/" : "/api/profile/player-info/";
            var response = await client.GetAsync(url);
            var apiContent = await response.Content.ReadAsStringAsync();
            var responseBack = JsonConvert.DeserializeObject<ApiResult>(apiContent);
            if (responseBack.IsSuccess)
            {
                return JsonConvert.DeserializeObject<PlayerDto>(Convert.ToString(responseBack.Result));
            }

            return null;
        }
        public async Task<bool> CreateGameHistoryAsync(GameHistory model)
        {
            var client = _clientFactory.CreateClient("Api");
            HttpRequestMessage message = new();
            message.Headers.Add("Accept", "application/json");
            message.RequestUri = new Uri(client.BaseAddress + "api/gameHistory/");
            message.Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            message.Method = HttpMethod.Post;

            var apiResponse = await client.SendAsync(message);
            if (apiResponse.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            return false;
        }
    }
}
