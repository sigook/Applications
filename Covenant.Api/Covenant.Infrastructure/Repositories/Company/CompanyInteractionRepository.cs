using Covenant.Common.Entities.Company;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Covenant.Infrastructure.Repositories.Company;

public class CompanyInteractionRepository(CovenantContext context) : ICompanyInteractionRepository
{
    public async Task<PaginatedList<CompanyInteractionListModel>> GetInteractions(Guid agencyId, GetCompanyInteractionsFilter filter)
    {
        var query = context.CompanyInteractions
            .Where(i => i.Company.AgencyId == agencyId)
            .Select(i => new CompanyInteractionListModel
            {
                Id = i.Id,
                CompanyProfileId = i.CompanyId,
                CompanyName = i.Company.FullName,
                OwnerId = i.OwnerId,
                OwnerName = context.AgencyPersonnel.Where(p => p.UserId == i.OwnerId).Select(p => p.Name).FirstOrDefault(),
                Description = i.Description,
                InteractionPurpose = i.InteractionPurpose,
                InteractionType = i.InteractionType,
                InteractionStatus = i.InteractionStatus,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt
            });
        query = query.Where(ApplyFilter(filter));
        query = ApplySort(query, filter);
        return await query.ToPaginatedList(filter);
    }

    public Task<CompanyInteraction> GetInteraction(Expression<Func<CompanyInteraction, bool>> expression) =>
        context.CompanyInteractions.FirstOrDefaultAsync(expression);

    public Task<bool> CompanyProfileBelongsToAgency(Guid companyProfileId, Guid agencyId) =>
        context.CompanyProfiles.AnyAsync(p => p.Id == companyProfileId && p.AgencyId == agencyId);

    public async Task Create(CompanyInteraction interaction) => await context.CompanyInteractions.AddAsync(interaction);

    public Task Delete(CompanyInteraction interaction) => Task.FromResult(context.CompanyInteractions.Remove(interaction));

    public Task Update(CompanyInteraction interaction) => Task.FromResult(context.CompanyInteractions.Update(interaction));

    public Task SaveChangesAsync() => context.SaveChangesAsync();

    private static Expression<Func<CompanyInteractionListModel, bool>> ApplyFilter(GetCompanyInteractionsFilter filter)
    {
        Expression<Func<CompanyInteractionListModel, bool>> predicate = i => true;
        if (filter.CompanyProfileId.HasValue)
            predicate = predicate.And(i => i.CompanyProfileId == filter.CompanyProfileId.Value);
        if (filter.OwnerId.HasValue)
            predicate = predicate.And(i => i.OwnerId == filter.OwnerId.Value);
        if (filter.InteractionPurpose.HasValue)
            predicate = predicate.And(i => i.InteractionPurpose == filter.InteractionPurpose.Value);
        if (filter.InteractionType.HasValue)
            predicate = predicate.And(i => i.InteractionType == filter.InteractionType.Value);
        if (filter.Statuses != null && filter.Statuses.Any())
            predicate = predicate.And(i => filter.Statuses.Contains(i.InteractionStatus));
        if (filter.CreatedAtFrom.HasValue && filter.CreatedAtTo.HasValue)
            predicate = predicate.And(i => i.CreatedAt.Date >= filter.CreatedAtFrom.Value.Date && i.CreatedAt.Date <= filter.CreatedAtTo.Value.Date);
        return predicate;
    }

    private static IQueryable<CompanyInteractionListModel> ApplySort(IQueryable<CompanyInteractionListModel> query, GetCompanyInteractionsFilter filter) =>
        filter.SortBy switch
        {
            GetCompanyInteractionsSortBy.Company => query.AddOrderBy(filter, i => i.CompanyName),
            GetCompanyInteractionsSortBy.Status => query.AddOrderBy(filter, i => i.InteractionStatus),
            GetCompanyInteractionsSortBy.CreatedAt => query.AddOrderBy(filter, i => i.CreatedAt),
            _ => query.AddOrderBy(filter, i => i.CreatedAt)
        };
}
