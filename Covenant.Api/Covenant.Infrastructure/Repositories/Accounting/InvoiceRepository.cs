using Covenant.Common.Entities.Accounting.Invoice;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Accounting.Invoice;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Models.Company;
using Covenant.Common.Repositories.Accounting;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Context;
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
        var query = from i in _context.Invoice.Where(i => i.Company.CompanyId == companyId)
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
        var query = from i in _context.InvoiceUSA.Where(i => i.CompanyProfile.CompanyId == companyId)
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

    public async Task<InvoiceListModelWithTotals> GetInvoicesForAgency(IEnumerable<Guid> agencyIds, GetInvoicesFilterV2 filter)
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

    public async Task<InvoiceListModelWithTotals> GetInvoicesUSAForAgency(IEnumerable<Guid> agencyIds, GetInvoicesFilterV2 filter)
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

    public async Task<List<InvoiceListModel>> GetAllInvoicesForAgency(IEnumerable<Guid> agencyIds, GetInvoicesFilterV2 filter)
    {
        var query = GetInvoicesQueryForAgency(agencyIds, filter);
        var result = await query.ToListAsync();
        return result;
    }

    public async Task<List<InvoiceListModel>> GetAllInvoicesUSAForAgency(IEnumerable<Guid> agencyIds, GetInvoicesFilterV2 filter)
    {
        var query = GetInvoicesUSAQueryForAgency(agencyIds, filter);
        var result = await query.ToListAsync();
        return result;
    }

    public async Task<(Guid InvoiceId, string InvoiceNumber)> DeleteInvoiceAndReportsSubcontractor(Guid invoiceId)
    {
        var invoice = await _context.Invoice
            .Include(i => i.InvoiceTotals).ThenInclude(i => i.TimeSheetTotal)
            .Where(c => c.Id == invoiceId)
            .SingleOrDefaultAsync();
        if (invoice is null)
        {
            return default;
        }
        var reports = await (from i in _context.Invoice.Where(c => c.Id == invoiceId)
                             join it in _context.InvoiceTotals on i.Id equals it.InvoiceId
                             join tst in _context.TimeSheetTotal on it.TimeSheetTotalId equals tst.Id
                             join tstP in _context.TimeSheetTotalPayroll on tst.TimeSheetId equals tstP.TimeSheetId
                             join rsw in _context.ReportSubcontractorWageDetail on tstP.Id equals rsw.TimeSheetTotalId
                             join rs in _context.ReportSubcontractor on rsw.ReportSubcontractorId equals rs.Id
                             select rs).Distinct().ToListAsync();
        var totalsPayroll = await (from it in _context.InvoiceTotals.Where(c => c.InvoiceId == invoiceId)
                                   join tst in _context.TimeSheetTotal on it.TimeSheetTotalId equals tst.Id
                                   join tstP in _context.TimeSheetTotalPayroll on tst.TimeSheetId equals tstP.TimeSheetId
                                   join rsw in _context.ReportSubcontractorWageDetail on tstP.Id equals rsw.TimeSheetTotalId
                                   select tstP).ToListAsync();
        _context.Invoice.Remove(invoice);
        if (invoice.InvoiceTotals.Any(it => it.TimeSheetTotal != null))
        {
            _context.TimeSheetTotal.RemoveRange(invoice.InvoiceTotals.Select(s => s.TimeSheetTotal));
        }
        _context.ReportSubcontractor.RemoveRange(reports);
        _context.TimeSheetTotalPayroll.RemoveRange(totalsPayroll);
        return (invoice.Id, invoice.DisplayInvoiceNumber());
    }

    public async Task<(Guid InvoiceId, string numberId)> DeleteInvoiceUSA(Guid invoiceId)
    {
        var invoice = await _context.InvoiceUSA
            .Where(c => c.Id == invoiceId)
            .Include(i => i.Items).ThenInclude(i => i.TimeSheetTotal)
            .SingleOrDefaultAsync();
        if (invoice is null)
        {
            return default;
        }
        _context.InvoiceUSA.Remove(invoice);
        if (invoice.Items.Any(i => i.TimeSheetTotal != null))
        {
            _context.TimeSheetTotal.RemoveRange(invoice.Items.Select(s => s.TimeSheetTotal));
        }
        return (invoice.Id, invoice.InvoiceNumber);
    }

    public virtual async Task<NextNumberModel> GetNextInvoiceNumber() =>
        (await _context.NextNumber.FromSqlRaw(SqlQueries.GetNextInvoiceNumber).ToListAsync()).Single();

    public virtual async Task<NextNumberModel> GetNextInvoiceUSANumber() =>
        (await _context.NextNumber.FromSqlRaw(SqlQueries.GetNextInvoiceUSANumber).ToListAsync()).Single();

    public async Task<InvoiceSummaryModel> GetInvoiceSummaryById(Guid id)
    {
        var q = from i in _context.Invoice.Where(c => c.Id == id)
                join cp in _context.CompanyProfile on i.CompanyId equals cp.Id
                join u in _context.User on cp.CompanyId equals u.Id
                join a in _context.Agencies on cp.AgencyId equals a.Id
                join cf in _context.CovenantFile on a.LogoId equals cf.Id into cf1
                from cf in cf1.DefaultIfEmpty()
                join cn in _context.CompanyProfileInvoiceNotes on cp.Id equals cn.CompanyProfileId
                    into cn1
                from cn in cn1.DefaultIfEmpty()
                select new InvoiceSummaryModel
                {
                    Id = i.Id,
                    CompanyProfileId = cp.Id,
                    CompanyFullName = cp.FullName,
                    PhonePrincipal = cp.Phone,
                    PhonePrincipalExt = cp.PhoneExt,
                    Fax = cp.Fax,
                    FaxExt = cp.FaxExt,
                    Email = i.Email != null ? i.Email : u.Email,
                    Address = (from cpl in cp.Locations.Where(c => c.IsBilling)
                               join l in _context.Location on cpl.LocationId equals l.Id
                               join city in _context.City on l.CityId equals city.Id
                               join p in _context.Province on city.ProvinceId equals p.Id
                               select $"{l.Address} {city.Value} {p.Code} {l.PostalCode}").FirstOrDefault(),
                    HstNumber = a.HstNumber,
                    HtmlNotes = cn == null ? string.Empty : cn.HtmlNotes,
                    AgencyFullName = a.FullName,
                    AgencyLogoFileName = cf == null ? null : cf.FileName,
                    AgencyAddress = (from al in a.Locations.Where(lW => lW.IsBilling)
                                     join l in _context.Location on al.LocationId equals l.Id
                                     join city in _context.City on l.CityId equals city.Id
                                     join p in _context.Province on city.ProvinceId equals p.Id
                                     select $"{l.Address} {city.Value} {p.Code} {l.PostalCode}").FirstOrDefault(),
                    AgencyPhone = a.PhonePrincipal,
                    AgencyPhoneExt = a.PhonePrincipalExt,
                    AgencyWebSite = a.WebPage,
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
                    InvoicePayroll = InvoicePayroll.Covenant
                };
        var invoice = await q.SingleOrDefaultAsync();
        invoice.Items = await GetInvoiceSummaryItemModels(id);
        return invoice;
    }

    public async Task<InvoiceSummaryModel> GetInvoiceUSASummaryById(Guid id)
    {
        var q = from i in _context.InvoiceUSA.Where(c => c.Id == id)
                join cp in _context.CompanyProfile on i.CompanyProfileId equals cp.Id
                join u in _context.User on cp.CompanyId equals u.Id
                join a in _context.Agencies on cp.AgencyId equals a.Id
                join cf in _context.CovenantFile on a.LogoId equals cf.Id into cf1
                from cf in cf1.DefaultIfEmpty()
                join cn in _context.CompanyProfileInvoiceNotes on cp.Id equals cn.CompanyProfileId
                    into cn1
                from cn in cn1.DefaultIfEmpty()
                select new InvoiceSummaryModel
                {
                    Id = i.Id,
                    CompanyProfileId = cp.Id,
                    CompanyFullName = cp.FullName,
                    PhonePrincipal = i.BillToPhone,
                    Fax = i.BillToFax,
                    Email = i.BillToEmail,
                    Address = i.BillToAddress,
                    HtmlNotes = cn == null ? string.Empty : cn.HtmlNotes,
                    AgencyFullName = a.FullName,
                    AgencyLogoFileName = cf == null ? null : cf.FileName,
                    AgencyAddress = i.BillFromAddress,
                    AgencyPhone = i.BillFromPhone,
                    AgencyFax = i.BillFromFax,
                    AgencyWebSite = a.WebPage,
                    CreatedAt = new DateOnly(i.CreatedAt.Year, i.CreatedAt.Month, i.CreatedAt.Day),
                    NumberId = i.NumberId,
                    InvoiceNumber = i.InvoiceNumber,
                    SubTotal = i.SubTotal,
                    Hst = i.Tax,
                    Total = i.TotalNet,
                    TaxName = "Tax",
                    WeedEnding = i.WeekEnding.HasValue ? i.WeekEnding.Value.Date : null,
                    Items = i.Items.Select(s => new InvoiceSummaryItemModel(s.Description, s.Quantity, s.UnitPrice, s.Total)).ToList(),
                    Discounts = i.Discounts.Select(d => new InvoiceSummaryDiscountModel
                    {
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice,
                        Amount = d.Total,
                        Description = d.Description
                    }).ToList(),
                    InvoiceColor = InvoiceColor.Sigook,
                    InvoicePayroll = InvoicePayroll.Sigook
                };
        var invoice = await q.SingleOrDefaultAsync();
        return invoice;
    }

    public async Task<List<CompanyRegularChargesByWorker>> GetCompanyRegularCharges(ParamsToGetRegularWages p)
    {
        var workers = from ts in _context.TimeSheet.Where(ts => p.RangeOfDaysWorkerMustWorkToReceiveHolidayPay.Contains(ts.Date.Date) && ts.TimeSheetTotal == null)
                      join cp in _context.CompanyProfile.Where(cp => cp.Id == p.ProfileId) on ts.WorkerRequest.Request.CompanyId equals cp.CompanyId
                      group ts by ts.WorkerRequest.WorkerId into g
                      select g.Key;

        return await (from i in _context.Invoice.Where(i => i.CompanyId == p.ProfileId)
                      join cp in _context.CompanyProfile on i.CompanyId equals cp.Id
                      join it in _context.InvoiceTotals on i.Id equals it.InvoiceId
                      join tst in _context.TimeSheetTotal on it.TimeSheetTotalId equals tst.Id
                      join ts in _context.TimeSheet.Where(s => s.Date.Date >= p.Start && s.Date.Date <= p.End) on tst.TimeSheetId equals ts.Id
                      join wr in _context.WorkerRequest.Where(wwr => workers.Contains(wwr.WorkerId)) on ts.WorkerRequestId equals wr.Id
                      join wp in _context.WorkerProfile on wr.WorkerId equals wp.WorkerId
                      select new { wr.WorkerId, wp.Id, it.AgencyRate, it.Regular, it.OtherRegular }
                      ).GroupBy(a => new { a.WorkerId, a.Id, a.AgencyRate })
            .Select(g => new CompanyRegularChargesByWorker(g.Key.WorkerId, g.Key.Id, g.Key.AgencyRate,
                g.Sum(r => r.Regular), g.Sum(or => or.OtherRegular)))
            .ToListAsync();
    }

    private IQueryable<InvoiceListModel> GetInvoicesQueryForAgency(IEnumerable<Guid> agencyIds, GetInvoicesFilterV2 filter)
    {
        var invoices = _context.Invoice.Where(i => agencyIds.Contains(i.Company.AgencyId));
        var query = from i in invoices
                    select new InvoiceListModel
                    {
                        Id = i.Id,
                        NumberId = i.NumberId,
                        InvoiceNumberId = i.InvoiceNumber,
                        CompanyFullName = i.Company.FullName,
                        CompanyProfileId = i.Company.Id,
                        SalesRepresentative = i.Company.SalesRepresentative.Name,
                        TotalNet = i.TotalNet,
                        CreatedAt = i.CreatedAt,
                        WeekEnding = i.WeekEnding.HasValue ? i.WeekEnding.Value : null,
                        InvoiceNumber = Invoice.PrefixInvoiceNumber + "-" +
                            i.InvoiceNumber.ToString().PadLeft(4, '0') + "-" +
                            (i.CreatedAt.Year % 100).ToString().PadLeft(2, '0'),
                        Email = i.Company.Company.Email
                    };
        var predicateNew = ApplyFilterInvoices(filter);
        query = query.Where(predicateNew);
        query = ApplySortInvoices(query, filter);
        return query;
    }

    private IQueryable<InvoiceListModel> GetInvoicesUSAQueryForAgency(IEnumerable<Guid> agencyIds, GetInvoicesFilterV2 filter)
    {
        var invoices = _context.InvoiceUSA.Where(i => agencyIds.Contains(i.CompanyProfile.AgencyId));
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

    private Expression<Func<InvoiceListModel, bool>> ApplyFilterInvoices(GetInvoicesFilterV2 filter)
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

    private IQueryable<InvoiceListModel> ApplySortInvoices(IQueryable<InvoiceListModel> query, GetInvoicesFilterV2 filter)
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

    private async Task<List<InvoiceSummaryItemModel>> GetInvoiceSummaryItemModels(Guid invoiceId)
    {
        return await _context.InvoiceTotals
            .Where(it => it.InvoiceId == invoiceId)
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
    }
}