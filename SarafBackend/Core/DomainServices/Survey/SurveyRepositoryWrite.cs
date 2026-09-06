using AutoMapper;
using Azure.Core;
using CommonLibrary.Core.Domain;
using CommonLibrary.Core.Domain.Dto;
using CommonLibrary.Core.Domain.Entities.Attachment;
using CommonLibrary.Core.Domain.Entities.Loans;
using CommonLibrary.Core.Domain.Entities.PlanNo;
using CommonLibrary.Core.Domain.Entities.Survey;
using CommonLibrary.Core.Domain.Enum;
using CommonLibrary.Core.Domain.Enum.Survey;
using CommonLibrary.Core.Domain.Interfaces;
using CommonLibrary.Core.Domain.RepositoryInterfaces;
using CommonLibrary.Infrastructure.Utils.Extensions;
using LoanMonitoringMicroService.Application.DTO.Survey;
using LoanMonitoringMicroService.Core.Domain.Entities;
using LoanMonitoringMicroService.Core.Domain.RepositoryInterfaces;
using LoanMonitoringMicroService.Core.Domain.ViewModel.Survey;
using LoanMonitoringMicroService.Core.DomainServices.Interfaces;
using LoanMonitoringMicroService.Domains.Supervision.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Transactions;

namespace LoanMonitoringMicroService.Core.DomainServices.LoansActions
{
    public class SurveyRepositoryWrite : ISurveyRepositoryWrite
    {
        private readonly ILoanMonitoringDbContext Context;
        private readonly IMapper mapper;
        private readonly IDapperHandler dapperHandler;
        private readonly IDateConvertor dateConvertor;
        private readonly LoginUserDTO loginUser;

        public SurveyRepositoryWrite(ILoanMonitoringDbContext Context, IMapper mapper, IDapperHandler _dapperHandler
            , IDateConvertor _dateConvertor, LoginUserDTO _loginUser)
        {
            this.Context = Context;
            this.mapper = mapper;
            this.dapperHandler = _dapperHandler;
            this.dateConvertor = _dateConvertor;
            this.loginUser = _loginUser;
        }
        public async Task<ResultObject<int>> SetOutsideSurvey(OutsideSurveyVM request)
        {
            var res = new ResultObject<int>();
            var userId = this.loginUser.ID;

            res = ValidateInput(request);
            if (!res.Result) return res;///12051 

            var outsideSurvey = new OutsideSurvey
            {
                SurveyDate = request.SurveyDate,
                PlanActivationTypeId = request.PlanActivationTypeId,
                PlanNoName = request.PlanNoName,
                numberOfJobsObligated = request.numberOfJobsObligated,
                numberOfJobsCreated = request.numberOfJobsCreated,
                numberOfInsurdPerson = request.numberOfInsurdPerson,
                ReasonSubmit = request.ReasonSubmit,
                DeadLineDate = request.DeadLineDate,
                LoanId = request.LoanId,
                CartableId = request.CartableId,
                CreatedBy = userId,
                IsDeleted = false,
                ModifiedBy = null,
                Phone = request.Phone,
                Address = request.Address
            };

            var obj = await this.GetSurveyByLoanId(request.LoanId);
            var guid = Guid.NewGuid();

            var survayDate = this.dateConvertor.ConvertPersianDatetimeToGregorian(request.SurveyDateFa);
            if (survayDate > DateTime.Now)
            {
                res.ServerErrors.Add(new ServerError()
                {
                    Hint = "تاریخ نظارت نمیتواند از تاریخ روز بزرگتر باشد"
                });
                return res;
            }


            var activityLst = this.loginUser.ActivityDtos;

            var outSideSurveyCount = await Context.OutsideSurvey.CountAsync(x => x.LoanId == request.LoanId);

            if (!activityLst.Any(x => x.ID == ConstActivities.SetOutsideSurveyMoreThanLimit) && outSideSurveyCount > 0)
            {
                var AcceptAmount = (await this.Context.LoanContracts.FirstOrDefaultAsync(p => p.LoanId == request.LoanId))?.AcceptAmount ?? 0;

                if (AcceptAmount > 4000000000)
                {
                    res.ServerErrors.Add(new ServerError()
                    {
                        Hint = "امکان ثبت نظارت کمیته برای مبالغ بالاتر از  400 میلیون تومان وجود ندارد",
                    });
                    return res;
                }

            }
            var cartable = await Context.Cartables.Where(c => c.LoanId == request.LoanId && !c.IsDeleted).OrderByDescending(c => c.Id).FirstOrDefaultAsync();
            if (cartable != null)
            {
                //if (cartable.CartableStatusTypeId != CartableStatusTypes.Pending)
                //{
                //    res.ServerErrors.Add(new ServerError()
                //    {
                //        Hint = "ابتدا باید پرونده در وضعیت ارجاع جهت نظارت قرار گیرد",  //"اطلاعات نظارت قبلا ثبت شده است",
                //    });
                //    return res;
                //}

                if (cartable.UserId != loginUser.ID)
                {
                    res.ServerErrors.Add(new ServerError()
                    {
                        Hint = "این پرونده به کارتابل شما ارجاع نشده است",
                    });
                    return res;
                }


                if (cartable.ReferenceDate.Date > survayDate.Date)
                {
                    res.ServerErrors.Add(new ServerError()
                    {
                        Hint = "تاریخ نظارت کمیته وارد شده از تاریخ ارجاع نظارت کوچکتر است ",
                    });
                    return res;
                }
                outsideSurvey.CartableId = cartable.Id;
                cartable.CartableStatusTypeId = 2;
            }
            if (outsideSurvey.Id == 0)
                this.Context.OutsideSurvey.Add(outsideSurvey);
            else
                this.Context.OutsideSurvey.Update(outsideSurvey);

            var loanRW = await this.Context.LoanRedundantViews.FirstOrDefaultAsync(p => p.LoanId == request.LoanId && !p.IsDeleted);
            if (loanRW != null)
            {
                if (cartable.IsNotScheduledReferred.HasValue)
                {
                    loanRW.NotScheduledReferredSurveyCount++;
                }
                else
                {
                    loanRW.OutSideSurveyCount++;
                }

                loanRW.LastOutSideSurveyDate = outsideSurvey.SurveyDate;
            }
            await Context.SaveChangesAsync();
            await SetOutsideSurveyAttachments(request.ImgPath, request.LoanId, userId, outsideSurvey.Id);
            return res;
        }
        private async Task<bool> hasLoanWithPlanNo(int loanId)
        {
            return await this.Context.LoanPlanNo.AnyAsync(c => c.LoanId != null && loanId==c.LoanId && !c.IsDeleted); ;
        }

        public async Task<int> GetSurveyByLoanId(int loanId)
        {
            var obj = await (from s in Context.Surveys
                             join c in Context.Cartables on s.CartableId equals c.Id
                             join l in Context.Loans on c.LoanId equals l.Id
                             where !c.IsDeleted && l.Id == loanId
                             select new Domains.Supervision.Entities.Survey
                             {
                                 Id = s.Id,

                             }).FirstOrDefaultAsync();

            if (obj != null)
            {
                return obj.Id;
            }
            else
            {
                //این نمیدونم صفر بزارم یا ایدی حتما باید باشه سوروی
                return 0;
            }
        }
        public ResultObject<int> ValidateInput(OutsideSurveyVM outsideSurveyDto)
        {
            var res = new ResultObject<int>();
            if (outsideSurveyDto.ImgPath.Count < 5)
            {
                res.ServerErrors.Add(new ServerError()
                {
                    Hint = "تعداد فایل های پیوستی نمی تواند کمتر از پنج تا باشد",
                });
                return res;
            }
            long size = 0;
            if (string.IsNullOrEmpty(outsideSurveyDto.SurveyDateFa) || string.IsNullOrEmpty(outsideSurveyDto.PlanNoName))
            {
                res.ServerErrors.Add(new ServerError()
                {
                    Hint = "فیلدهای ورودی نمی تواند خالی باشد",
                });
                return res;
            }   
            if (string.IsNullOrEmpty(outsideSurveyDto.SurveyDateFa) || string.IsNullOrEmpty(outsideSurveyDto.PlanNoName))
            {
                res.ServerErrors.Add(new ServerError()
                {
                    Hint = "فیلدهای ورودی نمی تواند خالی باشد",
                });
                return res;
            }
            return res;
        }
        public async Task<ResultObject<int>> setOutsideSurvey(OutsideSurveyVM request)
        {
            var result = new ResultObject<int>();
            var userId = this.loginUser.ID;
            var surveyDate = request.SurveyDate;
            if (surveyDate > DateTime.Now)
            {
                result.ServerErrors.Add(new ServerError { Hint = "تاریخ نظارت نمی‌تواند از تاریخ روز بزرگتر باشد", Code = (int)HttpStatusCode.BadRequest });
                return result;
            }

            //// 2. گرفتن کاربر فعلی
            //var userId = _userResolverService.GetCurrentUser(); 

            //// 3. گرفتن فعالیت‌های کاربر
            //var activityList = await _activityService.GetActivitiesAsync(userId);
            //if (!activityList.Any(x => x.Code == ConstActivities.SetOutsideSurveyMoreThanLimit))
            //{
            //    var acceptAmount = await _loanContractsService.GetAcceptAmountByLoanIdAsync(request.LoanId);
            //    if (acceptAmount > 2000000000)
            //    {
            //        result.ServerErrors.Add(new ServerErr
            //        {
            //            Hint = "مبلغ مورد تایید نمی‌تواند بیش از ۲۰۰ میلیون تومان باشد",
            //            Type = ConstErrorType.BusinessError
            //        });
            //        return result;
            //    }
            //}

            //// 4. بررسی وضعیت کارتابل
            //var cartable = await Context.CartableSurvey.FirstOrDefaultAsync(e => e.LoanId == request.LoanId);
            var cartable = await Context.Cartables.Where(c => c.LoanId == request.LoanId && !c.IsDeleted).OrderByDescending(c => c.Id).FirstOrDefaultAsync();

            if (cartable != null)
            {
                if (cartable.CartableStatusTypeId != CartableStatusTypes.Pending)
                {

                    result.ServerErrors.Add(new ServerError { Hint = "ابتدا باید پرونده در وضعیت ارجاع جهت نظارت قرار گیرد", Code = (int)HttpStatusCode.BadRequest });
                    return result;
                }

                if (cartable.ReferenceDate >= surveyDate)
                {

                    result.ServerErrors.Add(new ServerError { Hint = "تاریخ نظارت کمیته وارد شده از تاریخ ارجاع نظارت کوچکتر است", Code = (int)HttpStatusCode.BadRequest });
                    return result;
                }
            }

            var outsideSurvey = new OutsideSurvey
            {
                SurveyDate = request.SurveyDate,
                PlanActivationTypeId = request.PlanActivationTypeId,
                PlanNoName = request.PlanNoName,
                numberOfJobsObligated = request.numberOfJobsObligated,
                numberOfJobsCreated = request.numberOfJobsCreated,
                numberOfInsurdPerson = request.numberOfInsurdPerson,
                ReasonSubmit = request.ReasonSubmit,
                DeadLineDate = request.DeadLineDate,
                LoanId = request.LoanId,
                CartableId = request.CartableId,
                CreatedBy = userId,
                IsDeleted = false,
                ModifiedBy = null,
            };

            Context.OutsideSurvey.Add(outsideSurvey);


            await Context.SaveChangesAsync();

            await UpdateCartableStatusAsync(request.CartableId, 2);


            await SetOutsideSurveyAttachments(request.ImgPath, request.LoanId, userId, outsideSurvey.Id);



            return result;
        }
        private async Task UpdateCartableStatusAsync(int cartableId, byte statusTypeId)
        {
            var item = await this.Context.Cartables.FirstOrDefaultAsync(e => e.Id == cartableId);
            if (item != null)
            {
                item.CartableStatusTypeId = statusTypeId;
            }
            await Context.SaveChangesAsync();
        }
        public async Task SetOutsideSurveyAttachments(List<string> imgPaths, int LoanID, int createdBy, int Id)
        {
            if (imgPaths == null || !imgPaths.Any())
                return;

            var guid = Guid.NewGuid();

            foreach (var path in imgPaths)
            {
                Context.Attachments.Add(new Attachments
                {
                    LoanID = LoanID,
                    AttachmentTypeID = 45,
                    FileName = Path.GetFileName(path),
                    FileType = Path.GetExtension(path).ToLower() == ".png" ? "image/png" : "image/jpeg",
                    PictureName = Path.GetFileNameWithoutExtension(path),
                    Desc = "تصویر نظارت کمیته های شهرستانی و استانی",
                    Guid = guid,
                    Path = path,
                    SurveyId = null,
                    LoanActionID = null,
                    CustomerHeadID = 0,
                    CreateDate = DateTime.Now,
                    CreatedBy = createdBy,
                    EnactmentId = null,
                    OutsideSurveyId = Id
                });
            }
            await Context.SaveChangesAsync();
        }
        public async Task<ResultObject<int>> ConfirmWageMontlyPayment(List<int> userSurveyWageIds)
        {
            var result = new ResultObject<int>();
            var userSurveyWage = await Context.UserSurveyWages.Where(c => userSurveyWageIds.Contains(c.Id)
                && !c.IsDeleted && c.UserSurveyWageStatusTypeId == UserSurveyWageStatusTypeEnum.Computed).ToListAsync();
            if (!userSurveyWage
                .Any(c => c.UserSurveyWageStatusTypeId == UserSurveyWageStatusTypeEnum.Computed))
            {
                result.ServerErrors.Add(new ServerError { Hint = "امکان تایید حق السعی ناظر وجود ندارد" });
                return result;
            }
            List<SurveyWagePayInfo> surveyWagePayInfoList = makeSurveyWagePayInfo(userSurveyWage);
            Context.SurveyWagePayInfos.AddRange(surveyWagePayInfoList);
            result = await Context.SaveChangesAsync();
            return result;
        }
        private List<SurveyWagePayInfo> makeSurveyWagePayInfo(List<UserSurveyWage> userSurveyWageList)
        {
            var userSurveyWageListGroupedList = userSurveyWageListGrouped(userSurveyWageList);
            var surveyWagePayInfoList = new List<SurveyWagePayInfo>();
            foreach (var item in userSurveyWageListGroupedList)
            {
                var listItem = changeUserSurveyWagesStatus(item.ToList());
                var surveyWagePayInfo = mergeUserSurveyWage(listItem);
                surveyWagePayInfo.UserSurveyWages = listItem;
                surveyWagePayInfoList.Add(surveyWagePayInfo);
            }
            return surveyWagePayInfoList;
        }
        private List<IGrouping<int, UserSurveyWage>> userSurveyWageListGrouped(List<UserSurveyWage> userSurveyWageList)
        {
            return userSurveyWageList.GroupBy(c => c.UserId).ToList();
        }
        private List<UserSurveyWage> changeUserSurveyWagesStatus(List<UserSurveyWage> userSurveyWageList)
        {
            userSurveyWageList
                .ForEach(c => c.UserSurveyWageStatusTypeId = UserSurveyWageStatusTypeEnum.PaymentRequest);
            return userSurveyWageList;
        }
        private SurveyWagePayInfo mergeUserSurveyWage(List<UserSurveyWage> groupedUserSurveyWageList)
        {
            var config = this.Context.SurveyConfigs.FirstOrDefault();
            var pm = DateTime.Now.ComputePaymentMountNumber(config.PaymentMonthDeadline);
            System.Globalization.PersianCalendar pc = new System.Globalization.PersianCalendar();
            var surveyWagePayInfo = new SurveyWagePayInfo();
            surveyWagePayInfo.LongDistance = groupedUserSurveyWageList.Sum(c => c.LongDistance);
            surveyWagePayInfo.ShortDistancePointCount = groupedUserSurveyWageList.Sum(c => c.ShortDistancePointCount);
            surveyWagePayInfo.WageAmount = groupedUserSurveyWageList.Sum(c => c.WageAmount);
            surveyWagePayInfo.PointCount = groupedUserSurveyWageList.Sum(c => c.PointCount);
            surveyWagePayInfo.Distance = groupedUserSurveyWageList.Sum(c => c.Distance);
            surveyWagePayInfo.UserId = groupedUserSurveyWageList.FirstOrDefault().UserId;
            surveyWagePayInfo.PaymentDate = DateTime.Now;
            surveyWagePayInfo.CreateDate = DateTime.Now;
            surveyWagePayInfo.CreatedBy = this.loginUser.ID;
            surveyWagePayInfo.PaymentMonth = pm;
            surveyWagePayInfo.PaymentYear = pc.GetYear(DateTime.Now);
            return surveyWagePayInfo;
        }
        public async Task<ResultObject<int>> SetSurveyLoanAmountCondition(SurveyReferenceLoanAmountVM request)
        {
            var resultObject = new ResultObject<int>();
            var surveyReferenceLoanAmount = await Context.SurveyReferenceLoanAmounts
                .Where(c => c.Id == request.Id && !c.IsDeleted)
                .FirstOrDefaultAsync();

            if (surveyReferenceLoanAmount != null)
            {
                var existingActiveRecords = await Context.SurveyReferenceLoanAmounts
                    .Where(c => c.SurveyReferenceBaseInfoId == request.SurveyReferenceBaseInfoId && c.IsActive && !c.IsDeleted)
                    .ToListAsync();
                foreach (var rec in existingActiveRecords)
                {
                    rec.IsActive = false;
                    rec.UpdateDate = DateTime.Now;
                    rec.ModifiedBy = loginUser.ID;
                }
                //surveyReferenceLoanAmount.AmountMin = request.AmountMin;
                //surveyReferenceLoanAmount.AmountMax = request.AmountMax;
                //surveyReferenceLoanAmount.LoanMinorTypeId = request.LoanMinorTypeId;
                surveyReferenceLoanAmount.IsActive = request.IsActive;
                surveyReferenceLoanAmount.UpdateDate = DateTime.Now;
                surveyReferenceLoanAmount.ModifiedBy = loginUser.ID;
            }
            else
            {
                var existingActiveRecords = await Context.SurveyReferenceLoanAmounts
                    .Where(c => c.SurveyReferenceBaseInfoId == request.SurveyReferenceBaseInfoId && c.IsActive && !c.IsDeleted)
                    .ToListAsync();

                foreach (var rec in existingActiveRecords)
                {
                    rec.IsActive = false;
                    rec.UpdateDate = DateTime.Now;
                    rec.ModifiedBy = loginUser.ID;
                }
                surveyReferenceLoanAmount = new SurveyReferenceLoanAmount
                {
                    AmountMin = request.AmountMin,
                    AmountMax = request.AmountMax,
                    LoanMinorTypeId = request.LoanMinorTypeId,
                    SurveyReferenceBaseInfoId = request.SurveyReferenceBaseInfoId,
                    IsActive = true,
                    IsDeleted = false,
                    CreateDate = DateTime.Now,
                    CreatedBy = loginUser.ID
                };
                Context.SurveyReferenceLoanAmounts.Add(surveyReferenceLoanAmount);
            }
            await Context.SaveChangesAsync();
            return resultObject;
        }
        public async Task<ResultObject<int>> SetSurveyReferenceContractDateCondition(SurveyReferenceContractDateVM request)
        {
            var resultObject = new ResultObject<int>();
            var surveyReferenceContractDate = await Context.SurveyReferenceContractDates
                 .Where(c => c.Id == request.Id && !c.IsDeleted).FirstOrDefaultAsync();

            if (surveyReferenceContractDate != null)
            {
                var existingActiveRecords = await Context.SurveyReferenceContractDates
                    .Where(c => c.SurveyReferenceBaseInfoId == request.SurveyReferenceBaseInfoId && c.IsActive && !c.IsDeleted)
                    .ToListAsync();
                foreach (var rec in existingActiveRecords)
                {
                    rec.IsActive = false;
                    rec.UpdateDate = DateTime.Now;
                    rec.ModifiedBy = loginUser.ID;
                }
                surveyReferenceContractDate.IsActive = request.IsActive;
                surveyReferenceContractDate.UpdateDate = DateTime.Now;
                surveyReferenceContractDate.ModifiedBy = loginUser.ID;
            }
            else
            {
                var existingActiveRecords = await Context.SurveyReferenceContractDates
                    .Where(c => c.SurveyReferenceBaseInfoId == request.SurveyReferenceBaseInfoId && c.IsActive && !c.IsDeleted)
                    .ToListAsync();

                foreach (var rec in existingActiveRecords)
                {
                    rec.IsActive = false;
                    rec.UpdateDate = DateTime.Now;
                    rec.ModifiedBy = loginUser.ID;
                }
                surveyReferenceContractDate = new SurveyReferenceContractDate
                {
                    BeginDate = request.BeginDate,
                    EndDate = request.EndDate,
                    LoanMinorTypeId = request.LoanMinorTypeId,
                    SurveyReferenceBaseInfoId = request.SurveyReferenceBaseInfoId,
                    IsActive = request.IsActive,
                    IsDeleted = false,
                    CreateDate = DateTime.Now,
                    CreatedBy = loginUser.ID
                };
                Context.SurveyReferenceContractDates.Add(surveyReferenceContractDate);
            }


            await Context.SaveChangesAsync();
            return resultObject;
        }
        public async Task<ResultObject<int>> SetSurveyReferenceReagentCondition(SurveyReferenceReagentVM request)
        {
            var resultObject = new ResultObject<int>();
            var surveyReferenceReagent = await Context.SurveyReferenceReagents
                 .Where(c => c.Id == request.Id && !c.IsDeleted).FirstOrDefaultAsync();

            if (surveyReferenceReagent != null)
            {
                request.CustomerId = surveyReferenceReagent.CustomerId;
                surveyReferenceReagent.IsActive = request.IsActive;
                surveyReferenceReagent.UpdateDate = DateTime.Now;
                surveyReferenceReagent.ModifiedBy = loginUser.ID;
            }
            else
            {
                var existingActiveRecords = await Context.SurveyReferenceReagents
                    .Where(c => c.SurveyReferenceBaseInfoId == request.SurveyReferenceBaseInfoId && c.IsActive && !c.IsDeleted && c.CustomerId == request.CustomerId)
                    .ToListAsync();


                if (existingActiveRecords == null||existingActiveRecords.Count()==0) { 
                    surveyReferenceReagent = new SurveyReferenceReagent
                    {
                        CustomerId = request.CustomerId,

                        LoanMinorTypeId = request.LoanMinorTypeId,
                        SurveyReferenceBaseInfoId = request.SurveyReferenceBaseInfoId,
                        IsActive = request.IsActive,
                        IsDeleted = false,
                        CreateDate = DateTime.Now,
                        CreatedBy = loginUser.ID
                    };
                Context.SurveyReferenceReagents.Add(surveyReferenceReagent);
                }
            }
            await Context.SaveChangesAsync();
            return resultObject;
        }
        public async Task<ResultObject<int>> SetAllowedFirstTimeSupervisionRole(AllowedFirstTimeSupervisionRoleVM request)
        {
            var resultObject = new ResultObject<int>();
            var allowedFirstTimeSupervisionRole = await Context.AllowedFirstTimeSupervisionRoles
                 .Where(c => c.Id == request.Id && !c.IsDeleted).FirstOrDefaultAsync();

            if (allowedFirstTimeSupervisionRole != null)
            {
                allowedFirstTimeSupervisionRole.IsActive = request.IsActive;
                allowedFirstTimeSupervisionRole.UpdateDate = DateTime.Now;
                allowedFirstTimeSupervisionRole.ModifiedBy = loginUser.ID;
            }
            else
            {
                allowedFirstTimeSupervisionRole = new AllowedFirstTimeSupervisionRole
                {
                    RoleId = request.RoleId,
                    LoanMinorTypeId = request.LoanMinorTypeId,
                    SurveyReferenceBaseInfoId = request.SurveyReferenceBaseInfoId,
                    IsDeleted = false,
                    CreateDate = DateTime.Now,
                    CreatedBy = loginUser.ID
                };
                Context.AllowedFirstTimeSupervisionRoles.Add(allowedFirstTimeSupervisionRole);
            }
            await Context.SaveChangesAsync();
            return resultObject;
        }
        public async Task<ResultObject<int>> RemoveSurveyReferenceReagentCondition(int Id)
        {
            var resultObject = new ResultObject<int>();
            var surveyReferenceReagents = await Context.SurveyReferenceReagents
                 .Where(c => c.Id == Id && !c.IsDeleted).FirstOrDefaultAsync();

            if (surveyReferenceReagents != null)
            {
                surveyReferenceReagents.IsDeleted = true;
            }
            await Context.SaveChangesAsync();
            return resultObject;
        }
        public async Task<ResultObject<int>> RemoveSurveyLoanAmountCondition(int Id)
        {
            var resultObject = new ResultObject<int>();
            var surveyReferenceLoanAmounts = await Context.SurveyReferenceLoanAmounts
                 .Where(c => c.Id == Id && !c.IsDeleted).FirstOrDefaultAsync();

            if (surveyReferenceLoanAmounts != null)
            {
                surveyReferenceLoanAmounts.IsDeleted = true;
            }
            await Context.SaveChangesAsync();
            return resultObject;
        }
        public async Task<ResultObject<int>> RemoveAllowedFirstTimeSupervisionRole(int Id)
        {
            var resultObject = new ResultObject<int>();
            var allowedFirstTimeSupervisionRole = await Context.AllowedFirstTimeSupervisionRoles
                 .Where(c => c.Id == Id && !c.IsDeleted).FirstOrDefaultAsync();

            if (allowedFirstTimeSupervisionRole != null)
            {
                allowedFirstTimeSupervisionRole.IsDeleted = true;
            }
            await Context.SaveChangesAsync();
            return resultObject;
        }
        public async Task<ResultObject<int>> RemoveSurveyReferenceContractDateCondition(int Id)
        {
            var resultObject = new ResultObject<int>();
            var surveyReferenceContractDates = await Context.SurveyReferenceContractDates
                 .Where(c => c.Id == Id && !c.IsDeleted).FirstOrDefaultAsync();

            if (surveyReferenceContractDates != null)
            {
                surveyReferenceContractDates.IsDeleted = true;
            }
            await Context.SaveChangesAsync();
            return resultObject;
        }
        public async Task<ResultObject<int>> SetSurveyBaseInfo(LoanMinorTypeVM loanMinorTypes)
        {
            var resultObject = new ResultObject<int>();

            foreach (var item in loanMinorTypes.Id)
            {
                var exSurveyBaseInfo = await Context.SurveyReferenceBaseInfos.Where(a => a.LoanMinorTypeId == item && !a.IsDeleted).ToListAsync();

                if (!exSurveyBaseInfo.Any())
                {
                    var surveyBaseInfo = new SurveyReferenceBaseInfo
                    {
                        IsActive = true,
                        CreateDate = DateTime.Now,
                        CreatedBy = loginUser.ID,
                        IsDeleted = false,
                        LoanMinorTypeId = item
                    };
                    Context.SurveyReferenceBaseInfos.Add(surveyBaseInfo);
                }
            }
            await Context.SaveChangesAsync();
            return resultObject;
        }
        public async Task<ResultObject<int>> RemoveSurveyBaseInfo(int Id)
        {
            var resultObject = new ResultObject<int>();
            var exSurveyBaseInfo = await Context.SurveyReferenceBaseInfos.Where(a => a.Id == Id).FirstOrDefaultAsync();
            if (exSurveyBaseInfo != null)
            {
                exSurveyBaseInfo.IsDeleted = true;
            }
            await Context.SaveChangesAsync();
            return resultObject;
        }
        public async Task<ResultObject<int>> ActiveSurveyBaseInfo(int Id, bool Active)
        {
            var resultObject = new ResultObject<int>();
            var exSurveyBaseInfo = await Context.SurveyReferenceBaseInfos.Where(a => a.Id == Id).FirstOrDefaultAsync();
            if (exSurveyBaseInfo != null)
            {
                exSurveyBaseInfo.IsActive = Active;
            }
            await Context.SaveChangesAsync();
            return resultObject;
        }
        public async Task<ResultObject<int>> SetSurveyMaxCountCondition(SurveyReferenceMaxCountVM request)
        {
            var resultObject = new ResultObject<int>();
            var surveyReferenceMaxCount = await Context.SurveyReferenceMaxCounts
                 .Where(c => c.Id == request.Id && !c.IsDeleted).FirstOrDefaultAsync();
            if (surveyReferenceMaxCount != null)
            {

                surveyReferenceMaxCount.IsActive = request.IsActive;
                surveyReferenceMaxCount.UpdateDate = DateTime.Now;
                surveyReferenceMaxCount.ModifiedBy = loginUser.ID;
            }
            else
            {
                surveyReferenceMaxCount = new SurveyMaxCount
                {
                    LoanAmountMax = request.LoanAmountMax,
                    LoanAmountMin = request.LoanAmountMin,
                    MaxReferenceCountByYear = request.MaxReferenceCountByYear,
                    IsDeleted = false,
                    CreateDate = DateTime.Now,
                    CreatedBy = loginUser.ID
                };
                Context.SurveyReferenceMaxCounts.Add(surveyReferenceMaxCount);
            }
            await Context.SaveChangesAsync();
            return resultObject;
        }
        public async Task<ResultObject<int>> RemoveSurveyMaxCountCondition(int Id)
        {
            var resultObject = new ResultObject<int>();
            var surveyReferenceContractDates = await Context.SurveyReferenceMaxCounts
                 .Where(c => c.Id == Id && !c.IsDeleted).FirstOrDefaultAsync();

            if (surveyReferenceContractDates != null)
            {
                surveyReferenceContractDates.IsDeleted = true;
            }
            await Context.SaveChangesAsync();
            return resultObject;
        }

        public async Task<ResultObject<int>> ConfirmSurveyAddressRequest(int Id, int SurveyId)
        {
            var resultObject = new ResultObject<int>();
            var changeLoanPlanState = await Context.ChangeLoanPlanStates
                .Where(c => c.ID == Id && !c.IsDeleted).FirstOrDefaultAsync();
            if (changeLoanPlanState != null)
            {
                var states = await this.Context.ChangeLoanPlanStates.Where(p => p.LoanId == changeLoanPlanState.LoanId
                //&& !p.VerifyBy.HasValue 
                && !p.IsDeleted).ToListAsync();
                var state = states.OrderByDescending(p => p.RequestDate).FirstOrDefault();
                foreach (var s in states)
                {
                    if (s.ID != state.ID)
                        s.IsDeleted = true;
                }
                state.VerifyDate = DateTime.Now;
                state.VerifyBy = loginUser.ID;
                state.IsConfirmed = true;

                var survey = await Context.Surveys.FirstOrDefaultAsync(e => e.Id == SurveyId);

                var loanPlan = await Context.LoanPlans.FirstOrDefaultAsync(e => e.LoanId == state.LoanId);
                if (loanPlan != null && survey != null)
                {
                    loanPlan.Latitude = survey.Latitude;
                    loanPlan.Longitude = survey.Longitude;
                    survey.IsLocationConfirmed = true;
                }

                this.Context.LoanActionLogs.Add(new LoanActionLog
                {
                    Title = LoanActionLogTypeEnum.VerifyChangeLoanPlanStateRequest,
                    ActionType = LoanActionLogTypeEnum.Modify,
                    Description = "",
                    loanId = changeLoanPlanState.LoanId,
                    CreateDate = DateTime.Now,
                    CreatedBy = loginUser.ID
                });
                await Context.SaveChangesAsync();
                return resultObject;
            }

            return resultObject;
        }

        public async Task<ResultObject<int>> RejectSurveyAddressRequest(int Id, string Desc)
        {
            var resultObject = new ResultObject<int>();
            var changeLoanPlanState = await Context.ChangeLoanPlanStates
                .Where(c => c.ID == Id && !c.IsDeleted).FirstOrDefaultAsync();
            var loanPlan = await Context.LoanPlans.FirstOrDefaultAsync(e => e.LoanId == changeLoanPlanState.LoanId);

            if (changeLoanPlanState != null)
            {
                changeLoanPlanState.VerifyDate = DateTime.Now;
                changeLoanPlanState.VerifyBy = loginUser.ID;
                changeLoanPlanState.IsConfirmed = false;
                changeLoanPlanState.Description = Desc;

                this.Context.LoanActionLogs.Add(new LoanActionLog
                {
                    Title = LoanActionLogTypeEnum.NotVerifyChangeLoanPlanStateRequest,
                    ActionType = LoanActionLogTypeEnum.Modify,
                    Description = Desc,
                    loanId = changeLoanPlanState.LoanId,
                    CreateDate = DateTime.Now,
                    CreatedBy = loginUser.ID
                });
                this.Context.ChangLoanPlanHistories.Add(new ChangLoanPlanStateHistory
                {
                    ChangeLoanPlanStateId = changeLoanPlanState.ID,
                    CreatedBy = loginUser.ID,
                    CreatedDate = DateTime.Now,
                });
                await Context.SaveChangesAsync();
                return resultObject;
            }

            return resultObject;
        }
    }
}