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
        public async Task<IReadOnlyList<OrderDto>> GetAllOrders(CancellationToken ct) => await _orderService.GetAllOrdersAsync(ct);



        [HttpGet("my")]
        public async Task<IActionResult> GetAllOrdersByUserId(CancellationToken ct)
        {
            string? userIdStrig = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStrig, out Guid userId))
                return Unauthorized();

            IReadOnlyList<OrderDto> orders = await _orderService.GetAllOrdersByUserIdAsync(userId, ct);

            return Ok(orders);
        }



        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrder(Guid orderId, CancellationToken ct)
        {
            OrderExtendedDto? order = await _orderService.GetOrderAsync(orderId, ct);

            return order is not null ? Ok(order) : NotFound();
        }



        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto createDto, CancellationToken ct)
        {
            OrderDto order = await _orderService.CreateOrderAsync(createDto, ct);

            return CreatedAtAction(nameof(CreateOrder), new { orderId = order.Id }, order);
        }



        [HttpPatch("{orderId}")]
        public async Task<IActionResult> UpdateOrder(Guid orderId, [FromBody] UpdateOrderNoteDto updateDto, CancellationToken ct)
        {
            OrderDto? order = await _orderService.UpdateOrderNoteAsync(orderId, updateDto, ct);

            return order is not null ? Ok(order) : NotFound();
        }



        [Authorize(Roles = UserRoles.Admin)]
        [HttpDelete("{orderId}")]
        public async Task<IActionResult> DeleteOrder(Guid orderId, CancellationToken ct)
        {
            OrderDto? order = await _orderService.DeleteOrderAsync(orderId, ct);

            return order is not null ? Ok(order) : NotFound();
        }



        [HttpPatch("{orderId}/status")]
        public async Task<IActionResult> ChangeOrderStatus(Guid orderId, ChangeOrderStatusDto changeStatus, CancellationToken ct)
        {
            OrderDto? order = await _orderService.ChangeOrderStatusAsync(orderId, changeStatus, ct);

            return order is not null ? Ok(order) : NotFound();
        }



        [HttpPost("external")]
        public async Task<IActionResult> CreateOrderFromCart([FromBody] ExternalCreateOrderDto createDto, CancellationToken ct)
        {
            OrderDto? order = await _orderService.CreateOrderAndDeliveryFromCartAsync(createDto, ct);
            if (order is null)
                return BadRequest(new {Message = "Order created but delivery not created."});
            
            return CreatedAtAction(nameof(CreateOrder), new { orderId = order.Id }, order);
        }




    }
}
