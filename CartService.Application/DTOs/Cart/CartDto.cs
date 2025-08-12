namespace CartService.Application.DTOs.Cart
{
    public class CartDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public decimal TotalPrice { get; set; }

        public string? Note { get; set; }
    }
    
}
