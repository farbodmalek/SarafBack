using LoanMonitoringMicroService.Application.Commands.PlanNo;
using LoanMonitoringMicroService.Application.Queries.PlanNo;
using LoanMonitoringMicroService.Application.Queries.Survey;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LoanMonitoringMicroService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanNoController : ControllerBase
    {
        protected readonly IMediator _mediator;
        public PlanNoController(IMediator mediator)
        {
            this._mediator = mediator;
        }
        //SetLoanPlanNo
        /// <summary>
        /// تغییر رشته فعالیت در ارجاعات چرخ دنده
        /// </summary>
        /// <param name="loanPlanNo"></param>
        /// <returns></returns>
        [HttpPost("Loan/set")]
        public async Task<IActionResult> SetLoanPlanNoAsync([FromBody] SetLoanPlanNoCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        //SetPlanNo
        [HttpPost("set")]
        public async Task<IActionResult> SetPlanNoAsync([FromBody] SetPlanNoCommand command)
        {
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpPost("List")]
        public async Task<IActionResult> GetPlanNoListAsync([FromBody] GetPlanNoListQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("requests/list")]
        public async Task<IActionResult> GetPlanNoHistoryRequests([FromBody] GetPlanNoHistoryRequestsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("request/approve/{id}/{planNoId}")]
        public async Task<IActionResult> ApprovePlanNoHistoryRequest(int id, int planNoId)
        {
            var result = await _mediator.Send(new ChangePlanNoHistoryRequestCommand(id, true, planNoId));
            return Ok(result);
        }

        [HttpPost("request/reject")]
        public async Task<IActionResult> RejectPlanNoHistoryRequest([FromBody] RejectPlanNoHistoryRequestCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("Loan/PlanMarkers")]
        public async Task<IActionResult> GetLoanPlanMarkers([FromBody] GetPlanMarkerListQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        //GetPlanNoInfoList
        [HttpPost("info/list")]
        public async Task<IActionResult> GetPlanNoInfoListAsync([FromBody] GetPlanNoInfoListQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        //LoanPlanNoList
        [HttpPost]
        [Route("loan/list")]
        public async Task<IActionResult> GetLoanPlanNoList([FromBody] GetLoanPlanNoListQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        //RemovePlanNo
        [HttpGet("remove/{id}")]
        public async Task<IActionResult> RemovePlanNo(int id)
        {
            var result = await _mediator.Send(new RemovePlanNoCommand(id));
            return Ok(result);
        }

        //ChangeLoansPlanNo
        [HttpPost("change/LoansPlanNo")]
        public async Task<IActionResult> ChangeLoansPlanNo([FromBody] ChangeLoansPlanNoCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
