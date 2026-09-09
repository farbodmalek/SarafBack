
namespace CommonLibrary.Core.Domain.Dto
{
    public class BranchDto
    {
        public int Id { get; set; }
        public int SupervisorCode { get; set; }
        public string SupervisorName { get; set; }
        public int BranchCode { get; set; }
        public string BranchName { get; set; }
        public string CityName { get; set; }
        public string ProvinceName { get; set; }
    }
}
