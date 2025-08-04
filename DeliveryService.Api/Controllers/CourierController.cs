using DeliveryService.Application.DTOs.Courier;
using DeliveryService.Application.Interfaces.Services;
using DeliveryService.Domain.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = UserRoles.User)]
    public class CourierController(ICourierService courierService) : ControllerBase
    {
        private readonly ICourierService _courierService = courierService;


        [HttpGet]
        public async Task<IReadOnlyList<CourierDto>> GetAllCouriers() => await _courierService.GetAllCouriesAsync();



        [HttpGet("{courierId}")]
        public async Task<IActionResult> GetCourier(Guid courierId)
        {
            CourierDto? courier = await _courierService.GetCourierAsync(courierId);

            return courier is not null ? Ok(courier) : NotFound();
        }


        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateCourier([FromBody] CreateUpdateCourierDto createDto)
        {
            CourierDto courier = await _courierService.CreateCourierAsync(createDto);

            return CreatedAtAction(nameof(GetCourier), new { courierId = courier.Id }, courier);
        }


        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut("{courierId}")]
        public async Task<IActionResult> UpdateCourier(Guid courierId, [FromBody] CreateUpdateCourierDto updateCourierDto)
        {
            CourierDto? courier = await _courierService.UpdateCourierAsync(courierId, updateCourierDto);

            return courier is not null ? Ok(courier) : NotFound();
        }


        [Authorize(Roles = UserRoles.Admin)]
        [HttpDelete("{courierId}")]
        public async Task<IActionResult> DeleteCourier(Guid courierId)
        {
            CourierDto? courier = await _courierService.DeleteCourierAsync(courierId);

            return courier is not null ? Ok(courier) : NotFound();
        }



    }
}
