using System.Net.Http.Json;
using System.Net;
using AutoMapper;
using CartService.Application.Common;
using CartService.Application.DTOs.CartItem;
using CartService.Application.DTOs.External;
using CartService.Application.Interfaces.Repositories;
using CartService.Application.Interfaces.Services;
using CartService.Domain.Entity;
using Microsoft.Extensions.Logging;
using CartService.Application.Interfaces.External;

namespace CartService.Application.Services
{
    public class CartItemService(ICartItemRepository cartItemRepository, IMapper mapper, ILogger<CartItemService> logger, IProductReadClient client) : ICartItemService
    {
        private readonly ICartItemRepository _cartItemRepository = cartItemRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<CartItemService> _logger = logger;
        private readonly IProductReadClient _client = client;


        // helper
        private async Task<ProductDto?> GetProductAsync(Guid productId, CancellationToken ct)
        {
            ProductDto? product = await _client.GetProductAsync(productId, ct);

            return product is not null ? product : null;
        }



        public async Task<CartItemDto?> GetCartItemByIdAsync(Guid cartItemId, CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving cartItem. CartItemId: {CartItemId}.", cartItemId);

            CartItem? cartItem = await _cartItemRepository.FindByIdAsync(cartItemId, ct);
            if (cartItem is null)
            {
                _logger.LogWarning("CartItem not found. CartItemId: {CartItemId}.", cartItemId);
                return null;
            }

            _logger.LogInformation("CartItem found. CartItemId: {CartItemId}.", cartItemId);

            return _mapper.Map<CartItemDto>(cartItem);
        }



        /// <summary>
        /// Creates a new cart item or increases the quantity of an existing item in the cart, depending on whether the
        /// specified product is already present.
        /// </summary>
        /// <remarks>If the specified product is already present in the cart, the method increases its
        /// quantity by the specified amount. If the product is not present, a new cart item is created. The method
        /// fails if the product does not exist or if there is not enough stock to fulfill the requested
        /// quantity.</remarks>
        /// <param name="createDto">The details of the cart item to create or update, including the cart identifier, product identifier, and
        /// quantity. Must not be null.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A result containing the created or updated cart item if successful; otherwise, a failure result with a
        /// specific cart item error indicating the reason for failure, such as product not found or insufficient stock.</returns>
        public async Task<Result<CartItemDto, CartItemError>> CreateCartItemOrChangeQuantityAsync(CreateCartItemDto createDto, CancellationToken ct = default)
        {
            _logger.LogInformation("Creating new cartItem. CartId: {CartId}, ProductId: {ProductId}.", createDto.CartId, createDto.ProductId);

            ProductDto? productDto;

            // Check product duplicity in cart
            CartItem? alreadyExistCartItem = await _cartItemRepository.FindCartItemByCartIdAndProductIdAsync(createDto.CartId, createDto.ProductId, ct);
            if (alreadyExistCartItem is not null)
            {
                _logger.LogInformation("CartItem already exists for productId: {ProductId}, CartId: {CartId}. Changing cartItem quantity", createDto.ProductId, createDto.CartId);

                _logger.LogInformation("Checking product availability. ProductId: {ProductId}.", alreadyExistCartItem.ProductId);

                productDto = await GetProductAsync(alreadyExistCartItem.ProductId, ct);
                if (productDto is null)
                {
                    _logger.LogWarning("Cannot change cartItem quantity. Product not foud. ProdutId: {ProductId}.", alreadyExistCartItem.ProductId);
                    return Result<CartItemDto, CartItemError>.Fail(CartItemError.ProductNotFound);
                }

                uint newQuantity = alreadyExistCartItem.Quantity + createDto.Quantity;

                if (productDto.QuantityInStock < newQuantity)
                {
                    _logger.LogWarning("Cannot change cartItem quantity. Lack of goods in stock. CartItemId: {CartItemId}.", alreadyExistCartItem.Id);
                    return Result<CartItemDto, CartItemError>.Fail(CartItemError.OutOfStock);
                }

                alreadyExistCartItem.Quantity = newQuantity;
                alreadyExistCartItem.UpdatedAt = DateTime.UtcNow;

                await _cartItemRepository.SaveChangesAsync(ct);
                _logger.LogInformation("CartItem updated. CartItemId: {CartItemId}.", alreadyExistCartItem.Id);

                return Result<CartItemDto, CartItemError>.Ok(_mapper.Map<CartItemDto>(alreadyExistCartItem));
            }


            // Product doesn't exist in cart as cartItem

            CartItem cartItem = _mapper.Map<CartItem>(createDto);

            _logger.LogInformation("Checking product availability. ProductId: {ProductId}.", createDto.ProductId);


            productDto = await GetProductAsync(createDto.ProductId, ct);
            if (productDto is null)
            {
                _logger.LogWarning("Cannot create cartItem. Product not found. {ProductId}.", createDto.ProductId);
                return Result<CartItemDto, CartItemError>.Fail(CartItemError.ProductNotFound);
            }

            if (productDto.QuantityInStock < createDto.Quantity)
            {
                _logger.LogWarning("Cannot create cartItem. Lack of goods in stock.");
                return Result<CartItemDto, CartItemError>.Fail(CartItemError.OutOfStock);
            }

            cartItem.Id = Guid.Empty;
            cartItem.ProductName = productDto.Title;
            cartItem.UnitPrice = productDto.Price;
            cartItem.CreatedAt = DateTime.UtcNow;

            CartItem createdCartItem = await _cartItemRepository.InsertAsync(cartItem, ct);
            _logger.LogInformation("CartItem created. CartItemId: {CartItemId}, ProductId: {ProductId}, Quantity: {Quantity}.", createdCartItem.Id, createdCartItem.ProductId, createdCartItem.Quantity);

            return Result<CartItemDto, CartItemError>.Ok(_mapper.Map<CartItemDto>(createdCartItem));
        }



        /// <summary>
        /// Changes the quantity of a cart item or removes it from the cart if the quantity is set to zero.
        /// </summary>
        /// <remarks>If the specified cart item does not exist, or if the associated product is not found,
        /// the operation fails with a corresponding error. If the requested quantity exceeds the available stock, the
        /// operation fails with an out-of-stock error. Setting the quantity to zero removes the item from the
        /// cart.</remarks>
        /// <param name="cartItemId">The unique identifier of the cart item to update.</param>
        /// <param name="changeDto">An object containing the new quantity for the cart item. The quantity must be zero or a positive integer.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A result containing the updated cart item if the operation succeeds, or an error indicating why the change
        /// could not be made. If the quantity is set to zero, the result contains the deleted cart item.</returns>
        public async Task<Result<CartItemDto, CartItemError>> ChangeCartItemQuantityAsync(Guid cartItemId, ChangeQuantityCartItemDto changeDto, CancellationToken ct = default)
        {
            _logger.LogInformation("Changing cartItem quantity. CartItemId: {CartItemId}.", cartItemId);

            CartItem? cartItem = await _cartItemRepository.FindByIdAsync(cartItemId, ct);
            if (cartItem is null)
            {
                _logger.LogInformation("Cannot change cartItem quantity. CartItem not found. CartItemId: {CartItemId}.", cartItemId);
                return Result<CartItemDto, CartItemError>.Fail(CartItemError.ProductNotFound);
            }

            if (changeDto.Quantity == 0)
            {
                _logger.LogInformation("Quantity is 0. Deleting cartItem. CartItemId: {CartItemId}.", cartItem.Id);

                CartItemDto deletedCartItem = _mapper.Map<CartItemDto>(cartItem);

                _cartItemRepository.Remove(cartItem);
                await _cartItemRepository.SaveChangesAsync(ct);
                _logger.LogInformation("CartItem deleted. CartItemId: {CartItemId}.", cartItemId);

                return Result<CartItemDto, CartItemError>.Ok(deletedCartItem);
            }

            ProductDto? productDto;

            _logger.LogInformation("Checking product availability. ProductId: {ProductId}.", cartItem.ProductId);

            productDto = await GetProductAsync(cartItem.ProductId, ct);
            if (productDto is null)
            {
                _logger.LogWarning("Cannot change cartItem quantity. Product not foud. ProdutId: {ProductId}.", cartItem.ProductId);
                return Result<CartItemDto, CartItemError>.Fail(CartItemError.ProductNotFound);
            }


            if (productDto.QuantityInStock < changeDto.Quantity)
            {
                _logger.LogWarning("Cannot change cartItem quantity. Lack of goods in stock.");
                return Result<CartItemDto, CartItemError>.Fail(CartItemError.OutOfStock);
            }

            cartItem.Quantity = changeDto.Quantity;
            cartItem.UpdatedAt = DateTime.UtcNow;

            await _cartItemRepository.SaveChangesAsync(ct);
            _logger.LogInformation("CartItem updated. CartItemId: {CartItemId}.", cartItemId);

            return Result<CartItemDto, CartItemError>.Ok(_mapper.Map<CartItemDto>(cartItem));
        }



        public async Task<CartItemDto?> DeleteCartItemAsync(Guid cartItemId, CancellationToken ct = default)
        {
            _logger.LogInformation("Deleting cartItem. CartItemId: {CartItemId}.", cartItemId);

            CartItem? cartItem = await _cartItemRepository.FindByIdAsync(cartItemId, ct);
            if (cartItem is null)
            {
                _logger.LogWarning("Cannot delete. CartItem not foud. CartItemId: {CartItemId}.", cartItemId);
                return null;
            }

            CartItemDto deletedCartItem = _mapper.Map<CartItemDto>(cartItem);

            _cartItemRepository.Remove(cartItem);
            await _cartItemRepository.SaveChangesAsync(ct);
            _logger.LogInformation("CartItem deleted. CartItemId: {CartItemId}.", cartItemId);

            return deletedCartItem;
        }



    }
}
