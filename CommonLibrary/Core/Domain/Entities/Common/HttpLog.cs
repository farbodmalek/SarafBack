namespace CommonLibrary.Core.Domain.Entities.Common
{
    public class HttpLog
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string RequestMethod { get; set; }
        public string RequestUri { get; set; }
        public string QueryString { get; set; }
        public DateTime RequestDate { get; set; }
        public string RequestContentType { get; set; }
        public string RequestContent { get; set; }
        public int ResponseStatusCode { get; set; }
        public DateTime ResponseDate { get; set; }
        public string ResponseContentType { get; set; }
        public string ResponseContent { get; set; }
        public long ElapsedTime { get; set; }
        public string MicroServiceName { get; set; }
        public string ClientApi { get; set; }
        public string Host { get; set; }
        public string Logger { get; set; }
        public string CallSite { get; set; }
        public string AgentInfo { get; set; }
        public string Environment { get; set; }
    }
}
