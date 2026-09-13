# ماژول Product — معماری CQRS (روی همون اسکیمای دیتابیس فروشگاه)

دقیقاً همون الگوی HomeMicroservice، فقط برای دامنه فروشگاه آنلاین (Product/Category/Brand/Variant).

## چیدمان لایه‌ها

```
Application/
  Commands/Product/
    SetProductRequestCommand.cs → ثبت/ویرایش محصول (همراه واریانت‌ها و تصاویر)
    DeleteProductCommand.cs     → حذف محصول
  Query/Product/
    GetComprehensiveProductQuery.cs → لیست جامع با فیلتر (قیمت، برند، جستجو)
    GetProductDetailById.cs          → جزئیات یک محصول + واریانت‌ها
    GetProductListByCategoryQuery.cs → لیست محصولات یک دسته‌بندی
    GetFeaturedProductsQuery.cs      → محصولات ویژه/شاخص
  DTO/ProductDtos.cs      → ProductDetailDto, ProductListItemDto, ProductVariantDto, ProductFilterDto
  Mapping/ProductMappingProfile.cs → AutoMapper

Core/
  Domain/Product.cs → Product, ProductImage, ProductVariant, ProductVariantAttribute, Category, Brand
  Interfaces/IProductRepositoryRead.cs / IProductRepositoryWrite.cs
  DomainServices/Product/
    ProductRepositoryRead.cs  → Dapper (فقط خواندن، مستقیم روی جداول Products/ProductVariants/...)
    ProductRepositoryWrite.cs → EF Core (فقط نوشتن)

Infrastructure/
  Persistence/EntityFramework/ShopDbContext.cs + ProductEntityConfiguration.cs
  Persistence/Dappers/DapperContext.cs
  DependencyInjection/ProductServicesInjection.cs

Controllers/ProductController.cs → فقط MediatR.Send صدا می‌زند
```

## اتصال به Program.cs

```csharp
builder.Services.AddProductServices(builder.Configuration);
```

## appsettings.json

```json
{
  "ConnectionStrings": {
    "ShopWriteConnection": "Server=.;Database=online_shop_db;Trusted_Connection=True;TrustServerCertificate=True;",
    "ShopReadConnection":  "Server=.;Database=online_shop_db;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

توجه: این ماژول روی همون دیتابیس `online_shop_db` (فایل `online_shop_schema_sqlserver.sql`) کار می‌کنه.
جدول‌های `Products`, `ProductImages`, `ProductVariants`, `ProductVariantAttributes`,
`Categories`, `Brands` رو مستقیم می‌خونه/می‌نویسه. بقیه جدول‌های اون اسکیما (سفارش، سبد خرید،
کاربران و ...) رو می‌تونی به همین سبک، ماژول به ماژول (مثلاً `OrderMicroservice`,
`CartMicroservice`) اضافه کنی — همین یک الگو رو کپی و اسم عوض کن.

## پکیج‌های NuGet لازم

```
MediatR
AutoMapper
Dapper
Microsoft.Data.SqlClient
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.EntityFrameworkCore.Design
```
