using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderService.Application.DTOs.OrderItem;

namespace OrderService.Application.Interfaces.Services
{
    public interface IOrderItemService
    {
        Task<OrderItemDto?> ChangeOrderItemQuantityAsync(Guid orderItemId, ChangeOrderItemQuantityDto changeDto);
        Task<OrderItemDto> CreateOrderItemAsync(CreateOrderItemDto createDto);
        Task<OrderItemDto?> DeleteOrderItemAsync(Guid orderItemId);
        Task<IReadOnlyList<OrderItemDto>> GetAllOrderItemsByOrderIdAsync(Guid orderId);
        Task<OrderItemDto?> GetOrderItemAsync(Guid orderItemId);
    }
}
