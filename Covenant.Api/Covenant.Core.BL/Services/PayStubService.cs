using ClosedXML.Excel;
using Covenant.Common.Configuration;
using Covenant.Common.Entities.Accounting.PayStub;
using Covenant.Common.Entities.Request;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Models.Request.TimeSheet;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Accounting;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Repositories.Worker;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Covenant.Core.BL.Services;

public class PayStubService : IPayStubService
{
    private readonly IPayStubRepository payStubRepository;
    private readonly ITimesheetCalculatorService calculatorService;
    private readonly ITimeSheetRepository timeSheetRepository;
    private readonly ICatalogRepository catalogRepository;
    private readonly Rates rates;
    private readonly TimeLimits timeLimits;
    private static readonly SemaphoreSlim SemaphoreSlim = new(1, 1);

    public PayStubService(
        IPayStubRepository payStubRepository,
        ITimesheetCalculatorService calculatorService,
        ITimeSheetRepository timeSheetRepository,
        ICatalogRepository catalogRepository,
        Rates rates,
        TimeLimits timeLimits)
    {
        this.payStubRepository = payStubRepository;
        this.calculatorService = calculatorService;
        this.timeSheetRepository = timeSheetRepository;
        this.catalogRepository = catalogRepository;
        this.rates = rates;
        this.timeLimits = timeLimits;
    }

    public async Task<Result<ResultGenerateDocument<byte[]>>> GenerateT4(DateTime from, DateTime to)
    {
        var result = await payStubRepository.GetPayStubsByDates(from, to);
        using var stream = new MemoryStream();
        using var workbook = new XLWorkbook();
        GetDetail(result, workbook);
        GetReport(result, workbook);
        workbook.SaveAs(stream);
        return Result.Ok(new ResultGenerateDocument<byte[]>(stream.ToArray(), $"T4_Resume_{DateTime.Now.Year}.xlsx", string.Empty));
    }

    private void GetReport(IEnumerable<PayStubT4Model> result, XLWorkbook workbook)
    {
        var sheet = workbook.Worksheets.Add("Report");
        var headerNames = new string[]
        {
            "No.",
            "Name",
            "Address",
            "SIN #",
            "Total Earnings",
            "CPP Employer",
            "EI Employeer",
            "Other Deductions",
            "CPP Employee",
            "EI Employee",
            "Fed Tax",
            "Prov Tax",
            "Total Tax",
            "Net Pay",
            "Total"
        };
        sheet.SetupHeaders(headerNames);
        var startAt = 2;
        for (int i = 0; i < result.Count(); i++)
        {
            var data = result.ElementAt(i);
            var totalTax = data.Items.Sum(i => i.Employee.FederalTax + i.Employee.ProvincialTax);
            var netPay = data.Items.Sum(i => i.TotalEarnings - i.Employee.Cpp - i.Employee.EI - i.Employee.FederalTax - i.Employee.ProvincialTax);
            var total = data.Items.Sum(i => i.Employer.Cpp + i.Employer.EI + i.Employee.Cpp + i.Employee.EI + i.Employee.FederalTax + i.Employee.ProvincialTax);
            sheet.Cell($"A{startAt}").SetValue(i + 1);
            sheet.Cell($"B{startAt}").SetValue(data.WorkerFullName.Trim());
            sheet.Cell($"C{startAt}").SetValue(data.Location);
            sheet.Cell($"D{startAt}").SetValue(data.Sin);
            sheet.Cell($"E{startAt}").SetValue(data.Items.Sum(i => i.TotalEarnings)).SetMoneyType();
            sheet.Cell($"F{startAt}").SetValue(data.Items.Sum(i => i.Employer.Cpp)).SetMoneyType();
            sheet.Cell($"G{startAt}").SetValue(data.Items.Sum(i => i.Employer.EI)).SetMoneyType();
            sheet.Cell($"H{startAt}").SetValue(data.Items.Sum(i => i.Employer.OtherDeductions)).SetMoneyType();
            sheet.Cell($"I{startAt}").SetValue(data.Items.Sum(i => i.Employee.Cpp)).SetMoneyType();
            sheet.Cell($"J{startAt}").SetValue(data.Items.Sum(i => i.Employee.EI)).SetMoneyType();
            sheet.Cell($"K{startAt}").SetValue(data.Items.Sum(i => i.Employee.FederalTax)).SetMoneyType();
            sheet.Cell($"L{startAt}").SetValue(data.Items.Sum(i => i.Employee.ProvincialTax)).SetMoneyType();
            sheet.Cell($"M{startAt}").SetValue(totalTax).SetMoneyType();
            sheet.Cell($"N{startAt}").SetValue(netPay).SetMoneyType();
            sheet.Cell($"O{startAt}").SetValue(total).SetMoneyType();
            startAt++;
        }
    }

    private void GetDetail(IEnumerable<PayStubT4Model> result, XLWorkbook workbook)
    {
        var sheet = workbook.Worksheets.Add("Detail");
        var startAt = 1;
        for (int i = 0; i < result.Count(); i++)
        {
            var data = result.ElementAt(i);
            GenerateDetailTemplate(sheet, data, i, ref startAt);
        }
        startAt++;
        sheet.Cell($"P{startAt}").SetValue(result.Sum(r => r.Items.Sum(i => i.TotalEarnings))).SetMoneyType();
    }

    private void GenerateDetailTemplate(IXLWorksheet sheet, PayStubT4Model data, int index, ref int row)
    {
        Action<IXLCell> contentLabel = cell =>
        {
            cell.Style.Font.SetFontName("Arial Black");
            cell.Style.Font.SetFontSize(11);
            cell.Style.Font.SetBold(true);
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        };
        Action<IXLCell> contentValue = cell =>
        {
            cell.Style.Font.SetFontName("Arial");
            cell.Style.Font.SetFontSize(11);
            cell.Style.Font.SetBold(true);
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        };
        Action<IXLCell> contentHeader = cell =>
        {
            cell.Style.Font.SetFontName("Arial");
            cell.Style.Font.SetFontSize(11);
            cell.Style.Font.SetBold(true);
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        };
        Action<IXLCell> tableHeader = cell =>
        {
            cell.WorksheetColumn().Width = 15;
            cell.Style.Font.SetFontName("Arial");
            cell.Style.Font.SetFontSize(11);
            cell.Style.Font.SetBold(true);
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
            cell.Style.Fill.BackgroundColor = XLColor.FromArgb(166, 166, 166);
            cell.Style.Font.FontColor = XLColor.White;
        };
        Action<IXLCell> tableContent = cell =>
        {
            cell.Style.Border.LeftBorder = XLBorderStyleValues.Thick;
            cell.Style.Border.RightBorder = XLBorderStyleValues.Thick;
        };
        Action<IXLCell> emptyRow = cell =>
        {
            cell.Style.Border.RightBorder = XLBorderStyleValues.Thick;
            cell.Style.Border.LeftBorder = XLBorderStyleValues.Thick;
            cell.Style.Border.BottomBorder = XLBorderStyleValues.Thick;
        };
        Action<IXLCell> tableFooter = cell =>
        {
            cell.Style.Font.SetBold(true);
            cell.Style.Border.BottomBorder = XLBorderStyleValues.Double;
        };
        var items = data.Items.OrderBy(i => i.DatePaid);
        sheet.Cell($"A{row}").SetValue(index + 1).Config(cell =>
        {
            cell.Style.Font.SetFontName("Arial Black");
            cell.Style.Font.SetFontSize(11);
            cell.Style.Font.SetBold(true);
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Fill.BackgroundColor = XLColor.FromArgb(146, 208, 80);
            cell.Style.Font.FontColor = XLColor.Black;
        });
        sheet.Cell($"B{row}").SetValue("Name:").Config(contentLabel);
        sheet.Cell($"C{row}").SetValue(data.WorkerFullName.Trim()).Config(contentValue);
        sheet.Range($"C{row}:G{row}").Row(1).Merge().Style.Border.BottomBorder = XLBorderStyleValues.Thick;
        sheet.Cell($"H{row}").SetValue("SIN #:").Config(contentLabel);
        sheet.Cell($"I{row}").SetValue(data.Sin).Config(contentValue);
        sheet.Range($"I{row}:L{row}").Row(1).Merge().Style.Border.BottomBorder = XLBorderStyleValues.Thick;
        row++;
        sheet.Cell($"B{row}").SetValue("Address:").Config(contentLabel);
        sheet.Cell($"C{row}").SetValue(data.Location).Config(contentValue);
        sheet.Range($"C{row}:L{row}").Row(1).Merge().Style.Border.BottomBorder = XLBorderStyleValues.Thick;
        row++;
        sheet.Cell($"D{row}").SetValue("Phone:").Config(contentLabel);
        sheet.Cell($"E{row}").SetValue(data.Phone).Config(contentValue);
        sheet.Range($"E{row}:G{row}").Row(1).Merge().Style.Border.BottomBorder = XLBorderStyleValues.Thick;
        row += 2;
        sheet.Cell($"F{row}").SetValue("EMPLOYER").Config(contentHeader);
        sheet.Range($"F{row}:H{row}").Row(1).Merge().Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
        sheet.Cell($"I{row}").SetValue("EMPLOYEE").Config(contentHeader);
        sheet.Range($"I{row}:L{row}").Row(1).Merge().Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
        row++;
        sheet.Cell($"B{row}").SetValue("No.").Config(tableHeader);
        sheet.Cell($"C{row}").SetValue("Paystub #").Config(tableHeader);
        sheet.Cell($"D{row}").SetValue("Date Paid").Config(tableHeader);
        sheet.Cell($"E{row}").SetValue("Total Earnings").Config(tableHeader);
        sheet.Cell($"F{row}").SetValue("CPP").Config(tableHeader);
        sheet.Cell($"G{row}").SetValue("EI").Config(tableHeader);
        sheet.Cell($"H{row}").SetValue("Others Deductions").Config(tableHeader);
        sheet.Cell($"I{row}").SetValue("CPP").Config(tableHeader);
        sheet.Cell($"J{row}").SetValue("EI").Config(tableHeader);
        sheet.Cell($"K{row}").SetValue("FED TAX").Config(tableHeader);
        sheet.Cell($"L{row}").SetValue("PROV TAX").Config(tableHeader);
        for (int i = 0; i < items.Count(); i++)
        {
            row++;
            var item = items.ElementAt(i);
            sheet.Cell($"B{row}").SetValue(i + 1).Config(tableContent);
            sheet.Cell($"C{row}").SetValue(item.PayStubNumber).Config(tableContent);
            sheet.Cell($"D{row}").SetValue(item.DatePaid.ToString("dd-MMM-yyyy"))
                .Config(tableContent)
                .Config(cell =>
                {
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                });
            sheet.Cell($"E{row}").SetValue(item.TotalEarnings).SetMoneyType().Config(tableContent);
            sheet.Cell($"F{row}").SetValue(item.Employer.Cpp).SetMoneyType().Config(tableContent);
            sheet.Cell($"G{row}").SetValue(item.Employer.EI).SetMoneyType().Config(tableContent);
            sheet.Cell($"H{row}").SetValue(item.Employer.OtherDeductions).SetMoneyType().Config(tableContent);
            sheet.Cell($"I{row}").SetValue(item.Employee.Cpp).SetMoneyType().Config(tableContent);
            sheet.Cell($"J{row}").SetValue(item.Employee.EI).SetMoneyType().Config(tableContent);
            sheet.Cell($"K{row}").SetValue(item.Employee.FederalTax).SetMoneyType().Config(tableContent);
            sheet.Cell($"L{row}").SetValue(item.Employee.ProvincialTax).SetMoneyType().Config(tableContent);
        }
        row++;
        sheet.Cell($"B{row}").Config(emptyRow);
        sheet.Cell($"C{row}").Config(emptyRow);
        sheet.Cell($"D{row}").Config(emptyRow);
        sheet.Cell($"E{row}").Config(emptyRow);
        sheet.Cell($"F{row}").Config(emptyRow);
        sheet.Cell($"G{row}").Config(emptyRow);
        sheet.Cell($"H{row}").Config(emptyRow);
        sheet.Cell($"I{row}").Config(emptyRow);
        sheet.Cell($"J{row}").Config(emptyRow);
        sheet.Cell($"K{row}").Config(emptyRow);
        sheet.Cell($"L{row}").Config(emptyRow);
        row++;
        sheet.Cell($"E{row}").SetValue(items.Sum(i => i.TotalEarnings)).SetMoneyType().Config(tableFooter);
        sheet.Cell($"F{row}").SetValue(items.Sum(i => i.Employer.Cpp)).SetMoneyType().Config(tableFooter);
        sheet.Cell($"G{row}").SetValue(items.Sum(i => i.Employer.EI)).SetMoneyType().Config(tableFooter);
        sheet.Cell($"H{row}").SetValue(items.Sum(i => i.Employer.OtherDeductions)).SetMoneyType().Config(tableFooter);
        sheet.Cell($"I{row}").SetValue(items.Sum(i => i.Employee.Cpp)).SetMoneyType().Config(tableFooter);
        sheet.Cell($"J{row}").SetValue(items.Sum(i => i.Employee.EI)).SetMoneyType().Config(tableFooter);
        sheet.Cell($"K{row}").SetValue(items.Sum(i => i.Employee.FederalTax)).SetMoneyType().Config(tableFooter);
        sheet.Cell($"L{row}").SetValue(items.Sum(i => i.Employee.ProvincialTax)).SetMoneyType().Config(tableFooter);
        sheet.Cell($"P{row}").SetValue(items.Sum(i => i.TotalEarnings)).SetMoneyType().Config(c => c.WorksheetColumn().Width = 15);
        row++;
        sheet.Cell($"L{row}").SetFormulaA1($"SUM(F{row - 1}:L{row - 1})").SetMoneyType().Config(tableFooter);
        row++;
    }

    public async Task<Result> Generate(IEnumerable<Guid> agencyIds, IEnumerable<Guid> workerIds)
    {
        await SemaphoreSlim.WaitAsync();
        try
        {
            foreach (var workerId in workerIds)
            {
                var result = await GeneratePayStubForWorker(agencyIds, workerId);
                if (!result) return result;
            }
        }
        finally
        {
            SemaphoreSlim.Release();
        }

        return Result.Ok();
    }

    private async Task<Result> GeneratePayStubForWorker(IEnumerable<Guid> agencyIds, Guid workerId)
    {
        // Step 1: Get approved timesheets for this worker
        var timesheets = await timeSheetRepository.GetTimeSheetForCreatingPayStubs(agencyIds, workerId);
        if (!timesheets.Any()) return Result.Ok();

        // Step 2: Get next paystub number
        var nextNumbers = await payStubRepository.GetNextPayStubNumbers(1);
        var payStubNumber = nextNumbers.First().NextNumber;

        var workerProfileId = timesheets.First().WorkerProfileId;
        var typeOfWork = timesheets.First().TypeOfWork;
        var overtimeStartsAfter = timesheets.First().OvertimeStartsAfter;

        // Step 3: Process timesheets by week and request to calculate hours and wages
        var allWageDetails = new List<PayStubWageDetail>();
        var publicHolidays = new List<PayStubPublicHoliday>();

        // Dictionaries to accumulate hours by rate for each item type
        var regularByRate = new Dictionary<decimal, double>();
        var otherRegularByRate = new Dictionary<decimal, double>();
        var overtimeByRate = new Dictionary<decimal, double>();
        var holidayByRate = new Dictionary<decimal, double>();
        var missingByRate = new Dictionary<decimal, double>();
        var missingOvertimeByRate = new Dictionary<decimal, double>();

        var weekGroups = timesheets.GroupBy(t => t.Week).OrderBy(g => g.Key);
        foreach (var week in weekGroups)
        {
            // Step 3.1: Group by RequestId for overtime accumulation per request
            var requestGroups = week.GroupBy(t => t.RequestId);
            foreach (var request in requestGroups)
            {
                // Step 3.2: Process each day in order
                var accumulatedHours = TimeSpan.Zero;
                foreach (var ts in request.OrderBy(t => t.Date))
                {
                    if (!ts.TimeInApproved.HasValue || !ts.TimeOutApproved.HasValue) continue;

                    // Calculate hours breakdown
                    var hoursBreakdown = calculatorService.CalculateHoursBreakdown(
                        ts.TimeInApproved.Value,
                        ts.TimeOutApproved.Value,
                        ts.DurationBreak,
                        ts.BreakIsPaid,
                        ts.IsHoliday,
                        ts.HolidayIsPaid,
                        ref accumulatedHours,
                        overtimeStartsAfter,
                        timeLimits.MaxHoursWeek);

                    // Calculate amounts for each hour type
                    var workerRate = ts.WorkerRate;
                    var missingRateWorker = ts.MissingRateWorker <= decimal.Zero ? workerRate : ts.MissingRateWorker;

                    var regularAmount = calculatorService.CalculateRegularAmount(workerRate, hoursBreakdown.RegularHours.TotalHours).DefaultMoneyRound();
                    var otherRegularAmount = calculatorService.CalculateRegularAmount(workerRate, hoursBreakdown.OtherRegularHours.TotalHours).DefaultMoneyRound();
                    var overtimeAmount = calculatorService.CalculateOvertimeAmount(workerRate, rates.OverTime, hoursBreakdown.OvertimeHours.TotalHours).DefaultMoneyRound();
                    var holidayAmount = calculatorService.CalculateHolidayAmount(workerRate, rates.Holiday, hoursBreakdown.HolidayHours.TotalHours).DefaultMoneyRound();
                    var missingAmount = calculatorService.CalculateMissingAmount(missingRateWorker, ts.MissingHours.TotalHours).DefaultMoneyRound();
                    var missingOvertimeAmount = calculatorService.CalculateOvertimeAmount(missingRateWorker, rates.OverTime, ts.MissingHoursOvertime.TotalHours).DefaultMoneyRound();

                    // Create TimeSheetTotalPayroll entity
                    var timeSheetTotal = calculatorService.CreateTimeSheetTotalPayrollEntity(
                        ts.TimeSheetId, hoursBreakdown, accumulatedHours);

                    // Accumulate hours by rate for PayStubItem creation
                    if (regularAmount > 0)
                        Accumulate(regularByRate, workerRate, hoursBreakdown.RegularHours.TotalHours);
                    if (otherRegularAmount > 0)
                        Accumulate(otherRegularByRate, workerRate, hoursBreakdown.OtherRegularHours.TotalHours);
                    if (overtimeAmount > 0)
                        Accumulate(overtimeByRate, rates.OverTime * workerRate, hoursBreakdown.OvertimeHours.TotalHours);
                    if (holidayAmount > 0)
                        Accumulate(holidayByRate, rates.Holiday * workerRate, hoursBreakdown.HolidayHours.TotalHours);
                    if (missingAmount > 0)
                        Accumulate(missingByRate, missingRateWorker, ts.MissingHours.TotalHours);
                    if (missingOvertimeAmount > 0)
                        Accumulate(missingOvertimeByRate, rates.OverTime * missingRateWorker, ts.MissingHoursOvertime.TotalHours);

                    // Create wage detail linked to TimeSheetTotalPayroll
                    allWageDetails.Add(new PayStubWageDetail(
                        workerRate: workerRate,
                        regular: regularAmount,
                        otherRegular: otherRegularAmount,
                        missing: missingAmount,
                        missingOvertime: missingOvertimeAmount,
                        nightShift: 0,
                        holiday: holidayAmount,
                        overtime: overtimeAmount,
                        timeSheetTotalId: timeSheetTotal.Id)
                    {
                        TimeSheetTotal = timeSheetTotal
                    });
                }
            }

            // Step 3.3: Get public holidays for this week
            var firstDateInWeek = week.First().Date;
            var holidaysInWeek = await catalogRepository.GetHolidaysInWeek(firstDateInWeek);
            if (holidaysInWeek != null && holidaysInWeek.Any())
            {
                foreach (var holiday in holidaysInWeek)
                {
                    var wages = await payStubRepository.GetWorkerRegularWages(
                        new ParamsToGetRegularWages(workerProfileId, holiday));
                    if (wages == null) continue;
                    var amount = wages.AmountToPay;
                    if (amount <= 0) continue;

                    var rHoliday = PayStubPublicHoliday.Create(holiday, amount, wages.Description);
                    if (rHoliday) publicHolidays.Add(rHoliday.Value);
                }
            }
        }

        // Step 4: Create PayStubItems from accumulated hours by rate
        var payStubItems = new List<PayStubItem>();
        var itemErrors = new List<ResultError>();

        foreach (var entry in regularByRate)
            AddItem(PayStubItem.CreateRegular(entry.Value, entry.Key), payStubItems, itemErrors);
        foreach (var entry in otherRegularByRate)
            AddItem(PayStubItem.CreateOtherRegular(entry.Value, entry.Key), payStubItems, itemErrors);
        foreach (var entry in overtimeByRate)
            AddItem(PayStubItem.CreateOvertime(entry.Value, entry.Key), payStubItems, itemErrors);
        foreach (var entry in holidayByRate)
            AddItem(PayStubItem.CreateStatutoryWorkedHoliday(entry.Value, entry.Key), payStubItems, itemErrors);
        foreach (var entry in missingByRate)
            AddItem(PayStubItem.CreateMissing(entry.Value, entry.Key), payStubItems, itemErrors);
        foreach (var entry in missingOvertimeByRate)
            AddItem(PayStubItem.CreateMissingOvertime(entry.Value, entry.Key), payStubItems, itemErrors);

        if (itemErrors.Any()) return Result.Fail(itemErrors);

        // Step 5: Process extras from timesheets (bonus, reimbursements, deductions)
        var reimbursementItems = new List<PayStubItem>();
        var otherDeductions = new List<PayStubOtherDeduction>();

        // Bonus/Other earnings - group by description to consolidate
        var bonusGroups = new Dictionary<string, decimal>();
        var reimbursementGroups = new Dictionary<string, decimal>();
        foreach (var ts in timesheets)
        {
            if (ts.BonusOrOthers > 0)
            {
                var key = ts.BonusOrOthersDescription ?? "Bonus";
                if (!bonusGroups.ContainsKey(key)) bonusGroups[key] = ts.BonusOrOthers;
                bonusGroups[key] += ts.BonusOrOthers;
            }
            if (ts.Reimbursements > 0)
            {
                var key = ts.ReimbursementsDescription ?? "Reimbursement";
                if (!reimbursementGroups.ContainsKey(key)) reimbursementGroups[key] = ts.Reimbursements;
                reimbursementGroups[key] += ts.Reimbursements;
            }
            // Other deductions
            if (ts.DeductionsOthers > 0)
            {
                otherDeductions.Add(PayStubOtherDeduction.CreateDefaultDeduction(ts.DeductionsOthers, ts.DeductionsOthersDescription));
            }
        }
        foreach (var bonus in bonusGroups)
        {
            var rBonus = PayStubItem.CreateBonusOthersItem(bonus.Value, bonus.Key);
            if (rBonus) payStubItems.Add(rBonus.Value);
        }

        // Reimbursements - group by description to consolidate
        foreach (var reimbursement in reimbursementGroups)
        {
            var rReimb = PayStubItem.CreateReimbursements(reimbursement.Value, reimbursement.Key);
            if (rReimb) reimbursementItems.Add(rReimb.Value);
        }

        // Filter items with zero total
        payStubItems = [.. payStubItems.Where(i => i.Total > 0)];
        reimbursementItems = [.. reimbursementItems.Where(r => r.Total > 0)];

        if (payStubItems.Count == 0)
            return Result.Fail("There is not enough information to generate pay stub");

        // Step 6: Calculate totals
        var grossForVacations = payStubItems.Sum(i => i.Total).DefaultMoneyRound();
        var vacations = calculatorService.CalculateVacationsAmount(grossForVacations, rates.Vacations);
        var publicHolidayPay = publicHolidays.Sum(h => h.Amount).DefaultMoneyRound();

        // Add statutory holiday as a PayStubItem (previously shown separately in summary)
        if (publicHolidayPay > 0)
        {
            var rHolidayItem = PayStubItem.CreateStatutoryHoliday(publicHolidayPay);
            if (rHolidayItem) payStubItems.Add(rHolidayItem.Value);
        }

        var grossPayment = payStubItems.Sum(i => i.Total).DefaultMoneyRound();
        var totalEarnings = grossPayment.Add(vacations).DefaultMoneyRound();

        // Step 7: Calculate deductions
        var minDate = timesheets.Min(m => m.Date);
        var maxDate = timesheets.Max(m => m.Date);
        var dateWorkBegins = minDate.AddDays(-(int)minDate.DayOfWeek);
        var dateWorkEnd = maxDate.AddDays(6 - (int)maxDate.DayOfWeek);
        var numberOfWeeks = dateWorkBegins.GetNumberOfWeeksIn(dateWorkEnd);

        var deductions = await calculatorService.CalculateDeductions(totalEarnings, numberOfWeeks, dateWorkEnd.Year, workerProfileId);
        var otherDeductionsTotal = otherDeductions.Sum(d => d.Total);
        var totalDeductions = deductions.Cpp + deductions.Ei + (deductions.FederalTax ?? 0) + (deductions.ProvincialTax ?? 0) + otherDeductionsTotal;

        // Step 8: Calculate net pay
        var reimbursementTotal = reimbursementItems.Sum(r => r.Total).DefaultMoneyRound();
        var totalPaid = decimal.Subtract(totalEarnings, totalDeductions).DefaultMoneyRound().Add(reimbursementTotal);

        // Step 9: Build PayStub entity
        var paymentDate = allWageDetails.Any()
            ? dateWorkEnd.GetPaymentDateForExternalWorkers()
            : dateWorkEnd.GetPaymentDateForInternalWorkers();

        var regularWage = payStubItems
            .Where(i => i.Type == PayStubItemType.Regular)
            .Sum(i => i.Total)
            .DefaultMoneyRound();

        var payStub = new PayStub
        {
            Id = Guid.NewGuid(),
            WorkerProfileId = workerProfileId,
            PayStubNumber = GeneratePayStubNumber(payStubNumber, DateTime.Now),
            PayStubNumberId = payStubNumber,
            TypeOfWork = typeOfWork,
            DateWorkBegins = dateWorkBegins,
            DateWorkEnd = dateWorkEnd,
            PaymentDate = paymentDate,
            RegularWage = regularWage,
            GrossPayment = grossPayment,
            Vacations = vacations,
            TotalEarnings = totalEarnings,
            Cpp = deductions.Cpp,
            Ei = deductions.Ei,
            FederalTax = deductions.FederalTax ?? 0,
            ProvincialTax = deductions.ProvincialTax ?? 0,
            TotalDeductions = totalDeductions,
            TotalPaid = totalPaid,
            CreatedAt = DateTime.Now,
            WeekEnding = dateWorkEnd.GetWeekEndingCurrentWeek()
        };

        payStub.AddItems(payStubItems.Concat(reimbursementItems));
        payStub.AddWageDetails(allWageDetails);
        payStub.AddHolidays(publicHolidays);
        payStub.AddOtherDeductionsDetail(otherDeductions);

        // Step 10: Save
        await payStubRepository.Create(payStub);
        await payStubRepository.SaveChangesAsync();

        return Result.Ok();
    }

    public async Task<Result<PayStub>> CreateManualPayStub(CreatePayStubModel model)
    {
        // Step 1: Resolve pay stub number
        if (model.PayStubNumber <= 0)
        {
            var numbers = await payStubRepository.GetNextPayStubNumbers(1);
            model.PayStubNumber = numbers.First().NextNumber;
        }
        else
        {
            if (await payStubRepository.IsPayStubNumberTaken(model.PayStubNumber))
                return Result.Fail<PayStub>("Pay stub number is taken");
        }

        // Step 2: Create PayStubItems from model
        var payStubItems = new List<PayStubItem>();
        var itemErrors = new List<ResultError>();

        if (model.RegularHours > 0)
            AddItem(PayStubItem.CreateRegular(model.RegularHours, model.UnitPriceRegularHours), payStubItems, itemErrors);
        if (model.OvertimeHours > 0)
            AddItem(PayStubItem.CreateOvertime(model.OvertimeHours, model.UnitPriceOvertimeHours), payStubItems, itemErrors);
        if (model.MissingHours > 0)
            AddItem(PayStubItem.CreateMissing(model.MissingHours, model.UnitPriceMissingHours), payStubItems, itemErrors);
        if (model.MissingOvertimeHours > 0)
            AddItem(PayStubItem.CreateMissingOvertime(model.MissingOvertimeHours, model.UnitPriceMissingOvertimeHours), payStubItems, itemErrors);
        if (model.HolidayPremiumPayHours > 0)
            AddItem(PayStubItem.CreateHolidayPremiumPay(model.HolidayPremiumPayHours, model.UnitPriceHolidayPremiumPayHours), payStubItems, itemErrors);
        if (model.Other > 0)
            AddItem(PayStubItem.CreateBonusOthersItem(model.Other, model.UnitPriceOther, model.BonusOthersDescription), payStubItems, itemErrors);
        if (model.Other2 > 0)
            AddItem(PayStubItem.CreateBonusOthersItem(model.Other2, model.UnitPriceOther2, model.BonusOthersDescription2), payStubItems, itemErrors);
        if (model.Other3 > 0)
            AddItem(PayStubItem.CreateBonusOthersItem(model.Other3, model.UnitPriceOther3, model.BonusOthersDescription3), payStubItems, itemErrors);

        if (itemErrors.Any()) return Result.Fail<PayStub>(itemErrors);

        // Step 3: Create holidays
        var publicHolidays = new List<PayStubPublicHoliday>();
        foreach (var h in model.Holidays)
        {
            var rHoliday = PayStubPublicHoliday.Create(h.Holiday, h.Amount, "This amount was added manually");
            if (!rHoliday) return Result.Fail<PayStub>(rHoliday.Errors);
            publicHolidays.Add(rHoliday.Value);
        }

        // Step 4: Create other deductions
        var otherDeductions = model.OtherDeductions > 0
            ? new[] { PayStubOtherDeduction.CreateDefaultDeduction(model.OtherDeductions, model.OtherDeductionsDescription) }
            : [];

        // Step 5: Adjust dates to week boundaries (Sunday-Saturday)
        var dateWorkBegins = model.WorkBegins.AddDays(-(int)model.WorkBegins.DayOfWeek);
        var dateWorkEnd = model.WorkEnd.AddDays(6 - (int)model.WorkEnd.DayOfWeek);

        // Step 6: Filter and validate items
        payStubItems = [.. payStubItems.Where(i => i.Total > 0)];
        if (payStubItems.Count == 0)
            return Result.Fail<PayStub>("There is not enough information to generate pay stub");
        if (dateWorkBegins > dateWorkEnd)
            return Result.Fail<PayStub>("Dates of work: start must be before end");

        // Step 7: Calculate earnings
        var grossForVacations = payStubItems.Sum(i => i.Total).DefaultMoneyRound();
        var vacations = model.PayVacations
            ? calculatorService.CalculateVacationsAmount(grossForVacations, rates.Vacations)
            : decimal.Zero;
        var publicHolidayPay = publicHolidays.Sum(h => h.Amount).DefaultMoneyRound();

        if (publicHolidayPay > 0)
        {
            var rHolidayItem = PayStubItem.CreateStatutoryHoliday(publicHolidayPay);
            if (rHolidayItem) payStubItems.Add(rHolidayItem.Value);
        }

        var grossPayment = payStubItems.Sum(i => i.Total).DefaultMoneyRound();
        var totalEarnings = grossPayment.Add(vacations).DefaultMoneyRound();

        // Step 8: Calculate deductions
        var numberOfWeeks = dateWorkBegins.GetNumberOfWeeksIn(dateWorkEnd);
        var deductions = await calculatorService.CalculateDeductions(totalEarnings, numberOfWeeks, dateWorkEnd.Year, model.WorkerProfileId);
        var otherDeductionsTotal = otherDeductions.Sum(d => d.Total);
        var totalDeductions = deductions.Cpp + deductions.Ei + (deductions.FederalTax ?? 0) + (deductions.ProvincialTax ?? 0) + otherDeductionsTotal;

        // Step 9: Calculate net pay
        var totalPaid = decimal.Subtract(totalEarnings, totalDeductions).DefaultMoneyRound();

        // Step 10: Build PayStub entity
        var regularWage = payStubItems
            .Where(i => i.Type == PayStubItemType.Regular)
            .Sum(i => i.Total)
            .DefaultMoneyRound();

        var payStub = new PayStub
        {
            Id = Guid.NewGuid(),
            WorkerProfileId = model.WorkerProfileId,
            PayStubNumber = GeneratePayStubNumber(model.PayStubNumber, DateTime.Now),
            PayStubNumberId = model.PayStubNumber,
            TypeOfWork = model.TypeOfWork,
            DateWorkBegins = dateWorkBegins,
            DateWorkEnd = dateWorkEnd,
            PaymentDate = dateWorkEnd.GetPaymentDateForInternalWorkers(),
            RegularWage = regularWage,
            GrossPayment = grossPayment,
            Vacations = vacations,
            TotalEarnings = totalEarnings,
            Cpp = deductions.Cpp,
            Ei = deductions.Ei,
            FederalTax = deductions.FederalTax ?? 0,
            ProvincialTax = deductions.ProvincialTax ?? 0,
            TotalDeductions = totalDeductions,
            TotalPaid = totalPaid,
            CreatedAt = DateTime.Now,
            WeekEnding = dateWorkEnd.GetWeekEndingCurrentWeek()
        };

        payStub.AddItems(payStubItems);
        payStub.AddHolidays(publicHolidays);
        payStub.AddOtherDeductionsDetail(otherDeductions);

        // Step 11: Save
        await payStubRepository.Create(payStub);
        await payStubRepository.SaveChangesAsync();

        return Result.Ok(payStub);
    }

    private void Accumulate(Dictionary<decimal, double> dict, decimal rate, double hours)
    {
        if (!dict.ContainsKey(rate)) 
            dict[rate] = 0;
        dict[rate] += hours;
    }

    private void AddItem(Result<PayStubItem> result, List<PayStubItem> items, List<ResultError> errors)
    {
        if (result) items.Add(result.Value);
        else errors.AddRange(result.Errors);
    }

    private string GeneratePayStubNumber(long number, DateTime date) => $"PS-{number:0000}-{date:yy}";
}
