namespace DeliveryService.Application.DTOs.Courier
{
    public class CreateUpdateCourierDto
    {
        public string Name { get; set; } = "";

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }
    }
}
