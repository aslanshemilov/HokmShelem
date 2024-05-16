namespace Api.Interceptors
{
    public class JWTHttpClientHandlerInterceptor : DelegatingHandler
    {
        private readonly IHttpContextAccessor _accessor;

        public JWTHttpClientHandlerInterceptor(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _accessor.HttpContext.GetTokenAsync("access_token");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
