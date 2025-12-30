using Covenant.Common.Args;
using Covenant.Common.Entities;
using Covenant.Common.Interfaces;
using Covenant.WarnExpiredLicenses.Models;
using Covenant.WarnExpiredLicenses.Repositories;

namespace Covenant.WarnExpiredLicenses.Services
{
    public class WarnExpiredLicensesService : IWarnExpiredLicensesService
    {
        private readonly ITimeService _timeService;
        private readonly IWarnExpiredLicensesRepository _repository;
        public event Func<object, WorkerLicenseExpiredEventArgs, Task> OnAgencyToNotify;

        public WarnExpiredLicensesService(ITimeService timeService, IWarnExpiredLicensesRepository repository)
        {
            _timeService = timeService;
            _repository = repository;
        }

        public async Task Warn()
        {
            DateTime now = _timeService.GetCurrentDateTime();
            if (now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday) return;
            TrackNotification lastRun = await _repository.GetLastRun(nameof(WarnExpiredLicensesService));
            if (lastRun?.LastTimeRun.Date == now.Date) return;
            List<WorkerLicenseExpiredModel> list = await _repository.Get(now.AddDays(7));
            foreach (IGrouping<string, WorkerLicenseExpiredModel> agency in list.GroupBy(m => m.AgencyEmail))
            {
                OnAgencyToNotify?.Invoke(this, new WorkerLicenseExpiredEventArgs
                {
                    AgencyEmail = agency.Key,
                    Workers = agency.Select(m => new WorkerLicenseExpiredEventArgs.WorkerInformation
                    {
                        NumberId = m.NumberId,
                        WorkerFullName = m.WorkerFullName,
                        WorkerEmail = m.WorkerEmail,
                        MobileNumber = m.MobileNumber,
                        LicenseDescription = m.LicenseDescription,
                        LicenseNumber = m.LicenseNumber,
                        Expires = m.Expires
                    }).ToList()
                });
            }
            await _repository.CreateLastRun(new TrackNotification(now, nameof(WarnExpiredLicensesService)));
            await _repository.SaveChangesAsync();
        }
    }
}