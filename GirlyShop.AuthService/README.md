# GirlyShop.AuthService — سرویس مستقل ثبت‌نام و ورود

## این چیه؟ (خیلی ساده)

یه پروژه‌ی **جدا** که فقط یک کار می‌کند: **ثبت‌نام و ورود کاربرها**.
بقیه‌ی فروشگاه (محصول، سفارش، ...) توی پروژه‌ی دیگه‌ای می‌ماند و اصلاً کاری با رمز عبور ندارد.

سه تا چیز ساده که باید بدونی:

| اسم | تشبیه | توضیح |
|---|---|---|
| **BCrypt (هش رمز)** | آسیاب یک‌طرفه | رمز «123456» تبدیل می‌شود به یک رشته‌ی نامفهوم مثل `$2a$12$...` و برگشتنی نیست. فقط همین رشته داخل دیتابیس می‌رود. |
| **Access Token** | کارت ورود ۱۵ دقیقه‌ای | فرانت با هر درخواست نشانش می‌دهد تا بگوید «من لاگین هستم». |
| **Refresh Token** | کلید رزرو ۷ روزه | وقتی کارت ۱۵ دقیقه‌ای تمام شد، با این کلید کارت تازه می‌گیرد (بدون وارد کردن دوباره‌ی رمز). |

---

## چطور اجرا کنم؟

**۱.** فایل `SarafBackend.slnx` (در ریشه‌ی ریپو) را با Visual Studio باز کن؛ پروژه‌ی Auth همراه بقیه داخلش است و به `CommonLibrary` وصل است.

**۲.** دیتابیس: دیتابیس حالا `GirlyShop` است و جدول‌های `Users` و `RefreshTokens` را اسکریپت `SarafBackend/Database/01_Schema.sql` می‌سازد (فایل `Database/01_AuthTables.sql` همین Auth هم همان دو جدول را می‌سازد و بی‌خطر است).
اگه نه، فایل `Database/01_AuthTables.sql` را در SSMS اجرا کن.

**۳.** در `appsettings.json`، مقدار `Server=` در `ConnectionStrings` را با اسم SQL Server خودت (همانی که در SSMS وصل می‌شوی) عوض کن.

**۴.** پروژه را اجرا کن (F5 یا `dotnet run`). سرویس روی این آدرس بالا می‌آید:
- `https://localhost:7100`
- `http://localhost:5100`

**۵.** فایل `AuthService.http` را باز کن و دکمه‌ی «Send Request» بالای هر درخواست را بزن (اول ثبت‌نام، بعد ورود).

**۶.** برو SSMS و جدول `Users` را ببین: ستون `PasswordHash` باید یک رشته‌ی طولانی شبیه `$2a$12$...` باشد، **نه** رمز اصلی.

> ⚠️ چون جدول‌ها از قبل ساخته شده‌اند، **Migration نزن** (`dotnet ef migrations` لازم نیست).

---

## Endpoint ها

| روش | آدرس | کار |
|---|---|---|
| POST | `/api/auth/register` | ثبت‌نام (رمز هش می‌شود و ذخیره می‌شود) |
| POST | `/api/auth/login` | ورود |
| POST | `/api/auth/refresh` | گرفتن کارت ورود تازه با کلید رزرو |
| POST | `/api/auth/logout` | باطل کردن کلید رزرو |
| GET | `/api/auth/me` | اطلاعات کاربر لاگین‌شده (نیاز به توکن) |
| GET | `/health` | سالم بودن سرویس |

**خروجی ثبت‌نام/ورود** (دقیقاً با چیزی که فرانت‌اند انتظار دارد هماهنگ است):
```json
{
  "user":   { "id": 1, "fullName": "سارا احمدی", "phone": "09123456789", "email": null, "isAdmin": false },
  "tokens": { "accessToken": "...", "accessTokenExpiresAt": "...", "refreshToken": "...", "refreshTokenExpiresAt": "..." }
}
```

---

## ساخت اولین ادمین

همه با نقش «مشتری» ثبت‌نام می‌کنند. برای ادمین کردن خودت، بعد از ثبت‌نام در SSMS این را بزن:
```sql
UPDATE dbo.Users SET IsAdmin = 1 WHERE PhoneNumber = '09123456789';
```
(بعدش یک‌بار دوباره لاگین کن تا توکن جدید با نقش ادمین بگیری.)

---

## پروژه‌ی اصلی فروشگاه چطور بفهمد کاربر لاگین است؟

پروژه‌ی اصلی **رمز را نمی‌بیند**؛ فقط «کارت ورود» (JWT) را چک می‌کند.
برای این کار، در پروژه‌ی اصلی بسته‌ی `Microsoft.AspNetCore.Authentication.JwtBearer` را نصب کن و این را در Program.cs بگذار:

```csharp
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,   ValidIssuer   = "GirlyShop.AuthService",
            ValidateAudience = true, ValidAudience = "GirlyShop.Clients",
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            // ⚠️ دقیقاً همان Jwt:Key سرویس Auth
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            NameClaimType = "name",
            RoleClaimType = "role"
        };
    });
builder.Services.AddAuthorization();

// ... و در pipeline:
app.UseAuthentication();
app.UseAuthorization();
```
بعد روی هر Controller یا اکشن مثل پنل ادمین بنویس: `[Authorize(Roles = "Admin")]`.

**نکته‌ی مهم:** `Key` و `Issuer` و `Audience` باید در هر دو پروژه **یکسان** باشند، وگرنه توکن قبول نمی‌شود.

---

## وصل کردن فرانت‌اند

سرویس Auth روی پورت جدا (`7100`) است. در `.env.local` فرانت یک خط اضافه کن:
```
NEXT_PUBLIC_AUTH_API_URL=https://localhost:7100
```
و آدرس‌های login/register/refresh را به این آدرس بفرست (اگر خواستی همین را هم برایت پیاده‌سازی می‌کنم).
CORS برای `http://localhost:3000` از قبل باز است (در `appsettings.json` → `Cors:AllowedOrigins`).

---

## روی سرور واقعی (Production)

- `Jwt:Key` را **هرگز** داخل فایل نگذار؛ با متغیر محیطی بده: `Jwt__Key=یک-رشته-تصادفی-طولانی`
- آدرس سایت واقعی را به `Cors:AllowedOrigins` اضافه کن.
- فایل `appsettings.Development.json` فقط برای توسعه است و کلید آن عمومی/ضعیف است.

---

## ساختار پوشه‌ها (همان الگوی پروژه‌ی خودت)

```
Controllers/AuthController.cs
Application/
  Commands/Auth/   Register, Login, RefreshToken, Logout   (هرکدام Command + Handler)
  Query/Auth/      GetCurrentUser
  DTO/AuthDtos.cs
Core/
  Domain/          User, RefreshToken
  Interfaces/      IUserRepositoryRead (Dapper) / IUserRepositoryWrite (EF) / ...
  DomainServices/  Auth (BCrypt، JWT) و Users (Repository ها)
  Exceptions/
Infrastructure/
  DependencyInjection/AuthServicesInjection.cs
  Persistence/EntityFramework, Dappers
  ExceptionHandling/GlobalExceptionHandler.cs
Database/01_AuthTables.sql
```

## کارهای امنیتی که انجام شده

- رمز با **BCrypt** هش می‌شود (هیچ‌وقت خام ذخیره نمی‌شود).
- Refresh Token هم در دیتابیس **هش** می‌شود (اگر کسی دیتابیس را بدزدد، به درد نمی‌خورد).
- Refresh Token **یک‌بار مصرف** است (هر بار که استفاده شود، باطل و یکی تازه داده می‌شود).
- پیام خطای ورود برای «شماره نیست» و «رمز اشتباه» یکی است.
- محدودیت **۱۰ تلاش در دقیقه برای هر IP** روی ثبت‌نام/ورود/رفرش.
- شماره موبایل یکتا (حتی اگر دو نفر هم‌زمان ثبت‌نام کنند).

## یادداشت

این پروژه جایگزین زیپ قبلی `girly-shop-auth` است؛ آن دیگر لازم نیست.
