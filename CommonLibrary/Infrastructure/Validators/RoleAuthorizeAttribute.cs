using CommonLibrary.Core.Domain;
using CommonLibrary.Infrastructure.Proxy;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace CommonLibrary.Infrastructure.Validators
{
    public class RoleAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public string Roles { get; set; }
        private readonly List<string> allowedRoles = new() { "Admin", "Administrator", "SuperAdministrator" };
        public RoleAuthorizeAttribute(params string[] roles)
        {
            Roles = String.Join(",", roles);
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var accessToken = context.HttpContext.Request.Headers["Authorization"];
            var userMng = new UserManagementProxy();
            var data = userMng.GetLoggedUserInfo(accessToken, context.HttpContext);
            if (data != null && data.ID > 0
                            && (data.UserRoleBranchSystems.Any(p => allowedRoles.Contains(p.RoleTitle)) || data.UserRoleBranchSystems.Any(p => Roles.Contains(p.RoleTitle))))
                return;
            else
            {
                var res = new ResultObject<int>();
                res.ServerErrors.Add(new ServerError() { Hint = "شما دسترسی لازم را  ندارید." });

                var actionName = ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)context.ActionDescriptor).ActionName.ToString();


                var msg = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    ReasonPhrase = $"شما دسترسی لازم را ندارید.نوع دسترسی :{actionName}"

                };
                throw new MemberAccessException();
            }
        }
    }
}
