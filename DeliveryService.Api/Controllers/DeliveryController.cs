using DeliveryService.Application.DTOs.Delivery;
using DeliveryService.Application.Interfaces.Services;
using DeliveryService.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = UserRoles.User)]
    public class DeliveryController(IDeliveryService deliveryService) : ControllerBase
    {
        private readonly IDeliveryService _deliveryService = deliveryService;



        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetDelivery(Guid orderId, CancellationToken ct)
        {
            DeliveryExtendedDto? delivery = await _deliveryService.GetDeliveryByOrderIdAsync(orderId, ct);

            return delivery is not null ? Ok(delivery) : NotFound();
        }



        [HttpPost]
        public async Task<IActionResult> CreateDelivery([FromBody] CreateUpdateDeliveryDto createDto, CancellationToken ct)
        {
            DeliveryDto delivery = await _deliveryService.CreateDeliveryAsync(createDto, ct);

            return CreatedAtAction(nameof(GetDelivery), new { orderId = delivery.OrderId }, delivery);
        }



        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut("{deliveryId}")]
        public async Task<IActionResult> UpdateDelivery(Guid deliveryId, [FromBody] CreateUpdateDeliveryDto updateDto, CancellationToken ct)
        {
            DeliveryDto? delivery = await _deliveryService.UpdateDeliveryAsync(deliveryId, updateDto, ct);

            return delivery is not null ? Ok(delivery) : NotFound();
        }



        [Authorize(Roles = UserRoles.Admin)]
        [HttpDelete("{deliveryId}")]
        public async Task<IActionResult> DeleteDelivery(Guid deliveryId, CancellationToken ct)
        {
            DeliveryDto? delivery = await _deliveryService.DeleteDeliveryAsync(deliveryId, ct);

            return delivery is not null ? Ok(delivery) : NotFound();
        }



        [HttpPatch("{deliveryId}/status")]
        public async Task<IActionResult> ChangeDeliveryStatus(Guid deliveryId, [FromBody] ChangeDeliveryStatusDto changeDto, CancellationToken ct)
        {
            DeliveryDto? delivery = await _deliveryService.ChangeDeliveryStatusAsync(deliveryId, changeDto, ct);

            return delivery is not null ? Ok(delivery) : NotFound();
        }



    }
}
