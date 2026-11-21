/* ==========================================================================
   جدول‌های سرویس Auth  —  فقط دو جدول: Users و RefreshTokens
   ==========================================================================
   - اگه از قبل اسکریپت‌های 01/02 فروشگاه رو روی GirlyShop اجرا کردی،
     این جدول‌ها وجود دارند و این اسکریپت چیزی را خراب نمی‌کند
     (Idempotent است: هر چندبار اجرا شود مشکلی ندارد).
   - اگه می‌خواهی سرویس Auth دیتابیس جدا داشته باشد، فقط اسم دیتابیس را
     پایین عوض کن (مثلاً GirlyShopAuthDb) و Connection String را هم عوض کن.
   ========================================================================== */

IF DB_ID(N'GirlyShop') IS NULL
    CREATE DATABASE GirlyShop;
GO

USE GirlyShop;
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Users' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Users
    (
        Id              INT IDENTITY(1,1)  NOT NULL,
        FullName        NVARCHAR(150)       NOT NULL,
        PhoneNumber     NVARCHAR(20)        NOT NULL,
        Email           NVARCHAR(150)       NULL,
        PasswordHash    NVARCHAR(300)       NOT NULL,   -- فقط هش BCrypt، نه رمز خام
        IsAdmin         BIT                 NOT NULL DEFAULT 0,
        IsActive        BIT                 NOT NULL DEFAULT 1,
        CreatedAt       DATETIME2           NOT NULL DEFAULT SYSUTCDATETIME(),

        CONSTRAINT PK_Users PRIMARY KEY CLUSTERED (Id)
    );
    PRINT N'جدول Users ساخته شد.';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_Users_PhoneNumber' AND object_id = OBJECT_ID('dbo.Users'))
    CREATE UNIQUE INDEX UX_Users_PhoneNumber ON dbo.Users(PhoneNumber);
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'RefreshTokens' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.RefreshTokens
    (
        Id              INT IDENTITY(1,1)  NOT NULL,
        UserId          INT                NOT NULL,
        Token           NVARCHAR(500)      NOT NULL,   -- هش SHA256 توکن، نه خود توکن
        ExpiresAt       DATETIME2          NOT NULL,
        CreatedAt       DATETIME2          NOT NULL DEFAULT SYSUTCDATETIME(),
        RevokedAt       DATETIME2          NULL,
        CreatedByIp     NVARCHAR(50)       NULL,

        CONSTRAINT PK_RefreshTokens PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_RefreshTokens_User
            FOREIGN KEY (UserId) REFERENCES dbo.Users(Id)
            ON DELETE CASCADE
    );
    PRINT N'جدول RefreshTokens ساخته شد.';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_RefreshTokens_Token' AND object_id = OBJECT_ID('dbo.RefreshTokens'))
    CREATE UNIQUE INDEX UX_RefreshTokens_Token ON dbo.RefreshTokens(Token);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_RefreshTokens_UserId' AND object_id = OBJECT_ID('dbo.RefreshTokens'))
    CREATE INDEX IX_RefreshTokens_UserId ON dbo.RefreshTokens(UserId);
GO

PRINT N'اسکریپت Auth با موفقیت اجرا شد.';

/* ==========================================================================
   ساخت اولین ادمین (بعد از ثبت‌نام عادی، یک‌بار این را با شماره خودت اجرا کن)
   --------------------------------------------------------------------------
   UPDATE dbo.Users SET IsAdmin = 1 WHERE PhoneNumber = '09123456789';
   ========================================================================== */
