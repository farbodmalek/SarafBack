using Microsoft.AspNetCore.Mvc;
using ShopMicroservice.Infrastructure.DependencyInjection;
using ShopMicroservice.Infrastructure.ExceptionHandling;
using ShopMicroservice.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

const string FrontendCorsPolicy = "FrontendCorsPolicy";

builder.Services
    .AddControllers(options =>
    {
        // خالی‌بودن فیلدها را خود handler ها با پیام فارسی بررسی می‌کنند
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        // خطای ورودی نامعتبر هم مثل بقیه‌ی خطاها به شکل { "message": "..." } برگردد
        options.InvalidModelStateResponseFactory = _ =>
            new BadRequestObjectResult(new { message = "اطلاعات ارسالی معتبر نیست." });
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ---- CORS: اجازه دسترسی به فرانت Next.js (localhost:3000) ----
builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ---- همه سرویس‌های ماژول Product (Repository، DbContext، Dapper، MediatR، AutoMapper) ----
builder.Services.AddProductServices(builder.Configuration);

// ---- ماژول‌های فروشگاه: دسته‌بندی، سبد خرید، آدرس، علاقه‌مندی، سفارش، داشبورد ----
builder.Services.AddShopModules();

// ---- امنیت: بررسی توکن JWT که سرویس Auth ساخته ----
builder.Services.AddShopAuthentication(builder.Configuration);

// ---- خطاها به شکل { "message": "..." } برمی‌گردند ----
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// در توسعه، فرانت با http://localhost:5107 صحبت می‌کند؛ ریدایرکت به https درخواست CORS را خراب می‌کند.
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();              // عکس‌های آپلودشده: /uploads/...
app.UseCors(FrontendCorsPolicy);   // باید قبل از UseAuthentication بیاد
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
