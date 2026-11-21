using MediatR;
using ShopMicroservice.Application.DTO;
using ShopMicroservice.Core.Interfaces;

namespace ShopMicroservice.Application.Query.Categories
{
    public class GetCategoriesQuery : IRequest<List<CategoryDto>>
    {
        public bool IncludeInactive { get; set; }
    }

    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
    {
        private readonly ICategoryRepositoryRead _repository;

        public GetCategoriesQueryHandler(ICategoryRepositoryRead repository)
        {
            _repository = repository;
        }

        public Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
            => _repository.GetAllAsync(request.IncludeInactive, cancellationToken);
    }
}
