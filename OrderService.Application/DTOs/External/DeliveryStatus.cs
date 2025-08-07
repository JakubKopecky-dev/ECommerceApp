using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.DTOs.External
{
    public enum DeliveryStatus
    {
        Pending,      // The delivery is scheduled but not yet started.
        InProgress,   // The delivery is currently in progress.
        Delivered,    // The delivery has been successfully completed.
        Canceled      // The delivery has been canceled.
    }
}
