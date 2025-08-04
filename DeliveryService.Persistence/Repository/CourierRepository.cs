using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeliveryService.Application.Interfaces.Repositories;
using DeliveryService.Domain.Entity;

namespace DeliveryService.Persistence.Repository
{
    public class CourierRepository(DeliveryDbContext dbContext) : BaseRepository<Courier>(dbContext), ICourierRepository
    {
    }
}
