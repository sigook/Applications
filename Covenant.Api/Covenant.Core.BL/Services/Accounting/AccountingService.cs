using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Models.Accounting.Subcontractor;
using Covenant.Common.Repositories.Accounting;
using Covenant.Core.BL.Interfaces;
using Covenant.Documents.Services;
using MediatR;

namespace Covenant.Core.BL.Services.Accounting;

public class AccountingService : IAccountingService
{
    private readonly ICurrentUserService currentUserService;
    private readonly IPayStubRepository payStubRepository;
    private readonly ISubcontractorRepository subcontractorRepository;
    private readonly IMediator mediator;

    public AccountingService(
        ICurrentUserService currentUserService,
        IPayStubRepository payStubRepository,
        ISubcontractorRepository subcontractorRepository,
        IMediator mediator)
    {
        this.currentUserService = currentUserService;
        this.payStubRepository = payStubRepository;
        this.subcontractorRepository = subcontractorRepository;
        this.mediator = mediator;
    }

    public async Task<PaginatedList<WeeklyPayrollModel>> GetWeeklyPayrollGroupByPaymentDate(Pagination pagination)
    {
        var agencyIds = currentUserService.GetAgencyIds();
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
        var agencyId = currentUserService.GetAgencyId();
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

    public async Task<Result> DeleteSubcontractorReport(string weekEnding)
    {
        if (!DateTime.TryParse(weekEnding, out DateTime weekEndingDate))
        {
            return Result.Fail($"Invalid date format ({weekEnding})");
        }
        var agencyId = currentUserService.GetAgencyId();
        int deleted = await subcontractorRepository.DeleteReportsByWeekEnding(agencyId, weekEndingDate);
        if (deleted == 0)
        {
            return Result.Fail($"No subcontractor reports found for week ending {weekEndingDate:yyyy-MM-dd}");
        }
        return Result.Ok();
    }
}
