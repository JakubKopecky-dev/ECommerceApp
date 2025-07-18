using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Domain.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = "";

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

    }
}
