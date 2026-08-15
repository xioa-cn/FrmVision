namespace FrmMapper.Data
{
    public class Result
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        public static Result Fail(string message)
        {
            return new Result()
            {
                IsSuccess = false,
                Message = message
            };
        }

        public static Result Ok(bool isSuccess = true, string message = "")
        {
            return new Result()
            {
                IsSuccess = isSuccess,
                Message = message,
            };
        }
    }

    public class Result<T> : Result
    {
        public T Data { get; set; }
    }
}