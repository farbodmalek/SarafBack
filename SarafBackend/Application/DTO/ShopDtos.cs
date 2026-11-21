namespace ShopMicroservice.Application.DTO
{
    // ---------------- دسته‌بندی ----------------
    public class CategoryDto
    {
        public long Id { get; set; }
        public long? ParentId { get; set; }
        public string Name { get; set; } = default!;
        public string Slug { get; set; } = default!;
        public bool IsActive { get; set; }
        public int ProductCount { get; set; }
    }

    // ---------------- سبد خرید ----------------
    public class CartAttributeDto
    {
        public string Name { get; set; } = default!;
        public string Value { get; set; } = default!;
    }

    public class CartItemDto
    {
        public Guid VariantId { get; set; }
        public Guid ProductId { get; set; }
        public string Title { get; set; } = default!;
        public string? Image { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int Stock { get; set; }
        public List<CartAttributeDto> Attributes { get; set; } = new();
    }

    public class CartDto
    {
        public List<CartItemDto> Items { get; set; } = new();
        public int TotalQuantity => Items.Sum(i => i.Quantity);
        public decimal Subtotal => Items.Sum(i => i.Price * i.Quantity);
    }

    // ---------------- آدرس ----------------
    public class AddressDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string? RecipientName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string Detail { get; set; } = default!;
        public bool IsDefault { get; set; }
    }

    // ---------------- سفارش ----------------
    public class OrderListItemDto
    {
        public string OrderNumber { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public int ItemsCount { get; set; }
        public decimal Total { get; set; }
        /// <summary>processing | shipped | delivered | cancelled</summary>
        public string Status { get; set; } = default!;
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
    }

    public class OrderItemDto
    {
        public Guid? ProductId { get; set; }
        public string ProductName { get; set; } = default!;
        public string? VariantDescription { get; set; }
        public string? Sku { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class OrderDetailDto
    {
        public string OrderNumber { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = default!;
        public string RecipientName { get; set; } = default!;
        public string RecipientPhone { get; set; } = default!;
        public string ShippingAddress { get; set; } = default!;
        public string ShippingMethod { get; set; } = default!;
        public string ShippingMethodTitle { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = default!;
        public string PaymentMethodTitle { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Total { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class OrderFilterDto
    {
        /// <summary>processing | shipped | delivered | cancelled (خالی = همه)</summary>
        public string? Status { get; set; }
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    // ---------------- ادمین ----------------
    public class AdminProductListItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Slug { get; set; } = default!;
        public decimal BasePrice { get; set; }
        public string Status { get; set; } = default!;
        public string CategoryName { get; set; } = default!;
        public bool IsFeatured { get; set; }
        public int TotalStock { get; set; }
        public string? PrimaryImageUrl { get; set; }
    }

    public class DashboardDto
    {
        public int TotalProducts { get; set; }
        public int TodayOrders { get; set; }
        public int PendingOrders { get; set; }
        public int UsersCount { get; set; }
        public decimal MonthSales { get; set; }
    }
}
