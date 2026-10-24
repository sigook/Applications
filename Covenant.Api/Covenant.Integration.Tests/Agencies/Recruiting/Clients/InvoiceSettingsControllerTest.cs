using Covenant.Api.Controllers.Agencies.Recruiting.Clients;
using Covenant.Common.Entities.Companies;
using Covenant.Common.Models.Companies.Agencies;
using Covenant.Infrastructure.Contexts;
using Covenant.Integration.Tests.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Xunit;

namespace Covenant.Integration.Tests.Agencies.Recruiting.Clients;

public class InvoiceSettingsControllerTest : BaseTestOrder, IClassFixture<SeededWebApplicationFactory<ClientsTestStartup, ClientsTestData>>
{
    private readonly SeededWebApplicationFactory<ClientsTestStartup, ClientsTestData> _factory;
    private readonly ClientsTestData _data;
    private readonly HttpClient _client;

    public InvoiceSettingsControllerTest(SeededWebApplicationFactory<ClientsTestStartup, ClientsTestData> factory)
    {
        _factory = factory;
        _data = factory.Data;
        _client = _data.AsRecruiter(factory.CreateClient);
    }

    private CovenantContext Context => _factory.Services.GetRequiredService<CovenantContext>();

    private static string Notes(CompanyProfile company) => InvoiceNotesController.RouteName.Replace("{profileId}", company.Id.ToString());

    private static string Recipients(CompanyProfile company) => InvoiceRecipientsController.RouteName.Replace("{profileId}", company.Id.ToString());

    private async Task<Guid> CreateRecipient(CompanyProfile company, string email, string name)
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync(Recipients(company), new CompanyProfileInvoiceRecipientModel { Email = email, Name = name });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<CompanyProfileInvoiceRecipientModel>()).Id!.Value;
    }

    [Fact]
    public async Task InvoiceNotes_AreEmptyUntilSaved_ThenCreatedAndUpdated()
    {
        var empty = await _client.GetFromJsonAsync<CompanyProfileInvoiceNotesModel>(Notes(_data.Company));
        Assert.Null(empty.HtmlNotes);

        (await _client.PutAsJsonAsync(Notes(_data.Company), new CompanyProfileInvoiceNotesModel { HtmlNotes = "<p>PO required</p>" })).EnsureSuccessStatusCode();
        (await _client.PutAsJsonAsync(Notes(_data.Company), new CompanyProfileInvoiceNotesModel { HtmlNotes = "<p>PO 123</p>" })).EnsureSuccessStatusCode();

        var saved = await _client.GetFromJsonAsync<CompanyProfileInvoiceNotesModel>(Notes(_data.Company));
        Assert.Equal("<p>PO 123</p>", saved.HtmlNotes);
        Assert.Equal(1, await Context.CompanyProfileInvoiceNotes.CountAsync(n => n.CompanyProfileId == _data.Company.Id));
    }

    [Fact]
    public async Task InvoiceRecipients_CanBeCreatedListedUpdatedAndDeleted()
    {
        Guid id = await CreateRecipient(_data.OtherCompany, "ap@client.com", "Accounts Payable");
        await CreateRecipient(_data.Company, "elsewhere@client.com", "Other client");

        var list = await _client.GetFromJsonAsync<List<CompanyProfileInvoiceRecipientModel>>(Recipients(_data.OtherCompany));
        CompanyProfileInvoiceRecipientModel recipient = Assert.Single(list);
        Assert.Equal(id, recipient.Id);
        Assert.Equal("ap@client.com", recipient.Email);

        (await _client.PutAsJsonAsync($"{Recipients(_data.OtherCompany)}/{id}", new CompanyProfileInvoiceRecipientModel { Email = "billing@client.com", Name = "Billing" })).EnsureSuccessStatusCode();
        var updated = await Context.CompanyProfileInvoiceRecipients.AsNoTracking().SingleAsync(r => r.Id == id);
        Assert.Equal("billing@client.com", updated.Email);
        Assert.Equal("Billing", updated.Name);

        (await _client.DeleteAsync($"{Recipients(_data.OtherCompany)}/{id}")).EnsureSuccessStatusCode();
        Assert.Empty(await _client.GetFromJsonAsync<List<CompanyProfileInvoiceRecipientModel>>(Recipients(_data.OtherCompany)));
    }

    [Fact]
    public async Task InvoiceRecipients_DeleteUnknown_ReturnsBadRequest()
    {
        HttpResponseMessage response = await _client.DeleteAsync($"{Recipients(_data.Company)}/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
