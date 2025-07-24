using Microsoft.AspNetCore.Mvc;
using OrderService.Application.DTOs.Order;
using OrderService.Application.Interfaces.Services;
using OrderService.Domain.Entity;

namespace OrderService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController(IOrderService orderService) : ControllerBase
    {
        private readonly IOrderService _orderService = orderService;



        [HttpGet]
        public async Task<IReadOnlyList<OrderDto>> GetAllOrders() => await _orderService.GetAllOrdersAsync();



        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrder(Guid orderId)
        {
            OrderDto? order = await _orderService.GetOrderAsync(orderId);

            return order is not null ? Ok(order) : NotFound();
        }



        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto createDto)
        {
            OrderDto? order = await _orderService.CreateOrderAsync(createDto);

            return CreatedAtAction(nameof(CreateOrder), new { orderId = order.Id }, order);
        }



        [HttpPatch("{orderId}")]
        public async Task<IActionResult> UpdateOrder(Guid orderId, [FromBody] UpdateOrderNoteDto updateDto)
        {
            OrderDto? order = await _orderService.UpdateOrderNoteAsync(orderId, updateDto);

            return order is not null ? Ok(order) : NotFound();
        }



        [HttpDelete("{orderId}")]
        public async Task<IActionResult> DeleteOrder(Guid orderId)
        {
            OrderDto? order = await _orderService.DeleteOrderAsync(orderId);

            return order is not null ? Ok(order) : NotFound();
        }



        [HttpPatch("{orderId}/status")]
        public async Task<IActionResult> ChangeOrderStatus(Guid orderId, ChangeOrderStatusDto changeStatus)
        {
            OrderDto? order = await _orderService.ChangeOrderStatusAsync(orderId, changeStatus);

            return order is not null ? Ok(order) : NotFound();
        }



    }
}
