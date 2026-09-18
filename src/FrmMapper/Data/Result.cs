namespace FrmMapper.Data
{
    // 作者：xioa
    // 作者邮箱：1327916255@qq.com
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

    // 作者：xioa
    // 作者邮箱：1327916255@qq.com
    public class Result<T> : Result
    {
        public T Data { get; set; }
    }
}