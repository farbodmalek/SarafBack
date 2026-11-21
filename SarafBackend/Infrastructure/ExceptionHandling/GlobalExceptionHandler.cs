using Microsoft.AspNetCore.Diagnostics;
using ShopMicroservice.Core.Exceptions;

namespace ShopMicroservice.Infrastructure.ExceptionHandling
{
    /// <summary>همه‌ی خطاها از اینجا رد می‌شوند و به پاسخ JSON ساده‌ی { "message": "..." } تبدیل می‌شوند.</summary>
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
                case BusinessException:
                    status = StatusCodes.Status400BadRequest;
                    message = exception.Message;
                    break;

                case KeyNotFoundException:
                    status = StatusCodes.Status404NotFound;
                    message = exception.Message;
                    break;

                case ConflictException:
                    status = StatusCodes.Status409Conflict;
                    message = exception.Message;
                    break;

                case UnauthorizedAccessException:
                    status = StatusCodes.Status401Unauthorized;
                    message = "برای این کار باید وارد حساب کاربری شوید.";
                    break;

                default:
                    // جزئیات خطای ناشناخته فقط در لاگ سرور می‌ماند، نه برای کاربر
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
}
