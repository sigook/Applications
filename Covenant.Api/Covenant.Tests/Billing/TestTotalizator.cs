using System;
using System.Collections.Generic;
using System.Linq;
using Covenant.Common.Configuration;
using Covenant.TimeSheetTotal.Models;
using Covenant.TimeSheetTotal.Services;
using Xunit;

namespace Covenant.Tests.Billing
{
	public class TestTotalizator
	{
		[Theory]
		[InlineData("9262D1D2-3CA1-4E5B-90D9-C2D0E96BFC71")]
		[InlineData("9262D1D2-3CA1-4E5B-90D9-C2D0E96BFC71,D6C35264-96AA-43BA-ABC7-078503A6DE93")]
		[InlineData("9262D1D2-3CA1-4E5B-90D9-C2D0E96BFC71,D6C35264-96AA-43BA-ABC7-078503A6DE93,A160F8E1-9845-46A5-96C5-34A1AB7F87F1")]
		public void Totals_Are_Calculated_By_Request(string requestsId)
		{
			List<Guid> requests = requestsId.Split(",").Select(Guid.Parse).ToList();
			List<TotalizatorParams> @params = requests.Select(requestId => 
				new TotalizatorParams(requestId,
				default,
				default,
				default,
				default,
				default,
				default, default, default, default,
				false, false, false,
				default,
				default,
				TimeSpan.Zero, TimeSpan.Zero, Guid.NewGuid())).ToList();
			IReadOnlyCollection<Totals> result = @params.GetWorkerTotals(Rates.DefaultRates, TimeSpan.FromHours(44),TimeSpan.FromHours(44));
			Assert.Equal(requests.Count,result.Count);
		}

		[Fact]
		public void Accumulation()
		{
			List<TotalizatorParams> @params = Enumerable.Range(1,5).Select(day => 
				new TotalizatorParams(default, default,
					new DateTime(2019,01,day,00,00,00), 
					new DateTime(2019,01,day,08,00,00),
					default, default,
					default, default, default,
					default, false, false, false,
					default, default,
					default,default, default)).ToList();
			IReadOnlyCollection<Totals> result = @params.GetWorkerTotals(Rates.DefaultRates, TimeSpan.FromHours(44),TimeSpan.FromHours(44));
			Assert.Equal(TimeSpan.FromHours(40),result.Max(t => t.TimeSheetTotal.AccumulateWeekHours));
			Assert.Contains(new[] {8, 16, 24, 32, 40},
				i => result.Any(t => t.TimeSheetTotal.AccumulateWeekHours == TimeSpan.FromHours(i)));
		}
		
		[Fact]
		public void Accumulation_When_Is_Holiday_And_Is_Pay_Does_Not_Accumulate()
		{
			List<TotalizatorParams> @params = Enumerable.Range(1,5).Select(day => 
				new TotalizatorParams(default,
					default,
					new DateTime(2019,01,day,00,00,00), 
					new DateTime(2019,01,day,08,00,00),
					default,
					default,
					default, default, default, default,
					true, true, false,
					default,
					default,
					default,default, default)).ToList();
			IReadOnlyCollection<Totals> result = @params.GetWorkerTotals(Rates.DefaultRates, TimeSpan.FromHours(44),TimeSpan.FromHours(44));
			Assert.Equal(TimeSpan.FromHours(0),result.Max(t => t.TimeSheetTotal.AccumulateWeekHours));
			Assert.All(result,to => Assert.Equal(to.TimeSheetTotal.AccumulateWeekHours, TimeSpan.Zero));
		}
	}
}