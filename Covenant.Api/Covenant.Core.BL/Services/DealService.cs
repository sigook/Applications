using Covenant.Common.Entities.Company;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using Covenant.Common.Repositories.Company;
using Covenant.Core.BL.Interfaces;

namespace Covenant.Core.BL.Services;

public class DealService(
    IDealRepository dealRepository,
    IIdentityServerService identityServerService) : IDealService
{
    public async Task<PaginatedList<DealListModel>> GetDeals(GetDealsFilter filter)
    {
        var agencyId = identityServerService.GetAgencyId();
        if (identityServerService.IsSales())
            filter.OwnerId = identityServerService.GetUserId();
        return await dealRepository.GetDeals(agencyId, filter);
    }

    public async Task<Result<Guid>> Create(CreateDealModel model)
    {
        var agencyId = identityServerService.GetAgencyId();
        if (!await dealRepository.CompanyProfileBelongsToAgency(model.CompanyProfileId, agencyId))
            return Result.Fail<Guid>("Company profile not found");
        var ownerId = identityServerService.GetUserId();
        var deal = new Deal(model.Title, ownerId, model.CompanyProfileId, model.Date, model.Value,
            model.Type, model.Status, model.DocumentId);
        await dealRepository.Create(deal);
        await dealRepository.SaveChangesAsync();
        return Result.Ok(deal.Id);
    }

    public async Task<Result> Update(Guid id, UpdateDealModel model)
    {
        var result = await GetOwnedDeal(id);
        if (!result) return Result.Fail(result.Errors);
        result.Value.Update(model.Title, model.Date, model.Value, model.Type, model.Status, model.DocumentId);
        await dealRepository.Update(result.Value);
        await dealRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> Delete(Guid id)
    {
        var result = await GetOwnedDeal(id);
        if (!result) return Result.Fail(result.Errors);
        await dealRepository.Delete(result.Value);
        await dealRepository.SaveChangesAsync();
        return Result.Ok();
    }

    private async Task<Result<Deal>> GetOwnedDeal(Guid id)
    {
        var agencyId = identityServerService.GetAgencyId();
        var deal = await dealRepository.GetDeal(d => d.Id == id && d.Company.AgencyId == agencyId);
        if (deal is null) return Result.Fail<Deal>("Deal not found");
        if (identityServerService.IsSales() && deal.OwnerId != identityServerService.GetUserId())
            return Result.Fail<Deal>("You can only manage your own deals");
        return Result.Ok(deal);
    }
}
