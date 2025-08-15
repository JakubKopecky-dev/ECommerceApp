using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Application.Common
{
    public enum CartItemError
    {
        ProductNotFound,
        OutOfStock,
        InvalidQuantity
    }
}
