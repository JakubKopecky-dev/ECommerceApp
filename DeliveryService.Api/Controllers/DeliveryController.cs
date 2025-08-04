using DeliveryService.Application.DTOs.Delivery;
using DeliveryService.Application.Interfaces.Services;
using DeliveryService.Domain.Enum;
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
        public async Task<IActionResult> GetDelivery(Guid orderId)
        {
            DeliveryExtendedDto? delivery = await _deliveryService.GetDeliveryByOrderIdAsync(orderId);

            return delivery is not null ? Ok(delivery) : NotFound();
        }



        [HttpPost]
        public async Task<IActionResult> CreateDelivery([FromBody] CreateUpdateDeliveryDto createDto)
        {
            DeliveryDto delivery = await _deliveryService.CreateDeliveryAsync(createDto);

            return CreatedAtAction(nameof(GetDelivery), new { orderId = delivery.OrderId }, delivery);
        }


        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut("{deliveryId}")]
        public async Task<IActionResult> UpdateDelivery(Guid deliveryId, [FromBody] CreateUpdateDeliveryDto updateDto)
        {
            DeliveryDto? delivery = await _deliveryService.UpdateDeliveryAsync(deliveryId, updateDto);

            return delivery is not null ? Ok(delivery) : NotFound();
        }



        [Authorize(Roles = UserRoles.Admin)]
        [HttpDelete("{deliveryId}")]
        public async Task<IActionResult> DeleteDelivery(Guid deliveryId)
        {
            DeliveryDto? delivery = await _deliveryService.DeleteDeliveryAsync(deliveryId);

            return delivery is not null ? Ok(delivery) : NotFound();
        }



        [HttpPatch("{deliveryId}/status")]
        public async Task<IActionResult> ChangeDeliveryStatus(Guid deliveryId, [FromBody] ChangeDeliveryStatusDto changeDto)
        {
            DeliveryDto? delivery = await _deliveryService.ChangeDeliveryStatusAsync(deliveryId, changeDto);

            return delivery is not null ? Ok(delivery) : NotFound();
        }





    }
}
