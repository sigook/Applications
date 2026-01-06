using Covenant.Common.Models;

namespace Covenant.Common.Interfaces;

public interface IStartupFailureService
{
    StartupFailure? GetStartupFailure();
    void RecordStartupFailure(Exception exception, string message);
    void ClearStartupFailure();
}