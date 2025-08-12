using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.DTOs.OrderItem;
using OrderService.Application.Interfaces.Services;
using OrderService.Domain.Enum;

namespace OrderService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = UserRoles.User)]
    public class OrderItemController(IOrderItemService orderItemService) : ControllerBase
    {
        private readonly IOrderItemService _orderItemService = orderItemService;



        [HttpGet("by-order/{orderId}")]
        public async Task<IReadOnlyList<OrderItemDto>> GetAllOrderItems(Guid orderId, CancellationToken ct) => await _orderItemService.GetAllOrderItemsByOrderIdAsync(orderId, ct);



        [HttpGet("{orderItemId}")]
        public async Task<IActionResult> GetOrderItem(Guid orderItemId, CancellationToken ct)
        {
            OrderItemDto? orderItem = await _orderItemService.GetOrderItemAsync(orderItemId, ct);

            return orderItem is not null ? Ok(orderItem) : NotFound();
        }



        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateOrderItem([FromBody] CreateOrderItemDto createDto, CancellationToken ct)
        {
            OrderItemDto orderItem = await _orderItemService.CreateOrderItemAsync(createDto, ct);

            return CreatedAtAction(nameof(GetOrderItem), new { orderItemId = orderItem.Id }, orderItem);
        }



        [HttpPatch("{orderItemId}")]
        public async Task<IActionResult> ChangeOrderItemQuantity(Guid orderItemId, [FromBody] ChangeOrderItemQuantityDto changeDto, CancellationToken ct)
        {
            OrderItemDto? orderItem = await _orderItemService.ChangeOrderItemQuantityAsync(orderItemId, changeDto, ct);

            return orderItem is not null ? Ok(orderItem) : NotFound();
        }



        [Authorize(Roles = UserRoles.Admin)]
        [HttpDelete("{orderItemId}")]
        public async Task<IActionResult> DeleteOrderItem(Guid orderItemId, CancellationToken ct)
        {
            OrderItemDto? orderItem = await _orderItemService.DeleteOrderItemAsync(orderItemId, ct);

            return orderItem is not null ? Ok(orderItem) : NotFound();
        }



    }
}
