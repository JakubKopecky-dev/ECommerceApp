using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DeliveryService.Application.DTOs.Delivery;
using DeliveryService.Application.Interfaces.Repositories;
using DeliveryService.Application.Interfaces.Services;
using DeliveryService.Domain.Entity;
using DeliveryService.Domain.Enum;
using Microsoft.Extensions.Logging;

namespace DeliveryService.Application.Services
{
    public class DeliveryService(IDeliveryRepository deliveryRepository, IMapper mapper, ILogger<DeliveryService> logger) : IDeliveryService
    {
        private readonly IDeliveryRepository _deliveryRepository = deliveryRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<DeliveryService> _logger = logger;



        private static bool IsStatusChangeValid(DeliveryStatus current, DeliveryStatus next)
        {
            return (current, next) switch
            {
                (DeliveryStatus.Pending, DeliveryStatus.InProgress) => true,
                (DeliveryStatus.InProgress, DeliveryStatus.Delivered) => true,
                (DeliveryStatus.Pending, DeliveryStatus.Canceled) => true,
                (DeliveryStatus.InProgress, DeliveryStatus.Canceled) => true,
                _ => false
            };
        }



        public async Task<DeliveryExtendedDto?> GetDeliveryByOrderIdAsync(Guid orderId)
        {
            _logger.LogInformation("Retrieving delivry by orderId. OrderId: {OrderId}.", orderId);

            Delivery? delivery = await _deliveryRepository.FindDeliveryByOrderIdIncludeCourierAsync(orderId);
            if (delivery is null)
            {
                _logger.LogWarning("Delivery not found. OrderId: {OrderId}.", orderId);
                return null;
            }

            _logger.LogInformation("Delivery found. DeliveryId: {DeliveryId}, OrderId: {OrderId}.", delivery.Id, orderId);

            return _mapper.Map<DeliveryExtendedDto>(delivery);
        }



        public async Task<DeliveryDto> CreateDeliveryAsync(CreateUpdateDeliveryDto createDto)
        {
            _logger.LogInformation("Creating new delivery. OrderId: {OrderId}.", createDto.OrderId);

            Delivery delivery = _mapper.Map<Delivery>(createDto);
            delivery.Id = default;
            delivery.CreatedAt = DateTime.UtcNow;
            delivery.Status = DeliveryStatus.Pending;

            Delivery createdDelivery = await _deliveryRepository.InsertAsync(delivery);
            _logger.LogInformation("Delivery created. DelivryId: {DeliveryId}, OrderId: {OrderId}.", createdDelivery.Id, createdDelivery.OrderId);

            return _mapper.Map<DeliveryDto>(createdDelivery);
        }



        public async Task<DeliveryDto?> UpdateDeliveryAsync(Guid deliveryId, CreateUpdateDeliveryDto updateDto)
        {
            _logger.LogInformation("Updating delivry. DeliveryId: {DeliveryId}.", deliveryId);

            Delivery? deliveryDb = await _deliveryRepository.FindByIdAsync(deliveryId);
            if (deliveryDb is null)
            {
                _logger.LogWarning("Cannot update. Delivery not found. DeliveryId: {DeliveryId}.", deliveryId);
                return null;
            }

            _mapper.Map<CreateUpdateDeliveryDto, Delivery>(updateDto, deliveryDb);

            deliveryDb.UpdatedAt = DateTime.UtcNow;

            Delivery updatedDelivery = await _deliveryRepository.UpdateAsync(deliveryDb);
            _logger.LogInformation("Delivery updated. DeliveryId: {DeliveryId}.", deliveryId);

            return _mapper.Map<DeliveryDto>(updatedDelivery);
        }



        public async Task<DeliveryDto?> DeleteDeliveryAsync(Guid deliveryId)
        {
            _logger.LogInformation("Deleting delivery. DeliveryId: {DeliveryId}.", deliveryId);

            Delivery? delivery = await _deliveryRepository.FindByIdAsync(deliveryId);
            if (delivery is null)
            {
                _logger.LogWarning("Cannot delete. Delivery not found. DeliveryId: {DeliveryId}.", deliveryId);
                return null;
            }

            DeliveryDto deletedDelivery = _mapper.Map<DeliveryDto>(delivery);

            await _deliveryRepository.DeleteAsync(deliveryId);
            _logger.LogInformation("Delivery deleted. DeliveryId: {DeliveryId}.", deliveryId);

            return deletedDelivery;
        }



        public async Task<DeliveryDto?> ChangeDeliveryStatusAsync(Guid deliveryId, ChangeDeliveryStatusDto changeDto)
        {
            _logger.LogInformation("Chanding delivryStatus. DeliveryId: {DeliveryId}.", deliveryId);

            Delivery? delivery = await _deliveryRepository.FindByIdAsync(deliveryId);
            if (delivery is null)
            {
                _logger.LogWarning("Cannot change deliveryStatus. Delivery not found. DeliveryId: {DeliveryId}.", deliveryId);
                return null;
            }

            DeliveryStatus currentStatus = delivery.Status;

            if (!IsStatusChangeValid(currentStatus, changeDto.Status))
            {
                _logger.LogInformation("Cannot change deliveryStatus. DeliveryId: {DeliveryId}.", deliveryId);
                return null;
            }

            delivery.Status = changeDto.Status;
            delivery.UpdatedAt = DateTime.UtcNow;

            if(changeDto.Status == DeliveryStatus.Delivered)
                delivery.DeliveredAt = DateTime.UtcNow;

            Delivery updatedDelivery = await _deliveryRepository.UpdateAsync(delivery);
            _logger.LogInformation("DeliveryStatus changed. DeliveryId: {DeliveryId}.", deliveryId);

            return _mapper.Map<DeliveryDto>(updatedDelivery);
        }



    }
}
