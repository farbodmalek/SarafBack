using Azure.Core;
using CommonLibrary.Common.Maps;
using CommonLibrary.Core.Domain;
using CommonLibrary.Core.Domain.Dto;
using CommonLibrary.Core.Domain.Dto.Common;
using CommonLibrary.Core.Domain.Entities.Loans;
using CommonLibrary.Core.Domain.Entities.PlanNo;
using CommonLibrary.Core.Domain.Interfaces;
using CommonLibrary.Core.Domain.RepositoryInterfaces;
using CommonLibrary.Infrastructure.Utils.Extensions;
using CommonLibrary.Infrastructure.Utils.SqlSortUtil;
using LoanMicroService.Application.DTO.Loans;
using LoanMonitoringMicroService.Core.Domain.RepositoryInterfaces;
using LoanMonitoringMicroService.Core.Domain.ViewModel.Survey;
using LoanMonitoringMicroService.Core.DomainServices.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace LoanMonitoringMicroService.Core.DomainServices.LoansActions
{
    public class SurveyRepositoryRead : QueryParam, ISurveyRepositoryRead
    {
        private readonly IDapperHandler dapperHandler;
        private readonly IDateConvertor dateConvertor;
        private readonly ILoanMonitoringDbContext Context;
        private readonly LoginUserDTO loginUser;


        public SurveyRepositoryRead(IDapperHandler _dapperHandler, IDateConvertor _dateConvertor
            , ILoanMonitoringDbContext Context, LoginUserDTO _loginUser)
        {
            this.dapperHandler = _dapperHandler;
            this.dateConvertor = _dateConvertor;
            this.Context = Context;
            this.loginUser = _loginUser;
        }

        #region  Survey
        public async Task<ResultList<CartableSurveyVM>> GetCartableSurveyList(BaseSurveyFilterDTO request)
        {
            var list = new List<CartableSurveyVM>();
            var l1 = await this.GetCartableSurvyList(request);
            var l2 = await this.GetOutsideSurveyList(request);
            list = l1.Union(l2).OrderByDescending(p => p.ReferenceDate).ThenByDescending(p => p.SurveyDate).ToList();
            var result = new ResultList<CartableSurveyVM>()
            {
                Results = list
            };
            return result;
        }

        private async Task<List<CartableSurveyVM>> GetCartableSurvyList(BaseSurveyFilterDTO request)
        {
            var strQuery = @$"Select  1 as SurveyType
                                ,c.Id,isnull(s.Id,-1) as SurveyId ,c.LoanId,c.CreatedBy as ReferenceUserId
                                ,c.ReferenceDate,s.SurveyDate,isnull(c.UserId,-1) as SurveyUserId
                                ,isnull(c.CartableStatusTypeId,-1) as CartableStatusTypeId 
                                ,isnull(cst.[Name],'') as CartableStatusTypeName,'' as FacilitiesPart,s.CustomerOffer							
                                ,pa.[Name] as PlanActivationTypeDesc ,  s.endOfActivationDate ,c.IsNotScheduledReferred
                                      FROM survey.Cartable           as c
                                LEFT  JOIN survey.Survey             as s   on c.Id=s.CartableId AND s.IsDeleted=0                             
                               INNER  JOIN survey.CartableStatusType as cst on c.CartableStatusTypeId=cst.Id
                                LEFT  JOIN survey.PlanActivationType as  pa on s.PlanActivationTypeId=pa.ID
                                Where c.IsDeleted=0 and C.LoanId= {request.LoanId}
                                    and ((s.SurveyDate is not null) OR (s.SurveyDate is null AND  c.CartableStatusTypeId <>2))
                                    ";

            if (request.CartableStatusTypeId.HasValue)
            {
                strQuery += $" AND c.CartableStatusTypeId = {request.CartableStatusTypeId.Value} ";
            }
            strQuery += " ORDER BY c.ReferenceDate DESC;";
            return await this.dapperHandler.GetAll<CartableSurveyVM>(strQuery);
        }
        public async Task<ResultObject<SurveyDetailVM>> GetSurvyDetailById(BaseSurveyFilterDTO request)
        {

            var SurveyDetail = await this.GetSurvyDetail(request);
            var PlanServiceSurvey = await GetPlanServiceSurvey(request.Id);
            var PlanLivestockSurvey = await GetPlanLivestockSurvey(request.Id);
            var PlanGardenSurvey = await GetPlanGardenSurvey(request.Id);
            var IndustrialSurvey = await GetIndustrialSurvey(request.Id);

            var result = new SurveyDetailVM
            {
                CartableSurveyDetail = SurveyDetail,
                PlanServiceSurvey = PlanServiceSurvey ?? null,
                PlanLivestockSurvey = PlanLivestockSurvey ?? null,
                PlanGardenSurvey = PlanGardenSurvey ?? null,
                IndustrialSurvey = IndustrialSurvey ?? null,

            };

            return new ResultObject<SurveyDetailVM>()
            {
                Data = result,
            };

        }
        public async Task<CartableSurveyDetailVM> GetSurvyDetail(BaseSurveyFilterDTO request)
        {
            var strQuery = $@"Select  c.Id,isnull(s.Id,-1) as SurveyId
                                      ,isnull(c.CreatedBy,-1) as ReferenceUserId  
                                      ,isnull(u1.FullUserName,'') as ReferenceUserName  
                                     ,isnull((Select top 1 Path From Core.Attachments as att Where att.UserID=u1.UserID and att.AttachmentTypeID=5 Order by att.id desc),'') as ReferenceUserImagePath  --عکس کاربر ارجاع دهنده
                                      ,lp.OtherPlanNo
									  ,loan.BranchCode
									  ,branch.SupervisorCode
									  ,branch.BranchName
									  ,branch.SupervisorName
									  ,loan.LoanNumber
									  ,cust.FullName as FullCustomerName
                                      ,isnull(c.UserId,-1) as SurveyUserId  
                                      ,isnull(u2.FullUserName,'') as SurveyUserName 
                                     ,isnull((Select top 1 Path From Core.Attachments as att Where att.UserID=u2.UserID and att.AttachmentTypeID=5 Order by att.id desc),'') as SurveyUserImagePath  --عکس کاربر ارجاع ناظر
                                      ,isnull(c.LoanId,-1) as LoanId
                                      ,s.CustomerOffer
                                       ,c.ReferenceDate 
                                      ,s.SurveyDate  
                                 
                                      ,0 as IsValidation 
                                      ,lp.PlanNoId as PlanNoId 
                                      --,isnull(lp.PlanNoId,isnull(lpn.PlanNoId, -1 )) as PlanNoId 
                                      ,  pn2.[Name]  as PlanNoDesc
                                      --, CASE WHEN pn.[Name] like N'سایر' OR pn.[Name]='' OR pn.[Name] is null THEN pn2.[Name] ELSE pn.[Name] END as PlanNoDesc
									  ,isnull(pn2.Name,0) as PlanTypeName  
                                      ,isnull(pt.[Name],'') as PlanTypeDesc  
                                      ,'?' as CurrentPlanSurvey 
                                      ,'?' as PhaseBetweenGuarantee 
                                      ,isnull(s.EquipmentDescription,'') EquipmentDescription 									  
									  ,isnull(lp.Latitude,0) as Latitude
									  ,isnull(lp.Longitude,0) as Longitude
									  ,pa.[ID] as PlanActivationTypeId
									  ,isnull(pat.[Name],'') as PlanActivationTypeDesc
									  , Case When isnull(fla.BankPayAmount,0)<isnull(lcont.AcceptAmount,0) Then 'نظارت بین مرحله ای' Else 
									  'نظارت مرحله ای' END as SurveyPhase
                                      ,lp.[Address],pa.[Name] as PlanActivationType
                                      ,lp.LoanSurveyEconomidTypeId,lp.Id as loanPlanId , s.endOfActivationDate ,lst.Name ,let.[Desc]   as FacilitiesPart 
                                      From survey.Cartable as c
                                      LEFT JOIN survey.Survey as s on c.Id=s.CartableId
									  LEFT Join Core.Loans as loan on loan.Id = c.LoanId
									  LEFT Join Core.Branches as branch on loan.BranchCode = branch.BranchCode
									  LEFT Join Core.Customers as cust on loan.CustomerId = cust.Id
                                      LEFT JOIN survey.LoanPlan as lp on c.LoanId=lp.LoanId
                                      LEFT JOIN survey.LoanPlanNo lpn on c.LoanId=lpn.LoanId AND lpn.IsDeleted=0
									  LEFT JOIN survey.PlanNo pn2 on lp.PlanNoId=pn2.Id AND pn2.IsDeleted=0
                                      LEFT JOIN survey.PlanType as pt on lp.PlanTypeId=pt.Id
                                      LEFT JOIN Core.Users as u1 on c.CreatedBy=u1.UserID
                                      LEFT JOIN Core.Users as u2 on c.UserId=u2.UserID
                                      LEFT JOIN [survey].[LoanSurveyEconomicType] as lst on lp.LoanSurveyEconomicTypeId=lst.Id
									  LEFT JOIN survey.PlanActivationType as pat on s.PlanActivationTypeId=pat.ID
									  LEFT JOIN core.FactLoanAcc as fla on lp.LoanId=fla.LoanId
									  LEFT JOIN core.LoanContracts as lcont on lp.LoanId=lcont.LoanId
                                      LEFT JOIN  survey.PlanActivationType as pa on s.PlanActivationTypeId=pa.ID
                                          Left Join Core.LoanEconomicTypes as let on lcont.LoanEconomicTypeId=let.Id
                                      Where 1=1 and s.Id={request.Id}";
            return await this.dapperHandler.GetFirstOrDefault<CartableSurveyDetailVM>(strQuery);
        }
        public async Task<PlanServiceSurveyVM> GetPlanServiceSurvey(int? id)
        {
            var query = $@"Select
                                pss.Id
                                ,isnull(pss.SurveyId,-1) as SurveyId
                                ,isnull(pss.HasWorkPermission,0) as HasWorkPermission
                                ,case when isnull(pss.HasWorkPermission,0)=1 Then 'دارد'Else 'ندارد' END HasWorkPermissionDesc
                                ,isnull(pss.OwnerTypeId,-1) as OwnerTypeId
                                ,isnull(ot.[Name],'') as OwnerTypeDesc
                                ,case When isnull(s.NumberOfInsurdPerson,0)=0 then 'ندارد' Else 'دارد' END HaseInsuranceList
                                ,isnull(s.NumberOfJobsCreated,0) as NumberOfJobsCreated
                                ,isnull(s.NumberOfInsurdPerson,0) as NumberOfInsurdPerson
                                ,isnull(pss.PresenceTypeId ,0) as PresenceTypeId
                                ,isnull(pt.[Name],'') as PresenceTypeName
                                ,isnull(s.EquipmentDescription,'') as EquipmentDescription
                                ,isnull(s.SurveyReport,'') as SurveyReport
                                 from survey.PlanServiceSurvey as pss
                                 Left Outer Join survey.Survey as s on pss.SurveyId=s.Id
                                 Left Outer Join survey.OwnerType as ot on pss.OwnerTypeId=ot.Id
                                 left outer join survey.PresenceType as pt on pss.PresenceTypeId=pt.Id
                                 Where 1=1 and isnull(pss.SurveyId,-1)={id}";
            return await this.dapperHandler.GetFirstOrDefault<PlanServiceSurveyVM>(query);
        }
        public async Task<PlanLivestockSurveyVM> GetPlanLivestockSurvey(int? id)
        {
            var query = $@"Select
                                 pls.Id
                                 ,isnull(pls.SurveyId,-1) as SurveyId
                                 ,isnull(pls.LivestockBooklet,0) as LivestockBooklet
                                 , case When isnull(pls.LivestockBooklet,0)=1 then 'دارد' else 'ندارد' END LivestockBookletDesc
                                 ,isnull(pls.LivestockLicense,0) as LivestockLicense
                                 ,case When isnull(pls.LivestockLicense,0)=1 Then 'دارد' Else 'ندارد' END LivestockLicenseDesc
                                 ,pls.InsuranceDate
                                 ,isnull(pls.NumberOfInsuredLivestock,0) as NumberOfInsuredLivestock
                                 ,isnull(s.NumberOfJobsCreated,0)as NumberOfJobsCreated
                                 ,isnull(s.NumberOfInsurdPerson,0) as NumberOfInsurdPerson
                                 ,isnull(pls.NumberOfMaleLivestock,0) as NumberOfMaleLivestock
                                 ,ISNULL(pls.NumberOfFemaleLivestock,0) as NumberOfFemaleLivestock
                                 ,isnull(pls.NumberOfMaleLivestock,0)+ISNULL(pls.NumberOfFemaleLivestock,0) as NumberOfTotalLivestock
                                 ,isnull(pls.LivestockTypeId ,0) as LivestockTypeId
                                 ,isnull(lst.[Name],'') as LivestockTypeDesc
                                ,isnull(s.SurveyReport,'') as SurveyReport
								,isnull(pls.LivestockInsurance,0) as LivestockInsurance
								,case When isnull(pls.LivestockInsurance,0) = 1 Then 'دارد' Else 'ندارد' END LivestockInsuranceDesc
                                 From survey.PlanLivestockSurvey as pls
                                 Left Outer Join survey.Survey as s on pls.SurveyId=s.Id
                                 Left Outer Join survey.LivestockType as lst on pls.LivestockTypeId = lst.Id
                                 Where 1=1 and isnull(pls.SurveyId,-1)={id}";
            return await this.dapperHandler.GetFirstOrDefault<PlanLivestockSurveyVM>(query);
        }
        public async Task<PlanGardenSurveyVM> GetPlanGardenSurvey(int? id)
        {
            var query = $@"Select 
                                pgs.Id
                                ,isnull(pgs.SurveyId,-1) as SurveyId
                                ,isnull(pgs.OwnerTypeId,-1) as OwnerTypeId
                                ,isnull(ot.[Name],'') as OwnerTypeDesc
                                ,isnull(pgs.LandArea,0) as LandArea
                                ,ISNULL(pgs.CultivatedLandArea,0) as CultivatedLandArea
                                 ,isnull(s.NumberOfJobsCreated,0)as NumberOfJobsCreated
                                 ,isnull(s.NumberOfInsurdPerson,0) as NumberOfInsurdPerson
                                 ,isnull(pgs.HasAgriculturalInsurance,0) as HasAgriculturalInsurance
                                 ,Case When isnull(pgs.HasAgriculturalInsurance,0)=1 Then 'دارد' Else 'ندارد' END HasAgriculturalInsuranceDesc
                                ,isnull(s.SurveyReport,'') as SurveyReport
								,isnull(pgs.ProductTypeId,-1) as ProductTypeId
								,isnull(pt.[Name],'') as ProductTypeDesc
                                ,isnull(pgs.LandArea,0) as LandArea
                                From survey.PlanGardenSurvey as pgs
                                left Outer Join survey.OwnerType as ot on pgs.OwnerTypeId=ot.Id
                                Left Outer Join survey.Survey as s on pgs.SurveyId=s.Id
								left outer join survey.ProductType as pt on pgs.ProductTypeId=pt.ID
                                Where 1=1 and isnull(pgs.SurveyId,-1)={id}";
            return await this.dapperHandler.GetFirstOrDefault<PlanGardenSurveyVM>(query);
        }
        public async Task<IndustrialSurveyVM> GetIndustrialSurvey(int? id)
        {
            var query = $@" Select ps.id
                            ,isnull(ps.SurveyId,-1) as SurveyId,isnull(ps.HasWorkPermission,0) as HasWorkPermission 
                            ,case when isnull(ps.HasWorkPermission,0)=1 then 'دارد' Else 'ندارد' End HasWorkPermissionDesc
                            ,isnull(ps.OwnerTypeId,-1) as OwnerTypeId
                            ,isnull(ot.[Name],'') as OwnerTypeDesc
                            ,isnull(ps.PresenceTypeId ,0) as PresenceTypeId
                            ,isnull(pt.[Name],'') as PresenceTypeName
                            ,isnull(s.EquipmentDescription,'') as EquipmentDescription
                            ,isnull(s.SurveyReport,'') as SurveyReport
                            ,isnull(s.NumberOfInsurdPerson,0) as NumberOfInsurdPerson
                            ,isnull(c.LoanId,-1) as LoanId
                            ,isnull(lst.[Name],'') as LoanSurveyEconomicTypeDesc
                            ,case When isnull(s.NumberOfInsurdPerson,0)=0 then 'ندارد' Else 'دارد' END HaseInsuranceList
                            ,isnull(s.NumberOfJobsCreated,0)as NumberOfJobsCreated
                            From survey.PlanIndustrialSurvey as ps
                            Left Outer Join survey.Survey as s on ps.SurveyId=s.Id
                            Left Outer Join survey.OwnerType as ot on ps.OwnerTypeId=ot.Id
                            left outer join survey.PresenceType as pt on ps.PresenceTypeId=pt.Id
                            left outer join survey.Cartable as c on s.CartableId=c.Id
                            left outer join survey.LoanPlan as lp on c.Id=lp.CartableId
                            left outer Join survey.LoanSurveyEconomicType as lst on lp.LoanSurveyEconomicTypeId=lst.Id
                            Where 1=1  and s.Id={id}";
            return await this.dapperHandler.GetFirstOrDefault<IndustrialSurveyVM>(query);
        }


        public async Task<ResultObject<LoanSurveyDocumentVM>> GetSurveyDocument(int loanId, int CartableId = 0, int? type=null)
        {
            var model = await GetLoanSurveyDocumentModel(loanId, this.loginUser, CartableId);
            if (model == null)
                throw new("نظارتی برای پرونده وجود ندارد");
            model = await GetCustomerAddresse(model);
            model.surveyReportModels = await GetSurveyReportModels(loanId, CartableId);
            string finalQuery = string.Format(makeSelectOutsideSurvey(), loanId);
            var response = await this.dapperHandler.GetAll<OutsideSurveyVM>(finalQuery);
            model.OutSideSurveyVM = response;
            //model = FillLoanSurveyEconomicInfoList(model, loanId);
            model = GetSurveyCount(model);
            var result = new ResultObject<LoanSurveyDocumentVM>();
            result.Data = model;
            return result;
        }
        private async Task<LoanSurveyDocumentVM> GetLoanSurveyDocumentModel(int loanId, LoginUserDTO currentUser, int cartableId = 0)
        {

            var loanPlan = await (from lp in Context.LoanPlans
                                  join pn2 in Context.PlanNos on lp.PlanNoId equals pn2.Id
                                  where lp.LoanId == loanId && !lp.IsDeleted && (cartableId == 0 || lp.CartableId == cartableId)
                                  select new
                                  {
                                      LoanPlanAddress = lp.Address,
                                      LoanPlanPhone = lp.Phone,
                                      LoanPlanTitle = pn2.Name,//(pn2.Name.Equals("سایر") || string.IsNullOrEmpty(pn2.Name)) ? "" : pn2.Name,   //lp.OtherPlanNo,//11347
                                      LoanSurveyEconomicTypeId = lp.LoanSurveyEconomidTypeId
                                  }).OrderBy(p => p.LoanSurveyEconomicTypeId).LastOrDefaultAsync();


            var temp = await (from l in Context.Loans
                              join lc in Context.LoanContracts on l.Id equals lc.LoanId
                              join cs in Context.Customers on l.CustomerId equals cs.Id
                              join lp in Context.LoanPlans on l.Id equals lp.LoanId into loanPlans
                              from lp in loanPlans.DefaultIfEmpty()
                                  //join pn2 in context.PlanNos on lp.PlanNoId equals pn2.Id
                              join br in Context.Branches on l.BranchCode equals br.BranchCode
                              join lpn in Context.LoanPlanNo on l.Id equals lpn.LoanId
                              join pn in Context.PlanNos on lpn.PlanNoId equals pn.Id
                              where l.Id == loanId /*&& !lp.IsDeleted*/ && !lpn.IsDeleted //&& (CartableId==0 || lp.CartableId ==CartableId)
                              select new LoanSurveyDocumentVM
                              {
                                  CustomerId = cs.Id,
                                  CustomerNumber = cs.CustomerNumber,
                                  CustomerName = cs.FullName,
                                  BranchName = br.BranchName,
                                  SupervisorName = br.SupervisorName,
                                  CityName = br.CityName,
                                  AcceptAmount = lc.AcceptAmount,
                                  AcceptDate = lc.AcceptDate.ConvertGregorianToPersianDate(),
                                  LoanNumber = l.LoanNumber,
                                  BirthDate = cs.BirthDate.ConvertGregorianToPersianDate(),
                                  RequestDate = DateTime.Now,
                                  FatherName = cs.FatherName,
                                  IsFamilySupervisor = cs.IsFamilySupervisor.MakeBoolText(BoolType.YesNo),
                                  Gender = cs.Gender.MakeBoolText(BoolType.Gender),
                                  IsMarried = cs.IsMarried.MakeBoolText(BoolType.Married),
                                  UserName = currentUser.FirstName + " " + currentUser.LastName,
                                  LoanPlanAddress = lp.Address,
                                  LoanPlanPhone = lp.Phone,
                                  LoanPlanTitle = pn.Name,//(pn2.Name.Equals("سایر") || string.IsNullOrEmpty(pn2.Name)) ? pn.Name : pn2.Name,   //lp.OtherPlanNo,//11347
                                  LoanSurveyEconomicTypeId = lp.LoanSurveyEconomidTypeId
                              }).OrderBy(p => p.CustomerId).LastOrDefaultAsync();

            if (loanPlan != null)
            {
                temp.LoanPlanAddress = loanPlan.LoanPlanAddress;
                temp.LoanPlanPhone = loanPlan.LoanPlanPhone;
                temp.LoanPlanTitle = string.IsNullOrWhiteSpace(loanPlan.LoanPlanTitle) ? temp.LoanPlanTitle : loanPlan.LoanPlanTitle;
                temp.LoanSurveyEconomicTypeId = loanPlan.LoanSurveyEconomicTypeId;
            }

            return temp;
        }
        private async Task<LoanSurveyDocumentVM> GetCustomerAddresse(LoanSurveyDocumentVM model)
        {
            var addr = await this.dapperHandler.Get<CustomerAddressVM>(
                $"select * from Core.CustomerAddresses where CustomerId = {model.CustomerId}");
            model.CustomerAddresse = $"{addr.AddressLine1} - {addr.AddressLine2} - {addr.AddressLine3}";
            model.Phone = addr.Phone;
            model.Mobile = addr.Mobile;
            return model;
        }
        private async Task<List<SurveyReportModelDto>> GetSurveyReportModels(int loanId, int CartableId = 0)
        {
            var model = await (from c in Context.Cartables
                               join s in Context.Surveys on c.Id equals s.CartableId
                               join pa in Context.PlanActivationTypes on s.PlanActivationTypeId equals pa.ID
                               join lp in Context.LoanPlans on c.LoanId equals lp.LoanId
                               join set in Context.LoanSurveyEconomicTypes on lp.LoanSurveyEconomidTypeId equals set.Id

                               // Left join PlanGardenSurvey
                               join PlanGardenSurvey in Context.PlanGardenSurveys
                                    on s.Id equals PlanGardenSurvey.SurveyId into pgJoin
                               from PlanGardenSurvey in pgJoin.DefaultIfEmpty()

                                   // Left join PlanServiceSurvey
                               join PlanServiceSurvey in Context.PlanServiceSurveys
                                    on s.Id equals PlanServiceSurvey.SurveyId into psJoin
                               from PlanServiceSurvey in psJoin.DefaultIfEmpty()

                                   // Left join PlanIndustrialSurvey
                               join PlanIndustrialSurvey in Context.PlanIndustrialSurveys
                                    on s.Id equals PlanIndustrialSurvey.SurveyId into piJoin
                               from PlanIndustrialSurvey in piJoin.DefaultIfEmpty()

                                   // Left join PlanLivestockSurvey
                               join PlanLivestockSurvey in Context.PlanLivestockSurveys
                                    on s.Id equals PlanLivestockSurvey.SurveyId into plJoin
                               from PlanLivestockSurvey in plJoin.DefaultIfEmpty()

                               where c.LoanId == loanId && !c.IsDeleted && !s.IsDeleted && !lp.IsDeleted
                               where CartableId > 0 ? c.Id == CartableId : true
                               select new SurveyReportModelDto
                               {
                                   Id = s.Id,
                                   EquipmentDescription = s.EquipmentDescription,
                                   NumberOfInsurdPerson = s.NumberOfInsurdPerson,
                                   NumberOfJobsCreated = s.NumberOfJobsCreated,
                                   SurveyReport = s.SurveyReport,
                                   LoanSurveyEconomicType = set.Name,
                                   CustomerOffer = s.CustomerOffer,
                                   SurveyDateFa = s.SurveyDate.ConvertGregorianToPersianDate(),
                                   SurveyUserId = s.CreatedBy,
                                   PlanActivationType = pa.Name,
                                   LivestockBooklet = PlanLivestockSurvey.LivestockBooklet == true ? "دارد" : "ندارد",
                                   LivestockLicense = PlanLivestockSurvey.LivestockLicense == true ? "دارد" : "ندارد",
                                   NumberOfInsuredLivestock= PlanLivestockSurvey.NumberOfInsuredLivestock,
                                   LivestockInsurance = PlanLivestockSurvey.LivestockInsurance == true ? "دارد" : "ندارد",
                                   InsuranceDate = PlanLivestockSurvey.InsuranceDate.ConvertNullableGregorianToPersianDate(),
                               }).ToListAsync();

            return model;

            return model;
        }
        private LoanSurveyDocumentVM GetSurveyCount(LoanSurveyDocumentVM model)
        {
            for (int i = 0; i < model.surveyReportModels.Count; i++)
            {
                model.surveyReportModels[i].SurveyLevel =
                 model.surveyReportModels[i].LoanStatusTypeId == "E" ?
                "نظارت بین مرحله ای" : $"{i + 1} نظارت مرحله";
            }
            return model;
        }
        private LoanSurveyDocumentVM FillLoanSurveyEconomicInfoList(LoanSurveyDocumentVM model, int loanId)
        {
            //var loanSurveyEconomidTypeId = Context.LoanPlans.Single(c => c.LoanId == loanId && !c.IsDeleted).LoanSurveyEconomidTypeId;
            //foreach (var item in model.SurveyReportModels)
            //{
            //    fillLoanSurveyEconomicInfo(item, loanSurveyEconomidTypeId);
            //}
            return model;
        }
        #endregion

        #region  OutsideSurvey
        private async Task<List<CartableSurveyVM>> GetOutsideSurveyList(BaseSurveyFilterDTO request)
        {
            string strQuery = $@"SELECT 0 as SurveyType,
                                      isnull(c.Id,-1) AS Id,
                                      isnull(outs.Id,-1) AS SurveyId,
                                      outs.LoanId,
                                      c.ReferenceUserId AS ReferenceUserId,
                                      isnull(c.ReferenceDate,outs.SurveyDate)  AS ReferenceDate,
                                      outs.SurveyDate,
                                      c.CartableStatusTypeId AS CartableStatusTypeId ,
                                      case when pa.[ID]=3 then  'نظارت کمیته' 
                                                          else ' نظارت کمیته' 
                                      end  AS CartableStatusTypeName,--pa.[Name] AS CartableStatusTypeName,
                                      '' AS FacilitiesPart,
                                      '' AS CustomerOffer,
                                      outs.CreatedBy AS SurveyUserId,
                                      pa.[Name] as PlanActivationTypeDesc,   --outs.PlanNoName AS PlanActivationType,  
                                      outs.DeadlineDate as endOfActivationDate --, la.DeadlineDate as endOfActivationDate 
                                        ,c.IsNotScheduledReferred
                                 FROM survey.OutsideSurvey outs 
                               INNER JOIN survey.PlanActivationType as pa on outs.PlanActivationTypeId=pa.ID
                                LEFT JOIN survey.Cartable c on c.Id = outs.CartableId
								WHERE outs.LoanId = {request.LoanId} and outs.isdeleted <> 1 ";

            return await this.dapperHandler.GetAll<CartableSurveyVM>(strQuery);
        }
        public async Task<ResultList<OutsideSurveyVM>> GetOutsideSurveyList(OutSideSurveyFilterDTO request)
        {
            var result = new ResultList<OutsideSurveyVM>();

            string finalQuery = string.Format(makeSelectOutsideSurvey(), request.LoanId);
            var response = await this.dapperHandler.GetAll<OutsideSurveyVM>(finalQuery);
            result.TotalRows = response?.Count() ?? 0;
            result.Results = response ?? new List<OutsideSurveyVM>();
            return result;
        }
        private string makeSelectOutsideSurvey()
        {
            return @"SELECT [c.PlanActivationType].[Name] AS [PlanActivationType], 
               [c].[SurveyDate], 
               [c].[Id], 
               [c].[LoanId], 
               [c].[numberOfInsurdPerson], 
               [c].[numberOfJobsCreated], 
               [c].[numberOfJobsObligated], 
               [c].[PlanNoName], 
               [c].[DeadLineDate], 
               [c].[ReasonSubmit],
               [c].Address,
			   [c].Phone,
               [c].CreatedBy  as SurveyUserId
               FROM [survey].[OutsideSurvey] AS [c]
               INNER JOIN [survey].[PlanActivationType] AS [c.PlanActivationType] 
               ON [c].[PlanActivationTypeId] = [c.PlanActivationType].[ID]
               WHERE ([c].[IsDeleted] = 0) 
               AND ([c].[LoanId] = {0})
               ORDER BY [c].[SurveyDate] desc
              ";
        }

        #endregion

        #region Loan Plan Marker

        public async Task<ResultList<LoanPlanMarkerVM>> GetPlanMarkerList(LoanPlanMarkerFilterVM filter)
        {
            var finalQuery = MakeLoanPlanQuery() + MakeLoanPlanWhere(filter);
            var list = await this.dapperHandler.GetAll<LoanPlanMarkerVM>(finalQuery);
            var finalList = new List<LoanPlanMarkerVM>();
            var brancheCodeList = list.GroupBy(c => c.BranchCode).Select(c => c.Key).ToList();
            int take = brancheCodeList.Count() > 0 ? 500 / brancheCodeList.Count() : 500;
            foreach (var brancheCode in brancheCodeList)
            {
                finalList.AddRange(
                    list.Where(c => c.BranchCode == brancheCode).Take(take).ToList());
            }


            if (list.Count > 0 && (filter.BranchList.Count > 0 || filter.SupervisorList.Count > 0))
            {
                list[0].IsCenter = true;
                list[0].ZoomLevel = filter.SupervisorList.Count >= 1 ? 9 : filter.BranchList.Count >= 1 ? 12 : 7;
            }
            var result = new ResultList<LoanPlanMarkerVM>()
            {
                Results = finalList,
            };
            return result;
        }
        private string MakeLoanPlanQuery()
        {
            var strQuery = @"select l.BranchCode,b.BranchName,l.Id as loanId,lp.Latitude,lp.Longitude,lp.LoanSurveyEconomidTypeId as MarkerType ,sr.Id

                     from survey.LoanPlan lp
            		 JOIN core.Loans l on lp.LoanId=l.Id
                     JOIN core.LoanRedundantView lrw on lp.LoanId=lrw.LoanId
                     JOIN Core.LoanContracts as lcontract on l.Id=lcontract.LoanId
            		 JOIN core.Customers c on l.CustomerNumber=c.CustomerNumber
            		 JOIN core.Branches b on l.BranchCode=b.BranchCode
            		 JOIN core.LoanContracts lc on lp.LoanId=lc.LoanId
            		 JOIN survey.PlanNo pn on lp.PlanNoId=pn.Id
            	LEFT JOIN core.FactLoanAcc acc on l.Id =  acc.LoanId
            	     JOIN core.LoanStatusTypes ls on l.LoanStatusTypeId=ls.Id
                LEFT JOIN survey.Cartable cr on lrw.LastCartableId= cr.Id and cr.IsDeleted=0
            	LEFT JOIN survey.Survey sr on cr.id = sr.CartableId
                     JOIN core.LoanStatusTypes lst on l.LoanStatusTypeId=lst.Id
                      ";
            return strQuery;
        }
        public string MakeLoanPlanWhere(LoanPlanMarkerFilterVM filter)
        {
            this.AddQueryParamMap = new Dictionary<string, AddQueryParam>
            {
                { "BranchList", MakeBranchListQuery },
                { "SupervisorList", MakeSupervisorsQuery },
                { "SurveyUserId", SurveyUserIdQuery },
                { "LoanStatusTypeList", MakeLoanStatusTypeListQuery },
                { "LoanMinorTypeList", MakeLoanMinorTypeQuery },
                { "LastActionTypeId", MakeLastActionTypeIdQuery },
                { "ReferenceMinDateFa", MakeReferenceMinDateQuery },
                { "ReferenceMaxDateFa", MakeReferenceMaxDateQuery },
                { "PlanNoIdList", MakePlanNoIdListQuery },
                { "SurveyMinDateFa", MakeSurveyMinDateQuery },
                { "SurveyMaxDateFa", MakeSurveyMaxDateQuery },
                { "SurveyStageMin", MakeSurveyStageMinQuery },
                { "SurveyStageMax", MakeSurveyStageMaxQuery },
                { "PlanActivationTypeId", MakePlanActivationTypeIdQuery },
                { "ReagentList", MakeReagentListQuery },
                { "CustomerId", MakeCustomerIdQuery },
                { "CustomerNumber", MakeCustomerNumberQuery },
                { "LoanInstallmentTypeId", MakeLoanInstallmentTypeIdQuery },
                { "LoanAmountMin", MakeLoanAmountMinQuery },
                { "LoanAmountMax", MakeLoanAmountMaxQuery },
                { "ContractMinDateFa", MakeContractMinDateQuery },
                { "ContractMaxDateFa", MakeContractMaxDateQuery },
            };
            var whereSection = new StringBuilder();
            whereSection.Append(@$" Where   cr.IsDeleted = 0 and CartableStatusTypeId=2 and sr.id is not null");
            var filterItemList = filter.GetFieldProperties();
            foreach (var item in filterItemList)
            {
                if (this.AddQueryParamMap.ContainsKey(item.Key))
                    whereSection.AppendLine(this.AddQueryParamMap[item.Key](item.Value));
            }
            whereSection.Append(filter.MapEventPoint != null ? makeWhereByMapEvent(filter.MapEventPoint) : "");
            whereSection.AppendLine($" ORDER BY sr.SurveyDate OFFSET 0 ROWS FETCH NEXT {1000} ROWS ONLY");
            return whereSection.ToString();
        }
        private string makeWhereByMapEvent(MapEventPointDTO mapEventPointDto)
        {
            if (string.IsNullOrWhiteSpace(mapEventPointDto.Latitude) || string.IsNullOrWhiteSpace(mapEventPointDto.Latitude))
                return "";
            var distance = mapEventPointDto.ZoomLevel == 17 ? 3000 :
                            mapEventPointDto.ZoomLevel == 16 ? 5000 :
                            mapEventPointDto.ZoomLevel == 15 ? 10000 :
                            mapEventPointDto.ZoomLevel == 14 ? 25000 :
                            mapEventPointDto.ZoomLevel == 13 ? 40000 :
                            mapEventPointDto.ZoomLevel == 12 ? 40000 :
                            mapEventPointDto.ZoomLevel == 11 ? 50000 :
                            mapEventPointDto.ZoomLevel == 10 ? 50000 :
                            mapEventPointDto.ZoomLevel == 9 ? 250000 :
                            mapEventPointDto.ZoomLevel <= 8 ? 800000 : 0;
            return $@" AND (lp.GeoLocation.STDistance(CONVERT(geography,
                            'POINT({mapEventPointDto.Latitude} {mapEventPointDto.Longitude})')))<{distance}";
        }
        public string MakeBranchListQuery(object value)
        {
            if (value != null && ((List<int>)value).Count > 0)
                return $" and l.BranchCode in ({((List<int>)value).JoinToString()})";
            return "";
        }
        public string MakeSupervisorsQuery(object value)
        {
            if (value != null && ((List<int>)value).Count > 0)
                return $" and b.SupervisorCode in ({((List<int>)value).JoinToString()})";
            return "";
        }
        public string SurveyUserIdQuery(object value)
        {
            return $" and cr.UserId={value}";
        }
        public string MakeLoanNumberQuery(object value)
        {
            return $" and loan.CustomerNumber={value}";
        }
        public string MakeConfirmUserIdAddressRequestQuery(object value)
        {
            return $" and CLPS.VerifyBy={value}";
        }
        public string MakeLoanStatusTypeListQuery(object value)
        {
            if (value != null && value is List<string> list && list.Count > 0)
            {
                var ids = string.Join(", ", list.Select(id => $"'{id}'"));
                return $" and lst.Id in ({ids})";
            }
               
            return "";
        }
        public string MakeLoanMinorTypeQuery(object value)
        {
            if (value != null && ((List<int>)value).Count > 0)
                return $" and l.LoanMinorTypeId in ({((List<int>)value).JoinToString()})";
            return "";
        }
        public string MakeLastActionTypeIdQuery(object value)
        {
            return $" and l.LastActionTypeId = {value}";
        }
        public string MakeReferenceMinDateQuery(object value)
        {
            var date = this.dateConvertor.ConvertPersianDatetimeToGregorian(value.ToString());
            return $" and cr.ReferenceDate >= '{date}'";
        }
        public string MakeReferenceMaxDateQuery(object value)
        {
            var date = this.dateConvertor.ConvertPersianDatetimeToGregorian(value.ToString());
            return $" and cr.ReferenceDate <= '{date}'";
        }
        public string MakePlanNoIdListQuery(object value)
        {
            if (value != null && ((List<int>)value).Count > 0)
                return $" and pn.Id in ({((List<int>)value).JoinToString()})";
            return "";
        }
        public string MakeSurveyMinDateQuery(object value)
        {
            var date = this.dateConvertor.ConvertPersianDatetimeToGregorian(value.ToString());
            return $" and sr.SurveyDate >= '{date}'";
        }
        public string MakeSurveyMaxDateQuery(object value)
        {
            var date = this.dateConvertor.ConvertPersianDatetimeToGregorian(value.ToString());
            return $" and sr.SurveyDate <= '{date}'";
        }
        public string MakeSurveyUsersQuery(object value)
        {
            if (value != null && ((List<int>)value).Count > 0)
                return $" and lrw.LastSurveyUserId in ({((List<int>)value).JoinToString()})";
            return "";
        }
        public string MakeLastSurveyMinDateQuery(object value)
        {
            string query = " ";
            string date = value.ToString();
            if (!string.IsNullOrEmpty(date))
            {
                query = $" and lrw.LastSurveyDate >='{dateConvertor.ConvertPersianDatetimeToGregorianOrNull(date)}' ";
            }
            return query;
        }
        public string MakeLastSurveyMaxDateQuery(object value)
        {
            string query = " ";
            string date = value.ToString();
            if (!string.IsNullOrEmpty(date))
            {
                query = $" AND lrw.LastSurveyDate <='{dateConvertor.ConvertPersianDatetimeToGregorianOrNull(date)}' ";
            }
            return query;
        }
        public string MakeStatusQuery(object value)
        {
            if ((int)value == 1)
                return $" and CLPS.IsConfirmed =1";
            else if ((int)value == 0)
            {
                return $" and CLPS.IsConfirmed =0";
            }
            return $" and CLPS.IsConfirmed  is null ";
        }
        public string MakeSurveyStageMinQuery(object value)
        {
            return "";// $" and l.LastActionTypeId = {value}";
        }
        public string MakeSurveyStageMaxQuery(object value)
        {
            return "";// $" and l.LastActionTypeId = {value}";
        }
        public string MakePlanActivationTypeIdQuery(object value)
        {
            return $" and sr.PlanActivationTypeId = {value}";
        }
        public string MakeReagentListQuery(object value)
        {
            if (value != null && ((List<int>)value).Count > 0)
                return $" and l.Id IN(select LoanId from core.LoanGuaranties where GuarantorCustomerId IN ({((List<int>)value).JoinToString()}) and RelationTypeId=3)";
            return "";
        }
        public string MakeCustomerIdQuery(object value)
        {
            if (value != null && ((List<int>)value).Count > 0)
                return $" and c.Id in ({((List<int>)value).JoinToString()})";
            return "";
        }
        public string MakeCustomerNumberQuery(object value)
        {
            return $" and l.CustomerNumber = {value}";
        }
        public string MakeLoanInstallmentTypeIdQuery(object value)
        {
            return $" and lc.LoanInstallmentTypeId = {value}";
        }
        public string MakeLoanAmountMinQuery(object value)
        {
            return $" and lc.AcceptAmount >= {value}";
        }
        public string MakeLoanAmountMaxQuery(object value)
        {
            return $" and lc.AcceptAmount <= {value}";
        }
        public string MakeContractMinDateQuery(object value)
        {
            var date = this.dateConvertor.ConvertPersianDatetimeToGregorian(value.ToString());
            return $" and lc.BeginDate >= '{date}'";
        }
        public string MakeContractMaxDateQuery(object value)
        {
            var date = this.dateConvertor.ConvertPersianDatetimeToGregorian(value.ToString());
            return $" and lc.BeginDate <= '{date}'";
        }



        #endregion


        public async Task<ResultList<SurveyAddressRequestsVM>> GetSurveyAddressRequestsList(SurveyAddressRequestsFilterVM request)
        {
            this.AddQueryParamMap = new Dictionary<string, AddQueryParam>
            {
                { "StatusID",this.MakeStatusQuery},
                { "CustomerNumber", this.MakeLoanNumberQuery },
                { "BranchList", this.MakeBranchListQuery },
                { "SupervisorList", this.MakeSupervisorsQuery },
                { "SurveyUsers", this.MakeSurveyUsersQuery },
                { "SurveyMinDateFa", this.MakeLastSurveyMinDateQuery },
                { "SurveyMaxDateFa", this.MakeLastSurveyMaxDateQuery  },
                { "ConfirmUserId", this.MakeConfirmUserIdAddressRequestQuery  },

            };


            var result = new ResultList<SurveyAddressRequestsVM>();
            var query = @"   FROM [survey].[ChangeLonaPlanState] as CLPS
                              left join [survey].[LoanPlanLocationState] as LPLS on LPLS.ChangeLoanPlanStateId= CLPS.ID
							  left join [survey].[Survey] as S on s.CartableId=LPLS.CartableId
							  left join [survey].[LoanPlan] as LP on LP.ID=CLPS.LoanPlanId
                              inner join [Core].[Loans] as loan on loan.Id= CLPS.LoanId
                              inner join [Core].[FactLoanAcc] as FLC on FLC.LoanId= loan.Id
                              inner join [Core].[LoanRedundantView] as lrw on lrw.LoanId= loan.Id
                              inner join [Core].[LoanStatusTypes] as LST on LST.Id= FLC.LoanStatusTypeId
                              inner Join Core.Customers as cust on loan.CustomerId = cust.Id
                              inner Join Core.Branches as b on loan.BranchCode = b.BranchCode
							  inner Join [Core].[LoanContracts] as LCC on loan.Id = LCC.LoanId
                              ";

            var whereSection = new StringBuilder();
            whereSection.AppendLine("where CLPS.IsDeleted = 0 ");
            var filterItemList = request.GetFieldProperties();
            foreach (var item in filterItemList)
            {
                if (this.AddQueryParamMap.ContainsKey(item.Key))
                    whereSection.AppendLine(this.AddQueryParamMap[item.Key](item.Value));
            }
            //var sortQuery = SortTranslator
            //        .Instance(SortDictionary.SurveyAddressRequestsList)
            //        .makeSortQuery(request.SortDTO);
            string pagingQuery = $@"
                                   order by lrw.LastSurveyDate ,loan.LoanNumber  OFFSET {request.Skip} ROWS FETCH NEXT {request.Take} ROWS ONLY";
            var finalQuery = MakeSurveyAddressRequestsSection() + query + whereSection + pagingQuery;
            var list = await dapperHandler.GetAll<SurveyAddressRequestsVM>(finalQuery);
            foreach (var item in list.Where(x =>
       !string.IsNullOrWhiteSpace(x.currentLatitude) &&
       !string.IsNullOrWhiteSpace(x.currentLongitude) &&
       !string.IsNullOrWhiteSpace(x.newLatitude) &&
       !string.IsNullOrWhiteSpace(x.newLongitude)))
            {
                if (request.StatusID == 1)
                {
                    var survey = await GetSurveys(item.LoanId);
                    if (survey.Latitude != null && survey.Longitude != null)
                    {
                        item.currentLatitude = survey.Latitude;
                        item.currentLongitude = survey.Longitude;

                    }

                }
                item.DistsnceDifference = checkDistsnce(item.currentLatitude, item.currentLongitude, item.newLatitude, item.newLongitude);
            }

            result.TotalRows = await this.dapperHandler.GetFirstOrDefault<int>(MakeSurveyAddressRequestsSection(true) + query + whereSection);
            result.Results = list;
            return result;
        }
        private string MakeSurveyAddressRequestsSection(bool iscount = false)
        {
            if (iscount)
                return "select count(*) ";
            else
                return @"SELECT   
                          CLPS.[ID]
                          ,CLPS.[LoanPlanId]
                          ,CLPS.[LoanId]
                          ,[RequestBy]
                          ,CLPS.[IsConfirmed]
                          ,CLPS.Description
                          ,CLPS.[RequestDate]
                          ,[VerifyBy]  as ConfirmUserId
                          ,[VerifyDate] as ConfirmDate
	                      ,s.Longitude as newLongitude
	                      ,s.Latitude as newLatitude
						  ,LP.Longitude as  currentLongitude
	                      ,LP.Latitude as currentLatitude
	                     ,b.BranchName
	                     ,b.SupervisorName
	                     ,loan.LoanNumber
						  ,LCC.BeginDate
	                      ,cust.FullName as fullCustomerName
	                      ,loan.SurveyLevel
	                      ,FLC.BankPayAmount as LoanAmount
	                      ,LST.[Desc] as LoanStatusTitle
	                      ,lrw.LastSurveyDate
	                      ,lrw.LastSurveyUserId
                          ,s.Id as LastSurveyId ";
        }

        private double checkDistsnce(string startLatitude,
            string startLongitude,
            string endLatitude,
            string endLongitude
            )
        {
            var distance = GeoCoordinates.Instance.ComputeDistance(startLatitude, startLongitude, endLatitude, endLongitude);

            return Math.Round(distance * 1000, 2);
        }

        public async Task<ResultObject<SurveyAddressRequestsVM>> GetAddressRequestsById(int Id)
        {
            var query = @"   FROM [survey].[ChangeLonaPlanState] as CLPS
                              left join [survey].[LoanPlanLocationState] as LPLS on LPLS.ChangeLoanPlanStateId= CLPS.ID
							  left join [survey].[Survey] as S on s.CartableId=LPLS.CartableId
							  left join [survey].[LoanPlan] as LP on LP.ID=CLPS.LoanPlanId
                              inner join [Core].[Loans] as loan on loan.Id= CLPS.LoanId
                              inner join [Core].[FactLoanAcc] as FLC on FLC.LoanId= loan.Id
                              inner join [Core].[LoanRedundantView] as lrw on lrw.LoanId= loan.Id
                              inner join [Core].[LoanStatusTypes] as LST on LST.Id= FLC.LoanStatusTypeId
                              inner Join Core.Customers as cust on loan.CustomerId = cust.Id
                              inner Join Core.Branches as b on loan.BranchCode = b.BranchCode
							  inner Join [Core].[LoanContracts] as LCC on loan.Id = LCC.LoanId
                              ";

            var whereSection = new StringBuilder();
            whereSection.AppendLine($"where CLPS.IsDeleted = 0 and CLPS.id={Id}");
            var finalQuery = MakeSurveyAddressRequestsSection() + query + whereSection;
            var list = await dapperHandler.Get<SurveyAddressRequestsVM>(finalQuery);

            return new ResultObject<SurveyAddressRequestsVM>()
            {
                Data = list,
            };
        }


        private async Task<(string Latitude, string Longitude)> GetSurveys(int Id)

        {
            string query = $@"SELECT 
		 Latitude
		,Longitude
		FROM [survey].[Survey] as s
		inner join [survey].[Cartable] as c on c.Id= s.CartableId
		where c.LoanId ={Id}
		order by s.SurveyDate desc
		OFFSET  1 ROW
		FETCH NEXT 1 ROWS ONLY";

            var list = await dapperHandler.Get<(string Latitude, string Longitude)>(query);
            return list;
        }

        public async Task<ResultObject<AddressRejectHistoryVM>> RejectAddressHistoryById(int Id)
        {
            var query = @"   FROM [survey].[ChangeLonaPlanState] as CLPS
                              left join [survey].[LoanPlanLocationState] as LPLS on LPLS.ChangeLoanPlanStateId= CLPS.ID
							  left join [survey].[Survey] as S on s.CartableId=LPLS.CartableId
							  left join [survey].[LoanPlan] as LP on LP.ID=CLPS.LoanPlanId
                              inner join [Core].[Loans] as loan on loan.Id= CLPS.LoanId
                              inner join [Core].[FactLoanAcc] as FLC on FLC.LoanId= loan.Id
                              inner join [Core].[LoanRedundantView] as lrw on lrw.LoanId= loan.Id
                              inner join [Core].[LoanStatusTypes] as LST on LST.Id= FLC.LoanStatusTypeId
                              inner Join Core.Customers as cust on loan.CustomerId = cust.Id
                              inner Join Core.Branches as b on loan.BranchCode = b.BranchCode
							  inner Join [Core].[LoanContracts] as LCC on loan.Id = LCC.LoanId
                              ";
            var list = await dapperHandler.Get<AddressRejectHistoryVM>(query);

            return new ResultObject<AddressRejectHistoryVM>()
            {
                Data = list,
            };
        }
    }
}
