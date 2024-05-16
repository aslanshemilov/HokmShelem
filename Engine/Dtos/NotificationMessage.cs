namespace Engine.Dtos
{
    public class NotificationMessage
    {
        public NotificationMessage()
        {
            IsSuccess = true;
            UseToastr = true;
        }
        public NotificationMessage(string title, string message = "", bool isSuccess = true, bool useToastr = true)
        {
           
            Title = title;
            Message = message;
            IsSuccess = isSuccess;
            UseToastr = useToastr;
        }

        public bool IsSuccess { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool UseToastr { get; set; }
    }
}
