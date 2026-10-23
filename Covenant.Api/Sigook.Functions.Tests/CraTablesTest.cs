using Covenant.Common.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Sigook.Functions.Configuration;
using Sigook.Functions.Functions;
using Sigook.Functions.Tests.Fakes;
using System.Net;
using System.Text.Json;

namespace Sigook.Functions.Tests;

public class CraTablesTest
{
    private const string CppUrl = "https://api.sigook.test/api/jobs/deductions/cpp";
    private const string TaxUrl = "https://api.sigook.test/api/jobs/deductions/tax";

    private readonly FakeHttpClientFactory _http = new();

    private CraTables Create(string cppUrl = CppUrl, string taxUrl = TaxUrl)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["TeamsWebhook"] = "https://teams.test/webhook" })
            .Build();
        return new CraTables(
            _http,
            new FakeAccessTokenProvider(),
            NullLogger<CraTables>.Instance,
            configuration,
            Options.Create(new CraTablesOptions { CppApiUrl = cppUrl, TaxApiUrl = taxUrl }));
    }

    private RecordedRequest TeamsMessage => Assert.Single(_http.To(HttpClients.Teams));

    private void ApiReturns(HttpStatusCode status, string content) =>
        _http.Respond = (client, _) => client == HttpClients.Api
            ? new HttpResponseMessage(status) { Content = new StringContent(content) }
            : new HttpResponseMessage(HttpStatusCode.OK);

    [Fact]
    public async Task Cpp_Table_Is_Imported_Through_The_Cpp_Endpoint()
    {
        ApiReturns(HttpStatusCode.OK, "1520");

        await Create().CraTableUploaded(null!, "CPP WEEKLY 2027.pdf");

        RecordedRequest api = Assert.Single(_http.To(HttpClients.Api));
        Assert.Equal(CppUrl, api.Url);
        Assert.Equal("Bearer test-token", api.Authorization);
        using JsonDocument body = JsonDocument.Parse(api.Body);
        Assert.Equal("CPP WEEKLY 2027.pdf", body.RootElement.GetProperty("blobName").GetString());
        Assert.Equal(2027, body.RootElement.GetProperty("year").GetInt32());
        Assert.Equal(nameof(PayPeriod.Weekly), body.RootElement.GetProperty("payPeriod").GetString());
        Assert.Contains("CRA table imported: CPP WEEKLY 2027.pdf", TeamsMessage.Body);
        Assert.Contains("1520 Weekly CPP brackets were stored for 2027", TeamsMessage.Body);
    }

    [Fact]
    public async Task Tax_Table_Is_Imported_Through_The_Tax_Endpoint()
    {
        ApiReturns(HttpStatusCode.OK, "880");

        await Create().CraTableUploaded(null!, "TAX MONTHLY 2027.pdf");

        Assert.Equal(TaxUrl, Assert.Single(_http.To(HttpClients.Api)).Url);
        Assert.Contains("880 Monthly income tax brackets were stored for 2027", TeamsMessage.Body);
    }

    [Fact]
    public async Task Unrecognized_Blob_Name_Is_Ignored_And_Reported()
    {
        await Create().CraTableUploaded(null!, "payroll notes.pdf");

        Assert.Empty(_http.To(HttpClients.Api));
        Assert.Contains("CRA table ignored: payroll notes.pdf", TeamsMessage.Body);
    }

    [Fact]
    public async Task Missing_Endpoint_Setting_Skips_The_Import_And_Reports_It()
    {
        await Create(cppUrl: string.Empty).CraTableUploaded(null!, "CPP WEEKLY 2027.pdf");

        Assert.Empty(_http.To(HttpClients.Api));
        Assert.Contains("CraTables:CppApiUrl is not set", TeamsMessage.Body);
    }

    [Fact]
    public async Task Api_Error_Is_Reported_To_Teams()
    {
        ApiReturns(HttpStatusCode.BadRequest, "The PDF does not contain a CPP table");

        await Create().CraTableUploaded(null!, "CPP WEEKLY 2027.pdf");

        Assert.Contains("CRA table failed: CPP WEEKLY 2027.pdf", TeamsMessage.Body);
        Assert.Contains("The PDF does not contain a CPP table", TeamsMessage.Body);
    }
}
