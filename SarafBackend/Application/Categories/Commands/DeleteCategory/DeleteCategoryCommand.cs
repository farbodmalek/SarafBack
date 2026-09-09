using MediatR;

namespace GirlyShopBackend.Application.Categories.Commands.DeleteCategory;

public record DeleteCategoryCommand(int Id) : IRequest<bool>;
