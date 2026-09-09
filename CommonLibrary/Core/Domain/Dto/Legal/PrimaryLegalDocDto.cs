using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Core.Domain.Dto.Legal
{
    public class PrimaryLegalDocDto
    {
        public int Id { get; set; }
        public int? DocRegistrationTypeID { get; set; }
        public int? DocJudiciaryTypeID { get; set; }
        public int DocGroupTypeID { get; set; }
        public int Lah_AlahTypeID { get; set; }
        public int? BankAreaID { get; set; }
        public int? CashBranchID { get; set; }
        public int DocStatusTypeID { get; set; }
        public int? DebtorActorID { get; set; }
        public int? ClaimantTypeID { get; set; }
        public string? Complainant { get; set; }
        public int? SubjectTypeID { get; set; }
        public int? ClaimDocID { get; set; }
        public string? LegalAreaSendDate { get; set; }
        public string? RegDate { get; set; }
        public int? ClaimAmountJudicial { get; set; }
        public int? ClaimAmountOfficial { get; set; }
        public long? DecisiveAmount { get; set; }
        public int? Pmid { get; set; }
        public string? TrackingCode { get; set; }
        public string? DocSerialNo { get; set; }
        public string? ExecutiveSerialNo { get; set; }
        public int? CustomerID { get; set; }
        public string? CustomerFullName { get; set; }
        public string? LastPayBankeDate { get; set; }
        public int? NonExecutionAmount { get; set; }
        public int? LastActionID { get; set; }
        public int? ComissionTypeID { get; set; }
        public string? ContractNo { get; set; }
        public long TotalDelayedAmount { get; set; }


    }
}
