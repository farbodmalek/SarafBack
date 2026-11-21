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

        /// <summary>
        /// واریانت‌ها بر اساس SKU «به‌روزرسانی» می‌شوند، نه پاک و دوباره‌ساخته؛
        /// تا شناسه‌ی واریانت (که در سبد خرید و علاقه‌مندی‌ها استفاده شده) عوض نشود.
        /// </summary>
        private static void ReplaceVariants(Core.Domain.Product product, List<ProductVariantInput> variants)
        {
            var incomingSkus = variants.Select(v => v.Sku).ToHashSet();

            // واریانت‌هایی که دیگر در لیست نیستند حذف می‌شوند
            foreach (var removed in product.Variants.Where(v => !incomingSkus.Contains(v.Sku)).ToList())
            {
                product.Variants.Remove(removed);
            }

            foreach (var input in variants)
            {
                var variant = product.Variants.FirstOrDefault(v => v.Sku == input.Sku);

                if (variant is null)
                {
                    variant = new ProductVariant { Sku = input.Sku };
                    product.Variants.Add(variant);
                }

                variant.Price = input.Price;
                variant.CompareAtPrice = input.CompareAtPrice;
                variant.StockQuantity = input.StockQuantity;

                // ویژگی‌ها: موجودها به‌روز، تازه‌ها اضافه، حذف‌شده‌ها پاک
                foreach (var removedAttr in variant.Attributes
                             .Where(a => !input.Attributes.ContainsKey(a.AttributeName)).ToList())
                {
                    variant.Attributes.Remove(removedAttr);
                }

                foreach (var (key, value) in input.Attributes)
                {
                    var attr = variant.Attributes.FirstOrDefault(a => a.AttributeName == key);
                    if (attr is null)
                    {
                        variant.Attributes.Add(new ProductVariantAttribute
                        {
                            VariantId = variant.Id,
                            AttributeName = key,
                            AttributeValue = value
                        });
                    }
                    else
                    {
                        attr.AttributeValue = value;
                    }
                }
            }
        }
    }
}
