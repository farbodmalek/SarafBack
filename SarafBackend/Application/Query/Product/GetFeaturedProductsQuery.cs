using MediatR;
using ShopMicroservice.Application.DTO;
using ShopMicroservice.Core.Interfaces;

namespace ShopMicroservice.Application.Query.Product
{
    public class GetFeaturedProductsQuery : IRequest<PagedResult<ProductListItemDto>>
    {
        public ProductFilterDto Filter { get; set; } = new();
    }

    public class GetFeaturedProductsQueryHandler
        : IRequestHandler<GetFeaturedProductsQuery, PagedResult<ProductListItemDto>>
    {
        private readonly IProductRepositoryRead _repositoryRead;

        public GetFeaturedProductsQueryHandler(IProductRepositoryRead repositoryRead)
        {
            _repositoryRead = repositoryRead;
        }

        public Task<PagedResult<ProductListItemDto>> Handle(
            GetFeaturedProductsQuery request, CancellationToken cancellationToken)
        {
            return _repositoryRead.GetFeaturedListAsync(request.Filter, cancellationToken);
        }
    }
}
