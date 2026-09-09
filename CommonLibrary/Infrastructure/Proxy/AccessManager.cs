using CommonLibrary.Core.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace CommonLibrary.Infrastructure.Proxy
{
    public class AccessManager : IAccessManager
    {
        private readonly IHttpContextAccessor context;
        public AccessManager(IHttpContextAccessor context)
        {
            this.context = context;
        }

        public string GetRequestToken()
        {
            var accessToken = context.HttpContext.Request.Headers["Authorization"];
            if (accessToken.Count > 0)
                return accessToken[0].Replace("Bearer ", string.Empty);
            return string.Empty;
        }
    }
}
