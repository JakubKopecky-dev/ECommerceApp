using OrderService.Application.DTOs.Order;
using OrderService.Application.DTOs.OrderItem;
using OrderService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application
{
    public static class Mapper
    {

        public static OrderDto OrderToOrderDto(this Order order) =>
            new()
            {
                Id = order.Id,
                UserId = order.UserId,
                Note = order.Note,
                Status = order.Status,
                TotalPrice = order.TotalPrice,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt
            };



        public static OrderExtendedDto OrderToOrderExtendedDto(this Order order) =>
            new()
            {
                Id = order.Id,
                UserId = order.UserId,
                TotalPrice = order.TotalPrice,
                Note = order.Note,
                Status = order.Status,
                UpdatedAt = order.UpdatedAt,
                CreatedAt = order.CreatedAt,
                Items = [..order.Items.Select( i => new OrderItemForExtendedDto
                {
                    Id= i.Id,
                    ProductId= i.ProductId,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice= i.UnitPrice,
                })]
            };




        public static OrderItemDto OrderItemToOrderItemDto(this OrderItem orderItem) =>
            new()
            {
                Id = orderItem.Id,
                UnitPrice = orderItem.UnitPrice,
                Quantity = orderItem.Quantity,
                ProductName = orderItem.ProductName,
                ProductId = orderItem.ProductId,
                OrderId = orderItem.OrderId,
                CreatedAt = orderItem.CreatedAt,
                UpdatedAt = orderItem.UpdatedAt
            };











    }
}
