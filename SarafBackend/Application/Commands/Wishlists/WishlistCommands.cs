using MediatR;
using ShopMicroservice.Core.Domain;
using ShopMicroservice.Core.Interfaces;

namespace ShopMicroservice.Application.Commands.Wishlists
{
    public class AddToWishlistCommand : IRequest<Unit>
    {
        public int UserId { get; set; }
        public Guid ProductId { get; set; }
    }

    public class AddToWishlistCommandHandler : IRequestHandler<AddToWishlistCommand, Unit>
    {
        private readonly IWishlistRepositoryWrite _repository;

        public AddToWishlistCommandHandler(IWishlistRepositoryWrite repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(AddToWishlistCommand request, CancellationToken cancellationToken)
        {
            if (!await _repository.ProductExistsAsync(request.ProductId, cancellationToken))
            {
                throw new KeyNotFoundException("محصول پیدا نشد.");
            }

            // اگر از قبل بود کاری نمی‌کنیم (تکراری نمی‌شود)
            if (!await _repository.ExistsAsync(request.UserId, request.ProductId, cancellationToken))
            {
                await _repository.AddAsync(new WishlistItem
                {
                    UserId = request.UserId,
                    ProductId = request.ProductId
                }, cancellationToken);
                await _repository.SaveChangesAsync(cancellationToken);
            }

            return Unit.Value;
        }
    }

    public class RemoveFromWishlistCommand : IRequest<Unit>
    {
        public int UserId { get; set; }
        public Guid ProductId { get; set; }
    }

    public class RemoveFromWishlistCommandHandler : IRequestHandler<RemoveFromWishlistCommand, Unit>
    {
        private readonly IWishlistRepositoryWrite _repository;

        public RemoveFromWishlistCommandHandler(IWishlistRepositoryWrite repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(RemoveFromWishlistCommand request, CancellationToken cancellationToken)
        {
            await _repository.RemoveAsync(request.UserId, request.ProductId, cancellationToken);
            return Unit.Value;
        }
    }
}
