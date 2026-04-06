using DeliveryService.Domain.Common;
using DeliveryService.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace DeliveryService.Domain.Entities
{
    public class Courier : BaseEntity
    {
        public string Name { get; private set; } = "";

        public string? PhoneNumber { get; private set; }

        public Email? Email { get; private set; }


        private readonly List<Delivery> _deliveries = [];
        public IReadOnlyCollection<Delivery> Deliveries => _deliveries.AsReadOnly();

        private Courier() { }


        public static Courier Create(string name, string? phone, Email? email)
        {
            if (string.IsNullOrEmpty(name))
                throw new DomainException("Name is required");

            return new()
            {
                Id = Guid.NewGuid(),
                Name = name,
                PhoneNumber = phone,
                Email = email,
                CreatedAt = DateTime.UtcNow,
            };
        }



        public void ChangeContact(string name, string? phone, Email? email)
        {
            if (string.IsNullOrEmpty(name))
                throw new DomainException("Name is required");

            Name = name;
            PhoneNumber = phone;
            Email = email;
            UpdatedAt = DateTime.UtcNow;
        }








    }
}
