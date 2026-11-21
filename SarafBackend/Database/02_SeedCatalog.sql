/* ==========================================================================
   داده‌ی نمونه‌ی فروشگاه: ۴ دسته، ۳ برند، ۸ محصول با واریانت (سایز/رنگ) و تصویر
   - بعد از 01_Schema.sql اجرا کن. هر چندبار اجرا شود، تکراری نمی‌سازد.
   - عکس‌ها نمونه‌اند (picsum.photos)؛ بعداً از پنل ادمین عوض کن.
   ========================================================================== */
USE GirlyShop;
GO
SET NOCOUNT ON;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE Slug = N'lebas-majlesi') INSERT INTO dbo.Categories (Name, Slug, IsActive) VALUES (N'لباس مجلسی', N'lebas-majlesi', 1);
IF NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE Slug = N'lebas-zir') INSERT INTO dbo.Categories (Name, Slug, IsActive) VALUES (N'لباس زیر', N'lebas-zir', 1);
IF NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE Slug = N'jorab') INSERT INTO dbo.Categories (Name, Slug, IsActive) VALUES (N'جوراب', N'jorab', 1);
IF NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE Slug = N'roozmareh') INSERT INTO dbo.Categories (Name, Slug, IsActive) VALUES (N'پوشاک روزمره', N'roozmareh', 1);
IF NOT EXISTS (SELECT 1 FROM dbo.Brands WHERE Slug = N'ava') INSERT INTO dbo.Brands (Name, Slug, IsActive) VALUES (N'آوا', N'ava', 1);
IF NOT EXISTS (SELECT 1 FROM dbo.Brands WHERE Slug = N'mahtab') INSERT INTO dbo.Brands (Name, Slug, IsActive) VALUES (N'مهتاب', N'mahtab', 1);
IF NOT EXISTS (SELECT 1 FROM dbo.Brands WHERE Slug = N'narges') INSERT INTO dbo.Brands (Name, Slug, IsActive) VALUES (N'نرگس', N'narges', 1);
GO

-- ===== پیراهن مجلسی حریر =====
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Slug = N'silk-evening-dress')
BEGIN
  INSERT INTO dbo.Products (Id, CategoryId, BrandId, Name, Slug, Description, BasePrice, Status, IsFeatured)
  VALUES ('3D440364-CEDF-5ACC-93C1-A681D3E8105F', (SELECT Id FROM dbo.Categories WHERE Slug = N'lebas-majlesi'), (SELECT Id FROM dbo.Brands WHERE Slug = N'ava'), N'پیراهن مجلسی حریر', N'silk-evening-dress', N'پیراهن مجلسی از پارچه حریر درجه‌یک، مناسب مهمانی و مراسم شب. دارای آستر داخلی و دوخت دست‌دوز روی یقه.', 1850000, N'Published', 1);
  INSERT INTO dbo.ProductImages (Id, ProductId, ImageUrl, SortOrder, IsPrimary) VALUES ('0A59F496-B88F-5039-9EBB-818A50A80256', '3D440364-CEDF-5ACC-93C1-A681D3E8105F', N'https://picsum.photos/seed/silk-evening-dress-1/600/800', 0, 1);
  INSERT INTO dbo.ProductImages (Id, ProductId, ImageUrl, SortOrder, IsPrimary) VALUES ('E147C4C0-BE97-5DC3-B2BB-F89A40853956', '3D440364-CEDF-5ACC-93C1-A681D3E8105F', N'https://picsum.photos/seed/silk-evening-dress-2/600/800', 1, 0);
  INSERT INTO dbo.ProductImages (Id, ProductId, ImageUrl, SortOrder, IsPrimary) VALUES ('2F808463-612A-54D7-97D6-A4704DB1040E', '3D440364-CEDF-5ACC-93C1-A681D3E8105F', N'https://picsum.photos/seed/silk-evening-dress-3/600/800', 2, 0);
  INSERT INTO dbo.ProductVariants (Id, ProductId, Sku, Price, CompareAtPrice, StockQuantity, IsActive) VALUES ('6695A313-F841-501C-8A07-91F07AC902AC', '3D440364-CEDF-5ACC-93C1-A681D3E8105F', N'DRS-36-BLK', 1490000, 1850000, 6, 1);
  INSERT INTO dbo.ProductVariantAttributes (VariantId, AttributeName, AttributeValue) VALUES ('6695A313-F841-501C-8A07-91F07AC902AC', N'سایز', N'36'), ('6695A313-F841-501C-8A07-91F07AC902AC', N'رنگ', N'مشکی');
  INSERT INTO dbo.ProductVariants (Id, ProductId, Sku, Price, CompareAtPrice, StockQuantity, IsActive) VALUES ('DC7EB458-92BF-57E4-A1FB-F50A229B58A8', '3D440364-CEDF-5ACC-93C1-A681D3E8105F', N'DRS-36-BRD', 1490000, 1850000, 6, 1);
  INSERT INTO dbo.ProductVariantAttributes (VariantId, AttributeName, AttributeValue) VALUES ('DC7EB458-92BF-57E4-A1FB-F50A229B58A8', N'سایز', N'36'), ('DC7EB458-92BF-57E4-A1FB-F50A229B58A8', N'رنگ', N'بردو');
  INSERT INTO dbo.ProductVariants (Id, ProductId, Sku, Price, CompareAtPrice, StockQuantity, IsActive) VALUES ('5F4F02A9-8C0F-57EC-B9B0-6ACC773E09F3', '3D440364-CEDF-5ACC-93C1-A681D3E8105F', N'DRS-38-BLK', 1490000, 1850000, 6, 1);
  INSERT INTO dbo.ProductVariantAttributes (VariantId, AttributeName, AttributeValue) VALUES ('5F4F02A9-8C0F-57EC-B9B0-6ACC773E09F3', N'سایز', N'38'), ('5F4F02A9-8C0F-57EC-B9B0-6ACC773E09F3', N'رنگ', N'مشکی');
  INSERT INTO dbo.ProductVariants (Id, ProductId, Sku, Price, CompareAtPrice, StockQuantity, IsActive) VALUES ('22B1BD16-E44F-5C4F-A780-17C404E2DC0D', '3D440364-CEDF-5ACC-93C1-A681D3E8105F', N'DRS-38-BRD', 1490000, 1850000, 6, 1);
  INSERT INTO dbo.ProductVariantAttributes (VariantId, AttributeName, AttributeValue) VALUES ('22B1BD16-E44F-5C4F-A780-17C404E2DC0D', N'سایز', N'38'), ('22B1BD16-E44F-5C4F-A780-17C404E2DC0D', N'رنگ', N'بردو');
  INSERT INTO dbo.ProductVariants (Id, ProductId, Sku, Price, CompareAtPrice, StockQuantity, IsActive) VALUES ('614B6897-3D2B-52D8-A23F-F8DB4EC7F53D', '3D440364-CEDF-5ACC-93C1-A681D3E8105F', N'DRS-40-BLK', 1490000, 1850000, 6, 1);
  INSERT INTO dbo.ProductVariantAttributes (VariantId, AttributeName, AttributeValue) VALUES ('614B6897-3D2B-52D8-A23F-F8DB4EC7F53D', N'سایز', N'40'), ('614B6897-3D2B-52D8-A23F-F8DB4EC7F53D', N'رنگ', N'مشکی');
  INSERT INTO dbo.ProductVariants (Id, ProductId, Sku, Price, CompareAtPrice, StockQuantity, IsActive) VALUES ('7942609D-603B-54A8-8C88-16E5E2FA648D', '3D440364-CEDF-5ACC-93C1-A681D3E8105F', N'DRS-40-BRD', 1490000, 1850000, 0, 1);
  INSERT INTO dbo.ProductVariantAttributes (VariantId, AttributeName, AttributeValue) VALUES ('7942609D-603B-54A8-8C88-16E5E2FA648D', N'سایز', N'40'), ('7942609D-603B-54A8-8C88-16E5E2FA648D', N'رنگ', N'بردو');
END
GO

-- ===== جوراب ساق‌بلند نخی =====
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Slug = N'cotton-long-socks')
BEGIN
  INSERT INTO dbo.Products (Id, CategoryId, BrandId, Name, Slug, Description, BasePrice, Status, IsFeatured)
  VALUES ('1EA20068-6C48-5F78-892C-CC054B8DDD7A', (SELECT Id FROM dbo.Categories WHERE Slug = N'jorab'), (SELECT Id FROM dbo.Brands WHERE Slug = N'mahtab'), N'جوراب ساق‌بلند نخی', N'cotton-long-socks', N'جوراب ساق‌بلند از نخ پنبه‌ی نرم، مناسب استفاده‌ی روزانه و فصل‌های سرد.', 120000, N'Published', 1);
  INSERT INTO dbo.ProductImages (Id, ProductId, ImageUrl, SortOrder, IsPrimary) VALUES ('D529020E-0193-5A43-AA80-189BF1BD98CD', '1EA20068-6C48-5F78-892C-CC054B8DDD7A', N'https://picsum.photos/seed/cotton-long-socks-1/600/800', 0, 1);
  INSERT INTO dbo.ProductImages (Id, ProductId, ImageUrl, SortOrder, IsPrimary) VALUES ('D05B4294-8426-542E-89A2-88FA49268C66', '1EA20068-6C48-5F78-892C-CC054B8DDD7A', N'https://picsum.photos/seed/cotton-long-socks-2/600/800', 1, 0);
  INSERT INTO dbo.ProductImages (Id, ProductId, ImageUrl, SortOrder, IsPrimary) VALUES ('F05B2726-0830-520A-ADED-9ACFE81EB2F6', '1EA20068-6C48-5F78-892C-CC054B8DDD7A', N'https://picsum.photos/seed/cotton-long-socks-3/600/800', 2, 0);
  INSERT INTO dbo.ProductVariants (Id, ProductId, Sku, Price, CompareAtPrice, StockQuantity, IsActive) VALUES ('18E276D4-9701-5B67-8142-F6DECB33F379', '1EA20068-6C48-5F78-892C-CC054B8DDD7A', N'SOC-OS-WHT', 120000, NULL, 40, 1);
  INSERT INTO dbo.ProductVariantAttributes (VariantId, AttributeName, AttributeValue) VALUES ('18E276D4-9701-5B67-8142-F6DECB33F379', N'سایز', N'یک‌سایز'), ('18E276D4-9701-5B67-8142-F6DECB33F379', N'رنگ', N'سفید');
  INSERT INTO dbo.ProductVariants (Id, ProductId, Sku, Price, CompareAtPrice, StockQuantity, IsActive) VALUES ('623A4C55-56C5-5AFC-906F-5AA4E2B50823', '1EA20068-6C48-5F78-892C-CC054B8DDD7A', N'SOC-OS-BLK', 120000, NULL, 40, 1);
  INSERT INTO dbo.ProductVariantAttributes (VariantId, AttributeName, AttributeValue) VALUES ('623A4C55-56C5-5AFC-906F-5AA4E2B50823', N'سایز', N'یک‌سایز'), ('623A4C55-56C5-5AFC-906F-5AA4E2B50823', N'رنگ', N'مشکی');
  INSERT INTO dbo.ProductVariants (Id, ProductId, Sku, Price, CompareAtPrice, StockQuantity, IsActive) VALUES ('9F0DEF2B-2FC8-54B5-94A2-DD153C2B52F5', '1EA20068-6C48-5F78-892C-CC054B8DDD7A', N'SOC-OS-GRY', 120000, NULL, 40, 1);
  INSERT INTO dbo.ProductVariantAttributes (VariantId, AttributeName, AttributeValue) VALUES ('9F0DEF2B-2FC8-54B5-94A2-DD153C2B52F5', N'سایز', N'یک‌سایز'), ('9F0DEF2B-2FC8-54B5-94A2-DD153C2B52F5', N'رنگ', N'طوسی');
END
GO

-- ===== ست لباس زیر دنتلا =====
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Slug = N'dantela-lingerie-set')
BEGIN
  INSERT INTO dbo.Products (Id, CategoryId, BrandId, Name, Slug, Description, BasePrice, Status, IsFeatured)
  VALUES ('9D7CC6E6-1915-5913-90DD-093D8F1A66D1', (SELECT Id FROM dbo.Categories WHERE Slug = N'lebas-zir'), (SELECT Id FROM dbo.Brands WHERE Slug = N'narges'), N'ست لباس زیر دنتلا', N'dantela-lingerie-set', N'ست لباس زیر با دانتل ظریف و آستر نخی؛ راحت و مناسب استفاده‌ی طولانی.', 640000, N'Published', 1);
  INSERT INTO dbo.ProductImages (Id, ProductId, ImageUrl, SortOrder, IsPrimary) VALUES ('E11D2EE4-CECD-56DE-9681-4EC6248D0720', '9D7CC6E6-1915-5913-90DD-093D8F1A66D1', N'https://picsum.photos/seed/dantela-lingerie-set-1/600/800', 0, 1);
  INSERT INTO dbo.ProductImages (Id, ProductId, ImageUrl, SortOrder, IsPrimary) VALUES ('C9F3944B-D0E3-5C4A-B79D-451FFA536BE2', '9D7CC6E6-1915-5913-90DD-093D8F1A66D1', N'https://picsum.photos/seed/dantela-lingerie-set-2/600/800', 1, 0);
  INSERT INTO dbo.ProductImages (Id, ProductId, ImageUrl, SortOrder, IsPrimary) VALUES ('64D1C0E0-1A14-56B2-94F2-179B8166ECD4', '9D7CC6E6-1915-5913-90DD-093D8F1A66D1', N'https://picsum.photos/seed/dantela-lingerie-set-3/600/800', 2, 0);
  INSERT INTO dbo.ProductVariants (Id, ProductId, Sku, Price, CompareAtPrice, StockQuantity, IsActive) VALUES ('3B81BA4D-45E6-57FE-912E-7D53A90465C0', '9D7CC6E6-1915-5913-90DD-093D8F1A66D1', N'LNG-36-BLK', 640000, NULL, 8, 1);
  INSERT INTO dbo.ProductVariantAttributes (VariantId, AttributeName, AttributeValue) VALUES ('3B81BA4D-45E6-57FE-912E-7D53A90465C0', N'سایز', N'36'), ('3B81BA4D-45E6-57FE-912E-7D53A90465C0', N'رنگ', N'مشکی');
  INSERT INTO dbo.ProductVariants (Id, ProductId, Sku, Price, CompareAtPrice, StockQuantity, IsActive) VALUES ('9D7AFDEA-F825-5A03-936E-EFF44A4B4D70', '9D7CC6E6-1915-5913-90DD-093D8F1A66D1', N'LNG-38-BLK', 640000, NULL, 8, 1);
  INSERT INTO dbo.ProductVariantAttributes (VariantId, AttributeName, AttributeValue) VALUES ('9D7AFDEA-F825-5A03-936E-EFF44A4B4D70', N'سایز', N'38'), ('9D7AFDEA-F825-5A03-936E-EFF44A4B4D70', N'رنگ', N'مشکی');
  INSERT INTO dbo.ProductVariants (Id, ProductId, Sku, Price, CompareAtPrice, StockQuantity, IsActive) VALUES ('FC78DF79-FDD2-5D4C-8589-FFF76847FB16', '9D7CC6E6-1915-5913-90DD-093D8F1A66D1', N'LNG-40-BLK', 640000, NULL, 8, 1);
  INSERT INTO dbo.ProductVariantAttributes (VariantId, AttributeName, AttributeValue) VALUES ('FC78DF79-FDD2-5D4C-8589-FFF76847FB16', N'سایز', N'40'), ('FC78DF79-FDD2-5D4C-8589-FFF76847FB16', N'رنگ', N'مشکی');
  INSERT INTO dbo.ProductVariants (Id, ProductId, Sku, Price, CompareAtPrice, StockQuantity, IsActive) VALUES ('A7CD1824-2C49-5199-B8DA-72059A2B24D2', '9D7CC6E6-1915-5913-90DD-093D8F1A66D1', N'LNG-42-BLK', 640000, NULL, 0, 1);
  INSERT INTO dbo.ProductVariantAttributes (VariantId, AttributeName, AttributeValue) VALUES ('A7CD1824-2C49-5199-B8DA-72059A2B24D2', N'سایز', N'42'), ('A7CD1824-2C49-5199-B8DA-72059A2B24D2', N'رنگ', N'مشکی');
END
GO

-- ===== تونیک روزمره کتان =====
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Slug = N'casual-linen-tunic')
BEGIN
  INSERT INTO dbo.Products (Id, CategoryId, BrandId, Name, Slug, Description, BasePrice, Status, IsFeatured)
  VALUES ('C6AC9E1E-9913-5296-BDA8-A10809CD73D1', (SELECT Id FROM dbo.Categories WHERE Slug = N'roozmareh'), (SELECT Id FROM dbo.Brands WHERE Slug = N'ava'), N'تونیک روزمره کتان', N'casual-linen-tunic', N'تونیک کتان با برش آزاد و پارچه‌ی خنک، مناسب بهار و تابستان.', 590000, N'Published', 1);
  INSERT INTO dbo.ProductImages (Id, ProductId, ImageUrl, SortOrder, IsPrimary) VALUES ('1B0B498F-D9B9-5D86-A786-4867E11720D6', 'C6AC9E1E-9913-5296-BDA8-A10809CD73D1', N'https://picsum.photos/seed/casual-linen-tunic-1/600/800', 0, 1);
  INSERT INTO dbo.ProductImages (Id, ProductId, ImageUrl, SortOrder, IsPrimary) VALUES ('D8864BDC-146C-51F4-890E-88EE05C821F7', 'C6AC9E1E-9913-5296-BDA8-A10809CD73D1', N'https://picsum.photos/seed/casual-linen-tunic-2/600/800', 1, 0);
  INSERT INTO dbo.ProductImages (Id, ProductId, ImageUrl, SortOrder, IsPrimary) VALUES ('C58A52BE-FE9A-5D0A-8844-3CF0E40873D7', 'C6AC9E1E-9913-5296-BDA8-A10809CD73D1', N'https://picsum.photos/seed/casual-linen-tunic-3/600/800', 2, 0);
  INSERT INTO dbo.ProductVariants (Id, ProductId, Sku, Price, CompareAtPrice, StockQuantity, IsActive) VALUES ('D45B44AA-6A3C-503E-A2F4-70E1DD5E5B8D', 'C6AC9E1E-9913-5296-BDA8-A10809CD73D1', N'TUN-38-CRM', 590000, NULL, 7, 1);
  INSERT INTO dbo.ProductVariantAttributes (VariantId, AttributeName, AttributeValue) VALUES ('D45B44AA-6A3C-503E-A2F4-70E1DD5E5B8D', N'سایز', N'38'), ('D45B44AA-6A3C-503E-A2F4-70E1DD5E5B8D', N'رنگ', N'کرم');
  INSERT INTO dbo.ProductVariants (Id, ProductId, Sku, Price, CompareAtPrice, StockQuantity, IsActive) VALUES ('C37DA369-23BB-5F6F-85D8-12C22F0BBD5F', 'C6AC9E1E-9913-5296-BDA8-A10809CD73D1', N'TUN-38-NVY', 590000, NULL, 7, 1);
  INSERT INTO dbo.ProductVariantAttributes (VariantId, AttributeName, AttributeValue) VALUES ('C37DA369-23BB-5F6F-85D8-12C22F0BBD5F', N'سایز', N'38'), ('C37DA369-23BB-5F6F-85D8-12C22F0BBD5F', N'رنگ', N'سرمه‌ای');
  INSERT INTO dbo.ProductVariants (Id, ProductId, Sku, Price, CompareAtPrice, StockQuantity, IsActive) VALUES ('2290BEEA-1BAC-5043-9001-D331CFDE487C', 'C6AC9E1E-9913-5296-BDA8-A10809CD73D1', N'TUN-40-CRM', 590000, NULL, 7, 1);
  INSERT INTO dbo.ProductVariantAttributes (VariantId, AttributeName, AttributeValue) VALUES ('2290BEEA-1BAC-5043-9001-D331CFDE487C', N'سایز', N'40'), ('2290BEEA-1BAC-5043-9001-D331CFDE487C', N'رنگ', N'کرم');
  INSERT INTO dbo.ProductVariants (Id, ProductId, Sku, Price, CompareAtPrice, StockQuantity, IsActive) VALUES ('78C43A7E-5517-543F-B147-5C1C10282A78', 'C6AC9E1E-9913-5296-BDA8-A10809CD73D1', N'TUN-40-NVY', 590000, NULL, 7, 1);
  INSERT INTO dbo.ProductVariantAttributes (VariantId, AttributeName, AttributeValue) VALUES ('78C43A7E-5517-543F-B147-5C1C10282A78', N'سایز', N'40'), ('78C43A7E-5517-543F-B147-5C1C10282A78', N'رنگ', N'سرمه‌ای');
  INSERT INTO dbo.ProductVariants (Id, ProductId, Sku, Price, CompareAtPrice, StockQuantity, IsActive) VALUES ('ADCFB43D-B1E2-5860-973D-4068021D8D22', 'C6AC9E1E-9913-5296-BDA8-A10809CD73D1', N'TUN-42-CRM', 590000, NULL, 7, 1);
  INSERT INTO dbo.ProductVariantAttributes (VariantId, AttributeName, AttributeValue) VALUES ('ADCFB43D-B1E2-5860-973D-4068021D8D22', N'سایز', N'42'), ('ADCFB43D-B1E2-5860-973D-4068021D8D22', N'رنگ', N'کرم');
  INSERT INTO dbo.ProductVariants (Id, ProductId, Sku, Price, CompareAtPrice, StockQuantity, IsActive) VALUES ('599F4349-F3CD-5EF6-B38E-3D36342D23DC', 'C6AC9E1E-9913-5296-BDA8-A10809CD73D1', N'TUN-42-NVY', 590000, NULL, 0, 1);
  INSERT INTO dbo.ProductVariantAttributes (VariantId, AttributeName, AttributeValue) VALUES ('599F4349-F3CD-5EF6-B38E-3D36342D23DC', N'سایز', N'42'), ('599F4349-F3CD-5EF6-B38E-3D36342D23DC', N'رنگ', N'سرمه‌ای');
END
GO

-- ===== پیراهن شب بلند =====
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Slug = N'long-night-dress')
BEGIN
  INSERT INTO dbo.Products (Id, CategoryId, BrandId, Name, Slug, Description, BasePrice, Status, IsFeatured)
  VALUES ('AC6F74C7-5556-58CC-8A31-E858E51B7105', (SELECT Id FROM dbo.Categories WHERE Slug = N'lebas-majlesi'), (SELECT Id FROM dbo.Brands WHERE Slug = N'ava'), N'پیراهن شب بلند', N'long-night-dress', N'پیراهن بلند شب با پارچه‌ی ساتن و دنباله‌ی ملایم؛ مخصوص مراسم رسمی.', 2450000, N'Published', 1);
  INSERT INTO dbo.ProductImages (Id, ProductId, ImageUrl, SortOrder, IsPrimary) VALUES ('40AF7BF4-5017-548D-A847-71494DAC1350', 'AC6F74C7-5556-58CC-8A31-E858E51B7105', N'https://picsum.photos/seed/long-night-dress-1/600/800', 0, 1);
  INSERT INTO dbo.ProductImages (Id, ProductId, ImageUrl, SortOrder, IsPrimary) VALUES ('2D3B267A-6EEC-584D-9B71-C5ADF66D7BD6', 'AC6F74C7-5556-58CC-8A31-E858E51B7105', N'https://picsum.photos/seed/long-night-dress-2/600/800', 1, 0);
  INSERT INTO dbo.ProductImages (Id, ProductId, ImageUrl, SortOrder, IsPrimary) VALUES ('E688CB28-85FB-5803-B0A7-36186F8A1AC4', 'AC6F74C7-5556-58CC-8A31-E858E51B7105', N'https://picsum.photos/seed/long-night-dress-3/600/800', 2, 0);
  INSERT INTO dbo.ProductVariants (Id, ProductId, Sku, Price, CompareAtPrice, StockQuantity, IsActive) VALUES ('FD284C08-5454-50BA-888A-A69DCA9D7D4E', 'AC6F74C7-5556-58CC-8A31-E858E51B7105', N'NGT-36-BLK', 2450000, NULL, 3, 1);
  INSERT INTO dbo.ProductVariantAttributes (VariantId, AttributeName, AttributeValue) VALUES ('FD284C08-5454-50BA-888A-A69DCA9D7D4E', N'سایز', N'36'), ('FD284C08-5454-50BA-888A-A69DCA9D7D4E', N'رنگ', N'مشکی');
  INSERT INTO dbo.ProductVariants (Id, ProductId, Sku, Price, CompareAtPrice, StockQuantity, IsActive) VALUES ('E9F236E1-1004-5103-B71C-6F587539ACA9', 'AC6F74C7-5556-58CC-8A31-E858E51B7105', N'NGT-36-EMR', 2450000, NULL, 3, 1);
  INSERT INTO dbo.ProductVariantAttributes (VariantId, AttributeName, AttributeValue) VALUES ('E9F236E1-1004-5103-B71C-6F587539ACA9', N'سایز', N'36'), ('E9F236E1-1004-5103-B71C-6F587539ACA9', N'رنگ', N'زمردی');
  INSERT INTO dbo.ProductVariants (Id, ProductId, Sku, Price, CompareAtPrice, StockQuantity, IsActive) VALUES ('E1A78161-F8DE-5273-86F0-1224B3379810', 'AC6F74C7-5556-58CC-8A31-E858E51B7105', N'NGT-38-BLK', 2450000, NULL, 3, 1);
  INSERT INTO dbo.ProductVariantAttributes (VariantId, AttributeName, AttributeValue) VALUES ('E1A78161-F8DE-5273-86F0-1224B3379810', N'سایز', N'38'), ('E1A78161-F8DE-5273-86F0-1224B3379810', N'رنگ', N'مشکی');
  INSERT INTO dbo.ProductVariants (Id, ProductId, Sku, Price, CompareAtPrice, StockQuantity, IsActive) VALUES ('32785148-3677-5AE5-93C9-8C2FAEF831FE', 'AC6F74C7-5556-58CC-8A31-E858E51B7105', N'NGT-38-EMR', 2450000, NULL, 3, 1);
  INSERT INTO dbo.ProductVariantAttributes (VariantId, AttributeName, AttributeValue) VALUES ('32785148-3677-5AE5-93C9-8C2FAEF831FE', N'سایز', N'38'), ('32785148-3677-5AE5-93C9-8C2FAEF831FE', N'رنگ', N'زمردی');
  INSERT INTO dbo.ProductVariants (Id, ProductId, Sku, Price, CompareAtPrice, StockQuantity, IsActive) VALUES ('20D08458-DC32-5EC7-B36A-9756B66E35A1', 'AC6F74C7-5556-58CC-8A31-E858E51B7105', N'NGT-40-BLK', 2450000, NULL, 3, 1);
  INSERT INTO dbo.ProductVariantAttributes (VariantId, AttributeName, AttributeValue) VALUES ('20D08458-DC32-5EC7-B36A-9756B66E35A1', N'سایز', N'40'), ('20D08458-DC32-5EC7-B36A-9756B66E35A1', N'رنگ', N'مشکی');
  INSERT INTO dbo.ProductVariants (Id, ProductId, Sku, Price, CompareAtPrice, StockQuantity, IsActive) VALUES ('33680480-2491-52B0-A027-49CB6113DB74', 'AC6F74C7-5556-58CC-8A31-E858E51B7105', N'NGT-40-EMR', 2450000, NULL, 0, 1);
  INSERT INTO dbo.ProductVariantAttributes (VariantId, AttributeName, AttributeValue) VALUES ('33680480-2491-52B0-A027-49CB6113DB74', N'سایز', N'40'), ('33680480-2491-52B0-A027-49CB6113DB74', N'رنگ', N'زمردی');
END
GO

-- ===== جوراب ورزشی سه‌تایی =====
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Slug = N'sport-socks-3pack')
BEGIN
  INSERT INTO dbo.Products (Id, CategoryId, BrandId, Name, Slug, Description, BasePrice, Status, IsFeatured)
  VALUES ('C6E8BF53-E307-53C4-9B3A-B908E0E9799C', (SELECT Id FROM dbo.Categories WHERE Slug = N'jorab'), (SELECT Id FROM dbo.Brands WHERE Slug = N'mahtab'), N'جوراب ورزشی سه‌تایی', N'sport-socks-3pack', N'بسته‌ی سه‌تایی جوراب ورزشی با کفی ضدتعریق.', 150000, N'Published', 0);
  INSERT INTO dbo.ProductImages (Id, ProductId, ImageUrl, SortOrder, IsPrimary) VALUES ('1E50435C-6A47-5D78-AE2F-9638135123DA', 'C6E8BF53-E307-53C4-9B3A-B908E0E9799C', N'https://picsum.photos/seed/sport-socks-3pack-1/600/800', 0, 1);
  INSERT INTO dbo.ProductImages (Id, ProductId, ImageUrl, SortOrder, IsPrimary) VALUES ('E1CC0E19-EAEA-5B39-A7A4-DBDE96AFB4D3', 'C6E8BF53-E307-53C4-9B3A-B908E0E9799C', N'https://picsum.photos/seed/sport-socks-3pack-2/600/800', 1, 0);
  INSERT INTO dbo.ProductImages (Id, ProductId, ImageUrl, SortOrder, IsPrimary) VALUES ('A280C6E1-825C-558E-A6E8-758151B599AD', 'C6E8BF53-E307-53C4-9B3A-B908E0E9799C', N'https://picsum.photos/seed/sport-socks-3pack-3/600/800', 2, 0);
  INSERT INTO dbo.ProductVariants (Id, ProductId, Sku, Price, CompareAtPrice, StockQuantity, IsActive) VALUES ('825AA5EC-4469-5F10-A232-374F07C15CB3', 'C6E8BF53-E307-53C4-9B3A-B908E0E9799C', N'SPT-OS-WHT', 99000, 150000, 60, 1);
  INSERT INTO dbo.ProductVariantAttributes (VariantId, AttributeName, AttributeValue) VALUES ('825AA5EC-4469-5F10-A232-374F07C15CB3', N'سایز', N'یک‌سایز'), ('825AA5EC-4469-5F10-A232-374F07C15CB3', N'رنگ', N'سفید');
  INSERT INTO dbo.ProductVariants (Id, ProductId, Sku, Price, CompareAtPrice, StockQuantity, IsActive) VALUES ('1BE8D44B-103C-53BE-A116-03A1832678F7', 'C6E8BF53-E307-53C4-9B3A-B908E0E9799C', N'SPT-OS-BLK', 99000, 150000, 60, 1);
  INSERT INTO dbo.ProductVariantAttributes (VariantId, AttributeName, AttributeValue) VALUES ('1BE8D44B-103C-53BE-A116-03A1832678F7', N'سایز', N'یک‌سایز'), ('1BE8D44B-103C-53BE-A116-03A1832678F7', N'رنگ', N'مشکی');
END
GO

-- ===== سوتین بدون بند =====
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Slug = N'strapless-bra')
BEGIN
  INSERT INTO dbo.Products (Id, CategoryId, BrandId, Name, Slug, Description, BasePrice, Status, IsFeatured)
  VALUES ('CA7EA0A5-CA22-5444-BD4D-9D194836A783', (SELECT Id FROM dbo.Categories WHERE Slug = N'lebas-zir'), (SELECT Id FROM dbo.Brands WHERE Slug = N'narges'), N'سوتین بدون بند', N'strapless-bra', N'سوتین بدون بند با سیلیکون ضدلغزش و کاپ فوم‌دار.', 380000, N'Published', 0);
  INSERT INTO dbo.ProductImages (Id, ProductId, ImageUrl, SortOrder, IsPrimary) VALUES ('098C7E60-898D-56FE-B21D-9423E8802685', 'CA7EA0A5-CA22-5444-BD4D-9D194836A783', N'https://picsum.photos/seed/strapless-bra-1/600/800', 0, 1);
  INSERT INTO dbo.ProductImages (Id, ProductId, ImageUrl, SortOrder, IsPrimary) VALUES ('DDDEE465-4C0B-52C5-ADEC-55017C2B6361', 'CA7EA0A5-CA22-5444-BD4D-9D194836A783', N'https://picsum.photos/seed/strapless-bra-2/600/800', 1, 0);
  INSERT INTO dbo.ProductImages (Id, ProductId, ImageUrl, SortOrder, IsPrimary) VALUES ('36E98B0A-ACD2-5483-8B77-B15C697F493D', 'CA7EA0A5-CA22-5444-BD4D-9D194836A783', N'https://picsum.photos/seed/strapless-bra-3/600/800', 2, 0);
  INSERT INTO dbo.ProductVariants (Id, ProductId, Sku, Price, CompareAtPrice, StockQuantity, IsActive) VALUES ('73EF0303-F533-5C89-B79A-2BCD1318E772', 'CA7EA0A5-CA22-5444-BD4D-9D194836A783', N'BRA-36-BEG', 380000, NULL, 10, 1);
  INSERT INTO dbo.ProductVariantAttributes (VariantId, AttributeName, AttributeValue) VALUES ('73EF0303-F533-5C89-B79A-2BCD1318E772', N'سایز', N'36'), ('73EF0303-F533-5C89-B79A-2BCD1318E772', N'رنگ', N'بژ');
  INSERT INTO dbo.ProductVariants (Id, ProductId, Sku, Price, CompareAtPrice, StockQuantity, IsActive) VALUES ('7304D40E-95CE-5850-B63A-9C4CC21E84B7', 'CA7EA0A5-CA22-5444-BD4D-9D194836A783', N'BRA-36-BLK', 380000, NULL, 10, 1);
  INSERT INTO dbo.ProductVariantAttributes (VariantId, AttributeName, AttributeValue) VALUES ('7304D40E-95CE-5850-B63A-9C4CC21E84B7', N'سایز', N'36'), ('7304D40E-95CE-5850-B63A-9C4CC21E84B7', N'رنگ', N'مشکی');
  INSERT INTO dbo.ProductVariants (Id, ProductId, Sku, Price, CompareAtPrice, StockQuantity, IsActive) VALUES ('5F562399-53AD-58F4-9BF0-1A9D20359063', 'CA7EA0A5-CA22-5444-BD4D-9D194836A783', N'BRA-38-BEG', 380000, NULL, 10, 1);
  INSERT INTO dbo.ProductVariantAttributes (VariantId, AttributeName, AttributeValue) VALUES ('5F562399-53AD-58F4-9BF0-1A9D20359063', N'سایز', N'38'), ('5F562399-53AD-58F4-9BF0-1A9D20359063', N'رنگ', N'بژ');
  INSERT INTO dbo.ProductVariants (Id, ProductId, Sku, Price, CompareAtPrice, StockQuantity, IsActive) VALUES ('B6EEC349-4FEE-5584-84E0-B5DC3947681E', 'CA7EA0A5-CA22-5444-BD4D-9D194836A783', N'BRA-38-BLK', 380000, NULL, 10, 1);
  INSERT INTO dbo.ProductVariantAttributes (VariantId, AttributeName, AttributeValue) VALUES ('B6EEC349-4FEE-5584-84E0-B5DC3947681E', N'سایز', N'38'), ('B6EEC349-4FEE-5584-84E0-B5DC3947681E', N'رنگ', N'مشکی');
  INSERT INTO dbo.ProductVariants (Id, ProductId, Sku, Price, CompareAtPrice, StockQuantity, IsActive) VALUES ('48DD47F3-728D-52D2-BB61-393862D27A4F', 'CA7EA0A5-CA22-5444-BD4D-9D194836A783', N'BRA-40-BEG', 380000, NULL, 10, 1);
  INSERT INTO dbo.ProductVariantAttributes (VariantId, AttributeName, AttributeValue) VALUES ('48DD47F3-728D-52D2-BB61-393862D27A4F', N'سایز', N'40'), ('48DD47F3-728D-52D2-BB61-393862D27A4F', N'رنگ', N'بژ');
  INSERT INTO dbo.ProductVariants (Id, ProductId, Sku, Price, CompareAtPrice, StockQuantity, IsActive) VALUES ('A1A4B757-37B9-5308-B0CF-5787CFE6498F', 'CA7EA0A5-CA22-5444-BD4D-9D194836A783', N'BRA-40-BLK', 380000, NULL, 0, 1);
  INSERT INTO dbo.ProductVariantAttributes (VariantId, AttributeName, AttributeValue) VALUES ('A1A4B757-37B9-5308-B0CF-5787CFE6498F', N'سایز', N'40'), ('A1A4B757-37B9-5308-B0CF-5787CFE6498F', N'رنگ', N'مشکی');
END
GO

-- ===== شلوار جین راسته =====
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE Slug = N'straight-jeans')
BEGIN
  INSERT INTO dbo.Products (Id, CategoryId, BrandId, Name, Slug, Description, BasePrice, Status, IsFeatured)
  VALUES ('1A154D48-3757-502A-BB06-23F8167E27F2', (SELECT Id FROM dbo.Categories WHERE Slug = N'roozmareh'), (SELECT Id FROM dbo.Brands WHERE Slug = N'mahtab'), N'شلوار جین راسته', N'straight-jeans', N'شلوار جین راسته با پارچه‌ی کش‌دار و کمر بلند.', 720000, N'Published', 0);
  INSERT INTO dbo.ProductImages (Id, ProductId, ImageUrl, SortOrder, IsPrimary) VALUES ('0ADBC7FE-1046-57D1-A5AD-6EAA2F448E68', '1A154D48-3757-502A-BB06-23F8167E27F2', N'https://picsum.photos/seed/straight-jeans-1/600/800', 0, 1);
  INSERT INTO dbo.ProductImages (Id, ProductId, ImageUrl, SortOrder, IsPrimary) VALUES ('2CB686C0-1594-5A74-BDC5-1E8F6F55F3ED', '1A154D48-3757-502A-BB06-23F8167E27F2', N'https://picsum.photos/seed/straight-jeans-2/600/800', 1, 0);
  INSERT INTO dbo.ProductImages (Id, ProductId, ImageUrl, SortOrder, IsPrimary) VALUES ('B3479D1F-D088-5691-B2CF-889AA1C534CC', '1A154D48-3757-502A-BB06-23F8167E27F2', N'https://picsum.photos/seed/straight-jeans-3/600/800', 2, 0);
  INSERT INTO dbo.ProductVariants (Id, ProductId, Sku, Price, CompareAtPrice, StockQuantity, IsActive) VALUES ('D46EE9BE-149A-58F3-B7A3-87F795A2E45F', '1A154D48-3757-502A-BB06-23F8167E27F2', N'JNS-36-BLU', 720000, NULL, 9, 1);
  INSERT INTO dbo.ProductVariantAttributes (VariantId, AttributeName, AttributeValue) VALUES ('D46EE9BE-149A-58F3-B7A3-87F795A2E45F', N'سایز', N'36'), ('D46EE9BE-149A-58F3-B7A3-87F795A2E45F', N'رنگ', N'آبی');
  INSERT INTO dbo.ProductVariants (Id, ProductId, Sku, Price, CompareAtPrice, StockQuantity, IsActive) VALUES ('5462D17A-D483-5D2A-A2A0-6A2834C3B387', '1A154D48-3757-502A-BB06-23F8167E27F2', N'JNS-38-BLU', 720000, NULL, 9, 1);
  INSERT INTO dbo.ProductVariantAttributes (VariantId, AttributeName, AttributeValue) VALUES ('5462D17A-D483-5D2A-A2A0-6A2834C3B387', N'سایز', N'38'), ('5462D17A-D483-5D2A-A2A0-6A2834C3B387', N'رنگ', N'آبی');
  INSERT INTO dbo.ProductVariants (Id, ProductId, Sku, Price, CompareAtPrice, StockQuantity, IsActive) VALUES ('10E7882E-BFFF-5E23-9E3E-2C564B86B30D', '1A154D48-3757-502A-BB06-23F8167E27F2', N'JNS-40-BLU', 720000, NULL, 9, 1);
  INSERT INTO dbo.ProductVariantAttributes (VariantId, AttributeName, AttributeValue) VALUES ('10E7882E-BFFF-5E23-9E3E-2C564B86B30D', N'سایز', N'40'), ('10E7882E-BFFF-5E23-9E3E-2C564B86B30D', N'رنگ', N'آبی');
  INSERT INTO dbo.ProductVariants (Id, ProductId, Sku, Price, CompareAtPrice, StockQuantity, IsActive) VALUES ('7D49AEE3-B04E-52F3-88DD-4969F845A64E', '1A154D48-3757-502A-BB06-23F8167E27F2', N'JNS-42-BLU', 720000, NULL, 0, 1);
  INSERT INTO dbo.ProductVariantAttributes (VariantId, AttributeName, AttributeValue) VALUES ('7D49AEE3-B04E-52F3-88DD-4969F845A64E', N'سایز', N'42'), ('7D49AEE3-B04E-52F3-88DD-4969F845A64E', N'رنگ', N'آبی');
END
GO

PRINT N'داده‌ی نمونه‌ی کاتالوگ وارد شد.';
GO
