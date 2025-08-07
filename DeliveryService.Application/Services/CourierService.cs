using AutoMapper;
using DeliveryService.Application.DTOs.Courier;
using DeliveryService.Application.Interfaces.Repositories;
using DeliveryService.Application.Interfaces.Services;
using DeliveryService.Domain.Entity;
using Microsoft.Extensions.Logging;

namespace DeliveryService.Application.Services
{
    public class CourierService(ICourierRepository courierRepository, IMapper mapper, ILogger<CourierService> logger) : ICourierService
    {
        private readonly ICourierRepository _courierRepository = courierRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<CourierService> _logger = logger;



        public async Task<IReadOnlyList<CourierDto>> GetAllCouriesAsync()
        {
            _logger.LogInformation("Retrieving all couriers");

            IReadOnlyList<Courier> couriers = await _courierRepository.GetAllAsync();
            _logger.LogInformation("Retrieved all couriers. Count: {Count}.", couriers.Count);
            
            return _mapper.Map<List<CourierDto>>(couriers);
        }



        public async Task<CourierDto?> GetCourierAsync(Guid courierId)
        {
            _logger.LogInformation("Retrieving courier. CourierId: {courierId}.", courierId);

            Courier? courier = await _courierRepository.FindByIdAsync(courierId);
            if(courier is null)
            {
                _logger.LogInformation("Courier not found. CourierId: {CourierId}.", courierId);
                return null;
            }

            _logger.LogInformation("Courier found. CourierId: {CourierId}.", courierId);

            return _mapper.Map<CourierDto>(courier);
        }



        public async Task<CourierDto> CreateCourierAsync(CreateUpdateCourierDto createDto)
        {
            _logger.LogInformation("Creating new courier. Name: {Name}.", createDto.Name);

            Courier courier = _mapper.Map<Courier>(createDto);
            courier.Id = Guid.Empty;
            courier.CreatedAt = DateTime.UtcNow;

            Courier createdCourier = await _courierRepository.InsertAsync(courier);
            _logger.LogInformation("Courier created. CourierId: {CourierId}.", createdCourier.Id);

            return _mapper.Map<CourierDto>(createdCourier);
        }



        public async Task<CourierDto?> UpdateCourierAsync(Guid courierId,CreateUpdateCourierDto updateDto)
        {
            _logger.LogInformation("Updating courier. CourierId: {CourierId}.", courierId);

            Courier? courierDb = await _courierRepository.FindByIdAsync(courierId);
            if (courierDb is null)
            {
                _logger.LogWarning("Cannot udpdate. Courier not found. CourierId: {CourierId}.", courierId);
                return null;
            }

            _mapper.Map<CreateUpdateCourierDto,Courier>(updateDto,courierDb);

            courierDb.UpdatedAt = DateTime.UtcNow;

            Courier updatedCourier = await _courierRepository.UpdateAsync(courierDb);
            _logger.LogInformation("Courier updated. CourierId: {CourierId}.", courierId);

            return _mapper.Map<CourierDto>(updatedCourier);
        }



        public async Task<CourierDto?> DeleteCourierAsync(Guid courierId)
        {
            _logger.LogInformation("Deleting courier. CourierId: {CourierId}.", courierId);

            Courier? courierDb = await _courierRepository.FindByIdAsync(courierId);
            if (courierDb is null)
            {
                _logger.LogWarning("Cannot delete. Courier not found. CourierId: {CourierId}.", courierId);
                return null;
            }

            Courier deletedCourier = await _courierRepository.UpdateAsync(courierDb);
            _logger.LogInformation("Courier updated. CourierId: {CourierId}.", courierId);

            return _mapper.Map<CourierDto>(deletedCourier);
        }



    }
}
