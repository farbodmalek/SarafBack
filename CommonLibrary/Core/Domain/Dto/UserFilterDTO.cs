

namespace CommonLibrary.Core.Domain.Dto
{
    public class UserFilterDTO : BaseFilter
    {
        public int? SystemID { get; set; }
        public string? FullName { get; set; }
        public string? Keyword { get; set; }
        public int? RoleID { get; set; }
        public int? SematID { get; set; }
        public bool? IsActive { get; set; }
        public List<int>? Branches { get; set; }
        public List<int>? SupervisorList { get; set; }
        public List<int>? UserIds { get; set; }
        public List<int>? RoleIds { get; set; }
        public List<int>? ActivityIds { get; set; }
    }
}
