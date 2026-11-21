using MediatR;
using Microsoft.AspNetCore.Mvc;
using ShopMicroservice.Application.DTO;
using ShopMicroservice.Application.Query.Categories;

namespace ShopMicroservice.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>دسته‌بندی‌های فعال (عمومی، برای منو و فیلترها).</summary>
        [HttpGet]
        public async Task<ActionResult<List<CategoryDto>>> GetAll(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetCategoriesQuery { IncludeInactive = false }, cancellationToken);
            return Ok(result);
        }
    }
}
