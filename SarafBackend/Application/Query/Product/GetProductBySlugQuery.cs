using MediatR;
using ShopMicroservice.Application.DTO;
using ShopMicroservice.Core.Interfaces;

namespace ShopMicroservice.Application.Query.Product
{
    public class GetProductBySlugQuery : IRequest<ProductDetailDto?>
    {
        public string Slug { get; set; } = default!;

        public GetProductBySlugQuery(string slug)
        {
            Slug = slug;
        }
    }

    public class GetProductBySlugQueryHandler
        : IRequestHandler<GetProductBySlugQuery, ProductDetailDto?>
    {
        private readonly IProductRepositoryRead _repositoryRead;

        public GetProductBySlugQueryHandler(IProductRepositoryRead repositoryRead)
        {
            _repositoryRead = repositoryRead;
        }

        public Task<ProductDetailDto?> Handle(GetProductBySlugQuery request, CancellationToken cancellationToken)
        {
            return _repositoryRead.GetBySlugAsync(request.Slug, cancellationToken);
        }
    }
}
