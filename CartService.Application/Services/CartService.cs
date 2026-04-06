using CartService.Application.DTOs.Cart;
using CartService.Application.Interfaces.Repositories;
using CartService.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
using CartService.Application.DTOs.External;
using CartService.Application.DTOs.CartItem;
using CartService.Application.Common;
using CartService.Application.Interfaces.External;
using CartService.Domain.Entities;


namespace CartService.Application.Services
{
    public class CartService(ICartRepository cartRepository, ILogger<CartService> logger, IProductReadClient productClient, IOrderReadClient orderClient) : ICartService
    {
        private readonly ICartRepository _cartRepository = cartRepository;
        private readonly ILogger<CartService> _logger = logger;
        private readonly IProductReadClient _productClient = productClient;
        private readonly IOrderReadClient _orderClient = orderClient;



        /// <summary>
        /// Retrieves the shopping cart associated with the specified user identifier, or creates a new cart if none
        /// exists.
        /// </summary>
        /// <remarks>If a cart does not exist for the specified user, a new cart is created and returned.
        /// The returned cart includes all associated items. This method is thread-safe with respect to cart creation
        /// for a given user.</remarks>
        /// <param name="userId">The unique identifier of the user whose cart is to be retrieved or created.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="CartExtendedDto"/>
        /// representing the user's cart, including its items.</returns>
        public async Task<CartExtendedDto> GetOrCreateCartByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving cart. UserId: {UserId}.", userId);

            Cart? cart = await _cartRepository.FindCartByUserIdIncludeItemsAsync(userId, ct);
            if (cart is null)
            {
                _logger.LogInformation("Cart not found. Creating new cart for user. UserId: {UserId}.", userId);

                Cart newCart = Cart.Create(userId);

                await _cartRepository.AddAsync(newCart, ct);
                await _cartRepository.SaveChangesAsync(ct);
                _logger.LogInformation("New cart created. CartId: {CartId}.", newCart.Id);

                return newCart.CartToCartExtendedDto();
            }

            _logger.LogInformation("Cart found. CartId: {CartId}, UserId: {UserId}.", cart.Id, userId);

            return cart.CartToCartExtendedDto();
        }



       /// <summary>
       /// Asynchronously deletes the shopping cart and its items for the specified user, if one exists.
       /// </summary>
       /// <remarks>If no cart exists for the specified user, the method returns <see langword="false"/>
       /// and no action is taken.</remarks>
       /// <param name="userId">The unique identifier of the user whose cart should be deleted.</param>
       /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
       /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if a cart was
       /// found and deleted; otherwise, <see langword="false"/>.</returns>
        public async Task<bool> DeleteCartByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            _logger.LogInformation("Deleting cart. UserId: {UserId}.", userId);

            Cart? cart = await _cartRepository.FindCartByUserIdIncludeItemsAsync(userId, ct);
            if (cart is null)
            {
                _logger.LogInformation("Cannot delete. Cart not found. UserId: {UserId}.", userId);
                return false;
            }

            _cartRepository.Remove(cart);
            await _cartRepository.SaveChangesAsync(ct);
            _logger.LogInformation("Deleted cart and its items. UserId: {UserId}.", userId);

            return true;
        }



        /// <summary>
        /// Attempts to check out the shopping cart for the specified user, creating an order and delivery if all items
        /// are available.
        /// </summary>
        /// <remarks>If the cart is empty or does not exist, the operation fails with <see
        /// cref="CartError.CartNotFound"/>. If product availability checks fail, the result includes the unavailable
        /// products. The cart is deleted after a successful checkout or if order creation fails at certain stages. This
        /// method does not throw exceptions for expected business errors; instead, errors are represented in the
        /// result.</remarks>
        /// <param name="userId">The unique identifier of the user whose cart is to be checked out.</param>
        /// <param name="cartCheckoutRequestDto">An object containing checkout details such as courier selection, contact information, and delivery address.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the checkout operation.</param>
        /// <returns>A result containing a <see cref="CheckoutResult"/> if the checkout succeeds, or a <see cref="CartError"/>
        /// indicating the reason for failure. If some products are unavailable, the result indicates failure and lists
        /// the unavailable products.</returns>
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
                return Result<CheckoutResult, CartError>.Ok(new CheckoutResult(false, badProducts, null));
            }


            // creating order and delivery
            CreateOrderAndDeliveryDto checkOutCartDto = new()
            {
                UserId = userId,
                CourierId = cartCheckoutRequestDto.CourierId,
                TotalPrice = cart.TotalPrice,
                Note = cartCheckoutRequestDto.Note,
                Email = cartCheckoutRequestDto.Email,
                FirstName = cartCheckoutRequestDto.FirstName,
                LastName = cartCheckoutRequestDto.LastName,
                PhoneNumber = cartCheckoutRequestDto.PhoneNumber,
                Street = cartCheckoutRequestDto.Street,
                City = cartCheckoutRequestDto.City,
                PostalCode = cartCheckoutRequestDto.PostalCode,
                State = cartCheckoutRequestDto.State,
                Items = [.. cart.Items.Select(i =>  new CartItemForCheckoutDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity
                })]
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
                _logger.LogWarning("Order created but not its delivery. OrderId: {OrderId} UserId: {UserId}, CourierId: {CourierId}.", createdOrder.OrderId, userId, cartCheckoutRequestDto.CourierId);

                await DeleteCartByUserIdAsync(userId, ct);

                return Result<CheckoutResult, CartError>.Fail(CartError.DeliveryNotCreated);
            }


            await DeleteCartByUserIdAsync(userId, ct);

            _logger.LogInformation("Checkout completed successfully. UserId: {UserId}, OrderId: {OrderId}, DeliveryId: {DeliveryId}.", userId, createdOrder.OrderId, createdOrder.DeliveryId);

            return Result<CheckoutResult, CartError>.Ok(new CheckoutResult(true, [], createdOrder.CheckoutUrl));
        }



    }
}