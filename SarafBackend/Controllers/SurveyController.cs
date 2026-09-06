using CommonLibrary.Core.Domain;
using CommonLibrary.Core.Domain.Enum;
using CommonLibrary.Infrastructure.JwtManagers;
using CommonLibrary.Infrastructure.Validators;
using LoanMonitoringMicroService.Application.Commands.Survey;
using LoanMonitoringMicroService.Application.Commands.Surveys;
using LoanMonitoringMicroService.Application.Queries.PlanNo;
using LoanMonitoringMicroService.Application.Queries.Survey;
using LoanMonitoringMicroService.Application.Queries.Surveys;
using LoanMonitoringMicroService.Core.Domain.ViewModel.Survey;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Stimulsoft.Report;
using Stimulsoft.Report.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LoanMonitoringMicroService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(JwtAuthorize))]
    public class SurveyController : BaseController
    {
        private readonly IMediator _mediator;
        protected readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;
        public SurveyController(IMediator mediator, Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment)
        {
            _mediator = mediator;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost("List")]
        public async Task<IActionResult> GetSurveysList([FromBody] GetSurveysListQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("referenceToMe/list")]
        public async Task<IActionResult> GetSurveysList([FromBody] GetReferenceToMeSurveyListQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("detail")]
        public async Task<IActionResult> GetSurveydetailsById([FromBody] GetSurveyDetailByIdQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet]
        [Route("document/print/{loanId}/{type}")]
        [Route("document/print/{loanId}/{cartableId}/{type}")]
        //[ActivityAuthorize(ConstActivities.GetSurveyDocument)]
        public async Task<IActionResult> GetSurveyDocumentPrint(int loanId, int cartableId = 0, int type=0)
        {
            var obj = await _mediator.Send(new GetSurveyDocumentQuery(loanId, cartableId));
            StiReport report = LoadPdfData(obj.Data, type);
            report.Render(true);
            return StiNetCoreReportResponse.ResponseAsPdf(report, true);
        }

        [HttpGet]
        [Route("document/printOutbound/{loanId}")]
       
        //[ActivityAuthorize(ConstActivities.GetSurveyDocument)]
        public async Task<IActionResult> GetSurveyDocumentprintOutboundt(int loanId, int cartableId = 0)
        {
            var obj = await _mediator.Send(new GetSurveyDocumentQuery(loanId, cartableId));
            return Ok(obj); 
        }

        private StiReport LoadPdfData(LoanSurveyDocumentVM obj,int type)
        {
            StiReport report = new StiReport();
            string Path = _hostingEnvironment.WebRootPath + "/ReportFiles/";
            Stimulsoft.Base.StiFontCollection.AddFontFile(@"Resources\fonts\B Nazanin.ttf");
            Stimulsoft.Base.Localization.StiLocalization.Localization = Path + "fa.xml";
            report.RegBusinessObject("LoanSurveyDocumentModel", obj);
            if (obj.LoanSurveyEconomicTypeId == 3 ||
                obj.LoanSurveyEconomicTypeId == 4&& type == 1)
                report.Load(Path + "SurveyReport.mrt");
            if (obj.LoanSurveyEconomicTypeId == 1 &&type==1)
                report.Load(Path + "PlanLivestockSurveyReport.mrt");
            if (obj.LoanSurveyEconomicTypeId == 2 && type ==1)
                report.Load(Path + "PlanGardenSurveyReport.mrt");
            if(type == 2)
                report.Load(Path + "OutSideSurveyReport.mrt");
            report.ReportName = "›—„ ‰Ÿ«—  ";
            return report;
        }

        [HttpGet]
        [Route("FillAllowedSupervisionLoan")]
        public async Task<IActionResult> FillAllowedSupervisionLoan()
        {
            var result = await _mediator.Send( new SetFillAllowedSupervisionLoanQuery());
            return Ok(result);
        }

        #region Outside 
        [HttpPost("Outside/Set")]
        public async Task<IActionResult> SetOutsideSurvey([FromBody] SetOutsideSurveyCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("Outside/list")]
        public async Task<IActionResult> GetOutsideSurveyList([FromBody] GetOutsideSurveyListQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        #endregion

        #region Wage 
        [HttpPost("Wage/list")]
        public async Task<IActionResult> GetSurveyWageList([FromBody] GetSurveyWageListQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        [Route("Wage/Confirm/MonthlyPayment")]
        [ActivityAuthorize(ConstActivities.ConfirmUsersMontlyPayment)]
        public async Task<IActionResult> ConfirmWageMonthlyPaymentCommand([FromBody] ConfirmWageMontlyPaymentCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet]
        [Route("Wage/document/print/{id}")]
        public async Task<IActionResult> GetWageDocumentPrint(int id)
        {
            var result = await _mediator.Send(new GetWageDocumentQuery(id));
            StiReport report = new StiReport();
            string Path = _hostingEnvironment.WebRootPath + "/ReportFiles/";
            Stimulsoft.Base.Localization.StiLocalization.Localization = Path + "fa.xml";
            report.RegBusinessObject("SurveyWageInfoReportModel", result);
            report.Load(Path + "SurveyPaymentReport.mrt");
            report.Render(true);
            return StiNetCoreReportResponse.ResponseAsPdf(report, true);
        }

        #endregion

        #region  RefrenceInfo
   

        [HttpPost("ReferenceBaseInfo")]
        public async Task<IActionResult> GetSurveyReferenceBaseInfo([FromBody] GetSurveyReferenceBaseInfoQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        [Route("LoanAmountCondition/Set")]
        public async Task<IActionResult> SetSurveyLoanAmountCondition([FromBody] SetSurveyLoanAmountConditionCommand Command)
        {
            var result = await _mediator.Send(Command);
            return Ok(result);
        }
        [HttpGet("LoanAmountCondition/Remove/{Id}")]
        public async Task<IActionResult> RemoveSurveyLoanAmountCondition(int Id)
        {
            var result = await _mediator.Send(new RemoveSurveyLoanAmountConditionCommand(Id));
            return Ok(result);
        }
        [HttpPost]
        [Route("ContractDateCondition/Set")]
        public async Task<IActionResult> SetSurveyReferenceContractDateCondition([FromBody] SetSurveyReferenceContractDateConditionCommand Command)
        {
            var result = await _mediator.Send(Command);
            return Ok(result);
        }
        [HttpGet("ContractDateCondition/Remove/{Id}")]
        public async Task<IActionResult> RemoveSurveyReferenceContractDateCondition(int Id)
        {
            var result = await _mediator.Send(new RemoveSurveyReferenceContractDateConditionCommand(Id));
            return Ok(result);
        }
        [HttpPost]
        [Route("ReagentCondition/Set")]
        public async Task<IActionResult> SetSurveyReferenceReagentCondition([FromBody] SetSurveyReferenceReagentConditionCommand Command)
        {
            var result = await _mediator.Send(Command);
            return Ok(result);
        }
        [HttpGet]
        [Route("ReagentCondition/Remove/{Id}")]
        public async Task<IActionResult> RemoveSurveyReferenceReagentCondition(int Id)
        {
            var result = await _mediator.Send( new RemoveSurveyReferenceReagentConditionCommand(Id));
            return Ok(result);
        }
        [HttpPost]
        [Route("AllowedFirstTimeSupervisionRole/Set")]
        public async Task<IActionResult> SetAllowedFirstTimeSupervisionRole([FromBody] SetAllowedFirstTimeSupervisionRoleCommand Command)
        {
            var result = await _mediator.Send(Command);
            return Ok(result);
        }
        [HttpGet("AllowedSupervision/Remove/{Id}")]
        public async Task<IActionResult> RemoveAllowedFirstTimeSupervisionRole(int Id)
        {
            var result = await _mediator.Send(new RemoveAllowedFirstTimeSupervisionRoleCommand(Id));
            return Ok(result);
        }
        [HttpPost]
        [Route("SurveyBaseInfo/Set")]
        public async Task<IActionResult> SetSurveyBaseInfo([FromBody] SetSurveyBaseInfoCommand Command)
        {
            var result = await _mediator.Send(Command);
            return Ok(result);
        }
        [HttpGet("SurveyBaseInfo/Remove/{Id}")]
        public async Task<IActionResult> RemoveSurveyBaseInfo(int Id)
        {
            var result = await _mediator.Send(new RemoveSurveyBaseInfoCommand(Id));
            return Ok(result);
        }
        [HttpPost]
        [Route("SurveyBaseInfo/Active")]
        public async Task<IActionResult> SetSurveyBaseInfo([FromBody] ActiveSurveyBaseInfoCommand Command)
        {
            var result = await _mediator.Send(Command);
            return Ok(result);
        }
        [HttpPost]
        [Route("SurveyMaxCountCondition/Set")]
        public async Task<IActionResult> SetSurveyReferenceMaxCountCondition([FromBody] SetSurveyMaxCountConditionCommand Command)
        {
            var result = await _mediator.Send(Command);
            return Ok(result);
        }
        [HttpGet("SurveyMaxCountCondition/Remove/{Id}")]
        public async Task<IActionResult> RemoveSurveyReferenceMaxCountCondition(int Id)
        {
            var result = await _mediator.Send(new RemoveSurveyReferenceMaxCountConditionCommand(Id));
            return Ok(result);
        }

        [HttpPost("SurveyMaxCountCondition/list")]
        public async Task<IActionResult> GetSurveyMaxCountCondition([FromBody] GetSurveyMaxCountConditionQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("Address/requests/list")]
        public async Task<IActionResult> GetAddressRequests([FromBody] GetSurveyAddressRequestsListQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("Address/approve/{id}/{SurveyId}")]
        public async Task<IActionResult> ApprovePlanNoHistoryRequest(int id,int SurveyId)
        {
            var result = await _mediator.Send(new ConfirmSurveyAddressRequestCommand(id,SurveyId));
            return Ok(result);
        }

        [HttpGet("Address/requests/{id}")]
        public async Task<IActionResult> GetAddressRequestsById(int id)
        {
            var result = await _mediator.Send(new GetAddressRequestsById(id));
            return Ok(result);
        }


        [HttpPost("Address/reject")]
        public async Task<IActionResult> RejectPlanNoHistoryRequest([FromBody] RejectSurveyAddressRequestCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("Address/reject/History/{Id}")]
        public async Task<IActionResult> RejectAddressHistoryList(int Id)
        {
            var result = await _mediator.Send(new RejectAddressHistoryByIdQuery(Id));
            return Ok(result);
        }
        #endregion
    }
}
