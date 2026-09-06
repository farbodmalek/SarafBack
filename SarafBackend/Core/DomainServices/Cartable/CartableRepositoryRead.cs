using CommonLibrary.Core.Domain;
using CommonLibrary.Core.Domain.Interfaces;
using CommonLibrary.Core.Domain.RepositoryInterfaces;
using LoanMonitoringMicroService.Core.Domain.RepositoryInterfaces;
using LoanMonitoringMicroService.Core.DomainServices.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LoanMonitoringMicroService.Core.DomainServices.Cartables
{
    public class CartableRepositryRead: ICartableRepositryRead
    {

        private readonly IDapperHandler dapperHandler;
        private readonly IDateConvertor dateConvertor;
        private readonly ILoanMonitoringDbContext Context;
        public CartableRepositryRead(IDapperHandler _dapperHandler, IDateConvertor _dateConvertor, ILoanMonitoringDbContext Context)
        {
            this.dapperHandler = _dapperHandler;
            this.dateConvertor = _dateConvertor;
            this.Context = Context;
        }



        public async Task<ResultObject<int>> GetLastCartableByLoanId(int request)
        {

            var Cartable = await Context.Cartables.Where(c => c.LoanId == request && !c.IsDeleted).OrderByDescending(c => c.Id).FirstOrDefaultAsync();

            return new ResultObject<int>()
            {
                Data = (Cartable == null || Cartable.CartableStatusTypeId == 2) ? 0 : Cartable.Id
            };
        }




    }
}
