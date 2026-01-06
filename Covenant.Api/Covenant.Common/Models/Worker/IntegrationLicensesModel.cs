using Covenant.Common.Entities;

namespace Covenant.Common.Models.Worker
{
    public class IntegrationLicensesModel : IWorkerProfileLicense<CovenantFile>
    {
        public CovenantFile License { get; set; }
        public string Number { get; set; }
        public DateTime? Issued { get; set; }
        public DateTime? Expires { get; set; }
    }
}