using MediatR;
using ShopMicroservice.Application.DTO;
using ShopMicroservice.Core.Interfaces;

namespace ShopMicroservice.Application.Query.Addresses
{
    public class GetAddressesQuery : IRequest<List<AddressDto>>
    {
        public int UserId { get; set; }

        public GetAddressesQuery(int userId)
        {
            UserId = userId;
        }
    }

    public class GetAddressesQueryHandler : IRequestHandler<GetAddressesQuery, List<AddressDto>>
    {
        private readonly IAddressRepositoryRead _repository;

        public GetAddressesQueryHandler(IAddressRepositoryRead repository)
        {
            _repository = repository;
        }

        public Task<List<AddressDto>> Handle(GetAddressesQuery request, CancellationToken cancellationToken)
            => _repository.GetListAsync(request.UserId, cancellationToken);
    }
}
