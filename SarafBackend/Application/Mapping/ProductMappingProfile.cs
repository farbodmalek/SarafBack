using AutoMapper;
using ShopMicroservice.Application.DTO;
using ShopMicroservice.Core.Domain;

namespace ShopMicroservice.Application.Mapping
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<Product, ProductDetailDto>()
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.ImageUrls, o => o.MapFrom(s => s.Images.Select(i => i.ImageUrl)))
                .ForMember(d => d.Variants, o => o.MapFrom(s => s.Variants));

            CreateMap<ProductVariant, ProductVariantDto>()
                .ForMember(d => d.Attributes, o => o.MapFrom(s =>
                    s.Attributes.ToDictionary(a => a.AttributeName, a => a.AttributeValue)));

            CreateMap<Product, ProductListItemDto>()
                .ForMember(d => d.PrimaryImageUrl, o => o.MapFrom(s =>
                    s.Images.Where(i => i.IsPrimary).Select(i => i.ImageUrl).FirstOrDefault()));
        }
    }
}
