using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopMicroservice.Application.Commands.Wishlists;
using ShopMicroservice.Application.DTO;
using ShopMicroservice.Application.Query.Wishlists;
using ShopMicroservice.Infrastructure.Security;

namespace ShopMicroservice.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/wishlist")]
    public class WishlistController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WishlistController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>محصولات علاقه‌مندی (با اطلاعات کارت محصول).</summary>
        [HttpGet]
        public async Task<ActionResult<List<ProductListItemDto>>> GetAll(CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new GetWishlistQuery(User.GetUserId()), cancellationToken));

        /// <summary>فقط شناسه‌ها؛ برای پر کردن قلب‌ها در لیست محصولات.</summary>
        [HttpGet("ids")]
        public async Task<ActionResult<List<Guid>>> GetIds(CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new GetWishlistProductIdsQuery(User.GetUserId()), cancellationToken));

        [HttpPost("{productId:guid}")]
        public async Task<IActionResult> Add(Guid productId, CancellationToken cancellationToken)
        {
            await _mediator.Send(new AddToWishlistCommand { UserId = User.GetUserId(), ProductId = productId }, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{productId:guid}")]
        public async Task<IActionResult> Remove(Guid productId, CancellationToken cancellationToken)
        {
            await _mediator.Send(new RemoveFromWishlistCommand { UserId = User.GetUserId(), ProductId = productId }, cancellationToken);
            return NoContent();
        }
    }
}
