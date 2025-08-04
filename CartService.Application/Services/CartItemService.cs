using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CartService.Application.DTOs.CartItem;
using CartService.Application.Interfaces.Repositories;
using CartService.Application.Interfaces.Services;
using CartService.Domain.Entity;
using Microsoft.Extensions.Logging;

namespace CartService.Application.Services
{
    public class CartItemService(ICartItemRepository cartItemRepository, IMapper mapper, ILogger<CartItemService> logger) : ICartItemService
    {
        private readonly ICartItemRepository _cartItemRepository = cartItemRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<CartItemService> _logger = logger;



        public async Task<CartItemDto?> GetCartItemAsync(Guid cartItemId)
        {
            _logger.LogInformation("Retrieving cartItem. CartItemId: {CartItemId}.", cartItemId);

            CartItem? cartItem = await _cartItemRepository.FindByIdAsync(cartItemId);
            if (cartItem is null)
            {
                _logger.LogWarning("CartItem not found. CartItemId: {CartItemId}.", cartItemId);
                return null;
            }

            _logger.LogInformation("CartItem found. CartItemId: {CartItemId}.", cartItemId);

            return _mapper.Map<CartItemDto>(cartItem);
        }




        public async Task<CartItemDto> CreateCartItemAsync(CreateCartItemDto createDto)
        {
            _logger.LogInformation("Creating new cartItem. CartId: {CartId}, ProductId: {ProductId}.", createDto.CartId, createDto.ProductId);

            CartItem cartItem = _mapper.Map<CartItem>(createDto);
            cartItem.Id = default;
            cartItem.CreatedAt = DateTime.UtcNow;

            CartItem createdCartItem = await _cartItemRepository.InsertAsync(cartItem);
            _logger.LogInformation("CartItem created. CartItemId: ´{CartItemId}", createdCartItem.Id);

            return _mapper.Map<CartItemDto>(createdCartItem);
        }



        public async Task<CartItemDto?> ChangeCartItemQuantityAsync(Guid cartItemId, ChangeQuantityCartItemDto changeDto)
        {
            _logger.LogInformation("Changing cartItem quantity. CartItemId: {CartItemId}.", cartItemId);

            CartItem? cartItem = await _cartItemRepository.FindByIdAsync(cartItemId);
            if (cartItem is null)
            {
                _logger.LogInformation("Cannot change cartItem quantity. CartItem not found. CartItemId: {CartItemId}.", cartItemId);
                return null;
            }

            cartItem.Quantity = changeDto.Quantity;
            cartItem.UpdatedAt = DateTime.UtcNow;

            CartItem updatedCartItem = await _cartItemRepository.UpdateAsync(cartItem);
            _logger.LogInformation("CartItem updated. CartItemId: {CartItemId}.", cartItemId);

            return _mapper.Map<CartItemDto>(updatedCartItem);
        }



        public async Task<CartItemDto?> DeleteCartItemAsync(Guid cartItemId)
        {
            _logger.LogInformation("Deleting cartItem. CartItemId: {CartItemId}.", cartItemId);

            CartItem? cartItem = await _cartItemRepository.FindByIdAsync(cartItemId);
            if (cartItem is null)
            {
                _logger.LogWarning("Cannot delete. CartItem not foud. CartItemId: {CartItemId}.", cartItemId);
                return null;
            }

            CartItemDto deletedCartItem = _mapper.Map<CartItemDto>(cartItem);

            await _cartItemRepository.DeleteAsync(cartItemId);
            _logger.LogInformation("CartItem deleted. CartItemId: {CartItemId}.", cartItemId);

            return deletedCartItem;
        }






    }
}
