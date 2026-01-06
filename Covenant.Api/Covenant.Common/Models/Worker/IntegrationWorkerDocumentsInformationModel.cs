using Covenant.Common.Entities;

namespace Covenant.Common.Models.Worker
{
    public class IntegrationWorkerDocumentsInformationModel : IWorkerDocumentsInformation<BaseModel<Guid>, CovenantFile>
    {
        public string IdentificationNumber1 { get; set; }
        public BaseModel<Guid> IdentificationType1 { get; set; }
        public CovenantFile IdentificationType1File { get; set; }
        public string IdentificationNumber2 { get; set; }
        public BaseModel<Guid> IdentificationType2 { get; set; }
        public CovenantFile IdentificationType2File { get; set; }
        public bool HavePoliceCheckBackground { get; set; }
        public CovenantFile PoliceCheckBackGround { get; set; }
        public CovenantFile Resume { get; set; }
    }
}