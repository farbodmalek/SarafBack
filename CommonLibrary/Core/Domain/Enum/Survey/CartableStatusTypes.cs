using System.ComponentModel;

namespace CommonLibrary.Core.Domain.Enum.Survey
{
    public static class CartableStatusTypes
    {
        [Description("ارجاع-در انتظار نظارت")]
        public const int Pending = 1;
        [Description("نظارت شده")]
        public const int Observed = 2;
        [Description("تحویل سیستمی-عدم انجام نظارت")]
        public const int EndByUser = 3;
        [Description("محاسبه شده")]
        public const int Computed = 4;
        [Description("حذف محاسبه")]
        public const int RemoveCompute = 5;
    }
}
