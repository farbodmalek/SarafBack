using MediatR;
using ShopMicroservice.Application.DTO;
using ShopMicroservice.Core.Interfaces;

namespace ShopMicroservice.Application.Query.Product
{
    /// <summary>لیست جامع همه محصولات — مناسب صفحه فروشگاه اصلی با فیلترها.</summary>
    public class GetComprehensiveProductQuery : ProductFilterDto, IRequest<PagedResult<ProductListItemDto>>
    {
        public ProductFilterDto Filter { get; set; } = new();
    }

    public class GetComprehensiveProductQueryHandler
        : IRequestHandler<GetComprehensiveProductQuery, PagedResult<ProductListItemDto>>
    {
        private readonly IProductRepositoryRead _repositoryRead;

        public GetComprehensiveProductQueryHandler(IProductRepositoryRead repositoryRead)
        {
            _repositoryRead = repositoryRead;
        }

        public Task<PagedResult<ProductListItemDto>> Handle(
            GetComprehensiveProductQuery request, CancellationToken cancellationToken)
        {
            return _repositoryRead.GetComprehensiveListAsync(request.Filter, cancellationToken);
        }
    }
}
