using ClosedXML.Excel;
using Covenant.Common.Configuration;
using Covenant.Common.Entities.Accounting.PayStub;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting.PayStub;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Accounting;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;

namespace Covenant.Core.BL.Services;

public class PayStubService : IPayStubService
{
    private readonly IPayStubRepository payStubRepository;
    private readonly ITimesheetCalculatorService calculatorService;
    private readonly ITimeSheetRepository timeSheetRepository;
    private readonly ICatalogRepository catalogRepository;
    private readonly Rates rates;
    private readonly TimeLimits timeLimits;

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
        if (!result.Any())
            return Result.Fail<ResultGenerateDocument<byte[]>>("No data available for the selected dates");

        using var stream = new MemoryStream();
        using var workbook = new XLWorkbook();
        GetDetail(result, workbook);
        GetReport(result, workbook);
        workbook.SaveAs(stream);
        return Result.Ok(new ResultGenerateDocument<byte[]>(stream.ToArray(), $"T4_Resume_{DateTime.Now.Year}.xlsx", string.Empty));
    }

    public async Task<Result<ResultGenerateDocument<byte[]>>> GenerateCraPayroll(DateTime from, DateTime to)
    {
        var result = await payStubRepository.GetPayStubsByDates(from, to);
        if (!result.Any())
            return Result.Fail<ResultGenerateDocument<byte[]>>("No data available for the selected dates");

        using var stream = new MemoryStream();
        using var workbook = new XLWorkbook();
        GetCraPayrollReport(result, workbook);
        workbook.SaveAs(stream);
        return Result.Ok(new ResultGenerateDocument<byte[]>(stream.ToArray(), $"CRA_Payroll_{DateTime.Now.Year}.xlsx", string.Empty));
    }

    public async Task<Result<PayStub>> CreateManualPayStub(CreatePayStubModel model)
    {
        // Step 1: Resolve pay stub number
        var nextNumbers = await payStubRepository.GetNextPayStubNumbers(1);
        var payStubNumber = nextNumbers.First().NextNumber;

        // Step 2: Create PayStubItems from model
        var payStubItems = new List<PayStubItem>();
        var itemErrors = new List<ResultError>();

        foreach (var item in model.Items)
        {
            var result = item.Type switch
            {
                PayStubItemType.Regular => PayStubItem.CreateItem(item.Description ?? PayStubItem.RegularHoursLabel, item.Quantity, item.UnitPrice, PayStubItemType.Regular),
                PayStubItemType.OtherRegular => PayStubItem.CreateItem(item.Description ?? PayStubItem.OtherRegularHoursLabel, item.Quantity, item.UnitPrice, PayStubItemType.OtherRegular),
                PayStubItemType.Overtime => PayStubItem.CreateItem(item.Description ?? PayStubItem.OvertimeHoursLabel, item.Quantity, item.UnitPrice, PayStubItemType.Overtime),
                PayStubItemType.StatutoryHoliday => PayStubItem.CreateItem(item.Description ?? PayStubItem.StatutoryHolidayLabel, 1, item.UnitPrice, PayStubItemType.StatutoryHoliday),
                PayStubItemType.StatutoryWorkedHoliday => PayStubItem.CreateItem(item.Description ?? PayStubItem.StatutoryWorkedHolidayLabel, item.Quantity, item.UnitPrice, PayStubItemType.StatutoryWorkedHoliday),
                PayStubItemType.NightShift => PayStubItem.CreateItem(item.Description ?? PayStubItem.NightShiftHoursLabel, item.Quantity, item.UnitPrice, PayStubItemType.NightShift),
                PayStubItemType.Missing => PayStubItem.CreateItem(item.Description ?? PayStubItem.MissingHoursLabel, item.Quantity, item.UnitPrice, PayStubItemType.Missing),
                PayStubItemType.MissingOvertime => PayStubItem.CreateItem(item.Description ?? PayStubItem.MissingOvertimeHoursLabel, item.Quantity, item.UnitPrice, PayStubItemType.MissingOvertime),
                PayStubItemType.Vacations => PayStubItem.CreateItem(item.Description ?? "Vacations", item.Quantity, item.UnitPrice, PayStubItemType.Vacations),
                PayStubItemType.Other => PayStubItem.CreateBonusOthersItem(item.Quantity, item.UnitPrice, item.Description),
                PayStubItemType.Reimbursement => PayStubItem.CreateReimbursements(decimal.Multiply(new decimal(item.Quantity), item.UnitPrice), item.Description),
                _ => Result.Fail<PayStubItem>($"Unknown item type: {item.Type}")
            };
            AddItem(result, payStubItems, itemErrors);
        }

        if (itemErrors.Count != 0) return Result.Fail<PayStub>(itemErrors);

        // Step 3: Create other deductions
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

        // Step 6: Calculate earnings
        var vacations = 0m;
        var grossPayment = payStubItems
            .Where(i => i.Type != PayStubItemType.Vacations && i.Type != PayStubItemType.Reimbursement)
            .Sum(i => i.Total)
            .DefaultMoneyRound();
        if (model.PayVacations)
        {
            vacations = calculatorService.CalculateVacationsAmount(grossPayment, rates.Vacations).DefaultMoneyRound();
        }
        else if (model.Items.Any(i => i.Type == PayStubItemType.Vacations))
        {
            vacations = payStubItems
                .Where(i => i.Type == PayStubItemType.Vacations)
                .Sum(i => i.Total)
                .DefaultMoneyRound();
        }
        var totalEarnings = grossPayment.Add(vacations).DefaultMoneyRound();

        // Step 8: Calculate deductions
        var numberOfWeeks = dateWorkBegins.GetNumberOfWeeksIn(dateWorkEnd);
        var deductions = await calculatorService.CalculateDeductions(totalEarnings, numberOfWeeks, dateWorkEnd.Year, model.WorkerProfileId);
        var otherDeductionsTotal = otherDeductions.Sum(d => d.Total);
        var totalDeductions = deductions.Cpp + deductions.Ei + deductions.FederalTax + deductions.ProvincialTax + otherDeductionsTotal;

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
            PayStubNumber = GeneratePayStubNumber(payStubNumber, DateTime.Now),
            PayStubNumberId = payStubNumber,
            Position = model.Position,
            DateWorkBegins = dateWorkBegins,
            DateWorkEnd = dateWorkEnd,
            PaymentDate = dateWorkEnd.GetPaymentDateForInternalWorkers(),
            RegularWage = regularWage,
            GrossPayment = grossPayment,
            Vacations = vacations,
            TotalEarnings = totalEarnings,
            Cpp = deductions.Cpp,
            Ei = deductions.Ei,
            FederalTax = deductions.FederalTax,
            ProvincialTax = deductions.ProvincialTax,
            TotalDeductions = totalDeductions,
            TotalPaid = totalPaid,
            CreatedAt = DateTime.Now,
            WeekEnding = dateWorkEnd.GetWeekEndingCurrentWeek(),
            Items = payStubItems,
            OtherDeductions = [.. otherDeductions],
        };

        // Step 10: Save
        await payStubRepository.Create(payStub);
        await payStubRepository.SaveChangesAsync();

        return Result.Ok(payStub);
    }

    public async Task<Result> Generate(IEnumerable<Guid> agencyIds, IEnumerable<Guid> workerIds)
    {
        foreach (var workerId in workerIds)
        {
            var result = await GeneratePayStubForWorker(agencyIds, workerId);
            if (!result) return result;
        }
        return Result.Ok();
    }

    private async Task<Result> GeneratePayStubForWorker(IEnumerable<Guid> agencyIds, Guid workerId)
    {
        // Step 1: Get approved timesheets for this worker
        var timesheets = await timeSheetRepository.GetTimeSheetForCreatingPayStubs(agencyIds, workerId);
        if (timesheets.Count == 0) return Result.Ok();

        // Step 2: Get next paystub number
        var nextNumbers = await payStubRepository.GetNextPayStubNumbers(1);
        var payStubNumber = nextNumbers.First().NextNumber;

        var workerProfileId = timesheets.First().WorkerProfileId;
        var typeOfWork = timesheets.First().TypeOfWork;
        var overtimeStartsAfter = timesheets.First().OvertimeStartsAfter;
        var countryCode = timesheets.First().CountryCode;

        // Step 3: Process timesheets by week and request to calculate hours and wages
        var allWageDetails = new List<PayStubWageDetail>();
        var publicHolidays = new List<PayStubPublicHoliday>();

        // Dictionaries to accumulate hours by rate for each item type
        var regularByRate = new Dictionary<decimal, double>();
        var otherRegularByRate = new Dictionary<decimal, double>();
        var overtimeByRate = new Dictionary<decimal, double>();
        var workedHolidayByRate = new Dictionary<decimal, double>();
        var missingByRate = new Dictionary<decimal, double>();
        var missingOvertimeByRate = new Dictionary<decimal, double>();

        // Dictionaries to accumulate extras by description
        var bonusGroups = new Dictionary<string, decimal>();
        var reimbursementGroups = new Dictionary<string, decimal>();
        var otherDeductions = new List<PayStubOtherDeduction>();

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
                        Accumulate(workedHolidayByRate, rates.Holiday * workerRate, hoursBreakdown.HolidayHours.TotalHours);
                    if (missingAmount > 0)
                        Accumulate(missingByRate, missingRateWorker, ts.MissingHours.TotalHours);
                    if (missingOvertimeAmount > 0)
                        Accumulate(missingOvertimeByRate, rates.OverTime * missingRateWorker, ts.MissingHoursOvertime.TotalHours);

                    // Accumulate extras (bonus, reimbursements, deductions)
                    if (ts.BonusOrOthers > 0)
                    {
                        var key = ts.BonusOrOthersDescription ?? "Bonus";
                        if (!bonusGroups.ContainsKey(key)) bonusGroups[key] = 0;
                        bonusGroups[key] += ts.BonusOrOthers;
                    }
                    if (ts.Reimbursements > 0)
                    {
                        var key = ts.ReimbursementsDescription ?? "Reimbursement";
                        if (!reimbursementGroups.ContainsKey(key)) reimbursementGroups[key] = 0;
                        reimbursementGroups[key] += ts.Reimbursements;
                    }
                    if (ts.DeductionsOthers > 0)
                    {
                        otherDeductions.Add(PayStubOtherDeduction.CreateDefaultDeduction(ts.DeductionsOthers, ts.DeductionsOthersDescription));
                    }

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
            var holidaysInWeek = await catalogRepository.GetHolidaysInWeek(firstDateInWeek, countryCode);
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
        foreach (var entry in workedHolidayByRate)
            AddItem(PayStubItem.CreateStatutoryWorkedHoliday(entry.Value, entry.Key), payStubItems, itemErrors);
        foreach (var entry in missingByRate)
            AddItem(PayStubItem.CreateMissing(entry.Value, entry.Key), payStubItems, itemErrors);
        foreach (var entry in missingOvertimeByRate)
            AddItem(PayStubItem.CreateMissingOvertime(entry.Value, entry.Key), payStubItems, itemErrors);
        foreach (var entry in publicHolidays)
            AddItem(PayStubItem.CreateStatutoryHoliday(entry.Amount), payStubItems, itemErrors);
        foreach (var bonus in bonusGroups)
            AddItem(PayStubItem.CreateBonusOthersItem(bonus.Value, bonus.Key), payStubItems, itemErrors);
        foreach (var reimbursement in reimbursementGroups)
            AddItem(PayStubItem.CreateReimbursements(reimbursement.Value, reimbursement.Key), payStubItems, itemErrors);

        if (itemErrors.Count != 0) return Result.Fail(itemErrors);

        payStubItems = [.. payStubItems.Where(i => i.Total > 0)];

        if (payStubItems.Count == 0)
            return Result.Fail("There is not enough information to generate pay stub");

        // Step 6: Calculate totals
        var grossPayment = payStubItems
            .Where(p => p.Type != PayStubItemType.Reimbursement)
            .Sum(i => i.Total)
            .DefaultMoneyRound();
        var vacations = calculatorService.CalculateVacationsAmount(grossPayment, rates.Vacations);
        var totalEarnings = grossPayment.Add(vacations).DefaultMoneyRound();

        // Step 7: Calculate deductions
        var minDate = timesheets.Min(m => m.Date);
        var maxDate = timesheets.Max(m => m.Date);
        var dateWorkBegins = minDate.AddDays(-(int)minDate.DayOfWeek);
        var dateWorkEnd = maxDate.AddDays(6 - (int)maxDate.DayOfWeek);
        var numberOfWeeks = dateWorkBegins.GetNumberOfWeeksIn(dateWorkEnd);

        var deductions = await calculatorService.CalculateDeductions(totalEarnings, numberOfWeeks, dateWorkEnd.Year, workerProfileId);
        var otherDeductionsTotal = otherDeductions.Sum(d => d.Total);
        var totalDeductions = deductions.Cpp + deductions.Ei + deductions.FederalTax + deductions.ProvincialTax + otherDeductionsTotal;

        // Step 8: Calculate net pay
        var reimbursementTotal = payStubItems
            .Where(p => p.Type == PayStubItemType.Reimbursement)
            .Sum(r => r.Total)
            .DefaultMoneyRound();
        var totalPaid = decimal.Subtract(totalEarnings, totalDeductions).Add(reimbursementTotal).DefaultMoneyRound();

        // Step 9: Build PayStub entity
        var paymentDate = allWageDetails.Count != 0
            ? dateWorkEnd.GetPaymentDateForExternalWorkers()
            : dateWorkEnd.GetPaymentDateForInternalWorkers();

        var regularWage = payStubItems
            .Where(i => i.Type == PayStubItemType.Regular)
            .Sum(i => i.Total)
            .DefaultMoneyRound();

        var payStub = new PayStub
        {
            WorkerProfileId = workerProfileId,
            PayStubNumber = GeneratePayStubNumber(payStubNumber, DateTime.Now),
            PayStubNumberId = payStubNumber,
            Position = typeOfWork,
            DateWorkBegins = dateWorkBegins,
            DateWorkEnd = dateWorkEnd,
            PaymentDate = paymentDate,
            RegularWage = regularWage,
            GrossPayment = grossPayment,
            Vacations = vacations,
            TotalEarnings = totalEarnings,
            Cpp = deductions.Cpp,
            Ei = deductions.Ei,
            FederalTax = deductions.FederalTax,
            ProvincialTax = deductions.ProvincialTax,
            TotalDeductions = totalDeductions,
            TotalPaid = totalPaid,
            CreatedAt = DateTime.Now,
            WeekEnding = dateWorkEnd.GetWeekEndingCurrentWeek(),
            Items = payStubItems,
            WageDetails = allWageDetails,
            Holidays = publicHolidays,
            OtherDeductions = otherDeductions
        };

        // Step 10: Save
        await payStubRepository.Create(payStub);
        await payStubRepository.SaveChangesAsync();

        return Result.Ok();
    }

    private void GetReport(IEnumerable<PayStubT4Model> result, XLWorkbook workbook)
    {
        var sheet = workbook.Worksheets.Add("Report");
        var allColumns = new (string Header, Action<IXLCell, PayStubT4Model, int> WriteCell)[]
        {
            ("No.", (cell, _, idx) => cell.SetValue(idx + 1)),
            ("Name", (cell, d, _) => cell.SetValue(d.WorkerFullName.Trim())),
            ("Address", (cell, d, _) => cell.SetValue(d.Location)),
            ("SIN #", (cell, d, _) => cell.SetValue(d.Sin)),
            ("Total Earnings", (cell, d, _) => cell.SetValue(d.Items.Sum(i => i.TotalEarnings)).SetMoneyType()),
            ("CPP Employer", (cell, d, _) => cell.SetValue(d.Items.Sum(i => i.Employer.Cpp)).SetMoneyType()),
            ("EI Employeer", (cell, d, _) => cell.SetValue(d.Items.Sum(i => i.Employer.EI)).SetMoneyType()),
            ("Other Deductions", (cell, d, _) => cell.SetValue(d.Items.Sum(i => i.Employer.OtherDeductions)).SetMoneyType()),
            ("CPP Employee", (cell, d, _) => cell.SetValue(d.Items.Sum(i => i.Employee.Cpp)).SetMoneyType()),
            ("EI Employee", (cell, d, _) => cell.SetValue(d.Items.Sum(i => i.Employee.EI)).SetMoneyType()),
            ("Fed Tax", (cell, d, _) => cell.SetValue(d.Items.Sum(i => i.Employee.FederalTax)).SetMoneyType()),
            ("Prov Tax", (cell, d, _) => cell.SetValue(d.Items.Sum(i => i.Employee.ProvincialTax)).SetMoneyType()),
            ("Total Tax", (cell, d, _) => cell.SetValue(d.Items.Sum(i => i.Employee.FederalTax + i.Employee.ProvincialTax)).SetMoneyType()),
            ("Net Pay", (cell, d, _) => cell.SetValue(d.Items.Sum(i => i.TotalEarnings - i.Employee.Cpp - i.Employee.EI - i.Employee.FederalTax - i.Employee.ProvincialTax)).SetMoneyType()),
            ("Total", (cell, d, _) => cell.SetValue(d.Items.Sum(i => i.Employer.Cpp + i.Employer.EI + i.Employee.Cpp + i.Employee.EI + i.Employee.FederalTax + i.Employee.ProvincialTax)).SetMoneyType()),
        };

        sheet.SetupHeaders([.. allColumns.Select(c => c.Header)]);
        var startAt = 2;
        for (int i = 0; i < result.Count(); i++)
        {
            var data = result.ElementAt(i);
            for (int col = 0; col < allColumns.Length; col++)
            {
                allColumns[col].WriteCell(sheet.Cell($"{(char)('A' + col)}{startAt}"), data, i);
            }
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

    private void GetCraPayrollReport(IEnumerable<PayStubT4Model> result, XLWorkbook workbook)
    {
        var sheet = workbook.Worksheets.Add("Report");
        sheet.Range("D1:D2").Merge().SetValue("PAYSTUB #");
        sheet.Cell("D1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        sheet.Range("E1:E2").Merge().SetValue("Date Paid");
        sheet.Cell("E1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        sheet.Range("F1:F2").Merge().SetValue("TOTAL EARNINGS");
        sheet.Cell("F1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        sheet.Cell("G1").SetValue("EMPLOYEER");
        sheet.Range("G1:I1").Merge();
        sheet.Cell("J1").SetValue("EMPLOYEE");
        sheet.Range("J1:N1").Merge();
        sheet.Cell("G2").SetValue("CPP");
        sheet.Cell("H2").SetValue("EI");
        sheet.Cell("I2").SetValue("OTHER");
        sheet.Cell("J2").SetValue("CPP");
        sheet.Cell("K2").SetValue("EI");
        sheet.Cell("L2").SetValue("FED TAX");
        sheet.Cell("M2").SetValue("PROV TAX");
        sheet.Cell("N2").SetValue("Total Paid");
        sheet.Cell("A3").SetValue("No.");
        sheet.Cell("B3").SetValue("Names");

        for (int r = 1; r <= 3; r++)
        {
            sheet.Row(r).Style.Font.Bold = true;
            sheet.Row(r).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }

        var currentRow = 4;
        var workerIndex = 0;
        var dataStartRow = currentRow;

        foreach (var worker in result)
        {
            var items = worker.Items.ToList();
            var isFirstItem = true;
            workerIndex++;

            foreach (var item in items)
            {
                if (isFirstItem)
                {
                    if (workerIndex == 1)
                        sheet.Cell($"A{currentRow}").SetValue(workerIndex);
                    else
                        sheet.Cell($"A{currentRow}").SetFormulaA1($"1+A{dataStartRow}");

                    sheet.Cell($"B{currentRow}").SetValue(worker.WorkerFullName.Trim());
                    sheet.Cell($"C{currentRow}").SetValue(item.CompanyName ?? string.Empty);
                    dataStartRow = currentRow;
                    isFirstItem = false;
                }

                sheet.Cell($"D{currentRow}").SetValue(item.PayStubNumber);
                sheet.Cell($"E{currentRow}").SetValue(item.DatePaid.ToString("dd-MMM-yyyy"));
                sheet.Cell($"F{currentRow}").SetValue(item.TotalEarnings).SetMoneyType();
                sheet.Cell($"G{currentRow}").SetValue(item.Employer.Cpp).SetMoneyType();
                sheet.Cell($"H{currentRow}").SetFormulaA1($"K{currentRow}*1.4");
                sheet.Cell($"I{currentRow}").SetValue(item.Employer.OtherDeductions).SetMoneyType();
                sheet.Cell($"J{currentRow}").SetValue(item.Employee.Cpp).SetMoneyType();
                sheet.Cell($"K{currentRow}").SetValue(item.Employee.EI).SetMoneyType();
                sheet.Cell($"L{currentRow}").SetValue(item.Employee.FederalTax).SetMoneyType();
                sheet.Cell($"M{currentRow}").SetValue(item.Employee.ProvincialTax).SetMoneyType();
                sheet.Cell($"N{currentRow}").SetValue(
                    item.TotalEarnings - item.Employee.Cpp - item.Employee.EI
                    - item.Employee.FederalTax - item.Employee.ProvincialTax
                ).SetMoneyType();

                currentRow++;
            }
        }

        // Column widths
        sheet.Column("A").Width = 5;
        sheet.Column("B").Width = 30;
        sheet.Column("C").Width = 30;
        sheet.Column("D").Width = 16;
        sheet.Column("E").Width = 14;
        for (int c = 6; c <= 14; c++)
            sheet.Column(c).Width = 13;
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
