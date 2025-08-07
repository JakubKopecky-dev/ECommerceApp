using System.ComponentModel.DataAnnotations;

namespace OrderService.Application.DTOs.Order
{
    public class UpdateOrderNoteDto
    {

        [MaxLength(1000)]
        public string? Note { get; set; }


    }
}
