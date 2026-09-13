using MediatR;
using ShopMicroservice.Application.DTO;
using ShopMicroservice.Core.Interfaces;

namespace ShopMicroservice.Application.Query.Product
{
    public class GetProductListByCategoryQuery : IRequest<PagedResult<ProductListItemDto>>
    {
        public long CategoryId { get; set; }
        public ProductFilterDto Filter { get; set; } = new();
    }

    public class GetProductListByCategoryQueryHandler
        : IRequestHandler<GetProductListByCategoryQuery, PagedResult<ProductListItemDto>>
    {
        private readonly IProductRepositoryRead _repositoryRead;

        public GetProductListByCategoryQueryHandler(IProductRepositoryRead repositoryRead)
        {
            _repositoryRead = repositoryRead;
        }

        public Task<PagedResult<ProductListItemDto>> Handle(
            GetProductListByCategoryQuery request, CancellationToken cancellationToken)
        {
            return _repositoryRead.GetListByCategoryAsync(request.CategoryId, request.Filter, cancellationToken);
        }
    }
}
