namespace Covenant.Common.Models.Worker
{
    public class WorkerProfileBasicInformationModel : IWorkerBasicInformation<BaseModel<Guid>>
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string SecondLastName { get; set; }
        public DateTime BirthDay { get; set; }
        public BaseModel<Guid> Gender { get; set; }
        public bool HasVehicle { get; set; }
    }
}