namespace Covenant.Common.Enums;

public enum RequestStatus
{
    Open = 1,        // New order, no workers assigned, accepting applications
    InProgress = 2,  // Workers assigned, still has capacity available
    Filled = 3,      // All positions filled
    Cancelled = 4    // Cancelled (can only be cancelled from Open state)
}
