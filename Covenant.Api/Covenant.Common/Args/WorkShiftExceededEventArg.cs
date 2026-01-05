using System;

namespace Covenant.Common.Args
{
    public class WorkShiftExceededEventArg : EventArgs
    {
        public DateTime ClockOut { get; set; }
        public Guid WorkerRequestId { get; set; }
    }
}