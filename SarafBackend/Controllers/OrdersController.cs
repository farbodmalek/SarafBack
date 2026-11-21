using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopMicroservice.Application.Commands.Orders;
using ShopMicroservice.Application.DTO;
using ShopMicroservice.Application.Query.Orders;
using ShopMicroservice.Core.Domain;
using ShopMicroservice.Infrastructure.Security;

namespace ShopMicroservice.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public class PlaceOrderRequest
        {
            public int AddressId { get; set; }
            public string ShippingMethod { get; set; } = string.Empty;
            public string PaymentMethod { get; set; } = string.Empty;
        }

        /// <summary>روش‌های ارسال و پرداخت با قیمت (عمومی).</summary>
        [HttpGet("options")]
        [AllowAnonymous]
        public IActionResult Options()
        {
            var shipping = OrderOptions.ShippingMethods
                .Select(kv => new { id = kv.Key, title = kv.Value.Title, price = kv.Value.Price });
            var payment = OrderOptions.PaymentMethods
                .Select(kv => new { id = kv.Key, title = kv.Value });
            return Ok(new { shippingMethods = shipping, paymentMethods = payment });
        }

        /// <summary>ثبت سفارش از روی سبد خرید سرور. خروجی: شماره‌ی سفارش (مثل AV-100001).</summary>
        [HttpPost]
        public async Task<IActionResult> Place([FromBody] PlaceOrderRequest request, CancellationToken cancellationToken)
        {
            var orderNumber = await _mediator.Send(new PlaceOrderCommand
            {
                UserId = User.GetUserId(),
                AddressId = request.AddressId,
                ShippingMethod = request.ShippingMethod,
                PaymentMethod = request.PaymentMethod
            }, cancellationToken);

            return Ok(new { orderNumber });
        }

        [HttpGet]
        public async Task<ActionResult<List<OrderListItemDto>>> GetMine(CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new GetUserOrdersQuery(User.GetUserId()), cancellationToken));

        [HttpGet("{orderNumber}")]
        public async Task<ActionResult<OrderDetailDto>> GetMineByNumber(
            string orderNumber, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetUserOrderQuery(User.GetUserId(), orderNumber), cancellationToken);
            return result is null ? NotFound(new { message = "سفارش پیدا نشد." }) : Ok(result);
        }

        /// <summary>لغو سفارش (فقط وقتی «در حال پردازش» است).</summary>
        [HttpPost("{orderNumber}/cancel")]
        public async Task<IActionResult> Cancel(string orderNumber, CancellationToken cancellationToken)
        {
            await _mediator.Send(new CancelOrderCommand { UserId = User.GetUserId(), OrderNumber = orderNumber }, cancellationToken);
            return NoContent();
        }
    }
}
