using DeliveryService.Application.DTOs.Courier;
using DeliveryService.Application.DTOs.Delivery;
using DeliveryService.Domain.Entities;
using DeliveryService.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryService.Application
{
    public static class Mapper
    {

        public static CourierDto CourierToCourierDto(this Courier courier) =>
            new()
            {
                Id = courier.Id,
                Name = courier.Name,
                PhoneNumber = courier.PhoneNumber,
                Email = courier.Email?.ToString(),
                CreatedAt = courier.CreatedAt,
                UpdatedAt = courier.UpdatedAt
            };



        public static DeliveryExtendedDto DeliveryToDeliveryExtendedDto(this Delivery delivery) =>
            new()
            {
                Id = delivery.Id,
                OrderId = delivery.OrderId,
                Courier = delivery.Courier?.CourierToCourierDto() ?? throw new InvalidOperationException("Courier not loaded"),
                CreatedAt = delivery.CreatedAt,
                DeliveredAt = delivery.DeliveredAt,
                UpdatedAt = delivery.UpdatedAt,
                City = delivery.Address.City,
                PostalCode = delivery.Address.PostalCode,
                State = delivery.Address.State,
                Status = delivery.Status,
                Street = delivery.Address.Street,
                TrackingNumber = delivery.TrackingNumber,
                Email = delivery.Email.Value
            };



        public static DeliveryDto DeliveryToDeliveryDto(this Delivery delivery) =>
            new()
            {
                Id = delivery.Id,
                OrderId = delivery.OrderId,
                CourierId = delivery.CourierId,
                Status = delivery.Status,
                DeliveredAt = delivery.DeliveredAt,
                Street = delivery.Address.Street,
                City = delivery.Address.City,
                PostalCode = delivery.Address.PostalCode,
                State = delivery.Address.State,
                CreatedAt = delivery.CreatedAt,
                UpdatedAt = delivery.UpdatedAt,
                Email = delivery.Email.Value,
                FirstName = delivery.FirstName,
                LastName = delivery.LastName,
                PhoneNumber = delivery.PhoneNumber,
                TrackingNumber = delivery.TrackingNumber
            };






    }
}
