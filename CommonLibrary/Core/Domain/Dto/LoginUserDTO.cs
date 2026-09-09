using CommonLibrary.Application.DTO.Common;

namespace CommonLibrary.Core.Domain.Dto
{
    public class LoginUserDTO
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get { return this.FirstName + " " + this.LastName; } }
        public bool Gender { get; set; }
        public string Token { get; set; }
        public string NationalCode { get; set; }
        public string Mobile { get; set; }
        public string UID { get; set; }
        public string? SematTitle { get; set; }
        public List<ActivityDTO> ActivityDtos { get; set; }
        public List<UserRoleDto> Roles { get; set; }
        //public List<KeyValueDto> Systems { get; set; }
        public List<int>? Branches { get; set; }
        public List<BranchDTO>? BranchList { get; set; }
        public List<UserRoleBranchSystemDTO> UserRoleBranchSystems { get; set; }
        public List<RoleActivitySystemDTO> RoleActivitySystems { get; set; }
    }
    public class UserRoleBranchSystemDTO
    {
        public int ID { get; set; }
        public string RoleTitle { get; set; }
        public int RoleID { get; set; }
        public string RoleDesc { get; set; }
        public int? SystemID { get; set; }
        public string SystemTitle { get; set; }
        public string NationalCode { get; set; }
        public string FullName { get; set; }
        public int UserID { get; set; }
        public int? BranchID { get; set; }
        public string? BranchName { get; set; }
        public string? SupervisorName { get; set; }
        public int? SupervisorCode { get; set; }
    }
    public class RoleActivitySystemDTO
    {
        public int ID { get; set; }
        public string RoleTitle { get; set; }
        public int RoleID { get; set; }
        public string RoleDesc { get; set; }
        public int? SystemID { get; set; }
        public string SystemTitle { get; set; }
        public string ActivityName { get; set; }
        public string ActivityTitle { get; set; }
        public int ActivityID { get; set; }
        public int? CategoryID { get; set; }
        public string? CategoryTitle { get; set; }
    }
    public class ActivityDTO
    {
        public int ID { get; set; } = 27;
        public int? CategoryID { get; set; }
        public string CategoryTitle { get; set; }
        public int Code { get; set; }
        public int Order { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }
        public string IconClass { get; set; }
        public bool IsSelected { get; set; } = false;
    }
    public class UserRoleDto
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string RoleTitle { get; set; }
        public int RoleID { get; set; }
        public string RoleDesc { get; set; }
        public int DivisionType { get; set; }
        public int LevelID { get; set; }
    }
    public class BranchDTO
    {
        public int? Id { get; set; }
        public int? SupervisorCode { get; set; }
        public string? SupervisorName { get; set; }
        public int? BranchCode { get; set; }
        public string? BranchName { get; set; }
        public string? CityName { get; set; }
        public string? ProvinceName { get; set; }
        public string? RelatedSupervisors { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public bool? IsMetropolis { get; set; }
    }
}
