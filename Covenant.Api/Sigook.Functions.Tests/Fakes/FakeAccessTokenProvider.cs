using Sigook.Functions.Services;

namespace Sigook.Functions.Tests.Fakes;

public sealed class FakeAccessTokenProvider(string token = "test-token", Exception? failure = null) : IAccessTokenProvider
{
    public Task<string> GetToken() => failure is null ? Task.FromResult(token) : Task.FromException<string>(failure);
}
