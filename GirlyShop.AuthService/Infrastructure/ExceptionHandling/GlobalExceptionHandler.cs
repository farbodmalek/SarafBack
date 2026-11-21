using GirlyShop.AuthService.Core.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace GirlyShop.AuthService.Infrastructure.ExceptionHandling;

// همه‌ی خطاها از اینجا رد می‌شوند و به یک پاسخ JSON ساده تبدیل می‌شوند: { "message": "..." }
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        int status;
        string message;

        switch (exception)
        {
            case ConflictException:
                status = StatusCodes.Status409Conflict;
                message = exception.Message;
                break;

            case AuthenticationFailedException:
                status = StatusCodes.Status401Unauthorized;
                message = exception.Message;
                break;

            default:
                // خطای ناشناخته: جزئیاتش فقط در لاگ سرور می‌ماند، نه برای کاربر
                _logger.LogError(exception, "Unhandled exception");
                status = StatusCodes.Status500InternalServerError;
                message = "خطای داخلی سرور. لطفاً دوباره تلاش کنید.";
                break;
        }

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(new { message }, cancellationToken);
        return true;
    }
}
