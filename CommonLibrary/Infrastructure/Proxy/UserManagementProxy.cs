using CommonLibrary.Application.DTO.Common;
using CommonLibrary.Core.Domain;
using CommonLibrary.Core.Domain.Dto;
using CommonLibrary.Core.Services.Interfaces;
using CommonLibrary.Infrastructure.JwtManagers;
using CommonLibrary.Infrastructure.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CommonLibrary.Infrastructure.Proxy
{
    public class UserManagementProxy : IUserManagementProxy
    {
        private static StringValues Token;

        private readonly string accessToken;
        public UserManagementProxy()
        {
            
        }
        public UserManagementProxy(IAccessManager accessManager)
        {
            accessToken = accessManager.GetRequestToken();
        }
        public LoginUserDTO GetLoggedUserInfo(StringValues accessToken, HttpContext httpContext)
        {
            string guid = string.Empty;
            JwtSecurityToken jsonToken = new JwtSecurityToken();
            if (accessToken.Count > 0)
            {
                Token = accessToken;
                string jwt = accessToken[0].Replace("Bearer ", string.Empty);
                var simplePrinciple = JwtManager.GetPrincipal(jwt);
                var identity = simplePrinciple?.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    Token = accessToken;
                    guid = identity.FindFirst("uid")?.Value ?? null;
                    if (!string.IsNullOrEmpty(guid))
                    {
                        var sessionCacheService = (ISessionCacheService)httpContext.RequestServices.GetService(typeof(ISessionCacheService));
                        var data = sessionCacheService.GetUserSession(guid)?.Data;
                        if (data != null)
                            return data;
                        data = this.GetUserInfo(guid, jwt);
                        sessionCacheService.RemoveUserSession(data.UID);
                        //sessionCacheService.SetUserSession(data);

                        return data;
                    }
                }
            }
            return new LoginUserDTO() { ID = 0 };
        }
        private LoginUserDTO GetUserInfo(string guid, string jwt)
        {
            RestClient client = new RestClient();
            var requestUri = new Uri(GlobalConfig.Instance.UserManagementApi + "/api/auth/LoggedUser");
            var request = new RestRequest(requestUri, Method.Get);
            request.AddHeader("content-type", "application/json; charset=utf-8");
            request.AddHeader("Authorization", string.Format("Bearer {0}", jwt));
            var responce = client.Execute<ResultObject<LoginUserDTO>>(request);
            var data = responce.Data?.Data;
            if (data != null)
            {
                var roleIDs = data.UserRoleBranchSystems.Where(p => p.SystemID == 1).Select(p => p.RoleID).Distinct().ToList();
                var roles = data.Roles;
                data.UserRoleBranchSystems = data.UserRoleBranchSystems.Where(p => p.SystemID == 1).ToList();
                data.Roles = data.UserRoleBranchSystems.Where(p => p.SystemID == 1).Select(p => new UserRoleDto
                {
                    ID = p.RoleID,
                    RoleID = p.RoleID,
                    RoleTitle = p.RoleTitle,
                    RoleDesc = p.RoleDesc,
                    LevelID = roles.FirstOrDefault(c => c.RoleID == p.RoleID)?.LevelID ?? 0,
                    DivisionType = roles.FirstOrDefault(c => c.RoleID == p.RoleID)?.DivisionType ?? 0
                }).ToList();
                data.RoleActivitySystems = data.RoleActivitySystems.Where(p => p.SystemID == 1 && roleIDs.Contains(p.RoleID)).ToList();
                data.ActivityDtos = data.RoleActivitySystems.Where(p => p.SystemID == 1 && roleIDs.Contains(p.RoleID)).Select(p => new ActivityDTO
                {
                    ID = p.ActivityID,
                    Title = p.ActivityTitle,
                    Name = p.ActivityName,
                }).ToList();
                var brList = data.UserRoleBranchSystems.Where(p => p.BranchID.HasValue).Select(p => p.BranchID.Value).Distinct().ToList();
                data.Branches = brList;
                if (brList.Any())
                    data.BranchList = data.BranchList?.Where(p => brList.Contains(p.BranchCode.Value)).ToList();
                else
                    data.BranchList = null;
            }
            else
                return new LoginUserDTO() { ID = 0 };

            return data;
        }

        public async Task<UserDto> GetUserByID(int userID)
        {
            RestClient client = new RestClient();
            var requestUri = new Uri(GlobalConfig.Instance.UserManagementApi + "/api/Users/get/" + userID);
            var request = new RestRequest(requestUri, Method.Get);
            request.AddHeader("content-type", "application/json; charset=utf-8");
            request.AddHeader("Authorization", "Bearer " + accessToken);
            var responce = client.Execute<ResultObject<UserDto>>(request);
            if (responce != null && responce.Data != null)
            {
                return responce.Data.Data;
            }
            return new UserDto();
        }
        public async Task<List<UserDto>> GetUserList(UserFilterDTO filter, int? systemID = 1)
        {
            RestClient client = new RestClient();
            filter.SystemID = systemID;
            var requestUri = new Uri(GlobalConfig.Instance.UserManagementApi + "/api/Users/summary/list");
            var request = new RestRequest(requestUri, Method.Post);
            request.AddJsonBody(filter);
            request.AddHeader("content-type", "application/json; charset=utf-8");
            request.AddHeader("Authorization", "Bearer " + accessToken);
            var responce = client.Execute<ResultList<UserDto>>(request);
            if (responce != null && responce.Data != null)
            {
                return responce.Data.Results;
            }
            return new List<UserDto>();
        }
        public ResultObject<int> LogoutUser()
        {
            RestClient client = new RestClient();
            var requestUri = new Uri(GlobalConfig.Instance.UserManagementApi + "/api/Auth/session/logout");
            var request = new RestRequest(requestUri, Method.Get);
            request.AddHeader("content-type", "application/json; charset=utf-8");
            request.AddHeader("Authorization", "Bearer " + accessToken);
            var responce = client.Execute<ResultObject<int>>(request);
            return new ResultObject<int>();
        }

        public async Task<List<UserDto>> GetUserRoleActivity(UserFilterDTO filter)
        {
            RestClient client = new RestClient();
            filter.SystemID = 1;
            var requestUri = new Uri(GlobalConfig.Instance.UserManagementApi + "/api/Users/roleActivities/list");
            var request = new RestRequest(requestUri, Method.Post);
            request.AddJsonBody(filter);
            request.AddHeader("content-type", "application/json; charset=utf-8");
            request.AddHeader("Authorization", "Bearer " + accessToken);
            var responce = client.Execute<ResultList<UserDto>>(request);
            if (responce != null && responce.Data != null)
            {
                return responce.Data.Results;
            }
            return new List<UserDto>();
        }

        public async Task<List<KeyValueDto>> GetRolesList(UserFilterDTO filter)
        {
            RestClient client = new RestClient();
            filter.SystemID = 1;
            var requestUri = new Uri(GlobalConfig.Instance.UserManagementApi + "/api/Common/role/list");
            var request = new RestRequest(requestUri, Method.Post);
            request.AddJsonBody(filter);
            request.AddHeader("content-type", "application/json; charset=utf-8");
            request.AddHeader("Authorization", "Bearer " + accessToken);
            var responce = client.Execute<ResultList<KeyValueDto>>(request);
            if (responce != null && responce.Data != null)
            {
                return responce.Data.Results;
            }
            return new List<KeyValueDto>();
        }
    }
}
