using Covenant.Common.Args;
using Covenant.Common.Entities;
using Covenant.Common.Interfaces;
using Covenant.ReportWorkersSINExpired.Models;
using Covenant.ReportWorkersSINExpired.Repositories;

namespace Covenant.ReportWorkersSINExpired.Services
{
    public class ReportWorkersSINExpiredService : IReportWorkersSINExpiredService
    {
        private readonly ITimeService _timeService;
        private readonly IWorkerSINExpiredRepository _repository;
        public event Func<object, WorkerSINExpiredEventArgs, Task> OnAgencyToNotify;

        public ReportWorkersSINExpiredService(ITimeService timeService, IWorkerSINExpiredRepository repository)
        {
            _timeService = timeService;
            _repository = repository;
        }
        public async Task Report()
        {
            DateTime now = _timeService.GetCurrentDateTime();
            if (now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday) return;
            TrackNotification lastRun = await _repository.GetLastRun(nameof(ReportWorkersSINExpiredService));
            if (lastRun?.LastTimeRun.Date == now.Date) return;
            List<WorkerSINExpiredModel> list = await _repository.Get(now);
            foreach (IGrouping<string, WorkerSINExpiredModel> agency in list.GroupBy(m => m.AgencyEmail))
            {
                OnAgencyToNotify?.Invoke(this, new WorkerSINExpiredEventArgs
                {
                    AgencyEmail = agency.Key,
                    Workers = agency.Select(m => new WorkerSINExpiredEventArgs.WorkerSINExpired
                    {
                        Phone = m.Phone,
                        DueDate = m.DueDate,
                        MobileNumber = m.MobileNumber,
                        PhoneExt = m.PhoneExt,
                        SocialInsurance = m.SocialInsurance,
                        WorkerEmail = m.WorkerEmail,
                        WorkerFullName = m.WorkerFullName
                    }).ToList()
                });
            }
            await _repository.CreateLastRun(new TrackNotification(now, nameof(ReportWorkersSINExpiredService)));
            await _repository.SaveChangesAsync();
        }
    }
}