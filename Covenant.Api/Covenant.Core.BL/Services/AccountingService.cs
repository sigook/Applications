using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Models.Accounting.Subcontractor;
using Covenant.Common.Repositories.Accounting;
using Covenant.Core.BL.Interfaces;
using Covenant.Documents.Services;
using MediatR;

namespace Covenant.Core.BL.Services;

public class AccountingService : IAccountingService
{
    private readonly IIdentityServerService identityServerService;
    private readonly IPayStubRepository payStubRepository;
    private readonly ISubcontractorRepository subcontractorRepository;
    private readonly IMediator mediator;

    public AccountingService(
        IIdentityServerService identityServerService,
        IPayStubRepository payStubRepository,
        ISubcontractorRepository subcontractorRepository,
        IMediator mediator)
    {
        this.identityServerService = identityServerService;
        this.payStubRepository = payStubRepository;
        this.subcontractorRepository = subcontractorRepository;
        this.mediator = mediator;
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
}
