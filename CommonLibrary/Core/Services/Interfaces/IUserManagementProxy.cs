using CommonLibrary.Application.DTO.Common;
using CommonLibrary.Core.Domain;
using CommonLibrary.Core.Domain.Dto;

namespace CommonLibrary.Core.Services.Interfaces
{
    public interface IUserManagementProxy
    {
        Task<List<UserDto>> GetUserList(UserFilterDTO filter, int? systemID = 1);
        Task<List<UserDto>> GetUserRoleActivity(UserFilterDTO filter);
        Task<UserDto> GetUserByID(int userID);
        ResultObject<int> LogoutUser();

        Task<List<KeyValueDto>> GetRolesList(UserFilterDTO filter);
    }
}
