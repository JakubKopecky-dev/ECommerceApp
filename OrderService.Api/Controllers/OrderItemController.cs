using Microsoft.AspNetCore.Mvc;
using OrderService.Application.DTOs.OrderItem;
using OrderService.Application.Interfaces.Services;

namespace OrderService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderItemController(IOrderItemService orderItemService) : ControllerBase
    {
        private readonly IOrderItemService _orderItemService = orderItemService;



        [HttpGet]
        public async Task<IReadOnlyList<OrderItemDto>> GetAllOrderItems() => await _orderItemService.GetAllOrderItemsAsync();



        [HttpGet("{orderItemId}")]
        public async Task<IActionResult> GetOrderItem(Guid orderItemId)
        {
            OrderItemDto? orderItem = await _orderItemService.GetOrderItemAsync(orderItemId);

            return orderItem is not null ? Ok(orderItem) : NotFound();
        }



        [HttpPost]
        public async Task<IActionResult> CreateOrderItem([FromBody] CreateOrderItemDto createDto)
        {
            OrderItemDto orderItem = await _orderItemService.CreateOrderItemAsync(createDto);

            return CreatedAtAction(nameof(GetOrderItem), new { orderItemId = orderItem.Id }, orderItem);
        }



        [HttpPatch("{orderItemId}")]
        public async Task<IActionResult> ChangeOrderItemQuantity(Guid orderItemId, [FromBody] ChangeOrderItemQuantityDto changeDto)
        {
            OrderItemDto? orderItem = await _orderItemService.ChangeOrderItemQuantityAsync(orderItemId, changeDto);

            return orderItem is not null ? Ok(orderItem) : NotFound();
        }



        [HttpDelete("{orderItemId}")]
        public async Task<IActionResult> DeleteOrderItem(Guid orderItemId)
        {
            OrderItemDto? orderItem = await _orderItemService.DeleteOrderItemAsync(orderItemId);

            return orderItem is not null ? Ok(orderItem) : NotFound();
        }



    }
}
