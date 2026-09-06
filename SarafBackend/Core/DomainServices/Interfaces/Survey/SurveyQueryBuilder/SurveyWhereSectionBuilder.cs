using CommonLibrary.Core.Domain.Interfaces;
using CommonLibrary.Infrastructure.Utils.Extensions;
using System.Text;

namespace LoanMonitoringMicroService.Core.DomainServices.Loans.SurveyQueryBuilder
{
    public class SurveyWhereSectionBuilder
    {
       

        public static string MakeCustomerNumberQuery(object value)
        {
            return $" and loan.CustomerNumber={value}";
        }
        public static string MakeBranchListQuery(object value)
        {
            if (value != null && ((List<int>)value).Count > 0)
                return $" and loan.BranchCode in ({((List<int>)value).JoinToString()})";
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
        public static string MakeActorQuery(object value)
        {
            return $@" and (loan.Id in(select LoanId from Vision.LoanAction laa
									        left join Vision.Actor aa on laa.ActorId=aa.Id where laa.UserId={value}))";
        }

        //------------------------------------------------------------------------------
        public static string MakeListTyperQuery(object value)
        {
            string query = " ";
            int ListType = 0;
            _ = int.TryParse(value.ToString(), out ListType);

            // --نوع لیست - نیروهای شرکتی
            if (ListType == 7)
            {
                // var config = await Context.VisionConfigs.FirstAsync();
                // strWhere += $" and isnull(loan.IsLegal,0)=0 And isnull(lcc.AcceptAmount,0)<={config.AllowedReferralLoanAmount} And isnull(flc.LoanStatusTypeId,'') in ('5','4','F') and loan.Id NOT in (select loanId from Vision.Actor where HasLoan = 1 and IsDeleted=0) AND flc.TotalDelayedAmount>0 ";
                //strWhere += actorDateCondition;
            }
            if (ListType == 3)
            {
                query += $" and loan.LastActorId<>-1 and loan.IsLegal=0 ";
            }
            if (ListType == 8)
            {
                query += $" And isnull(loan.ActorStatusTypeId,-1) = 13 and isnull(loan.IsLegal,0)=0";
            }
            if (ListType == 4)
            {
                query += $" And isnull(loan.ActorStatusTypeId,-1) = 13 and isnull(loan.IsLegal,0)=0";
            }
            if (ListType == 6)
            {
                //query+=$" And isnull(loan.LastActorUserId,-1) = { currentUserId } and isnull(loan.IsLegal,0)=0");
            }
            if (ListType == 5)
            {
                query += $" " + " and isnull(loan.IsLegal,0) = 0 And isnull(flc.LoanStatusTypeId,'') in ('3','5','4','E','F') and loan.Id NOT in (select loanId from Vision.Actor where HasLoan = 1 and IsDeleted=0) ";
            }
            if (ListType == 1)
            {
                query += $" " + " and isnull(flc.NumberOfDelayedInstallment,0) >0 and isnull(loan.IsLegal,0)=0 And isnull(flc.LoanStatusTypeId,'') in ('3','5','4','F')  and isnull(loan.LastActorId,-1) =-1 and isnull(loan.IsLegal,0)=0";
                //strWhere += actorDateCondition;
            }

            return query;
        }


        // نوع اقدام
        public static string MakeReferenceQuery(object value)
        {
            string query = " ";
            int ReferenceItem = 0;
            _ = int.TryParse(value.ToString(), out ReferenceItem);

            
           
            if (ReferenceItem == 1)
            {
                query += $" and loan.Id NOT IN (select LoanId from Vision.Actor where IsDeleted=0)";
            }
            if (ReferenceItem == 2)
            {
                query += $" and loan.Id NOT IN (select LoanId from Vision.Actor where HasLoan=1 and IsDeleted=0)";
            }
            if (ReferenceItem == 3)
            {
                query += $" And isnull(loan.ActorStatusTypeId,-1) = 13 and isnull(loan.IsLegal,0)=0";
                query +=  @" and loan.Id IN
                                           (Select B.LoanId From 
                                           (Select A.LoanId,Sum(NotNullNumber) as NotNullNumber From 
                                           (Select actor.Id as actorId,actor.LoanId,la.Id as LoanActionId,Case when la.Id is null then 0 else 1 END NotNullNumber 
                                           From Vision.Actor as actor 
                                           left Outer join Vision.LoanAction as la on actor.Id=la.ActorId and la.IsDeleted=0 where actor.IsDeleted=0)A
                                            Group BY A.LoanId)B Where B.NotNullNumber=0)" ;
            }
           
            if (ReferenceItem == 4)
            {
                query += @" and loan.Id IN
                         (Select B.LoanId From
                          (Select A.LoanId, Sum(NotNullNumber) as NotNullNumber From
                           (Select actor.Id as actorId, actor.LoanId, la.Id as LoanActionId, Case when la.Id is null then 0 else 1 END NotNullNumber
                            From Vision.Actor as actor
                            left Outer join Vision.LoanAction as la on actor.Id = la.ActorId and la.IsDeleted = 0
                             where actor.IsDeleted = 0 and actor.HasLoan=1)A
                             Group BY A.LoanId)B Where B.NotNullNumber = 0)";
            }
         
            return query;
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
        
        //مدارک ضمانتی
        public static string MakeLoanCollateTypeIdListQuery(object value)
        {
            string query = " ";
            List<int> loanCollateTypeList = (List<int>)value;
            
            if (loanCollateTypeList?.Count > 0)
            {
                string loanCollateTypeListStr = "";
                for (int i = 0; i <= loanCollateTypeList.Count - 1; i++)
                {
                    loanCollateTypeListStr = loanCollateTypeListStr + ",'" + loanCollateTypeList[i] + "'";
                }
                loanCollateTypeListStr = loanCollateTypeListStr.Substring(1, loanCollateTypeListStr.Length - 1);
                query = $" and loan.Id in (Select lcs.LoanId From Core.LoanCollats as lcs Where lcs.CollatTypeId in ({loanCollateTypeListStr}))";
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

        public static string PlanActivationTypeList(object value)
        {
           
            if (value != null && ((List<int>)value).Count > 0)
                return $" and  s.PlanActivationTypeId in ({((List<int>)value).JoinToString()})";
            return "";
        }

        public static string MakeEnactmentQuery(object value)
        {
            var str = value?.ToString();
            return str == "2" ? "AND loan.Id IN (select LoanId from core.Enactment where IsDeleted=0)" : str == "3" ? "AND loan.Id NOT IN (select LoanId from core.Enactment where IsDeleted=0)" : "";

        }

        public static string MakeCartableStatusQuery(object value)
        {

            if (value != null && ((List<int>)value).Count > 0)
                return $" AND C.CartableStatusTypeId in ({((List<int>)value).JoinToString()})";
            return "";
        }

        public static string MakeServeyLicenseQuery(object value)
        {
            var str = value?.ToString();
            return str == "2" ? " AND C.CartableStatusTypeId=2 " : "";

        }

        public static string MakeCompanyLoanQuery(object value)
        {
            var str = value?.ToString();
            return str == "2" ? "and cust.IsCompany=1" :"";
        }

        // فوتی 
        public static string MakeDeathQuery(object value)
        {
            var str = value?.ToString();
            return str == "2" ? " and isnull(cust.IsDeath,0) = 1": str == "3"? " and isnull(cust.IsDeath,0) = 0": "";
        }
        // رشته فعالیت
        public static string MakePlanNoIdQuery(object value)
        {
            return $" AND lpn.PlanNoId={value}";
        }
        //نوع اقدام کننده
        public static string MakeActorTypeIdQuery(object value)
        {
            if (((List<int>)value).Count > 0)

                return $" And  loan.Id in (Select actor.LoanId From Vision.Actor as actor inner Join Core.UserRoles as ur on actor.UserId = ur.UserId Where actor.HasLoan=1 And  Ur.RoleId =({((List<int>)value).JoinToString()}))";
            return "";
        }

        //--عنوان تسهیلات
        public static string MakeLoanMinorTypeListQuery(object value)
        {
         string query = " ";
          List<int> LoanStatusTypeIdList = (List<int>)value;
            if (LoanStatusTypeIdList?.Count > 0)
            {
                string loanStatusStr = "";
                for (int i = 0; i <= LoanStatusTypeIdList.Count - 1; i++)
                {
                    loanStatusStr = loanStatusStr + ",'" + LoanStatusTypeIdList[i] + "'";
                }
            loanStatusStr = loanStatusStr.Substring(1, loanStatusStr.Length - 1);
                query = $" and loan.LoanMinorTypeId in ({loanStatusStr})";
            }
            return query;
        }

      //--مبلغ تسهیلات بیشینه
      public static string MakeLoanAmountMinQuery(object value)
        {
            long LoanAmountMin = (long)value;
            return $" AND lcc.AcceptAmount >= {LoanAmountMin}";
        }

        //--مبلغ تسهیلات کمینه
        public static string MakeLoanAmountMaxQuery(object value)
        {
            long LoanAmountMax = (long)value;
            return $" AND lcc.AcceptAmount <= {LoanAmountMax}";
        }

        //--کل مبلغ معوق - بیشینه
        public static string MakeTotalDelayedAmountMaxQuery(object value)
        {
            return $" AND flc.TotalDelayedAmount>={value}";
        }

        //--کل مبلغ معوق - کمینه
        public static string MakeTotalDelayedAmountMinQuery(object value)
        { 
            return $" AND flc.TotalDelayedAmount>={value}";
        }
        //--تعداد اقساط معوق - بيشينه
        public static string MakeMaturedInstallmentsCountMaxQuery(object value)
        {
            return $"AND flc.NumberOfDelayedInstallment<={value}";
        }
        //--تعداد اقساط معوق - کمینه
        public static string MakeMaturedInstallmentsCountMinQuery(object value)
        {
            return $"AND flc.NumberOfDelayedInstallment>={value}";
        }
        //-- نوع قسط
        public static string MakeLoanInstallmentTypeIdQuery(object value)
        {
            return $" AND lcc.LoanInstallmentTypeId={value}";
        }
       
        
        public static string MakeReferenceMaxDateQuery(object value, IDateConvertor dateConvertor)
        {
            string query = " ";
            string ReferenceMinDate = value.ToString();
            if (!string.IsNullOrEmpty(ReferenceMinDate))
            {
                query = $" AND actor.ReferenceDate <= '{dateConvertor.ConvertPersianDatetimeToGregorianOrNull(ReferenceMinDate)}'";
            }
            return query;
        }
        public static string MakeReferenceMinDateQuery(object value, IDateConvertor dateConvertor)
        {
            string query = " ";
            string ReferenceMinDate = value.ToString();
            if (!string.IsNullOrEmpty(ReferenceMinDate))
            {
                query = $" AND actor.ReferenceDate >='{dateConvertor.ConvertPersianDatetimeToGregorianOrNull(ReferenceMinDate)}'";
            }
            return query;
        }

        public static string MakeContractMinDateQuery(object value, IDateConvertor dateConvertor)
        {
            string query = " ";
            string ContractMinDate = value.ToString();
            if (!string.IsNullOrEmpty(ContractMinDate))
            {
                var fdate = "";
                query = $" and lcc.BeginDate >= '{dateConvertor.ConvertPersianDatetimeToGregorianOrNull(ContractMinDate)}'";
            }
            return query;
        }

        public static string MakeContractMaxDateQuery(object value, IDateConvertor dateConvertor)
        {
            string query = " ";
            string ContractMaxDate = value.ToString();
            if (!string.IsNullOrEmpty(ContractMaxDate))
            {
                var fdate = "";
                query = $" and lcc.BeginDate <= '{dateConvertor.ConvertPersianDatetimeToGregorianOrNull(ContractMaxDate)}'";
            }
            return query;
        }

        public static string MakeConstructionPeriodMaxQuery(object value)
        {
            return $" AND GracePeriod<={value}";
        }
        public static string MakeConstructionPeriodMinQuery(object value)
        {
            return $" AND GracePeriod>={value}";
        }

        public static string MakeGracePeriodMaxQuery(object value)
        {
            return $" flc.NumberOfDelayedInstallment>={value}";

        }
        public static string MakeGracePeriodMinQuery(object value)
        {
            return $" and loan.CustomerNumber={value}";
        }


    }
}
