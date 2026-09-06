using CommonLibrary.Core.Domain;
using CommonLibrary.Core.Domain.Dto;
using CommonLibrary.Core.Domain.Entities.Survey;
using CommonLibrary.Core.Domain.RepositoryInterfaces;
using LoanMonitoringMicroService.Application.DTO.Pwa;
using LoanMonitoringMicroService.Core.Domain.RepositoryInterfaces;
using LoanMonitoringMicroService.Core.Domain.ViewModel.PlanNo;
using LoanMonitoringMicroService.Core.DomainServices.PlanNo;
using LoanMonitoringMicroService.Domains.Core.Supervision.Entities.Dao;

using Microsoft.EntityFrameworkCore;

namespace LoanMonitoringMicroService.Core.DomainServices.PlanNo
{
    public class PlanNoRepositoryWrite : IPlanNoRepositoryWrite
    {
        private readonly IDapperHandler _dapperHandler;
        private readonly ILoanMonitoringDbContext dbContext;
        private readonly LoginUserDTO loginUser;

        public PlanNoRepositoryWrite(ILoanMonitoringDbContext _dbContext, IDapperHandler dapperHandler, LoginUserDTO _loginUser)
        {
            dbContext = _dbContext;
            _dapperHandler = dapperHandler;
            this.loginUser = _loginUser;
        }

        public Task<ResultList<PlanNoVM>> UpdateApprovedStatus(PlanNoFilterDto request)
        {
            throw new NotImplementedException();
        }
       
        public async Task<ResultObject<int>> SetLoanPlanNo(LoanPlanNoVM request)
        {
            var result = new ResultObject<int>();
            var loanPlanNoLasts = await dbContext.LoanPlanNo.Where(e => e.LoanId == request.LoanId && !e.IsDeleted).ToListAsync();
            var userId = this.loginUser.ID;

            foreach (var loanPlanNoLast in loanPlanNoLasts)
            {
                loanPlanNoLast.IsDeleted = true;
                loanPlanNoLast.UpdateDate = DateTime.Now;
                dbContext.LoanPlanNo.Update(loanPlanNoLast);
            }

            int? loanSurveyEconomicTypeId = request.LoanSurveyEconomicTypeId;

            if (!loanSurveyEconomicTypeId.HasValue || loanSurveyEconomicTypeId == 0)
            {
                var loanSurveyEconomicType = await GetLoanSurveyEconomicType(request.LoanId);
                loanSurveyEconomicTypeId = loanSurveyEconomicType.Data?.Id ?? 4;
            }

            var newLoanPlanNo = new LoanPlanNo
            {
                CreateDate = DateTime.Now,
                CreatedBy = userId, 
                ModifiedBy = null,
                UpdateDate = DateTime.Now,
                IsDeleted = false,
                LoanId = request.LoanId,
                PlanNoId = request.PlanNoId,
                LoanSurveyEconomicTypeId = loanSurveyEconomicTypeId ?? 0,
                //LoanSurveyEconomicTypeId1 = null,
                UserOtherPlanNo = null,
                UserPlanNoText = null
            };

            await dbContext.LoanPlanNo.AddAsync(newLoanPlanNo);
            await dbContext.SaveChangesAsync();

            result.Data = newLoanPlanNo.Id;
          

            return result;

        }

        public async Task<ResultObject<LoanSurveyEconomicTypeVM>> GetLoanSurveyEconomicType(int id)
        {
            var query = $@"SELECT TOP(1) [b].[Id], [b].[LoanEconomicTypeId], [b].[Name]
                            FROM [Core].[LoanContracts] AS [a]
                            INNER JOIN [survey].[LoanSurveyEconomicType] AS [b] ON [a].[LoanEconomicTypeId] = [b].[Id]
                            WHERE [a].[LoanId] = {id}"; 
            var data= await _dapperHandler.GetFirstOrDefault<LoanSurveyEconomicTypeVM>(query);
            if (data == null)
            {
                return new ResultObject<LoanSurveyEconomicTypeVM>
                {
                    
                    Data = null
                };
            }
            return new ResultObject<LoanSurveyEconomicTypeVM>
            {
                Data = data
            };
        }

        public Task<ResultList<PlanNoVM>> SetOtherPlanNo(PlanNoFilterDto request)
        {
            throw new NotImplementedException();
        }
       
        public async Task<ResultObject<int>> RemovePlanNo(int Id)
        {
            var result = new ResultObject<int>();

            var PlaNo = await dbContext.PlanNos.FirstOrDefaultAsync(e => e.Id == Id);
            PlaNo.IsDeleted = true;
            await  dbContext.SaveChangesAsync();

            return result;
        }
        public async Task<ResultObject<int>> ChangePlanNoForLoans(int id, bool isConfrim, int planNoId, string? description)
        {
            var result = new ResultObject<int>();
            var history = await this.dbContext.PlanNoHistories.FirstOrDefaultAsync(p => p.Id == id);
            if (history != null)
            {
                history.ConfirmUserId = this.loginUser.ID;
                history.ConfirmDate = DateTime.Now;
                if (isConfrim)
                {
                    history.PlanNoStatusId = 2;
                    var existingLoanPlans = await dbContext.LoanPlanNo.Where(e => e.LoanId == history.LoanId && !e.IsDeleted).ToListAsync();
                    foreach (var item in existingLoanPlans)
                    {
                        item.IsDeleted = true;
                    }

                    dbContext.LoanPlanNo.Add(new LoanPlanNo
                    {
                        LoanId = history.LoanId,
                        PlanNoId = planNoId,
                        LoanSurveyEconomicTypeId = existingLoanPlans.First().LoanSurveyEconomicTypeId,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        IsDeleted = false,
                        CreatedBy = loginUser.ID
                    });
                }
                else
                {
                    history.PlanNoStatusId = 3;
                    history.Description = description;
                }
            }
            await dbContext.SaveChangesAsync();
            return result;
        }
        public async Task<ResultObject<int>> ChangeLoansPlanNo(int currentPlanNoId, int newPlanNoId)
        {
            var result = new ResultObject<int>();

            var existingLoanPlans = await dbContext.LoanPlanNo.Where(e => e.PlanNoId == currentPlanNoId && !e.IsDeleted).ToListAsync();
            var newLoanPlanno = new List<LoanPlanNo>();
            foreach (var item in existingLoanPlans)
            {
                item.IsDeleted = true;
                newLoanPlanno.Add(new LoanPlanNo
                {
                    LoanId = item.LoanId,
                    PlanNoId = newPlanNoId,
                    LoanSurveyEconomicTypeId = item.LoanSurveyEconomicTypeId,
                    CreateDate= item.CreateDate,
                    UpdateDate= item.UpdateDate,    
                });
            }

            dbContext.LoanPlanNo.AddRange(newLoanPlanno);
            await dbContext.SaveChangesAsync();
            return result;
        }
        public async Task<ResultObject<int>> SetPlanNo(string name)
        {
            var result = new ResultObject<int>();
            var userId = this.loginUser.ID;
            if (this.dbContext.PlanNos.Any(p=> !p.IsDeleted && (p.Name.Trim() == name.Trim() || p.Name.Trim().Contains(name.Trim()))))
            {
                result.ServerErrors.Add(new ServerError { Hint = "عنوان رشته فعالیت تکراری است" });
                return result;
            }
            var NewPlanNo = new CommonLibrary.Core.Domain.Entities.PlanNo.PlanNo
            {
                Approved = false,
                CreateDate = DateTime.Now,
                CreatedBy = userId,
                IsDeleted = false,
                ModifiedBy = null,
                Name = name,
                NewSurveyEconomicTypeId = null,
                SuveryEconomicTypeId = 0,
                UpdateDate = null
            };

            dbContext.PlanNos.Add(NewPlanNo);
            await dbContext.SaveChangesAsync();
            return result;

        }
    }
}
