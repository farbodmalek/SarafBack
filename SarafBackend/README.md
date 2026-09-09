# معماری CQRS — راهنما

## این معماری چیه؟

**CQRS** یعنی جدا کردن مسیر «خوندن داده» (Query) از «تغییر دادن داده» (Command).
هر Entity دو تا Repository داره:

- `I{Entity}ReadRepository` → فقط GetAll/GetById/... با `AsNoTracking()` (سریع‌تر، چون EF Core لازم نیست ردش رو نگه داره)
- `I{Entity}WriteRepository` → فقط Add/Update/Remove/SaveChanges

و به‌جای اینکه Controller مستقیم Service رو صدا بزنه، یه **Command** یا **Query** می‌سازه و از طریق **MediatR** می‌فرسته. یه Handler جداگانه اون Command/Query رو می‌گیره و اجرا می‌کنه.

## جریان یک درخواست (مثلاً ساخت محصول)

```
Controller  →  CreateProductCommand  →  MediatR  →  CreateProductCommandHandler  →  IProductWriteRepository  →  دیتابیس
```

## ساختار پوشه‌ها

```
Application/                              ← لایه CQRS (Commands و Queries)
├── Categories/
│   ├── Commands/
│   │   ├── CreateCategory/
│   │   │   ├── CreateCategoryCommand.cs        (فقط داده ورودی)
│   │   │   └── CreateCategoryCommandHandler.cs (منطق اجرا)
│   │   └── DeleteCategory/
│   └── Queries/
│       └── GetAllCategories/
└── Products/
    ├── Commands/ (Create, Update, Delete)
    └── Queries/  (GetAll, GetBySlug)

Core/
├── Domain/
│   ├── Entities/                         ← ۱۳ Entity، هماهنگ با اسکریپت SQL نهایی
│   ├── RepositoryInterfaces/
│   │   ├── ICategoryReadRepository.cs
│   │   ├── ICategoryWriteRepository.cs
│   │   ├── IProductReadRepository.cs
│   │   └── IProductWriteRepository.cs
│   └── ViewModel/                        ← DTO های خروجی Query ها
└── DomainServices/
    └── SlugGenerator.cs                  ← منطق مشترک (نه مخصوص یک Command)

Infrastructure/
├── DependencyInjection/
│   └── GirlyShopBackendServicesInjection.cs   ← ثبت MediatR + Repository ها
└── Persistence/
    ├── Dappers/
    └── EntityFramework/
        ├── ApplicationDbContext.cs
        ├── Configurations/               ← یک فایل به‌ازای هر Entity
        └── Repositories/
            ├── CategoryReadRepository.cs
            ├── CategoryWriteRepository.cs
            ├── ProductReadRepository.cs
            └── ProductWriteRepository.cs

Controllers/
├── CategoriesController.cs               ← فقط IMediator.Send می‌کنه
└── ProductsController.cs
```

## چی کامل ساخته شده

**Category** و **Product** با تمام Command/Query لازم (Create, Update در Product، Delete، GetAll، GetBySlug) — این‌ها الگوی کامل رو نشون می‌دن.

## چی رو خودت باید اضافه کنی (دقیقاً همین الگو رو تکرار کن)

برای **Order، Address، Wishlist، Coupon، ProductReview**:

1. تو `Core/Domain/RepositoryInterfaces/` بساز: `IOrderReadRepository.cs` و `IOrderWriteRepository.cs`
2. تو `Infrastructure/Persistence/EntityFramework/Repositories/` پیاده‌سازیشون کن (دقیقاً مثل CategoryRead/WriteRepository)
3. تو `Application/Orders/Commands/` و `Application/Orders/Queries/` پوشه بساز و Command/Query های لازم رو اضافه کن (مثلاً `CreateOrderCommand`, `GetOrdersByUserQuery`)
4. تو `GirlyShopBackendServicesInjection.cs` دو خط اضافه کن:
   ```csharp
   services.AddScoped<IOrderReadRepository, OrderReadRepository>();
   services.AddScoped<IOrderWriteRepository, OrderWriteRepository>();
   ```
5. یه `OrdersController.cs` بساز که `IMediator` رو inject کنه

## نصب پکیج‌های لازم

```
dotnet add package MediatR
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.Data.SqlClient
```

## نکته مهم درباره namespace

فرض کردم اسم پروژه‌ت `GirlyShopBackend` هست. اگه فرق داره، با Find & Replace (Ctrl+Shift+H تو Visual Studio) عوضش کن.

## نکته درباره احراز هویت (Authentication)

چون به‌جای ASP.NET Identity از جدول `Users` سفارشی استفاده می‌کنیم، باید خودت:
- یه `AuthController` با اکشن `Login`/`Register` بسازی
- رمز عبور رو با `BCrypt.Net-Next` یا مشابه هش کنی (هیچ‌وقت متن خام ذخیره نشه)
- بعد از لاگین موفق، یه JWT بسازی و به فرانت برگردونی
- برای این بخش هم می‌تونی همین الگوی CQRS رو ادامه بدی: `LoginCommand`, `RegisterCommand`

اگه خواستی همین بخش (Auth با JWT) رو هم برات کامل بسازم، بگو.

## ساخت جدول‌ها

دیتابیس از قبل با اسکریپت‌های SQL (`01_CreateTables.sql` و `02_AdditionalTables.sql`) ساخته شده. این‌بار به‌جای Migration، مستقیم از همون جدول‌ها استفاده می‌کنیم — پس EF Core فقط باید بهشون وصل بشه، نیازی به `dotnet ef migrations add` نیست (چون جدول‌ها از قبل هستن).

اگه اسم ستون یا جدولی با چیزی که EF Core انتظار داره فرق داشت، خطای اجرا می‌گیری — در اون صورت بگو تا با هم چک کنیم.
