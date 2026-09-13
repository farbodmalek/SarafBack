using MediatR;
using Microsoft.AspNetCore.Mvc;
using ShopMicroservice.Application.Commands.Product;
using ShopMicroservice.Application.DTO;
using ShopMicroservice.Application.Query.Product;

namespace ShopMicroservice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>ثبت یا ویرایش محصول.</summary>
        [HttpPost("Set")]
        public async Task<ActionResult<Guid>> Set(
            [FromBody] SetProductRequestCommand command, CancellationToken cancellationToken)
        {
            var id = await _mediator.Send(command, cancellationToken);
            return Ok(id);
        }

        /// <summary>دریافت جزئیات یک محصول (شامل واریانت‌ها و تصاویر).</summary>
        [HttpGet("Info/{id:guid}")]
        public async Task<ActionResult<ProductDetailDto>> GetById(
            Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetProductDetailByIdQuery(id), cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }

        /// <summary>لیست جامع محصولات با فیلتر (قیمت، برند، جستجو و ...).</summary>
        [HttpPost("List")]
        public async Task<ActionResult<PagedResult<ProductListItemDto>>> GetComprehensive(
            [FromBody] GetComprehensiveProductQuery  query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>لیست محصولات یک دسته‌بندی خاص.</summary>
        [HttpGet("category/{categoryId:long}")]
        public async Task<ActionResult<PagedResult<ProductListItemDto>>> GetByCategory(
            long categoryId, [FromQuery] ProductFilterDto filter, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetProductListByCategoryQuery { CategoryId = categoryId, Filter = filter },
                cancellationToken);
            return Ok(result);
        }

        /// <summary>لیست محصولات ویژه/شاخص (برای اسلایدر صفحه اصلی).</summary>
        [HttpGet("featured")]
        public async Task<ActionResult<PagedResult<ProductListItemDto>>> GetFeatured(
            [FromQuery] ProductFilterDto filter, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetFeaturedProductsQuery { Filter = filter }, cancellationToken);
            return Ok(result);
        }

        /// <summary>حذف یک محصول.</summary>
        [HttpDelete("Remove/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteProductCommand(id), cancellationToken);
            return NoContent();
        }
    }
}
