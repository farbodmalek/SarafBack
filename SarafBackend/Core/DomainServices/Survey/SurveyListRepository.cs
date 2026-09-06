using Azure.Core;
using CommonLibrary.Application.DTO.Common;
using CommonLibrary.Core.Domain;
using CommonLibrary.Core.Domain.Dto;
using CommonLibrary.Core.Domain.Dto.Common;
using CommonLibrary.Core.Domain.Entities.Loans;
using CommonLibrary.Core.Domain.Enum;
using CommonLibrary.Core.Domain.Interfaces;
using CommonLibrary.Core.Domain.RepositoryInterfaces;
using CommonLibrary.Infrastructure.Utils.Extensions;
using CommonLibrary.Infrastructure.Utils.SqlSortUtil;
using LoanMonitoringMicroService.Application.DTO.Survey;
using LoanMonitoringMicroService.Core.Domain.Entities;
using LoanMonitoringMicroService.Core.Domain.RepositoryInterfaces;
using LoanMonitoringMicroService.Core.Domain.ViewModel.Survey;
using LoanMonitoringMicroService.Core.DomainServices.Interfaces;
using LoanMonitoringMicroService.Core.DomainServices.Survey.SurveyQueryBuilder;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Stimulsoft.Report.StiRecentConnections;

namespace LoanMonitoringMicroService.Core.DomainServices.Loans
{
    public class SurveyListRepository : QueryParam, ISurveyListRepositoryRead
    {
        private readonly IDapperHandler dapperHandler;
        private readonly ILoanMonitoringDbContext Context;
        private readonly IDateConvertor dateConvertor;
        private readonly LoginUserDTO loginUser;
        public SurveyListRepository(ILoanMonitoringDbContext Context, IDapperHandler _dapperHandler, IDateConvertor _dateConvertor, LoginUserDTO _loginUser)
        {
            this.Context = Context;
            this.dapperHandler = _dapperHandler;
            this.dateConvertor = _dateConvertor;
            this.loginUser = _loginUser;
            this.AddQueryParamMap = new Dictionary<string, AddQueryParam>
            {
                { "Keyword", SurveyWhereSectionBuilder.MakeKeywordQuery },
                { "CustomerNumber", SurveyWhereSectionBuilder.MakeCustomerNumberQuery },
                { "SurveyUserId", SurveyWhereSectionBuilder.MakeSurveyUserQuery },
                { "BranchList", SurveyWhereSectionBuilder.MakeBranchListQuery },
                { "SupervisorID", SurveyWhereSectionBuilder.MakeSupervisorQuery },
                { "SupervisorList", SurveyWhereSectionBuilder.MakeSupervisorsQuery },
                { "ReagentList", SurveyWhereSectionBuilder.MakeReagentQuery },
                { "LoanStatusTypeIdList",SurveyWhereSectionBuilder.MakeLoanStatusTypeIdListQuery},
                { "ContractMinDatefa",value => SurveyWhereSectionBuilder.MakeContractMinDateQuery(value,_dateConvertor)},
                { "ContractMaxDatefa", value =>SurveyWhereSectionBuilder.MakeContractMaxDateQuery(value,_dateConvertor)},
                { "LoanAmountMin",SurveyWhereSectionBuilder.MakeLoanAmountMinQuery},
                { "LoanAmountMax",SurveyWhereSectionBuilder.MakeLoanAmountMaxQuery},
                { "LegalRefrenceStatusId",SurveyWhereSectionBuilder.MakeLegalRefrenceStatusIdQuery},
                { "SurveyMinDateFa", value => SurveyWhereSectionBuilder.MakeSurveyMinDateQuery(value,_dateConvertor) },
                { "SurveyMaxDateFa", value => SurveyWhereSectionBuilder.MakeSurveyMaxDateQuery(value,_dateConvertor)  },
                { "ReferenceMinDateFa", value => SurveyWhereSectionBuilder.MakeReferenceMinDateQuery(value,_dateConvertor) },
                { "ReferenceMaxDateFa", value => SurveyWhereSectionBuilder.MakeReferenceMaxDateQuery(value,_dateConvertor)  },
                { "LoanInstallmentTypeId", SurveyWhereSectionBuilder.MakeLoanInstallmentTypeIdQuery },
                { "LoanMinorTypeList", SurveyWhereSectionBuilder.MakeLoanMinorTypeListQuery },
                { "Death", SurveyWhereSectionBuilder.MakeDeathQuery },
                { "PlanActivationTypeIdList", SurveyWhereSectionBuilder.MakePlanActivationTypeListQuery },
            };
        }

        public async Task<ResultList<SurveyVM>> GetSurveysList(SurveyFilterVM request)
        {
            var whereSection = new StringBuilder();
            var result = new ResultList<SurveyVM>();
            var filterItemList = request.GetFieldProperties();
            foreach (var item in filterItemList)
            {
                if (this.AddQueryParamMap.ContainsKey(item.Key))
                    whereSection.AppendLine(this.AddQueryParamMap[item.Key](item.Value));
            }
            whereSection.AppendLine(" AND loan.NotSupervision=0");

            var sortQuery = SortTranslator
                    .Instance(SortDictionary.SurveyList)
                    .makeSortQuery(request.SortDTO); 
            
            var pagingQuery = $@" {sortQuery} OFFSET {request.Skip} ROWS FETCH NEXT {request.Take} ROWS ONLY ";

            var query = MakeLoanSelectQuery(request) + MakeSurveyTableQuery(request) + makeWhereQuery(request) + whereSection.ToString();

            if (request.SurveyListType == SurveyListType.Referral)
            {
                result =  await makeReferralSurveyQuery(query, pagingQuery, request, true);
                
            }
            else
            {    result.Results = await this.dapperHandler.GetAll<SurveyVM>(query + pagingQuery);
                var count = MakeLoanSelectQuery(request, true) + MakeSurveyTableQuery(request) + makeWhereQuery(request) + whereSection.ToString();
                 result.TotalRows = await this.dapperHandler.Get<int>(MakeLoanSelectQuery(request,true) + MakeSurveyTableQuery(request) + makeWhereQuery(request) + whereSection.ToString());
            }

            
            var loanIds = result.Results.Select(a => a.Id).ToList();
            if (loanIds.Any())
            {
                var data = await this.dapperHandler.GetAll<KeyValueDto<int>>(
                    @$"  select lpn.Id as ID, LoanId as [Key], pn.name as [Value] from survey.LoanPlanNo lpn
                          LEFT JOIN survey.PlanNo pn on lpn.PlanNoId=pn.Id
                          where LoanId in ({loanIds.JoinToString()}) and lpn.IsDeleted = 0
                          order by lpn.Id desc");
                foreach (var loan in result.Results)
                {
                    loan.PlanNoName = data.FirstOrDefault(p => p.Key == loan.Id)?.Value;
                    loan.PlanNoId = data.FirstOrDefault(p => p.Key == loan.Id)?.ID;
                }
            }
           
            return result;
        }
        public async Task<ResultList<SurveyVM>> GetReferenceToMeSurveyList(SurveyFilterVM request)
        {


            var sortQuery = SortTranslator
                    .Instance(SortDictionary.ReferenceToMeSurvey)
                    .makeSortQuery(request.SortDTO);


            var pagingQuery = $@" {sortQuery} OFFSET {request.Skip} ROWS FETCH NEXT {request.Take} ROWS ONLY ";
            string whereSection = $@" where cr.UserId = {this.loginUser.ID} and cr.CartableStatusTypeId = 1 ";
            var query = MakeLoanSelectQuery(request) + MakeSurveyTableQuery(request) + whereSection + pagingQuery;
            var list = await this.dapperHandler.GetAll<SurveyVM>(query);
            var loanIds = list.Select(a => a.Id).ToList();
            if (loanIds.Any())
            {
                var data = await this.dapperHandler.GetAll<KeyValueDto<int>>(
                    @$"  select lpn.Id as ID, LoanId as [Key], pn.name as [Value] from survey.LoanPlanNo lpn
                          LEFT JOIN survey.PlanNo pn on lpn.PlanNoId=pn.Id
                          where LoanId in ({loanIds.JoinToString()}) and lpn.IsDeleted = 0
                          order by lpn.Id desc");
                foreach (var loan in list)
                {
                    loan.PlanNoName = data.FirstOrDefault(p => p.Key == loan.Id)?.Value;
                    loan.PlanNoId = data.FirstOrDefault(p => p.Key == loan.Id)?.ID;
                }
            }
            var cnt = MakeLoanSelectQuery(request, true) + MakeSurveyTableQuery(request) + whereSection;
            var result = new ResultList<SurveyVM>()
            {
                Results = list,
                TotalRows = await this.dapperHandler.Get<int>(MakeLoanSelectQuery(request, true) + MakeSurveyTableQuery(request) + whereSection)
            };
            return result;
        }
        private string MakeLoanSelectQuery(SurveyFilterVM request, bool isCount = false)
        {
            if (isCount)
                return @"SELECT COUNT(distinct loan.Id)
                        ";
            else
                return @"SELECT distinct loan.Id, loan.LoanNumber, lc.BeginDate, it.[Desc] AS InstallmentType 
				            , cs.FullName as CustomerName, cs.CustomerNumber, lrw.LastReferenceDate
				            ,branch.BranchName,branch.SupervisorName, branch.BranchCode,branch.SupervisorCode
                            ,lc.AcceptAmount AS loanAmount,isnull(cs.IsDeath,0) as IsDeath
				            ,acc.InstallmentCount,acc.NumberOfDelayedInstallment AS DelayedInstallmentCount
				            , IIF(lrw.LastSurveyDate IS NULL OR lrw.LastSurveyDate < lrw.LastOutSideSurveyDate, lrw.LastOutSideSurveyDate, lrw.LastSurveyDate) AS LastSurveyDate
				            , lrw.SurveyLevel, lrw.LastSurveyUserId
				            , ls.[Desc] AS LastLoanStatusDesc,lri.LegalReferenceStatusTypeId
                            ,(lrw.OutSideSurveyCount + lrw.SurveyCount) SurveyCount
                            ,(lrw.OutSideSurveyCount + lrw.SurveyCartableCount) CartableCount
                            ,cr.Description
							,cr.IsNotScheduledReferred
                            ,pn.[name] as planName
							,lrw.NotScheduledReferredSurveyCount
                            ";
        }
        private string MakeSurveyTableQuery(SurveyFilterVM request, bool isCount = false)
        {
            var table = new StringBuilder();

            table.Append(@$" FROM core.Loans AS loan
                             inner JOIN core.Customers cs on loan.CustomerId=cs.Id
                             inner JOIN core.FactLoanAcc AS acc on loan.Id = acc.LoanId
                             inner JOIN core.LoanContracts AS lc on loan.Id=lc.LoanId
                             LEFT JOIN core.LoanInstallmentTypes it on lc.LoanInstallmentTypeId=it.Id
			                 LEFT JOIN core.LoanStatusTypes ls on loan.LoanStatusTypeId = ls.Id
			                 inner JOIN core.LoanRedundantView lrw on loan.id=lrw.LoanId
                             LEFT JOIN survey.Cartable as cr on lrw.LastCartableId = cr.Id
                             LEFT JOIN survey.OutsideSurvey as Os on Os.CartableId = cr.Id
							 LEFT JOIN survey.Survey as s on s.CartableId = cr.Id
                             inner JOIN core.Branches AS branch on loan.BranchCode = branch.Id
							 LEFT JOIN Legal.LegalReferenceInfo lri on lri.LoanId = loan.id and lri.IsDeleted = 0
                              left join survey.loanPlanNo as lpn on lpn.loanid = loan.id and lpn.IsDeleted=0 
						     left JOIN survey.planno as pn on pn.id = lpn.plannoid
                            ");

            if (request.SurveyListType == 3 || request.SurveyListType == 1|| request.SurveyListType ==2)
            {
                table.Append(@" inner JOIN survey.AllowedSupervisionLoan ASl on loan.Id=ASl.LoanId
                                 ");
            }
            if (request.SurveyListType == 1)
            {
                table.Append(@"LEFT JOIN SurveyCountByLoan scbl ON scbl.LoanId = loan.Id ");
            }

            if (request.SurveyListType == 4 && request.ReagentList != null)
            {
                table.Append(@" left join core.LoanGuaranties lg on loan.Id = lg.LoanId
                                left join Core.Customers lgc on lg.GuarantorCustomerNumber = lgc.CustomerNumber");
            }

            return table.ToString();
        }
        private string makeWhereQuery(SurveyFilterVM filter)
        {
            var query = @$" Where cs.IsDeath= {(filter.Death == 2 ? '1' : '0')} ";

            if (filter.SurveyListType == SurveyListType.Referral)
                query += makeReferralFilter(filter);
            if (filter.SurveyListType == SurveyListType.Referral && filter.LoanStatusTypeIdList.Count > 0)
                query += makeReferralFilterLoanStatus(filter);
            if (filter.SurveyListType == SurveyListType.Referral && filter.PlanActivationTypeIdList.Count > 0)
                query += makeReferralFilterPlanActivation(filter);

            if (filter.SurveyListType == SurveyListType.ReferralWaiting)
                query += makeReferralWaitingFilter(filter);

            if (filter.SurveyListType == SurveyListType.ReferredAll)
                query += makeReferredAllFilter(filter);
            if (filter.SurveyListType == SurveyListType.ReferredAll && filter.LoanStatusTypeIdList.Count > 0)
                query += makeReferredAllFilterLoanStatus(filter);
            if (filter.SurveyListType == SurveyListType.ReferredAll && filter.PlanActivationTypeIdList.Count > 0)
                query += makeReferredAllFilterPlanActivation(filter);


            if (filter.SurveyListType == SurveyListType.Supervised)
                query += makeSupervisedFilter(filter);
            if (filter.SurveyListType == SurveyListType.Supervised && filter.LoanStatusTypeIdList.Count > 0)
                query += makeSupervisedFilterLoanStatus(filter);
            if (filter.SurveyListType == SurveyListType.Supervised && filter.PlanActivationTypeIdList.Count > 0)
                query += makeSupervisedFilterPlanActivation(filter);

            if (filter.SurveyListType == SurveyListType.RequiredToSurvey)
                query += makeRequiredToSurveyFilter(filter);
            query += makeOutSideSurveyDateWhere(filter);
            return query;
        }
        private string makeOutSideSurveyDateWhere(SurveyFilterVM filter)
        {
            var querybuilder = new StringBuilder();
            if (!string.IsNullOrEmpty(filter.OutSideSurveyMaxDateFa) || !string.IsNullOrEmpty(filter.OutSideSurveyMinDateFa))
            {
                querybuilder.Append(" AND (SELECT count(1) FROM survey.OutsideSurvey os WHERE os.IsDeleted=0 AND os.LoanId=loan.id");
                querybuilder.Append(!string.IsNullOrEmpty(filter.OutSideSurveyMaxDateFa) ? $" AND os.SurveyDate >= '{dateConvertor.ConvertPersianDatetimeToGregorianOrNull(filter.OutSideSurveyMaxDateFa)}'" : "");
                querybuilder.Append(!string.IsNullOrEmpty(filter.OutSideSurveyMinDateFa) ? $" AND os.SurveyDate <= '{dateConvertor.ConvertPersianDatetimeToGregorianOrNull(filter.OutSideSurveyMinDateFa)}'" : "");
                //querybuilder.Append(filter.SurveyUserId.HasValue ? $" AND  os.CreatedBy = ({filter.SurveyUserId.Value}) " : "");
                querybuilder.Append(")>0");
            }
            return querybuilder.ToString();
        }
        private string makeReferralFilter(SurveyFilterVM filter)
        {
            //      return $@"  AND (loan.NotSupervision IS Null OR  loan.NotSupervision = 0 )
            //         AND loan.LoanStatusTypeId IN ('3','4','5','F')  
            //AND loan.IsLegal <> 1
            //         AND ((cr.Id is null AND lrw.LastOutSideSurveyDate is null)
            //              OR cr.CartableStatusTypeId = 3  
            //              OR ( (cr.CartableStatusTypeId = 2  or ( cr.CartableStatusTypeId is null and loan.SurveyLevel+lrw.OutSideSurveyCount>0   ))
            //     AND   DATEADD(MONTH,CASE loan.SurveyLevel+lrw.OutSideSurveyCount 
            //                          WHEN 1 THEN 3
            //                          WHEN 2 THEN 6
            //                          WHEN 3 THEN 8
            //                          WHEN 4 THEN 10
            //      	                 WHEN 5 THEN 12
            //                          END			   
            //                   , IIF(lrw.LastSurveyDate IS NULL OR lrw.LastSurveyDate < lrw.LastOutSideSurveyDate, lrw.LastOutSideSurveyDate, lrw.LastSurveyDate))<=GETDATE())
            //    ) 		  
            //AND lc.AcceptAmount > 200000000
            //AND  DATEADD(YEAR,3, lc.BeginDate) > GETDATE() 
            //                  ";

            return $@"AND (loan.NotSupervision IS NULL OR loan.NotSupervision = 0)
        AND loan.LoanStatusTypeId IN ('3','4','5','F')
        AND loan.IsLegal <> 1
        AND lc.AcceptAmount > 1000000000
        AND DATEADD(YEAR, 3, lc.BeginDate) > GETDATE()
        AND loan.NotSupervision = 0
        AND (
				(cr.CartableStatusTypeId = 1 AND ISNULL(cr.IsNotScheduledReferred,0) = 1)
				OR
				(cr.Id IS NULL AND lrw.LastOutSideSurveyDate IS NULL)
				OR
				(cr.CartableStatusTypeId = 3)
				OR
				(cr.CartableStatusTypeId = 2 OR (cr.CartableStatusTypeId IS NULL AND loan.SurveyLevel + lrw.OutSideSurveyCount > 0))
			) 
		
		And(
		    
			(lc.AcceptAmount <= 4000000000 AND   (ISNULL(scbl.SurveyCount, 0) = 0)
										OR
			(lc.AcceptAmount > 4000000000 AND lc.AcceptAmount <= 20000000000		
															     --1010000000
															     
															    

			 And
			  (
               (CASE
                     WHEN lrw.LastSurveyDate IS NULL
						OR lrw.LastSurveyDate < lrw.LastOutSideSurveyDate
						 THEN lrw.LastOutSideSurveyDate
                      ELSE lrw.LastSurveyDate
                  END
				) >= @BaseDate
              AND
                (ISNULL(scbl.SurveyCount, 0) < 2
                 AND 
					DATEADD(MONTH, 6,
						CASE
                            WHEN lrw.LastSurveyDate IS NULL
                                 OR lrw.LastSurveyDate < lrw.LastOutSideSurveyDate
                            THEN lrw.LastOutSideSurveyDate
                            ELSE lrw.LastSurveyDate
                        END) < GETDATE()
                  AND dbo.CheckLastSurveyYear
						(
                            CASE
                                WHEN lrw.LastSurveyDate IS NULL
                                     OR lrw.LastSurveyDate < lrw.LastOutSideSurveyDate
                                THEN lrw.LastOutSideSurveyDate
                                ELSE lrw.LastSurveyDate
                            END
                        ) = 1)
                )
                 OR
                    (
                        (
                            CASE
                                WHEN lrw.LastSurveyDate IS NULL
                                     OR lrw.LastSurveyDate < lrw.LastOutSideSurveyDate
                                THEN lrw.LastOutSideSurveyDate
                                ELSE lrw.LastSurveyDate
                            END
                        ) < @BaseDate
                    )
                )
	       )
		   OR
		   ( 

        (COALESCE(lrw.LastSurveyDate, lrw.LastOutSideSurveyDate) IS  NULL)
            Or
            lc.AcceptAmount >= 20000000000
								
			 And
			  (
            (COALESCE(lrw.LastSurveyDate, lrw.LastOutSideSurveyDate) IS  NULL)
          Or
               (CASE
                     WHEN lrw.LastSurveyDate IS NULL
						OR lrw.LastSurveyDate < lrw.LastOutSideSurveyDate
						 THEN lrw.LastOutSideSurveyDate
                      ELSE lrw.LastSurveyDate
                  END
				) >= @BaseDate
              AND
                (ISNULL(scbl.SurveyCount, 0) < 3
                 AND 
					DATEADD(MONTH, 6,
						CASE
                            WHEN lrw.LastSurveyDate IS NULL
                                 OR lrw.LastSurveyDate < lrw.LastOutSideSurveyDate
                            THEN lrw.LastOutSideSurveyDate
                            ELSE lrw.LastSurveyDate
                        END) < GETDATE()
                  AND dbo.CheckLastSurveyYear
						(
                            CASE
                                WHEN lrw.LastSurveyDate IS NULL
                                     OR lrw.LastSurveyDate < lrw.LastOutSideSurveyDate
                                THEN lrw.LastOutSideSurveyDate
                                ELSE lrw.LastSurveyDate
                            END
                        ) = 1)
                )
                 OR
                    (
                        (
                            CASE
                                WHEN lrw.LastSurveyDate IS NULL
                                     OR lrw.LastSurveyDate < lrw.LastOutSideSurveyDate
                                THEN lrw.LastOutSideSurveyDate
                                ELSE lrw.LastSurveyDate
                            END
                        ) < @BaseDate
                    )
                )
	       )
                                  

                                  ";


            //    return $@" AND loan.LoanStatusTypeId IN ('3','4','5','F')  
            //AND loan.IsLegal <> 1
            //AND(
            //(cr.CartableStatusTypeId = 1 AND ISNULL(cr.IsNotScheduledReferred,0) = 1 ) 
            //or 
            //(cr.Id is null AND lrw.LastOutSideSurveyDate is null)
            //OR 
            //(cr.CartableStatusTypeId = 3 )
            //OR 
            //(cr.CartableStatusTypeId = 2  or ( cr.CartableStatusTypeId is null and loan.SurveyLevel+lrw.OutSideSurveyCount>0 )))

            //AND
            //((lc.AcceptAmount <= 4000000000 AND ((lrw.SurveyCount + lrw.OutSideSurveyCount)-lrw.NotScheduledReferredSurveyCount) = 0) OR (lc.AcceptAmount > 4000000000 
            //			   AND lc.AcceptAmount <= 20000000000
            //		AND ((lrw.SurveyCount + lrw.OutSideSurveyCount)-lrw.NotScheduledReferredSurveyCount) < 2
            //		AND DATEADD(MONTH,6,
            //			CASE
            //				WHEN lrw.LastSurveyDate IS NULL
            //					 OR lrw.LastSurveyDate < lrw.LastOutSideSurveyDate
            //				THEN lrw.LastOutSideSurveyDate
            //				ELSE lrw.LastSurveyDate
            //			END) > GETDATE()
            //		AND dbo.CheckLastSurveyYear(
            //			CASE
            //				WHEN lrw.LastSurveyDate IS NULL
            //					 OR lrw.LastSurveyDate < lrw.LastOutSideSurveyDate
            //				THEN lrw.LastOutSideSurveyDate
            //				ELSE lrw.LastSurveyDate
            //			END) = 1
            //	   )

            //	OR (lc.AcceptAmount > 20000000000
            //		AND ((lrw.SurveyCount + lrw.OutSideSurveyCount)-lrw.NotScheduledReferredSurveyCount) < 3
            //		AND DATEADD(MONTH,6,
            //			CASE
            //				WHEN lrw.LastSurveyDate IS NULL
            //					 OR lrw.LastSurveyDate < lrw.LastOutSideSurveyDate
            //				THEN lrw.LastOutSideSurveyDate
            //				ELSE lrw.LastSurveyDate
            //			END) > GETDATE()
            //		AND dbo.CheckLastSurveyYear(
            //			CASE
            //				WHEN lrw.LastSurveyDate IS NULL
            //					 OR lrw.LastSurveyDate < lrw.LastOutSideSurveyDate
            //				THEN lrw.LastOutSideSurveyDate
            //				ELSE lrw.LastSurveyDate
            //			END) = 1
            //	   )
            //)





            //       AND lc.AcceptAmount > 1000000000
            //        AND DATEADD(YEAR,3, lc.BeginDate) > GETDATE()
            //        AND loan.NotSupervision = 0   
            //                ";
        }
        private string makeReferralFilterLoanStatus(SurveyFilterVM filter)
        {
            return $@"{getByLevel()} AND loan.LoanStatusTypeId IN ('3','4','5','F')  
                        AND (cr.CartableStatusTypeId<>1 OR lrw.LastCartableId IS NULL) 
                        AND (lrw.IsHeaderSupervised IS NULL OR lrw.IsHeaderSupervised=0)
                         {getYearlyWhere()}";
        }
        private string makeReferralFilterPlanActivation(SurveyFilterVM filter)
        {
            for (int i = 0; i < filter.PlanActivationTypeIdList.Count; i++)
            {
                if (filter.PlanActivationTypeIdList[i] == 1)//در activation ایدی فعال 1 هست
                {
                    LoanStatusType loanStatus = new LoanStatusType();
                    loanStatus.Id = 3.ToString(); //ولی در loanstatus ایدی فعال 3
                    filter.LoanStatusTypeIdList.Add(loanStatus.Id);
                    //و چون نمیخوام کوئری عوض کنم activation رو ریختن روی loanstatus که کوئری کار خودش بکنه 
                    //و فعال هارو بیاره
                }
                if (filter.PlanActivationTypeIdList[i] == 2)//در activation ایدی فعال 1 هست
                {

                    LoanStatusType loanStatus = new LoanStatusType();
                    loanStatus.Id = 7.ToString(); //ولی در loanstatus ایدی حذف شده 7
                    filter.LoanStatusTypeIdList.Add(loanStatus.Id);
                    //و چون نمیخوام کوئری عوض کنم activation رو ریختن روی loanstatus که کوئری کار خودش بکنه 
                    //و فعال هارو بیاره
                }
                if (filter.PlanActivationTypeIdList[i] == 3)//در activation ایدی فعال 1 هست
                {
                    LoanStatusType loanStatus = new LoanStatusType();
                    loanStatus.Id = 1.ToString(); //ولی در loanstatus ایدی فعال 3
                    filter.LoanStatusTypeIdList.Add(loanStatus.Id);
                    //و چون نمیخوام کوئری عوض کنم activation رو ریختن روی loanstatus که کوئری کار خودش بکنه 
                    //و فعال هارو بیاره
                }
            }
            return $@"{getByLevel()} AND loan.LoanStatusTypeId IN ('3','4','5','F')  
                        AND (cr.CartableStatusTypeId<>1 OR lrw.LastCartableId IS NULL) 
                        AND (lrw.IsHeaderSupervised IS NULL OR lrw.IsHeaderSupervised=0)
                        {getYearlyWhere()}";
        }
        private string makeReferredAllFilterPlanActivation(SurveyFilterVM filter)
        {
            for (int i = 0; i < filter.PlanActivationTypeIdList.Count; i++)
            {
                if (filter.PlanActivationTypeIdList[i] == 1)//در activation ایدی فعال 1 هست
                {
                    LoanStatusType loanStatus = new LoanStatusType();
                    loanStatus.Id = 3.ToString(); //ولی در loanstatus ایدی فعال 3
                    filter.LoanStatusTypeIdList.Add(loanStatus.Id);
                    //و چون نمیخوام کوئری عوض کنم activation رو ریختن روی loanstatus که کوئری کار خودش بکنه 
                    //و فعال هارو بیاره
                }
                if (filter.PlanActivationTypeIdList[i] == 2)//در activation ایدی فعال 1 هست
                {
                    LoanStatusType loanStatus = new LoanStatusType();
                    loanStatus.Id = 7.ToString(); //ولی در loanstatus ایدی حذف شده 7
                    filter.LoanStatusTypeIdList.Add(loanStatus.Id);
                    //و چون نمیخوام کوئری عوض کنم activation رو ریختن روی loanstatus که کوئری کار خودش بکنه 
                    //و فعال هارو بیاره
                }
                if (filter.PlanActivationTypeIdList[i] == 3)//در activation ایدی فعال 1 هست
                {
                    LoanStatusType loanStatus = new LoanStatusType();
                    loanStatus.Id = 1.ToString(); //ولی در loanstatus ایدی فعال 3
                    filter.LoanStatusTypeIdList.Add(loanStatus.Id);
                    //و چون نمیخوام کوئری عوض کنم activation رو ریختن روی loanstatus که کوئری کار خودش بکنه 
                    //و فعال هارو بیاره
                }
            }
            string query = $" AND cr.CartableStatusTypeId=1  ";
            return query;
        }
        private string makeSupervisedFilterPlanActivation(SurveyFilterVM filter)
        {
            for (int i = 0; i < filter.PlanActivationTypeIdList.Count; i++)
            {
                if (filter.PlanActivationTypeIdList[i] == 1)
                {
                    LoanStatusType loanStatus = new LoanStatusType();
                    loanStatus.Id = 3.ToString();
                    filter.LoanStatusTypeIdList.Add(loanStatus.Id);

                }
                if (filter.PlanActivationTypeIdList[i] == 2)
                {
                    LoanStatusType loanStatus = new LoanStatusType();
                    loanStatus.Id = 7.ToString();
                    filter.LoanStatusTypeIdList.Add(loanStatus.Id);

                }
                if (filter.PlanActivationTypeIdList[i] == 3)
                {
                    LoanStatusType loanStatus = new LoanStatusType();
                    loanStatus.Id = 3.ToString();
                    filter.LoanStatusTypeIdList.Add(loanStatus.Id);
                }
            }
            string query = "";
            if (filter.IsHeaderSurveyedLoan)
                query += makeHeaderSupervisedFilter(filter);
            else
            {
                query = @"AND (cr.CartableStatusTypeId in(2,4) OR lrw.LastOutSideSurveyDate IS not null) ";

            }
            return $@"{query} {makePermission()}";
        }
        private string makeReferralWaitingFilter(SurveyFilterVM filter)
        {
            //      return $@"  AND (loan.NotSupervision IS Null OR  loan.NotSupervision = 0 )
            //                  AND loan.LoanStatusTypeId ='E' 
            //       AND (lrw.IsHeaderSupervised IS NULL OR lrw.IsHeaderSupervised=0)
            //	      AND lc.AcceptAmount > 1000000000
            //                AND (cr.Id is null OR cr.CartableStatusTypeId = 3
            //OR 
            // ( (  ISNULL(cr.IsNotScheduledReferred,0) = 1 or cr.CartableStatusTypeId = 2 AND  
            //    (CONVERT(date,(select top(1) pb.PayDate from Core.LoanPayBanks pb where pb.LoanId = loan.Id order by Id Desc )) > 
            // CONVERT(date, IIF(lrw.LastSurveyDate IS NULL OR lrw.LastSurveyDate < lrw.LastOutSideSurveyDate, lrw.LastOutSideSurveyDate, lrw.LastSurveyDate) ))
            //  OR 
            //  (CONVERT(date,(select top(1) pb.PayDate from Core.LoanPayBanks pb where pb.LoanId = loan.Id order by Id Desc )) 
            //  < CONVERT(date,  IIF(lrw.LastSurveyDate IS NULL OR lrw.LastSurveyDate < lrw.LastOutSideSurveyDate, lrw.LastOutSideSurveyDate, lrw.LastSurveyDate))
            //  and  
            //   GETDATE()  >
            //  DATEADD(month,3, IIF(lrw.LastSurveyDate IS NULL OR lrw.LastSurveyDate < lrw.LastOutSideSurveyDate, lrw.LastOutSideSurveyDate, lrw.LastSurveyDate))))))






            //  AND loan.NotSupervision=0";
            return $@"  AND (loan.NotSupervision IS Null OR  loan.NotSupervision = 0 )
							 AND loan.LoanStatusTypeId ='E' 
			                 AND (lrw.IsHeaderSupervised IS NULL OR lrw.IsHeaderSupervised=0)
			   	             AND lc.AcceptAmount > 1000000000
             AND (cr.Id is null OR cr.CartableStatusTypeId = 3
					OR 
					(ISNULL(cr.IsNotScheduledReferred,0) = 1 
					or
					(cr.CartableStatusTypeId = 2 
					AND  
					(CONVERT(date,(select top(1) pb.PayDate from Core.LoanPayBanks pb where pb.LoanId = loan.Id order by Id Desc )) > 
					CONVERT(date, IIF(lrw.LastSurveyDate IS NULL OR lrw.LastSurveyDate < lrw.LastOutSideSurveyDate, lrw.LastOutSideSurveyDate, lrw.LastSurveyDate)))
					OR 
					(CONVERT(date,(select top(1) pb.PayDate from Core.LoanPayBanks pb where pb.LoanId = loan.Id order by Id Desc )) 
					< CONVERT(date,  IIF(lrw.LastSurveyDate IS NULL OR lrw.LastSurveyDate < lrw.LastOutSideSurveyDate, lrw.LastOutSideSurveyDate, lrw.LastSurveyDate))
					and 
					GETDATE()>DATEADD(month,2, IIF(lrw.LastSurveyDate IS NULL OR lrw.LastSurveyDate < lrw.LastOutSideSurveyDate, lrw.LastOutSideSurveyDate, lrw.LastSurveyDate)))
					)))
                       
                AND loan.NotSupervision=0";
        }
        private string makeReferredAllFilter(SurveyFilterVM filter)
        {
            string query = $" AND cr.CartableStatusTypeId = 1  ";
            return query;
        }
        private string makeReferredAllFilterLoanStatus(SurveyFilterVM filter)
        {
            string query = $" AND cr.CartableStatusTypeId=1  ";
            return query;
        }
        private string makeSupervisedFilter(SurveyFilterVM filter)
        {
            string query = "";
            if (filter.IsHeaderSurveyedLoan)
                query += makeHeaderSupervisedFilter(filter);
            else if (filter.ReagentList != null)
            {
                query = @"
                and (2=2) ";
                return $@"{query} {makePermission()}";
            }
            else
            {
                query = @"
                and ( (cr.CartableStatusTypeId in(2,4) 
                 OR lrw.LastOutSideSurveyDate IS not null)) ";

            }
            return $@"{query} {makePermission()}";
        }
        private string makeSupervisedFilterLoanStatus(SurveyFilterVM filter)
        {
            string query = "";
            if (filter.IsHeaderSurveyedLoan)
                query += makeHeaderSupervisedFilter(filter);
            else
            {
                query = @"AND (cr.CartableStatusTypeId in(2,4) OR lrw.LastOutSideSurveyDate IS not null) ";

            }
            return $@"{query}{makePermission()}";
        }
        private string makeHeaderSupervisedFilter(SurveyFilterVM filter)
        {
            string query = $" AND (lrw.IsHeaderSupervised IS NULL OR lrw.IsHeaderSupervised=1) ";
            return $@"{query} {makePermission()}";
        }
        private string makeRequiredToSurveyFilter(SurveyFilterVM filter)
        {
            string query = $" AND loan.LoanStatusTypeId IN ('3','4','5','F','E') AND cr.CartableStatusTypeId in(1,3,2,4) ";
            return query;
        }
        private string getYearlyWhere()
        {
            return @" 	AND (lc.BeginDate > '2021-03-21 00:00:00.000'
					    AND lc.AcceptAmount > 200000000)";
        }
        private string makePermission()
        {
            string serPermission = "";
            //if (currentUserRoles.Any(a =>
            //a == UserRoleName.CityExpert ||
            //   a == UserRoleName.WageWorker ||
            //   a == UserRoleName.MonitoringWageWorker ||
            //   a == UserRoleName.SoldierWorker))
            //    serPermission = serPermission + " and " + this.DataPermissionCondition("loan.BranchCode", null, "loan.LoanMinorTypeId", "Loan.Id", "branch.SupervisorCode", "branch.RelatedSupervisors", "Loan.RegisterDate");
            //else
            //    serPermission = serPermission + " and " + this.DataPermissionCondition("loan.BranchCode", null, "loan.LoanMinorTypeId", "Loan.Id", "branch.SupervisorCode", "branch.RelatedSupervisors", "Loan.RegisterDate");
            return serPermission;
        }
        private string getByLevel()
        {
            return @" AND (			   
			   (lrw.SurveyLevel is null OR			   
			   DATEADD(day,CASE loan.SurveyLevel+lrw.OutSideSurveyCount 
                WHEN 0 THEN 45
			    WHEN 1 THEN 180
			    WHEN 2 THEN 240
			    WHEN 3 THEN 300
			    WHEN 4 THEN 360
			   ELSE 360
			   END			   
			   ,loan.LastSurveyDate)<=GETDATE())
			   AND 
			   (lrw.LastOutSideSurveyDate is null OR 
			     DATEADD(day,CASE loan.SurveyLevel+lrw.OutSideSurveyCount 
                WHEN 0 THEN 45
			    WHEN 1 THEN 180
			    WHEN 2 THEN 240
			    WHEN 3 THEN 300
			    WHEN 4 THEN 360
			   ELSE 360
			   END			   
			   ,lrw.LastOutSideSurveyDate)<=GETDATE()))";
        }

        public async Task<ResultList<SurveyReferenceBaseInfoVM>> GetSurveyReferenceBaseInfoList(string? KeyWord, List<int>? ids, int? surveyReferenceBaseInfoId)
        {
            var query = from a in Context.SurveyReferenceBaseInfos
                        join b in Context.LoanMinorTypes on a.LoanMinorTypeId equals b.Id
                        where !a.IsDeleted
                        select new { a, b };

            // فیلتر لیست ids
            if (ids != null && ids.Any())
            {
                query = query.Where(x => ids.Contains(x.a.LoanMinorTypeId));
            }
             if (surveyReferenceBaseInfoId !=null )
            { 
                query = query.Where(x => x.a.Id==surveyReferenceBaseInfoId);
            }

            // فیلتر کلیدواژه روی توضیحات نوع تسهیلات یا هر فیلد دیگری
            if (!string.IsNullOrWhiteSpace(KeyWord))
            {
                query = query.Where(x => x.b.Desc.Contains(KeyWord));
            }

            var minorType = await query
                .Select(x => new SurveyReferenceBaseInfoVM
                {
                    Id = x.a.Id,
                    LoanMinorTypeDesc = x.b.Desc,
                    LoanMinorTypeId = x.a.LoanMinorTypeId,
                    IsActive = x.a.IsActive,
                    AllowedFirstTimeSupervisionRoles = Context.AllowedFirstTimeSupervisionRoles
                        .Where(r => r.SurveyReferenceBaseInfoId == x.a.Id && !r.IsDeleted  ).ToList(),

                    SurveyReferenceLoanAmounts = Context.SurveyReferenceLoanAmounts
                        .Where(r => r.SurveyReferenceBaseInfoId == x.a.Id && !r.IsDeleted).ToList(),

                    SurveyReferenceContractDates = Context.SurveyReferenceContractDates
                        .Where(r => r.SurveyReferenceBaseInfoId == x.a.Id && !r.IsDeleted ).ToList(),

                    SurveyReferenceReagents = (from r in Context.SurveyReferenceReagents
                                               join c in Context.Customers on r.CustomerId equals c.Id
                                               where r.SurveyReferenceBaseInfoId == x.a.Id && !r.IsDeleted
                                               select new SurveyReferenceReagentVM
                                               {
                                                   Id = r.Id,   
                                                   CustomerId = r.CustomerId,
                                                   IsActive = r.IsActive,
                                                   CustomerName = c.FullName,
                                                   SurveyReferenceBaseInfoId = r.SurveyReferenceBaseInfoId
                                               }).ToList(),
                })
                .OrderBy(a => a.LoanMinorTypeId)
                .ToListAsync();

            return new ResultList<SurveyReferenceBaseInfoVM>
            {
                Results = minorType,
                TotalRows = minorType.Count()
            };
        }



        public async Task<ResultList<SurveyReferenceMaxCountVM>> GetSurveyMaxCountCondition(BaseFilter request)
        {
            var pagingQuery = $@" ORDER BY Id desc OFFSET {request.Skip} ROWS FETCH NEXT {request.Take} ROWS ONLY ";
            string query = $@" SELECT *FROM [survey].[SurveyMaxCount] where isdeleted=0";
            var finalquery = query + pagingQuery;
            var list = await this.dapperHandler.GetAll<SurveyReferenceMaxCountVM>(finalquery);
            var result = new ResultList<SurveyReferenceMaxCountVM>()
            {
                Results = list,
                TotalRows = await this.dapperHandler.Get<int>(finalquery)
            };
            return result;
        }

        private async Task<string> GenerateSP()
        {
            var query = await makeSurveyReferenceBaseInfoQuery();
            return $@"   ALTER PROCEDURE [dbo].[SP_OperationSetAllowedSupervisionLoan]	
                         AS
                         BEGIN
                         TRUNCATE TABLE survey.AllowedSupervisionLoan
                         INSERT INTO survey.AllowedSupervisionLoan
                         {query}
                            AND loan.Id NOT IN (					  
						          SELECT [LoanId]
						          FROM [survey].[ExceptedSurveyLoan])	
                            END";
        }
        public async Task<ResultObject<int>> FillAllowedSupervisionLoan()
        {
            var res = new ResultObject<int>();
            var querySp = await GenerateSP();
            var res1 = await this.dapperHandler.Get<int>(querySp);
            //await this.dapperHandler.ExecuteAsync(querySp,null);
            await this.dapperHandler.Get<int>("EXEC [dbo].[SP_OperationSetAllowedSupervisionLoan]");
            return res;
        }


        private async Task<string> makeSurveyReferenceBaseInfoQuery()
        {
            var queryStr = @"SELECT DISTINCT loan.Id as loanId
                     FROM core.Loans as loan
                     JOIN core.LoanContracts AS lc on loan.Id = lc.LoanId
                LEFT JOIN core.LoanGuaranties as lg on loan.Id = lg.LoanId and lg.RelationTypeId = 3
                     WHERE 1 = 1 AND(";
            var baseMinorTypeCodes = await Context.SurveyReferenceBaseInfos.Where(c => !c.IsDeleted).Select(c => c.LoanMinorTypeId).ToListAsync();
            var reagent = await Context.SurveyReferenceReagents.Where(c => !c.IsDeleted).ToListAsync();
            var registerdate = await Context.SurveyReferenceContractDates.Where(c => !c.IsDeleted).ToListAsync();
            var amount = await Context.SurveyReferenceLoanAmounts.Where(c => !c.IsDeleted).ToListAsync();

            foreach (var item in baseMinorTypeCodes)
            {
                if (registerdate.Any(c => c.LoanMinorTypeId == item) ||
                    reagent.Any(c => c.LoanMinorTypeId == item) ||
                     amount.Any(c => c.LoanMinorTypeId == item))
                {
                    queryStr += $@"OR (loan.LoanMinorTypeId={item}";
                    if (reagent.Any(c => c.LoanMinorTypeId == item))
                    {
                        var activeReagent = reagent.Where(c => c.IsActive && c.LoanMinorTypeId == item).Select(c => c.CustomerId);
                        var deActiveReagent = reagent.Where(c => !c.IsActive && c.LoanMinorTypeId == item).Select(c => c.CustomerId);
                        queryStr +=
                            activeReagent.Count() > 0 ?
                            $@" AND ISNULL(lg.GuarantorCustomerId,0) IN ({string.Join(',', activeReagent)})" : "";
                        queryStr +=
                              deActiveReagent.Count() > 0 ?
                              $@" AND ISNULL(lg.GuarantorCustomerId,0) NOT IN ({string.Join(',', deActiveReagent)})" : "";
                    }
                    if (registerdate.Any(c => c.LoanMinorTypeId == item))
                    {
                        registerdate.Where(c => c.IsActive && c.LoanMinorTypeId == item).ToList().ForEach(c =>
                        {
                            queryStr += c.BeginDate.HasValue ? $" and loan.RegisterDate >= '{c.BeginDate}'" : "";
                            queryStr += c.EndDate.HasValue ? $" and loan.RegisterDate <= '{c.EndDate}'" : "";
                        });
                    }
                    if (amount.Any(c => c.LoanMinorTypeId == item))
                    {
                        amount.Where(c => c.IsActive && c.LoanMinorTypeId == item).ToList().ForEach(c =>
                        {
                            queryStr += c.AmountMin.HasValue ? $" and lc.AcceptAmount > '{c.AmountMin}'" : "";
                            queryStr += c.AmountMax.HasValue ? $" and lc.AcceptAmount < '{c.AmountMax}'" : "";
                        });
                    }
                    queryStr += ") ";
                }
                queryStr += @"
                            ";
            }
            queryStr = queryStr.Replace("AND(OR", "AND(");
            queryStr += ")";
            return queryStr;
        }


        public async Task<ResultList<SurveyVM>> makeReferralSurveyQuery(string query,string Paging, SurveyFilterVM request, bool isCount)
        {
            var result = new ResultList<SurveyVM>();

            var  queryStr = $@"DECLARE @BaseDate DATETIME = '2026-01-29'
                                ;WITH SurveyCountByLoan AS
                                (
                                   SELECT
                                        LoanId,
                                        COUNT(LoanId) as SurveyCount 
                                   FROM [survey].[Cartable]
                                 WHERE CartableStatusTypeId=2 AND ReferenceDate>=@BaseDate AND ISNULL(IsNotScheduledReferred,0)=0

                                   GROUP BY LoanId
                                ),
                                Main AS
                                ({query} )
                                ";
            result.Results = await this.dapperHandler.GetAll<SurveyVM>(queryStr + $"SELECT * FROM Main"+Paging);
            result.TotalRows = await this.dapperHandler.Get<int>(queryStr+$"SELECT COUNT(distinct id) FROM Main;");



            return result;
        }
    }
}
