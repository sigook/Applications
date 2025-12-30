using Covenant.Common.Entities.Accounting.PayStub;
using Covenant.TimeSheetTotal.Models;

namespace Covenant.PayStubs.Models
{
    public class PayStubsAndTotals
    {
        public IReadOnlyCollection<PayStub> PayStubs { get; }
        public IReadOnlyCollection<Totals> Totals { get; }

        public PayStubsAndTotals(IEnumerable<PayStub> payStubs, IEnumerable<Totals> totals)
        {
            PayStubs = new List<PayStub>(payStubs);
            Totals = new List<Totals>(totals);
        }
    }
}