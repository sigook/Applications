namespace Covenant.Common.Entities.Request
{
    public class TimeSheetPhoto
    {
        private TimeSheetPhoto()
        {
        }

        public TimeSheet TimeSheet { get; private set; }
        public Guid TimeSheetId { get; private set; }
        public CovenantFile Photo { get; private set; }
        public Guid PhotoId { get; private set; }
        public DateTime CreatedAt { get; internal set; } = DateTime.Now;

        public static TimeSheetPhoto Create(Guid timeSheetId, CovenantFile photo)
        {
            return new TimeSheetPhoto
            {
                TimeSheetId = timeSheetId,
                Photo = photo ?? throw new ArgumentNullException(nameof(photo)),
                PhotoId = photo.Id
            };
        }
    }
}