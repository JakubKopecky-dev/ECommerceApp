using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.DTOs.ProductReview;
using ProductService.Application.Interfaces.Services;
using ProductService.Domain.Enums;

namespace ProductService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductReviewController(IProductReviewService productReviewService) : ControllerBase
    {
        private readonly IProductReviewService _productReviewService = productReviewService;


        [HttpGet]
        public async Task<IReadOnlyList<ProductReviewDto>> GetAllProductReviews(CancellationToken ct) => await _productReviewService.GetAllProductReviewsAsync(ct);



        [HttpGet("{reviewId}")]
        public async Task<IActionResult> GetProductReview(Guid reviewId, CancellationToken ct)
        {
            ProductReviewDto? review = await _productReviewService.GetProductReviewAsync(reviewId, ct);

            return review is not null ? Ok(review) : NotFound();
        }



        [Authorize(Roles = UserRoles.User)]
        [HttpPost]
        public async Task<IActionResult> CreateProductReview([FromBody] CreateProductReviewDto createDto, CancellationToken ct)
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(!Guid.TryParse(userIdString, out Guid userId))
                return Unauthorized();

            string? userName = User.FindFirstValue(ClaimTypes.Name);
            if (userName is null)
                return Unauthorized();

            ProductReviewDto review = await _productReviewService.CreateProductReviewAsync(createDto,userId,userName, ct);

            return CreatedAtAction(nameof(GetProductReview), new { reviewId = review.Id }, review);
        }



        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut("{reviewId}")]
        public async Task<IActionResult> UpdateProductReview(Guid reviewId, [FromBody] UpdateProductReviewDto updateDto, CancellationToken ct)
        {
            ProductReviewDto? review = await _productReviewService.UpdateProductReviewAsync(reviewId, updateDto, ct);

            return review is not null ? Ok(review) : NotFound();

        }



        [Authorize(Roles = UserRoles.Admin)]
        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> DeleteProductReview(Guid reviewId, CancellationToken ct)
        {
            ProductReviewDto? review = await _productReviewService.DeleteProductReviewAsync(reviewId, ct);

            return review is not null ? Ok(review) : NotFound();
        }



        [Authorize(Roles = UserRoles.User)]
        [HttpDelete("my/{reviewId}")]
        public async Task<IActionResult> DeleteOwnProductReview(Guid reviewId, CancellationToken ct)
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
                return Unauthorized();

            ProductReviewDto? review = await _productReviewService.DeleteOwnProductReviewAsync(reviewId, userId, ct);

            return review is not null ? Ok(review) : NotFound();
        }



        [HttpGet("by-productId/{productId}")]
        public async Task<IReadOnlyList<ProductReviewDto>> GetAllProductReviewsByProductId(Guid productId, CancellationToken ct) => await _productReviewService.GetAllProductReviewsByProductIdAsync(productId, ct);



    }
}
