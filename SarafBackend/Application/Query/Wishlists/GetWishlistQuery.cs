using MediatR;
using ShopMicroservice.Application.DTO;
using ShopMicroservice.Core.Interfaces;

namespace ShopMicroservice.Application.Query.Wishlists
{
    public class GetWishlistQuery : IRequest<List<ProductListItemDto>>
    {
        public int UserId { get; set; }

        public GetWishlistQuery(int userId)
        {
            UserId = userId;
        }
    }

    public class GetWishlistQueryHandler : IRequestHandler<GetWishlistQuery, List<ProductListItemDto>>
    {
        private readonly IWishlistRepositoryRead _repository;

        public GetWishlistQueryHandler(IWishlistRepositoryRead repository)
        {
            _repository = repository;
        }

        public Task<List<ProductListItemDto>> Handle(GetWishlistQuery request, CancellationToken cancellationToken)
            => _repository.GetListAsync(request.UserId, cancellationToken);
    }

    /// <summary>فقط شناسه‌ی محصولات علاقه‌مندی؛ برای پر کردن قلب‌های لیست محصولات.</summary>
    public class GetWishlistProductIdsQuery : IRequest<List<Guid>>
    {
        public int UserId { get; set; }

        public GetWishlistProductIdsQuery(int userId)
        {
            UserId = userId;
        }
    }

    public class GetWishlistProductIdsQueryHandler : IRequestHandler<GetWishlistProductIdsQuery, List<Guid>>
    {
        private readonly IWishlistRepositoryRead _repository;

        public GetWishlistProductIdsQueryHandler(IWishlistRepositoryRead repository)
        {
            _repository = repository;
        }

        public Task<List<Guid>> Handle(GetWishlistProductIdsQuery request, CancellationToken cancellationToken)
            => _repository.GetProductIdsAsync(request.UserId, cancellationToken);
    }
}
