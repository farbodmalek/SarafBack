namespace ShopMicroservice.Core.Domain
{
    public enum OrderStatus
    {
        Processing = 0,
        Shipped = 1,
        Delivered = 2,
        Cancelled = 3
    }

    public class Address
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = default!;
        public string? RecipientName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string Detail { get; set; } = default!;
        public bool IsDefault { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class CartItem
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public Guid VariantId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

    public class WishlistItem
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public Guid ProductId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class Order
    {
        public int Id { get; set; }

        /// <summary>در دیتابیس ساخته می‌شود (AV-100001، ...). از روی Id محاسبه می‌شود.</summary>
        public string OrderNumber { get; set; } = default!;

        public int UserId { get; set; }
        public string RecipientName { get; set; } = default!;
        public string RecipientPhone { get; set; } = default!;
        public string ShippingAddress { get; set; } = default!;
        public string ShippingMethod { get; set; } = default!;
        public string PaymentMethod { get; set; } = default!;
        public OrderStatus Status { get; set; } = OrderStatus.Processing;
        public decimal Subtotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }

    /// <summary>«عکس لحظه‌ی خرید»: نام و قیمت همان لحظه ذخیره می‌شود تا ویرایش محصول، سفارش قدیمی را عوض نکند.</summary>
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Guid? VariantId { get; set; }
        public Guid? ProductId { get; set; }
        public string ProductName { get; set; } = default!;
        public string? VariantDescription { get; set; }
        public string? Sku { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string? ImageUrl { get; set; }
    }

    /// <summary>روش‌های ارسال و پرداخت (قیمت‌ها فقط سمت سرور تعیین می‌شوند، نه از فرانت).</summary>
    public static class OrderOptions
    {
        public static readonly IReadOnlyDictionary<string, (string Title, decimal Price)> ShippingMethods =
            new Dictionary<string, (string Title, decimal Price)>
            {
                ["standard"] = ("پست پیشتاز", 45000m),
                ["express"] = ("ارسال اکسپرس", 120000m),
                ["pickup"] = ("تحویل حضوری از فروشگاه", 0m)
            };

        public static readonly IReadOnlyDictionary<string, string> PaymentMethods =
            new Dictionary<string, string>
            {
                ["online"] = "پرداخت آنلاین",
                ["cod"] = "پرداخت در محل"
            };
    }
}
