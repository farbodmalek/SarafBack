namespace CommonLibrary.Core.Domain
{
    public class ResultList<T> where T : class
    {
        public ResultList()
        {
            Results = new List<T>();
            ServerErrors = new List<ServerError>();
        }
        public int PageNumber { get; set; }
        public int MaxPageRows { get; set; }
        public int TotalRows { get; set; }

        public List<T> Results { get; set; }

        public int ResultCode
        {
            get
            {
                return (!ServerErrors.Any() ? 200 : 400);
            }
        }
        public IList<ServerError> ServerErrors { get; set; }


    }
}
