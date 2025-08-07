namespace ProductService.Application.DTOs.Brand
{
    public  class BrandDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = "";

        public string Description { get; set; } = "";

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
