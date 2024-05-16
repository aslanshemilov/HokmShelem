namespace Api.Dtos
{
    public class ApiResponse
    {
        public ApiResponse()
        {
            
        }
        public ApiResponse(int statusCode, string title = null, string message = null, string details = null, IEnumerable<string> errors = null, bool isHtmlEnabled = false)
        {
            StatusCode = statusCode;
            Title = title ?? GetDefaultTitleForStatusCode(statusCode);
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
            Details = details;
            Errors = errors;
            IsHtmlEnabled = isHtmlEnabled;
        }
        public int StatusCode { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public bool IsHtmlEnabled { get; set; }
        private string GetDefaultMessageForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                200 => "Success",
                201 => "Success",
                400 => "Bad Request",
                401 => "Please try to login",
                404 => "Not Found",
                500 => SM.InternalServerError,
                _ => null
            };
        }

        private string GetDefaultTitleForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                200 => "Success",
                201 => "Success",
                401 => "Unauthorized",
                _ => "Error"
            };
        }
    }
}
