using AutoMapper;
using ProductService.Application.DTOs.Brand;
using ProductService.Application.DTOs.Category;
using ProductService.Application.DTOs.Product;
using ProductService.Application.DTOs.ProductReview;
using ProductService.Domain.Entity;

namespace ProductService.Application.Mapping
{
    public class AutomapperConfigurationProfile : Profile
    {

        public AutomapperConfigurationProfile()
        {
            CreateMap<CreateUpdateBrandDto, Brand>();
            CreateMap<Brand, BrandDto>();


            CreateMap<CreateUpdateCategoryDto, Category>();
            CreateMap<Category, CategoryDto>();


            CreateMap<CreateProductDto, Product>()
                .ForMember(dest => dest.Categories, opt => opt.Ignore());
                
            
            
            CreateMap<UpdateProductDto, Product>()
                .ForMember(dest => dest.Categories, opt => opt.Ignore());

            CreateMap<Product, ProductDto>();
            CreateMap<Product, ProductExtendedDto>()
                .ForMember(dest => dest.Categories,
                    opt => opt.MapFrom(src => src.Categories.Select(c => c.Title).ToList())
                );


            CreateMap<CreateProductReviewDto, ProductReview>();
            CreateMap<UpdateProductReviewDto, ProductReview>();
            CreateMap<ProductReview, ProductReviewDto>();

        }

    }
}
