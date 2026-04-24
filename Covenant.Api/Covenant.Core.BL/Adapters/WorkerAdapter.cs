using Covenant.Common.Entities;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Interfaces.Adapters;
using Covenant.Common.Models.Worker;

namespace Covenant.Core.BL.Adapters;

public class WorkerAdapter : IWorkerAdapter
{
    public WorkerProfile MapToWorkerProfile(WorkerProfileCreateModel model, Common.Entities.Agency.Agency agency, User user)
    {
        var entity = new WorkerProfile
        {
            AgencyId = agency.Id,
            Agency = agency,
            Worker = user,
            WorkerId = user.Id,
            ApprovedToWork = false,
            FirstName = model.FirstName,
            MiddleName = model.MiddleName,
            LastName = model.LastName,
            SecondLastName = model.SecondLastName,
            BirthDay = model.BirthDay,
            GenderId = model.Gender?.Id == Guid.Empty ? null : model.Gender?.Id,
            HasVehicle = model.HasVehicle,
            MobileNumber = model.MobileNumber,
            Phone = model.Phone,
            PhoneExt = model.PhoneExt,
            Location = new Location
            {
                CityId = model.Location.City.Id,
                Address = model.Location.Address,
                PostalCode = model.Location.PostalCode
            },
            LiftId = model.Lift?.Id,
            IdentificationNumber1 = model.IdentificationNumber1,
            IdentificationType1Id = model.IdentificationType1?.Id,
            IdentificationNumber2 = model.IdentificationNumber2,
            IdentificationType2Id = model.IdentificationType2?.Id,
            HaveAnyHealthProblem = model.HaveAnyHealthProblem,
            HealthProblem = model.HealthProblem,
            OtherHealthProblem = model.OtherHealthProblem,
            ContactEmergencyName = model.ContactEmergencyName,
            ContactEmergencyLastName = model.ContactEmergencyLastName,
            ContactEmergencyPhone = model.ContactEmergencyPhone
        };
        entity.LocationId = entity.Location.Id;

        if (!string.IsNullOrEmpty(model.ProfileImage?.FileName))
        {
            entity.ProfileImage = new CovenantFile(model.ProfileImage.FileName, model.ProfileImage.Description);
            entity.ProfileImageId = entity.ProfileImage.Id;
        }

        if (!string.IsNullOrEmpty(model.IdentificationType1File?.FileName))
        {
            entity.IdentificationType1File = new CovenantFile(model.IdentificationType1File.FileName, model.IdentificationType1File.Description);
            entity.IdentificationType1FileId = entity.IdentificationType1File.Id;
        }

        if (!string.IsNullOrEmpty(model.IdentificationType2File?.FileName))
        {
            entity.IdentificationType2File = new CovenantFile(model.IdentificationType2File.FileName, model.IdentificationType2File.Description);
            entity.IdentificationType2FileId = entity.IdentificationType2File.Id;
        }

        if (!string.IsNullOrEmpty(model.PoliceCheckBackGround?.FileName))
        {
            entity.PoliceCheckBackGround = new CovenantFile(model.PoliceCheckBackGround.FileName, model.PoliceCheckBackGround.Description);
            entity.PoliceCheckBackGroundId = entity.PoliceCheckBackGround.Id;
        }

        if (!string.IsNullOrEmpty(model.Resume?.FileName))
        {
            entity.Resume = new CovenantFile(model.Resume.FileName, model.Resume.Description);
            entity.ResumeId = entity.Resume.Id;
        }

        entity.Availabilities = [.. model.Availabilities
            .Where(a => a.Id != Guid.Empty)
            .Select(a => new WorkerProfileAvailability { AvailabilityId = a.Id, WorkerProfile = entity })];

        entity.AvailabilityTimes = [.. model.AvailabilityTimes
            .Where(t => t.Id != Guid.Empty)
            .Select(t => new WorkerProfileAvailabilityTime { AvailabilityTimeId = t.Id, WorkerProfile = entity })];

        entity.AvailabilityDays = [.. model.AvailabilityDays
            .Where(d => d.Id != Guid.Empty)
            .Select(d => new WorkerProfileAvailabilityDay { DayId = d.Id, WorkerProfile = entity })];

        entity.LocationPreferences = [.. model.LocationPreferences
            .Where(c => c.Id != Guid.Empty)
            .Select(c => new WorkerProfileLocationPreference { CityId = c.Id, WorkerProfile = entity })];

        entity.Languages = [.. model.Languages
            .Where(l => l.Id != Guid.Empty)
            .Select(l => new WorkerProfileLanguage { LanguageId = l.Id, WorkerProfile = entity })];

        entity.Skills = [.. model.Skills.Select(s => new WorkerProfileSkill { Skill = s.Skill, WorkerProfile = entity })];

        entity.Licenses = [.. model.Licenses
            .Select(l =>
            {
                var file = new CovenantFile(l.License.FileName, l.License.Description);
                return new WorkerProfileLicense
                {
                    License = file,
                    LicenseId = file.Id,
                    Number = l.Number,
                    Issued = l.Issued,
                    Expires = l.Expires,
                    WorkerProfile = entity
                };
            })];

        entity.Certificates = [.. model.Certificates
            .Select(c =>
            {
                var file = new CovenantFile(c.FileName, c.Description);
                return new WorkerProfileCertificate
                {
                    Certificate = file,
                    CertificateId = file.Id,
                    WorkerProfile = entity
                };
            })];

        foreach (var document in model.OtherDocuments)
        {
            if (string.IsNullOrEmpty(document?.FileName)) continue;
            var file = new CovenantFile(document.FileName, document.Description);
            entity.OtherDocuments.Add(WorkerProfileOtherDocument.Create(entity.Id, file).Value);
        }

        return entity;
    }
}
