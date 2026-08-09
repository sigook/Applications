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

public class DealRepository(CovenantContext context) : IDealRepository
{
    public async Task<PaginatedList<DealListModel>> GetDeals(Guid agencyId, GetDealsFilter filter)
    {
        var query = context.Deals
            .Where(d => d.CompanyProfile.AgencyId == agencyId)
            .Select(d => new DealListModel
            {
                Id = d.Id,
                Title = d.Title,
                CompanyProfileId = d.CompanyProfileId,
                CompanyName = d.CompanyProfile.FullName,
                OwnerId = d.UserId,
                OwnerName = context.AgencyPersonnel.Where(p => p.UserId == d.UserId).Select(p => p.Name).FirstOrDefault(),
                Date = d.Date,
                Value = d.Value,
                Type = d.Type,
                Status = d.Status,
                DocumentId = d.DocumentId,
                DocumentName = d.Document.FileName,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt
            });
        query = query.Where(ApplyFilter(filter));
        query = ApplySort(query, filter);
        return await query.ToPaginatedList(filter);
    }

    public Task<Deal> GetDeal(Expression<Func<Deal, bool>> expression) =>
        context.Deals.FirstOrDefaultAsync(expression);

    public Task<bool> CompanyProfileBelongsToAgency(Guid companyProfileId, Guid agencyId) =>
        context.CompanyProfiles.AnyAsync(p => p.Id == companyProfileId && p.AgencyId == agencyId);

    public async Task Create(Deal deal) => await context.Deals.AddAsync(deal);

    public Task Delete(Deal deal) => Task.FromResult(context.Deals.Remove(deal));

    public Task Update(Deal deal) => Task.FromResult(context.Deals.Update(deal));

    public Task SaveChangesAsync() => context.SaveChangesAsync();

    private static Expression<Func<DealListModel, bool>> ApplyFilter(GetDealsFilter filter)
    {
        Expression<Func<DealListModel, bool>> predicate = d => true;
        if (filter.CompanyProfileId.HasValue)
            predicate = predicate.And(d => d.CompanyProfileId == filter.CompanyProfileId.Value);
        if (filter.OwnerId.HasValue)
            predicate = predicate.And(d => d.OwnerId == filter.OwnerId.Value);
        if (filter.Type.HasValue)
            predicate = predicate.And(d => d.Type == filter.Type.Value);
        if (filter.Statuses != null && filter.Statuses.Any())
            predicate = predicate.And(d => filter.Statuses.Contains(d.Status));
        if (filter.DateFrom.HasValue && filter.DateTo.HasValue)
            predicate = predicate.And(d => d.Date.Date >= filter.DateFrom.Value.Date && d.Date.Date <= filter.DateTo.Value.Date);
        return predicate;
    }

    private static IQueryable<DealListModel> ApplySort(IQueryable<DealListModel> query, GetDealsFilter filter) =>
        filter.SortBy switch
        {
            GetDealsSortBy.Company => query.AddOrderBy(filter, d => d.CompanyName),
            GetDealsSortBy.Value => query.AddOrderBy(filter, d => d.Value),
            GetDealsSortBy.Status => query.AddOrderBy(filter, d => d.Status),
            GetDealsSortBy.Date => query.AddOrderBy(filter, d => d.Date),
            _ => query.AddOrderBy(filter, d => d.Date)
        };
}
