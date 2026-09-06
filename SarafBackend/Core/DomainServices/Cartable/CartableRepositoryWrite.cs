using CommonLibrary.Core.Domain;
using CommonLibrary.Core.Domain.Dto;
using CommonLibrary.Core.Domain.Entities;
using CommonLibrary.Core.Domain.Entities.Loans;
using CommonLibrary.Core.Domain.Entities.Survey;
using CommonLibrary.Core.Domain.Enum;
using CommonLibrary.Core.Domain.Enum.Survey;
using CommonLibrary.Core.Domain.Interfaces;
using CommonLibrary.Core.Domain.RepositoryInterfaces;
using LoanMonitoringMicroService.Core.Domain.RepositoryInterfaces;
using LoanMonitoringMicroService.Core.Domain.ViewModel.Cartable;
using LoanMonitoringMicroService.Core.DomainServices.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Transactions;


namespace LoanMonitoringMicroService.Core.DomainServices.Cartables
{
    public class CartableRepositryWrite : ICartableRepositryWrite
    {
        private readonly IDapperHandler dapperHandler;
        private readonly IDateConvertor dateConvertor;
        private readonly ILoanMonitoringDbContext context;
        private readonly LoginUserDTO loginUser;

        public CartableRepositryWrite(IDapperHandler _dapperHandler, IDateConvertor _dateConvertor
            , ILoanMonitoringDbContext LoanDbContext, LoginUserDTO _loginUser)
        {
            dapperHandler = _dapperHandler;
            dateConvertor = _dateConvertor;
            context = LoanDbContext;
            this.loginUser = _loginUser;
        }
        #region Set Cartable
        public async Task<ResultObject<int>> SetCartable(UserCartableVM request)
        {
            ResultObject<int> resultObject = new ResultObject<int>();
            if (!await hasLoanWithPlanNo(request.LoansId))
            {
                resultObject.ServerErrors.Add(new ServerError()
                {
                    Hint = "عنوان رشته فعالیت برای وام انتخاب نشده است",
                });
                return resultObject;
            }

            if (await hasLoanPlanNoChange(request.LoansId))
            {
                resultObject.ServerErrors.Add(new ServerError()
                {
                    Hint = "عنوان رشته فعالیت تایید نشده است",
                });
                return resultObject;
            }
            if (!await CanDoSupervision(request.LoansId))
            {
                resultObject.ServerErrors.Add(new ServerError()
                {
                    Hint = "پرونده مجوز نظارت ندارد",
                });
                return resultObject;
            }

            //if (!await HasBranchPermission(request.LoansId))
            //{
            //    resultObject.ServerErrors.Add(new ServerError()
            //    {
            //        Hint = "دسترسی ارجاع به شعب انتخاب شده ندارید",
            //    });
            //    return resultObject;
            //}
            var checkResult = await canSetActorToLoans(request);
            if (checkResult.ServerErrors.Count > 0)
                return checkResult;
            var cartables = new List<Cartable>();
            request.LoansId = request.LoansId.Distinct().ToList();
            var loanplan = await this.GetLoanPalnNoByLoanIdList(request.LoansId);
            //var changeLoanPlanState = await this.GetNotVerifiedRequestByLoanIdListAsync(request.LoansId);
            foreach (var loanID in request.LoansId)
            {
                var changeLoanPlanState = this.context.ChangeLoanPlanStates
                .Where(c => c.LoanId == loanID && !c.IsDeleted).OrderByDescending(c=>c.RequestDate).FirstOrDefault();
                if (changeLoanPlanState != null && !changeLoanPlanState.VerifyBy.HasValue)
                {
                    resultObject.ServerErrors.Add(new ServerError()
                    { Hint = $"درخواست تغییر محل طرح وام تایید نشده است" });
                    return resultObject;
                }
            }
            
            foreach (var item in request.LoansId)
            {
                cartables.Add(new Cartable
                {
                    LoanId = item,
                    UserId = request.UserId,
                    CartableStatusTypeId = 1,
                    PlanNoId = loanplan.First(c => c.LoanId == item).PlanNoId,
                    Description = request.Description ?? "",
                    ReferenceUserId = this.loginUser.ID,
                    ExpireDate = DateTime.Now.AddDays(7),
                    ReferenceDate = DateTime.Now,
                    CreateDate = DateTime.Now,
                    CreatedBy = this.loginUser.ID,
                    IsNotScheduledReferred= request.IsNotScheduledReferred
                });
                this.context.LoanActionLogs.Add(new LoanActionLog
                {
                    Title = LoanActionLogTypeEnum.NewActor,
                    ActionType = LoanActionLogTypeEnum.Insert,
                    Description = LoanActionLogTypeEnum.NewSurveyUser,
                    loanId = item,
                    CreateDate = DateTime.Now,
                    CreatedBy = this.loginUser.ID,
                    IpAddress = ""
                });
            }
            this.context.Cartables.AddRange(cartables);
            try
            {
                this.context.SaveChanges();
            }
            catch (Exception ex)
            {

                throw;
            }
            await this.FillCartableInfo(request.LoansId);
            return resultObject;
        }
        private async Task<bool> hasLoanWithPlanNo(List<int> loanIds)
        {
            var t1= await this.context.LoanPlanNo.AnyAsync(c => c.LoanId != null && loanIds.Contains(c.LoanId) && !c.IsDeleted); 
            return t1;
        }

        private async Task<bool> hasLoanPlanNoChange(List<int> loanIds)
        {
            return await this.context.PlanNoHistories
                .Where(c =>loanIds.Contains(c.LoanId)&& c.CartableId != null&& c.ConfirmDate==null&&c.ConfirmUserId==null )
                .OrderByDescending(x=>x.RequestDate).AnyAsync(); ;
        }
        private async Task<bool> HasBranchPermission(List<int> loanIds)
        {
            var branches = this.loginUser.UserRoleBranchSystems.Where(p => p.BranchID.HasValue).Select(p => p.BranchID.Value).ToList();
            if (branches.Any())
            {
                var loanBranches = await this.context.Loans.Where(c => loanIds.Contains(c.Id)).Select(p => p.BranchCode).ToListAsync();
                return !loanBranches.Except(branches).Any();
            }
            return true;
        }
        private async Task<bool> CanDoSupervision(List<int> loanIds)
        {
            bool hasNotSupervisionLoan = await context.Loans
                .AnyAsync(l => loanIds.Contains(l.Id) && l.NotSupervision.Value);

            return !hasNotSupervisionLoan;
        }


        private async Task<ResultObject<int>> canSetActorToLoans(UserCartableVM userCartable)
        {
            ResultObject<int> resultObject = new ResultObject<int>();
            var currentUserRolesId = this.loginUser.UserRoleBranchSystems.Select(p => p.RoleID).Distinct().ToList();
            var checkyearly = await checkYearly(userCartable.LoansId);
            if (!checkyearly)
            {
                resultObject.ServerErrors.Add(new ServerError()
                {
                    Hint = "پرونده در سال جاری نظارت شده است"
                });
                return resultObject;
            }
            var loans = await this.GetSurveyReferralLoanListByLoanIdsAsync(userCartable.LoansId);
            if (loans == null || loans.Count == 0)
            {
                resultObject.ServerErrors.Add(new ServerError()
                {
                    Hint = "پرونده دارای ناظر فعال می باشد"
                });
                return resultObject;
            }
            if (loans.Any(c => c.LoanContract.AcceptAmount < 200000000))
            {
                resultObject.ServerErrors.Add(new ServerError()
                {
                    Hint = "مبلغ وام کمتر از حد مجاز",
                });
            }
            //var currentUserRoles = await _userService.GetUserRolesAsync(userCartable.UserId);
            //var currentUserRolesId = currentUserRoles.Select(c => c.RoleId).ToList();
            //var currentUserRolesId = this.loginUser.;
            var surveyConfig = this.context.SurveyConfigs.FirstOrDefault();
            if (currentUserRolesId.Any(c => c == UserRoleId.WageWorker))
            {
                resultObject.ServerErrors.Add(new ServerError()
                {
                    Hint = "امکان ارجاع پرونده جاری یافعال به نیروی حق الزحمه ای یا نیروی شرکتی وجود ندارد",
                });
                return resultObject;
            }
            if (loans.Any(c => c.LoanContract.AcceptAmount > surveyConfig.MaxAllowedLoanAmount) &&
            currentUserRolesId.Any(c => c == UserRoleId.MonitoringWroker || c == UserRoleId.MonitoringWageWorker))
            {
                var customerNumber = loans.FirstOrDefault(c => c.LoanContract.AcceptAmount > surveyConfig.MaxAllowedLoanAmount).CustomerNumber;
                var err = $"مبلغ وام شماره{customerNumber} بیشتر از سقف مجاز({surveyConfig.MaxAllowedLoanAmount}ریال)   می باشد";
                resultObject.ServerErrors.Add(new ServerError()
                {
                    Hint = err,
                });
                return resultObject;
            }
            // resultObject = await HasLoanCartableAsync(loans, userCartable.UserId);// بدستور اقای اشکانی چک کردن اولین نظارت کامنت شد
            return resultObject;
        }
        public async Task<bool> checkYearly(List<int> loanid)
        {
            var query = $@"SELECT a.Id
                            FROM core.Loans a
                            JOIN core.LoanRedundantView b ON a.Id=b.LoanId
                            JOIN survey.Cartable c ON a.Id=c.LoanId
                            JOIN survey.Survey d ON c.Id=d.CartableId
                            JOIN Core.LoanContracts e ON a.id=e.LoanId
                            WHERE d.SurveyDate>'2023-03-21 00:00:00.000' 
                            AND e.AcceptAmount < 200000001
                            AND e.BeginDate>='2021-03-21 00:00:00.000'";
            var yearlyList = await this.dapperHandler.GetAll<int>(query);
            foreach (var item in loanid)
            {
                if (yearlyList.Any(c => c == item))
                {
                    return false;
                }
            }
            return true;
        }
        public async Task<List<Loan>> GetSurveyReferralLoanListByLoanIdsAsync(List<int> loanIds)
        {
            try
            {
                var notAllowedLoanIdList = context.Cartables.Where(c =>
                loanIds.Contains(c.LoanId) && !c.IsDeleted &&
                  c.CartableStatusTypeId != CartableStatusTypes.Computed
                  && c.CartableStatusTypeId != CartableStatusTypes.EndByUser
                  && c.CartableStatusTypeId != CartableStatusTypes.Observed)
                 .Select(c => c.LoanId);
                var loans = await context.Loans
                    .Include(c => c.Cartables)
                    .Include(c => c.LoanContract)
                    .Where(l => loanIds.Contains(l.Id) &&
                    !notAllowedLoanIdList.Contains(l.Id)
                    ).ToListAsync();
                return loans;
            }
            catch (Exception e)
            {
                return null;
                throw;
            }
        }

        public async Task<List<LoanPlanNo>> GetLoanPalnNoByLoanIdList(List<int> loanIdList)
        {
            return await this.context.LoanPlanNo.Where(c => c.LoanId != null && loanIdList.Contains(c.LoanId) && !c.IsDeleted).ToListAsync();
        }
        public async Task<List<ChangeLoanPlanState>> GetNotVerifiedRequestByLoanIdListAsync(List<int> loanIdList)
        {
            return await this.context.ChangeLoanPlanStates
                .Where(c => loanIdList.Contains(c.LoanId) && !c.VerifyBy.HasValue && !c.IsDeleted).ToListAsync();
        }

        public async Task FillCartableInfo(List<int> loanIdList)
        {
            var loanIdListQuery = string.Join(',', loanIdList);
            await this.dapperHandler.Get<int>(fillLastCartableId(loanIdListQuery));
            await this.dapperHandler.Get<int>(fillLastReferenceDate(loanIdListQuery));
            await this.dapperHandler.Get<int>(fillSurveyCartableCount(loanIdListQuery));
            await this.dapperHandler.Get<int>(fillLastSurveyUserId(loanIdListQuery));
        }
        private string fillLastCartableId(string loanIds)
        {
            return
             $@"
                UPDATE core.LoanRedundantView
                SET LastCartableId=null WHERE LoanId IN ({loanIds})
                UPDATE core.LoanRedundantView
                            set LastCartableId= aa.Id
                             from  core.LoanRedundantView a
                            join
                            (select
                             ROW_NUMBER() OVER (PARTITION BY loanId ORDER BY ReferenceDate DESC) rn,Id,LoanId
                             from survey.Cartable
                             where IsDeleted=0 AND LoanId IN ({loanIds}))aa
                             on a.LoanId=aa.LoanId and aa.rn=1";
        }
        private string fillLastReferenceDate(string loanIds)
        {
            return
                    $@"update core.LoanRedundantView
                    set LastReferenceDate=null WHERE LoanId IN ({loanIds})
                    update core.LoanRedundantView
                         set LastReferenceDate= aa.ReferenceDate
                          from  core.LoanRedundantView a
                         join
                         (select
                          ROW_NUMBER() OVER (PARTITION BY loanId ORDER BY ReferenceDate DESC) rn,ReferenceDate,LoanId
                         from survey.Cartable
                         where IsDeleted=0 AND LoanId IN ({loanIds}))aa
                         on a.LoanId=aa.LoanId and aa.rn=1";
        }
        private string fillSurveyCartableCount(string loanIds)
        {
            return $@"
                    update core.LoanRedundantView
                    set SurveyCartableCount=null WHERE LoanId IN ({loanIds})
                    update core.LoanRedundantView
                    set SurveyCartableCount=b.cnt
                    from core.LoanRedundantView a
                    join (
                    select count(c.Id) as cnt,loanid from survey.Cartable c
                    LEFT  JOIN survey.Survey as s on c.Id=s.CartableId AND s.IsDeleted=0  
                    where c.IsDeleted=0 AND LoanId IN({loanIds}) and  ((s.SurveyDate is not null ) --AND  c.CartableStatusTypeId = 2)
								    OR (s.SurveyDate is null and c.CartableStatusTypeId <>2))
                    group by LoanId)b on a.LoanId=b.LoanId";
        }
        private string fillLastSurveyUserId(string loanIds)
        {
            return $@" 
                    update core.LoanRedundantView
                    set LastSurveyUserId=null WHERE LoanId IN ({loanIds})
                     update core.LoanRedundantView
                    set LastSurveyUserId=aa.UserId 
                     from  core.LoanRedundantView a
                    join
                    (select
                     ROW_NUMBER() OVER (PARTITION BY loanId ORDER BY ReferenceDate DESC) rn,UserId,LoanId
                    from survey.Cartable
                    where IsDeleted=0 and CartableStatusTypeId IN(1,2) AND LoanId IN({loanIds}))aa
                    on a.LoanId=aa.LoanId and aa.rn=1";
        }

        #endregion

        #region Remove Cartable
        public async Task<ResultObject<int>> RemoveCartable(int loanID, int SurveyListTypeId)
        {
            var result = new ResultObject<int>();
            var list = new List<int> { 1, 2 };
            var cartable = context.Cartables.FirstOrDefault(c => !c.IsDeleted
                            && (list.Contains(c.CartableStatusTypeId)) && c.LoanId == loanID); 
            if (cartable == null)
            {
                result.ServerErrors.Add(new ServerError() { Hint = "امکان حذف کارتابل وجود ندارد" });
                return result;
            }
            await this.RemoveCartable(cartable);
            return result;
        }

        public async Task<ResultObject<int>> RemoveCartableByID(int id)
        {
            var result = new ResultObject<int>();
            var cartable = await this.context.Cartables.SingleAsync(p => p.Id == id);
            if (cartable.CartableStatusTypeId != 1)
            {
                result.ServerErrors.Add(new ServerError() { Hint = "امکان حذف کارتابل وجود ندارد" });
                return result;
            }
            await this.RemoveCartable(cartable);
            return result;
        }
        public async Task<ResultObject<int>> RemoveCartableList(List<int> loanIds)
        {
            var result = new ResultObject<int>();
            var cartables = new List<Cartable>();
            foreach (var loanId in loanIds)
            {
                var cartable = this.context.Cartables.Where(p => p.LoanId == loanId && !p.IsDeleted)
                    .OrderByDescending(p=>p.ReferenceDate).FirstOrDefault();
                if (cartable != null && cartable.CartableStatusTypeId != 1)
                {
                    result.ServerErrors.Add(new ServerError() { Hint = "امکان حذف کارتابل وجود ندارد" });
                    return result;
                }
                if (cartable != null)
                    cartables.Add(cartable);
            }
            //var cartables = this.context.Cartables.Where(p => loanIds.Contains(p.LoanId) && !p.IsDeleted).ToList();
            //foreach (var cartable in cartables)
            //{
            //    if (cartable.CartableStatusTypeId != 1)
            //    {
            //        result.ServerErrors.Add(new ServerError() { Hint = "امکان حذف کارتابل وجود ندارد" });
            //        return result;
            //    }
            //}
            foreach (var cartable in cartables)
                await this.RemoveCartable(cartable);
            return result;
        }
        private async Task RemoveCartable(Cartable cartable)
        {
            var surveys = await this.context.Surveys.Where(p => p.CartableId == cartable.Id).ToListAsync();
            cartable.IsDeleted = true;
            cartable.ModifiedBy = this.loginUser.ID;
            cartable.UpdateDate = DateTime.Now;
            foreach (var survey in surveys)
            {
                survey.IsDeleted = true;
                survey.ModifiedBy = this.loginUser.ID;
                survey.UpdateDate = DateTime.Now;
            }
            this.context.LoanActionLogs.Add(new LoanActionLog
            {
                Title = LoanActionLogTypeEnum.RemoveCartable,
                ActionType = LoanActionLogTypeEnum.Modify,
                Description = LoanActionLogTypeEnum.RemoveCartable,
                loanId = cartable.LoanId,
                CreateDate = DateTime.Now,
                CreatedBy = this.loginUser.ID,
            });
            await this.context.SaveChangesAsync();
            await this.FillCartableInfo([cartable.LoanId]);
        }
        #endregion

    }
}
