using CommonLibrary.Core.Domain;
using LoanMonitoringMicroService.Core.DomainServices.Interfaces;
using MediatR;

namespace LoanMonitoringMicroService.Application.Commands.Cartable
{
    public class RemoveCartableListCommand : IRequest<ResultObject<int>>
    {
        public List<int> Ids { get; set; }
        public class RemoveCartableListCommandHandler : IRequestHandler<RemoveCartableListCommand, ResultObject<int>>
        {
            private readonly ICartableRepositryWrite cartableRepositryWrite;
            public RemoveCartableListCommandHandler(ICartableRepositryWrite _cartableRepositryWrite)
            {
                cartableRepositryWrite = _cartableRepositryWrite;
            }
            public async Task<ResultObject<int>> Handle(RemoveCartableListCommand request, CancellationToken cancellationToken)
            {
                var result = new ResultObject<int>();
                result = await cartableRepositryWrite.RemoveCartableList(request.Ids);
                return result;
            }
        }
    }
}
