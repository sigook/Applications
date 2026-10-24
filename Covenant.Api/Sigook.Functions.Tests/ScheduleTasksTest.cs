using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Sigook.Functions.Configuration;
using Sigook.Functions.Functions;
using Sigook.Functions.Services;
using Sigook.Functions.Tests.Fakes;
using System.Net;

namespace Sigook.Functions.Tests;

public class ScheduleTasksTest
{
    private const string ApiUrl = "https://api.sigook.test/api/jobs/scheduletasks/";
    private const string Webhook = "https://teams.test/webhook";

    private readonly FakeHttpClientFactory _http = new();

    private ScheduleTasks Create(string apiUrl = ApiUrl, IAccessTokenProvider? tokens = null, string? webhook = Webhook)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["TeamsWebhook"] = webhook })
            .Build();
        return new ScheduleTasks(
            _http,
            tokens ?? new FakeAccessTokenProvider(),
            NullLogger<ScheduleTasks>.Instance,
            configuration,
            Options.Create(new ScheduleTasksOptions { ApiUrl = apiUrl }));
    }

    private RecordedRequest TeamsMessage => Assert.Single(_http.To(HttpClients.Teams));

    [Theory]
    [InlineData(nameof(ScheduleTasks.NotificationSinExpiration))]
    [InlineData(nameof(ScheduleTasks.WarnLicensesExpiration))]
    [InlineData(nameof(ScheduleTasks.MarkOverdueInvoices))]
    public async Task Each_Timer_Posts_Its_Action_With_A_Bearer_Token(string action)
    {
        ScheduleTasks sut = Create();

        Task run = action switch
        {
            nameof(ScheduleTasks.NotificationSinExpiration) => sut.NotificationSinExpiration(null!),
            nameof(ScheduleTasks.WarnLicensesExpiration) => sut.WarnLicensesExpiration(null!),
            _ => sut.MarkOverdueInvoices(null!)
        };
        await run;

        RecordedRequest api = Assert.Single(_http.To(HttpClients.Api));
        Assert.Equal(HttpMethod.Post, api.Method);
        Assert.Equal($"{ApiUrl}{action}", api.Url);
        Assert.Equal("Bearer test-token", api.Authorization);
        Assert.Equal(Webhook, TeamsMessage.Url);
        Assert.Contains($"{ApiUrl}{action}", TeamsMessage.Body);
        Assert.Contains("\"OK\"", TeamsMessage.Body);
    }

    [Fact]
    public async Task Api_Error_Is_Reported_To_Teams_With_The_Response_Body()
    {
        _http.Respond = (client, _) => client == HttpClients.Api
            ? new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent("database unavailable") }
            : new HttpResponseMessage(HttpStatusCode.OK);

        await Create().MarkOverdueInvoices(null!);

        Assert.Contains("database unavailable", TeamsMessage.Body);
        Assert.DoesNotContain("\"OK\"", TeamsMessage.Body);
    }

    [Fact]
    public async Task Missing_Api_Url_Skips_The_Call_And_Reports_It()
    {
        await Create(apiUrl: string.Empty).MarkOverdueInvoices(null!);

        Assert.Empty(_http.To(HttpClients.Api));
        Assert.Contains("ScheduleTasks:ApiUrl is not set", TeamsMessage.Body);
    }

    [Fact]
    public async Task Token_Failure_Is_Reported_To_Teams()
    {
        await Create(tokens: new FakeAccessTokenProvider(failure: new InvalidOperationException("Token request failed: invalid_client"))).MarkOverdueInvoices(null!);

        Assert.Empty(_http.To(HttpClients.Api));
        Assert.Contains("Token request failed: invalid_client", TeamsMessage.Body);
    }

    [Fact]
    public async Task Missing_Teams_Webhook_Does_Not_Fail_The_Task()
    {
        await Create(webhook: null).MarkOverdueInvoices(null!);

        Assert.Single(_http.To(HttpClients.Api));
        Assert.Empty(_http.To(HttpClients.Teams));
    }

    [Fact]
    public async Task Teams_Failure_Does_Not_Fail_The_Task()
    {
        _http.Respond = (client, _) => client == HttpClients.Teams
            ? new HttpResponseMessage(HttpStatusCode.BadRequest)
            : new HttpResponseMessage(HttpStatusCode.OK);

        await Create().MarkOverdueInvoices(null!);

        Assert.Single(_http.To(HttpClients.Teams));
    }
}
