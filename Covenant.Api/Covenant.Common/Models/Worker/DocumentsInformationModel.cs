namespace Covenant.Common.Models.Worker
{
    public class DocumentsInformationModel : IWorkerDocumentsInformation<BaseModel<Guid>, CovenantFileModel>
    {
        public string IdentificationNumber1 { get; set; }
        public BaseModel<Guid> IdentificationType1 { get; set; }
        public CovenantFileModel IdentificationType1File { get; set; }
        public string IdentificationNumber2 { get; set; }
        public BaseModel<Guid> IdentificationType2 { get; set; }
        public CovenantFileModel IdentificationType2File { get; set; }
        public bool HavePoliceCheckBackground { get; set; }
        public CovenantFileModel PoliceCheckBackGround { get; set; }
        public CovenantFileModel Resume { get; set; }
    }
}