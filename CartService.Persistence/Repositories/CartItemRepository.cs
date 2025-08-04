using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CartService.Application.Interfaces.Repositories;
using CartService.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace CartService.Persistence.Repositories
{
    public class CartItemRepository(CartDbContext dbContext) : BaseRepository<CartItem>(dbContext), ICartItemRepository
    {
    }
}
