using DeliveryService.Application.Interfaces.Repositories;
using DeliveryService.Domain.Entity;

namespace DeliveryService.Persistence.Repository
{
    public class CourierRepository(DeliveryDbContext dbContext) : BaseRepository<Courier>(dbContext), ICourierRepository
    {
    }
}
