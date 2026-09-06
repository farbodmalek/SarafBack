namespace LoanMonitoringMicroService.Core.Domain.ViewModel.Cartable
{
    public class UserCartableVM
    {
        public List<int> LoansId { get; set; }
        public int UserId { get; set; }
        public bool? IsNotScheduledReferred { get; set; }
        public string? Description { get; set; }

    }
}
