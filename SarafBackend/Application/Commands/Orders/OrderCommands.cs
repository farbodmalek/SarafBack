using MediatR;
using ShopMicroservice.Core.Domain;
using ShopMicroservice.Core.Exceptions;
using ShopMicroservice.Core.Interfaces;

namespace ShopMicroservice.Application.Commands.Orders
{
    // =============================== ثبت سفارش ===============================
    /// <summary>سفارش از روی «سبد خرید سرور» ساخته می‌شود؛ قیمت‌ها همیشه از دیتابیس می‌آیند، نه از فرانت.</summary>
    public class PlaceOrderCommand : IRequest<string>
    {
        public int UserId { get; set; }
        public int AddressId { get; set; }
        public string ShippingMethod { get; set; } = default!;
        public string PaymentMethod { get; set; } = default!;
    }

    public class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, string>
    {
        private readonly IOrderRepositoryWrite _orders;
        private readonly IAddressRepositoryRead _addresses;

        public PlaceOrderCommandHandler(IOrderRepositoryWrite orders, IAddressRepositoryRead addresses)
        {
            _orders = orders;
            _addresses = addresses;
        }

        public async Task<string> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
        {
            if (!OrderOptions.ShippingMethods.TryGetValue(request.ShippingMethod ?? string.Empty, out var shipping))
            {
                throw new BusinessException("روش ارسال معتبر نیست.");
            }
            if (!OrderOptions.PaymentMethods.ContainsKey(request.PaymentMethod ?? string.Empty))
            {
                throw new BusinessException("روش پرداخت معتبر نیست.");
            }

            var address = await _addresses.GetByIdAsync(request.UserId, request.AddressId, cancellationToken)
                ?? throw new BusinessException("آدرس انتخاب‌شده پیدا نشد.");

            var lines = await _orders.GetCartLinesAsync(request.UserId, cancellationToken);
            if (lines.Count == 0)
            {
                throw new BusinessException("سبد خرید خالی است.");
            }

            var unavailable = lines.FirstOrDefault(l => !l.IsAvailable);
            if (unavailable is not null)
            {
                throw new BusinessException($"محصول «{unavailable.ProductName}» دیگر در دسترس نیست؛ از سبد خرید حذفش کن.");
            }

            var subtotal = lines.Sum(l => l.UnitPrice * l.Quantity);

            var order = new Order
            {
                UserId = request.UserId,
                RecipientName = string.IsNullOrWhiteSpace(address.RecipientName) ? address.Title : address.RecipientName!,
                RecipientPhone = address.PhoneNumber ?? string.Empty,
                ShippingAddress = string.Join("، ",
                    new[] { address.City, address.Detail }.Where(s => !string.IsNullOrWhiteSpace(s))),
                ShippingMethod = request.ShippingMethod!,
                PaymentMethod = request.PaymentMethod!,
                Status = OrderStatus.Processing,
                Subtotal = subtotal,
                ShippingCost = shipping.Price,
                Total = subtotal + shipping.Price
            };

            foreach (var line in lines)
            {
                order.Items.Add(new OrderItem
                {
                    VariantId = line.VariantId,
                    ProductId = line.ProductId,
                    ProductName = line.ProductName,
                    VariantDescription = line.VariantDescription,
                    Sku = line.Sku,
                    UnitPrice = line.UnitPrice,
                    Quantity = line.Quantity,
                    ImageUrl = line.ImageUrl
                });
            }

            // همه‌چیز یا هیچ‌چیز: اگر موجودی یک آیتم کم بود، کل تراکنش برمی‌گردد
            await using var transaction = await _orders.BeginTransactionAsync(cancellationToken);

            foreach (var line in lines)
            {
                var reserved = await _orders.TryReserveStockAsync(line.VariantId, line.Quantity, cancellationToken);
                if (!reserved)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw new BusinessException($"موجودی «{line.ProductName}» برای این تعداد کافی نیست.");
                }
            }

            await _orders.AddAsync(order, cancellationToken);
            await _orders.SaveChangesAsync(cancellationToken);
            await _orders.ClearCartAsync(request.UserId, cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            // شماره‌ی سفارش (AV-100001) را خود دیتابیس ساخته و EF بعد از Insert خوانده است
            return order.OrderNumber;
        }
    }

    // =============================== لغو سفارش (مشتری) ===============================
    public class CancelOrderCommand : IRequest<Unit>
    {
        public int UserId { get; set; }
        public string OrderNumber { get; set; } = default!;
    }

    public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Unit>
    {
        private readonly IOrderRepositoryWrite _orders;

        public CancelOrderCommandHandler(IOrderRepositoryWrite orders)
        {
            _orders = orders;
        }

        public async Task<Unit> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orders.GetTrackedByNumberAsync(request.OrderNumber, cancellationToken);

            // سفارش کس دیگری را «پیدا نشد» نشان می‌دهیم، نه «ممنوع»
            if (order is null || order.UserId != request.UserId)
            {
                throw new KeyNotFoundException("سفارش پیدا نشد.");
            }
            if (order.Status != OrderStatus.Processing)
            {
                throw new BusinessException("فقط سفارشِ «در حال پردازش» قابل لغو است.");
            }

            await OrderStatusChanger.ApplyAsync(_orders, order, OrderStatus.Cancelled, cancellationToken);
            return Unit.Value;
        }
    }

    // =============================== تغییر وضعیت (ادمین) ===============================
    public class UpdateOrderStatusCommand : IRequest<Unit>
    {
        public string OrderNumber { get; set; } = default!;
        public string Status { get; set; } = default!;
    }

    public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, Unit>
    {
        private readonly IOrderRepositoryWrite _orders;

        public UpdateOrderStatusCommandHandler(IOrderRepositoryWrite orders)
        {
            _orders = orders;
        }

        public async Task<Unit> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            if (!Enum.TryParse<OrderStatus>(request.Status, true, out var newStatus))
            {
                throw new BusinessException("وضعیت معتبر نیست (processing, shipped, delivered, cancelled).");
            }

            var order = await _orders.GetTrackedByNumberAsync(request.OrderNumber, cancellationToken)
                ?? throw new KeyNotFoundException("سفارش پیدا نشد.");

            if (order.Status == newStatus)
            {
                return Unit.Value;
            }

            // «تحویل‌شده» و «لغوشده» پایان مسیرند و دیگر عوض نمی‌شوند
            if (order.Status is OrderStatus.Delivered or OrderStatus.Cancelled)
            {
                throw new BusinessException("وضعیت این سفارش دیگر قابل تغییر نیست.");
            }

            // از «ارسال شده» نمی‌شود به «در حال پردازش» برگشت
            if (order.Status == OrderStatus.Shipped && newStatus == OrderStatus.Processing)
            {
                throw new BusinessException("سفارش ارسال‌شده به «در حال پردازش» برنمی‌گردد.");
            }

            await OrderStatusChanger.ApplyAsync(_orders, order, newStatus, cancellationToken);
            return Unit.Value;
        }
    }

    /// <summary>تغییر وضعیت؛ اگر سفارش لغو شود، موجودی کالاها به انبار برمی‌گردد (در یک تراکنش).</summary>
    internal static class OrderStatusChanger
    {
        public static async Task ApplyAsync(
            IOrderRepositoryWrite orders, Order order, OrderStatus newStatus, CancellationToken cancellationToken)
        {
            await using var transaction = await orders.BeginTransactionAsync(cancellationToken);

            if (newStatus == OrderStatus.Cancelled)
            {
                foreach (var item in order.Items.Where(i => i.VariantId.HasValue))
                {
                    await orders.ReleaseStockAsync(item.VariantId!.Value, item.Quantity, cancellationToken);
                }
            }

            order.Status = newStatus;
            order.UpdatedAt = DateTime.UtcNow;

            await orders.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
    }
}
