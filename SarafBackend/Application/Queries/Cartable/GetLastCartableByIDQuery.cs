using CommonLibrary.Core.Domain;
using LoanMonitoringMicroService.Core.DomainServices.Interfaces;
using LoanMonitoringMicroService.Core.DomainServices.LoansActions;
using MediatR;

namespace LoanMonitoringMicroService.Application.Queries.Cartable
{


    public class GetLastCartableLoanIdQuery : IRequest<ResultObject<int>>
    {
        public int LoanId { get; set; }
        public GetLastCartableLoanIdQuery(int id)
        {
            LoanId = id;
        }
        public class GetLastCartableLoanIdQueryHandler : IRequestHandler<GetLastCartableLoanIdQuery, ResultObject<int>>
        {
            private readonly ICartableRepositryRead CartableRepositryRead;
            public GetLastCartableLoanIdQueryHandler(ICartableRepositryRead _CartableRepositryRead)
            {
                CartableRepositryRead = _CartableRepositryRead;
            }
            public async Task<ResultObject<int>> Handle(GetLastCartableLoanIdQuery request, CancellationToken cancellationToken)
            {
                var result = new ResultObject<int>();
                result = await CartableRepositryRead.GetLastCartableByLoanId(request.LoanId);
                return result;
            }
        }
    }
}
