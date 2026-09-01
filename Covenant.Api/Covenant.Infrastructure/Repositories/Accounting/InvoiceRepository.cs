using Covenant.Common.Entities.Accounting.Invoice;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Accounting.Invoice;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Models.Company;
using Covenant.Common.Repositories.Accounting;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Covenant.Infrastructure.Repositories.Accounting;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly CovenantContext _context;

    public InvoiceRepository(CovenantContext context) => _context = context;

    public async Task Create<T>(T entity) where T : class => await _context.Set<T>().AddAsync(entity);

    public Task SaveChangesAsync() => _context.SaveChangesAsync();

    public async Task<PaginatedList<InvoiceListModel>> GetInvoicesForCompany(Guid companyId, GetCompanyInvoiceFilter filter)
    {
        var query = from i in _context.Invoices.Where(i => i.CompanyProfile.CompanyId == companyId)
                    select new InvoiceListModel
                    {
                        Id = i.Id,
                        NumberId = i.NumberId,
                        InvoiceNumberId = i.InvoiceNumber,
                        TotalNet = i.TotalNet,
                        CreatedAt = i.CreatedAt,
                        WeekEnding = i.WeekEnding,
                    };
        return await query.ToPaginatedList(filter);
    }

    public async Task<PaginatedList<InvoiceListModel>> GetInvoicesForCompanyUSA(Guid companyId, GetCompanyInvoiceFilter filter)
    {
        var query = from i in _context.InvoicesUSA.Where(i => i.CompanyProfile.CompanyId == companyId)
                    select new InvoiceListModel
                    {
                        Id = i.Id,
                        NumberId = i.NumberId,
                        InvoiceNumber = i.InvoiceNumber,
                        TotalNet = i.TotalNet,
                        CreatedAt = i.CreatedAt,
                        WeekEnding = i.WeekEnding
                    };
        return await query.ToPaginatedList(filter);
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

    public async Task<InvoiceListModelWithTotals> GetInvoicesUSAForAgency(IEnumerable<Guid> agencyIds, GetInvoicesFilter filter)
    {
        var query = GetInvoicesUSAQueryForAgency(agencyIds, filter);
        var detail = await query.ToPaginatedList(filter);
        var total = query.Sum(d => d.TotalNet);
        return new InvoiceListModelWithTotals
        {
            Detail = detail,
            Total = total
        };
    }

    public async Task<List<InvoiceListModel>> GetAllInvoicesForAgency(IEnumerable<Guid> agencyIds, GetInvoicesFilter filter)
    {
        var query = GetInvoicesQueryForAgency(agencyIds, filter);
        var result = await query.ToListAsync();
        return result;
    }

    public async Task<List<InvoiceListModel>> GetAllInvoicesUSAForAgency(IEnumerable<Guid> agencyIds, GetInvoicesFilter filter)
    {
        var query = GetInvoicesUSAQueryForAgency(agencyIds, filter);
        var result = await query.ToListAsync();
        return result;
    }

    public async Task<(Guid InvoiceId, string InvoiceNumber)> DeleteInvoiceAndReportsSubcontractor(Guid invoiceId)
    {
        var invoice = await _context.Invoices
            .Include(i => i.InvoiceTotals).ThenInclude(i => i.TimeSheetTotal)
            .Where(c => c.Id == invoiceId)
            .SingleOrDefaultAsync();
        if (invoice is null)
        {
            return default;
        }
        var reports = await (from i in _context.Invoices.Where(c => c.Id == invoiceId)
                             from it in i.InvoiceTotals
                             join tstP in _context.TimeSheetTotalPayrolls on it.TimeSheetTotal.TimeSheetId equals tstP.TimeSheetId
                             join rsw in _context.ReportSubcontractorWageDetails on tstP.Id equals rsw.TimeSheetTotalId
                             select rsw.ReportSubcontractor).Distinct().ToListAsync();
        var totalsPayroll = await (from it in _context.InvoiceTotals.Where(c => c.InvoiceId == invoiceId)
                                   join tstP in _context.TimeSheetTotalPayrolls on it.TimeSheetTotal.TimeSheetId equals tstP.TimeSheetId
                                   join rsw in _context.ReportSubcontractorWageDetails on tstP.Id equals rsw.TimeSheetTotalId
                                   select tstP).ToListAsync();
        _context.Invoices.Remove(invoice);
        var timesheetTotals = invoice.InvoiceTotals
            .Where(it => it.TimeSheetTotal != null)
            .Select(s => s.TimeSheetTotal);
        if (timesheetTotals.Any())
        {
            _context.TimeSheetTotals.RemoveRange(timesheetTotals);
        }
        _context.ReportSubcontractors.RemoveRange(reports);
        _context.TimeSheetTotalPayrolls.RemoveRange(totalsPayroll);
        return (invoice.Id, invoice.DisplayInvoiceNumber());
    }

    public async Task<(Guid InvoiceId, string numberId)> DeleteInvoiceUSA(Guid invoiceId)
    {
        var invoice = await _context.InvoicesUSA
            .Where(c => c.Id == invoiceId)
            .Include(i => i.Items).ThenInclude(i => i.TimeSheetTotal)
            .SingleOrDefaultAsync();
        if (invoice is null)
        {
            return default;
        }
        _context.InvoicesUSA.Remove(invoice);
        var timesheetTotal = invoice.Items
            .Where(i => i.TimeSheetTotal != null)
            .Select(s => s.TimeSheetTotal);
        if (timesheetTotal.Any())
        {
            _context.TimeSheetTotals.RemoveRange(timesheetTotal);
        }
        return (invoice.Id, invoice.InvoiceNumber);
    }

    public virtual async Task<NextNumberModel> GetNextInvoiceNumber() =>
        (await _context.NextNumber.FromSqlRaw(SqlQueries.GetNextInvoiceNumber).ToListAsync()).Single();

    public virtual async Task<NextNumberModel> GetNextInvoiceUSANumber() =>
        (await _context.NextNumber.FromSqlRaw(SqlQueries.GetNextInvoiceUSANumber).ToListAsync()).Single();

    public async Task<InvoiceSummaryModel> GetInvoiceSummaryById(Guid id)
    {
        var q = from i in _context.Invoices.Where(c => c.Id == id)
                join cn in _context.CompanyProfileInvoiceNotes on i.CompanyProfileId equals cn.CompanyProfileId
                    into cn1
                from cn in cn1.DefaultIfEmpty()
                select new InvoiceSummaryModel
                {
                    Id = i.Id,
                    CompanyProfileId = i.CompanyProfileId,
                    CompanyFullName = i.CompanyProfile.FullName,
                    PhonePrincipal = i.CompanyProfile.Phone,
                    PhonePrincipalExt = i.CompanyProfile.PhoneExt,
                    Fax = i.CompanyProfile.Fax,
                    FaxExt = i.CompanyProfile.FaxExt,
                    Email = i.Email != null ? i.Email : i.CompanyProfile.Company.Email,
                    Address = (from cpl in i.CompanyProfile.Locations.Where(c => c.IsBilling)
                               select $"{cpl.Location.Address} {cpl.Location.City.Value} {cpl.Location.City.Province.Code} {cpl.Location.PostalCode}").FirstOrDefault(),
                    HstNumber = i.CompanyProfile.Agency.HstNumber,
                    HtmlNotes = cn == null ? string.Empty : cn.HtmlNotes,
                    AgencyFullName = i.CompanyProfile.Agency.FullName,
                    AgencyLogoFileName = i.CompanyProfile.Agency.Logo == null ? null : i.CompanyProfile.Agency.Logo.FileName,
                    AgencyAddress = (from al in i.CompanyProfile.Agency.Locations.Where(lW => lW.IsBilling)
                                     select $"{al.Location.Address} {al.Location.City.Value} {al.Location.City.Province.Code} {al.Location.PostalCode}").FirstOrDefault(),
                    AgencyPhone = i.CompanyProfile.Agency.PhonePrincipal,
                    AgencyPhoneExt = i.CompanyProfile.Agency.PhonePrincipalExt,
                    AgencyWebSite = i.CompanyProfile.Agency.WebPage,
                    CreatedAt = new DateOnly(i.CreatedAt.Year, i.CreatedAt.Month, i.CreatedAt.Day),
                    NumberId = i.NumberId,
                    InvoiceNumber = $"{Invoice.PrefixInvoiceNumber}-{i.InvoiceNumber:0000}-{i.CreatedAt:yy}",
                    SubTotal = i.SubTotal,
                    Hst = i.Hst,
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
                    InvoiceColor = InvoiceColor.Covenant,
                    InvoicePayroll = InvoicePayroll.Covenant,
                    ClientSiteAddress = i.AdditionalDetail != null ? i.AdditionalDetail.ClientSiteAddress : null
                };
        var invoice = await q.SingleOrDefaultAsync();
        var items = await _context.InvoiceTotals
            .Where(it => it.InvoiceId == id)
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
        invoice.Items = items;
        return invoice;
    }

    public async Task<InvoiceSummaryModel> GetInvoiceUSASummaryById(Guid id)
    {
        var q = from i in _context.InvoicesUSA.Where(c => c.Id == id)
                join cn in _context.CompanyProfileInvoiceNotes on i.CompanyProfileId equals cn.CompanyProfileId
                    into cn1
                from cn in cn1.DefaultIfEmpty()
                select new InvoiceSummaryModel
                {
                    Id = i.Id,
                    CompanyProfileId = i.CompanyProfileId,
                    CompanyFullName = i.CompanyProfile.FullName,
                    PhonePrincipal = i.BillToPhone,
                    Fax = i.BillToFax,
                    Email = i.BillToEmail,
                    Address = i.BillToAddress,
                    HtmlNotes = cn == null ? string.Empty : cn.HtmlNotes,
                    AgencyFullName = i.CompanyProfile.Agency.FullName,
                    AgencyLogoFileName = i.CompanyProfile.Agency.Logo == null ? null : i.CompanyProfile.Agency.Logo.FileName,
                    AgencyAddress = i.BillFromAddress,
                    AgencyPhone = i.BillFromPhone,
                    AgencyFax = i.BillFromFax,
                    AgencyWebSite = i.CompanyProfile.Agency.WebPage,
                    CreatedAt = new DateOnly(i.CreatedAt.Year, i.CreatedAt.Month, i.CreatedAt.Day),
                    NumberId = i.NumberId,
                    InvoiceNumber = i.InvoiceNumber,
                    SubTotal = i.SubTotal,
                    Hst = i.Tax,
                    Total = i.TotalNet,
                    TaxName = "Tax",
                    WeedEnding = i.WeekEnding.HasValue ? i.WeekEnding.Value.Date : null,
                    Discounts = i.Discounts.Select(d => new InvoiceSummaryDiscountModel
                    {
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice,
                        Amount = d.Total,
                        Description = d.Description
                    }).ToList(),
                    InvoiceColor = InvoiceColor.Sigook,
                    InvoicePayroll = InvoicePayroll.Sigook,
                    ClientSiteAddress = i.AdditionalDetail != null ? i.AdditionalDetail.ClientSiteAddress : null
                };
        var invoice = await q.SingleOrDefaultAsync();
        var items = await _context.InvoiceUSAItems
            .Where(i => i.InvoiceUSAId == id)
            .GroupBy(i => i.Description)
            .Select(group => new InvoiceSummaryItemModel
            {
                Description = group.Key,
                Quantity = group.Sum(i => i.Quantity),
                Total = group.Sum(i => i.Total),
                UnitPrice = group.Sum(i => i.Quantity) > 0
                    ? group.Sum(i => i.Total) / (decimal)group.Sum(i => i.Quantity)
                    : 0
            }).ToListAsync();
        invoice.Items = items;
        return invoice;
    }

    public async Task<List<CompanyRegularChargesByWorker>> GetCompanyRegularCharges(Guid companyProfileId, DateTime holiday, DateTime start, DateTime end, IEnumerable<DateTime> qualifyingDays)
    {
        var alreadyBilled = from i in _context.Invoices.Where(i => i.CompanyProfileId == companyProfileId)
                            from ih in i.Holidays.Where(ih => ih.Holiday.Date == holiday.Date && ih.WorkerProfileId != null)
                            select ih.WorkerProfileId.Value;

        var workers = from ts in _context.TimeSheets.Where(ts => qualifyingDays.Contains(ts.Date.Date)
                          && ts.TimeSheetTotal == null
                          && ts.TimeInApproved != null
                          && ts.TimeOutApproved != null)
                      where ts.WorkerRequest.Request.CompanyProfileId == companyProfileId
                      group ts by ts.WorkerRequest.WorkerProfileId into g
                      select g.Key;

        return await (from i in _context.Invoices.Where(i => i.CompanyProfileId == companyProfileId)
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
        var invoices = _context.Invoices.Where(i => agencyIds.Contains(i.CompanyProfile.AgencyId));
        var query = from i in invoices
                    select new InvoiceListModel
                    {
                        Id = i.Id,
                        NumberId = i.NumberId,
                        InvoiceNumberId = i.InvoiceNumber,
                        CompanyFullName = i.CompanyProfile.FullName,
                        CompanyProfileId = i.CompanyProfile.Id,
                        SalesRepresentative = i.CompanyProfile.SalesRepresentative.Name,
                        TotalNet = i.TotalNet,
                        CreatedAt = i.CreatedAt,
                        WeekEnding = i.WeekEnding.HasValue ? i.WeekEnding.Value : null,
                        InvoiceNumber = Invoice.PrefixInvoiceNumber + "-" +
                            i.InvoiceNumber.ToString().PadLeft(4, '0') + "-" +
                            (i.CreatedAt.Year % 100).ToString().PadLeft(2, '0'),
                        Email = i.CompanyProfile.Company.Email
                    };
        var predicateNew = ApplyFilterInvoices(filter);
        query = query.Where(predicateNew);
        query = ApplySortInvoices(query, filter);
        return query;
    }

    private IQueryable<InvoiceListModel> GetInvoicesUSAQueryForAgency(IEnumerable<Guid> agencyIds, GetInvoicesFilter filter)
    {
        var invoices = _context.InvoicesUSA.Where(i => agencyIds.Contains(i.CompanyProfile.AgencyId));
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
                        TotalNet = i.TotalNet,
                        CreatedAt = i.CreatedAt,
                        WeekEnding = i.WeekEnding.HasValue ? i.WeekEnding.Value : null,
                        Email = i.CompanyProfile.Company.Email
                    };
        var predicateNew = ApplyFilterInvoices(filter);
        query = query.Where(predicateNew);
        query = ApplySortInvoices(query, filter);
        return query;
    }

    private Expression<Func<InvoiceListModel, bool>> ApplyFilterInvoices(GetInvoicesFilter filter)
    {
        var predicate = PredicateBuilder.New<InvoiceListModel>(true);
        if (!string.IsNullOrWhiteSpace(filter.InvoiceNumber))
            predicate = predicate.And(i => i.InvoiceNumber.Contains(filter.InvoiceNumber));
        if (filter.CreatedAtFrom.HasValue && filter.CreatedAtTo.HasValue)
            predicate = predicate.And(i => i.CreatedAt >= filter.CreatedAtFrom.Value && i.CreatedAt <= filter.CreatedAtTo.Value);
        if (!string.IsNullOrWhiteSpace(filter.CompanyFullName))
            predicate = predicate.And(i => i.CompanyFullName.ToLower().Contains(filter.CompanyFullName.ToLower()));
        if (!string.IsNullOrWhiteSpace(filter.SalesRepresentative))
            predicate = predicate.And(i => i.SalesRepresentative.ToLower().Contains(filter.SalesRepresentative.ToLower()));
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
        }
        return query;
    }
}