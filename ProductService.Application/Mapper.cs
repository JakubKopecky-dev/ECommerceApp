using ProductService.Application.DTOs.Brand;
using ProductService.Application.DTOs.Category;
using ProductService.Application.DTOs.Product;
using ProductService.Application.DTOs.ProductReview;
using ProductService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Application
{
    public static class Mapper
    {

        public static ProductDto ProductToProductDto(this Product product) =>
            new()
            {
                Id = product.Id,
                CreatedAt = DateTime.Now,
                ImageUrl = product.ImageUrl.Value,
                IsActive = product.IsActive,
                UpdatedAt = DateTime.Now,
                BrandId = product.BrandId,
                Description = product.Description,
                Price = product.Price,
                QuantityInStock = product.QuantityInStock,
                SoldCount = product.SoldCount,
                Title = product.Title
            };



        public static ProductExtendedDto ProductToProductExtendedDto(this Product product) =>
            new()
            {
                Id = product.Id,
                CreatedAt = product.CreatedAt,
                IsActive = product.IsActive,
                UpdatedAt = product.UpdatedAt,
                BrandId = product.BrandId,
                Description = product.Description,
                ImageUrl = product.ImageUrl.Value,
                Price = product.Price,
                Title = product.Title,
                QuantityInStock = product.QuantityInStock,
                SoldCount = product.SoldCount,
                Categories = [.. product.Categories.Select(c => c.Title)]
            };



        public static BrandDto BrandToBrandDto(this Brand brand) =>
            new()
            {
                Id = brand.Id,
                Title = brand.Title,
                Description = brand.Description,
                UpdatedAt = brand.UpdatedAt,
                CreatedAt = brand.CreatedAt,
            };


        public static CategoryDto CategoryToCategoryDto(this Category category) =>
            new()
            {
                Id = category.Id,
                Title = category.Title,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            };



        public static ProductReviewDto ProductReviewToProductReviewDto(this ProductReview review) =>
            new()
            {
                Id = review.Id,
                UserName = review.UserName,
                UserId = review.UserId,
                Comment = review.Comment,
                Title = review.Title,
                Rating = review.Rating,
                ProductId = review.ProductId,
                CreatedAt = review.CreatedAt,
                UpdatedAt = review.UpdatedAt
            };

    }
}
