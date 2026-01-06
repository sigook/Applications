using Covenant.Common.Args;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Payment;
using Covenant.Common.Repositories.AppCommission;

namespace Covenant.AppCommissionCharges.Services.Impl
{
    public class ChargeFailedAppCommissionChargesService : IChargeFailedAppCommissionChargesService
    {
        private readonly IAppCommissionRepository _appCommissionRepository;
        private readonly IBamboraService _chargeService;
        private readonly ITimeService _timeService;
        public event Func<object, AppCommissionChargeSucceededEventArgs, Task> OnAppCommissionChargeSucceeded;
        public event Func<object, AppCommissionChargeFailedEventArgs, Task> OnAppCommissionChargeFailed;

        public ChargeFailedAppCommissionChargesService(IAppCommissionRepository appCommissionRepository, IBamboraService chargeService, ITimeService timeService)
        {
            _appCommissionRepository = appCommissionRepository;
            _chargeService = chargeService;
            _timeService = timeService;
        }
        public async Task AttemptToChargeFailedAppCommissionCharges()
        {
            DateTime now = _timeService.GetCurrentDateTime();
            const int maxAttempts = 5;
            var charges = await _appCommissionRepository.GetFailedAppCommissionCharges(now.Subtract(TimeSpan.FromHours(24)), maxAttempts);
            foreach (var chargeModel in charges)
            {
                var charge = await _appCommissionRepository.GetAppCommissionChargeById(chargeModel.Id);
                decimal amount = charge.Amount;
                var result = await _chargeService.CreateCharge(chargeModel.AgencyId, new ChargeOptions(amount, "Charge for App Commission"));
                if (result) charge.ChangeChargeToSucceeded(result.Value.TransactionId, now);
                else charge.AddFailedAttempt(now, result.Errors.First().Message);

                await _appCommissionRepository.UpdateAppCommissionCharge(charge);
                await _appCommissionRepository.SaveChangesAsync();

                if (result) OnAppCommissionChargeSucceeded?.Invoke(this, new AppCommissionChargeSucceededEventArgs(chargeModel.AppCommissionNumberId, amount, chargeModel.AgencyId, chargeModel.AgencyEmail));
                else OnAppCommissionChargeFailed?.Invoke(this, new AppCommissionChargeFailedEventArgs(chargeModel.AppCommissionNumberId, amount, chargeModel.AgencyId, chargeModel.AgencyEmail));
            }
        }
    }
}