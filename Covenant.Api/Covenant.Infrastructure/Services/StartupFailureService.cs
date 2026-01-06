using Covenant.Common.Interfaces;
using Covenant.Common.Models;

namespace Covenant.Infrastructure.Services;

public class StartupFailureService : IStartupFailureService
{
    private StartupFailure _startupFailure;

    public StartupFailure GetStartupFailure() => _startupFailure;

    public void RecordStartupFailure(Exception exception, string message)
    {
        ArgumentNullException.ThrowIfNull(exception);
        
        _startupFailure = new StartupFailure(exception, message, DateTime.UtcNow);
    }

    public void ClearStartupFailure()
    {
        _startupFailure = null;
    }
}