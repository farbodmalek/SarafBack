namespace CommonLibrary.Core.Domain.Entities.Common
{
    public class BaseLoan 
    {
        //    [ForeignKey("Branch")]
        public int BranchCode { get; set; }
        /// <summary>
        /// کد نوع تسهیلات
        /// </summary>
        public int LoanMinorTypeId { get; set; }
        public int CustomerNumber { get; set; }
        public int LoanSerial { get; set; }
        /// <summary>
        /// شماره پرونده تسهیلات
        /// </summary>
        public string LoanNumber { get; set; }
    }
}
