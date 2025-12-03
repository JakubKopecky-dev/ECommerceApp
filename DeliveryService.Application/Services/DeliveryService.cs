using System.Net.Http.Headers;
using System.Net.Http.Json;
using AutoMapper;
using DeliveryService.Application.DTOs.Delivery;
using DeliveryService.Application.DTOs.External;
using DeliveryService.Application.Interfaces.External;
using DeliveryService.Application.Interfaces.Repositories;
using DeliveryService.Application.Interfaces.Services;
using DeliveryService.Domain.Entities;
using DeliveryService.Domain.Enums;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Events;

namespace DeliveryService.Application.Services
{
    public class DeliveryService(IDeliveryRepository deliveryRepository, IMapper mapper, ILogger<DeliveryService> logger, IPublishEndpoint publishEndpoint, IOrderReadClient orderReadClient) : IDeliveryService
    {
        private readonly IDeliveryRepository _deliveryRepository = deliveryRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<DeliveryService> _logger = logger;
        private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
        private readonly IOrderReadClient _orderReadClient = orderReadClient;




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



        public async Task<DeliveryExtendedDto?> GetDeliveryByOrderIdAsync(Guid orderId, CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving delivry by orderId. OrderId: {OrderId}.", orderId);

            Delivery? delivery = await _deliveryRepository.FindDeliveryByOrderIdIncludeCourierAsync(orderId, ct);
            if (delivery is null)
            {
                _logger.LogWarning("Delivery not found. OrderId: {OrderId}.", orderId);
                return null;
            }

            _logger.LogInformation("Delivery found. DeliveryId: {DeliveryId}, OrderId: {OrderId}.", delivery.Id, orderId);

            return _mapper.Map<DeliveryExtendedDto>(delivery);
        }



        public async Task<DeliveryDto> CreateDeliveryAsync(CreateUpdateDeliveryDto createDto, CancellationToken ct = default)
        {
            _logger.LogInformation("Creating new delivery. OrderId: {OrderId}.", createDto.OrderId);

            Delivery delivery = _mapper.Map<Delivery>(createDto);
            delivery.Id = Guid.Empty;
            delivery.CreatedAt = DateTime.UtcNow;
            delivery.Status = DeliveryStatus.Pending;

            Delivery createdDelivery = await _deliveryRepository.InsertAsync(delivery, ct);
            _logger.LogInformation("Delivery created. DelivryId: {DeliveryId}, OrderId: {OrderId}.", createdDelivery.Id, createdDelivery.OrderId);

            return _mapper.Map<DeliveryDto>(createdDelivery);
        }



        public async Task<DeliveryDto?> UpdateDeliveryAsync(Guid deliveryId, CreateUpdateDeliveryDto updateDto, CancellationToken ct = default)
        {
            _logger.LogInformation("Updating delivry. DeliveryId: {DeliveryId}.", deliveryId);

            Delivery? deliveryDb = await _deliveryRepository.FindByIdAsync(deliveryId, ct);
            if (deliveryDb is null)
            {
                _logger.LogWarning("Cannot update. Delivery not found. DeliveryId: {DeliveryId}.", deliveryId);
                return null;
            }

            _mapper.Map<CreateUpdateDeliveryDto, Delivery>(updateDto, deliveryDb);

            deliveryDb.UpdatedAt = DateTime.UtcNow;

            await _deliveryRepository.SaveChangesAsync(ct);
            _logger.LogInformation("Delivery updated. DeliveryId: {DeliveryId}.", deliveryId);

            return _mapper.Map<DeliveryDto>(deliveryDb);
        }



        public async Task<DeliveryDto?> DeleteDeliveryAsync(Guid deliveryId, CancellationToken ct = default)
        {
            _logger.LogInformation("Deleting delivery. DeliveryId: {DeliveryId}.", deliveryId);

            Delivery? delivery = await _deliveryRepository.FindByIdAsync(deliveryId, ct);
            if (delivery is null)
            {
                _logger.LogWarning("Cannot delete. Delivery not found. DeliveryId: {DeliveryId}.", deliveryId);
                return null;
            }

            DeliveryDto deletedDelivery = _mapper.Map<DeliveryDto>(delivery);

            _deliveryRepository.Remove(delivery);
            await _deliveryRepository.SaveChangesAsync(ct);
            _logger.LogInformation("Delivery deleted. DeliveryId: {DeliveryId}.", deliveryId);

            return deletedDelivery;
        }



        public async Task<DeliveryDto?> ChangeDeliveryStatusAsync(Guid deliveryId, ChangeDeliveryStatusDto changeDto, CancellationToken ct = default)
        {
            _logger.LogInformation("Chanding delivryStatus. DeliveryId: {DeliveryId}.", deliveryId);

            Delivery? delivery = await _deliveryRepository.FindByIdAsync(deliveryId, ct);
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

            if (changeDto.Status == DeliveryStatus.Delivered)
                delivery.DeliveredAt = DateTime.UtcNow;

            await _deliveryRepository.SaveChangesAsync(ct);
            _logger.LogInformation("DeliveryStatus changed. DeliveryId: {DeliveryId}.", deliveryId);


            // changing status on order to completed
            if (delivery.Status == DeliveryStatus.Delivered)
            {
                DeliveryDeliveredEvent deliveryDeliveredEvent = new() { OrderId = delivery.OrderId };
                await _publishEndpoint.Publish(deliveryDeliveredEvent, ct);
            }
            
            
            // notification user delivery canceled
            if (delivery.Status == DeliveryStatus.Canceled)
            {
                Guid? userId = await _orderReadClient.GetUserIdByOrderIdAsync(delivery.OrderId,ct);

                if (userId is null)
                {
                    _logger.LogWarning("Cannot sent notification. Order not found. OrderId: {OrderId}.", delivery.OrderId);
                    return null;
                }
            

                DeliveryCanceledEvent deliveryCanceledEvent = new() { UserId = userId.Value, OrderId = delivery.OrderId };
                await _publishEndpoint.Publish(deliveryCanceledEvent, ct);
            }
              
            return _mapper.Map<DeliveryDto>(delivery);
        }



    }
}
