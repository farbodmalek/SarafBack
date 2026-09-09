using System.ComponentModel;

namespace CommonLibrary.Infrastructure.Utils.Extensions
{
    public enum BoolType
    {
        [Description("بله/خیر")]
        YesNo = 0,
        [Description("دارد/ندارد")]
        Have = 1,
        [Description("تاهل")]
        Married = 2,
        [Description("جنیست")]
        Gender = 3,
    }
    public static class BoolExtention
    {
        public static string MakeBoolText(this bool? value, BoolType type)
        {
            if (!value.HasValue)
                return "";
            return MakeBoolText(value.Value, type);
        }
        public static string MakeBoolText(this bool value, BoolType type)
        {
            if (type == BoolType.YesNo)
                return value ? "بله" : "خیر";
            if (type == BoolType.Have)
                return value ? "دارد" : "ندارد";
            if (type == BoolType.Married)
                return value ? "متاهل" : "مجرد";
            if (type == BoolType.Gender)
                return value ? "زن" : "مرد";
            return "";
        }
    }
}
