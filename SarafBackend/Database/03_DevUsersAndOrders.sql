/* ==========================================================================
   (اختیاری، فقط برای توسعه‌ی روی سیستم خودت) کاربر ادمین + یک مشتری نمونه + چند سفارش نمونه
   --------------------------------------------------------------------------
   ادمین:   موبایل 09120000000   رمز  Admin@12345
   مشتری:   موبایل 09121111111   رمز  Test@12345
   ⚠️ این رمزها را فقط روی سیستم شخصی استفاده کن؛ روی سرور واقعی هرگز اجرا نکن.
   بعد از 02_SeedCatalog.sql اجرا کن. تکراری نمی‌سازد.
   ========================================================================== */
USE GirlyShop;
GO
SET NOCOUNT ON;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE PhoneNumber = N'09120000000')
    INSERT INTO dbo.Users (FullName, PhoneNumber, PasswordHash, IsAdmin, IsActive)
    VALUES (N'مدیر فروشگاه', N'09120000000', N'$2b$12$0/OPQEtePPIdnPrTGRMMy.j4W7zG7QPnGNWf7yYrsuGSnY.4JwTMa', 1, 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE PhoneNumber = N'09121111111')
    INSERT INTO dbo.Users (FullName, PhoneNumber, PasswordHash, IsAdmin, IsActive)
    VALUES (N'سارا احمدی', N'09121111111', N'$2b$12$F7maJiu2lR1BgKRVmJqhCOeeO7rvRUNSN8ivFzcrIM4bNAxJ0qGAC', 0, 1);
GO

DECLARE @UserId INT = (SELECT Id FROM dbo.Users WHERE PhoneNumber = N'09121111111');

IF NOT EXISTS (SELECT 1 FROM dbo.Addresses WHERE UserId = @UserId)
BEGIN
    INSERT INTO dbo.Addresses (UserId, Title, RecipientName, PhoneNumber, City, PostalCode, Detail, IsDefault)
    VALUES
    (@UserId, N'منزل — تهران، ونک', N'سارا احمدی', N'09121111111', N'تهران', N'1991634553', N'خیابان ولیعصر، پلاک ۱۲، واحد ۳', 1),
    (@UserId, N'محل کار — تهران، سعادت‌آباد', N'سارا احمدی', N'09121111111', N'تهران', NULL, N'بلوار سعادت‌آباد، برج نگین، طبقه ۵', 0);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Orders WHERE UserId = @UserId)
BEGIN
    DECLARE @Addr NVARCHAR(700) = N'تهران، خیابان ولیعصر، پلاک ۱۲، واحد ۳';
    DECLARE @OrderId INT;

    -- سفارش ۱: تحویل شده
    INSERT INTO dbo.Orders (UserId, RecipientName, RecipientPhone, ShippingAddress, ShippingMethod, PaymentMethod, Status, Subtotal, ShippingCost, Total, CreatedAt)
    VALUES (@UserId, N'سارا احمدی', N'09121111111', @Addr, N'standard', N'online', N'Delivered', 1730000, 45000, 1775000, DATEADD(DAY, -40, SYSUTCDATETIME()));
    SET @OrderId = SCOPE_IDENTITY();
    INSERT INTO dbo.OrderItems (OrderId, VariantId, ProductId, ProductName, VariantDescription, Sku, UnitPrice, Quantity)
    SELECT @OrderId, v.Id, p.Id, p.Name, N'سایز 38 / رنگ مشکی', v.Sku, v.Price, 1 FROM dbo.ProductVariants v JOIN dbo.Products p ON p.Id = v.ProductId WHERE v.Sku = N'DRS-38-BLK';
    INSERT INTO dbo.OrderItems (OrderId, VariantId, ProductId, ProductName, VariantDescription, Sku, UnitPrice, Quantity)
    SELECT @OrderId, v.Id, p.Id, p.Name, N'سایز یک‌سایز / رنگ سفید', v.Sku, v.Price, 2 FROM dbo.ProductVariants v JOIN dbo.Products p ON p.Id = v.ProductId WHERE v.Sku = N'SOC-OS-WHT';
    UPDATE dbo.Orders SET Subtotal = (SELECT SUM(UnitPrice * Quantity) FROM dbo.OrderItems WHERE OrderId = @OrderId),
                          Total = ShippingCost + (SELECT SUM(UnitPrice * Quantity) FROM dbo.OrderItems WHERE OrderId = @OrderId)
    WHERE Id = @OrderId;

    -- سفارش ۲: ارسال شده
    INSERT INTO dbo.Orders (UserId, RecipientName, RecipientPhone, ShippingAddress, ShippingMethod, PaymentMethod, Status, Subtotal, ShippingCost, Total, CreatedAt)
    VALUES (@UserId, N'سارا احمدی', N'09121111111', @Addr, N'express', N'online', N'Shipped', 0, 120000, 0, DATEADD(DAY, -8, SYSUTCDATETIME()));
    SET @OrderId = SCOPE_IDENTITY();
    INSERT INTO dbo.OrderItems (OrderId, VariantId, ProductId, ProductName, VariantDescription, Sku, UnitPrice, Quantity)
    SELECT @OrderId, v.Id, p.Id, p.Name, N'سایز 38 / رنگ مشکی', v.Sku, v.Price, 1 FROM dbo.ProductVariants v JOIN dbo.Products p ON p.Id = v.ProductId WHERE v.Sku = N'LNG-38-BLK';
    UPDATE dbo.Orders SET Subtotal = (SELECT SUM(UnitPrice * Quantity) FROM dbo.OrderItems WHERE OrderId = @OrderId),
                          Total = ShippingCost + (SELECT SUM(UnitPrice * Quantity) FROM dbo.OrderItems WHERE OrderId = @OrderId)
    WHERE Id = @OrderId;

    -- سفارش ۳: در حال پردازش
    INSERT INTO dbo.Orders (UserId, RecipientName, RecipientPhone, ShippingAddress, ShippingMethod, PaymentMethod, Status, Subtotal, ShippingCost, Total, CreatedAt)
    VALUES (@UserId, N'سارا احمدی', N'09121111111', @Addr, N'standard', N'cod', N'Processing', 0, 45000, 0, DATEADD(HOUR, -5, SYSUTCDATETIME()));
    SET @OrderId = SCOPE_IDENTITY();
    INSERT INTO dbo.OrderItems (OrderId, VariantId, ProductId, ProductName, VariantDescription, Sku, UnitPrice, Quantity)
    SELECT @OrderId, v.Id, p.Id, p.Name, N'سایز 40 / رنگ کرم', v.Sku, v.Price, 1 FROM dbo.ProductVariants v JOIN dbo.Products p ON p.Id = v.ProductId WHERE v.Sku = N'TUN-40-CRM';
    INSERT INTO dbo.OrderItems (OrderId, VariantId, ProductId, ProductName, VariantDescription, Sku, UnitPrice, Quantity)
    SELECT @OrderId, v.Id, p.Id, p.Name, N'سایز 38 / رنگ آبی', v.Sku, v.Price, 1 FROM dbo.ProductVariants v JOIN dbo.Products p ON p.Id = v.ProductId WHERE v.Sku = N'JNS-38-BLU';
    UPDATE dbo.Orders SET Subtotal = (SELECT SUM(UnitPrice * Quantity) FROM dbo.OrderItems WHERE OrderId = @OrderId),
                          Total = ShippingCost + (SELECT SUM(UnitPrice * Quantity) FROM dbo.OrderItems WHERE OrderId = @OrderId)
    WHERE Id = @OrderId;

    -- سفارش ۴: لغو شده
    INSERT INTO dbo.Orders (UserId, RecipientName, RecipientPhone, ShippingAddress, ShippingMethod, PaymentMethod, Status, Subtotal, ShippingCost, Total, CreatedAt)
    VALUES (@UserId, N'سارا احمدی', N'09121111111', @Addr, N'pickup', N'cod', N'Cancelled', 0, 0, 0, DATEADD(DAY, -20, SYSUTCDATETIME()));
    SET @OrderId = SCOPE_IDENTITY();
    INSERT INTO dbo.OrderItems (OrderId, VariantId, ProductId, ProductName, VariantDescription, Sku, UnitPrice, Quantity)
    SELECT @OrderId, v.Id, p.Id, p.Name, N'سایز یک‌سایز / رنگ مشکی', v.Sku, v.Price, 1 FROM dbo.ProductVariants v JOIN dbo.Products p ON p.Id = v.ProductId WHERE v.Sku = N'SPT-OS-BLK';
    UPDATE dbo.Orders SET Subtotal = (SELECT SUM(UnitPrice * Quantity) FROM dbo.OrderItems WHERE OrderId = @OrderId),
                          Total = ShippingCost + (SELECT SUM(UnitPrice * Quantity) FROM dbo.OrderItems WHERE OrderId = @OrderId)
    WHERE Id = @OrderId;
END
GO

PRINT N'کاربران و سفارش‌های نمونه وارد شد.';
GO
