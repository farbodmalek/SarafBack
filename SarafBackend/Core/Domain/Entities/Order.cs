namespace GirlyShopBackend.Core.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int AddressId { get; set; }
    public Address Address { get; set; } = null!;

    public int? CouponId { get; set; }
    public Coupon? Coupon { get; set; }
    public decimal DiscountAmount { get; set; }

    public string ShippingMethod { get; set; } = string.Empty;
    public decimal ShippingCost { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public OrderStatus Status { get; set; } = OrderStatus.Processing;

    public decimal Subtotal { get; set; }
    public decimal Total { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
