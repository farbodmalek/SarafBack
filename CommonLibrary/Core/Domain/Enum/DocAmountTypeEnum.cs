using Domain.Base.Models;

namespace CommonLibrary.Core.Domain.Enum
{
    public class LegalAttachmentTypeEnum : Enumeration
    {
        public static LegalAttachmentTypeEnum AttType1 = new LegalAttachmentTypeEnum(58, "تصویر اخطارهای ارسالی به وام گیرنده و ضامنین(بخش وصول مطالبات) - حقوقي");
        public static LegalAttachmentTypeEnum AttType2 = new LegalAttachmentTypeEnum(66, "تصویر اقدام بر روی چک ضمانتی ( برگشت چک ) - حقوقي");
        public static LegalAttachmentTypeEnum AttType3 = new LegalAttachmentTypeEnum(77, "تصویر گزارش نظارت (کارگروه نظارت / صندوق )");
        public static LegalAttachmentTypeEnum AttType4 = new LegalAttachmentTypeEnum(69, "تصویر سفته موجود در پرونده - حقوقي");
        public LegalAttachmentTypeEnum(int id, string name) : base(id, name)
        {
        }
    }
}
