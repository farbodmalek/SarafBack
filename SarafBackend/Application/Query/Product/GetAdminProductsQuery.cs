using MediatR;
using ShopMicroservice.Application.DTO;
using ShopMicroservice.Core.Interfaces;

namespace ShopMicroservice.Application.Query.Product
{
    /// <summary>لیست محصولات برای پنل ادمین (همه‌ی وضعیت‌ها + مجموع موجودی).</summary>
    public class GetAdminProductsQuery : IRequest<PagedResult<AdminProductListItemDto>>
    {
        public ProductFilterDto Filter { get; set; } = new();
    }

    public class GetAdminProductsQueryHandler
        : IRequestHandler<GetAdminProductsQuery, PagedResult<AdminProductListItemDto>>
    {
        private readonly IProductRepositoryRead _repositoryRead;

        public GetAdminProductsQueryHandler(IProductRepositoryRead repositoryRead)
        {
            _repositoryRead = repositoryRead;
        }

        public Task<PagedResult<AdminProductListItemDto>> Handle(
            GetAdminProductsQuery request, CancellationToken cancellationToken)
        {
            return _repositoryRead.GetAdminListAsync(request.Filter, cancellationToken);
        }
    }
}
