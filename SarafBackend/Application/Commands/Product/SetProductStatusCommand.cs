using MediatR;
using ShopMicroservice.Core.Domain;
using ShopMicroservice.Core.Exceptions;
using ShopMicroservice.Core.Interfaces;

namespace ShopMicroservice.Application.Commands.Product
{
    /// <summary>منتشر کردن، پیش‌نویس کردن یا بایگانی کردن یک محصول (Draft | Published | Archived).</summary>
    public class SetProductStatusCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string Status { get; set; } = default!;
    }

    public class SetProductStatusCommandHandler : IRequestHandler<SetProductStatusCommand, Unit>
    {
        private readonly IProductRepositoryWrite _repositoryWrite;

        public SetProductStatusCommandHandler(IProductRepositoryWrite repositoryWrite)
        {
            _repositoryWrite = repositoryWrite;
        }

        public async Task<Unit> Handle(SetProductStatusCommand request, CancellationToken cancellationToken)
        {
            if (!Enum.TryParse<ProductStatus>(request.Status, true, out var status))
            {
                throw new BusinessException("وضعیت معتبر نیست (Draft, Published, Archived).");
            }

            var product = await _repositoryWrite.GetTrackedByIdAsync(request.Id, cancellationToken)
                ?? throw new KeyNotFoundException("محصول پیدا نشد.");

            product.Status = status;
            await _repositoryWrite.UpdateAsync(product, cancellationToken);
            await _repositoryWrite.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
