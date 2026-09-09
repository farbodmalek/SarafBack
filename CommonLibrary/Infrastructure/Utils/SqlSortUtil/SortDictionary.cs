

namespace CommonLibrary.Infrastructure.Utils.SqlSortUtil
{
    public static class SortDictionary
    {
        public static Dictionary<string, string> LoanSortList
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { "id", "loan.Id" },
                    { "AcceptAmount", "lcc.AcceptAmount" },
                    { "LoanAmount", "lcc.AcceptAmount" },
                    { "beginDate", "lcc.BeginDate" },
                    { "NumberOfDelayedInstallment", "isnull(flc.NumberOfDelayedInstallment,0)" },
                    { "TotalDelayedAmount", "isnull(flc.TotalDelayedAmount,0)" },
                    { "h_id", "loan.Id"},
                    { "h_actor", "isnull(loan.LastActorId,-1)"},
                    { "h_branche_supervisorNames", "branch.BranchName"},
                    { "h_factLoanAcc_totalRefundAmount", "isnull(flc.TotalRefundAmount,0)"},
                    { "h_factLoanAcc_loanStatusTypeDesc", "isnull(flc.LoanStatusTypeId,'')"},
                    { "h_loanNumber", "loan.LoanNumber"},
                    { "h_customer", "isnull(cust.FullName,'')"},
                    { "h_loanContract_acceptAmount", "lcc.AcceptAmount"},
                    { "h_loantype", "isnull(lit.[Desc],'')" },
                    { "h_loanContract_beginDate", "lcc.BeginDate"},
                    { "h_branche_supervisorName", "branch.SupervisorName" },
                    { "h_factLoanAcc_lastInstallmentDate", "flc.LastInstallmentDate"},
                    { "h_factLoanAcc_numberOfDelayedInstallment", "isnull(flc.NumberOfDelayedInstallment,0)"},
                    { "h_factLoanAcc_totalDelayedAmount", "isnull(flc.TotalDelayedAmount,0)" },
                    { "h_factLoanAcc_visionAmount", "isnull(flc.VisionAmount,0)"},
                    { "h_last_action", "loan.LastActionId" },
                    { "h_factLoanAcc_referenceDate", "actor.ReferenceDate" },
                    { "lastSurveyDateFa", "srv.SurveyDate" },
                    { "totalAmount", "totalAmount" },
                    { "penaltyAmount", "penaltyAmount" },
                    { "mainAmount", "mainAmount" },
                    {"profitAmount", "profitAmount" },
                    {"waitProfitAmount", "waitProfitAmount" },
                    {"payDetailDate", "payDetailDate" },
                    {"payProfit", "payProfit" },
                    {"payAmount", "payAmount" },
                };
            }
        }

        public static Dictionary<string, string> LoanAggregateList
        {
            get
            {
                return new Dictionary<string, string>()
                {
                     { "Code", "a.Code" },
                    {"RemainAmount", "RemainAmount" },
                    {"TotalRemainAmount", "TotalRemainAmount" },
                    {"CurrentAmount", "CurrentAmount" },
                    {"UnCurrentAmount", "UnCurrentAmount" },
                    {"DoubtfullAmount", "DoubtfullAmount" },
                    {"DelayedAmount", "DelayedAmount" },
                    {"PastDuoAmount", "PastDuoAmount" },
                    {"Title", "a.Title" },

                };
            }
        }

        public static Dictionary<string, string> ActorWageList
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    {"calculationDate", "a.ComputeDate" },
                    {"supervisorConfirmationDate", "a.SupervisorConfirmationDate" },
                    {"cityManagerConfirmationDate", "a.CityConfirmationDate" },
                    {"actorConfirmationDate", "a.actorConfirmationDate" },
                    {"Id", "a.Id" },
                };
            }
        }

        public static Dictionary<string, string> LoanPayBankCard
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { "id", "lpc.Id" },
                     { "payDate", "lpc.PayDate" },
                    { "totalAmount", "totalAmount" },
                    { "penaltyAmount", "penaltyAmount" },
                    { "mainAmount", "mainAmount" },
                    {"profitAmount", "profitAmount" },
                    {"waitProfitAmount", "waitProfitAmount" },
                    {"payDetailDate", "payDetailDate" },
                    {"payProfit", "payProfit" },
                    {"payAmount", "payAmount" },
                };
            }
        }

        public static Dictionary<string, string> LoanPayInfoeDetailListModel
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { "id", "lpi.Id" },
                    { "payDate", "lpi.payDate" },
                    { "payNo", "lpi.payNo" },
                    { "baseAmount", "lpi.baseAmount" },
                    {"penaltyAmount", "lpi.penaltyAmount" },
                    {"mainAmount", "lpi.mainAmount" },
                    {"profitAmount", "lpi.ProfitAmount" },
                    {"waitProfitAmount", "lpi.waitProfitAmount" },
                    {"accountNo", "lpi.accountNo" },
                    {"remainAmount", "lpd2.remainAmount" },
                    {"payDateTime", "lpi.payDateTime" },
                };
            }
        }

        public static Dictionary<string, string> ReferredCases
        {
            get
            {
                return new Dictionary<string, string>()
                {
                     {"lastReferenceDate", "ReferenceDate" },
                    { "SupervisorName", "SupervisorName" },
                    { "BranchName", "Branchcode" },
                    {"Begindate", "BeginDate" },
                    {"AcceptAmount", "AcceptAmount" },
                    {"DelayedInstallmentCount", "InstallmentCount" },
                    {"lastActorUserName", "lastactoruserid" },
                };
            }
        }


        public static Dictionary<string, string> LoanPayBank
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { "id", "lpb.Id" },
                    { "payDate", "lpb.payDate" },
                    { "loanId", "lpb.loanId" },
                    { "payAmount", "lpb.payAmount" },
                };
            }
        }



        public static Dictionary<string, string> LoanDailyStatusList
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { "id", "id" },
                    { "rBDate", "B.rBDate" },
                    { "loanStatusTypeId", "B.loanStatusTypeId" },
                    { "nextDate", "nextDate" },
                    { "dayCount", "dayCount" },
                    { "effectiveTime", "effectiveTime" },
                    { "loanId", "B.loanId" },


                };
            }
        }
        
        public static Dictionary<string, string> ReferenceToMeSurvey
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { "id", "id" },
                    { "CustomerNumber", "cs.CustomerNumber" },
                    { "LastReferenceDate", "lrw.LastReferenceDate" },
                    { "LastSurveyDate", "LastSurveyDate" },
                    { "LastLoanStatusDesc", " ls.[Desc]" },
                   
                };
            }
        }




        public static Dictionary<string, string> SurveySortList
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { "h_end", "loan.Id" },
                    { "registerDate", "lc.AcceptDate" },
                    { "h_id", "loan.Id"},
                    { "h_supervisor", "b.supervisorCode"},
                    { "h_actor", "isnull(loan.LastActorId,-1)"},
                    { "h_branche_supervisorNames", "branch.BranchName"},
                    { "h_factLoanAcc_totalRefundAmount", "isnull(flc.TotalRefundAmount,0)"},
                    { "h_factLoanAcc_loanStatusTypeDesc", "isnull(flc.LoanStatusTypeId,'')"},
                    { "h_loanNumber", "loan.LoanNumber"},
                    { "h_customer", "isnull(c.FullName,'')"},
                    { "h_loanContract_acceptAmount", "isnull(lc.AcceptAmount,0)"},
                    {"h_loantype", "isnull(lit.[Desc],'')" },
                    { "h_loanContract_beginDate", "lc.BeginDate"},
                    {"h_branche_supervisorName", "branch.SupervisorName" },
                    { "h_factLoanAcc_lastInstallmentDate", "flc.LastInstallmentDate"},
                    { "h_factLoanAcc_numberOfDelayedInstallment", "isnull(flc.NumberOfDelayedInstallment,0)"},
                    {"h_factLoanAcc_totalDelayedAmount", "isnull(flc.TotalDelayedAmount,0)" },
                    { "h_factLoanAcc_visionAmount", "isnull(flc.VisionAmount,0)"},
                    {"h_last_action", "isnull(loan.LastActionId,-1)" },
                    {"h_factLoanAcc_referenceDateFa", "actor.ReferenceDate" },
                    { "lastSurveyDateFa", "srv.SurveyDate" }
            };
            }
        }
        public static Dictionary<string, string> SurveyWageSortList
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { "h_wageDate", "usw.ComputeDate" },
                    { "h_surveyUser", "u.FullUserName" },
                    { "h_supervisor", "SupervisorName"},
                    { "h_branch", "BranchName"},
                    { "h_amount", "usw.WageAmount"},
                    { "h_surveyCount", "usw.PointCount"},
                    { "h_shortDistancePointCount", "usw.ShortDistancePointCount"},
                    { "h_longdistance", "usw.LongDistance"}
            };
            }
        }
        public static Dictionary<string, string> ActorAggregatedWage
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { "", "prh.RequestYear desc,prh.RequestMonth desc" },
                    { "PaymentMonth", "prh.RequestMonth" },
                    { "paymentYear", "prh.RequestYear" },
                    { "actor", "u.FullUserName" },
                    { "ReferralAmount", "COUNT(actor.Id)" },
                    { "CollectedDebt", "SUM(actor.PayedAmount)" },
                    { "WageAmount", "SUM(actor.WageAmount)" },
                    { "ReceivedAmount", "SUM(actor.Amount)" },
                    { "BankCost", "SUM(actor.WageAmount)-SUM(actor.Amount)" }
                };
            }
        }
        public static Dictionary<string, string> BranchAggregatedWage
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { "", "prh.RequestYear desc,prh.RequestMonth desc" },
                    { "PaymentMonth", "prh.RequestMonth" },
                    { "paymentYear", "prh.RequestYear" },
                    { "branch", "branch.branchName " },
                    { "ReferralAmount", "COUNT(actor.Id)" },
                    { "CollectedDebt", "SUM(actor.PayedAmount)" },
                    { "WageAmount", "SUM(actor.WageAmount)" },
                    { "ReceivedAmount", "SUM(actor.Amount)" },
                    { "BankCost", "SUM(actor.WageAmount)-SUM(actor.Amount)" }
                };
            }
        }

        public static Dictionary<string, string> SurveyAddressRequestsList
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { "SurveyAddressStatusId", "CLPS.RequestDate desc"},
                    { "BeginDate", "BeginDate"},
                    { "LastSurveyDate", "LastSurveyDate"},
                    { "surveyUserName", "a.RequestUserId" },
                    { "ConfirmDate", "a.ConfirmDate"},
                };
            }
        }

        public static Dictionary<string, string> SupervisorAggregatedWage
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { "", "prh.RequestYear desc,prh.RequestMonth desc" },
                    { "PaymentMonth", "prh.RequestMonth" },
                    { "paymentYear", "prh.RequestYear" },
                    { "supervisor", "branch.SupervisorName " },
                    { "ReferralAmount", "COUNT(actor.Id)" },
                    { "CollectedDebt", "SUM(actor.PayedAmount)" },
                    { "WageAmount", "SUM(actor.WageAmount)" },
                    { "ReceivedAmount", "SUM(actor.Amount)" },
                    { "BankCost", "SUM(actor.WageAmount)-SUM(actor.Amount)" }
                };
            }
        }
        public static Dictionary<string, string> CustomerSortList
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { "", "C.Id" },
                    { "customerNumber", "C.CustomerNumber" },
                    { "firstName", "C.FirstName" },
                    { "lastName", "C.LastName" },
                    { "nationalNumber", "C.NationalNumber" },
                    { "idNumber", "C.IdNumber" },
                    { "fatherName", "C.FatherName" },
                    { "mobileNo", "C.MobileNo" },
                    { "homePhone", "C.Phone" },
                    { "homePostalCode", "C.PostalCode" },
                    { "branchName", "b.BranchName" },

                };
            }
        }






        public static Dictionary<string, string> CustomerModelSortList
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { "id", "C.Id" },
                    { "customerNumber", "C.CustomerNumber"},
                    { "firstName", "isnull(c.FirstName,'')"},
                    { "lastName", " isnull(c.LastName,'')"},
                    { "fullName", "isnull(c.FullName,'')"},
                    { "nationalNumber", "isnull(c.NationalNumber,'')"},
                    { "legalNationalityNumber", " isnull(c.LegalNationalityNumber,'')"},
                    { "idNumber", "isnull(c.IdNumber,0)"},
                    { "fatherName", "isnull(c.FatherName,'')"},
                    { "mobileNo", "isnull(c.MobileNo,'') " },
                    { "branchId", "c.BranchId"},
                    { "isDeath", "ISNULL(c.IsDeath,0)" },
                    { "isDeathName", "Case When ISNULL(c.IsDeath,0)=1 Then N'فوتی' Else '' End"},
                    { "isCompany", "isnull(c.IsCompany,0)"},
                    { "IsCompanyName", "Case When isnull(c.IsCompany,0)=1 Then N'مشتری حقوقی' Else N'مشتری حقیقی' End" },
                    { "branchName", "branchName"},
                    { "supervisorCode", "SupervisorCode" },
                    { "supervisorName", "SupervisorName" },
                    { "isLegal", "isnull(l.IsLegal,0)" },
                    { "isLegalDesc", "case When isnull(l.IsLegal,0) = 1 Then N'وام حقوقی' Else N'وام غیر حقوقی' End" },
                };
            }
        }

        public static Dictionary<string, string> LastSurveyList
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { "TotalDelayedAmount", "TotalDelayedAmount"},
                    { "MaturedInstallmentsCount", "MaturedInstallmentsCount"},
                    { "BankPayAmount", "BankPayAmount"},
                    { "SupervisorName", "SupervisorName"},
                };
            }
        }

        public static Dictionary<string, string> SurveyList
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { "AcceptDate", " loanAmount"},
                    { "registerDate", "BeginDate"},
                    { "loanAmount", "loanAmount"},
                    { "DelayedInstallmentCount", "DelayedInstallmentCount" },
                    { "lastReferenceDate", "LastReferenceDate"},
                    { "lastSurveyDate","LastSurveyDate"}
                };
            }
        }


        public static Dictionary<string, string> ALLSurveyList
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { "AcceptDate", " loanAmount"},
                    { "BeginDate", "BeginDate"},
                    { "loanAmount", "loanAmount"},
                    { "DelayedInstallmentCount", "DelayedInstallmentCount" },
                    { "lastReferenceDate", "LastReferenceDate"},
                    { "lastSurveyDate","LastSurveyDate"}
                };
            }
        }


        public static Dictionary<string, string>  ClaimList
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { "Id", "Id"},
                    { "FirstDelayedInstallmentDate", "FirstDelayedInstallmentDate"},
                    { "NumberOfDelayedInstallment", "NumberOfDelayedInstallment"},
                    { "loanAmount", "loanAmount"},
                    { "BeginDate", "BeginDate" },
                    { "LastReferenceDate", "LA.ReferenceDate"},
                };
            }
        }



        public static Dictionary<string, string> RequestPlanNoList
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { "PlanNoStatusId", "a.PlanNoStatusId, a.RequestDate"},
                    { "BeginDate", "BeginDate"},
                    { "LastSurveyDate", "LastSurveyDate"},
                    { "surveyUserName", "a.RequestUserId" },
                    { "ConfirmDate", "a.ConfirmDate"},
                };
            }
        }

        public static Dictionary<string, string> FacilityFileDetailsSort
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { "BeginDatefa", "BeginDate" },
                    { "BankPayAmount", "BankPayAmount" },
                    { "TotalDelayedAmount", "TotalDelayedAmount"},
                    { "delayedInstallmentCount", "NumberOfDelayedInstallment"}
                };
            }
        }

    }
}
