using Covenant.Infrastructure.Contexts;

namespace Covenant.Integration.Tests.Configuration;

public interface ITestData
{
    void Seed(CovenantContext context);
}
