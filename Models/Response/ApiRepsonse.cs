namespace Project_X.Models.Response
{
    public class ApiRepsonse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public List<string> Errors { get; set; } = new();

        public static ApiRepsonse SuccessResponse(string message, object data = null)
        {
            return new ApiRepsonse
            {
                Success = true,
                Message = message,
                Data = data,
            };
        }
        public static ApiRepsonse FailureResponse(string message, List<string> errors = null)
        {
            return new ApiRepsonse
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>(),
            };
        }
    }
}
