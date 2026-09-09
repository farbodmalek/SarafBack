using MediatR;

namespace GirlyShopBackend.Application.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(string Name, int? ParentCategoryId, string? ImageUrl) : IRequest<int>;
