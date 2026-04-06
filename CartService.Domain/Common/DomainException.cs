using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Domain.Common
{
    public sealed class DomainException(string message) : Exception(message);

}
