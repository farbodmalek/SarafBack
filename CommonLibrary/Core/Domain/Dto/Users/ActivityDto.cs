
namespace CommonLibrary.Core.Domain.Dto.Users
{
    public class ActivityDto
    {
        public int Id { get; set; }

        public int ParentId { get; set; }
        public int Order { get; set; }
        public long Code { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public string IconClass { get; set; }
        public bool IsActive { get; set; }
        public bool IsMenu { get; set; }

    }
}
