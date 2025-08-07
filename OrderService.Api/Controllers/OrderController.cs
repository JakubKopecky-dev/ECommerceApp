using System.Security.Claims;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.DTOs.Order;
using OrderService.Application.Interfaces.Services;
using OrderService.Domain.Enum;
using Shared.Contracts.Events;

namespace OrderService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = UserRoles.User)]
    public class OrderController(IOrderService orderService) : ControllerBase
    {
        private readonly IOrderService _orderService = orderService;



        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet]
        public async Task<IReadOnlyList<OrderDto>> GetAllOrders() => await _orderService.GetAllOrdersAsync();



        [HttpGet("my")]
        public async Task<IActionResult> GetAllOrdersByUserId()
        {
            string? userIdStrig = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStrig, out Guid userId))
                return Unauthorized();

            IReadOnlyList<OrderDto> orders = await _orderService.GetAllOrdersByUserIdAsync(userId);

            return Ok(orders);
        }



        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrder(Guid orderId)
        {
            OrderExtendedDto? order = await _orderService.GetOrderAsync(orderId);

            return order is not null ? Ok(order) : NotFound();
        }



        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto createDto)
        {
            OrderDto order = await _orderService.CreateOrderAsync(createDto);

            return CreatedAtAction(nameof(CreateOrder), new { orderId = order.Id }, order);
        }



        [HttpPatch("{orderId}")]
        public async Task<IActionResult> UpdateOrder(Guid orderId, [FromBody] UpdateOrderNoteDto updateDto)
        {
            OrderDto? order = await _orderService.UpdateOrderNoteAsync(orderId, updateDto);

            return order is not null ? Ok(order) : NotFound();
        }



        [Authorize(Roles = UserRoles.Admin)]
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



        [HttpPost("external")]
        public async Task<IActionResult> CreateOrderFromCart([FromBody] ExternalCreateOrderDto createDto)
        {
            OrderDto order = await _orderService.CreateOrderFromCartAsync(createDto);

            return CreatedAtAction(nameof(CreateOrder), new { orderId = order.Id }, order);
        }




    }
}
