using CommonLibrary.Core.Domain;
using LoanMonitoringMicroService.Application.DTO.Cartable;
using LoanMonitoringMicroService.Core.DomainServices.Interfaces;
using MediatR;

namespace LoanMonitoringMicroService.Application.Queries.Cartable
{
    public class SetCartableCommand : UserCartableDto, IRequest<ResultObject<int>>
    {       
        public class GetLastCartableLoanIdQueryHandler : IRequestHandler<SetCartableCommand, ResultObject<int>>
        {
            private readonly ICartableRepositryWrite CartableRepositryWrite;
            public GetLastCartableLoanIdQueryHandler(ICartableRepositryWrite _CartableRepositryWrite)
            {
                CartableRepositryWrite = _CartableRepositryWrite;
            }
            public async Task<ResultObject<int>> Handle(SetCartableCommand request, CancellationToken cancellationToken)
            {
                var result = new ResultObject<int>();
                result = await CartableRepositryWrite.SetCartable(request);
                return result;
            }
        }
    }
}
