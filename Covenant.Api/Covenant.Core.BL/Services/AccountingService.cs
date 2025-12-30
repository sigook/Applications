using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Accounting.Invoice;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Models.Accounting.Subcontractor;
using Covenant.Common.Repositories.Accounting;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Covenant.Core.BL.Services.Invoices;
using Covenant.Documents.Services;
using Covenant.PayStubs.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Covenant.Core.BL.Services;

public class AccountingService : IAccountingService
{
    private readonly IAgencyRepository agencyRepository;
    private readonly IInvoiceRepository invoiceRepository;
    private readonly IIdentityServerService identityServerService;
    private readonly IPayStubRepository payStubRepository;
    private readonly IPayStubsContainer payStubsContainer;
    private readonly ISubcontractorRepository subcontractorRepository;
    private readonly ITimeSheetRepository timeSheetRepository;
    private readonly ISkipPayrollNumberRepository skipPayrollNumberRepository;
    private readonly CreatePayStubUsingTimeSheet createPayStubUsingTimeSheet;
    private readonly IMediator mediator;
    private readonly IServiceProvider serviceProvider;

    public AccountingService(
        IAgencyRepository agencyRepository,
        IInvoiceRepository invoiceRepository,
        IIdentityServerService identityServerService,
        IPayStubRepository payStubRepository,
        IPayStubsContainer payStubsContainer,
        ISubcontractorRepository subcontractorRepository,
        ITimeSheetRepository timeSheetRepository,
        ISkipPayrollNumberRepository skipPayrollNumberRepository,
        CreatePayStubUsingTimeSheet createPayStubUsingTimeSheet,
        IMediator mediator,
        IServiceProvider serviceProvider)
    {
        this.agencyRepository = agencyRepository;
        this.invoiceRepository = invoiceRepository;
        this.identityServerService = identityServerService;
        this.payStubRepository = payStubRepository;
        this.payStubsContainer = payStubsContainer;
        this.subcontractorRepository = subcontractorRepository;
        this.timeSheetRepository = timeSheetRepository;
        this.skipPayrollNumberRepository = skipPayrollNumberRepository;
        this.createPayStubUsingTimeSheet = createPayStubUsingTimeSheet;
        this.mediator = mediator;
        this.serviceProvider = serviceProvider;
    }

    public async Task DeletePayStub(Guid payStubId)
    {
        await payStubRepository.Delete(new Guid[] { payStubId });
        await payStubsContainer.DeleteFileIfExists(payStubId.ToPayStubBlobName());
        await payStubRepository.SaveChangesAsync();
    }

    public async Task<Result> GeneratePayStubs(IEnumerable<Guid> workerIds)
    {
        var result = Result.Ok();
        var agencyIds = identityServerService.GetAgencyIds();
        foreach (var workerId in workerIds)
        {
            var timeSheet = await timeSheetRepository.GetTimeSheetForCreatingPayStubs(agencyIds, workerId);
            result = await createPayStubUsingTimeSheet.Create(timeSheet);
            if (!result)
            {
                break;
            }
        }
        return result;
    }

    public async Task<InvoiceListModelWithTotals> GetInvoices(GetInvoicesFilterV2 filter)
    {
        var agencyId = identityServerService.GetAgencyId();
        var agency = await agencyRepository.GetAgency(agencyId);
        var agencyIds = identityServerService.GetAgencyIds();
        var result = default(InvoiceListModelWithTotals);
        if (!agency.Locations.Any(l => l.Location.IsUSA))
        {
            result = await invoiceRepository.GetInvoicesForAgency(agencyIds, filter);
        }
        else
        {
            result = await invoiceRepository.GetInvoicesUSAForAgency(agencyIds, filter);
        }
        return result;
    }

    public async Task<ResultGenerateDocument<byte[]>> GetInvoicesFile(GetInvoicesFilterV2 filter)
    {
        var agencyId = identityServerService.GetAgencyId();
        var agency = await agencyRepository.GetAgency(agencyId);
        var agencyIds = identityServerService.GetAgencyIds();
        var result = default(List<InvoiceListModel>);
        if (!agency.Locations.Any(l => l.Location.IsUSA))
        {
            result = await invoiceRepository.GetAllInvoicesForAgency(agencyIds, filter);
        }
        else
        {
            result = await invoiceRepository.GetAllInvoicesUSAForAgency(agencyIds, filter);
        }
        var file = await mediator.Send(new GenerateInvoicesReport(result));
        return file;
    }

    public async Task<PaginatedList<PayStubListModel>> GetPayStubs(GetPayStubsFilter filter)
    {
        var agencyIds = identityServerService.GetAgencyIds();
        var result = await payStubRepository.GetPayStubs(agencyIds, filter);
        return result;
    }

    public async Task<ResultGenerateDocument<byte[]>> GetPayStubsFile(GetPayStubsFilter filter)
    {
        var agencyIds = identityServerService.GetAgencyIds();
        var payStubs = await payStubRepository.GetAllPayStubs(agencyIds, filter);
        return await mediator.Send(new GeneratePayStubsReport(payStubs));
    }

    public async Task<PaginatedList<WeeklyPayrollModel>> GetWeeklyPayrollGroupByPaymentDate(Pagination pagination)
    {
        var agencyIds = identityServerService.GetAgencyIds();
        var result = await payStubRepository.GetWeeklyPayrollGroupByPaymentDate(agencyIds, pagination);
        return result;
    }

    public async Task<Result<ResultGenerateDocument<byte[]>>> GetWeeklyPayrollGroupByPaymentDateFile(string weekEnding)
    {
        if (!DateTime.TryParse(weekEnding, out DateTime weekEndingDate))
        {
            return Result.Fail<ResultGenerateDocument<byte[]>>($"Invalid date format ({weekEnding})");
        }
        var data = await payStubRepository.GetWeeklyPayrollDetailByPaymentDate(weekEndingDate);
        return Result.Ok(await mediator.Send(new GeneratePaymentReport(data)));
    }

    public async Task<PaginatedList<PayrollSubContractorListModel>> GetSubcontractors(Pagination filter)
    {
        var agencyId = identityServerService.GetAgencyId();
        var result = await subcontractorRepository.GetPayrollsSubcontractor(agencyId, filter);
        return result;
    }

    public async Task<Result<ResultGenerateDocument<byte[]>>> GetSubcontractorFile(string weekEnding)
    {
        if (!DateTime.TryParse(weekEnding, out DateTime weekEndingDate))
        {
            return Result.Fail<ResultGenerateDocument<byte[]>>($"Invalid date format ({weekEnding})");
        }
        var data = await subcontractorRepository.GetReportsSubcontractorSummary(weekEndingDate);
        return Result.Ok(await mediator.Send(new GenerateSubcontractorReport(data)));
    }

    public async Task<Result> CreateSkipPayrollNumber(BaseModel<Guid> model)
    {
        if (model is null || string.IsNullOrWhiteSpace(model.Value))
        {
            return Result.Fail("Invalid model or value");
        }
        var result = await skipPayrollNumberRepository.Create(model);
        return result;
    }

    public async Task<Result<InvoicePreviewModel>> PreviewInvoice(CreateInvoiceModel model)
    {
        var agencyId = identityServerService.GetAgencyId();
        var invoiceService = await GetInvoiceService(agencyId);
        var agencyIds = identityServerService.GetAgencyIds();
        return await invoiceService.PreviewAsync(agencyIds, model);
    }

    public async Task<Result<Guid>> CreateInvoice(CreateInvoiceModel model)
    {
        var agencyId = identityServerService.GetAgencyId();
        var invoiceService = await GetInvoiceService(agencyId);
        var agencyIds = identityServerService.GetAgencyIds();
        return await invoiceService.CreateAsync(agencyIds, model);
    }

    private async Task<BaseInvoiceService> GetInvoiceService(Guid agencyId)
    {
        var billingLocation = await agencyRepository.GetBillingLocation(agencyId);

        return billingLocation?.IsUSA == true
            ? serviceProvider.GetRequiredService<UsaInvoiceService>()
            : serviceProvider.GetRequiredService<CanadaInvoiceService>();
    }
}
