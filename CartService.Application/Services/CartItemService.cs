using System.Net.Http.Json;
using AutoMapper;
using CartService.Application.Common;
using CartService.Application.DTOs.CartItem;
using CartService.Application.DTOs.External;
using CartService.Application.Interfaces.Repositories;
using CartService.Application.Interfaces.Services;
using CartService.Domain.Entity;
using Microsoft.Extensions.Logging;

namespace CartService.Application.Services
{
    public class CartItemService(ICartItemRepository cartItemRepository, IMapper mapper, ILogger<CartItemService> logger, IHttpClientFactory httpClientFactory) : ICartItemService
    {
        private readonly ICartItemRepository _cartItemRepository = cartItemRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<CartItemService> _logger = logger;
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;



        private async Task<ProductDto?> GetProductAsync(Guid productId, CancellationToken ct)
        {
            var httpClient = _httpClientFactory.CreateClient("ProductService");

            return await httpClient.GetFromJsonAsync<ProductDto>($"api/Product/{productId}", ct);
        }



        public async Task<CartItemDto?> GetCartItemAsync(Guid cartItemId, CancellationToken ct = default)
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



        public async Task<Result<CartItemDto,CartItemError>> CreateCartItemOrChangeQuantityAsync(CreateCartItemDto createDto, CancellationToken ct = default)
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
                    return Result<CartItemDto,CartItemError>.Fail(CartItemError.ProductNotFound);
                }

                uint newQuantity = alreadyExistCartItem.Quantity + createDto.Quantity;

                if (productDto.QuantityInStock < newQuantity)
                {
                    _logger.LogWarning("Cannot change cartItem quantity. Lack of goods in stock. CartItemId: {CartItemId}.", alreadyExistCartItem.Id);
                    return Result<CartItemDto,CartItemError>.Fail(CartItemError.OutOfStock);
                }

                alreadyExistCartItem.Quantity = newQuantity;
                alreadyExistCartItem.UpdatedAt = DateTime.UtcNow;

                CartItem updatedCartItem = await _cartItemRepository.UpdateAsync(alreadyExistCartItem, ct);
                _logger.LogInformation("CartItem updated. CartItemId: {CartItemId}.", updatedCartItem.Id);

                return Result<CartItemDto,CartItemError>.Ok(_mapper.Map<CartItemDto>(updatedCartItem));
            }


            // Product doesn't exist in cart as cartItem

            CartItem cartItem = _mapper.Map<CartItem>(createDto);

            _logger.LogInformation("Checking product availability. ProductId: {ProductId}.", createDto.ProductId);


            productDto = await GetProductAsync(createDto.ProductId, ct);
            if (productDto is null)
            {
                _logger.LogWarning("Cannot create cartItem. Product not found. {ProductId}.", createDto.ProductId);
                return Result<CartItemDto,CartItemError>.Fail(CartItemError.ProductNotFound);
            }

            if (productDto.QuantityInStock < createDto.Quantity)
            {
                _logger.LogWarning("Cannot create cartItem. Lack of goods in stock.");
                return Result<CartItemDto,CartItemError>.Fail(CartItemError.OutOfStock);
            }

            cartItem.Id = Guid.Empty;
            cartItem.ProductName = productDto.Title;
            cartItem.UnitPrice = productDto.Price;
            cartItem.CreatedAt = DateTime.UtcNow;

            CartItem createdCartItem = await _cartItemRepository.InsertAsync(cartItem, ct);
            _logger.LogInformation("CartItem created. CartItemId: {CartItemId}, ProductId: {ProductId}, Quantity: {Quantity}.", createdCartItem.Id, createdCartItem.ProductId, createdCartItem.Quantity);

            return Result<CartItemDto, CartItemError>.Ok(_mapper.Map<CartItemDto>(createdCartItem));
        }



        public async Task<Result<CartItemDto,CartItemError>> ChangeCartItemQuantityAsync(Guid cartItemId, ChangeQuantityCartItemDto changeDto, CancellationToken ct = default)
        {
            _logger.LogInformation("Changing cartItem quantity. CartItemId: {CartItemId}.", cartItemId);

            CartItem? cartItem = await _cartItemRepository.FindByIdAsync(cartItemId, ct);
            if (cartItem is null)
            {
                _logger.LogInformation("Cannot change cartItem quantity. CartItem not found. CartItemId: {CartItemId}.", cartItemId);
                return Result<CartItemDto,CartItemError>.Fail(CartItemError.ProductNotFound);
            }

            if (changeDto.Quantity == 0)
            {
                _logger.LogInformation("Quantity is 0. Deleting cartItem. CartItemId: {CartItemId}.", cartItem.Id);

                CartItemDto deletedCartItem = _mapper.Map<CartItemDto>(cartItem);

                await _cartItemRepository.DeleteAsync(cartItemId, ct);
                _logger.LogInformation("CartItem deleted. CartItemId: {CartItemId}.", cartItemId);

                return Result<CartItemDto,CartItemError>.Ok(deletedCartItem);
            }

            ProductDto? productDto;

            _logger.LogInformation("Checking product availability. ProductId: {ProductId}.", cartItem.ProductId);

            productDto = await GetProductAsync(cartItem.ProductId, ct);
            if (productDto is null)
            {
                _logger.LogWarning("Cannot change cartItem quantity. Product not foud. ProdutId: {ProductId}.", cartItem.ProductId);
                return Result<CartItemDto,CartItemError>.Fail(CartItemError.ProductNotFound);
            }


            if (productDto.QuantityInStock < changeDto.Quantity)
            {
                _logger.LogWarning("Cannot change cartItem quantity. Lack of goods in stock.");
                return Result<CartItemDto,CartItemError>.Fail(CartItemError.OutOfStock);
            }

            cartItem.Quantity = changeDto.Quantity;
            cartItem.UpdatedAt = DateTime.UtcNow;

            CartItem updatedCartItem = await _cartItemRepository.UpdateAsync(cartItem, ct);
            _logger.LogInformation("CartItem updated. CartItemId: {CartItemId}.", cartItemId);

            return Result<CartItemDto, CartItemError>.Ok(_mapper.Map<CartItemDto>(updatedCartItem));
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

            await _cartItemRepository.DeleteAsync(cartItemId, ct);
            _logger.LogInformation("CartItem deleted. CartItemId: {CartItemId}.", cartItemId);

            return deletedCartItem;
        }



    }
}
