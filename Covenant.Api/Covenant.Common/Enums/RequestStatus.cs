namespace Covenant.Common.Enums;

public enum RequestStatus
{
    Open = 1,        // Active order (with or without workers), still has capacity
    // InProgress = 2,  // REMOVED - no longer used
    Filled = 3,      // All positions filled
    Cancelled = 4    // Cancelled (can only cancel Open orders without workers)
}
