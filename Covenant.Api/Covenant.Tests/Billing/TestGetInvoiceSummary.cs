using Covenant.Common.Constants;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Accounting.Invoice;
using Covenant.Common.Entities.Company;
using Covenant.Common.Entities.Request;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Repositories.Accounting;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Covenant.Tests.Billing
{
    public class TestGetInvoiceSummary
    {
        private static readonly CompanyProfile FakeProfile = new CompanyProfile
        {
            Company = new User(CvnEmail.Create("abc@mail.com").Value),
            Phone = "6478888888",
            Agency = new Covenant.Common.Entities.Agency.Agency { HstNumber = "TS123", Logo = new CovenantFile("logo.png") },
            Locations = new List<CompanyProfileLocation> { new CompanyProfileLocation { IsBilling = true, Location = new Location { Address = "4917", City = new City { Province = new Province { Country = new Country() } } } } }
        };

        private static readonly CompanyProfileInvoiceNotes InvoiceNotes = new CompanyProfileInvoiceNotes(FakeProfile.Id, "Pay before Saturday");
        private readonly List<Covenant.Common.Entities.Request.TimeSheetTotal> _fakeTimeSheetTotal;
        private readonly CovenantContext _context;

        private static readonly CompanyProfileJobPositionRate PositionRate = new CompanyProfileJobPositionRate
        {
            CompanyProfile = FakeProfile
        };

        private static readonly Covenant.Common.Entities.Request.Request FakeRequest = Covenant.Common.Entities.Request.Request.AgencyCreateRequest(FakeProfile.Agency.Id, FakeProfile.CompanyId, new Location { City = new City { Province = new Province { Country = new Country() } } }, new DateTime(2019, 01, 01), PositionRate.Id).Value;

        public TestGetInvoiceSummary()
        {
            DbContextOptions<CovenantContext> options =
                new DbContextOptionsBuilder<CovenantContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;
            _context = new CovenantContext(options);
            _fakeTimeSheetTotal = SaveFakeTimeSheet(_context);
            FakeRequest.UpdateJobTitle("General Labour");
        }

        [Fact]
        public async Task GetInvoiceSummaryById()
        {
            var invoice = FakeInvoice();
            await _context.CompanyProfile.AddAsync(FakeProfile);
            await _context.CompanyProfileJobPositionRate.AddAsync(PositionRate);
            await _context.CompanyProfileInvoiceNotes.AddAsync(InvoiceNotes);
            await _context.Invoice.AddAsync(invoice);
            await _context.SaveChangesAsync();

            var sub = new InvoiceRepository(_context);
            var invoiceSummary = await sub.GetInvoiceSummaryById(invoice.Id);
            Assert.NotNull(invoiceSummary);
            var summaryModel = invoiceSummary;
            Assert.Equal(invoice.Id, summaryModel.Id);
            Assert.Equal(invoice.NumberId, summaryModel.NumberId);
            Assert.Equal(Invoice.BuildInvoiceNumber(invoice.InvoiceNumber, invoice.CreatedAt), summaryModel.InvoiceNumber);
            Assert.Equal(new DateOnly(invoice.CreatedAt.Year, invoice.CreatedAt.Month, invoice.CreatedAt.Day), summaryModel.CreatedAt);
            Assert.Equal(invoice.CompanyId, summaryModel.CompanyProfileId);
            Assert.Equal(FakeProfile.FullName, summaryModel.CompanyFullName);
            Assert.Equal(FakeProfile.Company.Email, summaryModel.Email);
            Assert.Equal(FakeProfile.Locations.First().Location.FormattedAddress, summaryModel.Address);
            Assert.Equal(InvoiceNotes.HtmlNotes, summaryModel.HtmlNotes);
            Assert.Equal(FakeProfile.Fax, summaryModel.Fax);
            Assert.Equal(FakeProfile.FaxExt, summaryModel.FaxExt);
            Assert.Equal(FakeProfile.Agency.HstNumber, summaryModel.HstNumber);
            Assert.Equal(FakeProfile.Phone, summaryModel.PhonePrincipal);
            Assert.Equal(FakeProfile.PhoneExt, summaryModel.PhonePrincipalExt);
            Assert.Equal(FakeProfile.Agency.FullName, summaryModel.AgencyFullName);
            Assert.Equal(FakeProfile.Agency.Logo.FileName, summaryModel.AgencyLogoFileName);
            Assert.Equal(invoice.SubTotal, summaryModel.SubTotal);
            Assert.Equal(invoice.Hst, summaryModel.Hst);
            Assert.Equal(invoice.TotalNet, summaryModel.Total);
            Assert.Equal(invoice.WeekEnding, summaryModel.WeedEnding);
            Assert.Equal(invoice.Discounts.Count(), summaryModel.Discounts.Count);
            Assert.Contains(invoice.Discounts, id =>
                summaryModel.Discounts.Any(isd =>
                    isd.Quantity.Equals(id.Quantity) &&
                    isd.UnitPrice == id.UnitPrice &&
                    isd.Amount == id.Amount &&
                    isd.Description == id.Description));
            Assert.Equal(invoice.Holidays.Count(), summaryModel.Holidays.Count);
            Assert.Contains(invoice.Holidays, ih =>
                summaryModel.Holidays.Any(sih => sih.Amount == ih.Amount
                                                 && sih.Hours.Equals(ih.Hours)
                                                 && sih.UnitPrice == ih.UnitPrice
                                                 && sih.Description.Contains(ih.Holiday.ToString("D"))));
            Assert.Equal(invoice.AdditionalItems.Count(), summaryModel.AdditionalItems.Count);
            Assert.Contains(invoice.AdditionalItems, ai =>
                summaryModel.AdditionalItems.Any(isa => isa.Description == ai.Description
                && isa.Quantity.Equals(ai.Quantity)
                && isa.Total == ai.Total
                && isa.UnitPrice == ai.UnitPrice));

            Assert.NotEmpty(summaryModel.Items);
            Assert.All(summaryModel.Items, s => Assert.Contains(FakeRequest.JobTitle, s.Description));

            var regular = summaryModel.Items.Single(w => w.Description.EndsWith(CovenantConstants.Invoice.RegularLabel));
            Assert.Equal(3, regular.Quantity);
            Assert.Equal(3, regular.Total);
            Assert.Equal(1, regular.UnitPrice);

            var overtime = summaryModel.Items.Single(w => w.Description.EndsWith($"/ {CovenantConstants.Invoice.OvertimeLabel}"));
            Assert.Equal(2, overtime.Quantity);
            Assert.Equal(2, overtime.Total);
            Assert.Equal(1, overtime.UnitPrice);

            var holiday = summaryModel.Items.Single(w => w.Description.EndsWith(CovenantConstants.Invoice.HolidayLabel));
            Assert.Equal(3, holiday.Quantity);
            Assert.Equal(3, holiday.Total);
            Assert.Equal(1, holiday.UnitPrice);

            var missing = summaryModel.Items.Single(w => w.Description.EndsWith(CovenantConstants.Invoice.MissingLabel));
            Assert.Equal(4, missing.Quantity);
            Assert.Equal(4, missing.Total);
            Assert.Equal(1, missing.UnitPrice);

            var missingOvertime = summaryModel.Items.Single(w => w.Description.EndsWith(CovenantConstants.Invoice.MissingOvertimeLabel));
            Assert.Equal(5, missingOvertime.Quantity);
            Assert.Equal(5, missingOvertime.Total);
            Assert.Equal(1, missingOvertime.UnitPrice);

            var missingDifferentRate = summaryModel.Items.Single(w => w.Description.EndsWith(CovenantConstants.Invoice.MissingDifferentRateLabel));
            Assert.Equal(6, missingDifferentRate.Quantity);
            Assert.Equal(6, missingDifferentRate.Total);
            Assert.Equal(1, missingDifferentRate.UnitPrice);

            var missingOvertimeDifferentRate = summaryModel.Items.Single(w => w.Description.EndsWith(CovenantConstants.Invoice.MissingOvertimeDifferentRateLabel));
            Assert.Equal(7, missingOvertimeDifferentRate.Quantity);
            Assert.Equal(7, missingOvertimeDifferentRate.Total);
            Assert.Equal(1, missingOvertimeDifferentRate.UnitPrice);

            var nightShift = summaryModel.Items.Single(w => w.Description.EndsWith(CovenantConstants.Invoice.NightShiftLabel));
            Assert.Equal(8, nightShift.Quantity);
            Assert.Equal(8, nightShift.Total);
            Assert.Equal(1, nightShift.UnitPrice);
        }

        private Invoice FakeInvoice()
        {
            var invoice = new Invoice(FakeProfile.Id, 1,
                    1, 1, 1, 1, 1, 1, 1, 1, 1)
            { CreatedAt = new DateTime(2019, 01, 01), WeekEnding = new DateTime(2019, 01, 07) };

            invoice.AddDiscounts(new[] { new InvoiceDiscount(1, 1, "One"), new InvoiceDiscount(1, 2, "Two") });
            invoice.AddHolidays(new[]{
                 InvoiceHoliday.Create(new DateTime(2019,01,02),8,80).Value,
                 InvoiceHoliday.Create(new DateTime(2019,01,03),9,90).Value});
            invoice.AddAdditionalItems(new[]{new InvoiceAdditionalItem(1,2,"One"),
                new InvoiceAdditionalItem(2,4,"Two") });

            // Create InvoiceTotals with Description, Quantity, and Total for test expectations
            var invoiceTotals = new List<InvoiceTotal>
            {
                new InvoiceTotal
                {
                    TimeSheetTotalId = _fakeTimeSheetTotal[0].Id,
                    Description = $"Charge for {FakeRequest.JobTitle} / {CovenantConstants.Invoice.RegularLabel}",
                    Quantity = 3,
                    Total = 3,
                    UnitPrice = 1,
                    AgencyRate = 1
                },
                new InvoiceTotal
                {
                    TimeSheetTotalId = _fakeTimeSheetTotal[1].Id,
                    Description = $"Charge for {FakeRequest.JobTitle} / {CovenantConstants.Invoice.OvertimeLabel}",
                    Quantity = 2,
                    Total = 2,
                    UnitPrice = 1,
                    AgencyRate = 1
                },
                new InvoiceTotal
                {
                    TimeSheetTotalId = _fakeTimeSheetTotal[2].Id,
                    Description = $"Charge for {FakeRequest.JobTitle} / {CovenantConstants.Invoice.HolidayLabel}",
                    Quantity = 3,
                    Total = 3,
                    UnitPrice = 1,
                    AgencyRate = 1
                },
                new InvoiceTotal
                {
                    TimeSheetTotalId = _fakeTimeSheetTotal[3].Id,
                    Description = $"Charge for {FakeRequest.JobTitle} / {CovenantConstants.Invoice.MissingLabel}",
                    Quantity = 4,
                    Total = 4,
                    UnitPrice = 1,
                    AgencyRate = 1
                },
                new InvoiceTotal
                {
                    TimeSheetTotalId = _fakeTimeSheetTotal[4].Id,
                    Description = $"Charge for {FakeRequest.JobTitle} / {CovenantConstants.Invoice.MissingOvertimeLabel}",
                    Quantity = 5,
                    Total = 5,
                    UnitPrice = 1,
                    AgencyRate = 1
                },
                new InvoiceTotal
                {
                    TimeSheetTotalId = _fakeTimeSheetTotal[5].Id,
                    Description = $"Charge for {FakeRequest.JobTitle} / {CovenantConstants.Invoice.MissingDifferentRateLabel}",
                    Quantity = 6,
                    Total = 6,
                    UnitPrice = 1,
                    AgencyRate = 1
                },
                new InvoiceTotal
                {
                    TimeSheetTotalId = _fakeTimeSheetTotal[6].Id,
                    Description = $"Charge for {FakeRequest.JobTitle} / {CovenantConstants.Invoice.MissingOvertimeDifferentRateLabel}",
                    Quantity = 7,
                    Total = 7,
                    UnitPrice = 1,
                    AgencyRate = 1
                },
                new InvoiceTotal
                {
                    TimeSheetTotalId = _fakeTimeSheetTotal[7].Id,
                    Description = $"Charge for {FakeRequest.JobTitle} / {CovenantConstants.Invoice.NightShiftLabel}",
                    Quantity = 8,
                    Total = 8,
                    UnitPrice = 1,
                    AgencyRate = 1
                }
            };

            invoice.AddInvoiceTotals(invoiceTotals);
            return invoice;
        }

        private static List<Covenant.Common.Entities.Request.TimeSheetTotal> SaveFakeTimeSheet(CovenantContext context)
        {
            var worker = new User(CvnEmail.Create("wor@mail.com").Value);
            var workerRequest = Covenant.Common.Entities.Request.WorkerRequest.AgencyBook(worker.Id, FakeRequest.Id);

            TimeSheet tRegular = FakeTimeSheet(workerRequest.Id);
            var tstRegular = Covenant.Common.Entities.Request.TimeSheetTotal.CreateTotal(tRegular.Id, TimeSpan.FromHours(1),
                TimeSpan.FromHours(1), TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero);

            TimeSheet tOtherRegular = FakeTimeSheet(workerRequest.Id);
            var tstOtherRegular = Covenant.Common.Entities.Request.TimeSheetTotal.CreateTotalWithOtherRegular(tOtherRegular.Id,
                TimeSpan.FromHours(1), TimeSpan.FromHours(1),
                TimeSpan.FromHours(1), TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero);

            TimeSheet tOvertime = FakeTimeSheet(workerRequest.Id);
            var tstOvertime = Covenant.Common.Entities.Request.TimeSheetTotal.CreateTotal(tOvertime.Id,
                TimeSpan.FromHours(2),
                TimeSpan.Zero, TimeSpan.FromHours(2), TimeSpan.Zero, TimeSpan.Zero);

            TimeSheet tHoliday = FakeTimeSheet(workerRequest.Id);
            var tstHoliday = Covenant.Common.Entities.Request.TimeSheetTotal.CreateTotalForHoliday(tHoliday.Id,
                TimeSpan.FromHours(3), TimeSpan.Zero);

            TimeSheet tMissing = FakeTimeSheet(workerRequest.Id);
            tMissing.MissingHours = TimeSpan.FromHours(4);
            var tstMissing = Covenant.Common.Entities.Request.TimeSheetTotal.CreateTotal(tMissing.Id,
                TimeSpan.Zero,
                TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero);

            TimeSheet tMissingOvertime = FakeTimeSheet(workerRequest.Id);
            tMissingOvertime.MissingHoursOvertime = TimeSpan.FromHours(5);
            var tstMissingOvertime = Covenant.Common.Entities.Request.TimeSheetTotal.CreateTotal(tMissingOvertime.Id,
                TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero,
                TimeSpan.Zero, TimeSpan.Zero);

            TimeSheet tMissingDifferentRate = FakeTimeSheet(workerRequest.Id);
            tMissingDifferentRate.MissingHours = TimeSpan.FromHours(6);
            tMissingDifferentRate.MissingRateAgency = 10;
            var tstMissingDifferentRate = Covenant.Common.Entities.Request.TimeSheetTotal.CreateTotal(tMissingDifferentRate.Id,
                TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero,
                TimeSpan.Zero, TimeSpan.Zero);

            TimeSheet tMissingOvertimeDifferentRate = FakeTimeSheet(workerRequest.Id);
            tMissingOvertimeDifferentRate.MissingHoursOvertime = TimeSpan.FromHours(7);
            tMissingOvertimeDifferentRate.MissingRateAgency = 10;
            var tstMissingOvertimeDifferentRate = Covenant.Common.Entities.Request.TimeSheetTotal.CreateTotal(tMissingOvertimeDifferentRate.Id,
                TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero,
                TimeSpan.Zero, TimeSpan.Zero);

            TimeSheet tNightShift = FakeTimeSheet(workerRequest.Id);
            var tstNightShift = Covenant.Common.Entities.Request.TimeSheetTotal.CreateTotal(tNightShift.Id,
                TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero,
                TimeSpan.FromHours(8), TimeSpan.Zero);

            context.Request.Add(FakeRequest);
            context.User.Add(worker);
            context.WorkerRequest.Add(workerRequest);
            context.TimeSheet.AddRange(tRegular, tOtherRegular, tOvertime, tHoliday, tMissing,
                tMissingOvertime, tMissingDifferentRate, tMissingOvertimeDifferentRate, tNightShift);
            context.TimeSheetTotal.AddRange(tstRegular, tstOtherRegular, tstOvertime, tstHoliday, tstMissing,
                tstMissingOvertime, tstMissingDifferentRate, tstMissingOvertimeDifferentRate, tstNightShift);
            context.SaveChanges();
            return context.TimeSheetTotal.Include(ts => ts.TimeSheet).ToList();
        }

        private static TimeSheet FakeTimeSheet(Guid workerRequestId) =>
             TimeSheet.CreateTimeSheet(workerRequestId, new DateTime(2019, 01, 01),
                TimeSpan.FromHours(12), now: new DateTime(2019, 01, 01)).Value;
    }
}