using MediatR;
using ShopMicroservice.Core.Domain;
using ShopMicroservice.Core.Exceptions;
using ShopMicroservice.Core.Interfaces;

namespace ShopMicroservice.Application.Commands.Carts
{
    // ---------- افزودن به سبد (اگر از قبل بود، تعدادش زیاد می‌شود) ----------
    public class AddCartItemCommand : IRequest<Unit>
    {
        public int UserId { get; set; }
        public Guid VariantId { get; set; }
        public int Quantity { get; set; } = 1;
    }

    public class AddCartItemCommandHandler : IRequestHandler<AddCartItemCommand, Unit>
    {
        private readonly ICartRepositoryWrite _repository;

        public AddCartItemCommandHandler(ICartRepositoryWrite repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(AddCartItemCommand request, CancellationToken cancellationToken)
        {
            if (request.Quantity <= 0)
            {
                throw new BusinessException("تعداد باید حداقل ۱ باشد.");
            }

            var variant = await _repository.GetPurchasableVariantAsync(request.VariantId, cancellationToken)
                ?? throw new KeyNotFoundException("این محصول دیگر در دسترس نیست.");

            var item = await _repository.GetItemAsync(request.UserId, request.VariantId, cancellationToken);
            var newQuantity = (item?.Quantity ?? 0) + request.Quantity;

            if (newQuantity > variant.StockQuantity)
            {
                throw new BusinessException(
                    variant.StockQuantity <= 0
                        ? "این گزینه فعلاً ناموجود است."
                        : $"فقط {variant.StockQuantity} عدد از این گزینه موجود است.");
            }

            if (item is null)
            {
                await _repository.AddAsync(new CartItem
                {
                    UserId = request.UserId,
                    VariantId = request.VariantId,
                    Quantity = newQuantity
                }, cancellationToken);
            }
            else
            {
                item.Quantity = newQuantity;
                item.UpdatedAt = DateTime.UtcNow;
            }

            await _repository.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }

    // ---------- تغییر تعداد (۰ یا کمتر = حذف) ----------
    public class SetCartItemQuantityCommand : IRequest<Unit>
    {
        public int UserId { get; set; }
        public Guid VariantId { get; set; }
        public int Quantity { get; set; }
    }

    public class SetCartItemQuantityCommandHandler : IRequestHandler<SetCartItemQuantityCommand, Unit>
    {
        private readonly ICartRepositoryWrite _repository;

        public SetCartItemQuantityCommandHandler(ICartRepositoryWrite repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(SetCartItemQuantityCommand request, CancellationToken cancellationToken)
        {
            var item = await _repository.GetItemAsync(request.UserId, request.VariantId, cancellationToken)
                ?? throw new KeyNotFoundException("این آیتم در سبد خرید نیست.");

            if (request.Quantity <= 0)
            {
                _repository.Remove(item);
            }
            else
            {
                var variant = await _repository.GetPurchasableVariantAsync(request.VariantId, cancellationToken)
                    ?? throw new BusinessException("این محصول دیگر در دسترس نیست.");

                if (request.Quantity > variant.StockQuantity)
                {
                    throw new BusinessException($"فقط {variant.StockQuantity} عدد از این گزینه موجود است.");
                }

                item.Quantity = request.Quantity;
                item.UpdatedAt = DateTime.UtcNow;
            }

            await _repository.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }

    // ---------- حذف یک آیتم ----------
    public class RemoveCartItemCommand : IRequest<Unit>
    {
        public int UserId { get; set; }
        public Guid VariantId { get; set; }
    }

    public class RemoveCartItemCommandHandler : IRequestHandler<RemoveCartItemCommand, Unit>
    {
        private readonly ICartRepositoryWrite _repository;

        public RemoveCartItemCommandHandler(ICartRepositoryWrite repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
        {
            var item = await _repository.GetItemAsync(request.UserId, request.VariantId, cancellationToken);
            if (item is not null)
            {
                _repository.Remove(item);
                await _repository.SaveChangesAsync(cancellationToken);
            }
            return Unit.Value;
        }
    }

    // ---------- خالی کردن سبد ----------
    public class ClearCartCommand : IRequest<Unit>
    {
        public int UserId { get; set; }
    }

    public class ClearCartCommandHandler : IRequestHandler<ClearCartCommand, Unit>
    {
        private readonly ICartRepositoryWrite _repository;

        public ClearCartCommandHandler(ICartRepositoryWrite repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(ClearCartCommand request, CancellationToken cancellationToken)
        {
            await _repository.ClearAsync(request.UserId, cancellationToken);
            return Unit.Value;
        }
    }

    // ---------- ادغام سبد مهمان (localStorage) با سبد سرور، بعد از ورود ----------
    public class MergeCartLine
    {
        public Guid VariantId { get; set; }
        public int Quantity { get; set; }
    }

    public class MergeCartCommand : IRequest<Unit>
    {
        public int UserId { get; set; }
        public List<MergeCartLine> Lines { get; set; } = new();
    }

    public class MergeCartCommandHandler : IRequestHandler<MergeCartCommand, Unit>
    {
        private readonly ICartRepositoryWrite _repository;

        public MergeCartCommandHandler(ICartRepositoryWrite repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(MergeCartCommand request, CancellationToken cancellationToken)
        {
            // اگر یک واریانت چند بار آمده بود، جمع می‌شوند
            var grouped = request.Lines
                .Where(l => l.Quantity > 0)
                .GroupBy(l => l.VariantId)
                .Select(g => new { VariantId = g.Key, Quantity = g.Sum(x => x.Quantity) });

            foreach (var line in grouped)
            {
                var variant = await _repository.GetPurchasableVariantAsync(line.VariantId, cancellationToken);
                if (variant is null || variant.StockQuantity <= 0)
                {
                    continue; // آیتم نامعتبر یا ناموجود بی‌صدا نادیده گرفته می‌شود
                }

                var item = await _repository.GetItemAsync(request.UserId, line.VariantId, cancellationToken);
                var wanted = (item?.Quantity ?? 0) + line.Quantity;
                var finalQuantity = Math.Min(wanted, variant.StockQuantity);

                if (item is null)
                {
                    await _repository.AddAsync(new CartItem
                    {
                        UserId = request.UserId,
                        VariantId = line.VariantId,
                        Quantity = finalQuantity
                    }, cancellationToken);
                }
                else
                {
                    item.Quantity = finalQuantity;
                    item.UpdatedAt = DateTime.UtcNow;
                }

                // هر خط جدا ذخیره می‌شود تا اولین نشانه‌ی تداخل، کل ادغام را خراب نکند
                await _repository.SaveChangesAsync(cancellationToken);
            }

            return Unit.Value;
        }
    }
}
