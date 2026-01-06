namespace Covenant.Common.Enums
{
    public enum WorkerRequestStatus
    {
        //None,
        [Obsolete] Applied,//Applied
        [Obsolete] Declined,//Declined
        Rejected,//Rejected
        Booked,//Booked
        [Obsolete] InQueue
    }
}