using CommonLibrary.Core.Domain;
using CommonLibrary.Core.Domain.Dto;
using CommonLibrary.Core.Domain.Interfaces;
using CommonLibrary.Core.Domain.RepositoryInterfaces;
using LoanMonitoringMicroService.Application.DTO.Pwa;
using LoanMonitoringMicroService.Applications.AppService.ServiceDto.Survey;
using LoanMonitoringMicroService.Core.Domain.RepositoryInterfaces;
using LoanMonitoringMicroService.Core.Domain.ViewModel.Pwa;
using LoanMonitoringMicroService.Core.DomainServices.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace LoanMonitoringMicroService.Core.DomainServices.Cartables
{
    public class PwaRepositoryRead : IPwaRepositoryRead
    {

        private readonly IDapperHandler dapperHandler;
        private readonly IDateConvertor dateConvertor;
        private readonly ILoanMonitoringDbContext Context;
        private readonly LoginUserDTO loginUser;
        public PwaRepositoryRead(IDapperHandler _dapperHandler, IDateConvertor _dateConvertor, ILoanMonitoringDbContext Context, LoginUserDTO _loginUser)
        {
            this.dapperHandler = _dapperHandler;
            this.dateConvertor = _dateConvertor;
            this.Context = Context;
            this.loginUser = _loginUser;
        }
        public async Task<ResultList<SurveyCartableVM>> GetSurveysCartableList(SurveyCartableFilterDTO request)
        {
            var whereSection = $" WHERE cartable.UserId={loginUser.ID} and cartable.CartableStatusTypeId=1 and cartable.IsDeleted=0 and cartable.[ExpireDate]>= GETDATE()";
            var pagingQuery = $@" ORDER BY cartable.Id desc OFFSET {request.Skip} ROWS FETCH NEXT {request.Take} ROWS ONLY ";
            var query = MakeCartableSelectQuery(request) + MakeCartableTableQuery(request)  + whereSection.ToString() + pagingQuery;
            var list = await this.dapperHandler.GetAll<SurveyCartableVM>(query);
            var defaultplanNo = await Context.PlanNos.FirstOrDefaultAsync(c => c.IsDeleted);
            foreach (var cartable in list)
            {
                cartable.SurveDetail = await GetSurveyDetailById(cartable.LoanId);         
            }

            var result = new ResultList<SurveyCartableVM>()
            {
                Results = list,
                TotalRows = await this.dapperHandler.Get<int>(MakeCartableSelectQuery(request, true) + MakeCartableTableQuery(request)  + whereSection.ToString())
            };
            return result;
        }

        private async Task<SurveDetailVM> GetSurveyDetailById(int LoanId)
        {
            var result = new SurveDetailVM();
            var query = MakeSurveyDetailQuery(LoanId);
            var obj = await this.dapperHandler.Get<SurveDetailVM>(query);
            result = obj;
            return result;
        }

        private string MakeCartableSelectQuery(SurveyCartableFilterDTO request, bool isCount = false)
        {
            if (isCount)
                return @"SELECT COUNT(distinct loan.Id)
                        ";
            else
                return @"select 
		                          cartable.Id as CartableId,
		                          loan.Id as loanId
		                          ,customer.FullName as customerName
		                          ,loan.CustomerNumber
		                          ,acc.LoanAmount
		                          ,acc.EachInstallmentAmount
		                          ,acc.TotalDelayedAmount
		                          ,acc.NumberOfDelayedInstallment
		                          ,customer.MobileNo as customerMobileNo
		                          ,cartable.CartableStatusTypeId
		                          ,cartable.[ExpireDate] as ExpireDate
	                              ,loanminorType.[Desc] as LoanMinorTypeDesc
		                          ,lp.ID loanplanId
		                          ,lp.IsFamilySupervisor
		                          ,lp.Latitude,lp.Longitude
	                              ,lp.MaritalStatusId
		                          ,lp.EducationTypeId
		                          ,lp.MobileNo
		                          ,lp.Phone
		                          ,lp.PlanTypeId
		                          ,lp.ResidentTypeId
		                          ,lp.[Address]
		                          ,lp.GenderType
		                          ,customer.Gender as customerGenderType
		                          ,lc.LoanEconomicTypeId,
		                          lc.LoanNumber	        
                                  ,IIF(pn.Id is null OR pn.Id <1
		                          ,(SELECT top(1) pn2.id 
                                   FROM survey.LoanPlanNo  lpn 
                                   LEFT JOIN survey.PlanNo  
		                           pn2 ON lpn.PlanNoId=pn2.Id
		                         WHERE lpn.LoanId  =cartable.LoanId  order by pn2.CreateDate desc),pn.Id) as PlanNoId,
		                         pn.[Name] as PlanNoName
                                 ,lp.LoanSurveyEconomidTypeId as LoanSurveyEconomicTypeId
		                         ,let.Name as LoanSurveyEconomicTypeName
                                 ,lp.OtherPlanNo    
                                 ,loan.CustomerId
                                 ";
        }

        private string MakeCartableTableQuery(SurveyCartableFilterDTO request, bool isCount = false)
        {
            var table = new StringBuilder();

            table.Append(@$" 
                        FROM survey.Cartable cartable
							LEFT JOIN survey.Survey as s on s.CartableId = cartable.Id
							JOIN core.Loans as loan on cartable.LoanId = loan.Id
							JOIN core.FactLoanAcc as acc on loan.Id = acc.LoanId
                            JOIN core.LoanContracts lc on  loan.Id = lc.LoanId
							JOIN core.Customers as customer on loan.CustomerId=customer.Id
							LEFT JOIN survey.loanplan as lp on loan.Id =lp.LoanId and lp.IsDeleted=0
                            LEFT JOIN survey.LoanPlanNo lpn on loan.Id=lpn.LoanId and lpn.IsDeleted=0
							JOIN core.LoanMinorTypes as loanminorType on loan.LoanMinorTypeId=loanminorType.Id 
                          	LEFT JOIN survey.PlanNo pn on lpn.PlanNoId = pn.Id and pn.Approved = 1 and pn.Approved is not null
							LEFT JOIN survey.LoanSurveyEconomicType let on lp.LoanSurveyEconomidTypeId=let.Id");

            return table.ToString();
        }

        private string MakeSurveyDetailQuery( int LoanId)
        {
            var table = new StringBuilder();

            table.Append(@$" 
                     SELECT c.[Id] as CartableId
                                 ,c.LoanId AS id
	                            ,s.Id as SurveyId
                                ,[ReferenceDate]
                                ,[ExpireDate]
                                ,[CartableStatusTypeId]
                                ,[UserId]
                                ,c.[LoanId]
                                ,c.[LoanSurveyEconomicTypeId]
                                ,c.[PlanNoId]
                                ,[EndDate]
                                ,[ReferenceUserId]
                                ,[Description]
                                ,[EndOfActivationDate]
                                ,[ConstructionApproval]
                                ,[ConstructionPercentageProgress]
                                ,[ConstructionDescription]
                                ,[IsEquipmentBought2]
                                ,[IsFactorMatch]
                                ,[EquipmentDescription]
                                ,[SurveyReport]
                                ,[CustomerOffer]
                                ,[NumberOfJobsCreated]
                                ,[NumberOfInsurdPerson]
                                ,[SurveyDate]
                                ,[EquipmentTypeId]
                                ,[PlanActivationTypeId]
                                ,[IsOffline]
                                ,[OfflineDate]
                                ,[IsEquipmentBought]
	                            ,lp.WorkshopCode AS WorkShopCode
	                            ,pls.Id as planLivestockSurvey -- دامپروری
								,pls.LivestockBooklet -- دفترچه دامداری
								,pls.LivestockLicense-- پروانه دامداری
								,pls.LivestockInsurance-- بیمهنامه دامداری
								,pls.InsuranceDate-- تاریخ بیمه نامه
								,pls.Id As insuranceTypeId 
								,pls.NumberOfInsuredLivestock-- تعداد ام بیمه شده
								,pls.LivestockTypeId-- تعداد دام
								,pls.NumberOfMaleLivestock-- تعداد نر
								,pls.NumberOfFemaleLivestock -- تعداد ماده
								,pgs.ProductTypeId--نوع محصول
								,lp.InsuranceTypeId
                                ,pgs.LandArea-- وسعت زمین
								,pgs.CultivatedLandArea-- میزان زیر کشت
								,pgs.HasAgriculturalInsurance-- بیمه نامه کشاورزس
								,pgs.EndOfAgriculturalInsurance -- تاریخ پایان بیمه نامه
								,indus.Id as planIndustrialSurvey -- صنعتی
								,pss.Id as planServiceSurvey -- خدمات
	                            ,isnull(pss.HasWorkPermission,0) as HasWorkPermission --شناسه مجوز کار
	                            ,case When isnull(s.NumberOfInsurdPerson,0)=0 then 0 Else 1 END HaseInsuranceList
	                            ,case when isnull(pss.OwnerTypeId,0)   <> 0 then pss.OwnerTypeId 
									when isnull(pgs.OwnerTypeId,0)   <> 0 then pgs.OwnerTypeId  
									when isnull(indus.OwnerTypeId,0) <> 0 then indus.OwnerTypeId 
									else 0 end   as OwnerTypeId   --شناسه مالکیت
                             FROM [survey].[Cartable] as c
                            inner join [survey].[Survey] as S on s.CartableId=c.Id
                            LEFT JOIN survey.LoanPlan as lp on c.LoanId=lp.LoanId
                            LEFT JOIN survey.PlanGardenSurvey as pgs on pgs.SurveyId = s.Id
							LEFT JOIN survey.OwnerType as pot on pgs.OwnerTypeId=pot.Id
							LEFT JOIN survey.PlanLivestockSurvey as pls on pls.SurveyId = s.Id
							LEFT JOIN survey.PlanServiceSurvey as pss ON pss.SurveyId = s.Id
							LEFT JOIN survey.OwnerType as ot on pss.OwnerTypeId=ot.Id
							LEFT JOIN survey.PlanIndustrialSurvey as indus on indus.SurveyId = s.Id
							LEFT JOIN survey.OwnerType as iot on indus.OwnerTypeId=iot.Id
                              where c.LoanId={LoanId} and  CartableStatusTypeId=2
                              order by s.SurveyDate desc");

            return table.ToString();
        }


    }
}
