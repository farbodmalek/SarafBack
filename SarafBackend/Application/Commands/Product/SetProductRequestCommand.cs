using ShopMicroservice.Core.Domain;
using ShopMicroservice.Core.Interfaces;
using MediatR;

namespace ShopMicroservice.Application.Commands.Product
{
    public class ProductVariantInput
    {
        public string Sku { get; set; } = default!;
        public decimal Price { get; set; }
        public decimal? CompareAtPrice { get; set; }
        public int StockQuantity { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new();
    }

    /// <summary>
    /// "Set" یعنی هم برای ایجاد محصول جدید (Id خالی) و هم ویرایش (Id موجود)
    /// استفاده می‌شود.
    /// </summary>
    public class SetProductRequestCommand : IRequest<Guid>
    {
        public Guid? Id { get; set; }
        public long CategoryId { get; set; }
        public long? BrandId { get; set; }
        public string Name { get; set; } = default!;
        public string Slug { get; set; } = default!;
        public string? Description { get; set; }
        public decimal BasePrice { get; set; }
        public bool IsFeatured { get; set; }
        public List<string> ImageUrls { get; set; } = new();
        public List<ProductVariantInput> Variants { get; set; } = new();
    }

    public class SetProductRequestCommandHandler : IRequestHandler<SetProductRequestCommand, Guid>
    {
        private readonly IProductRepositoryWrite _repositoryWrite;

        public SetProductRequestCommandHandler(IProductRepositoryWrite repositoryWrite)
        {
            _repositoryWrite = repositoryWrite;
        }

        public async Task<Guid> Handle(SetProductRequestCommand request, CancellationToken cancellationToken)
        {
            var isUpdate = request.Id.HasValue && request.Id.Value != Guid.Empty;

            Core.Domain.Product product;

            if (isUpdate)
            {
                product = await _repositoryWrite.GetTrackedByIdAsync(request.Id!.Value, cancellationToken)
                    ?? throw new KeyNotFoundException($"محصولی با شناسه {request.Id} پیدا نشد.");

                MapRequestToProduct(request, product);
                ReplaceImages(product, request.ImageUrls);
                ReplaceVariants(product, request.Variants);

                await _repositoryWrite.UpdateAsync(product, cancellationToken);
            }
            else
            {
                product = new Core.Domain.Product();
                MapRequestToProduct(request, product);
                ReplaceImages(product, request.ImageUrls);
                ReplaceVariants(product, request.Variants);

                await _repositoryWrite.AddAsync(product, cancellationToken);
            }

            await _repositoryWrite.SaveChangesAsync(cancellationToken);
            return product.Id;
        }

        private static void MapRequestToProduct(SetProductRequestCommand request, Core.Domain.Product product)
        {
            product.CategoryId = request.CategoryId;
            product.BrandId = request.BrandId;
            product.Name = request.Name;
            product.Slug = request.Slug;
            product.Description = request.Description;
            product.BasePrice = request.BasePrice;
            product.IsFeatured = request.IsFeatured;

            if (product.Status == ProductStatus.Draft)
            {
                product.Status = ProductStatus.Published;
            }
        }

        private static void ReplaceImages(Core.Domain.Product product, List<string> imageUrls)
        {
            product.Images.Clear();
            foreach (var (url, index) in imageUrls.Select((u, i) => (u, i)))
            {
                product.Images.Add(new ProductImage
                {
                    ImageUrl = url,
                    SortOrder = index,
                    IsPrimary = index == 0
                });
            }
        }

        private static void ReplaceVariants(Core.Domain.Product product, List<ProductVariantInput> variants)
        {
            product.Variants.Clear();
            foreach (var variantInput in variants)
            {
                var variant = new ProductVariant
                {
                    Sku = variantInput.Sku,
                    Price = variantInput.Price,
                    CompareAtPrice = variantInput.CompareAtPrice,
                    StockQuantity = variantInput.StockQuantity
                };

                foreach (var (key, value) in variantInput.Attributes)
                {
                    variant.Attributes.Add(new ProductVariantAttribute
                    {
                        VariantId = variant.Id,
                        AttributeName = key,
                        AttributeValue = value
                    });
                }

                product.Variants.Add(variant);
            }
        }
    }
}
