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
        public async Task<PlayerDto> GetPlayerInfoAsync()
        {
            var client = _clientFactory.CreateClient("Api");
            var response = await client.GetAsync($"/api/profile/player-info/");
            var apiContent = await response.Content.ReadAsStringAsync();
            var responseBack = JsonConvert.DeserializeObject<ApiResult>(apiContent);
            if (responseBack.IsSuccess)
            {
                return JsonConvert.DeserializeObject<PlayerDto>(Convert.ToString(responseBack.Result));
            }

            return null;
        }
    }
}
