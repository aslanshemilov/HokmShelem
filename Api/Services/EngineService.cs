using Newtonsoft.Json;

namespace Api.Services
{
    public class EngineService : IEngineService
    {
        private readonly IHttpClientFactory _clientFactory;

        public EngineService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
        public async Task AddPlayerAsync(PlayerDto model)
        {
            HttpClient client = _clientFactory.CreateClient("Engine");
            HttpRequestMessage message = new();

            message.RequestUri = new Uri(client.BaseAddress +  "api/play/add-player");
            message.Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            message.Method = HttpMethod.Post;
           

            var response = await client.SendAsync(message);
            var apiContent = await response.Content.ReadAsStringAsync();
            var responseBack = JsonConvert.DeserializeObject<ApiResult>(apiContent);
        }

        public async Task AddTest()
        {
            var client = _clientFactory.CreateClient("Engine");
            var response = await client.GetAsync($"/api/play/add-test/");
            var apiContent = await response.Content.ReadAsStringAsync();
            var responseBack = JsonConvert.DeserializeObject<ApiResult>(apiContent);

            //if (responseBack.IsSuccess)
            //{
            //    return JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(responseBack.Result));
            //}

            //return new CouponDto();
        }
    }
}
