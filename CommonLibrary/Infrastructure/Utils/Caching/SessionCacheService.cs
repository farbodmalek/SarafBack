using CommonLibrary.Core.Domain.Dto;
using CommonLibrary.Core.Domain;
using CommonLibrary.Core.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace CommonLibrary.Infrastructure.Utils.Caching
{
    public class SessionCacheService : ISessionCacheService
    {
        private readonly ICacheService cacheService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public SessionCacheService(ICacheService _cacheService, IHttpContextAccessor _httpContextAccessor)
        {
            httpContextAccessor = _httpContextAccessor;
            cacheService = _cacheService;
        }
        public ResultObject<int> SetUserSession(LoginUserDTO user)
        {
            var result = new ResultObject<int>();
            cacheService.SetData<LoginUserDTO>(user.UID, user, DateTimeOffset.Now.AddHours(5));
            cacheService.SetData<string>($"{user.UID}-{user.UID}", httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress.ToString()
                , DateTimeOffset.Now.AddHours(5));
            return result;
        }
        public ResultObject<LoginUserDTO> GetUserSession(string guid)
        {
            var result = new ResultObject<LoginUserDTO>();
            var cacheData = cacheService.GetData<LoginUserDTO>(guid);
            if (cacheData != null)
                result.Data = cacheData;
            return result;
        }
        public bool ValidateUserSession(string guid)
        {
            var cacheData = cacheService.GetData<LoginUserDTO>(guid);
            var ipData = cacheService.GetData<string>($"{guid}-{guid}");
            var ip = httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress.ToString();
            if (cacheData != null)
            {
                return true;
            }
            return false;
        }
        public void RemoveUserSession(string username)
        {
            cacheService.RemoveData(username);
        }
    }
}
