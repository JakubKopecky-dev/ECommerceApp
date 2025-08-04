using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CartService.Application.DTOs.CartItem;

namespace CartService.Application.DTOs.Cart
{
    public class CartDto
    {
        public Guid UserId { get; set; }

        public decimal TotalPrice { get; set; }

        public string? Note { get; set; }
    }
    
}
