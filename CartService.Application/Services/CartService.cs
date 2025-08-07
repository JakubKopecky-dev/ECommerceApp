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


namespace CartService.Application.Services
{
    public class CartService(ICartRepository cartRepository, ICartItemRepository cartItemRepository, IMapper mapper, ILogger<CartService> logger,IHttpClientFactory httpClientFactory , IHttpContextAccessor httpContextAccessor) : ICartService
    {
        private readonly ICartRepository _cartRepository = cartRepository;
        private readonly ICartItemRepository _cartItemRepository = cartItemRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<CartService> _logger = logger;
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;


        public async Task<CartExtendedDto> GetOrCreateCartAsync(Guid userId)
        {
            _logger.LogInformation("Retrieving cart. UserId: {UserId}.", userId);

            Cart? cart = await _cartRepository.FindCartByUserIdIncludeItemsAsync(userId);
            if (cart is null)
            {
                _logger.LogInformation("Cart not found. Creating new cart for user. UserId: {UserId}.", userId);

                Cart newCart = new() { Id = Guid.Empty, UserId = userId };
                Cart createdCart = await _cartRepository.InsertAsync(newCart);
                _logger.LogInformation("New cart created. CartId: {CartId}.", createdCart.Id);

                return _mapper.Map<CartExtendedDto>(createdCart);
            }

            _logger.LogInformation("Cart found. CartId: {CartId}, UserId: {UserId}.", cart.Id, userId);

            return _mapper.Map<CartExtendedDto>(cart);
        }


        public async Task<CartDto?> DeleteCartAsync(Guid userId)
        {
            _logger.LogInformation("Deleting cart. UserId: {UserId}.", userId);

            Cart? cart = await _cartRepository.FindCartByUserIdIncludeItemsAsync(userId);
            if (cart is null)
            {
                _logger.LogInformation("Cannot delete. Cart not found. UserId: {UserId}.", userId);
                return null;
            }

            CartDto deletedCart = _mapper.Map<CartDto>(cart);

            if (cart.Items.Count > 0)
            {
                _logger.LogInformation("Deleting related cartItems before deleting cart. Count: {Count}, UserId: {UserId}.", cart.Items.Count, userId);

                var deletedTask = cart.Items.Select(item => _cartItemRepository.DeleteAsync(item.Id));
                await Task.WhenAll(deletedTask);
                _logger.LogInformation("All related cartItems deleted.");
            }

            await _cartRepository.DeleteAsync(cart.Id);
            _logger.LogInformation("Deleted cart. UserId: {UserId}.", userId);

            return deletedCart;
        }


        public async Task<bool> CheckoutCartAsync(Guid userId)
        {
            _logger.LogInformation("Checking out cart. UserId: {UserId}.", userId);

            Cart? cart = await _cartRepository.FindCartByUserIdIncludeItemsAsync(userId);
            if (cart is null || cart.Items.Count == 0)
            {
                _logger.LogWarning("Checkout failed. Cart not found or empty. UserId: {UserId}", userId);
                return false;
            }

            CheckoutCartDto checkoutCartDto = _mapper.Map<CheckoutCartDto>(cart);

            var httpclient = _httpClientFactory.CreateClient("OrderService");
            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            if (!string.IsNullOrWhiteSpace(token))
                httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


            try
            {
                var response = await httpclient.PostAsJsonAsync("api/Order/external", checkoutCartDto);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to create order. StatusCode: {StatusCode}.", response.StatusCode);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during checkout request.");
                return false;
            }

            await DeleteCartAsync(userId);

            _logger.LogInformation("Checkout completed successfully. UserId: {UserId}.", userId);

            return true;
        }
    }
}