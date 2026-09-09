using GirlyShopBackend.Core.Domain.Entities;
using GirlyShopBackend.Core.Domain.RepositoryInterfaces;
using GirlyShopBackend.Core.DomainServices;
using MediatR;

namespace GirlyShopBackend.Application.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
{
    private readonly IProductWriteRepository _writeRepository;
    private readonly IProductReadRepository _readRepository;

    public CreateProductCommandHandler(
        IProductWriteRepository writeRepository,
        IProductReadRepository readRepository)
    {
        _writeRepository = writeRepository;
        _readRepository = readRepository;
    }

    public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var baseSlug = SlugGenerator.Generate(request.Title);
        var slug = baseSlug;
        var counter = 1;
        while (await _readRepository.SlugExistsAsync(slug))
        {
            slug = $"{baseSlug}-{counter}";
            counter++;
        }

        var product = new Product
        {
            Title = request.Title,
            Slug = slug,
            Description = request.Description,
            BasePrice = request.BasePrice,
            DiscountPrice = request.DiscountPrice,
            CategoryId = request.CategoryId,
            Variants = request.Variants.Select(v => new ProductVariant
            {
                Sku = v.Sku,
                Stock = v.Stock,
                PriceOverride = v.PriceOverride,
                AttributeValues = v.Attributes.Select(a => new VariantAttributeValue
                {
                    AttributeName = a.Name,
                    AttributeValue = a.Value
                }).ToList()
            }).ToList()
        };

        await _writeRepository.AddAsync(product);
        await _writeRepository.SaveChangesAsync();

        return product.Id;
    }
}
