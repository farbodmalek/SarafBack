using MediatR;
using ShopMicroservice.Application.DTO;
using ShopMicroservice.Core.Interfaces;

namespace ShopMicroservice.Application.Query.Carts
{
    public class GetCartQuery : IRequest<CartDto>
    {
        public int UserId { get; set; }

        public GetCartQuery(int userId)
        {
            UserId = userId;
        }
    }

    public class GetCartQueryHandler : IRequestHandler<GetCartQuery, CartDto>
    {
        private readonly ICartRepositoryRead _repository;

        public GetCartQueryHandler(ICartRepositoryRead repository)
        {
            _repository = repository;
        }

        public Task<CartDto> Handle(GetCartQuery request, CancellationToken cancellationToken)
            => _repository.GetCartAsync(request.UserId, cancellationToken);
    }
}
