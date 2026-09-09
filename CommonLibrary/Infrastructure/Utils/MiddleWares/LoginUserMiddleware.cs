using CommonLibrary.Core.Domain.Dto;
using Microsoft.AspNetCore.Http;
using CommonLibrary.Infrastructure.Proxy;

namespace CommonLibrary.Infrastructure.Utils.MiddleWares
{
    public class LoginUserMiddleware
    {
        public static LoginUserDTO SetLoginUser(HttpContext httpContext)
        {
            string username = string.Empty;
            string guid = string.Empty;
            try
            {
                var accessToken = httpContext.Request.Headers["Authorization"];
                var userMng = new UserManagementProxy();
                return userMng.GetLoggedUserInfo(accessToken, httpContext);
            }
            catch (Exception)
            {
                return new LoginUserDTO() { ID = 0 };
            }

        }
    }
}
