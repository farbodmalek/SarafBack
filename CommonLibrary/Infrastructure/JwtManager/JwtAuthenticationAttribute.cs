using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.Http.Filters;

namespace CommonLibrary.Infrastructure.JwtManagers
{
    public class JwtAuthenticationAttribute : Attribute, IAuthenticationFilter
    {
        public string Realm { get; set; }
        public bool AllowMultiple => false;

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var request = context.Request;
            var authorization = request.Headers.Authorization;
            if (authorization == null || authorization.Scheme != "Bearer")
                return;

            if (string.IsNullOrEmpty(authorization.Parameter))
            {
                return;
            }

            var token = authorization.Parameter;
            var principal = await AuthenticateJwtToken(token);
            context.Principal = principal;
        }

        public static bool ValidateToken(string token, out string username)
        {
            username = null;
            var simplePrinciple = JwtManager.GetPrincipal(token);
            var identity = simplePrinciple?.Identity as ClaimsIdentity;
            if (identity == null)
                return false;

            if (!identity.IsAuthenticated)
                return false;

            var usernameClaim = identity.FindFirst("uid");
            username = usernameClaim?.Value;

            if (string.IsNullOrEmpty(username))
                return false;
            return true;
        }

        public static string GetCurrentUser(HttpContext context)
        {
            var accessToken = context.Request.Headers["Authorization"];
            if (accessToken.Count > 0)
            {
                string token = accessToken[0].Replace("Bearer ", string.Empty);
                var simplePrinciple = JwtManager.GetPrincipal(token);
                var identity = simplePrinciple?.Identity as ClaimsIdentity;
                if (identity == null || !identity.IsAuthenticated)
                    return string.Empty;
                var usernameClaim = identity.FindFirst("uid");
                return usernameClaim?.Value;
            }
            return string.Empty;
        }

        protected Task<IPrincipal> AuthenticateJwtToken(string token)
        {
            string username;

            if (ValidateToken(token, out username))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username)
                };

                var identity = new ClaimsIdentity(claims, "Jwt");
                IPrincipal user = new ClaimsPrincipal(identity);

                return Task.FromResult(user);
            }

            return Task.FromResult<IPrincipal>(null);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            Challenge(context);
            return Task.FromResult(0);
        }

        private void Challenge(HttpAuthenticationChallengeContext context)
        {
            string parameter = null;

            if (!string.IsNullOrEmpty(Realm))
                parameter = "realm=\"" + Realm + "\"";
        }
    }
}
