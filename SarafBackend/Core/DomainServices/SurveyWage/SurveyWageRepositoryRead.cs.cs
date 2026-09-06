using CommonLibrary.Core.Domain;
using CommonLibrary.Core.Domain.Dto;
using CommonLibrary.Core.Domain.Entities.Users;
using CommonLibrary.Core.Domain.Enum;
using CommonLibrary.Core.Domain.Interfaces;
using CommonLibrary.Core.Domain.RepositoryInterfaces;
using CommonLibrary.Infrastructure.Utils;
using CommonLibrary.Infrastructure.Utils.Extensions;
using LoanMonitoringMicroService.Core.Domain.Entities;
using LoanMonitoringMicroService.Core.Domain.RepositoryInterfaces;
using LoanMonitoringMicroService.Core.Domain.ViewModel.Wage;
using LoanMonitoringMicroService.Core.DomainServices.Interfaces;
using LoanMonitoringMicroService.Domains.Core.Supervision.Entities.Dao;
using LoanMonitoringMicroService.Domains.Supervision.Service;
using Microsoft.EntityFrameworkCore;
using PaymentMonitoring.Domains.Core.Entities;
using System.Linq;
using System.Text;

namespace LoanMonitoringMicroService.Core.DomainServices.Wage
{
    public class SurveyWageRepositoryRead : ISurveyWageRepositoryRead
    {
        private readonly IDapperHandler _dapperHandler;
        private readonly IDateConvertor dateConvertor;
        private readonly ILoanMonitoringDbContext Context;
        private readonly LoginUserDTO loginUser;
        public SurveyWageRepositoryRead(IDapperHandler dapperHandler, IDateConvertor dateConvertor
            ,ILoanMonitoringDbContext Context, LoginUserDTO _loginUser)
        {
            _dapperHandler = dapperHandler;
            this.dateConvertor = dateConvertor;
            this.Context = Context;
            this.loginUser = _loginUser;
        }

        public async Task<ResultList<SurveyWageVM>> GetSurveyWageList(SurveyWageFilterVM request)
        {
            var result = new ResultList<SurveyWageVM>();

            var table = "UserSurveyWage";
            var wageDate = "ComputeDate";
            if (request.ShowConfirmedSurveys)
            {
                table = "SurveyWagePayInfo";
                wageDate = "PaymentDate";
            }
            var query = @$"SELECT [a].[Id],
                        [a].[UserId], [a].[WageAmount],
                        --[b].[BranchName] AS [Branch],
                        [a].[LongDistance],
                        [a].[Distance], [a].[PointCount],
                        [a].[ShortDistancePointCount],
                        --[b].[SupervisorName] AS [Supervisor],
                        [a].[{wageDate}] AS [WageDate]{(request.ShowConfirmedSurveys ? ",a.PaymentMonth" : "")}
                        FROM [survey].[{table}] AS [a]
                        ";

            string pagingQuery = $" ORDER BY [a].[Id] OFFSET {request.Skip} ROWS FETCH NEXT {request.Take} ROWS ONLY";

            var finalQuery = query + makeWhereSurveyWage(request) + pagingQuery;
            var response = await _dapperHandler.GetAll<SurveyWageVM>(finalQuery);
            foreach ( var item in response)
            {
                if (!string.IsNullOrEmpty(item.PaymentMonth))
                    item.PaymentMonth = item.WageDate.GetMonthName(int.Parse(item.PaymentMonth));
            }
            result.TotalRows = await this._dapperHandler.Get<int>($"select count(*) FROM [survey].[{table}] a " + makeWhereSurveyWage(request));
            result.Results = response ?? new List<SurveyWageVM>();
            return result;
        }

        private string makeWhereSurveyWage(SurveyWageFilterVM request)
        {
            var wageDate = "ComputeDate";
            if (request.ShowConfirmedSurveys)
            {
                wageDate = "PaymentDate";
            }
            var whereQuery = new StringBuilder();
            whereQuery.Append($"WHERE [a].[IsDeleted] = 0 ");
            if (!request.ShowConfirmedSurveys)
                whereQuery.Append(" AND [a].[UserSurveyWageStatusTypeId] = 1 ");

            if (request.SurveyUserId.HasValue)
                whereQuery.Append($" AND [a].[UserID] = {request.SurveyUserId}");
            if (request.UserIds?.Count > 0)
                whereQuery.Append($" AND [a].[UserID] in ({request.UserIds.JoinToString()})");

            //if (request.BranchList != null && request.BranchList.Any())
            //{
            //    var branchIds = string.Join(",", request.BranchList);
            //    whereQuery.Append($" AND [b].[BranchCode] IN ({branchIds})");
            //}

            //if (request.SupervisorList != null && request.SupervisorList.Any())
            //{
            //    var SupervisorList = string.Join(",", request.SupervisorList);
            //    whereQuery.Append($" AND [b].[SupervisorCode] IN ({SupervisorList})");
            //}

            if (!string.IsNullOrEmpty(request.SurveyMinDate) && !string.IsNullOrEmpty(request.SurveyMaxDate))
            {
                var fromDate = this.dateConvertor.ConvertPersianDatetimeToGregorian(request.SurveyMinDate).ToShortDateString();
                var toDate = this.dateConvertor.ConvertPersianDatetimeToGregorian(request.SurveyMaxDate).ToShortDateString();

                whereQuery.Append($" AND ISNULL([a].[{wageDate}], '1900-01-01') BETWEEN '{fromDate}' AND '{toDate}' ");
            }
            else if (!string.IsNullOrEmpty(request.SurveyMinDate))
            {
                var fromDate = this.dateConvertor.ConvertPersianDatetimeToGregorian(request.SurveyMinDate).ToShortDateString();
                whereQuery.Append($" AND ISNULL([a].[{wageDate}], '1900-01-01') >= '{fromDate}' ");
            }
            else if (!string.IsNullOrEmpty(request.SurveyMaxDate))
            {
                var toDate = this.dateConvertor.ConvertPersianDatetimeToGregorian(request.SurveyMaxDate).ToShortDateString();
                whereQuery.Append($" AND ISNULL([a].[{wageDate}], '1900-01-01') <= '{toDate}' ");
            }

            if (request.AmountMax.HasValue && request.SurveyMaxDate != null)
            {
                whereQuery.Append($" AND ISNULL([a].[WageAmount], 0) BETWEEN {request.AmountMin} AND {request.AmountMax} ");
            }
            else if (request.AmountMin.HasValue)
            {
                whereQuery.Append($" AND ISNULL([a].[WageAmount], 0) >= {request.AmountMin} ");
            }
            else if (request.AmountMax.HasValue)
            {
                whereQuery.Append($" AND ISNULL([a].[WageAmount], 0) <= {request.AmountMax} ");
            }


            return whereQuery.ToString();
        }
           

           

        public Task<ResultList<SurveyWageVM>> ComputeSurveyUsersWage(SurveyWageFilterVM request)
        {
            throw new NotImplementedException();
        }
        public Task<ResultList<SurveyWageVM>> ComputeSurveyUserWage(SurveyWageFilterVM request)
        {
            throw new NotImplementedException();
        }

        public async Task<SurveyWageDocumentVM> GetSurveyWageInfoReport(int surveyWagePayInfoId)
        {
            var surveyWagePayInfo = await Context.SurveyWagePayInfos.Include(c => c.UserSurveyWages)
              .FirstAsync(c => c.Id == surveyWagePayInfoId);
            var surveyWageInfoReport = new SurveyWageDocumentVM
            {
                SurveyUserName = "",//user.FullUserName,
                //Branch = makeBranchesName(userBranches),
                //Supervisor = makeSupervisorName(userBranches),
                SurveyUserID = surveyWagePayInfo.UserId,
                WageAmount = surveyWagePayInfo.WageAmount,
                Distance = surveyWagePayInfo.Distance,
                LongDistance = surveyWagePayInfo.LongDistance,
                PaymentMonth = DateTimeExtension.MakeMonthName(surveyWagePayInfo.PaymentMonth),
                PointCount = surveyWagePayInfo.PointCount,
                ShortDistancePointCount = surveyWagePayInfo.ShortDistancePointCount,
                WageDate = dateConvertor.ConvertGregorianToPersianDate(surveyWagePayInfo.PaymentDate),
                UserName = loginUser.FullName,
                PrintDate = dateConvertor.ConvertGregorianToPersianDate(DateTime.Now),
            };
            var userSurveyWagesId = surveyWagePayInfo.UserSurveyWages.Select(c => c.Id).ToList();
            var surveyWageLoanPlan = await Context.SurveyWageLoanPlan
                .Include(c => c.LoanPlan)//.ThenInclude(c => c.PlanNoId)
                .Include(c => c.LoanPlan).ThenInclude(c => c.Cartable)//.ThenInclude(c => c.Surveys).ThenInclude(c => c.PlanActivationType)
                .Include(c => c.LoanPlan).ThenInclude(c => c.Loan).ThenInclude(c => c.Customer)//.ThenInclude(c => c.Branch)
                .Where(c => userSurveyWagesId.Contains(c.UserSurveyWageId)
                && !c.UserSurveyWage.IsDeleted
                ).ToListAsync();
            var surveyWageInfoLoanReports = makeSurveyWageInfoLoanReport(surveyWageLoanPlan);
            surveyWageInfoReport.SurveyWageInfoLoanReports = surveyWageInfoLoanReports;
            return surveyWageInfoReport;
        }

        private List<SurveyWageInfoLoanReport> makeSurveyWageInfoLoanReport(List<SurveyWageLoanPlan> surveyWageLoanPlans)
        {
            var surveyWageInfoLoanReports = new List<SurveyWageInfoLoanReport>();
            var loanIds = surveyWageLoanPlans.Select(c => c.LoanPlan.LoanId).Distinct().ToList();
            var loanSurveyCount = Context.LoanRedundantViews.Where(c => loanIds.Contains(c.LoanId)).ToList();
            foreach (var item in surveyWageLoanPlans)
            {
                var survey = Context.Surveys.First(p => p.CartableId == item.LoanPlan.Cartable.Id);
                var branch = Context.Branches.First(p => p.BranchCode == item.LoanPlan.Loan.BranchCode);
                surveyWageInfoLoanReports.Add(new SurveyWageInfoLoanReport
                {
                    LoanId = item.LoanPlan.Loan.Id,
                    CustomerName = item.LoanPlan.Loan.Customer?.FullName,
                    LoanNumber = item.LoanPlan.Loan.LoanNumber,
                    PlanNoName = Context.PlanNos.First(p=>p.Id == item.LoanPlan.PlanNoId)?.Name,// item.LoanPlan.PlanNo.Name,
                    ReferenceDate = DateTimeExtension.ConvertGregorianToPersianDate(item.LoanPlan.Cartable.ReferenceDate),
                    SurveyDate = DateTimeExtension.ConvertGregorianToPersianDate(survey.SurveyDate),
                    Branch = branch?.BranchName,
                    Supervisor = branch?.SupervisorName,
                    NumberOfInsurdPerson = survey.NumberOfInsurdPerson,
                    NumberOfJobsCreated = survey.NumberOfJobsCreated,
                    PlanActivationType = Context.PlanActivationTypes.First(p=>p.ID== survey.PlanActivationTypeId)?.Name,
                    SurveyCount = loanSurveyCount.Single(c => c.LoanId == item.LoanPlan.LoanId).SurveyCount
                });
            }
            surveyWageInfoLoanReports = surveyWageInfoLoanReports.GroupBy(c => c.LoanId).Select(c => c.FirstOrDefault()).ToList();
            surveyWageInfoLoanReports = setLoanReagent(surveyWageInfoLoanReports);
            return surveyWageInfoLoanReports;
        }

        private List<SurveyWageInfoLoanReport> setLoanReagent(List<SurveyWageInfoLoanReport> surveyWageInfoLoanReportList)
        {
            var loanIdList = surveyWageInfoLoanReportList.Select(c => c.LoanId).ToList();
            var reagent = Context.LoanGuaranties.Include(c => c.Customer)
                .Where(c => loanIdList.Contains(c.LoanId.Value) && c.RelationTypeId == LoanRelationTypeEnum.Reagent).ToList();
            foreach (var item in surveyWageInfoLoanReportList)
            {
                item.Reagent = reagent.FirstOrDefault(c => c.LoanId == item.LoanId)?.Customer?.FullName;
            }
            return surveyWageInfoLoanReportList;
        }
    }
}
