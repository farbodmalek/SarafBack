using GirlyShopBackend.Core.Domain.ViewModel.Product;
using MediatR;

namespace GirlyShopBackend.Application.Products.Queries.GetProductBySlug;

public record GetProductBySlugQuery(string Slug) : IRequest<ProductDetailViewModel?>;
