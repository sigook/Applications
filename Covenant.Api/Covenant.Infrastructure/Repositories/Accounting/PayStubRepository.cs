using Covenant.Common.Configuration;
using Covenant.Common.Entities.Accounting.PayStub;
using Covenant.Common.Enums;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Repositories.Accounting;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Context;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Linq.Expressions;

namespace Covenant.Infrastructure.Repositories.Accounting;

public class PayStubRepository : IPayStubRepository
{
    private readonly Rates rates;
    private readonly CovenantContext _context;

    public PayStubRepository(Rates rates, CovenantContext context)
    {
        this.rates = rates;
        _context = context;
    }

    public virtual Task<List<NextNumberModel>> GetNextPayStubNumbers(int limit) =>
        _context.NextNumber.FromSqlRaw(SqlQueries.GetNextPayStubNumbers, new NpgsqlParameter("limit", limit)).ToListAsync();

    public Task<bool> IsPayStubNumberTaken(long payStubNumber) =>
        _context.PayStub.AnyAsync(s => s.PayStubNumberId == payStubNumber);

    public Task<PaginatedList<PayStubListModel>> GetPayStubs(IEnumerable<Guid> agencyIds, GetPayStubsFilter filter)
    {
        var query = GetPayStubsQuery(agencyIds, filter);
        var result = query.ToPaginatedList(filter);
        return result;
    }

    public Task<List<PayStubListModel>> GetAllPayStubs(IEnumerable<Guid> agencyIds, GetPayStubsFilter filter)
    {
        var query = GetPayStubsQuery(agencyIds, filter);
        var result = query.ToListAsync();
        return result;
    }

    public Task<PayStubDetailModel> GetPayStubDetail(Guid payStubId)
    {
        return (from ps in _context.PayStub.Where(s => s.Id == payStubId)
                join wp in _context.WorkerProfile on ps.WorkerProfileId equals wp.Id
                join wpu in _context.User on wp.WorkerId equals wpu.Id
                join a in _context.Agencies on wp.AgencyId equals a.Id
                join cfa in _context.CovenantFile on a.LogoId equals cfa.Id into tmp1
                from cfa in tmp1.DefaultIfEmpty()
                select new PayStubDetailModel
                {
                    Id = ps.Id,
                    NumberId = ps.NumberId,
                    PayrollNumberId = ps.PayStubNumberId,
                    PayrollNumber = ps.PayStubNumber,
                    AgencyFullName = a.FullName,
                    AgencyPhone = a.PhonePrincipal,
                    AgencyPhoneExt = a.PhonePrincipalExt,
                    AgencyLogoFileName = cfa == null ? null : cfa.FileName,
                    AgencyLocation = (from al in a.Locations
                                      join l in _context.Location on al.LocationId equals l.Id
                                      join c in _context.City on l.CityId equals c.Id
                                      join province in _context.Province on c.ProvinceId equals province.Id
                                      select $"{l.Address} {c.Value} {province.Code} {l.PostalCode}").FirstOrDefault(),
                    WorkerFullName = wp.FirstName + " " + wp.MiddleName + " " + wp.LastName + " " + wp.SecondLastName,
                    WorkerEmail = wpu.Email,
                    CreatedAt = ps.CreatedAt,
                    PaymentDate = ps.PaymentDate,
                    StartDate = ps.DateWorkBegins,
                    EndDate = ps.DateWorkEnd,
                    TypeOfJob = ps.TypeOfWork,
                    Holiday = ps.PublicHolidayPay,
                    Gross = ps.GrossPayment,
                    Vacations = ps.Vacations,
                    Earnings = ps.TotalEarnings,
                    DeductionCpp = ps.Cpp,
                    DeductionEi = ps.Ei,
                    DeductionTax = ps.FederalTax,
                    DeductionProvincialTax = ps.ProvincialTax,
                    DeductionOthers = ps.OtherDeductions,
                    DeductionTotal = ps.TotalDeductions,
                    TotalNet = ps.TotalPaid,
                    Items = ps.Items.Select(psi => new PayStubDetailItemModel(psi.Description, psi.Quantity, psi.UnitPrice, psi.Total)).ToList(),
                    OtherDeductionsDetail = ps.OtherDeductionsDetail.Select(d => new PayStubDetailItemModel
                    {
                        Total = d.Total,
                        Description = d.Description
                    }).ToList(),
                    FederalCategory = wp.WorkerProfileTaxCategory != null && wp.WorkerProfileTaxCategory.FederalCategory.HasValue ? wp.WorkerProfileTaxCategory.FederalCategory.Value : TaxCategory.Cc1,
                    ProvincialCategory = wp.WorkerProfileTaxCategory != null && wp.WorkerProfileTaxCategory.ProvincialCategory.HasValue ? wp.WorkerProfileTaxCategory.ProvincialCategory.Value : TaxCategory.Cc1
                }).SingleOrDefaultAsync();
    }

    public async Task<RegularWageWorker> GetWorkerRegularWages(ParamsToGetRegularWages p)
    {
        var queryable = from ps1 in _context.PayStub.Where(s => s.WorkerProfileId == p.ProfileId && s.DateWorkEnd.Date >= p.Start && s.DateWorkEnd.Date <= p.End)
                        join wp1 in _context.WorkerProfile on ps1.WorkerProfileId equals wp1.Id
                        group ps1 by ps1.WorkerProfileId into tmp1
                        select new
                        {
                            RegularWage = tmp1.Sum(ps => ps.RegularWage),
                            Vacations = tmp1.Sum(ps => ps.Vacations)
                        };
        var result = queryable.Select(w => new RegularWageWorker
        {
            RegularWage = w.RegularWage,
            Vacations = w.Vacations,
            HolidayWasPaid = (from ps2 in _context.PayStub.Where(s => s.WorkerProfileId == p.ProfileId)
                              join psh in _context.PayStubPublicHolidays.Where(h => h.Holiday == p.Holiday) on ps2.Id equals psh.PayStubId
                              select psh.Holiday).Any(),
            CustomPublicHolidayValue = (from wph in _context.WorkerProfileHoliday.Where(ph => ph.WorkerProfileId == p.ProfileId)
                                        join h in _context.Holiday.Where(hw => hw.Date.Date == p.Holiday.Date) on wph.HolidayId equals h.Id
                                        select wph.StatPaidWorker).FirstOrDefault(),
            IsEntitledToReceiveHolidayPay = (from wp2 in _context.WorkerProfile.Where(pr => pr.Id == p.ProfileId)
                                             join wr2 in _context.WorkerRequest on wp2.WorkerId equals wr2.WorkerId
                                             join ts in _context.TimeSheet.Where(s => p.RangeOfDaysWorkerMustWorkToReceiveHolidayPay.Contains(s.Date.Date)) on wr2.Id equals ts.WorkerRequestId
                                             select ts.Date).Any()
        });
        return await result.SingleOrDefaultAsync();
    }

    public Task<List<PayStubDeleteWarningListModel>> GetPayStubs(Guid invoiceId) =>
        (from i in _context.Invoice.Where(i => i.Id == invoiceId)
         join it in _context.InvoiceTotals on i.Id equals it.InvoiceId
         join tst in _context.TimeSheetTotal on it.TimeSheetTotalId equals tst.Id
         join tstP in _context.TimeSheetTotalPayroll on tst.TimeSheetId equals tstP.TimeSheetId
         join psw in _context.PayStubWageDetail on tstP.Id equals psw.TimeSheetTotalId
         join ps in _context.PayStub on psw.PayStubId equals ps.Id
         group ps by new { ps.Id, ps.PayStubNumber }
            into result
         select new { result.Key.PayStubNumber, PayStubId = result.Key.Id })
        .Select(a => new PayStubDeleteWarningListModel
        {
            PayStubId = a.PayStubId,
            PayStubNumber = a.PayStubNumber
        })
        .ToListAsync();

    public Task<PaginatedList<WeeklyPayrollModel>> GetWeeklyPayrollGroupByPaymentDate(IEnumerable<Guid> agencyIds, Pagination pagination)
    {
        var query = from ps in _context.PayStub
                    join wp in _context.WorkerProfile on ps.WorkerProfileId equals wp.Id
                    where agencyIds.Contains(wp.AgencyId)
                    group new { ps.PaymentDate, ps.TotalPaid } by ps.PaymentDate.Date into temp
                    orderby temp.Key descending
                    select new WeeklyPayrollModel
                    {
                        TotalNet = temp.Sum(t => t.TotalPaid),
                        WeekEnding = temp.Key,
                        NumberOfPayStubs = temp.Count()
                    };
        return query.ToPaginatedList(pagination);
    }

    public Task<List<WeeklyPayStubModel>> GetWeeklyPayrollDetailByPaymentDate(DateTime paymentDate)
    {
        return (from ps in _context.PayStub.Where(s => s.PaymentDate.Date == paymentDate.Date)
                join wp in _context.WorkerProfile on ps.WorkerProfileId equals wp.Id
                join wpu in _context.User on wp.WorkerId equals wpu.Id
                orderby ps.PayStubNumberId
                select new WeeklyPayStubModel
                {
                    PayStubNumber = ps.PayStubNumber,
                    Email = wpu.Email,
                    FullName = wp.FirstName + " " + wp.MiddleName + " " + wp.LastName + " " + wp.SecondLastName,
                    GrossPayment = ps.GrossPayment,
                    Vacations = ps.Vacations,
                    PublicHoliday = ps.PublicHolidayPay,
                    TotalEarnings = ps.TotalEarnings,
                    Cpp = ps.Cpp,
                    Ei = ps.Ei,
                    FederalTax = ps.FederalTax,
                    ProvincialTax = ps.ProvincialTax,
                    OtherDeductions = ps.OtherDeductions,
                    OtherDeductionsDetail = ps.OtherDeductionsDetail.Select(od => od.Description).ToList(),
                    TotalDeductions = ps.TotalDeductions,
                    TotalPaid = ps.TotalPaid,
                    WeedEnding = ps.WeekEnding,
                    PaymentDate = ps.PaymentDate,
                    Items = ps.Items.Select(i => new WeeklyPayStubItemModel
                    {
                        Description = i.Description,
                        Quantity = i.Quantity,
                        Total = i.Total,
                        UnitPrice = i.UnitPrice
                    }).OrderBy(d => d.Description).ToList(),
                    Companies = (from wd in ps.WageDetails
                                 join tst in _context.TimeSheetTotalPayroll on wd.TimeSheetTotalId equals tst.Id
                                 join ts in _context.TimeSheet on tst.TimeSheetId equals ts.Id
                                 join wr in _context.WorkerRequest on ts.WorkerRequestId equals wr.Id
                                 join r in _context.Request on wr.RequestId equals r.Id
                                 join cp in _context.CompanyProfile on new { cpId = r.CompanyId, aId = r.AgencyId } equals new { cpId = cp.CompanyId, aId = cp.AgencyId }
                                 select cp.FullName).Distinct().ToList()
                }).ToListAsync();
    }

    public async Task<IReadOnlyList<string>> Delete(IEnumerable<Guid> payStubsId)
    {
        if (payStubsId is null || !payStubsId.Any()) return Array.Empty<string>();
        var payStubs = await _context.PayStub.Where(s => payStubsId.Contains(s.Id))
            .Include(i => i.WageDetails)
            .ToListAsync();
        if (payStubs is null || !payStubs.Any()) return Array.Empty<string>();

        var timeSheetTotal = await (from ps in _context.PayStub.Where(psW => payStubsId.Contains(psW.Id))
                                    join psw in _context.PayStubWageDetail on ps.Id equals psw.PayStubId
                                    join tstP in _context.TimeSheetTotalPayroll on psw.TimeSheetTotalId equals tstP.Id
                                    select tstP).ToListAsync();

        _context.PayStub.RemoveRange(payStubs);
        foreach (PayStub ps in payStubs) _context.PayStubWageDetail.RemoveRange(ps.WageDetails);
        _context.TimeSheetTotalPayroll.RemoveRange(timeSheetTotal);
        return payStubs.Select(c => c.PayStubNumber).ToList();
    }

    public async Task Create<T>(T entity) where T : class => await _context.Set<T>().AddAsync(entity);

    public async Task<IEnumerable<PayStubT4Model>> GetPayStubsByDates(DateTime startDate, DateTime endDate)
    {
        var query = _context.PayStub
            .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate)
            .Select(p => new
            {
                WorkerProfileId = p.WorkerProfile.Id,
                p.WorkerProfile.FirstName,
                p.WorkerProfile.MiddleName,
                p.WorkerProfile.LastName,
                p.WorkerProfile.SecondLastName,
                Sin = p.WorkerProfile.SocialInsurance,
                p.WorkerProfile.Location.Address,
                City = p.WorkerProfile.Location.City.Value,
                ProvinceCode = p.WorkerProfile.Location.City.Province.Code,
                p.WorkerProfile.Location.PostalCode,
                Phone = p.WorkerProfile.Phone ?? p.WorkerProfile.MobileNumber,
                p.PayStubNumber,
                p.PaymentDate,
                p.TotalEarnings,
                p.Cpp,
                p.Ei,
                p.FederalTax,
                p.ProvincialTax,
                p.OtherDeductions
            });
        var result = await query.GroupBy(p => p.WorkerProfileId).Select(p => new PayStubT4Model
        {
            FirstName = p.FirstOrDefault().FirstName,
            MiddleName = p.FirstOrDefault().MiddleName,
            LastName = p.FirstOrDefault().LastName,
            SecondLastName = p.FirstOrDefault().SecondLastName,
            Sin = p.FirstOrDefault().Sin,
            Address = p.FirstOrDefault().Address,
            City = p.FirstOrDefault().City,
            ProvinceCode = p.FirstOrDefault().ProvinceCode,
            PostalCode = p.FirstOrDefault().PostalCode,
            Phone = p.FirstOrDefault().Phone,
            Items = p.Select(i => new PayStubT4Base
            {
                PayStubNumber = i.PayStubNumber,
                DatePaid = i.PaymentDate,
                TotalEarnings = i.TotalEarnings,
                Employer = new PayStubT4Tax
                {
                    Cpp = i.Cpp,
                    EI = i.Ei * rates.EmployerInsurance,
                    OtherDeductions = i.OtherDeductions
                },
                Employee = new PayStubT4Tax
                {
                    Cpp = i.Cpp,
                    EI = i.Ei,
                    FederalTax = i.FederalTax,
                    ProvincialTax = i.ProvincialTax
                }
            })
        }).OrderBy(p => p.FirstName + p.MiddleName + p.LastName + p.SecondLastName).ToListAsync();
        return result;
    }

    public Task SaveChangesAsync() => _context.SaveChangesAsync();

    private IQueryable<PayStubListModel> GetPayStubsQuery(IEnumerable<Guid> agencyIds, GetPayStubsFilter filter)
    {
        var query = from ps in _context.PayStub.Where(p => agencyIds.Contains(p.WorkerProfile.AgencyId))
                    select new PayStubListModel
                    {
                        Id = ps.Id,
                        NumberId = ps.NumberId,
                        PayStubNumberId = ps.PayStubNumberId,
                        PayStubNumber = ps.PayStubNumber,
                        WorkerFullName =
                            ps.WorkerProfile.FirstName +
                            (string.IsNullOrWhiteSpace(ps.WorkerProfile.MiddleName) ? string.Empty : " " + ps.WorkerProfile.MiddleName) +
                            " " + ps.WorkerProfile.LastName +
                            (string.IsNullOrWhiteSpace(ps.WorkerProfile.SecondLastName) ? string.Empty : " " + ps.WorkerProfile.SecondLastName),
                        CreatedAt = ps.CreatedAt,
                        TotalPaid = ps.TotalPaid,
                        CanDelete = !ps.WageDetails.Any()
                    };
        var predicateNew = ApplyFilterPayStubs(filter);
        query = query.Where(predicateNew);
        query = ApplySortPayStubs(query, filter);
        return query;
    }

    private Expression<Func<PayStubListModel, bool>> ApplyFilterPayStubs(GetPayStubsFilter filter)
    {
        var predicate = PredicateBuilder.New<PayStubListModel>(true);
        if (!string.IsNullOrWhiteSpace(filter.PayStubNumber))
            predicate = predicate.And(p => p.PayStubNumber.Contains(filter.PayStubNumber));
        if (filter.CreatedAtFrom.HasValue && filter.CreatedAtTo.HasValue)
            predicate = predicate.And(i => i.CreatedAt >= filter.CreatedAtFrom.Value && i.CreatedAt <= filter.CreatedAtTo.Value);
        if (!string.IsNullOrWhiteSpace(filter.WorkerFullName))
        {
            var fullName = filter.WorkerFullName.ToLower();
            predicate = predicate.And(p => EF.Functions.Like(p.WorkerFullName.ToLower(), $"%{fullName}%"));
        }
        return predicate;
    }

    private IQueryable<PayStubListModel> ApplySortPayStubs(IQueryable<PayStubListModel> query, GetPayStubsFilter filter)
    {
        switch (filter.SortBy)
        {
            case GetPayStubsFilterSortBy.PayStubNumber:
                query = query.AddOrderBy(filter, p => p.PayStubNumberId);
                break;
            case GetPayStubsFilterSortBy.CreatedAt:
                query = query.AddOrderBy(filter, p => p.CreatedAt);
                break;
            case GetPayStubsFilterSortBy.WorkerFullName:
                query = query.AddOrderBy(filter, p => p.WorkerFullName);
                break;
        }
        return query;
    }
}