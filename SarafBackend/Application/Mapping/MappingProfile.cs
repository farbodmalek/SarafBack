using AutoMapper;
using LoanMonitoringMicroService.Application.DTO;
using LoanMonitoringMicroService.Application.DTO.Pwa;
using LoanMonitoringMicroService.Application.DTO.Survey;
using LoanMonitoringMicroService.Application.Queries.PlanNo;
using LoanMonitoringMicroService.Application.Queries.Survey;
using LoanMonitoringMicroService.Applications.AppService.ServiceDto.Survey;
using LoanMonitoringMicroService.Applications.AppService.ServiceDto.Surveys;
using LoanMonitoringMicroService.Core.Domain.Entities;
using LoanMonitoringMicroService.Core.Domain.ViewModel.PlanNo;
using LoanMonitoringMicroService.Core.Domain.ViewModel.Pwa;
using LoanMonitoringMicroService.Core.Domain.ViewModel.Survey;
using LoanMonitoringMicroService.Domains.Core.Supervision.Entities;
using LoanMonitoringMicroService.Domains.Core.Supervision.Entities.Dao;
using LoanMonitoringMicroService.Domains.Supervision.Entities;
using PaymentMoLoanMonitoringMicroServicenitoring.Applications.AppService.ServiceDto.Survey;


namespace LoanMonitoringMicroService.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //CreateMap<SurveyLoanDto, SurveyLoanDto>().ReverseMap();
            CreateMap<SurveyWageVM, SurveyWageDto>().ReverseMap();
            CreateMap<OutsideSurveyDTO, OutsideSurveyVM>().ReverseMap();
            CreateMap<SurveyDetailDTO, SurveyDetailVM>().ReverseMap();
            CreateMap<LoanPlanMarkerFilterVM, GetPlanMarkerListQuery>().ReverseMap();
            CreateMap<SurveyFilterVM, SurveyFilterDTO>().ReverseMap();
            CreateMap<SurveyVM, SurveyDTO>().ReverseMap();
            CreateMap<SurveyFilterVM, GetReferenceToMeSurveyListQuery>().ReverseMap();

            CreateMap<PlanNoVM, PlanNoDto>().ReverseMap();
            CreateMap<Core.Domain.ViewModel.PlanNo.LoanPlanNoDto, LoanPlanNoVM>().ReverseMap();
            CreateMap<Core.Domain.ViewModel.PlanNo.LoanPlanNoDto, Core.Domain.ViewModel.PlanNo.LoanPlanNoDto>().ReverseMap();
            CreateMap<PlanNoHistoryFilterVM, GetPlanNoHistoryRequestsQuery>().ReverseMap();
            CreateMap<SurveyCartableDTO, SurveyCartableVM>().ReverseMap();
            CreateMap<PlanServiceSurvey, PlanServiceVM>().ReverseMap();
            CreateMap<PlanIndustrialSurvey, PlanIndustrialVM>().ReverseMap();
            CreateMap<PlanGardenSurvey, PlanGardenVM>().ReverseMap();
            CreateMap<PlanLivestockSurvey, PlanLivestockVM>().ReverseMap();
            CreateMap<SurveyReferenceBaseInfoVM, SurveyReferenceBaseInfoDTO>();
            CreateMap<SurveyReferenceLoanAmount, SurveyReferenceLoanAmountDTO>();
            CreateMap<SurveyReferenceReagentVM, SurveyReferenceReagentDTO>();
            CreateMap<SurveyReferenceContractDate, SurveyReferenceContractDateDTO>();
            CreateMap<AllowedFirstTimeSupervisionRole, AllowedFirstTimeSupervisionRoleDTO>();
            CreateMap<SurveyReferenceMaxCountDTO, SurveyReferenceMaxCountVM>().ReverseMap();
            CreateMap<SurveyAddressRequestsFilterVM, GetSurveyAddressRequestsListQuery>().ReverseMap();

        }
    }
}
