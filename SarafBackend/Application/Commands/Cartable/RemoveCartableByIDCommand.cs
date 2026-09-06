using CommonLibrary.Core.Domain;
using LoanMonitoringMicroService.Core.DomainServices.Interfaces;
using MediatR;

namespace LoanMonitoringMicroService.Application.Commands.Cartable
{
    public class RemoveCartableByIDCommand : IRequest<ResultObject<int>>
    {
        public int ID { get; set; }
        public RemoveCartableByIDCommand(int id)
        {
            ID = id;
        }
        public class RemoveCartableByIDCommandHandler : IRequestHandler<RemoveCartableByIDCommand, ResultObject<int>>
        {
            private readonly ICartableRepositryWrite cartableRepositryWrite;
            public RemoveCartableByIDCommandHandler(ICartableRepositryWrite _cartableRepositryWrite)
            {
                cartableRepositryWrite = _cartableRepositryWrite;
            }
            public async Task<ResultObject<int>> Handle(RemoveCartableByIDCommand request, CancellationToken cancellationToken)
            {
                var result = new ResultObject<int>();
                result = await cartableRepositryWrite.RemoveCartableByID(request.ID);
                return result;
            }
        }
    }
}
