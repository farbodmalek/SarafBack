using MediatR;
using ShopMicroservice.Core.Interfaces;

namespace ShopMicroservice.Application.Commands.Product
{
    public class DeleteProductCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }

        public DeleteProductCommand(Guid id)
        {
            Id = id;
        }
    }

    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Unit>
    {
        private readonly IProductRepositoryWrite _repositoryWrite;

        public DeleteProductCommandHandler(IProductRepositoryWrite repositoryWrite)
        {
            _repositoryWrite = repositoryWrite;
        }

        public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            await _repositoryWrite.DeleteAsync(request.Id, cancellationToken);
            await _repositoryWrite.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
