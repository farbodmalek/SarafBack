using CommonLibrary.Infrastructure.JwtManagers;
using LoanMonitoringMicroService.Application.Commands.Surveys;
using LoanMonitoringMicroService.Application.Queries.PlanNo;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LoanMonitoringMicroService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(JwtAuthorize))]
    public class PwaController : BaseController
    {
        private readonly IMediator _mediator;

        public PwaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("Surveys/List")]
        public async Task<IActionResult> GetSurveysList([FromBody] GetSurveysCartableListQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        [HttpPost("Survey/Set")]
        public async Task<IActionResult> SetSurveysList([FromBody] SetSurveyCommand query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
