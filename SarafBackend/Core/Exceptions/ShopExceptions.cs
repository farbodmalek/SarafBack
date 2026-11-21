namespace ShopMicroservice.Core.Exceptions
{
    /// <summary>خطای «منطق کسب‌وکار» که پیامش برای کاربر قابل‌نمایش است (مثلاً موجودی کافی نیست) → خروجی 400.</summary>
    public class BusinessException : Exception
    {
        public BusinessException(string message) : base(message) { }
    }

    /// <summary>مثلاً اسلاگ تکراری → خروجی 409.</summary>
    public class ConflictException : Exception
    {
        public ConflictException(string message) : base(message) { }
    }
}
