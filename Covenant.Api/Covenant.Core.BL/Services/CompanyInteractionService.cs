using Covenant.Common.Entities.Company;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using Covenant.Common.Repositories.Company;
using Covenant.Core.BL.Interfaces;

namespace Covenant.Core.BL.Services;

public class CompanyInteractionService(
    ICompanyInteractionRepository interactionRepository,
    IIdentityServerService identityServerService) : ICompanyInteractionService
{
    public async Task<PaginatedList<CompanyInteractionListModel>> GetInteractions(GetCompanyInteractionsFilter filter)
    {
        var agencyId = identityServerService.GetAgencyId();
        if (identityServerService.IsSales())
            filter.OwnerId = identityServerService.GetUserId();
        return await interactionRepository.GetInteractions(agencyId, filter);
    }

    public async Task<Result<Guid>> Create(CreateCompanyInteractionModel model)
    {
        var agencyId = identityServerService.GetAgencyId();
        if (!await interactionRepository.CompanyProfileBelongsToAgency(model.CompanyProfileId, agencyId))
            return Result.Fail<Guid>("Company profile not found");
        var ownerId = identityServerService.GetUserId();
        var interaction = new CompanyInteraction(model.Description, ownerId, model.CompanyProfileId,
            model.InteractionPurpose, model.InteractionType, model.InteractionStatus);
        await interactionRepository.Create(interaction);
        await interactionRepository.SaveChangesAsync();
        return Result.Ok(interaction.Id);
    }

    public async Task<Result> Update(Guid id, UpdateCompanyInteractionModel model)
    {
        var interaction = await GetOwnedInteraction(id);
        if (!interaction) return Result.Fail(interaction.Errors);
        interaction.Value.Update(model.Description, model.InteractionPurpose, model.InteractionType, model.InteractionStatus);
        await interactionRepository.Update(interaction.Value);
        await interactionRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> Delete(Guid id)
    {
        var interaction = await GetOwnedInteraction(id);
        if (!interaction) return Result.Fail(interaction.Errors);
        await interactionRepository.Delete(interaction.Value);
        await interactionRepository.SaveChangesAsync();
        return Result.Ok();
    }

    private async Task<Result<CompanyInteraction>> GetOwnedInteraction(Guid id)
    {
        var agencyId = identityServerService.GetAgencyId();
        var interaction = await interactionRepository.GetInteraction(i => i.Id == id && i.Company.AgencyId == agencyId);
        if (interaction is null) return Result.Fail<CompanyInteraction>("Interaction not found");
        if (identityServerService.IsSales() && interaction.OwnerId != identityServerService.GetUserId())
            return Result.Fail<CompanyInteraction>("You can only manage your own interactions");
        return Result.Ok(interaction);
    }
}
