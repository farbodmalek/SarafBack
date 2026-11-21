using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopMicroservice.Application.Commands.Categories;
using ShopMicroservice.Application.Commands.Orders;
using ShopMicroservice.Application.Commands.Product;
using ShopMicroservice.Application.DTO;
using ShopMicroservice.Application.Query.Categories;
using ShopMicroservice.Application.Query.Dashboard;
using ShopMicroservice.Application.Query.Orders;
using ShopMicroservice.Application.Query.Product;

namespace ShopMicroservice.Controllers
{
    /// <summary>همه‌ی endpoint های پنل ادمین. فقط کاربری که توکنش role=Admin دارد وارد می‌شود.</summary>
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public class StatusRequest
        {
            public string Status { get; set; } = string.Empty;
        }

        public class CategoryRequest
        {
            public string Name { get; set; } = string.Empty;
            public string? Slug { get; set; }
            public long? ParentId { get; set; }
            public bool IsActive { get; set; } = true;
        }

        // ------------------------------ داشبورد ------------------------------
        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardDto>> Dashboard(CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new GetDashboardQuery(), cancellationToken));

        // ------------------------------ سفارش‌ها ------------------------------
        [HttpGet("orders")]
        public async Task<ActionResult<PagedResult<OrderListItemDto>>> Orders(
            [FromQuery] OrderFilterDto filter, CancellationToken cancellationToken)
        {
            filter.PageSize = Math.Clamp(filter.PageSize, 1, 100);
            return Ok(await _mediator.Send(new GetAdminOrdersQuery { Filter = filter }, cancellationToken));
        }

        [HttpGet("orders/{orderNumber}")]
        public async Task<ActionResult<OrderDetailDto>> Order(string orderNumber, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAdminOrderQuery(orderNumber), cancellationToken);
            return result is null ? NotFound(new { message = "سفارش پیدا نشد." }) : Ok(result);
        }

        [HttpPut("orders/{orderNumber}/status")]
        public async Task<IActionResult> UpdateOrderStatus(
            string orderNumber, [FromBody] StatusRequest request, CancellationToken cancellationToken)
        {
            await _mediator.Send(new UpdateOrderStatusCommand
            {
                OrderNumber = orderNumber,
                Status = request.Status
            }, cancellationToken);
            return NoContent();
        }

        // ------------------------------ دسته‌بندی‌ها ------------------------------
        [HttpGet("categories")]
        public async Task<ActionResult<List<CategoryDto>>> Categories(CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new GetCategoriesQuery { IncludeInactive = true }, cancellationToken));

        [HttpPost("categories")]
        public async Task<IActionResult> CreateCategory(
            [FromBody] CategoryRequest request, CancellationToken cancellationToken)
        {
            var id = await _mediator.Send(ToCommand(null, request), cancellationToken);
            return Ok(new { id });
        }

        [HttpPut("categories/{id:long}")]
        public async Task<IActionResult> UpdateCategory(
            long id, [FromBody] CategoryRequest request, CancellationToken cancellationToken)
        {
            await _mediator.Send(ToCommand(id, request), cancellationToken);
            return NoContent();
        }

        [HttpDelete("categories/{id:long}")]
        public async Task<IActionResult> DeleteCategory(long id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteCategoryCommand(id), cancellationToken);
            return NoContent();
        }

        // ------------------------------ محصولات ------------------------------
        /// <summary>لیست همه‌ی محصولات (حتی پیش‌نویس و بایگانی‌شده) همراه مجموع موجودی.</summary>
        [HttpGet("products")]
        public async Task<ActionResult<PagedResult<AdminProductListItemDto>>> Products(
            [FromQuery] ProductFilterDto filter, CancellationToken cancellationToken)
        {
            filter.PageSize = Math.Clamp(filter.PageSize, 1, 100);
            return Ok(await _mediator.Send(new GetAdminProductsQuery { Filter = filter }, cancellationToken));
        }

        /// <summary>منتشر کردن / پیش‌نویس / بایگانی (Draft | Published | Archived).</summary>
        [HttpPut("products/{id:guid}/status")]
        public async Task<IActionResult> SetProductStatus(
            Guid id, [FromBody] StatusRequest request, CancellationToken cancellationToken)
        {
            await _mediator.Send(new SetProductStatusCommand { Id = id, Status = request.Status }, cancellationToken);
            return NoContent();
        }

        private static SetCategoryCommand ToCommand(long? id, CategoryRequest r) => new()
        {
            Id = id,
            Name = r.Name,
            Slug = r.Slug,
            ParentId = r.ParentId,
            IsActive = r.IsActive
        };
    }
}
