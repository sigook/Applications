using Covenant.Common.Configuration;
using Covenant.Common.Entities.Accounting.PayStub;
using Covenant.Common.Enums;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Repositories.Accounting;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Linq.Expressions;

namespace Covenant.Infrastructure.Repositories.Accounting;

public class PayStubRepository(Rates rates, CovenantContext context) : IPayStubRepository
{
    public virtual Task<List<NextNumberModel>> GetNextPayStubNumbers(int limit) =>
        context.NextNumber.FromSqlRaw(SqlQueries.GetNextPayStubNumbers, new NpgsqlParameter("limit", limit)).ToListAsync();

    public Task<bool> IsPayStubNumberTaken(long payStubNumber) =>
        context.PayStubs.AnyAsync(s => s.PayStubNumberId == payStubNumber);

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
        return (from ps in context.PayStubs.Where(s => s.Id == payStubId)
                select new PayStubDetailModel
                {
                    Id = ps.Id,
                    NumberId = ps.NumberId,
                    PayrollNumberId = ps.PayStubNumberId,
                    PayrollNumber = ps.PayStubNumber,
                    AgencyFullName = ps.WorkerProfile.Agency.FullName,
                    AgencyPhone = ps.WorkerProfile.Agency.PhonePrincipal,
                    AgencyPhoneExt = ps.WorkerProfile.Agency.PhonePrincipalExt,
                    AgencyLogoFileName = ps.WorkerProfile.Agency.Logo == null ? null : ps.WorkerProfile.Agency.Logo.FileName,
                    AgencyLocation = (from al in ps.WorkerProfile.Agency.Locations
                                      select $"{al.Location.Address} {al.Location.City.Value} {al.Location.City.Province.Code} {al.Location.PostalCode}").FirstOrDefault(),
                    WorkerFullName = ps.WorkerProfile.FirstName + " " + ps.WorkerProfile.MiddleName + " " + ps.WorkerProfile.LastName + " " + ps.WorkerProfile.SecondLastName,
                    SinNumber = ps.WorkerProfile.SocialInsurance,
                    EmployeeId = ps.WorkerProfile.NumberId,
                    WorkerEmail = ps.WorkerProfile.Worker.Email,
                    CreatedAt = ps.CreatedAt,
                    PaymentDate = ps.PaymentDate,
                    StartDate = ps.DateWorkBegins,
                    EndDate = ps.DateWorkEnd,
                    Position = ps.Position,
                    Gross = ps.GrossPayment,
                    Vacations = ps.Vacations,
                    Earnings = ps.TotalEarnings,
                    DeductionCpp = ps.Cpp,
                    DeductionEi = ps.Ei,
                    DeductionTax = ps.FederalTax,
                    DeductionProvincialTax = ps.ProvincialTax,
                    DeductionTotal = ps.TotalDeductions,
                    TotalNet = ps.TotalPaid,
                    Items = ps.Items.OrderBy(psi => psi.Type).Select(psi => new PayStubDetailItemModel(psi.Description, psi.Quantity, psi.UnitPrice, psi.Total, psi.Type)).ToList(),
                    OtherDeductions = ps.OtherDeductions.Select(d => new PayStubDetailItemModel
                    {
                        Total = d.Total,
                        Description = d.Description
                    }).ToList(),
                    FederalCategory = ps.WorkerProfile.WorkerProfileTaxCategory != null && ps.WorkerProfile.WorkerProfileTaxCategory.FederalCategory.HasValue ? ps.WorkerProfile.WorkerProfileTaxCategory.FederalCategory.Value : TaxCategory.Cc1,
                    ProvincialCategory = ps.WorkerProfile.WorkerProfileTaxCategory != null && ps.WorkerProfile.WorkerProfileTaxCategory.ProvincialCategory.HasValue ? ps.WorkerProfile.WorkerProfileTaxCategory.ProvincialCategory.Value : TaxCategory.Cc1,
                    WorkerProfileId = ps.WorkerProfileId
                }).SingleOrDefaultAsync();
    }

    public async Task<PayStubYtdModel> GetYtdSummary(Guid workerProfileId, int year)
    {
        var result = await context.PayStubs
            .Where(ps => ps.WorkerProfileId == workerProfileId && ps.DateWorkEnd.Year == year)
            .GroupBy(ps => ps.WorkerProfileId)
            .Select(g => new PayStubYtdModel
            {
                Gross = g.Sum(ps => ps.GrossPayment),
                Vacations = g.Sum(ps => ps.Vacations),
                Earnings = g.Sum(ps => ps.TotalEarnings),
                Cpp = g.Sum(ps => ps.Cpp),
                Ei = g.Sum(ps => ps.Ei),
                FederalTax = g.Sum(ps => ps.FederalTax),
                ProvincialTax = g.Sum(ps => ps.ProvincialTax),
                TotalDeductions = g.Sum(ps => ps.TotalDeductions),
                TotalPaid = g.Sum(ps => ps.TotalPaid)
            })
            .SingleOrDefaultAsync();

        return result ?? new PayStubYtdModel();
    }

    public async Task<RegularWageWorker> GetWorkerRegularWages(Guid workerProfileId, DateTime holiday, IEnumerable<DateTime> qualifyingDays)
    {
        var workerProfileExists = await context.WorkerProfiles.AnyAsync(wp => wp.Id == workerProfileId);
        if (!workerProfileExists) return null;

        var holidayWasPaid = await context.PayStubPublicHolidays
            .AnyAsync(psh => psh.Holiday == holiday && psh.PayStub.WorkerProfileId == workerProfileId);

        var customPublicHolidayValue = await context.WorkerProfileHolidays
            .Where(wph => wph.WorkerProfileId == workerProfileId && wph.Holiday.Date.Date == holiday.Date)
            .Select(wph => wph.StatPaidWorker)
            .FirstOrDefaultAsync();

        var isEntitledToReceiveHolidayPay = await context.TimeSheets
            .AnyAsync(ts => ts.WorkerRequest.WorkerProfileId == workerProfileId
                && qualifyingDays.Contains(ts.Date.Date));

        return new RegularWageWorker
        {
            HolidayWasPaid = holidayWasPaid,
            CustomPublicHolidayValue = customPublicHolidayValue,
            IsEntitledToReceiveHolidayPay = isEntitledToReceiveHolidayPay
        };
    }

    public Task<List<PayStubDeleteWarningListModel>> GetPayStubs(Guid invoiceId) =>
        (from i in context.Invoices.Where(i => i.Id == invoiceId)
         from it in i.InvoiceTotals
         join tstP in context.TimeSheetTotalPayrolls on it.TimeSheetTotal.TimeSheetId equals tstP.TimeSheetId
         join psw in context.PayStubWageDetails on tstP.Id equals psw.TimeSheetTotalId
         group psw by new { psw.PayStub.Id, psw.PayStub.PayStubNumber }
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
        var query = from ps in context.PayStubs
                    where agencyIds.Contains(ps.WorkerProfile.AgencyId)
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
        return (from ps in context.PayStubs.Where(s => s.PaymentDate.Date == paymentDate.Date)
                orderby ps.PayStubNumberId
                select new WeeklyPayStubModel
                {
                    PayStubNumber = ps.PayStubNumber,
                    Email = ps.WorkerProfile.Worker.Email,
                    FullName = ps.WorkerProfile.FirstName + " " + ps.WorkerProfile.MiddleName + " " + ps.WorkerProfile.LastName + " " + ps.WorkerProfile.SecondLastName,
                    NumberId = ps.WorkerProfile.NumberId,
                    GrossPayment = ps.GrossPayment,
                    Vacations = ps.Vacations,
                    TotalEarnings = ps.TotalEarnings,
                    Cpp = ps.Cpp,
                    Ei = ps.Ei,
                    FederalTax = ps.FederalTax,
                    ProvincialTax = ps.ProvincialTax,
                    OtherDeductions = ps.OtherDeductions.Select(od => od.Description).ToList(),
                    TotalDeductions = ps.TotalDeductions,
                    TotalPaid = ps.TotalPaid,
                    WeedEnding = ps.WeekEnding,
                    PaymentDate = ps.PaymentDate,
                    Items = ps.Items.Select(i => new WeeklyPayStubItemModel
                    {
                        Description = i.Description,
                        Quantity = i.Quantity,
                        Total = i.Total,
                        UnitPrice = i.UnitPrice,
                        Type = i.Type
                    }).OrderBy(d => d.Type).ToList(),
                    Companies = (from wd in ps.WageDetails
                                 select wd.TimeSheetTotal.TimeSheet.WorkerRequest.Request.CompanyProfile.FullName).Distinct().ToList()
                }).ToListAsync();
    }

    public async Task<IReadOnlyList<string>> Delete(IEnumerable<Guid> payStubsId)
    {
        if (payStubsId is null || !payStubsId.Any()) return [];
        var payStubs = await context.PayStubs.Where(s => payStubsId.Contains(s.Id))
            .Include(i => i.WageDetails)
            .ToListAsync();
        if (payStubs?.Count == 0) return [];

        var timeSheetTotal = await (from ps in context.PayStubs.Where(psW => payStubsId.Contains(psW.Id))
                                    from psw in ps.WageDetails
                                    select psw.TimeSheetTotal).ToListAsync();

        context.PayStubs.RemoveRange(payStubs);
        foreach (PayStub ps in payStubs) context.PayStubWageDetails.RemoveRange(ps.WageDetails);
        context.TimeSheetTotalPayrolls.RemoveRange(timeSheetTotal);
        return [.. payStubs.Select(c => c.PayStubNumber)];
    }

    public async Task Create<T>(T entity) where T : class => await context.Set<T>().AddAsync(entity);

    public async Task<IEnumerable<PayStubT4Model>> GetPayStubsByDates(DateTime startDate, DateTime endDate)
    {
        var query = context.PayStubs
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
                p.OtherDeductions,
                CompanyName = p.WageDetails
                    .Select(w => w.TimeSheetTotal.TimeSheet.WorkerRequest.Request.JobPositionRate.CompanyProfile.FullName)
                    .FirstOrDefault()
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
                CompanyName = i.CompanyName,
                Employer = new PayStubT4Tax
                {
                    Cpp = i.Cpp,
                    EI = i.Ei * rates.EmployerInsurance,
                    OtherDeductions = i.OtherDeductions.Sum(od => od.Total)
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

    public Task SaveChangesAsync() => context.SaveChangesAsync();

    private IQueryable<PayStubListModel> GetPayStubsQuery(IEnumerable<Guid> agencyIds, GetPayStubsFilter filter)
    {
        var query = from ps in context.PayStubs.Where(p => agencyIds.Contains(p.WorkerProfile.AgencyId))
                    select new PayStubListModel
                    {
                        Id = ps.Id,
                        NumberId = ps.WorkerProfile.NumberId,
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

    private static Expression<Func<PayStubListModel, bool>> ApplyFilterPayStubs(GetPayStubsFilter filter)
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
        if (filter.NumberId.HasValue)
            predicate = predicate.And(p => p.NumberId == filter.NumberId.Value);
        return predicate;
    }

    private static IQueryable<PayStubListModel> ApplySortPayStubs(IQueryable<PayStubListModel> query, GetPayStubsFilter filter)
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
            case GetPayStubsFilterSortBy.NumberId:
                query = query.AddOrderBy(filter, p => p.NumberId);
                break;
        }
        return query;
    }
}