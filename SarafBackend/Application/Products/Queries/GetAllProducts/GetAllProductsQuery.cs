using GirlyShopBackend.Core.Domain.ViewModel.Product;
using MediatR;

namespace GirlyShopBackend.Application.Products.Queries.GetAllProducts;

public record GetAllProductsQuery(string? CategorySlug) : IRequest<IEnumerable<ProductListItemViewModel>>;
