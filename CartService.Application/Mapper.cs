using CartService.Application.DTOs.Cart;
using CartService.Domain.Entities;
using CartService.Application.DTOs.CartItem;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Application
{
    public static class Mapper
    {
        public static CartExtendedDto CartToCartExtendedDto(this Cart cart) =>
            new()
            {
                Id = cart.Id,
                TotalPrice = cart.TotalPrice,
                UserId = cart.UserId,
                Items = [.. cart.Items.Select(i => new CartItemDto
                {
                    Id = i.Id,
                    CartId = i.Id,
                    UnitPrice = i.UnitPrice,
                    CreatedAt = i.CreatedAt,
                    UpdatedAt = i.UpdatedAt,
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                })]
            };



        public static CartItemDto CartItemToCartItemDto(this CartItem item) =>
            new()
            {
                Id = item.Id,
                UnitPrice = item.UnitPrice,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                CartId = item.CartId,
                ProductId = item.ProductId,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt
            };










    }
}
