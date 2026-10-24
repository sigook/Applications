using Microsoft.Extensions.Options;
using Sigook.Functions.Configuration;
using Sigook.Functions.Services;
using Sigook.Functions.Tests.Fakes;
using System.Net;

namespace Sigook.Functions.Tests;

public class AccessTokenProviderTest
{
    private const string AccountsUrl = "https://api.sigook.test/connect/token";

    private readonly FakeHttpClientFactory _http = new();

    private AccessTokenProvider Create(string accountsUrl = AccountsUrl, string clientId = "functions", string clientSecret = "secret") =>
        new(_http, Options.Create(new ScheduleTasksOptions { AccountsUrl = accountsUrl, ClientId = clientId, ClientSecret = clientSecret }));

    private void TokenEndpointReturns(string accessToken, int expiresIn) =>
        _http.Respond = (_, _) => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent($$"""{"access_token":"{{accessToken}}","token_type":"Bearer","expires_in":{{expiresIn}}}""", System.Text.Encoding.UTF8, "application/json")
        };

    [Fact]
    public async Task Requests_A_Client_Credentials_Token()
    {
        TokenEndpointReturns("abc", 3600);

        string token = await Create().GetToken();

        Assert.Equal("abc", token);
        RecordedRequest request = Assert.Single(_http.Requests);
        Assert.Equal(HttpClients.Api, request.Client);
        Assert.Equal(AccountsUrl, request.Url);
        Assert.Contains("grant_type=client_credentials", request.Body);
        Assert.Contains("scope=api1", request.Body);
    }

    [Fact]
    public async Task Reuses_The_Token_Until_It_Expires()
    {
        TokenEndpointReturns("abc", 3600);
        AccessTokenProvider sut = Create();

        await sut.GetToken();
        string second = await sut.GetToken();

        Assert.Equal("abc", second);
        Assert.Single(_http.Requests);
    }

    [Fact]
    public async Task Requests_A_New_Token_When_It_Is_Within_The_Expiration_Margin()
    {
        TokenEndpointReturns("short-lived", 10);
        AccessTokenProvider sut = Create();

        await sut.GetToken();
        await sut.GetToken();

        Assert.Equal(2, _http.Requests.Count);
    }

    [Theory]
    [InlineData("", "functions", "secret")]
    [InlineData(AccountsUrl, "", "secret")]
    [InlineData(AccountsUrl, "functions", "")]
    public async Task Missing_Credentials_Throw_Without_Calling_The_Server(string accountsUrl, string clientId, string clientSecret)
    {
        var error = await Assert.ThrowsAsync<InvalidOperationException>(() => Create(accountsUrl, clientId, clientSecret).GetToken());

        Assert.Contains("credentials are not configured", error.Message);
        Assert.Empty(_http.Requests);
    }

    [Fact]
    public async Task Token_Endpoint_Error_Throws_With_The_Error_Code()
    {
        _http.Respond = (_, _) => new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("""{"error":"invalid_client","error_description":"Bad secret"}""", System.Text.Encoding.UTF8, "application/json")
        };

        var error = await Assert.ThrowsAsync<InvalidOperationException>(() => Create().GetToken());

        Assert.Contains("invalid_client", error.Message);
        Assert.Contains("Bad secret", error.Message);
    }
}
