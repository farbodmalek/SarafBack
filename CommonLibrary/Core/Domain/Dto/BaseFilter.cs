using CommonLibrary.Core.Domain.Dto.Common;

namespace CommonLibrary.Core.Domain.Dto
{
    public class BaseFilter
    {
        public int PageNumber { get; set; } = 1;
        public int Take { get; set; } = 1000;

        public int Skip
        {
            get
            {
                return (PageNumber - 1) * Take;
            }
        }
        public SortDTO? SortDTO { get; set; }
        //public string? FromDate { get; set; }
        ////[FromToCompareValidator("FromDate", ErrorMessage = "بازه ی فیلدهای تاریخ درست انتخاب شود")]
        //public string? ToDate { get; set; }
    }
}
