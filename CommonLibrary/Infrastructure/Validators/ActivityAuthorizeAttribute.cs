using CommonLibrary.Core.Domain;
using CommonLibrary.Infrastructure.Proxy;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace CommonLibrary.Infrastructure.Validators
{
    public class ActivityAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public List<int> Activities { get; set; } = new List<int>();
        public ActivityAuthorizeAttribute(params int[] activities)
        {
            foreach (int id in activities) 
                Activities.Add(id);
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var accessToken = context.HttpContext.Request.Headers["Authorization"];
            var userMng = new UserManagementProxy();
            var data = userMng.GetLoggedUserInfo(accessToken, context.HttpContext);
            if (data != null && data.ID > 0
                            && data.RoleActivitySystems.Any(p => Activities.Contains(p.ActivityID)))
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
