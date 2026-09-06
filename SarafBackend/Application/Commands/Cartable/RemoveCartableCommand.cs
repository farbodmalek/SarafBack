using CommonLibrary.Core.Domain;
using LoanMonitoringMicroService.Core.DomainServices.Interfaces;
using MediatR;

namespace LoanMonitoringMicroService.Application.Commands.Cartable
{
    public class RemoveCartableCommand : IRequest<ResultObject<int>>
    {
        public int SurveyListTypeId { get; set; }
        public int LoanId { get; set; }
        public class RemoveCartableCommandHandler : IRequestHandler<RemoveCartableCommand, ResultObject<int>>
        {
            private readonly ICartableRepositryWrite cartableRepositryWrite;
            public RemoveCartableCommandHandler(ICartableRepositryWrite _cartableRepositryWrite)
            {
                cartableRepositryWrite = _cartableRepositryWrite;
            }
            public async Task<ResultObject<int>> Handle(RemoveCartableCommand request, CancellationToken cancellationToken)
            {
                var result = new ResultObject<int>();
                result = await cartableRepositryWrite.RemoveCartable(request.LoanId, request.SurveyListTypeId);
                return result;
            }
        }
    }
}
