using System.Threading.RateLimiting;
using GirlyShop.AuthService.Infrastructure.DependencyInjection;
using GirlyShop.AuthService.Infrastructure.ExceptionHandling;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// مدیریت خطاها (خروجی { "message": "..." })
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// دیتابیس، CQRS، هش رمز، JWT
builder.Services.AddAuthServices(builder.Configuration);

// اجازه به فرانت‌اند (Next.js) برای صدا زدن این سرویس
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                     ?? Array.Empty<string>();
builder.Services.AddCors(options =>
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod()));

// جلوگیری از حمله «حدس زدن رمز»: هر IP در هر دقیقه حداکثر ۱۰ تلاش
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("auth", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
});

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();   // توضیح API در /openapi/v1.json
}

// در محیط توسعه، فرانت با http://localhost:5100 صحبت می‌کند؛ اگر اینجا به https هدایتش کنیم،
// مرورگر درخواست CORS را رد می‌کند. در سرور واقعی redirect فعال می‌شود.
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowFrontend");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.Run();
