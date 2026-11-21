using MediatR;
using ShopMicroservice.Application.DTO;
using ShopMicroservice.Core.Interfaces;

namespace ShopMicroservice.Application.Query.Orders
{
    public class GetUserOrdersQuery : IRequest<List<OrderListItemDto>>
    {
        public int UserId { get; set; }

        public GetUserOrdersQuery(int userId)
        {
            UserId = userId;
        }
    }

    public class GetUserOrdersQueryHandler : IRequestHandler<GetUserOrdersQuery, List<OrderListItemDto>>
    {
        private readonly IOrderRepositoryRead _repository;

        public GetUserOrdersQueryHandler(IOrderRepositoryRead repository)
        {
            _repository = repository;
        }

        public Task<List<OrderListItemDto>> Handle(GetUserOrdersQuery request, CancellationToken cancellationToken)
            => _repository.GetUserOrdersAsync(request.UserId, cancellationToken);
    }

    public class GetUserOrderQuery : IRequest<OrderDetailDto?>
    {
        public int UserId { get; set; }
        public string OrderNumber { get; set; }

        public GetUserOrderQuery(int userId, string orderNumber)
        {
            UserId = userId;
            OrderNumber = orderNumber;
        }
    }

    public class GetUserOrderQueryHandler : IRequestHandler<GetUserOrderQuery, OrderDetailDto?>
    {
        private readonly IOrderRepositoryRead _repository;

        public GetUserOrderQueryHandler(IOrderRepositoryRead repository)
        {
            _repository = repository;
        }

        public Task<OrderDetailDto?> Handle(GetUserOrderQuery request, CancellationToken cancellationToken)
            => _repository.GetUserOrderAsync(request.UserId, request.OrderNumber, cancellationToken);
    }

    public class GetAdminOrdersQuery : IRequest<PagedResult<OrderListItemDto>>
    {
        public OrderFilterDto Filter { get; set; } = new();
    }

    public class GetAdminOrdersQueryHandler : IRequestHandler<GetAdminOrdersQuery, PagedResult<OrderListItemDto>>
    {
        private readonly IOrderRepositoryRead _repository;

        public GetAdminOrdersQueryHandler(IOrderRepositoryRead repository)
        {
            _repository = repository;
        }

        public Task<PagedResult<OrderListItemDto>> Handle(GetAdminOrdersQuery request, CancellationToken cancellationToken)
            => _repository.GetAdminOrdersAsync(request.Filter, cancellationToken);
    }

    public class GetAdminOrderQuery : IRequest<OrderDetailDto?>
    {
        public string OrderNumber { get; set; }

        public GetAdminOrderQuery(string orderNumber)
        {
            OrderNumber = orderNumber;
        }
    }

    public class GetAdminOrderQueryHandler : IRequestHandler<GetAdminOrderQuery, OrderDetailDto?>
    {
        private readonly IOrderRepositoryRead _repository;

        public GetAdminOrderQueryHandler(IOrderRepositoryRead repository)
        {
            _repository = repository;
        }

        public Task<OrderDetailDto?> Handle(GetAdminOrderQuery request, CancellationToken cancellationToken)
            => _repository.GetAdminOrderAsync(request.OrderNumber, cancellationToken);
    }
}
