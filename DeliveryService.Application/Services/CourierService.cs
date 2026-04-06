using DeliveryService.Application.DTOs.Courier;
using DeliveryService.Application.Interfaces.Repositories;
using DeliveryService.Application.Interfaces.Services;
using DeliveryService.Domain.Entities;
using DeliveryService.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace DeliveryService.Application.Services
{
    public class CourierService(ICourierRepository courierRepository, ILogger<CourierService> logger) : ICourierService
    {
        private readonly ICourierRepository _courierRepository = courierRepository;
        private readonly ILogger<CourierService> _logger = logger;



        public async Task<IReadOnlyList<CourierDto>> GetAllCouriesAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving all couriers");

            IReadOnlyList<Courier> couriers = await _courierRepository.GetAllAsync(ct);
            _logger.LogInformation("Retrieved all couriers. Count: {Count}.", couriers.Count);

            return [.. couriers.Select(c => c.CourierToCourierDto())];
        }



        public async Task<CourierDto?> GetCourierByIdAsync(Guid courierId, CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving courier. CourierId: {courierId}.", courierId);

            Courier? courier = await _courierRepository.FindByIdAsync(courierId, ct);
            if (courier is null)
                _logger.LogInformation("Courier not found. CourierId: {CourierId}.", courierId);
            else
                _logger.LogInformation("Courier found. CourierId: {CourierId}.", courierId);

            return courier?.CourierToCourierDto();
        }



        public async Task<CourierDto> CreateCourierAsync(CreateUpdateCourierDto createDto, CancellationToken ct = default)
        {
            _logger.LogInformation("Creating new courier. Name: {Name}.", createDto.Name);

            Courier courier = Courier.Create(createDto.Name, createDto.PhoneNumber, createDto.Email is null ? null : new Email(createDto.Email));

            await _courierRepository.AddAsync(courier, ct);
            await _courierRepository.SaveChangesAsync(ct);

            _logger.LogInformation("Courier created. CourierId: {CourierId}.", courier.Id);

            return courier.CourierToCourierDto();
        }



        public async Task<CourierDto?> UpdateCourierAsync(Guid courierId, CreateUpdateCourierDto updateDto, CancellationToken ct = default)
        {
            _logger.LogInformation("Updating courier. CourierId: {CourierId}.", courierId);

            Courier? courier = await _courierRepository.FindByIdAsync(courierId, ct);
            if (courier is null)
            {
                _logger.LogWarning("Cannot udpdate. Courier not found. CourierId: {CourierId}.", courierId);
                return null;
            }

            courier.ChangeContact(updateDto.Name, updateDto.PhoneNumber, updateDto.Email is null ? null : new Email(updateDto.Email));

            await _courierRepository.SaveChangesAsync(ct);
            _logger.LogInformation("Courier updated. CourierId: {CourierId}.", courierId);

            return courier.CourierToCourierDto();
        }



        public async Task<bool> DeleteCourierAsync(Guid courierId, CancellationToken ct = default)
        {
            _logger.LogInformation("Deleting courier. CourierId: {CourierId}.", courierId);

            Courier? courier = await _courierRepository.FindByIdAsync(courierId, ct);
            if (courier is null)
            {
                _logger.LogWarning("Cannot delete. Courier not found. CourierId: {CourierId}.", courierId);
                return false;
            }

            _courierRepository.Remove(courier);
            await _courierRepository.SaveChangesAsync(ct);
            _logger.LogInformation("Courier updated. CourierId: {CourierId}.", courierId);

            return true;
        }



    }
}
