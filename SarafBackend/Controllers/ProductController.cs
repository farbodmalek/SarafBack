using MediatR;
using Microsoft.AspNetCore.Authorization;
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

        /// <summary>ثبت یا ویرایش محصول (فقط ادمین).</summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Guid>> Set(
            [FromBody] SetProductRequestCommand command, CancellationToken cancellationToken)
        {
            var id = await _mediator.Send(command, cancellationToken);
            return Ok(id);
        }

        /// <summary>دریافت جزئیات یک محصول با شناسه، با همه‌ی وضعیت‌ها (برای فرم ویرایش ادمین).</summary>
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductDetailDto>> GetById(
            Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetProductDetailByIdQuery(id), cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }

        /// <summary>دریافت جزئیات یک محصول با اسلاگ (برای صفحه محصول در فرانت).</summary>
        [HttpGet("slug/{slug}")]
        public async Task<ActionResult<ProductDetailDto>> GetBySlug(
            string slug, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetProductBySlugQuery(slug), cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }

        /// <summary>لیست جامع محصولات با فیلتر (قیمت، برند، جستجو و ...).</summary>
        [HttpGet]
        public async Task<ActionResult<PagedResult<ProductListItemDto>>> GetComprehensive(
            [FromQuery] ProductFilterDto filter, CancellationToken cancellationToken)
        {
            filter.PageSize = Math.Clamp(filter.PageSize, 1, 100);
            var result = await _mediator.Send(
                new GetComprehensiveProductQuery { Filter = filter }, cancellationToken);
            return Ok(result);
        }

        /// <summary>لیست محصولات یک دسته‌بندی خاص.</summary>
        [HttpGet("category/{categoryId:long}")]
        public async Task<ActionResult<PagedResult<ProductListItemDto>>> GetByCategory(
            long categoryId, [FromQuery] ProductFilterDto filter, CancellationToken cancellationToken)
        {
            filter.PageSize = Math.Clamp(filter.PageSize, 1, 100);
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
            filter.PageSize = Math.Clamp(filter.PageSize, 1, 100);
            var result = await _mediator.Send(new GetFeaturedProductsQuery { Filter = filter }, cancellationToken);
            return Ok(result);
        }

        /// <summary>حذف یک محصول (فقط ادمین).</summary>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteProductCommand(id), cancellationToken);
            return NoContent();
        }
    }
}
