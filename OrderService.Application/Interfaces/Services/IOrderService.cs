using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderService.Application.DTOs.Order;

namespace OrderService.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<OrderDto?> ChangeOrderStatusAsync(Guid orderId, ChangeOrderStatusDto statusDto);
        Task<OrderDto> CreateOrderAsync(CreateOrderDto createDto);
        Task<OrderDto?> DeleteOrderAsync(Guid orderId);
        Task<IReadOnlyList<OrderDto>> GetAllOrdersAsync();
        Task<OrderDto?> GetOrderAsync(Guid orderId);
        Task<OrderDto?> UpdateOrderNoteAsync(Guid orderId, UpdateOrderNoteDto updateDto);
    }
}
