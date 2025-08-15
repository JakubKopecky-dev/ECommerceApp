using System.Net.Http.Headers;
using System.Net.Http.Json;
using AutoMapper;
using CartService.Application.DTOs.Cart;
using CartService.Application.Interfaces.Repositories;
using CartService.Application.Interfaces.Services;
using CartService.Domain.Entity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using CartService.Application.DTOs.External;
using CartService.Application.DTOs.CartItem;
using CartService.Application.Common;


namespace CartService.Application.Services
{
    public class CartService(ICartRepository cartRepository, ICartItemRepository cartItemRepository, IMapper mapper, ILogger<CartService> logger, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor) : ICartService
    {
        private readonly ICartRepository _cartRepository = cartRepository;
        private readonly ICartItemRepository _cartItemRepository = cartItemRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<CartService> _logger = logger;
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;



        public async Task<CartExtendedDto> GetOrCreateCartByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving cart. UserId: {UserId}.", userId);

            Cart? cart = await _cartRepository.FindCartByUserIdIncludeItemsAsync(userId, ct);
            if (cart is null)
            {
                _logger.LogInformation("Cart not found. Creating new cart for user. UserId: {UserId}.", userId);

                Cart newCart = new() { Id = Guid.Empty, UserId = userId };
                Cart createdCart = await _cartRepository.InsertAsync(newCart, ct);
                _logger.LogInformation("New cart created. CartId: {CartId}.", createdCart.Id);

                return _mapper.Map<CartExtendedDto>(createdCart);
            }

            _logger.LogInformation("Cart found. CartId: {CartId}, UserId: {UserId}.", cart.Id, userId);

            return _mapper.Map<CartExtendedDto>(cart);
        }



        public async Task<CartDto?> DeleteCartByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            _logger.LogInformation("Deleting cart. UserId: {UserId}.", userId);

            Cart? cart = await _cartRepository.FindCartByUserIdIncludeItemsAsync(userId, ct);
            if (cart is null)
            {
                _logger.LogInformation("Cannot delete. Cart not found. UserId: {UserId}.", userId);
                return null;
            }

            CartDto deletedCart = _mapper.Map<CartDto>(cart);

            await _cartRepository.DeleteAsync(cart.Id, ct);
            _logger.LogInformation("Deleted cart and its items. UserId: {UserId}.", userId);

            return deletedCart;
        }



        public async Task<Result<CheckoutResult,CartError>> CheckoutCartByUserIdAsync(Guid userId, CartCheckoutRequestDto cartCheckoutRequestDto, CancellationToken ct = default)
        {
            _logger.LogInformation("Checking out cart. UserId: {UserId}.", userId);

            Cart? cart = await _cartRepository.FindCartByUserIdIncludeItemsAsync(userId, ct);
            if (cart is null || cart.Items.Count == 0)
            {
                _logger.LogWarning("Checkout failed. Cart not found or empty. UserId: {UserId}", userId);
                return Result<CheckoutResult, CartError>.Fail(CartError.CartNotFound);
            }

            var orderHttpClient = _httpClientFactory.CreateClient("OrderService");
            var productHttpClient = _httpClientFactory.CreateClient("ProductService");
            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");


            if (!string.IsNullOrWhiteSpace(token))
            {
                orderHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                productHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }


            List<ProductQuantityCheckRequestDto> cartItemProducts = [.. cart.Items.Select(c => new ProductQuantityCheckRequestDto { Id = c.Id, Quantity = c.Quantity })];

            var productResponse = await productHttpClient.PostAsJsonAsync("api/Product/availability",cartItemProducts,ct);

            IReadOnlyList<ProductQuantityCheckResponseDto> badProducts = await productResponse.Content.ReadFromJsonAsync<IReadOnlyList<ProductQuantityCheckResponseDto>>(ct) ?? [];

            if (badProducts.Any())
            {
                _logger.LogWarning("Cannot check out cart. Lack of goods in stock. CartId: {CartId}, ProductIds: {ProductIds}.", cart.Id, string.Join(", ", badProducts.Select(p => p.Id)));
                return Result<CheckoutResult, CartError>.Ok(new CheckoutResult(false, badProducts));
            }

            // creating order and delivery
            CreateOrderAndDelivery checkoutCartDto = new()
            {
                UserId = userId,
                CourierId = cartCheckoutRequestDto.CourierId,
                TotalPrice = cart.Items.Sum(i => i.UnitPrice * i.Quantity),
                Note = cartCheckoutRequestDto.Note,
                Email = cartCheckoutRequestDto.Email,
                FirstName = cartCheckoutRequestDto.FirstName,
                LastName = cartCheckoutRequestDto.LastName,
                PhoneNumber = cartCheckoutRequestDto.PhoneNumber,
                Street = cartCheckoutRequestDto.Street,
                City = cartCheckoutRequestDto.City,
                PostalCode = cartCheckoutRequestDto.PostalCode,
                State = cartCheckoutRequestDto.State,
                Items = _mapper.Map<List<CartItemForCheckoutDto>>(cart.Items)
            };


            var orderResponse = await orderHttpClient.PostAsJsonAsync("api/Order/external", checkoutCartDto, ct);

            if (!orderResponse.IsSuccessStatusCode)
            {
                var errorMessage = await orderResponse.Content.ReadAsStringAsync(ct);

                _logger.LogError("Failed to create order or order delivery. StatusCode: {StatusCode}, Message: {Message}.", orderResponse.StatusCode, errorMessage);
                return Result<CheckoutResult,CartError>.Fail(CartError.OrderNotCreated);
            }

            await DeleteCartByUserIdAsync(userId, ct);

            _logger.LogInformation("Checkout completed successfully. UserId: {UserId}.", userId);

            return Result<CheckoutResult, CartError>.Ok(new CheckoutResult(true, []));
        }



    }
}