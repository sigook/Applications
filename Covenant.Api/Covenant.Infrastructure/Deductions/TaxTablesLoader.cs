using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Covenant.Deductions.Entities;
using Microsoft.Extensions.Logging;

namespace Covenant.Infrastructure.Deductions
{
	/// <summary>
	/// Reads the values from an excel with the payroll deduction tables found it on the CRA page
	/// https://www.canada.ca/en/revenue-agency/services/tax/businesses/topics/payroll/t4032-payroll-deductions-tables.html 
	/// </summary>
	public abstract class TaxTablesLoader
	{
		private readonly ILogger<TaxTablesLoader> _logger;
		protected TaxTablesLoader(ILogger<TaxTablesLoader> logger) => _logger = logger;

		protected async Task LoadTablesFromExcel(string excelPath, int year,
			Func<ITax, Task> create, Func<int, Task> delete, Func<Task> save)
		{
			var changed = false;
			await ReadWorkbook(excelPath, async result =>
			{
				await create(result);
				changed = true;
			}, year);
			if (!changed) return;
			await delete(year);
			await save();
		}
		
		protected async Task ReadWorkbook(string excelPath, Func<ITax, Task> onReadValidRecord, int year)
		{
			if (!File.Exists(excelPath)) return;
			using (var workbook = new XLWorkbook(excelPath))
			{
				IXLWorksheet federal = workbook.Worksheets.First();
				await ReadSheet(federal, onReadValidRecord, year);
			}
		}

		private async Task ReadSheet(IXLWorksheet worksheet, Func<ITax, Task> onReadValidRecord, int year)
		{
			IXLRow row = worksheet.FirstRowUsed();
			IXLRangeRow rangeRow = row.RowUsed();
			while (rangeRow != null && !rangeRow.IsEmpty())
			{
				ITax tax = GetRowValues(rangeRow, year);
				if(tax != null)await onReadValidRecord(tax);
				rangeRow = rangeRow.RowBelow();
			}
		}

		private ITax GetRowValues(IXLRangeRow rangeRow, int year)
		{
			(decimal? fromResult, decimal? toResult, string stringValue) = GetFromToValue(rangeRow, 1);
			if (!string.IsNullOrEmpty(stringValue))
			{
				_logger?.LogError("Invalid tax value {Value}", stringValue);
				return null;
			}
			(decimal? cc0, string cc0String) = GetCellValue(rangeRow, 2);
			(decimal? cc1, string cc1String) = GetCellValue(rangeRow, 3);
			(decimal? cc2, string cc2String) = GetCellValue(rangeRow, 4);
			(decimal? cc3, string cc3String) = GetCellValue(rangeRow, 5);
			(decimal? cc4, string cc4String) = GetCellValue(rangeRow, 6);
			(decimal? cc5, string cc5String) = GetCellValue(rangeRow, 7);
			(decimal? cc6, string cc6String) = GetCellValue(rangeRow, 8);
			(decimal? cc7, string cc7String) = GetCellValue(rangeRow, 9);
			(decimal? cc8, string cc8String) = GetCellValue(rangeRow, 10);
			(decimal? cc9, string cc9String) = GetCellValue(rangeRow, 11);
			(decimal? cc10, string cc10String) = GetCellValue(rangeRow, 12);
			return new TaxWeekly(fromResult.Value, toResult.Value, cc0, cc1, cc2, cc3, cc4, cc5, cc6, cc7, cc8, cc9, cc10, year);
		}

		private static (decimal? fromResult, decimal? toResult, string stringResult) GetFromToValue(IXLRangeRow rangeRow, int columnNumber)
		{
			if (rangeRow.Cell(columnNumber).TryGetValue(out decimal toResult)) return (0, toResult, null); //It's the first value which start from 0 to the value in the cell
			string stringValue = rangeRow.Cell(columnNumber).GetString();
			string[] split = stringValue.Split("-");
			if (split.Length != 2) return (null, null, stringValue);
			if (!decimal.TryParse(split[0], out decimal fromResult)) return (null, null, stringValue);
			if (!decimal.TryParse(split[1], out toResult)) return (null, null, stringValue);
			return (fromResult, toResult, null);
		}

		private static (decimal?, string) GetCellValue(IXLRangeRow rangeRow, int columnNumber)
		{
			if (rangeRow.Cell(columnNumber).TryGetValue(out decimal fromValue)) return (fromValue, null);
			string stringValue = rangeRow.Cell(columnNumber).GetString();
			if (string.IsNullOrEmpty(stringValue)) return (null, stringValue);
			var s = new string(stringValue.Where(w => char.IsDigit(w) || '.' == w).ToArray());
			if (decimal.TryParse(s, out fromValue)) return (fromValue, null);
			return (null, stringValue);
		}
	}
}