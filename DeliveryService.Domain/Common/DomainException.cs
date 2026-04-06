using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryService.Domain.Common
{
    public sealed class DomainException(string message) : Exception(message);

}
