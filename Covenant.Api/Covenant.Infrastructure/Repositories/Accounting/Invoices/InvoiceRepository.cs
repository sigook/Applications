using Covenant.Common.Entities.Accounting.Invoices;
using Covenant.Common.Enums;
using Covenant.Common.Models.Accounting.Invoices.Companies;
using Covenant.Common.Models.Accounting.Invoices;
using Covenant.Common.Models.Accounting.PayStubs;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Companies;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Accounting.Invoices;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Extensions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Covenant.Infrastructure.Repositories.Accounting.Invoices;

public class InvoiceRepository(CovenantContext context) : BaseRepository<CovenantContext>(context), IInvoiceRepository
{
    public async Task<PaginatedList<InvoiceListModel>> GetInvoicesForCompany(Guid companyId, GetCompanyInvoiceFilter filter)
    {
        var query = from i in Context.Invoices.Where(i => i.CompanyProfile.CompanyId == companyId)
                    select new InvoiceListModel
                    {
                        Id = i.Id,
                        NumberId = i.NumberId,
                        InvoiceNumberId = i.InvoiceNumberId,
                        InvoiceNumber = i.InvoiceNumber,
                        TotalNet = i.TotalNet,
                        CreatedAt = i.CreatedAt,
                        WeekEnding = i.WeekEnding,
                        PdfGeneratedAt = i.PdfGeneratedAt
                    };
        return await query.OrderByDescending(i => i.NumberId).ToPaginatedList(filter);
    }

    public async Task<InvoiceListModelWithTotals> GetInvoicesForAgency(IEnumerable<Guid> agencyIds, GetInvoicesFilter filter)
    {
        var query = GetInvoicesQueryForAgency(agencyIds, filter);
        var detail = await query.ToPaginatedList(filter);
        var total = query.Sum(d => d.TotalNet);
        return new InvoiceListModelWithTotals
        {
            Detail = detail,
            Total = total
        };
    }

    public Task<Invoice> GetInvoiceForAgency(IEnumerable<Guid> agencyIds, Guid invoiceId) =>
        Context.Invoices.FirstOrDefaultAsync(i => i.Id == invoiceId && agencyIds.Contains(i.CompanyProfile.AgencyId));

    public async Task<IReadOnlyList<string>> MarkPendingInvoicesAsDue(DateTime today)
    {
        var overdue = await Context.Invoices
            .Where(i => i.Status == InvoiceStatus.Pending && i.CreatedAt.Date.AddDays(i.CompanyProfile.PaymentTermDays) < today.Date)
            .OrderBy(i => i.NumberId)
            .Select(i => new { i.Id, i.InvoiceNumber })
            .ToListAsync();
        if (overdue.Count == 0) return [];

        var ids = overdue.Select(i => i.Id).ToList();
        await Context.Invoices
            .Where(i => ids.Contains(i.Id))
            .ExecuteUpdateAsync(s => s
                .SetProperty(i => i.Status, InvoiceStatus.Due)
                .SetProperty(i => i.UpdatedAt, today)
                .SetProperty(i => i.UpdatedBy, (Guid?)null));
        return overdue.Select(i => i.InvoiceNumber).ToList();
    }

    public Task MarkPdfGenerated(Guid invoiceId, DateTime generatedAt) =>
        Context.Invoices
            .Where(i => i.Id == invoiceId && i.PdfGeneratedAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(i => i.PdfGeneratedAt, generatedAt));

    public async Task<List<InvoiceListModel>> GetAllInvoicesForAgency(IEnumerable<Guid> agencyIds, GetInvoicesFilter filter)
    {
        var query = GetInvoicesQueryForAgency(agencyIds, filter);
        return await query.ToListAsync();
    }

    public async Task<(Guid InvoiceId, string InvoiceNumber)> DeleteInvoiceAndReportsSubcontractor(Guid invoiceId)
    {
        var invoice = await Context.Invoices
            .Include(i => i.InvoiceTotals).ThenInclude(i => i.TimeSheetTotal)
            .Where(c => c.Id == invoiceId)
            .SingleOrDefaultAsync();
        if (invoice is null)
        {
            return default;
        }
        var reports = await (from i in Context.Invoices.Where(c => c.Id == invoiceId)
                             from it in i.InvoiceTotals
                             join tstP in Context.TimeSheetTotalPayrolls on it.TimeSheetTotal.TimeSheetId equals tstP.TimeSheetId
                             join rsw in Context.ReportSubcontractorWageDetails on tstP.Id equals rsw.TimeSheetTotalId
                             select rsw.ReportSubcontractor).Distinct().ToListAsync();
        var totalsPayroll = await (from it in Context.InvoiceTotals.Where(c => c.InvoiceId == invoiceId)
                                   join tstP in Context.TimeSheetTotalPayrolls on it.TimeSheetTotal.TimeSheetId equals tstP.TimeSheetId
                                   join rsw in Context.ReportSubcontractorWageDetails on tstP.Id equals rsw.TimeSheetTotalId
                                   select tstP).ToListAsync();
        Context.Invoices.Remove(invoice);
        var timesheetTotals = invoice.InvoiceTotals
            .Where(it => it.TimeSheetTotal != null)
            .Select(s => s.TimeSheetTotal);
        if (timesheetTotals.Any())
        {
            Context.TimeSheetTotals.RemoveRange(timesheetTotals);
        }
        Context.ReportSubcontractors.RemoveRange(reports);
        Context.TimeSheetTotalPayrolls.RemoveRange(totalsPayroll);
        return (invoice.Id, invoice.InvoiceNumber);
    }

    public virtual async Task<NextNumberModel> GetNextInvoiceNumber(Guid agencyId) =>
        (await Context.NextNumber.FromSqlRaw(SqlQueries.GetNextInvoiceNumber, agencyId).ToListAsync()).Single();

    public Task<InvoiceSummaryModel> GetInvoiceSummaryById(Guid id) =>
        GetInvoiceSummary(i => i.Id == id);

    public Task<InvoiceSummaryModel> GetInvoiceSummaryForCompany(Guid companyId, Guid id) =>
        GetInvoiceSummary(i => i.Id == id && i.CompanyProfile.CompanyId == companyId);

    private async Task<InvoiceSummaryModel> GetInvoiceSummary(Expression<Func<Invoice, bool>> predicate)
    {
        var q = from i in Context.Invoices.Where(predicate)
                join cn in Context.CompanyProfileInvoiceNotes on i.CompanyProfileId equals cn.CompanyProfileId
                    into cn1
                from cn in cn1.DefaultIfEmpty()
                select new InvoiceSummaryModel
                {
                    Id = i.Id,
                    AgencyId = i.CompanyProfile.AgencyId,
                    CompanyProfileId = i.CompanyProfileId,
                    CompanyFullName = i.CompanyProfile.FullName,
                    PhonePrincipal = i.CompanyProfile.Phone,
                    PhonePrincipalExt = i.CompanyProfile.PhoneExt,
                    Fax = i.CompanyProfile.Fax,
                    FaxExt = i.CompanyProfile.FaxExt,
                    Email = i.Email ?? i.CompanyProfile.Company.Email,
                    Address = (from cpl in i.CompanyProfile.Locations.Where(c => c.IsBilling).OrderBy(c => c.LocationId)
                               select $"{cpl.Location.Address} {cpl.Location.City.Value} {cpl.Location.City.Province.Code} {cpl.Location.PostalCode}").FirstOrDefault(),
                    HstNumber = i.CompanyProfile.Agency.HstNumber,
                    HtmlNotes = cn == null ? string.Empty : cn.HtmlNotes,
                    AgencyFullName = i.CompanyProfile.Agency.FullName,
                    AgencyLogoFileName = i.CompanyProfile.Agency.Logo == null ? null : i.CompanyProfile.Agency.Logo.FileName,
                    AgencyAddress = (from al in i.CompanyProfile.Agency.Locations.Where(lW => lW.IsBilling).OrderBy(lW => lW.LocationId)
                                     select $"{al.Location.Address} {al.Location.City.Value} {al.Location.City.Province.Code} {al.Location.PostalCode}").FirstOrDefault(),
                    AgencyPhone = i.CompanyProfile.Agency.PhonePrincipal,
                    AgencyPhoneExt = i.CompanyProfile.Agency.PhonePrincipalExt,
                    AgencyWebSite = i.CompanyProfile.Agency.WebPage,
                    CreatedAt = new DateOnly(i.CreatedAt.Year, i.CreatedAt.Month, i.CreatedAt.Day),
                    NumberId = i.NumberId,
                    InvoiceNumber = i.InvoiceNumber,
                    SubTotal = i.SubTotal,
                    Tax = i.Tax,
                    Total = i.TotalNet,
                    WeedEnding = i.WeekEnding,
                    Discounts = i.Discounts.Select(d => new InvoiceSummaryDiscountModel
                    {
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice,
                        Amount = d.Amount,
                        Description = d.Description
                    }).ToList(),
                    Holidays = i.Holidays.Select(h => new InvoiceSummaryHolidayModel(h.Amount, h.Hours, $"Charge for Holiday {h.Holiday:D}")).ToList(),
                    AdditionalItems = i.AdditionalItems.Select(h => new InvoiceSummaryAdditionalItemModel(h.Quantity, h.UnitPrice, h.Total, h.Description)).ToList(),
                    ClientSiteAddress = i.AdditionalDetail != null ? i.AdditionalDetail.ClientSiteAddress : null
                };
        var invoice = await q.SingleOrDefaultAsync();
        if (invoice is null) return null;
        invoice.Items = await Context.InvoiceTotals
            .Where(it => it.InvoiceId == invoice.Id)
            .GroupBy(it => it.Description)
            .Select(group => new InvoiceSummaryItemModel
            {
                Description = group.Key,
                Quantity = group.Sum(i => i.Quantity),
                Total = group.Sum(i => i.Total),
                UnitPrice = group.Sum(i => i.Quantity) > 0
                    ? group.Sum(i => i.Total) / (decimal)group.Sum(i => i.Quantity)
                    : 0
            }).ToListAsync();
        return invoice;
    }

    public async Task<List<CompanyRegularChargesByWorker>> GetCompanyRegularCharges(Guid companyProfileId, DateTime holiday, DateTime start, DateTime end, IEnumerable<DateTime> qualifyingDays)
    {
        var alreadyBilled = from i in Context.Invoices.Where(i => i.CompanyProfileId == companyProfileId)
                            from ih in i.Holidays.Where(ih => ih.Holiday.Date == holiday.Date && ih.WorkerProfileId != null)
                            select ih.WorkerProfileId.Value;

        var workers = from ts in Context.TimeSheets.Where(ts => qualifyingDays.Contains(ts.Date.Date)
                          && ts.TimeSheetTotal == null
                          && ts.TimeInApproved != null
                          && ts.TimeOutApproved != null)
                      where ts.WorkerRequest.Request.CompanyProfileId == companyProfileId
                      group ts by ts.WorkerRequest.WorkerProfileId into g
                      select g.Key;

        return await (from i in Context.Invoices.Where(i => i.CompanyProfileId == companyProfileId)
                      from it in i.InvoiceTotals
                      where it.TimeSheetTotal.TimeSheet.Date.Date >= start
                            && it.TimeSheetTotal.TimeSheet.Date.Date <= end
                            && workers.Contains(it.TimeSheetTotal.TimeSheet.WorkerRequest.WorkerProfileId)
                            && !alreadyBilled.Contains(it.TimeSheetTotal.TimeSheet.WorkerRequest.WorkerProfileId)
                      select new
                      {
                          it.TimeSheetTotal.TimeSheet.WorkerRequest.WorkerProfile.WorkerId,
                          Id = it.TimeSheetTotal.TimeSheet.WorkerRequest.WorkerProfileId,
                          it.AgencyRate,
                          it.Regular,
                          it.OtherRegular
                      }
                      ).GroupBy(a => new { a.WorkerId, a.Id, a.AgencyRate })
            .Select(g => new CompanyRegularChargesByWorker(g.Key.WorkerId, g.Key.Id, g.Key.AgencyRate,
                g.Sum(r => r.Regular), g.Sum(or => or.OtherRegular)))
            .ToListAsync();
    }

    private IQueryable<InvoiceListModel> GetInvoicesQueryForAgency(IEnumerable<Guid> agencyIds, GetInvoicesFilter filter)
    {
        var invoices = Context.Invoices.Where(i => agencyIds.Contains(i.CompanyProfile.AgencyId));
        var query = from i in invoices
                    select new InvoiceListModel
                    {
                        Id = i.Id,
                        NumberId = i.NumberId,
                        InvoiceNumberId = i.InvoiceNumberId,
                        InvoiceNumber = i.InvoiceNumber,
                        CompanyFullName = i.CompanyProfile.FullName,
                        CompanyProfileId = i.CompanyProfile.Id,
                        SalesRepresentative = i.CompanyProfile.SalesRepresentative.Name,
                        SubTotal = i.SubTotal,
                        Tax = i.Tax,
                        TotalNet = i.TotalNet,
                        CreatedAt = i.CreatedAt,
                        WeekEnding = i.WeekEnding.HasValue ? i.WeekEnding.Value : null,
                        Email = i.Email ?? i.CompanyProfile.Company.Email,
                        Status = i.Status,
                        PaymentTermDays = i.CompanyProfile.PaymentTermDays,
                        PaidAt = i.PaidAt,
                        PdfGeneratedAt = i.PdfGeneratedAt,
                        UpdatedAt = i.UpdatedAt,
                        UpdatedByName = Context.AgencyPersonnel
                            .Where(p => p.UserId == i.UpdatedBy && p.AgencyId == i.CompanyProfile.AgencyId)
                            .Select(p => p.Name)
                            .FirstOrDefault() ?? i.UpdatedByUser.Email
                    };
        var predicateNew = ApplyFilterInvoices(filter);
        query = query.Where(predicateNew);
        query = ApplySortInvoices(query, filter);
        return query;
    }

    private Expression<Func<InvoiceListModel, bool>> ApplyFilterInvoices(GetInvoicesFilter filter)
    {
        Expression<Func<InvoiceListModel, bool>> predicate = PredicateBuilder.New<InvoiceListModel>(true);
        if (filter.CreatedAtFrom.HasValue && filter.CreatedAtTo.HasValue)
            predicate = predicate.And(i => i.CreatedAt >= filter.CreatedAtFrom.Value && i.CreatedAt <= filter.CreatedAtTo.Value);
        predicate = predicate
            .AndSearch(filter.InvoiceNumber, i => i.InvoiceNumber)
            .AndSearch(filter.CompanyFullName, i => i.CompanyFullName)
            .AndSearch(filter.SalesRepresentative, i => i.SalesRepresentative);
        if (filter.Status.HasValue)
            predicate = predicate.And(i => i.Status == filter.Status.Value);
        return predicate;
    }

    private IQueryable<InvoiceListModel> ApplySortInvoices(IQueryable<InvoiceListModel> query, GetInvoicesFilter filter)
    {
        switch (filter.SortBy)
        {
            case GetInvoicesFilterSortBy.InvoiceNumber:
                query = query.AddOrderBy(filter, i => i.InvoiceNumberId);
                break;
            case GetInvoicesFilterSortBy.CreatedAt:
                query = query.AddOrderBy(filter, i => i.CreatedAt);
                break;
            case GetInvoicesFilterSortBy.CompanyFullName:
                query = query.AddOrderBy(filter, i => i.CompanyFullName);
                break;
            case GetInvoicesFilterSortBy.SalesRepresentative:
                query = query.AddOrderBy(filter, i => i.SalesRepresentative);
                break;
            case GetInvoicesFilterSortBy.Status:
                query = query.AddOrderBy(filter, i => i.Status);
                break;
            default:
                query = query.OrderByDescending(i => i.NumberId);
                break;
        }
        return query;
    }
}
