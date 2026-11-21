using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopMicroservice.Application.Commands.Carts;
using ShopMicroservice.Application.DTO;
using ShopMicroservice.Application.Query.Carts;
using ShopMicroservice.Infrastructure.Security;

namespace ShopMicroservice.Controllers
{
    /// <summary>سبد خرید کاربر لاگین‌شده. هر درخواست، سبد به‌روز را برمی‌گرداند.</summary>
    [ApiController]
    [Authorize]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CartController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public class AddCartItemRequest
        {
            public Guid VariantId { get; set; }
            public int Quantity { get; set; } = 1;
        }

        public class SetQuantityRequest
        {
            public int Quantity { get; set; }
        }

        public class MergeCartRequest
        {
            public List<MergeCartLine> Items { get; set; } = new();
        }

        [HttpGet]
        public async Task<ActionResult<CartDto>> Get(CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new GetCartQuery(User.GetUserId()), cancellationToken));

        [HttpPost("items")]
        public async Task<ActionResult<CartDto>> Add(
            [FromBody] AddCartItemRequest request, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            await _mediator.Send(new AddCartItemCommand
            {
                UserId = userId,
                VariantId = request.VariantId,
                Quantity = request.Quantity
            }, cancellationToken);
            return Ok(await _mediator.Send(new GetCartQuery(userId), cancellationToken));
        }

        [HttpPut("items/{variantId:guid}")]
        public async Task<ActionResult<CartDto>> SetQuantity(
            Guid variantId, [FromBody] SetQuantityRequest request, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            await _mediator.Send(new SetCartItemQuantityCommand
            {
                UserId = userId,
                VariantId = variantId,
                Quantity = request.Quantity
            }, cancellationToken);
            return Ok(await _mediator.Send(new GetCartQuery(userId), cancellationToken));
        }

        [HttpDelete("items/{variantId:guid}")]
        public async Task<ActionResult<CartDto>> Remove(Guid variantId, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            await _mediator.Send(new RemoveCartItemCommand { UserId = userId, VariantId = variantId }, cancellationToken);
            return Ok(await _mediator.Send(new GetCartQuery(userId), cancellationToken));
        }

        [HttpDelete]
        public async Task<ActionResult<CartDto>> Clear(CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            await _mediator.Send(new ClearCartCommand { UserId = userId }, cancellationToken);
            return Ok(await _mediator.Send(new GetCartQuery(userId), cancellationToken));
        }

        /// <summary>سبد مهمان (که در مرورگر ذخیره بود) را بعد از ورود با سبد سرور ادغام می‌کند.</summary>
        [HttpPost("merge")]
        public async Task<ActionResult<CartDto>> Merge(
            [FromBody] MergeCartRequest request, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            await _mediator.Send(new MergeCartCommand { UserId = userId, Lines = request.Items }, cancellationToken);
            return Ok(await _mediator.Send(new GetCartQuery(userId), cancellationToken));
        }
    }
}
