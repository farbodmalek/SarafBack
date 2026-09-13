using MediatR;
using ShopMicroservice.Application.DTO;
using ShopMicroservice.Core.Interfaces;

namespace ShopMicroservice.Application.Query.Product
{
    public class GetProductDetailByIdQuery : IRequest<ProductDetailDto?>
    {
        public Guid Id { get; set; }

        public GetProductDetailByIdQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetProductDetailByIdQueryHandler
        : IRequestHandler<GetProductDetailByIdQuery, ProductDetailDto?>
    {
        private readonly IProductRepositoryRead _repositoryRead;

        public GetProductDetailByIdQueryHandler(IProductRepositoryRead repositoryRead)
        {
            _repositoryRead = repositoryRead;
        }

        public Task<ProductDetailDto?> Handle(GetProductDetailByIdQuery request, CancellationToken cancellationToken)
        {
            return _repositoryRead.GetByIdAsync(request.Id, cancellationToken);
        }
    }
}
