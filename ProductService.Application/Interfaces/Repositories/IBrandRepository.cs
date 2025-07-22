using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductService.Domain.Entity;

namespace ProductService.Application.Interfaces.Repositories
{
    public interface IBrandRepository : IBaseRepository<Brand>
    {
        Task<Brand?> FindBrandByIdWithIncludes(Guid brandId);
    }
}
