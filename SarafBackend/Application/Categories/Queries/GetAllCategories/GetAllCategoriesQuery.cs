using GirlyShopBackend.Core.Domain.ViewModel.Category;
using MediatR;

namespace GirlyShopBackend.Application.Categories.Queries.GetAllCategories;

public record GetAllCategoriesQuery : IRequest<IEnumerable<CategoryViewModel>>;
