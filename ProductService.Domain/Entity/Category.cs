using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductService.Domain.Common;

namespace ProductService.Domain.Entity
{
    public class Category : BaseEntity
    {
        public ICollection<Product> Products { get; set; } = [];

    }
}
