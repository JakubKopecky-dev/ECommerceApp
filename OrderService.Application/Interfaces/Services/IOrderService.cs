using OrderService.Application.DTOs.Order;

namespace OrderService.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<OrderDto?> ChangeOrderStatusAsync(Guid orderId, ChangeOrderStatusDto statusDto, CancellationToken ct = default);
        Task<OrderDto> CreateOrderAsync(CreateOrderDto createDto, CancellationToken ct = default);
        Task<OrderDto> CreateOrderFromCartAsync(ExternalCreateOrderDto createDto, CancellationToken ct = default);
        Task<OrderDto?> DeleteOrderAsync(Guid orderId, CancellationToken ct = default);
        Task<IReadOnlyList<OrderDto>> GetAllOrdersAsync(CancellationToken ct = default);
        Task<IReadOnlyList<OrderDto>> GetAllOrdersByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<OrderExtendedDto?> GetOrderAsync(Guid orderId, CancellationToken ct = default);
        Task<OrderDto?> SetOrderStatusCompletedFromDelivery(Guid orderId, CancellationToken ct = default);
        Task<OrderDto?> UpdateOrderNoteAsync(Guid orderId, UpdateOrderNoteDto updateDto, CancellationToken ct = default);
    }
}
