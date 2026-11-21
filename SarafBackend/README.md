# سرویس اصلی فروشگاه (پورت 5107)

CQRS با MediatR: خواندن با Dapper، نوشتن با EF Core. توکن را سرویس Auth (پورت 5100) می‌سازد و این سرویس فقط بررسی می‌کند
(`Jwt:Issuer/Audience/Key` باید در هر دو سرویس یکی باشد).

## API ها
عمومی (بدون توکن):
- `GET  /api/product` · `/api/product/featured` · `/api/product/category/{id}` · `/api/product/slug/{slug}`
- `GET  /api/categories`
- `GET  /api/orders/options` (روش‌های ارسال و پرداخت)

مشتری (نیاز به توکن):
- سبد: `GET /api/cart` · `POST /api/cart/items {variantId, quantity}` · `PUT /api/cart/items/{variantId} {quantity}` · `DELETE /api/cart/items/{variantId}` · `DELETE /api/cart` · `POST /api/cart/merge {items:[{variantId,quantity}]}`
- آدرس: `GET/POST /api/addresses` · `PUT/DELETE /api/addresses/{id}`
- علاقه‌مندی: `GET /api/wishlist` · `GET /api/wishlist/ids` · `POST/DELETE /api/wishlist/{productId}`
- سفارش: `POST /api/orders {addressId, shippingMethod, paymentMethod}` · `GET /api/orders` · `GET /api/orders/{AV-100001}` · `POST /api/orders/{AV-100001}/cancel`

ادمین (توکن با role=Admin):
- `GET /api/admin/dashboard`
- `GET /api/admin/orders?status=&searchTerm=&pageNumber=` · `GET /api/admin/orders/{n}` · `PUT /api/admin/orders/{n}/status {status}`
- `GET/POST /api/admin/categories` · `PUT/DELETE /api/admin/categories/{id}`
- `GET /api/admin/products` · `PUT /api/admin/products/{id}/status {status}` · `POST /api/product` (ثبت/ویرایش) · `DELETE /api/product/{id}` · `GET /api/product/{id}`
- `POST /api/admin/uploads` (multipart، فیلد `file`؛ jpg/png/webp تا ۵ مگابایت)

## نکته‌های مهم
- قیمت و موجودی همیشه از دیتابیس خوانده می‌شود، نه از فرانت. ثبت سفارش موجودی را اتمی کم می‌کند و اگر یکی کم بود کل سفارش برمی‌گردد.
- سفارش «عکس لحظه‌ی خرید» را نگه می‌دارد (نام، قیمت، آدرس)؛ ویرایش یا حذف محصول سفارش‌های قدیمی را عوض نمی‌کند.
- لغو سفارش موجودی را به انبار برمی‌گرداند. «تحویل‌شده» و «لغوشده» پایان مسیرند.
- پرداخت آنلاین هنوز به درگاه بانکی وصل نیست؛ فقط روش پرداخت ثبت می‌شود.
