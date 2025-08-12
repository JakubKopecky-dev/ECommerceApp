using OrderService.Application.DTOs.OrderItem;

namespace OrderService.Application.Interfaces.Services
{
    public interface IOrderItemService
    {
        Task<OrderItemDto?> ChangeOrderItemQuantityAsync(Guid orderItemId, ChangeOrderItemQuantityDto changeDto, CancellationToken ct = default);
        Task<OrderItemDto> CreateOrderItemAsync(CreateOrderItemDto createDto, CancellationToken ct = default);
        Task<OrderItemDto?> DeleteOrderItemAsync(Guid orderItemId, CancellationToken ct = default);
        Task<IReadOnlyList<OrderItemDto>> GetAllOrderItemsByOrderIdAsync(Guid orderId, CancellationToken ct = default);
        Task<OrderItemDto?> GetOrderItemAsync(Guid orderItemId, CancellationToken ct = default);
    }
}
