using OrderService.Application.DTOs.Order;

namespace OrderService.Application.Interfaces.Services
{
    public interface IOrderService
    {
        Task<OrderDto?> ChangeOrderStatusAsync(Guid orderId, ChangeOrderStatusDto statusDto, CancellationToken ct = default);
        Task<OrderDto> CreateOrderAsync(CreateOrderDto createDto, CancellationToken ct = default);
        Task<OrderDto?> DeleteOrderAsync(Guid orderId, CancellationToken ct = default);
        Task<IReadOnlyList<OrderDto>> GetAllOrdersAsync(CancellationToken ct = default);
        Task<IReadOnlyList<OrderDto>> GetAllOrdersByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<OrderExtendedDto?> GetOrderByIdAsync(Guid orderId, CancellationToken ct = default);
        Task<OrderDto?> SetOrderStatusCompletedFromDelivery(Guid orderId, CancellationToken ct = default);
        Task<OrderDto?> UpdateOrderNoteAsync(Guid orderId, UpdateOrderNoteDto updateDto, CancellationToken ct = default);
        Task<CreateOrderFromCartResponseDto> CreateOrderAndDeliveryFromCartAsync(ExternalCreateOrderDto createDto, CancellationToken ct = default);
        Task<OrderDto?> ChangeInternalOrderStatusAsync(Guid orderId, ChangeInternalOrderStatusDto changeDto, CancellationToken ct = default);
        Task<IReadOnlyList<OrderDto>> GetAllOrdersWithDeliveryFaildInternalStatusAsync(CancellationToken ct = default);
    }
}
