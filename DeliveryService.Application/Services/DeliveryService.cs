using DeliveryService.Application.DTOs.Delivery;
using DeliveryService.Application.DTOs.External;
using DeliveryService.Application.Interfaces.External;
using DeliveryService.Application.Interfaces.Repositories;
using DeliveryService.Application.Interfaces.Services;
using DeliveryService.Domain.Entities;
using DeliveryService.Domain.Enums;
using DeliveryService.Domain.Events;
using DeliveryService.Domain.ValueObjects;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Events;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace DeliveryService.Application.Services
{
    public class DeliveryService(IDeliveryRepository deliveryRepository, ILogger<DeliveryService> logger, IPublishEndpoint publishEndpoint, IOrderReadClient orderReadClient) : IDeliveryService
    {
        private readonly IDeliveryRepository _deliveryRepository = deliveryRepository;
        private readonly ILogger<DeliveryService> _logger = logger;
        private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
        private readonly IOrderReadClient _orderReadClient = orderReadClient;


        public async Task<DeliveryExtendedDto?> GetDeliveryByOrderIdAsync(Guid orderId, CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving delivry by orderId. OrderId: {OrderId}.", orderId);

            Delivery? delivery = await _deliveryRepository.FindDeliveryByOrderIdIncludeCourierAsync(orderId, ct);
            if (delivery is null)
                _logger.LogWarning("Delivery not found. OrderId: {OrderId}.", orderId);
            else
                _logger.LogInformation("Delivery found. DeliveryId: {DeliveryId}, OrderId: {OrderId}.", delivery.Id, orderId);

            return delivery?.DeliveryToDeliveryExtendedDto();
        }



        public async Task<DeliveryDto> CreateDeliveryAsync(CreateUpdateDeliveryDto createDto, CancellationToken ct = default)
        {
            _logger.LogInformation("Creating new delivery. OrderId: {OrderId}.", createDto.OrderId);

            Address address = new(createDto.Street, createDto.City, createDto.PostalCode, createDto.State);

            Delivery delivery = Delivery.Create(createDto.OrderId, createDto.CourierId, new Email(createDto.Email), createDto.FirstName,
                createDto.LastName, createDto.PhoneNumber, address, createDto.TrackingNumber);

            await _deliveryRepository.AddAsync(delivery, ct);
            await _deliveryRepository.SaveChangesAsync(ct);

            _logger.LogInformation("Delivery created. DelivryId: {DeliveryId}, OrderId: {OrderId}.", delivery.Id, delivery.OrderId);

            return delivery.DeliveryToDeliveryDto();
        }



        public async Task<DeliveryDto?> UpdateDeliveryAsync(Guid deliveryId, CreateUpdateDeliveryDto updateDto, CancellationToken ct = default)
        {
            _logger.LogInformation("Updating delivry. DeliveryId: {DeliveryId}.", deliveryId);

            Delivery? delivery = await _deliveryRepository.FindByIdAsync(deliveryId, ct);
            if (delivery is null)
            {
                _logger.LogWarning("Cannot update. Delivery not found. DeliveryId: {DeliveryId}.", deliveryId);
                return null;
            }

            Address address = new(updateDto.Street, updateDto.City, updateDto.PostalCode, updateDto.State);
            delivery.Update(updateDto.OrderId, updateDto.CourierId, new Email(updateDto.Email), updateDto.FirstName, updateDto.LastName,
                updateDto.PhoneNumber, address, updateDto.TrackingNumber);


            await _deliveryRepository.SaveChangesAsync(ct);
            _logger.LogInformation("Delivery updated. DeliveryId: {DeliveryId}.", deliveryId);

            return delivery.DeliveryToDeliveryDto();
        }



        public async Task<bool> DeleteDeliveryAsync(Guid deliveryId, CancellationToken ct = default)
        {
            _logger.LogInformation("Deleting delivery. DeliveryId: {DeliveryId}.", deliveryId);

            Delivery? delivery = await _deliveryRepository.FindByIdAsync(deliveryId, ct);
            if (delivery is null)
            {
                _logger.LogWarning("Cannot delete. Delivery not found. DeliveryId: {DeliveryId}.", deliveryId);
                return false;
            }

            _deliveryRepository.Remove(delivery);
            await _deliveryRepository.SaveChangesAsync(ct);
            _logger.LogInformation("Delivery deleted. DeliveryId: {DeliveryId}.", deliveryId);

            return true;
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


            delivery.ChangeStatus(changeDto.Status);

            await _deliveryRepository.SaveChangesAsync(ct);
            _logger.LogInformation("DeliveryStatus changed. DeliveryId: {DeliveryId}.", deliveryId);

            foreach (var domainEvent in delivery.PopDomainEvents())
            {
                if (domainEvent is DeliveryDeliveredDomainEvent delivered)
                {
                    // changing status on order to completed
                    await _publishEndpoint.Publish(new DeliveryDeliveredEvent { OrderId = delivered.OrderId }, ct);
                }

                if (domainEvent is DeliveryCanceledDomainEvent canceled)
                {
                    Guid? userId = await _orderReadClient.GetUserIdByOrderIdAsync(canceled.OrderId, ct);
                    if (userId is null)
                    {
                        _logger.LogWarning("Cannot send notification. Order not found. OrderId: {OrderId}.", canceled.OrderId);
                        return null;
                    }

                    // notification user delivery canceled
                    await _publishEndpoint.Publish(new DeliveryCanceledEvent { UserId = userId.Value, OrderId = canceled.OrderId }, ct);
                }
            }

            return delivery.DeliveryToDeliveryDto();
        }



    }
}
