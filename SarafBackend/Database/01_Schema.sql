/* ==========================================================================
   GirlyShop — ساخت دیتابیس و همه‌ی جدول‌ها (فقط ساختار، بدون داده)
   ==========================================================================
   - دیتابیس جدیدی به اسم  GirlyShop  می‌سازد (دیتابیس‌های قدیمی‌ات مثل
     GirlyShopDb و online_shop_db دست نمی‌خورند).
   - هر چندبار اجرا کنی مشکلی پیش نمی‌آید (اگر جدولی هست دوباره نمی‌سازد).
   - ترتیب اجرا:  01_Schema.sql  ←  02_SeedCatalog.sql  ←  (اختیاری) 03_DevUsersAndOrders.sql
   ========================================================================== */

IF DB_ID(N'GirlyShop') IS NULL
    CREATE DATABASE GirlyShop;
GO

USE GirlyShop;
GO

/* ---------------------------------------------------------------- Users -- */
IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        Id              INT IDENTITY(1,1)  NOT NULL,
        FullName        NVARCHAR(150)      NOT NULL,
        PhoneNumber     NVARCHAR(20)       NOT NULL,
        Email           NVARCHAR(150)      NULL,
        PasswordHash    NVARCHAR(300)      NOT NULL,          -- فقط هش BCrypt
        IsAdmin         BIT                NOT NULL CONSTRAINT DF_Users_IsAdmin DEFAULT 0,
        IsActive        BIT                NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT 1,
        CreatedAt       DATETIME2          NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT PK_Users PRIMARY KEY CLUSTERED (Id)
    );
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Users_PhoneNumber' AND object_id = OBJECT_ID(N'dbo.Users'))
    CREATE UNIQUE INDEX UX_Users_PhoneNumber ON dbo.Users(PhoneNumber);
GO

IF OBJECT_ID(N'dbo.RefreshTokens', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.RefreshTokens
    (
        Id              INT IDENTITY(1,1)  NOT NULL,
        UserId          INT                NOT NULL,
        Token           NVARCHAR(500)      NOT NULL,          -- هش SHA256 توکن
        ExpiresAt       DATETIME2          NOT NULL,
        CreatedAt       DATETIME2          NOT NULL CONSTRAINT DF_RefreshTokens_CreatedAt DEFAULT SYSUTCDATETIME(),
        RevokedAt       DATETIME2          NULL,
        CreatedByIp     NVARCHAR(50)       NULL,
        CONSTRAINT PK_RefreshTokens PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_RefreshTokens_User FOREIGN KEY (UserId) REFERENCES dbo.Users(Id) ON DELETE CASCADE
    );
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_RefreshTokens_Token' AND object_id = OBJECT_ID(N'dbo.RefreshTokens'))
    CREATE UNIQUE INDEX UX_RefreshTokens_Token ON dbo.RefreshTokens(Token);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_RefreshTokens_UserId' AND object_id = OBJECT_ID(N'dbo.RefreshTokens'))
    CREATE INDEX IX_RefreshTokens_UserId ON dbo.RefreshTokens(UserId);
GO

/* ----------------------------------------------------- Categories/Brands -- */
IF OBJECT_ID(N'dbo.Categories', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Categories
    (
        Id          BIGINT IDENTITY(1,1) NOT NULL,
        ParentId    BIGINT               NULL,
        Name        NVARCHAR(150)        NOT NULL,
        Slug        NVARCHAR(170)        NOT NULL,
        IsActive    BIT                  NOT NULL CONSTRAINT DF_Categories_IsActive DEFAULT 1,
        CONSTRAINT PK_Categories PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_Categories_Parent FOREIGN KEY (ParentId) REFERENCES dbo.Categories(Id)
    );
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Categories_Slug' AND object_id = OBJECT_ID(N'dbo.Categories'))
    CREATE UNIQUE INDEX UX_Categories_Slug ON dbo.Categories(Slug);
GO

IF OBJECT_ID(N'dbo.Brands', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Brands
    (
        Id          BIGINT IDENTITY(1,1) NOT NULL,
        Name        NVARCHAR(150)        NOT NULL,
        Slug        NVARCHAR(170)        NOT NULL,
        IsActive    BIT                  NOT NULL CONSTRAINT DF_Brands_IsActive DEFAULT 1,
        CONSTRAINT PK_Brands PRIMARY KEY CLUSTERED (Id)
    );
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Brands_Slug' AND object_id = OBJECT_ID(N'dbo.Brands'))
    CREATE UNIQUE INDEX UX_Brands_Slug ON dbo.Brands(Slug);
GO

/* ------------------------------------------------------------- Products -- */
IF OBJECT_ID(N'dbo.Products', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Products
    (
        Id           UNIQUEIDENTIFIER NOT NULL,
        CategoryId   BIGINT           NOT NULL,
        BrandId      BIGINT           NULL,
        Name         NVARCHAR(255)    NOT NULL,
        Slug         NVARCHAR(280)    NOT NULL,
        Description  NVARCHAR(4000)   NULL,
        BasePrice    DECIMAL(14,2)    NOT NULL,
        Status       NVARCHAR(20)     NOT NULL CONSTRAINT DF_Products_Status DEFAULT N'Draft',  -- Draft | Published | Archived
        IsFeatured   BIT              NOT NULL CONSTRAINT DF_Products_IsFeatured DEFAULT 0,
        CreatedAt    DATETIME2        NOT NULL CONSTRAINT DF_Products_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt    DATETIME2        NULL,
        CONSTRAINT PK_Products PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_Products_Category FOREIGN KEY (CategoryId) REFERENCES dbo.Categories(Id),
        CONSTRAINT FK_Products_Brand    FOREIGN KEY (BrandId)    REFERENCES dbo.Brands(Id),
        CONSTRAINT CK_Products_Status CHECK (Status IN (N'Draft', N'Published', N'Archived'))
    );
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Products_Slug' AND object_id = OBJECT_ID(N'dbo.Products'))
    CREATE UNIQUE INDEX UX_Products_Slug ON dbo.Products(Slug);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Products_CategoryId' AND object_id = OBJECT_ID(N'dbo.Products'))
    CREATE INDEX IX_Products_CategoryId ON dbo.Products(CategoryId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Products_Status' AND object_id = OBJECT_ID(N'dbo.Products'))
    CREATE INDEX IX_Products_Status ON dbo.Products(Status);
GO

IF OBJECT_ID(N'dbo.ProductImages', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ProductImages
    (
        Id         UNIQUEIDENTIFIER NOT NULL,
        ProductId  UNIQUEIDENTIFIER NOT NULL,
        ImageUrl   NVARCHAR(500)    NOT NULL,
        SortOrder  INT              NOT NULL CONSTRAINT DF_ProductImages_Sort DEFAULT 0,
        IsPrimary  BIT              NOT NULL CONSTRAINT DF_ProductImages_Primary DEFAULT 0,
        CONSTRAINT PK_ProductImages PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_ProductImages_Product FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id) ON DELETE CASCADE
    );
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ProductImages_ProductId' AND object_id = OBJECT_ID(N'dbo.ProductImages'))
    CREATE INDEX IX_ProductImages_ProductId ON dbo.ProductImages(ProductId);
GO

IF OBJECT_ID(N'dbo.ProductVariants', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ProductVariants
    (
        Id              UNIQUEIDENTIFIER NOT NULL,
        ProductId       UNIQUEIDENTIFIER NOT NULL,
        Sku             NVARCHAR(100)    NOT NULL,
        Price           DECIMAL(14,2)    NOT NULL,
        CompareAtPrice  DECIMAL(14,2)    NULL,             -- قیمت قبل از تخفیف
        StockQuantity   INT              NOT NULL CONSTRAINT DF_ProductVariants_Stock DEFAULT 0,
        IsActive        BIT              NOT NULL CONSTRAINT DF_ProductVariants_IsActive DEFAULT 1,
        CONSTRAINT PK_ProductVariants PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_ProductVariants_Product FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id) ON DELETE CASCADE,
        CONSTRAINT CK_ProductVariants_Stock CHECK (StockQuantity >= 0)
    );
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_ProductVariants_Sku' AND object_id = OBJECT_ID(N'dbo.ProductVariants'))
    CREATE UNIQUE INDEX UX_ProductVariants_Sku ON dbo.ProductVariants(Sku);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_ProductVariants_ProductId' AND object_id = OBJECT_ID(N'dbo.ProductVariants'))
    CREATE INDEX IX_ProductVariants_ProductId ON dbo.ProductVariants(ProductId);
GO

IF OBJECT_ID(N'dbo.ProductVariantAttributes', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ProductVariantAttributes
    (
        VariantId       UNIQUEIDENTIFIER NOT NULL,
        AttributeName   NVARCHAR(100)    NOT NULL,         -- مثلاً «سایز» یا «رنگ»
        AttributeValue  NVARCHAR(150)    NOT NULL,         -- مثلاً «38» یا «مشکی»
        CONSTRAINT PK_ProductVariantAttributes PRIMARY KEY CLUSTERED (VariantId, AttributeName),
        CONSTRAINT FK_ProductVariantAttributes_Variant FOREIGN KEY (VariantId) REFERENCES dbo.ProductVariants(Id) ON DELETE CASCADE
    );
END
GO

/* ------------------------------------------------------------ Addresses -- */
IF OBJECT_ID(N'dbo.Addresses', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Addresses
    (
        Id             INT IDENTITY(1,1) NOT NULL,
        UserId         INT               NOT NULL,
        Title          NVARCHAR(150)     NOT NULL,         -- مثلاً «منزل — تهران، ونک»
        RecipientName  NVARCHAR(150)     NULL,
        PhoneNumber    NVARCHAR(20)      NULL,
        City           NVARCHAR(100)     NULL,
        PostalCode     NVARCHAR(20)      NULL,
        Detail         NVARCHAR(500)     NOT NULL,         -- آدرس کامل
        IsDefault      BIT               NOT NULL CONSTRAINT DF_Addresses_IsDefault DEFAULT 0,
        CreatedAt      DATETIME2         NOT NULL CONSTRAINT DF_Addresses_CreatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT PK_Addresses PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_Addresses_User FOREIGN KEY (UserId) REFERENCES dbo.Users(Id) ON DELETE CASCADE
    );
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Addresses_UserId' AND object_id = OBJECT_ID(N'dbo.Addresses'))
    CREATE INDEX IX_Addresses_UserId ON dbo.Addresses(UserId);
GO

/* ------------------------------------------------------ Cart / Wishlist -- */
IF OBJECT_ID(N'dbo.CartItems', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.CartItems
    (
        Id         INT IDENTITY(1,1) NOT NULL,
        UserId     INT               NOT NULL,
        VariantId  UNIQUEIDENTIFIER  NOT NULL,
        Quantity   INT               NOT NULL,
        CreatedAt  DATETIME2         NOT NULL CONSTRAINT DF_CartItems_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt  DATETIME2         NULL,
        CONSTRAINT PK_CartItems PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_CartItems_User    FOREIGN KEY (UserId)    REFERENCES dbo.Users(Id) ON DELETE CASCADE,
        CONSTRAINT FK_CartItems_Variant FOREIGN KEY (VariantId) REFERENCES dbo.ProductVariants(Id) ON DELETE CASCADE,
        CONSTRAINT CK_CartItems_Quantity CHECK (Quantity > 0)
    );
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_CartItems_User_Variant' AND object_id = OBJECT_ID(N'dbo.CartItems'))
    CREATE UNIQUE INDEX UX_CartItems_User_Variant ON dbo.CartItems(UserId, VariantId);
GO

IF OBJECT_ID(N'dbo.WishlistItems', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.WishlistItems
    (
        Id         INT IDENTITY(1,1) NOT NULL,
        UserId     INT               NOT NULL,
        ProductId  UNIQUEIDENTIFIER  NOT NULL,
        CreatedAt  DATETIME2         NOT NULL CONSTRAINT DF_WishlistItems_CreatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT PK_WishlistItems PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_WishlistItems_User    FOREIGN KEY (UserId)    REFERENCES dbo.Users(Id) ON DELETE CASCADE,
        CONSTRAINT FK_WishlistItems_Product FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id) ON DELETE CASCADE
    );
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_WishlistItems_User_Product' AND object_id = OBJECT_ID(N'dbo.WishlistItems'))
    CREATE UNIQUE INDEX UX_WishlistItems_User_Product ON dbo.WishlistItems(UserId, ProductId);
GO

/* -------------------------------------------------------------- Orders --- */
-- نکته: سفارش‌ها «عکس لحظه‌ی خرید» را نگه می‌دارند (نام، قیمت، آدرس). به همین خاطر
-- OrderItems عمداً FK به ProductVariants ندارد؛ ویرایش/حذف محصول، سفارش‌های قدیمی را خراب نمی‌کند.
IF OBJECT_ID(N'dbo.Orders', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Orders
    (
        Id               INT IDENTITY(1,1) NOT NULL,
        OrderNumber      AS (N'AV-' + CAST(100000 + Id AS NVARCHAR(12))) PERSISTED,   -- AV-100001 ...
        UserId           INT               NOT NULL,
        RecipientName    NVARCHAR(150)     NOT NULL,
        RecipientPhone   NVARCHAR(20)      NOT NULL,
        ShippingAddress  NVARCHAR(700)     NOT NULL,
        ShippingMethod   NVARCHAR(30)      NOT NULL,       -- standard | express | pickup
        PaymentMethod    NVARCHAR(30)      NOT NULL,       -- online | cod
        Status           NVARCHAR(20)      NOT NULL CONSTRAINT DF_Orders_Status DEFAULT N'Processing',
        Subtotal         DECIMAL(14,2)     NOT NULL,
        ShippingCost     DECIMAL(14,2)     NOT NULL,
        Total            DECIMAL(14,2)     NOT NULL,
        CreatedAt        DATETIME2         NOT NULL CONSTRAINT DF_Orders_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt        DATETIME2         NULL,
        CONSTRAINT PK_Orders PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_Orders_User FOREIGN KEY (UserId) REFERENCES dbo.Users(Id),
        CONSTRAINT CK_Orders_Status CHECK (Status IN (N'Processing', N'Shipped', N'Delivered', N'Cancelled'))
    );
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Orders_OrderNumber' AND object_id = OBJECT_ID(N'dbo.Orders'))
    CREATE UNIQUE INDEX UX_Orders_OrderNumber ON dbo.Orders(OrderNumber);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Orders_UserId' AND object_id = OBJECT_ID(N'dbo.Orders'))
    CREATE INDEX IX_Orders_UserId ON dbo.Orders(UserId, CreatedAt DESC);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Orders_Status' AND object_id = OBJECT_ID(N'dbo.Orders'))
    CREATE INDEX IX_Orders_Status ON dbo.Orders(Status, CreatedAt DESC);
GO

IF OBJECT_ID(N'dbo.OrderItems', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.OrderItems
    (
        Id                  INT IDENTITY(1,1) NOT NULL,
        OrderId             INT               NOT NULL,
        VariantId           UNIQUEIDENTIFIER  NULL,        -- بدون FK (عمداً)
        ProductId           UNIQUEIDENTIFIER  NULL,        -- بدون FK (عمداً)
        ProductName         NVARCHAR(255)     NOT NULL,
        VariantDescription  NVARCHAR(300)     NULL,        -- مثلاً «سایز 38 / رنگ مشکی»
        Sku                 NVARCHAR(100)     NULL,
        UnitPrice           DECIMAL(14,2)     NOT NULL,
        Quantity            INT               NOT NULL,
        ImageUrl            NVARCHAR(500)     NULL,
        CONSTRAINT PK_OrderItems PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_OrderItems_Order FOREIGN KEY (OrderId) REFERENCES dbo.Orders(Id) ON DELETE CASCADE,
        CONSTRAINT CK_OrderItems_Quantity CHECK (Quantity > 0)
    );
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_OrderItems_OrderId' AND object_id = OBJECT_ID(N'dbo.OrderItems'))
    CREATE INDEX IX_OrderItems_OrderId ON dbo.OrderItems(OrderId);
GO

PRINT N'GirlyShop: همه‌ی جدول‌ها آماده‌اند.';
GO
