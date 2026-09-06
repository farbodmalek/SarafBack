using CommonLibrary.Core.Domain.Interfaces;
using CommonLibrary.Infrastructure.Utils.Extensions;
using System.Text;

namespace LoanMonitoringMicroService.Core.DomainServices.Survey.SurveyQueryBuilder
{
    public class SurveyWhereSectionBuilder
    {
        public static string MakeKeywordQuery(object value)
        {
            return $" and (cast(loan.CustomerNumber as varchar) like '{value}%' or cs.FullName like N'%{value}%' or NationalNumber = '{value}')";
        }
        public static string MakeSurveyUserQuery(object value)
        {
            return $" AND lrw.LastSurveyUserId={value}";
        }
        public static string MakeCustomerNumberQuery(object value)
        {
            return $" AND cs.CustomerNumber={value}";
        }
        public static string MakeBranchListQuery(object value)
        {
            if (value != null && ((List<int>)value).Count > 0)
                return $" AND branch.BranchCode in ({((List<int>)value).JoinToString()})";
            return "";
        }
        public static string MakeSupervisorQuery(object value)
        {
            int LoanAmountMin = (int)value;
            return $" and branch.SupervisorCode = {LoanAmountMin}";
        }
        public static string MakeSupervisorsQuery(object value)
        {
            if (value != null && ((List<int>)value).Count > 0)
                return $" and branch.SupervisorCode in ({((List<int>)value).JoinToString()})";
            return "";
        }
        public static string MakeReagentQuery(object value)
        {
            if (value != null && ((List<int>)value).Count > 0)
                return $"  and loan.Id in (Select LoanId From Core.LoanGuaranties as lg WHERE lg.GuarantorCustomerNumber in({((List<int>)value).JoinToString()}) and lg.RelationTypeId=3)";
            return "";
        }
        
        public static string MakeLoanStatusTypeIdListQuery(object value)
        {
            string query = " ";
            List<string> LoanStatusTypeIdList = (List<string>)value;
            //وضعیت پرونده
            if (LoanStatusTypeIdList?.Count > 0)
            {
                string loanStatusStr = "";
                for (int i = 0; i <= LoanStatusTypeIdList.Count - 1; i++)
                {
                    loanStatusStr = loanStatusStr + ",'" + LoanStatusTypeIdList[i] + "'";
                }
                loanStatusStr = loanStatusStr.Substring(1, loanStatusStr.Length - 1);
                query = $" and loan.LoanStatusTypeId in ({loanStatusStr})";
            }
            return query;
        }



        public static string MakeLegalRefrenceStatusIdQuery(object value)
        {
            int LegalRefrenceStatusId = (int)value;
            StringBuilder query = new StringBuilder("");
            //-+-- پرونده حقوقي 
            if (LegalRefrenceStatusId > 0)
            {
                //  loan.IsLegal<> 1 and loan.IsReadyToLegal = 1
                if (LegalRefrenceStatusId < 6)
                    query.Append($"and lri.LegalReferenceStatusTypeId = {LegalRefrenceStatusId}");
                else if (LegalRefrenceStatusId == 7)//مقادیر خالی
                    query.Append(" and (lri.LegalReferenceStatusTypeId IS NULL AND loan.IsReadyToLegal <> 1 ) ");
                else
                    query.Append(" and loan.IsLegal<> 1 and loan.IsReadyToLegal = 1 ");

            }
            return query.ToString();
        }

        public static string MakePlanActivationTypeListQuery(object value)
        {
            if (value != null && ((List<int>)value).Count > 0)
                return $" AND s.PlanActivationTypeId IN ({((List<int>)value).JoinToString()})";
            return "";
        }

  
        public static string MakeCartableStatusQuery(object value)
        {

            if (value != null && ((List<int>)value).Count > 0)
                return $" AND C.CartableStatusTypeId in ({((List<int>)value).JoinToString()})";
            return "";
        }

   
      

        // فوتی 
        public static string MakeDeathQuery(object value)
        {
            return $" and cs.IsDeath={value}";
        }
     
        //--عنوان تسهیلات
        public static string MakeLoanMinorTypeListQuery(object value)
        {
            if (value != null && ((List<int>)value).Count > 0)
                return $" and loan.LoanMinorTypeId in ({((List<int>)value).JoinToString()})";
            return "";
        }

        //--مبلغ تسهیلات بیشینه
        public static string MakeLoanAmountMinQuery(object value)
        {
            long LoanAmountMin = (long)value;
            return $" AND lc.AcceptAmount >= {LoanAmountMin}";
        }

        //--مبلغ تسهیلات کمینه
        public static string MakeLoanAmountMaxQuery(object value)
        {
            long LoanAmountMax = (long)value;
            return $" AND lc.AcceptAmount <= {LoanAmountMax}";
        }

        
        //-- نوع قسط
        public static string MakeLoanInstallmentTypeIdQuery(object value)
        {
            return $" AND lc.LoanInstallmentTypeId = {value}";
        }

        public static string MakeReferenceMaxDateQuery(object value, IDateConvertor dateConvertor)
        {
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                return $"AND CONVERT(date, lrw.LastReferenceDate) <= '{dateConvertor.ConvertPersianDatetimeToGregorianOrNull(value.ToString())}'";
            }
            return "";
        }
        public static string MakeReferenceMinDateQuery(object value, IDateConvertor dateConvertor)
        {
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                return $"AND CONVERT(date, lrw.LastReferenceDate) >= '{dateConvertor.ConvertPersianDatetimeToGregorianOrNull(value.ToString())}'";
            }
            return "";
        }
        public static string MakeSurveyMinDateQuery(object value, IDateConvertor dateConvertor)
        {
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                return $" AND CONVERT(date,lrw.LastSurveyDate) >='{dateConvertor.ConvertPersianDatetimeToGregorianOrNull(value.ToString())}'";
            }
            return "";
        }
        public static string MakeSurveyMaxDateQuery(object value, IDateConvertor dateConvertor)
        {
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                return $" AND CONVERT(date,lrw.LastSurveyDate) <= '{dateConvertor.ConvertPersianDatetimeToGregorianOrNull(value.ToString())}'";
            }
            return "";
        }

        public static string MakeContractMinDateQuery(object value, IDateConvertor dateConvertor)
        {
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                return $" AND lc.BeginDate >= '{dateConvertor.ConvertPersianDatetimeToGregorianOrNull(value.ToString())}'";
            }
            return "";
        }

        public static string MakeContractMaxDateQuery(object value, IDateConvertor dateConvertor)
        {
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                return $" AND lc.BeginDate <= '{dateConvertor.ConvertPersianDatetimeToGregorianOrNull(value.ToString())}'";
            }
            return "";
        }

    }
}
