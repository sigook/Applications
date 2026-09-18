using System.Globalization;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using Covenant.Common.Models.Company.SalesDashboard;
using Covenant.Common.Models.Request;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using FluentValidation;

namespace Covenant.Core.BL.Services;

public class SalesService(
    IRequestService requestService,
    IRequestRepository requestRepository,
    ICompanyRepository companyRepository,
    IIdentityServerService identityServerService,
    IUploadedFilesService uploadedFilesService,
    IDocumentService documentService,
    IValidator<CreateCompanyInteractionModel> createInteractionValidator,
    IValidator<UpdateCompanyInteractionModel> updateInteractionValidator,
    IValidator<CreateDealModel> createDealValidator,
    IValidator<UpdateDealModel> updateDealValidator,
    ITimeService timeService,
    IValidator<GetDealsByStatusFilter> dealsByStatusValidator) : ISalesService
{
    private Guid? SalesScope => identityServerService.IsSales() ? identityServerService.GetAgencyPersonnelId() : null;

    private Guid? OwnerScope => identityServerService.IsAdmin() ? null : identityServerService.GetUserId();

    public async Task<AgencyRequestsPagedResponse> GetRequests(GetRequestForAgencyFilter filter)
    {
        Guid agencyId = filter.AgencyId ?? identityServerService.GetAgencyId();
        ApplyScope(filter);
        return await requestService.GetRequestsForAgency(agencyId, filter);
    }

    public IEnumerable<AgencyRequestListModel> GetRequestsForReport(GetRequestForAgencyFilter filter)
    {
        ApplyScope(filter);
        return requestRepository.GetAllRequestsForAgency(identityServerService.GetAgencyId(), filter);
    }

    public async Task<PaginatedList<CompanyProfileListModel>> GetCompanies(GetCompanyForAgencyFilter filter)
    {
        filter.SalesPersonnelId = SalesScope;
        return await companyRepository.GetCompaniesProfileForAgency(identityServerService.GetAgencyId(), filter);
    }

    public IEnumerable<CompanyProfileListModel> GetCompaniesForReport(GetCompanyForAgencyFilter filter)
    {
        filter.SalesPersonnelId = SalesScope;
        return companyRepository.GetAllCompaniesProfileForAgency(identityServerService.GetAgencyId(), filter);
    }

    public async Task<PaginatedList<CompanyInteractionListModel>> GetInteractions(GetCompanyInteractionsFilter filter)
    {
        var agencyId = identityServerService.GetAgencyId();
        filter.OwnerId = OwnerScope ?? filter.OwnerId;
        return await companyRepository.GetInteractions(agencyId, filter);
    }

    public async Task<Result<Guid>> CreateInteraction(CreateCompanyInteractionModel model)
    {
        var validationResult = await createInteractionValidator.ValidateAsync(model);
        if (!validationResult.IsValid) return validationResult.ToResultFailure<Guid>();
        var userId = identityServerService.GetUserId();
        var interaction = new CompanyInteraction(model.Description, userId, model.CompanyProfileId,
            model.InteractionPurpose, model.InteractionType, model.InteractionStatus);
        await companyRepository.Create(interaction);
        await companyRepository.SaveChangesAsync();
        return Result.Ok(interaction.Id);
    }

    public async Task<Result> UpdateInteraction(Guid id, UpdateCompanyInteractionModel model)
    {
        var validationResult = await updateInteractionValidator.ValidateAsync(model);
        if (!validationResult.IsValid) return validationResult.ToResultFailure();
        var interaction = await GetOwnedInteraction(id);
        if (!interaction) return Result.Fail(interaction.Errors);
        interaction.Value.Update(model.Description, model.InteractionPurpose, model.InteractionType, model.InteractionStatus);
        companyRepository.Update(interaction.Value);
        await companyRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> DeleteInteraction(Guid id)
    {
        var interaction = await GetOwnedInteraction(id);
        if (!interaction) return Result.Fail(interaction.Errors);
        companyRepository.Delete(interaction.Value);
        await companyRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<PaginatedList<DealListModel>> GetDeals(GetDealsFilter filter)
    {
        var agencyId = identityServerService.GetAgencyId();
        filter.OwnerId = OwnerScope ?? filter.OwnerId;
        return await companyRepository.GetDeals(agencyId, filter);
    }

    public async Task<Result<Guid>> CreateDeal()
    {
        var validation = uploadedFilesService.Validate();
        if (!validation) return Result.Fail<Guid>(validation.Errors);
        var model = uploadedFilesService.GetModel<CreateDealModel>();
        var validationResult = await createDealValidator.ValidateAsync(model);
        if (!validationResult.IsValid) return validationResult.ToResultFailure<Guid>();
        var userId = identityServerService.GetUserId();
        var deal = new Deal(model.Title, userId, model.CompanyProfileId, model.Date, model.Value,
            model.Type, model.Status, model.DocumentId);
        if (!string.IsNullOrWhiteSpace(model.FileName))
        {
            var file = CovenantFile.Create(model.FileName);
            if (!file) return Result.Fail<Guid>(file.Errors);
            await companyRepository.Create(file.Value);
            deal.Document = file.Value;
        }
        await companyRepository.Create(deal);
        await companyRepository.SaveChangesAsync();
        if (!string.IsNullOrWhiteSpace(model.FileName))
            await uploadedFilesService.Upload([model.FileName]);
        return Result.Ok(deal.Id);
    }

    public async Task<Result> UpdateDeal(Guid id)
    {
        var validation = uploadedFilesService.Validate();
        if (!validation) return Result.Fail(validation.Errors);
        var model = uploadedFilesService.GetModel<UpdateDealModel>();
        var validationResult = await updateDealValidator.ValidateAsync(model);
        if (!validationResult.IsValid) return validationResult.ToResultFailure();
        var result = await GetOwnedDeal(id);
        if (!result) return Result.Fail(result.Errors);
        var deal = result.Value;
        var previousDocumentId = deal.DocumentId;
        var documentId = model.DocumentId;
        var hasNewFile = !string.IsNullOrWhiteSpace(model.FileName);
        if (hasNewFile)
        {
            var file = CovenantFile.Create(model.FileName);
            if (!file) return Result.Fail(file.Errors);
            await companyRepository.Create(file.Value);
            documentId = file.Value.Id;
        }
        deal.Update(model.Title, model.Date, model.Value, model.Type, model.Status, documentId);
        companyRepository.Update(deal);
        await companyRepository.SaveChangesAsync();
        if (hasNewFile)
        {
            await uploadedFilesService.Upload([model.FileName]);
            if (previousDocumentId.HasValue) await documentService.DeleteFile(previousDocumentId.Value);
        }
        return Result.Ok();
    }

    public async Task<Result> DeleteDeal(Guid id)
    {
        var result = await GetOwnedDeal(id);
        if (!result) return Result.Fail(result.Errors);
        companyRepository.Delete(result.Value);
        await companyRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result<DealsByStatusModel>> GetDealsByStatus(GetDealsByStatusFilter filter)
    {
        var validationResult = await dealsByStatusValidator.ValidateAsync(filter);
        if (!validationResult.IsValid) return validationResult.ToResultFailure<DealsByStatusModel>();
        var agencyId = identityServerService.GetAgencyId();
        filter.OwnerId = OwnerScope ?? filter.OwnerId;
        var statuses = (filter.Statuses ?? []).Distinct().OrderBy(s => s).ToList();
        var window = GetPeriodWindow(filter.Period, timeService.GetCurrentDateTimeOffset());
        var rows = await companyRepository.GetDealsByStatus(agencyId, filter.OwnerId, window.FromUtc, window.ToUtcExclusive, statuses);
        var items = FillStatuses(rows, statuses.Count > 0 ? statuses : Enum.GetValues<DealStatus>().ToList());
        return Result.Ok(new DealsByStatusModel
        {
            Period = window.Range,
            TotalCount = items.Sum(i => i.Count),
            TotalValue = items.Sum(i => i.TotalValue),
            Items = items
        });
    }

    public async Task<SalesDashboardSummaryModel> GetDashboardSummary(GetSalesDashboardSummaryFilter filter)
    {
        var agencyId = identityServerService.GetAgencyId();
        var ownerId = OwnerScope ?? filter.OwnerId;
        var now = timeService.GetCurrentDateTimeOffset();
        var quarter = GetPeriodWindow(SalesPeriod.Quarter, now);
        var week = GetPeriodWindow(SalesPeriod.Week, now);
        var pipeline = await companyRepository.GetDealsByStatus(agencyId, ownerId, quarter.FromUtc, quarter.ToUtcExclusive, []);
        var activity = await companyRepository.GetInteractionsByType(agencyId, ownerId, week.FromUtc, week.ToUtcExclusive);
        return new SalesDashboardSummaryModel
        {
            AsOf = now.UtcDateTime,
            Quarter = quarter.Range,
            Week = week.Range,
            Pipeline = FillStatuses(pipeline, Enum.GetValues<DealStatus>().ToList()),
            Activity = FillTypes(activity)
        };
    }

    private SalesPeriodWindow GetPeriodWindow(SalesPeriod period, DateTimeOffset now)
    {
        DateTime today = now.UtcDateTime.Date;
        (DateTime from, DateTime toExclusive) = period switch
        {
            SalesPeriod.Day => (today, today.AddDays(1)),
            SalesPeriod.Week => (today.StartOfWeekSunday(), today.StartOfWeekSunday().AddDays(7)),
            SalesPeriod.Month => (today.StartOfMonth(), today.StartOfMonth().AddMonths(1)),
            SalesPeriod.Quarter => (today.StartOfQuarter(), today.StartOfQuarter().AddMonths(3)),
            _ => throw new ArgumentOutOfRangeException(nameof(period))
        };
        DateTime to = toExclusive.AddDays(-1);
        return new SalesPeriodWindow
        {
            Range = new SalesPeriodRangeModel
            {
                Period = period,
                From = DateTime.SpecifyKind(from, DateTimeKind.Unspecified),
                To = DateTime.SpecifyKind(to, DateTimeKind.Unspecified),
                Label = GetLabel(period, from, to)
            },
            FromUtc = DateTime.SpecifyKind(from, DateTimeKind.Utc),
            ToUtcExclusive = DateTime.SpecifyKind(toExclusive, DateTimeKind.Utc)
        };
    }

    private static string GetLabel(SalesPeriod period, DateTime from, DateTime to) => period switch
    {
        SalesPeriod.Day => from.ToString("MMM d, yyyy", CultureInfo.InvariantCulture),
        SalesPeriod.Week => $"{from.ToString("MMM d", CultureInfo.InvariantCulture)} - {to.ToString("MMM d, yyyy", CultureInfo.InvariantCulture)}",
        SalesPeriod.Month => from.ToString("MMMM yyyy", CultureInfo.InvariantCulture),
        SalesPeriod.Quarter => $"Q{from.Quarter()} {from.Year}",
        _ => throw new ArgumentOutOfRangeException(nameof(period))
    };

    private static List<DealStatusSummaryModel> FillStatuses(List<DealStatusSummaryModel> rows, List<DealStatus> statuses) =>
        statuses
            .Select(status => rows.FirstOrDefault(r => r.Status == status)
                ?? new DealStatusSummaryModel { Status = status, Count = 0, TotalValue = 0 })
            .ToList();

    private static List<InteractionTypeSummaryModel> FillTypes(List<InteractionTypeSummaryModel> rows) =>
        Enum.GetValues<InteractionType>()
            .Select(type => rows.FirstOrDefault(r => r.Type == type)
                ?? new InteractionTypeSummaryModel { Type = type, Count = 0 })
            .ToList();

    private void ApplyScope(GetRequestForAgencyFilter filter)
    {
        filter.HasPermissionToSeeInternalRequests = identityServerService.IsAdmin();
        filter.SalesPersonnelId = SalesScope;
    }

    private async Task<Result<CompanyInteraction>> GetOwnedInteraction(Guid id)
    {
        var interaction = await companyRepository.GetInteraction(i => i.Id == id);
        if (interaction is null) return Result.Fail<CompanyInteraction>("Interaction not found");
        if (!identityServerService.IsAdmin() && interaction.UserId != identityServerService.GetUserId())
            return Result.Fail<CompanyInteraction>("You can only manage your own interactions");
        return Result.Ok(interaction);
    }

    private async Task<Result<Deal>> GetOwnedDeal(Guid id)
    {
        var deal = await companyRepository.GetDeal(d => d.Id == id);
        if (deal is null) return Result.Fail<Deal>("Deal not found");
        if (!identityServerService.IsAdmin() && deal.UserId != identityServerService.GetUserId())
            return Result.Fail<Deal>("You can only manage your own deals");
        return Result.Ok(deal);
    }
}
