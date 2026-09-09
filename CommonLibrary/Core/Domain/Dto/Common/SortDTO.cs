
namespace CommonLibrary.Core.Domain.Dto.Common
{
    public class SortDTO
    {
        /// <summary>
        /// نحوه مرتب سازی افزایشی یا کاهشی 
        /// 1 نزولی
        /// 0 صعودی
        /// </summary>
        public int DirID { get; set; }
        public string Dir { get { return DirID == 1 ? "desc" : "asc"; } }

        /// <summary>
        /// اسم فیلد برای مرتب سازی  
        /// </summary>
        public string Field { get; set; }
    }
}
