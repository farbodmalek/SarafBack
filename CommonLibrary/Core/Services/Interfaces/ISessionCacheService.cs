using CommonLibrary.Core.Domain.Dto;
using CommonLibrary.Core.Domain;

namespace CommonLibrary.Core.Services.Interfaces
{
    public interface ISessionCacheService
    {
        ResultObject<int> SetUserSession(LoginUserDTO user);
        ResultObject<LoginUserDTO> GetUserSession(string guid);
        void RemoveUserSession(string username);
        bool ValidateUserSession(string guid);
    }
}
