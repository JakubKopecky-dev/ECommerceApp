using DeliveryService.Domain.Common;
using DeliveryService.Domain.Enums;
using DeliveryService.Domain.Events;
using DeliveryService.Domain.ValueObjects;

namespace DeliveryService.Domain.Entities
{
    public class Delivery : BaseEntity
    {
        public Guid OrderId { get; private set; }

        public Guid CourierId { get; private set; }
        public Courier? Courier { get; private set; }

        public DeliveryStatus Status { get; private set; }

        public DateTime? DeliveredAt { get; private set; }

        public Email Email { get; private set; } = null!;

        public string FirstName { get; private set; } = "";

        public string LastName { get; private set; } = "";

        public string PhoneNumber { get; private set; } = "";

        public string? TrackingNumber { get; private set; }

        public Address Address { get; private set; } = null!;

        private Delivery() { }


    
        public static Delivery Create( Guid orderId, Guid courierId,Email email,string firstName, string lastName, string phone,Address address,string? trackingNumber)
        {

            ValidateOrderId(orderId);
            ValidateCourierId(courierId);

            ValidateName(firstName, nameof(FirstName));
            ValidateName(lastName, nameof(LastName));

            ValidatePhone(phone);

            return new()
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                CourierId = courierId,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phone,
                Address = address,
                Status = DeliveryStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                TrackingNumber = trackingNumber
            };
        }


        public void ChangeStatus(DeliveryStatus newStatus)
        {
            if (!IsStatusChangeValid(Status, newStatus))
                throw new DomainException("Invalid status transition");

            Status = newStatus;
            UpdatedAt = DateTime.UtcNow;

            if (newStatus == DeliveryStatus.Delivered)
            {
                DeliveredAt = DateTime.UtcNow;
                AddDomainEvent(new DeliveryDeliveredDomainEvent { OrderId = OrderId });
            }

            if (newStatus == DeliveryStatus.Canceled)
                AddDomainEvent(new DeliveryCanceledDomainEvent { OrderId = OrderId });
        }


        public void AssignCourier(Guid courierId)
        {
            ValidateCourierId(courierId);

            CourierId = courierId;
            UpdatedAt = DateTime.UtcNow;
        }


        public void ChangeRecipient(string firstName, string lastName)
        {
            ValidateName(firstName, nameof(FirstName));
            ValidateName(lastName, nameof(LastName));

            FirstName = firstName;
            LastName = lastName;
            UpdatedAt = DateTime.UtcNow;
        }


        public void ChangeContact(Email email, string phone)
        {
            ValidatePhone(phone);

            Email = email;
            PhoneNumber = phone;
            UpdatedAt = DateTime.UtcNow;
        }


        public void ChangeAddress(Address address)
        {
            Address = address;
            UpdatedAt = DateTime.UtcNow;
        }


        public void SetTrackingNumber(string? trackingNumber)
        {
            if (!string.IsNullOrWhiteSpace(trackingNumber) && trackingNumber.Length > 40)
                throw new DomainException("TrackingNumber is too long");

            TrackingNumber = trackingNumber;
            UpdatedAt = DateTime.UtcNow;
        }


        public void ChangeOrder(Guid orderId)
        {
            ValidateOrderId(orderId);

            OrderId = orderId;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Update(Guid orderId,Guid courierId,Email email,string firstName,string lastName,string phone,Address address,string? trackingNumber)
        {
            ChangeOrder(orderId);

            AssignCourier(courierId);
            ChangeRecipient(firstName, lastName);
            ChangeContact(email, phone);
            ChangeAddress(address);
            SetTrackingNumber(trackingNumber);
        }




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




        private static void ValidateName(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException($"{fieldName} is required");

            if (value.Length > 20)
                throw new DomainException($"{fieldName} too long");
        }


        private static void ValidatePhone(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("PhoneNumber is required");

            if (value.Length > 20)
                throw new DomainException("PhoneNumber is too long");
        }


        private static void ValidateOrderId(Guid orderId)
        {
            if (orderId == Guid.Empty)
                throw new DomainException("OrderId is required");
        }

        private static void ValidateCourierId(Guid courierId)
        {
            if (courierId == Guid.Empty)
                throw new DomainException("CourierId is required");
        }












    }
}
