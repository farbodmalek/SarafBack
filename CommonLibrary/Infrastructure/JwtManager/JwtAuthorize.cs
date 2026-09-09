using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CommonLibrary.Infrastructure.JwtManagers
{
    public class JwtAuthorize : AuthorizeAttribute, IAuthorizationFilter
    {
        //private readonly ISessionCacheService sessionCacheService;
        //public JwtAuthorize(ISessionCacheService sessionCacheService)
        //{
        //    this.sessionCacheService = sessionCacheService;
        //}
        public string UserName = string.Empty;
        public DateTime RequestDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
        [AttributeUsage(AttributeTargets.Method)]
        public class AllowAnonymousAttribute : Attribute
        {

        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous)
                return;
            var accessToken = context.HttpContext.Request.Headers["Authorization"];
            if (accessToken.Count == 0)
            {
                context.Result = new UnauthorizedObjectResult(string.Empty);
                return;
            }
            if (accessToken.Count > 0)
            {
                string jwt = accessToken[0].Replace("Bearer ", string.Empty);
                var isTokenValid = JwtAuthenticationAttribute.ValidateToken(jwt, out UserName);
                if (!isTokenValid)
                {
                    context.Result = new UnauthorizedObjectResult(string.Empty);
                    return;
                }
                //if (!this.sessionCacheService.ValidateUserSession(UserName))
                //    context.Result = new UnauthorizedObjectResult(string.Empty);
            }
        }
    }
}
