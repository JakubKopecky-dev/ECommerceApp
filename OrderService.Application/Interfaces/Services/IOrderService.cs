using OrderService.Application.DTOs.Order;

namespace OrderService.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<OrderDto?> ChangeOrderStatusAsync(Guid orderId, ChangeOrderStatusDto statusDto);
        Task<OrderDto> CreateOrderAsync(CreateOrderDto createDto);
        Task<OrderDto> CreateOrderFromCartAsync(ExternalCreateOrderDto createDto);
        Task<OrderDto?> DeleteOrderAsync(Guid orderId);
        Task<IReadOnlyList<OrderDto>> GetAllOrdersAsync();
        Task<IReadOnlyList<OrderDto>> GetAllOrdersByUserIdAsync(Guid userId);
        Task<OrderExtendedDto?> GetOrderAsync(Guid orderId);
        Task<OrderDto?> SetOrderStatusCompletedFromDelivery(Guid orderId);
        Task<OrderDto?> UpdateOrderNoteAsync(Guid orderId, UpdateOrderNoteDto updateDto);
    }
}
