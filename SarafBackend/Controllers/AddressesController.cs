using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopMicroservice.Application.Commands.Addresses;
using ShopMicroservice.Application.DTO;
using ShopMicroservice.Application.Query.Addresses;
using ShopMicroservice.Infrastructure.Security;

namespace ShopMicroservice.Controllers
{
    /// <summary>آدرس‌های کاربر لاگین‌شده. هر درخواست، لیست به‌روز آدرس‌ها را برمی‌گرداند.</summary>
    [ApiController]
    [Authorize]
    [Route("api/addresses")]
    public class AddressesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AddressesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public class AddressRequest
        {
            public string Title { get; set; } = string.Empty;
            public string? RecipientName { get; set; }
            public string? PhoneNumber { get; set; }
            public string? City { get; set; }
            public string? PostalCode { get; set; }
            public string Detail { get; set; } = string.Empty;
            public bool IsDefault { get; set; }
        }

        [HttpGet]
        public async Task<ActionResult<List<AddressDto>>> GetAll(CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new GetAddressesQuery(User.GetUserId()), cancellationToken));

        [HttpPost]
        public async Task<ActionResult<List<AddressDto>>> Create(
            [FromBody] AddressRequest request, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            await _mediator.Send(ToCommand(userId, null, request), cancellationToken);
            return Ok(await _mediator.Send(new GetAddressesQuery(userId), cancellationToken));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<List<AddressDto>>> Update(
            int id, [FromBody] AddressRequest request, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            await _mediator.Send(ToCommand(userId, id, request), cancellationToken);
            return Ok(await _mediator.Send(new GetAddressesQuery(userId), cancellationToken));
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<List<AddressDto>>> Delete(int id, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            await _mediator.Send(new DeleteAddressCommand { UserId = userId, AddressId = id }, cancellationToken);
            return Ok(await _mediator.Send(new GetAddressesQuery(userId), cancellationToken));
        }

        private static SetAddressCommand ToCommand(int userId, int? id, AddressRequest r) => new()
        {
            UserId = userId,
            Id = id,
            Title = r.Title,
            RecipientName = r.RecipientName,
            PhoneNumber = r.PhoneNumber,
            City = r.City,
            PostalCode = r.PostalCode,
            Detail = r.Detail,
            IsDefault = r.IsDefault
        };
    }
}
