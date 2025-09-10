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
using CartService.Application.Interfaces.External;


namespace CartService.Application.Services
{
    public class CartService(ICartRepository cartRepository, IMapper mapper, ILogger<CartService> logger, IProductReadClient productClient, IOrderReadClient orderClient) : ICartService
    {
        private readonly ICartRepository _cartRepository = cartRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<CartService> _logger = logger;
        private readonly IProductReadClient _productClient = productClient;
        private readonly IOrderReadClient _orderClient = orderClient;




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

            _cartRepository.Remove(cart);
            await _cartRepository.SaveChangesAsync(ct);
            _logger.LogInformation("Deleted cart and its items. UserId: {UserId}.", userId);

            return deletedCart;
        }



        public async Task<Result<CheckoutResult, CartError>> CheckoutCartByUserIdAsync(Guid userId, CartCheckoutRequestDto cartCheckoutRequestDto, CancellationToken ct = default)
        {
            _logger.LogInformation("Checking out cart. UserId: {UserId}.", userId);

            Cart? cart = await _cartRepository.FindCartByUserIdIncludeItemsAsync(userId, ct);
            if (cart is null || cart.Items.Count == 0)
            {
                _logger.LogWarning("Checkout failed. Cart not found or empty. UserId: {UserId}", userId);
                return Result<CheckoutResult, CartError>.Fail(CartError.CartNotFound);
            }


            List<ProductQuantityCheckRequestDto> cartItemProducts = [.. cart.Items.Select(c => new ProductQuantityCheckRequestDto { Id = c.ProductId, Quantity = c.Quantity })];

            IReadOnlyList<ProductQuantityCheckResponseDto> badProducts = await _productClient.CheckProductAvailabilityAsync(cartItemProducts, ct);

            if (badProducts.Any())
            {
                _logger.LogWarning("Cannot check out cart. Lack of goods in stock. CartId: {CartId}, ProductIds: {ProductIds}.", cart.Id, string.Join(", ", badProducts.Select(p => p.Id)));
                return Result<CheckoutResult, CartError>.Ok(new CheckoutResult(false, badProducts,null));
            }
                

            // creating order and delivery
            CreateOrderAndDeliveryDto checkOutCartDto = new()
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


            CreateOrderFromCartResponseDto createdOrder = await _orderClient.CreateOrderAndDeliveryAsync(checkOutCartDto, ct);

            if (createdOrder.DeliveryId is null && createdOrder.CheckoutUrl is null)
            {
                _logger.LogWarning("Order created but not its delivery and payment checkout url. OrderId: {OrderId} UserId: {UserId}, CourierId: {CourierId}.", createdOrder.OrderId, userId, cartCheckoutRequestDto.CourierId);
                await DeleteCartByUserIdAsync(userId, ct);

                return Result<CheckoutResult, CartError>.Fail(CartError.DeliveryAndPaymentCheckoutNotCreated);
            }


            if (createdOrder.CheckoutUrl is null)
            {
                _logger.LogWarning("Order created but not its payment checkout url. OrderId: {OrderId} UserId: {UserId}, CourierId: {CourierId}.", createdOrder.OrderId, userId, cartCheckoutRequestDto.CourierId);
                await DeleteCartByUserIdAsync(userId, ct);

                return Result<CheckoutResult, CartError>.Fail(CartError.PaymentCheckoutUrlNotCreated);
            }


            if (createdOrder.DeliveryId is null)
            {
                _logger.LogWarning("Order created but not its delivery. OrderId: {OrderId} UserId: {UserId}, CourierId: {CourierId}.",createdOrder.OrderId,userId,cartCheckoutRequestDto.CourierId);

                await DeleteCartByUserIdAsync(userId, ct);

                return Result<CheckoutResult, CartError>.Fail(CartError.DeliveryNotCreated);
            }
            

            await DeleteCartByUserIdAsync(userId, ct);

            _logger.LogInformation("Checkout completed successfully. UserId: {UserId}, OrderId: {OrderId}, DeliveryId: {DeliveryId}.", userId,createdOrder.OrderId,createdOrder.DeliveryId);

            return Result<CheckoutResult, CartError>.Ok(new CheckoutResult(true, [], createdOrder.CheckoutUrl));
        }



    }
}