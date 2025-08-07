namespace Shared.Contracts.Enums;

public enum DeliveryStatus
{
    Pending,      // The delivery is scheduled but not yet started.
    InProgress,   // The delivery is currently in progress.
    Delivered,    // The delivery has been successfully completed.
    Canceled      // The delivery has been canceled.
}
