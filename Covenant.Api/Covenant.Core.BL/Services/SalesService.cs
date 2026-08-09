using Covenant.Common.Entities.Company;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
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
    ICompanyInteractionRepository interactionRepository,
    IDealRepository dealRepository,
    IIdentityServerService identityServerService,
    IValidator<CreateCompanyInteractionModel> createInteractionValidator,
    IValidator<UpdateCompanyInteractionModel> updateInteractionValidator,
    IValidator<CreateDealModel> createDealValidator,
    IValidator<UpdateDealModel> updateDealValidator) : ISalesService
{
    private Guid? SalesScope => identityServerService.IsSales() ? identityServerService.GetAgencyPersonnelId() : null;

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
        filter.OwnerId = identityServerService.GetUserId();
        return await interactionRepository.GetInteractions(agencyId, filter);
    }

    public async Task<Result<Guid>> CreateInteraction(CreateCompanyInteractionModel model)
    {
        var validationResult = await createInteractionValidator.ValidateAsync(model);
        if (!validationResult.IsValid) return validationResult.ToResultFailure<Guid>();
        var agencyId = identityServerService.GetAgencyId();
        if (!await interactionRepository.CompanyProfileBelongsToAgency(model.CompanyProfileId, agencyId))
            return Result.Fail<Guid>("Company profile not found");
        var userId = identityServerService.GetUserId();
        var interaction = new CompanyInteraction(model.Description, userId, model.CompanyProfileId,
            model.InteractionPurpose, model.InteractionType, model.InteractionStatus);
        await interactionRepository.Create(interaction);
        await interactionRepository.SaveChangesAsync();
        return Result.Ok(interaction.Id);
    }

    public async Task<Result> UpdateInteraction(Guid id, UpdateCompanyInteractionModel model)
    {
        var validationResult = await updateInteractionValidator.ValidateAsync(model);
        if (!validationResult.IsValid) return validationResult.ToResultFailure();
        var interaction = await GetOwnedInteraction(id);
        if (!interaction) return Result.Fail(interaction.Errors);
        interaction.Value.Update(model.Description, model.InteractionPurpose, model.InteractionType, model.InteractionStatus);
        await interactionRepository.Update(interaction.Value);
        await interactionRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> DeleteInteraction(Guid id)
    {
        var interaction = await GetOwnedInteraction(id);
        if (!interaction) return Result.Fail(interaction.Errors);
        await interactionRepository.Delete(interaction.Value);
        await interactionRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<PaginatedList<DealListModel>> GetDeals(GetDealsFilter filter)
    {
        var agencyId = identityServerService.GetAgencyId();
        filter.OwnerId = identityServerService.GetUserId();
        return await dealRepository.GetDeals(agencyId, filter);
    }

    public async Task<Result<Guid>> CreateDeal(CreateDealModel model)
    {
        var validationResult = await createDealValidator.ValidateAsync(model);
        if (!validationResult.IsValid) return validationResult.ToResultFailure<Guid>();
        var agencyId = identityServerService.GetAgencyId();
        if (!await dealRepository.CompanyProfileBelongsToAgency(model.CompanyProfileId, agencyId))
            return Result.Fail<Guid>("Company profile not found");
        var userId = identityServerService.GetUserId();
        var deal = new Deal(model.Title, userId, model.CompanyProfileId, model.Date, model.Value,
            model.Type, model.Status, model.DocumentId);
        await dealRepository.Create(deal);
        await dealRepository.SaveChangesAsync();
        return Result.Ok(deal.Id);
    }

    public async Task<Result> UpdateDeal(Guid id, UpdateDealModel model)
    {
        var validationResult = await updateDealValidator.ValidateAsync(model);
        if (!validationResult.IsValid) return validationResult.ToResultFailure();
        var result = await GetOwnedDeal(id);
        if (!result) return Result.Fail(result.Errors);
        result.Value.Update(model.Title, model.Date, model.Value, model.Type, model.Status, model.DocumentId);
        await dealRepository.Update(result.Value);
        await dealRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> DeleteDeal(Guid id)
    {
        var result = await GetOwnedDeal(id);
        if (!result) return Result.Fail(result.Errors);
        await dealRepository.Delete(result.Value);
        await dealRepository.SaveChangesAsync();
        return Result.Ok();
    }

    private void ApplyScope(GetRequestForAgencyFilter filter)
    {
        filter.HasPermissionToSeeInternalRequests = identityServerService.IsAdmin();
        filter.SalesPersonnelId = SalesScope;
    }

    private async Task<Result<CompanyInteraction>> GetOwnedInteraction(Guid id)
    {
        var agencyId = identityServerService.GetAgencyId();
        var interaction = await interactionRepository.GetInteraction(i => i.Id == id && i.CompanyProfile.AgencyId == agencyId);
        if (interaction is null) return Result.Fail<CompanyInteraction>("Interaction not found");
        if (identityServerService.IsSales() && interaction.UserId != identityServerService.GetUserId())
            return Result.Fail<CompanyInteraction>("You can only manage your own interactions");
        return Result.Ok(interaction);
    }

    private async Task<Result<Deal>> GetOwnedDeal(Guid id)
    {
        var agencyId = identityServerService.GetAgencyId();
        var deal = await dealRepository.GetDeal(d => d.Id == id && d.CompanyProfile.AgencyId == agencyId);
        if (deal is null) return Result.Fail<Deal>("Deal not found");
        if (identityServerService.IsSales() && deal.UserId != identityServerService.GetUserId())
            return Result.Fail<Deal>("You can only manage your own deals");
        return Result.Ok(deal);
    }
}
