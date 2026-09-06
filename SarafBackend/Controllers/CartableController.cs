using CommonLibrary.Core.Domain;
using CommonLibrary.Core.Domain.Enum;
using CommonLibrary.Infrastructure.JwtManagers;
using CommonLibrary.Infrastructure.Validators;
using LoanMonitoringMicroService.Application.Commands.Cartable;
using LoanMonitoringMicroService.Application.Queries.Cartable;
using LoanMonitoringMicroService.Application.Queries.Surveys;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LoanMonitoringMicroService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(JwtAuthorize))]
    public class CartableController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CartableController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("survey/list")]
        public async Task<IActionResult> GetCartableSurveyList([FromBody] GetCartableSurveyList query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }


        [HttpGet("last/{LoanId}")]
        public async Task<IActionResult> GetCartableSurveyList(int LoanId)
        {
            var result = await _mediator.Send(new GetLastCartableLoanIdQuery(LoanId));
            return Ok(result);
        }

        [HttpPost("survey/Set")]
        [ActivityAuthorize(ConstActivities.SetCartable)]
        public async Task<IActionResult> SetCartableSurvey([FromBody] SetCartableCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("survey/list/remove")]
        [ActivityAuthorize(ConstActivities.RemoveCartable)]
        public async Task<IActionResult> RemoveCartable([FromBody] RemoveCartableListCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("survey/remove/{id}")]
        [ActivityAuthorize(ConstActivities.RemoveCartable)]
        public async Task<IActionResult> RemoveCartable(int id)
        {
            var result = await _mediator.Send(new RemoveCartableByIDCommand(id));
            return Ok(result);
        }

        [HttpPost("survey/remove")]
        [ActivityAuthorize(ConstActivities.RemoveCartable)]
        public async Task<IActionResult> RemoveCartable([FromBody] RemoveCartableCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
