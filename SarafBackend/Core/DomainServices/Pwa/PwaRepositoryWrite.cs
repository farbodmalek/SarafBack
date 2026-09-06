using AutoMapper;
using Azure.Core;
using CommonLibrary.Common.Maps;
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
using CommonLibrary.Domains.Core.Supervision.Entities;
using LoanMonitoringMicroService.Application.DTO.Pwa;
using LoanMonitoringMicroService.Application.DTO.Survey;
using LoanMonitoringMicroService.Applications.AppService.ServiceDto.Survey;
using LoanMonitoringMicroService.Core.Domain.RepositoryInterfaces;
using LoanMonitoringMicroService.Core.Domain.ViewModel.PlanNo;
using LoanMonitoringMicroService.Core.Domain.ViewModel.Pwa;
using LoanMonitoringMicroService.Core.Domain.ViewModel.Survey;
using LoanMonitoringMicroService.Core.DomainServices.Interfaces;
using LoanMonitoringMicroService.Domains.Core.Supervision.Entities;
using LoanMonitoringMicroService.Domains.Supervision.Entities;
using Microsoft.EntityFrameworkCore;
using Stimulsoft.System.Data;
using System.Net.Mail;
using System.Transactions;


namespace LoanMonitoringMicroService.Core.DomainServices.Cartables
{
    public class PwaRepositoryWrite : IPwaRepositoryWrite
    {
        private readonly IDapperHandler dapperHandler;
        private readonly IDateConvertor dateConvertor;
        private readonly ILoanMonitoringDbContext context;
        private readonly LoginUserDTO loginUser;
        private readonly IMapper mapper;

        public PwaRepositoryWrite(IDapperHandler _dapperHandler, IDateConvertor _dateConvertor
            , ILoanMonitoringDbContext LoanDbContext, LoginUserDTO _loginUser, IMapper _mapper)
        {
            dapperHandler = _dapperHandler;
            dateConvertor = _dateConvertor;
            context = LoanDbContext;
            this.loginUser = _loginUser;
            mapper = _mapper;
        }

        public async Task<ResultObject<int>> SetLoanPlanSurvey(PwaSurveyLoanPlanVM request)
        {
            var result = new ResultObject<int>();
            var cartable = context.Cartables.FirstOrDefault(e => e.Id == request.Survey.CartableId);
            if (
                request.Survey.PlanGardenSurvey == null &&
                request.Survey.PlanLivestockSurvey == null &&
                request.Survey.PlanIndustrialSurvey == null &&
                request.Survey.PlanServiceSurvey == null
                )
            {

                result.ServerErrors.Add(new ServerError()
                {
                    Hint = "لطفا همه اطلاعات طرح را وارد کنید",

                });
                return result;
            }
            if (cartable == null)
                return result;
            if (cartable.CartableStatusTypeId != CartableStatusTypes.Pending)
            {
                result.ServerErrors.Add(new ServerError()
                {
                    Hint = "اطلاعات نظارت قبلا ثبت شده است",
                });
                return result;
            }

            if (!string.IsNullOrWhiteSpace(request.LoanPlan.VillageName)
                && request.LoanPlan.VillageName.Length > 50)
            {
                result.ServerErrors.Add(new ServerError()
                {
                    Hint = "نام روستا اشتباه وارد شده است",
                });
                return result;
            }
            if (!request.LoanPlan.IsValidPlanNo && cartable.PlanNoId.Value != 0)
            {
                request.LoanPlan.PlanNoId = cartable.PlanNoId.Value;
            }
            else
            {
                this.context.PlanNoHistories.Add(new PlanNoHistory
                {
                    LoanId = request.LoanPlan.LoanId,
                    OtherPlanNo = "",
                    RequestDate = DateTime.Now,
                    PlanNoId = request.LoanPlan.PlanNoId,
                    RequestUserId = this.loginUser.ID,
                    PlanNoStatusId=1,
                    CartableId = request.Survey.CartableId

                });
                request.LoanPlan.PlanNoId = request.LoanPlan.PlanNoId;
                request.LoanPlan.OtherPlanNo = "";
                request.LoanPlanNo.UserPlanNoText = request.LoanPlanNo.UserPlanNoText;
                request.LoanPlanNo.UserOtherPlanNo = request.LoanPlanNo.UserOtherPlanNo;
                var planNo = await this.context.LoanPlanNo.FirstOrDefaultAsync(c => c.LoanId == cartable.LoanId && !c.IsDeleted);
                if (planNo != null)
                    planNo.IsDeleted = true;
                this.context.LoanPlanNo.Add(
                    new LoanPlanNo
                    {
                        PlanNoId = request.LoanPlan.PlanNoId,
                        LoanSurveyEconomicTypeId = 12,
                        LoanId = cartable.LoanId,
                        UserOtherPlanNo = request.LoanPlanNo.UserOtherPlanNo,
                        UserPlanNoText = request.LoanPlanNo.UserPlanNoText,
                        CreateDate = DateTime.Now,
                        CreatedBy = this.loginUser.ID
                    });
            }
            var canSetSurvey = CanSetSurvey(request, cartable);
            if (canSetSurvey.ServerErrors.Count > 0)
                return canSetSurvey;


            var currentLoanPlan = context.LoanPlans.FirstOrDefault(e => e.ID == request.LoanPlan.Id);

            if (currentLoanPlan != null)
            {
                if (isChangeLoanPlanLocation(currentLoanPlan, request.LoanPlan) && 
                    (checkDistsnce(currentLoanPlan.Latitude, currentLoanPlan.Longitude, request.LoanPlan.Latitude, request.LoanPlan.Longitude, 500)))
                {
                    await SetChangeLoanPlanStateRequestAsync(currentLoanPlan, request.LoanPlan.CartableId);
                    request.Survey.IsLocationConfirmed = false;
                }
                else
                    request.Survey.IsLocationConfirmed = !isChangeSurveyLocation(currentLoanPlan, request.Survey);
            }
            else
            {
                request.Survey.IsLocationConfirmed = true;
            }

            var Cartable = await SetCartable(cartable);


            if (!await CheckSurveyUniq(request.Survey.CartableId, request.Survey.SurveyDate))
            {
                result.ServerErrors.Add(new ServerError()
                {
                    Hint = "نظارت تکراری می باشد",
                });
                return result;
            }

            var newSurveyId = await SetSurvey(request.Survey);
            if (newSurveyId <= 0)
                throw new Exception("Survey ثبت نشد و SurveyId معتبر دریافت نشد.");
            var newLoanPlan = await SetLoanPlan(request.LoanPlan);
            await SetAttachment(request.Survey.GuidList, newSurveyId);
            await updateLoan(cartable.LoanId);
            this.context.LoanActionLogs.Add(new LoanActionLog
            {
                Title = LoanActionLogTypeEnum.NewSurveyUser,
                ActionType = LoanActionLogTypeEnum.Insert,
                Description = LoanActionLogTypeEnum.NewSurveyUser,
                loanId = cartable.LoanId,
                CreateDate = DateTime.Now,
                CreatedBy = loginUser.ID,
            });
            this.context.LoanActionLogs.Add(new LoanActionLog
            {
                Title = LoanActionLogTypeEnum.NewLoanPlan,
                ActionType = LoanActionLogTypeEnum.Insert,
                Description = request.LoanPlan.Id > 0 ? LoanActionLogTypeEnum.EditLoanPlan : LoanActionLogTypeEnum.NewLoanPlan,
                loanId = request.LoanPlan.LoanId,
                CreateDate = DateTime.Now,
                CreatedBy = loginUser.ID,
            });
            await fillSurveyInfo(request.LoanPlan.LoanId, request.Survey.SurveyDate);
            await this.context.SaveChangesAsync();
            return result;
        }

        private ResultObject<int> CanSetSurvey(PwaSurveyLoanPlanVM surveyLoanPlan, Cartable cartable)
        {
            var result = new ResultObject<int>();
            if (cartable == null)
            {
                result.ServerErrors.Add(new ServerError() { Hint = "کارتابلی برای این پرونده وجود ندارد", });
                return result;
            }
            if (cartable.ExpireDate.HasValue && cartable.ExpireDate.Value.Date < DateTime.Now.Date)
            {
                result.ServerErrors.Add(new ServerError()
                {
                    Hint = "مهلت اقدام منقضی شده است",

                });
                return result;
            }
            if (IsExistLoanPlan(surveyLoanPlan.LoanPlan.LoanId) && surveyLoanPlan.LoanPlan.Id == 0)
            {
                result.ServerErrors.Add(new ServerError() { Hint = "اطلاعات طرح وام قبلا ثبت شده است", });
                return result;
            }
            //if (CheckDistsnce(surveyLoanPlan.Survey.Latitude, surveyLoanPlan.Survey.Longitude, surveyLoanPlan.LoanPlan.Latitude, surveyLoanPlan.LoanPlan.Longitude))
            //{
            //    result.ServerErrorors.Add(new ServerError() { Hint = "فاصله محل نظارت و محل ثبت طرح بیش از حد مجاز", Type = ConstErrorType.DistanceError });
            //    return result;
            //}
            if (surveyLoanPlan.Survey.PlanActivationTypeId == 2 && surveyLoanPlan.Survey.GuidList.Count < 2)
            {
                result.ServerErrors.Add(new ServerError()
                {
                    Hint = "حداقل 2 عکس باید بارگذاری شود",

                });
                return result;
            }
            else if (surveyLoanPlan.Survey.GuidList.Count < 5)
            {
                result.ServerErrors.Add(new ServerError()
                {
                    Hint = "حداقل 5 عکس باید بارگذاری شود",

                });
            }
            return result;
        }

        private bool isChangeLoanPlanLocation(LoanPlan currentLoanPlan, PwaLoanplanVM loanPlan)
        {
            
            var e= !(currentLoanPlan.Latitude == loanPlan.Latitude &&
                 currentLoanPlan.Longitude == loanPlan.Longitude &&
                 currentLoanPlan.Address == loanPlan.Address);
            return  e;  
        }

        private bool isChangeSurveyLocation(LoanPlan currentLoanPlan, PwaSurveyVM survey)
        {
            return (checkDistsnce(currentLoanPlan.Latitude, currentLoanPlan.Longitude, survey.Latitude, survey.Longitude, 500));
        }

        private bool checkDistsnce(string startLatitude,
            string startLongitude,
            string endLatitude,
            string endLongitude,
            int maxDistance)
        {
            var distance = GeoCoordinates.Instance.ComputeDistance(startLatitude, startLongitude, endLatitude, endLongitude);
            return (distance > Convert.ToDouble(maxDistance) / 1000);
        }

        public async Task<bool> SetCartable(Cartable cartable)
        {
            cartable.CartableStatusTypeId = 2;
            cartable.EndDate = DateTime.Now;
            return true;
        }

        public async Task<bool> CheckSurveyUniq(int cartableId, DateTime SurveyDate)
        {
            DateTime dt = SurveyDate.AddDays(-1);

            var res = context.Surveys.Where(x => !x.IsDeleted && x.CartableId == cartableId && x.SurveyDate > dt).Count();
            if (res == 0) return true;
            return false;
        }

        public async Task<int> SetSurvey(PwaSurveyVM request)
        {
            var survey = await context.Surveys.FirstOrDefaultAsync(e => e.Id == request.Id);
            survey = new Domains.Supervision.Entities.Survey
            {
                SurveyStatusTypeId = SurveyStatusTypeEnum.Supervised,
                SurveyRateBaseInfoId = 1,
                IsEquipmentBought = request.IsEquipmentBought,
                Longitude = request.Longitude,
                Latitude = request.Latitude,
                NumberOfInsurdPerson = request.NumberOfInsurdPerson,
                EndOfActivationDate = request.EndOfActivationDate,
                ConstructionApproval = request.ConstructionApproval,
                ConstructionDescription = request.ConstructionDescription,
                ConstructionPercentageProgress = request.ConstructionPercentageProgress,
                IsFactorMatch = request.IsFactorMatch,
                EquipmentDescription = request.EquipmentDescription,
                SurveyReport = request.SurveyReport,
                CustomerOffer = request.CustomerOffer,
                NumberOfJobsCreated = request.NumberOfJobsCreated,
                EquipmentTypeId = request.EquipmentTypeId,
                PlanActivationTypeId = request.PlanActivationTypeId,
                CartableId = request.CartableId,
                UpdateDate = DateTime.Now,
                CreateDate = DateTime.Now,
                CreatedBy = loginUser.ID,
                SurveyDate = request.SurveyDate,
            };
            if (request.PlanLivestockSurvey != null)
            {
                var mappedLivestock = mapper.Map<PlanLivestockSurvey>(request.PlanLivestockSurvey);
                survey.PlanLivestockSurvey.Add(mappedLivestock);
            }
            if (request.PlanServiceSurvey != null)
            {
                var mappedService = mapper.Map<PlanServiceSurvey>(request.PlanServiceSurvey);
                survey.PlanServiceSurvey.Add(mappedService);
            }
            if (request.PlanIndustrialSurvey != null)
            {
                var mappedIndustrial = mapper.Map<PlanIndustrialSurvey>(request.PlanIndustrialSurvey);
                survey.PlanIndustrialSurvey.Add(mappedIndustrial);
            }
            if (request.PlanGardenSurvey != null)
            {
                var mappedGarden = mapper.Map<PlanGardenSurvey>(request.PlanGardenSurvey);
                survey.PlanGardenSurvey.Add(mappedGarden);
            }

            context.Surveys.Add(survey);
            try
            {
                await this.context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                //string messageError = Newtonsoft.Json.JsonConvert.SerializeObject(survey) + " -> \n" + ex.Message + " -> \n" + ex.InnerException;
                //_loanActionLogService.CreateLoanActionLog(new LoanActionLog
                //{
                //    Title = LoanActionLogTypeEnum.Error,
                //    ActionType = LoanActionLogTypeEnum.Insert,
                //    Description = messageError,
                //    loanId = request.LoanPlan.LoanId,
                //});
                //result.ServerErrors.Add(new ServerError()
                //{
                //    Hint = "بروز خطا هنگام ثبت نظارت" + " -> \n" + ex.Message + " -> \n" + ex.InnerException,

                //});
            }
            return survey.Id;
        }
        public async Task<LoanPlan> SetLoanPlan(PwaLoanplanVM request)
        {
            var loanPlan = await context.LoanPlans.FirstOrDefaultAsync(e => e.ID == request.Id);
            if (loanPlan != null)
            {
                loanPlan.Phone = request.Phone;
                loanPlan.Address = request.Address;
                loanPlan.EducationTypeId = request.EducationTypeId;
                loanPlan.GenderType = request.GenderType;
                loanPlan.InsuranceTypeId = request.InsuranceTypeId;
                loanPlan.IsFamilySupervisor = request.IsFamilySupervisor;
                //loanPlan.Latitude = request.Latitude;
                loanPlan.LoanSurveyEconomidTypeId = request.LoanSurveyEconomidTypeId;
                //loanPlan.Longitude = request.Longitude;
                loanPlan.MaritalStatusId = request.MaritalStatusId;
                loanPlan.MobileNo = request.MobileNo;
                loanPlan.PlanNoId = request.PlanNoId;
                loanPlan.PlanTypeId = request.PlanTypeId;
                loanPlan.ResidentTypeId = request.ResidentTypeId;
                loanPlan.WorkshopCode = request.WorkshopCode;
                loanPlan.LoanSystemTypeId = 1;
                loanPlan.CartableId = loanPlan.CartableId;
            }
            else
            {
                loanPlan = new LoanPlan
                {
                    Phone = request.Phone,
                    Address = request.Address,
                    EducationTypeId = request.EducationTypeId,
                    GenderType = request.GenderType,
                    InsuranceTypeId = request.InsuranceTypeId,
                    IsFamilySupervisor = request.IsFamilySupervisor,
                    Latitude = request.Latitude,
                    LoanSurveyEconomidTypeId = request.LoanSurveyEconomidTypeId,
                    Longitude = request.Longitude,
                    MaritalStatusId = request.MaritalStatusId,
                    MobileNo = request.MobileNo,
                    PlanNoId = request.PlanNoId,
                    PlanTypeId = request.PlanTypeId,
                    ResidentTypeId = request.ResidentTypeId,
                    WorkshopCode = request.WorkshopCode,
                    LoanSystemTypeId = 1,
                    CartableId = request.CartableId,
                    LoanId = request.LoanId,
                    CreateDate = DateTime.Now,
                    IsDeleted = false,
                    CreatedBy = loginUser.ID
                };
                context.LoanPlans.Add(loanPlan);
            }
            return loanPlan;
        }
        public bool IsExistLoanPlan(int loanId)
        {
            return context.LoanPlans.Any(l => l.LoanId == loanId && !l.IsDeleted);
        }
        public async Task<ResultObject<int>> SetChangeLoanPlanStateRequestAsync(LoanPlan loanPlan, int cartableId)
        {
            var resultObject = new ResultObject<int>();
            var loanId = loanPlan.LoanId;
            var states = await this.context.ChangeLoanPlanStates.Where(p => p.LoanId == loanPlan.LoanId && !p.VerifyBy.HasValue && !p.IsDeleted).ToListAsync();
            foreach (var state in states)
                state.IsDeleted = true;
            var changeLoanPlanState = new ChangeLoanPlanState();
            changeLoanPlanState = new ChangeLoanPlanState
            {
                LoanId = loanId,
                LoanPlanId = loanPlan.ID,
                RequestBy = loginUser.ID,
                RequestDate = DateTime.Now,

            };
            changeLoanPlanState.LoanPlanLocationStates = new List<LoanPlanLocationState>();
            changeLoanPlanState.LoanPlanLocationStates.Add(new LoanPlanLocationState
            {
                LoanPlanId = loanPlan.ID,
                Address = loanPlan.Address,
                Latitude = loanPlan.Latitude,
                Longitude = loanPlan.Longitude,
                CartableId = cartableId
            });

            context.ChangeLoanPlanStates.Add(changeLoanPlanState);  
            await context.SaveChangesAsync();

            return resultObject;
        }
        public async Task SetChangeLoanPlanState(ChangeLoanPlanState changeLoanPlanState)
        {
            if (changeLoanPlanState.ID == 0)
            {
                var loanPlanStates = await context.ChangeLoanPlanStates
                    .Where(a => a.LoanId == changeLoanPlanState.LoanId
                                && !a.IsDeleted
                                && a.VerifyBy == null
                                && a.VerifyDate == null)
                    .ToListAsync();

                foreach (var loanPlanState in loanPlanStates)
                {
                    loanPlanState.IsDeleted = true;
                }
                await context.ChangeLoanPlanStates.AddAsync(changeLoanPlanState);

            }
        }
        private async Task updateLoan(int loanId)
        {
            var loan = await context.Loans.FirstOrDefaultAsync(e => e.Id == loanId);
            loan.LastSurveyDate = DateTime.Now;
            if (loan.LoanStatusTypeId != "E")
                loan.SurveyLevel++;
        }
        private async Task fillSurveyInfo(int loanId, DateTime surveyDate)
        {
            var loanRedundantView = await context.LoanRedundantViews.FirstOrDefaultAsync(e => e.LoanId == loanId);
            loanRedundantView.LastSurveyDate = DateTime.Now;
            loanRedundantView.SurveyCount++;
            var loan = await context.Loans.FirstOrDefaultAsync(e => e.Id == loanId);
            if (loan.LoanStatusTypeId != "E")
                loanRedundantView.SurveyLevel = loanRedundantView.SurveyLevel.HasValue ? loanRedundantView.SurveyLevel + 1 : 1;

        }

        public async Task SetAttachment(List<string> attachments, int surveyId)
        {
            foreach (var item in attachments)
            {
                var attachment = new Attachments
                {
                    SurveyId = surveyId,
                    UpdateDate = DateTime.Now,
                    Path = item,
                    CreateDate = DateTime.Now,
                    CreatedBy = loginUser.ID,
                    IsDeleted = false,
                    AttachmentTypeID = AttachmentTypes.Survey
                };

                context.Attachments.Add(attachment);
                await context.SaveChangesAsync();
            }
        }
    }
}



