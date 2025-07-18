using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Domain.Common
{
    public class AuditEventLog
    {
        public int Id { get; set; }

        public string EntityName { get; set; } = "";

        public DateTime InsertedDate { get; set; }

        public string EventType { get; set; } = "";

        public string Data { get; set; } = "";
    }
}
