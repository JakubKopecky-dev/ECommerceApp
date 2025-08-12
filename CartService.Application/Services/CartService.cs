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

            Cart? cart = await _cartRepository.FindCartByUserIdIncludeItemsAsync(userId,ct);
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

            await _cartRepository.DeleteAsync(cart.Id,ct);
            _logger.LogInformation("Deleted cart and its items. UserId: {UserId}.", userId);

            return deletedCart;
        }


        public async Task<bool> CheckoutCartByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            _logger.LogInformation("Checking out cart. UserId: {UserId}.", userId);

            Cart? cart = await _cartRepository.FindCartByUserIdIncludeItemsAsync(userId, ct);
            if (cart is null || cart.Items.Count == 0)
            {
                _logger.LogWarning("Checkout failed. Cart not found or empty. UserId: {UserId}", userId);
                return false;
            }

            /*
            var productIds = cart.Items.Select(c => c.ProductId).ToList();
            foreach (Guid productId in productIds)
            {


            VALIDACE PŘI CHECKOUTU DODĚLAT

            }
            */

            CheckoutCartDto checkoutCartDto = _mapper.Map<CheckoutCartDto>(cart);

            var httpclient = _httpClientFactory.CreateClient("OrderService");

            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            if (!string.IsNullOrWhiteSpace(token))
                httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            
                var response = await httpclient.PostAsJsonAsync("api/Order/external", checkoutCartDto,ct);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to create order. StatusCode: {StatusCode}.", response.StatusCode);
                    return false;
                }
            

            await DeleteCartByUserIdAsync(userId,ct);

            _logger.LogInformation("Checkout completed successfully. UserId: {UserId}.", userId);

            return true;
        }
    }
}