using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductService.Domain.Common;

namespace ProductService.Domain.Entity
{
    public class ProductReview : BaseEntity
    {
        public Guid ProductId { get; set; }
        public required Product Product { get; set; }

        public Guid UserId { get; set; }

        public string UserName { get; set; } = "";

        public uint Rating { get; set; }

        public string Comment { get; set; } = "";
    }
}
