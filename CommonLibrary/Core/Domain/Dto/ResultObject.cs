namespace CommonLibrary.Core.Domain
{
    public class ResultObject<T>
    {
        public ResultObject() => ServerErrors = new List<ServerError>();
        public bool Result
        {
            get => this.ServerErrors.Count == 0;
        }
        public T Data { get; set; }
        public IList<ServerError> ServerErrors { get; set; }
        public IList<string> Errors { get; set; } = new List<string>();

        public int ResultCode
        {
            get
            {
                return (Result ? 200 : 400);
            }
        }

        public static ResultObject<T> Make(T data, List<ServerError> serverErrors = null)
        {
            return new ResultObject<T>()
            {
                Data = data,
                ServerErrors = serverErrors
            };
        }

    }
}
