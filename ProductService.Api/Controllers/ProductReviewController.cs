using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.DTOs.ProductReview;
using ProductService.Application.Interfaces.Services;
using ProductService.Domain.Entity;
using ProductService.Domain.Enum;

namespace ProductService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductReviewController(IProductReviewService productReviewService) : ControllerBase
    {
        private readonly IProductReviewService _productReviewService = productReviewService;


        [HttpGet]
        public async Task<IReadOnlyList<ProductReviewDto>> GetAllProductReviews() => await _productReviewService.GetAllProductReviewsAsync();



        [HttpGet("{reviewId}")]
        public async Task<IActionResult> GetProductReview(Guid reviewId)
        {
            ProductReviewDto? review = await _productReviewService.GetProductReviewAsync(reviewId);

            return review is not null ? Ok(review) : NotFound();
        }



        [Authorize(Roles = UserRoles.User)]
        [HttpPost]
        public async Task<IActionResult> CreateProductReview([FromBody] CreateProductReviewDto createDto)
        {
            ProductReviewDto review = await _productReviewService.CreateProductReviewAsync(createDto);

            return CreatedAtAction(nameof(GetProductReview), new { reviewId = review.Id }, review);
        }



        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut("{reviewId}")]
        public async Task<IActionResult> UpdateProductReview(Guid reviewId, [FromBody] UpdateProductReviewDto updateDto)
        {
            ProductReviewDto? review = await _productReviewService.UpdateProductReviewAsync(reviewId,updateDto);

            return review is not null ? Ok(review) : NotFound();

        }



        [Authorize(Roles = UserRoles.Admin)]
        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> DeleteProductReview(Guid reviewId)
        {
            ProductReviewDto? review = await _productReviewService.DeleteProductReviewAsync(reviewId);

            return review is not null ? Ok(review) : NotFound();
        }



        [Authorize(Roles = UserRoles.User)]
        [HttpDelete("my/{reviewId}")]
        public async Task<IActionResult> DeleteOwnProductReview(Guid reviewId)
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
                return Unauthorized();

            ProductReviewDto? review = await _productReviewService.DeleteOwnProductReviewAsync(reviewId, userId);

            return review is not null ? Ok(review) : NotFound();
        }



    }
}
