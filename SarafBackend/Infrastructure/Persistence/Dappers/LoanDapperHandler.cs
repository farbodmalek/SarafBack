using CommonLibrary.Infrastructure.Persistence.Dappers;

namespace LoanMicroService.Infrastructure.Persistence.Dappers
{
    public class LoanMonitoringDapperHandler : DapperHandler
    {
        public LoanMonitoringDapperHandler(IConfiguration configuration) : base(configuration.GetConnectionString("PaymentMonitoringConnectionString"))
        {
        }
    }
}
