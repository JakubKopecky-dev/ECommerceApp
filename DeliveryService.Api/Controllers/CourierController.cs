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
        public async Task<IReadOnlyList<CourierDto>> GetAllCouriers(CancellationToken ct) => await _courierService.GetAllCouriesAsync(ct);



        [HttpGet("{courierId}")]
        public async Task<IActionResult> GetCourier(Guid courierId, CancellationToken ct)
        {
            CourierDto? courier = await _courierService.GetCourierByIdAsync(courierId, ct);

            return courier is not null ? Ok(courier) : NotFound();
        }


        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateCourier([FromBody] CreateUpdateCourierDto createDto, CancellationToken ct)
        {
            CourierDto courier = await _courierService.CreateCourierAsync(createDto, ct);

            return CreatedAtAction(nameof(GetCourier), new { courierId = courier.Id }, courier);
        }


        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut("{courierId}")]
        public async Task<IActionResult> UpdateCourier(Guid courierId, [FromBody] CreateUpdateCourierDto updateCourierDto, CancellationToken ct)
        {
            CourierDto? courier = await _courierService.UpdateCourierAsync(courierId, updateCourierDto, ct);

            return courier is not null ? Ok(courier) : NotFound();
        }


        [Authorize(Roles = UserRoles.Admin)]
        [HttpDelete("{courierId}")]
        public async Task<IActionResult> DeleteCourier(Guid courierId, CancellationToken ct)
        {
            CourierDto? courier = await _courierService.DeleteCourierAsync(courierId, ct);

            return courier is not null ? Ok(courier) : NotFound();
        }



    }
}
