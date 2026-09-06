using CommonLibrary.Application.DTO.Common;
using CommonLibrary.Core.Domain;
using CommonLibrary.Core.Domain.Dto.Common;
using CommonLibrary.Core.Domain.Interfaces;
using CommonLibrary.Core.Domain.RepositoryInterfaces;
using CommonLibrary.Infrastructure.Utils.Extensions;
using CommonLibrary.Infrastructure.Utils.SqlSortUtil;
using LoanMonitoringMicroService.Application.DTO.Pwa;
using LoanMonitoringMicroService.Applications.AppService.ServiceDto.Surveys;
using LoanMonitoringMicroService.Core.Domain.ViewModel.PlanNo;
using LoanMonitoringMicroService.Domains.Core.Supervision.Entities.Dao;
using System.Collections.Generic;
using System.Text;

namespace LoanMonitoringMicroService.Core.DomainServices.PlanNo
{
    public class PlanNoRepositoryRead : QueryParam, IPlanNoRepositoryRead
    {
        private readonly IDapperHandler dapperHandler;
        private readonly IDateConvertor dateConvertor;

        public PlanNoRepositoryRead(IDapperHandler dapperHandler, IDateConvertor _dateConvertor)
        {
            this.dapperHandler = dapperHandler;
            this.dateConvertor = _dateConvertor;
            this.AddQueryParamMap = new Dictionary<string, AddQueryParam>
            {
                { "StatusID",this.MakeStatusQuery},
                { "CustomerNumber", this.MakeCustomerNumberQuery },
                { "BranchList", this.MakeBranchListQuery },
                { "SupervisorList", this.MakeSupervisorsQuery },
                { "SurveyUsers", this.MakeSurveyUsersQuery },
                { "SurveyMinDateFa", this.MakeSurveyMinDateQuery },
                { "SurveyMaxDateFa", this.MakeSurveyMaxDateQuery  },
                { "PlanNoId", this.MakePlanNoIdQuery  },
                { "ConfirmUserId", this.MakeConfirmUserIdQuery  },

            };
        }
        public async Task<ResultList<PlanNoVM>> GetPlanNoList(PlanNoFilterDto request)
        {
            
            var query = @"SELECT *
                  FROM [survey].[PlanNo] as p 
                    ";
            var whereSection = "where p.IsDeleted = 0 ";
            if (!string.IsNullOrEmpty(request.PlanNoName))
                whereSection += $" and p.Name like  N'%{request.PlanNoName}%'";
            if (request.Id != null)
                whereSection += $" and p.Id  = '{request.Id}'";

            string pagingQuery = string.Format(" ORDER BY p.Id   OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", request.Skip, request.Take);

            var finalQuery = query + whereSection + pagingQuery;
            var list = await this.dapperHandler.GetAll<PlanNoVM>(finalQuery);
            var countQuery = new StringBuilder("SELECT COUNT(*) FROM [survey].[PlanNo] AS p " + whereSection);
            var result = new ResultList<PlanNoVM>()
            {
                Results = list,
                TotalRows = await this.dapperHandler.Get<int>(countQuery.ToString())
            };
            return result;
        }

        #region PlanNo History

        public async Task<ResultList<PlanNoHistoryVM>> GetPlanNoHistoryRequestList(PlanNoHistoryFilterVM request)
        {
            var result = new ResultList<PlanNoHistoryVM>();
            var query = @" FROM [survey].[PlanNoHistory] a
                            Left join survey.PlanNoStatus pns on a.PlanNoStatusId = pns.Id
                             INNER JOIN [Core].Loans l on a.LoanId= l.Id
							 LEFT JOIN core.LoanRedundantView lrw on l.id=lrw.LoanId
							 inner Join core.LoanContracts as lcc on l.id = lcc.LoanId
                             inner Join Core.Customers as cust on l.CustomerId = cust.Id
                             Inner Join Core.Branches as branch on l.BranchCode = branch.BranchCode
                             Inner JOIN survey.LoanPlanNo lpn on a.LoanId = lpn.LoanId 
                             Inner JOIN [survey].[LoanPlan] lp on a.LoanId = lp.LoanId
                              ";
            if (request.PlanNoId.HasValue)
                query += @" LEFT JOIN survey.LoanPlanNo lpn on l.Id = lpn.LoanId
                             LEFT JOIN survey.PlanNo pn on lpn.PlanNoId = pn.Id and lpn.IsDeleted = 0
                            ";
            var whereSection = new StringBuilder();
            whereSection.AppendLine("where 1 = 1 and a.CartableId is not null  ");
            var filterItemList = request.GetFieldProperties();
            foreach (var item in filterItemList)
            {
                if (this.AddQueryParamMap.ContainsKey(item.Key))
                    whereSection.AppendLine(this.AddQueryParamMap[item.Key](item.Value));
            }

            var sortQuery = SortTranslator
                    .Instance(SortDictionary.RequestPlanNoList)
                    .makeSortQuery(request.SortDTO);
            string pagingQuery = $@"
                                  {sortQuery} OFFSET {request.Skip} ROWS FETCH NEXT {request.Take} ROWS ONLY";
            var finalQuery = MakePlanoHistorySelectSection() + query + whereSection + pagingQuery;
            var list = await dapperHandler.GetAll<PlanNoHistoryVM>(finalQuery);
            var loanIds = list.Select(a => a.LoanId).ToList();
            if (loanIds.Any())
            {
                var data = await this.dapperHandler.GetAll<KeyValueDto<int>>(
                    @$"  select lpn.Id as ID, LoanId as [Key], pn.name as [Value] from survey.LoanPlanNo lpn
                          LEFT JOIN survey.PlanNo pn on lpn.PlanNoId=pn.Id
                          where LoanId in ({loanIds.JoinToString()}) and lpn.IsDeleted = 0
                          order by lpn.Id desc");
                foreach (var loan in list)
                {
                    loan.LoanPlanTitle = data.FirstOrDefault(p => p.Key == loan.LoanId)?.Value;
                    loan.LoanPlanId = data.FirstOrDefault(p => p.Key == loan.LoanId)?.ID;
                }
                var prevPlanNo = await this.dapperHandler.GetAll<KeyValueDto<int>>(
                    @$"  select  lpn.Id as ID, LoanId as [Key], pn.name as [Value] from survey.LoanPlanNo lpn
                          LEFT JOIN survey.PlanNo pn on lpn.PlanNoId=pn.Id
                          where LoanId in ({loanIds.JoinToString()}) and lpn.IsDeleted = 1
                          order by lpn.Id desc");
                foreach (var loan in list)
                {
                    loan.OldLoanPlanTitle = prevPlanNo.FirstOrDefault(p => p.Key == loan.LoanId)?.Value;
                    loan.OldLoanPlanId = prevPlanNo.FirstOrDefault(p => p.Key == loan.LoanId)?.ID;
                }



            }
            result.TotalRows = await this.dapperHandler.GetFirstOrDefault<int>(MakePlanoHistorySelectSection(true) + query + whereSection);
            result.Results = list;
            return result;

        }
        private string MakePlanoHistorySelectSection(bool iscount = false)
        {
            if (iscount)
                return "select count(*) ";
            else
                return @"SELECT distinct a.Id, l.LoanNumber, lcc.BeginDate, a.LoanId, a.PlanNoStatusId as StatusId, pns.Title StatusTitle
							, cust.FullName as FullCustomerName
							, l.BranchCode,branch.BranchName,branch.SupervisorCode,branch.SupervisorName
                            , lcc.AcceptAmount, lrw.LastSurveyUserId
							, IIF(lrw.LastSurveyDate IS NULL OR lrw.LastSurveyDate < lrw.LastOutSideSurveyDate,lrw.LastOutSideSurveyDate, lrw.LastSurveyDate) AS LastSurveyDate
                            --, p.Name PlanNoTitle
							, a.ConfirmDate, a.RequestDate
                            , a.RequestUserId, a.ConfirmUserId
                            , a.OtherPlanNo, a.Description 
                            ";
        }
        public string MakeStatusQuery(object value)
        {
            if ((int)value == 1)
                return $" and (a.PlanNoStatusId = 1 or a.PlanNoStatusId is null)";
            return $" and a.PlanNoStatusId = {value}";
        }
        public string MakeCustomerNumberQuery(object value)
        {
            return $" AND cust.CustomerNumber={value}";
        }
        public string MakePlanNoIdQuery(object value)
        {
            return $" AND pn.Id={value}";
        }
        public string MakeConfirmUserIdQuery(object value)
        {
            return $" AND a.ConfirmUserId={value}";
        }
        public string MakeBranchListQuery(object value)
        {
            if (value != null && ((List<int>)value).Count > 0)
                return $" AND branch.BranchCode in ({((List<int>)value).JoinToString()})";
            return "";
        }
        public string MakeSupervisorsQuery(object value)
        {
            if (value != null && ((List<int>)value).Count > 0)
                return $" and branch.SupervisorCode in ({((List<int>)value).JoinToString()})";
            return "";
        }
        public string MakeSurveyUsersQuery(object value)
        {
            if (value != null && ((List<int>)value).Count > 0)
                return $" and lrw.LastSurveyUserId in ({((List<int>)value).JoinToString()})";
            return "";
        }
        public string MakeSurveyMinDateQuery(object value)
        {
            string query = " ";
            string date = value.ToString();
            if (!string.IsNullOrEmpty(date))
            {
                query = $" and lrw.LastSurveyDate >='{dateConvertor.ConvertPersianDatetimeToGregorianOrNull(date)}' ";
            }
            return query;
        }

        public string MakeSurveyMaxDateQuery(object value)
        {
            string query = " ";
            string date = value.ToString();
            if (!string.IsNullOrEmpty(date))
            {
                query = $" AND lrw.LastSurveyDate <='{dateConvertor.ConvertPersianDatetimeToGregorianOrNull(date)}' ";
            }
            return query;
        }
        #endregion

        public async Task<ResultList<LoanPlanNoStatistics>> GetLoanPlanNoList(PlanNoFilterDto request)
        {

            var select = @"
            SELECT 
            p.Id  as PlanNoId,
            p.Name AS PlanNoName,
            p.CreatedBy,
            p.CreateDate,
            COUNT(CASE WHEN l.LoanId IS NOT NULL and l.IsDeleted=0 THEN 1  END) AS LoanCount
            ";
            var table = @"  FROM [survey].[PlanNo] p
            Left JOIN [survey].[LoanPlanNo] l ON l.PlanNoId = p.Id
            WHERE p.IsDeleted = 0 ";

            if (!string.IsNullOrEmpty(request.PlanNoName))
            {
                table += $" and p.Name like  N'%{request.PlanNoName}%'";
            }
            table += @"
                        GROUP BY p.Id, p.Name, p.CreatedBy, p.CreateDate ";
            string pagingQuery = @$"  order by loanCount desc  OFFSET {request.Skip} ROWS FETCH NEXT {request.Take} ROWS ONLY";

            var finalQuery = select + table + pagingQuery;
            var response = await dapperHandler.GetAll<LoanPlanNoStatistics>(finalQuery);
            var countQuery = new StringBuilder($"SELECT top 1 TotalRows = count(*) over () {table}");
            var result = new ResultList<LoanPlanNoStatistics>()
            {
                Results = response,
                TotalRows = await this.dapperHandler.Get<int>(countQuery.ToString())
            };
            
            return result;
        }
    }
}
