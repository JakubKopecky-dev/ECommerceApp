using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CartService.Domain.Entity;

namespace CartService.Application.Interfaces.Repositories
{
    public interface ICartItemRepository : IBaseRepository<CartItem>
    {
    }
}
