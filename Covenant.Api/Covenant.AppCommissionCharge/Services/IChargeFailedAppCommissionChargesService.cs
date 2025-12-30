using Covenant.Common.Args;

namespace Covenant.AppCommissionCharges.Services
{
    public interface IChargeFailedAppCommissionChargesService
    {
        event Func<object, AppCommissionChargeSucceededEventArgs, Task> OnAppCommissionChargeSucceeded;
        event Func<object, AppCommissionChargeFailedEventArgs, Task> OnAppCommissionChargeFailed;
        Task AttemptToChargeFailedAppCommissionCharges();
    }
}