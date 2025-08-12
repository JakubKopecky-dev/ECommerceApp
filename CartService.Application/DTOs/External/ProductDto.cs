using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Application.DTOs.External
{
    public class ProductDto
    {
        public string Title { get; set; } = "";

        public uint QuantityInStock { get; set; }

        public decimal Price { get; set; }

    }
}
