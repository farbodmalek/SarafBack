namespace GirlyShop.AuthService.Core.Exceptions;

// مثلاً: شماره موبایل تکراری  →  خروجی 409
public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}

// مثلاً: رمز اشتباه یا توکن نامعتبر  →  خروجی 401
public class AuthenticationFailedException : Exception
{
    public AuthenticationFailedException(string message) : base(message) { }
}
