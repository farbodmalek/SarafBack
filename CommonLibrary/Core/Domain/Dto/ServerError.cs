using CommonLibrary.Core.Domain.Enum;
using System.Net;

namespace CommonLibrary.Core.Domain
{
    public class ServerError
    {
        public string Hint { get; set; }
        public int Type { get; set; } = (int)ErrorTypeEnum.Business;
        public int Code { get; set; } = (int)HttpStatusCode.OK;
    }
}
