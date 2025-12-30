using ClosedXML.Excel;
using Covenant.Deductions.Entities;
using Covenant.Deductions.Repositories;
using Covenant.Deductions.Services;
using Microsoft.Extensions.Logging;

namespace Covenant.Infrastructure.Deductions
{
	public class CppTablesLoader : ICppTablesLoader
	{
		private readonly IDeductionsRepository _repository;
		private readonly ILogger<CppTablesLoader> _logger;

		public CppTablesLoader(IDeductionsRepository repository, ILogger<CppTablesLoader> logger)
		{
			_repository = repository;
			_logger = logger;
		}

		public async Task LoadWeeklyTablesFromExcel(string excelPath, int year)
		{
			var cppChanged = false;
			await ReadSheet(excelPath, async (cpp, s) =>
			{
				if (cpp is null)
				{
					_logger?.LogError("Invalid cpp value {Value}", s);
					return;
				}
				await _repository.CreateCppWeekly(new CppWeekly(cpp, year));
				cppChanged = true;
			});
			if (!cppChanged) return;
			await _repository.DeleteCppWeekly(year);
			await _repository.SaveChangesAsync();
		}

		public async Task LoadBiWeeklyTablesFromExcel(string excelPath, int year)
		{
			var cppChanged = false;
			await ReadSheet(excelPath, async (cpp, s) =>
			{
				if (cpp is null)
				{
					_logger?.LogError("Invalid cpp value {Value}", s);
					return;
				}
				await _repository.CreateCppBiWeekly(new CppBiWeekly(cpp, year));
				cppChanged = true;
			});
			if (!cppChanged) return;
			await _repository.DeleteCppBiWeekly(year);
			await _repository.SaveChangesAsync();
		}

		public async Task LoadSemiMonthlyTablesFromExcel(string excelPath, int year)
		{
			var cppChanged = false;
			await ReadSheet(excelPath, async (cpp, s) =>
			{
				if (cpp is null)
				{
					_logger?.LogError("Invalid cpp value {Value}", s);
					return;
				}
				await _repository.CreateCppSemiMonthly(new CppSemiMonthly(cpp, year));
				cppChanged = true;
			});
			if (!cppChanged) return;
			await _repository.DeleteCppSemiMonthly(year);
			await _repository.SaveChangesAsync();
		}

		public async Task LoadMonthlyTablesFromExcel(string excelPath, int year)
		{
			var cppChanged = false;
			await ReadSheet(excelPath, async (cpp, s) =>
			{
				if (cpp is null)
				{
					_logger?.LogError("Invalid cpp value {Value}", s);
					return;
				}
				await _repository.CreateCppMonthly(new CppMonthly(cpp, year));
				cppChanged = true;
			});
			if (!cppChanged) return;
			await _repository.DeleteCppMonthly(year);
			await _repository.SaveChangesAsync();
		}

		private static async Task ReadSheet(string excelPath, Func<ICpp, string, Task> create)
		{
			if (!File.Exists(excelPath)) return;
			using (var workbook = new XLWorkbook(excelPath))
			{
				IXLWorksheet worksheet = workbook.Worksheets.First();
				IXLRow row = worksheet.FirstRowUsed();
				IXLRangeRow rangeRow = row.RowUsed();
				while (rangeRow != null && !rangeRow.IsEmpty())
				{
					IXLAddress lastAddress = rangeRow.RangeAddress.LastAddress;
					if (lastAddress.ColumnNumber >= 3)
					{
						(ICpp cpp1, string sCpp1) = GetCppValues(rangeRow, 1, 2, 3);
						await create(cpp1, sCpp1);
						if (lastAddress.ColumnNumber >= 6)
						{
							(ICpp cpp2, string sCpp2) = GetCppValues(rangeRow, 4, 5, 6);
							await create(cpp2, sCpp2);
							if (lastAddress.ColumnNumber >= 9)
							{
								(ICpp cpp3, string sCpp3) = GetCppValues(rangeRow, 7, 8, 9);
								await create(cpp3, sCpp3);
								if (lastAddress.ColumnNumber >= 12)
								{
									(ICpp cpp4, string sCpp4) = GetCppValues(rangeRow, 10, 11, 12);
									await create(cpp4, sCpp4);
								}
							}
						}
					}

					rangeRow = rangeRow.RowBelow();
				}
			}
		}

		private static (ICpp, string) GetCppValues(IXLRangeRow rangeRow, int columnFrom, int columnTo, int columnCpp)
		{
			(decimal? fromValue, string fromString) = GetCellValue(rangeRow, columnFrom);
			if (fromValue is null) return (null, fromString);
			(decimal? toValue, string toString) = GetCellValue(rangeRow, columnTo);
			if (toValue is null) return (null, toString);
			(decimal? cppValue, string cppString) = GetCellValue(rangeRow, columnCpp);
			if (cppValue is null) return (null, cppString);
			var myCpp = new MyCpp { From = fromValue.Value, To = toValue.Value, Cpp = cppValue.Value };
			return (myCpp, null);
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

		private class MyCpp : ICpp
		{
			public Guid Id { get; set; }
			public decimal From { get; set; }
			public decimal To { get; set; }
			public decimal Cpp { get; set; }
			public int Year { get; set; }
		}
	}
}