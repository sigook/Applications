using Covenant.Common.Constants;
using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Models.Worker;
using Covenant.Common.Resources;
using Covenant.Common.Utils;
using Covenant.Common.Utils.Extensions;
using System.Text;

namespace Covenant.Common.Entities.Worker
{
    public class WorkerProfile :
        ISinInformation<CovenantFile>,
        IWorkerBasicInformation<Gender>,
        IEmergencyInformation,
        IWorkerDocumentsInformation<IdentificationType, CovenantFile>,
        IWorkerContactInformation<Location, City>,
        IWorkerProfileOtherInformation<Lift>
    {
        public WorkerProfile()
        {
        }

        public WorkerProfile(User user)
        {
            Worker = user ?? throw new ArgumentNullException(nameof(user));
            WorkerId = user.Id;
        }

        public WorkerProfile(User user, Guid agencyId) : this(user) => AgencyId = agencyId;

        public Guid Id { get; set; } = Guid.NewGuid();
        public int NumberId { get; set; }
        public Guid WorkerId { get; private set; }
        public Guid AgencyId { get; set; }
        public Guid? ProfileImageId { get; private set; }
        public string FirstName { get; private set; }
        public string MiddleName { get; private set; }
        public string LastName { get; private set; }
        public string SecondLastName { get; private set; }
        public DateTime BirthDay { get; private set; }
        public Guid? GenderId { get; private set; }
        public bool HasVehicle { get; private set; }
        public string SocialInsurance { get; set; }
        public string MaskedSocialInsurance => MaskSINNumber(SocialInsurance);
        public bool SocialInsuranceExpire { get; private set; }
        public DateTime? DueDate { get; private set; }
        public Guid? SocialInsuranceFileId { get; private set; }
        public string IdentificationNumber1 { get; set; }
        public Guid? IdentificationType1Id { get; set; }
        public Guid? IdentificationType1FileId { get; set; }
        public string IdentificationNumber2 { get; set; }
        public Guid? IdentificationType2Id { get; set; }
        public Guid? IdentificationType2FileId { get; set; }
        public bool HavePoliceCheckBackground { get; set; }
        public Guid? PoliceCheckBackGroundId { get; set; }
        public Guid? ResumeId { get; set; }
        public Guid LocationId { get; set; }
        public string MobileNumber { get; set; }
        public string Phone { get; set; }
        public int? PhoneExt { get; set; }
        public Guid? LiftId { get; set; }
        public bool HaveAnyHealthProblem { get; set; }
        public string HealthProblem { get; set; }
        public string ContactEmergencyPhone { get; set; }
        public string OtherHealthProblem { get; set; }
        public string ContactEmergencyName { get; set; }
        public string ContactEmergencyLastName { get; set; }
        public bool ApprovedToWork { get; set; }
        public bool Dnu { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }
        public bool IsSubcontractor { get; set; }
        public string PunchCardId { get; set; }
        public bool IsContractor { get; set; } = false;
        public string TextSearch { get; set; }
        public WorkerProfileTaxCategory WorkerProfileTaxCategory { get; set; }
        public Gender Gender { get; private set; }
        public User Worker { get; set; }
        public Agency.Agency Agency { get; set; }
        public CovenantFile ProfileImage { get; private set; }
        public CovenantFile SocialInsuranceFile { get; private set; }
        public IdentificationType IdentificationType1 { get; set; }
        public CovenantFile IdentificationType1File { get; set; }
        public CovenantFile IdentificationType2File { get; set; }
        public IdentificationType IdentificationType2 { get; set; }
        public CovenantFile PoliceCheckBackGround { get; set; }
        public CovenantFile Resume { get; set; }
        public Location Location { get; set; }
        public Lift Lift { get; set; }
        public List<WorkerProfileOtherDocument> OtherDocuments { get; set; } = new List<WorkerProfileOtherDocument>();
        public List<WorkerProfileNote> Notes { get; set; } = new List<WorkerProfileNote>();
        public ICollection<WorkerProfileAvailability> Availabilities { get; set; } = new List<WorkerProfileAvailability>();
        public ICollection<WorkerProfileAvailabilityTime> AvailabilityTimes { get; set; } = new List<WorkerProfileAvailabilityTime>();
        public ICollection<WorkerProfileAvailabilityDay> AvailabilityDays { get; set; } = new List<WorkerProfileAvailabilityDay>();
        public ICollection<WorkerProfileLocationPreference> LocationPreferences { get; set; } = new List<WorkerProfileLocationPreference>();
        public ICollection<WorkerProfileLanguage> Languages { get; set; } = new List<WorkerProfileLanguage>();
        public ICollection<WorkerProfileSkill> Skills { get; set; } = new List<WorkerProfileSkill>();
        public ICollection<WorkerProfileLicense> Licenses { get; set; } = new List<WorkerProfileLicense>();
        public ICollection<WorkerProfileCertificate> Certificates { get; set; } = new List<WorkerProfileCertificate>();
        public ICollection<WorkerProfileJobExperience> JobExperiences { get; set; } = new List<WorkerProfileJobExperience>();

        public event EventHandler<CovenantFile> OnNewDocumentAdded;

        public void UpdateTextSearch()
        {
            var b = new StringBuilder();
            if (NumberId != 0) b.Append($"{NumberId:000} ");
            b.Append($"{FirstName} {MiddleName} {LastName} {SecondLastName} ");
            if (!string.IsNullOrEmpty(Worker?.Email)) b.Append($"{Worker.Email} ");
            if (!string.IsNullOrEmpty(SocialInsurance)) b.Append($"{SocialInsurance} ");
            if (!string.IsNullOrEmpty(IdentificationNumber1)) b.Append($"{IdentificationNumber1} ");
            if (!string.IsNullOrEmpty(IdentificationNumber2)) b.Append($"{IdentificationNumber2} ");
            if (!string.IsNullOrEmpty(MobileNumber)) b.Append($"{MobileNumber} ");
            if (!string.IsNullOrEmpty(Phone)) b.Append($"{Phone} ");
            if (!string.IsNullOrEmpty(Gender?.Value)) b.Append($"{Gender.Value} ");
            if (HasVehicle) b.Append("HasVehicle ");
            b.Append(ApprovedToWork ? "Approved " : "NotApproved ");
            b.Append(IsSubcontractor ? "Subcontractor " : "NotSubcontractor ");
            b.Append(string.IsNullOrEmpty(SocialInsurance) ? "NoSIN " : "HasSIN ");
            TextSearch = b.ToString();
        }

        public Result PatchContactInformation<TLocation, TCity>(IWorkerContactInformation<TLocation, TCity> contactInformation)
            where TLocation : ILocation<TCity>
            where TCity : ICatalog<Guid>
        {
            if (contactInformation is null) throw new ArgumentNullException(nameof(contactInformation));
            if (default(TLocation)?.Equals(contactInformation.Location) ?? false) return Result.Fail(ValidationMessages.RequiredMsg(ApiResources.Location));
            var rLocation = Location.Create(contactInformation.Location.City.Id, contactInformation.Location.Address, contactInformation.Location.PostalCode);
            if (!rLocation) return Result.Fail(rLocation.Errors);
            if (Location is null)
            {
                Location = rLocation.Value;
                LocationId = Location.Id;
            }
            else Location.Update(rLocation.Value);

            if (!CommonValidators.IsValidPhoneNumber(contactInformation.MobileNumber))
                return Result.Fail(ValidationMessages.InvalidFormatMsg(ApiResources.MobileNumber));

            if (!CommonValidators.IsValidPhoneNumber(contactInformation.Phone))
                return Result.Fail(ValidationMessages.InvalidFormatMsg(ApiResources.Phone));

            MobileNumber = contactInformation.MobileNumber;
            Phone = contactInformation.Phone;
            PhoneExt = contactInformation.PhoneExt;
            return Result.Ok();
        }

        public Result PatchBasicInformation<TGender>(IWorkerBasicInformation<TGender> information)
            where TGender : ICatalog<Guid>
        {
            if (information is null) throw new ArgumentNullException(nameof(information));
            if (!information.FirstName.IsValidLength(CovenantConstants.Validation.FirstNameMinLength, CovenantConstants.Validation.FirstNameMaxLength))
                return Result.Fail(ValidationMessages.LengthMsg(ApiResources.FirstName, CovenantConstants.Validation.FirstNameMinLength, CovenantConstants.Validation.FirstNameMaxLength));
            if (!information.LastName.IsValidLength(CovenantConstants.Validation.LastNameMinLength, CovenantConstants.Validation.LastNameMaxLength))
                return Result.Fail(ValidationMessages.LengthMsg(ApiResources.LastName, CovenantConstants.Validation.LastNameMinLength, CovenantConstants.Validation.LastNameMaxLength));
            if (!string.IsNullOrEmpty(information.MiddleName))
            {
                if (!information.MiddleName.IsValidLength(CovenantConstants.Validation.FirstNameMinLength, CovenantConstants.Validation.FirstNameMaxLength))
                    return Result.Fail(ValidationMessages.LengthMsg(ApiResources.MiddleName, CovenantConstants.Validation.FirstNameMinLength, CovenantConstants.Validation.FirstNameMaxLength));
            }

            if (!string.IsNullOrEmpty(information.SecondLastName))
            {
                if (!information.SecondLastName.IsValidLength(CovenantConstants.Validation.FirstNameMinLength, CovenantConstants.Validation.FirstNameMaxLength))
                    return Result.Fail(ValidationMessages.LengthMsg(ApiResources.SecondLastName, CovenantConstants.Validation.FirstNameMinLength, CovenantConstants.Validation.FirstNameMaxLength));
            }

            //TODO write unit test
            if (information.BirthDay > DateTime.Now.Date.AddYears(-CovenantConstants.Validation.BirthdayMinimumAge)) return Result.Fail(ValidationMessages.AgeMsg(CovenantConstants.Validation.BirthdayMinimumAge));

            FirstName = information.FirstName;
            MiddleName = information.MiddleName;
            LastName = information.LastName;
            SecondLastName = information.SecondLastName;
            BirthDay = information.BirthDay;
            Guid? genderId = information.Gender?.Id;
            if (genderId == Guid.Empty) genderId = null;
            GenderId = genderId;
            HasVehicle = information.HasVehicle;
            return Result.Ok();
        }

        public Result PatchSinInformation<TFile>(ISinInformation<TFile> sinInformation) where TFile : ICovenantFile
        {
            if (sinInformation is null) throw new ArgumentNullException();
            if (!string.IsNullOrEmpty(sinInformation.SocialInsurance))
            {
                int length = sinInformation.SocialInsurance.Length;
                if (length < CovenantConstants.Validation.SocialInsuranceMinLength || length > CovenantConstants.Validation.SocialInsuranceMaxLength)
                    return Result.Fail(ValidationMessages.LengthMsg(ApiResources.SocialInsurance, CovenantConstants.Validation.SocialInsuranceMinLength, CovenantConstants.Validation.SocialInsuranceMaxLength));
                if (sinInformation.SocialInsuranceExpire)
                {
                    if (sinInformation.DueDate is null) return Result.Fail(ValidationMessages.RequiredMsg(ApiResources.SocialInsuranceDate));
                }
            }

            SocialInsurance = sinInformation.SocialInsurance;
            SocialInsuranceExpire = sinInformation.SocialInsuranceExpire;
            DueDate = sinInformation.DueDate;
            if (string.IsNullOrEmpty(sinInformation.SocialInsuranceFile?.FileName))
            {
                SocialInsuranceFile = null;
                SocialInsuranceFileId = null;
            }
            else
            {
                if (SocialInsuranceFile is null)
                {
                    SocialInsuranceFile = new CovenantFile(sinInformation.SocialInsuranceFile.FileName, sinInformation.SocialInsuranceFile.Description);
                    SocialInsuranceFileId = SocialInsuranceFile.Id;
                    OnNewDocumentAdded?.Invoke(this, SocialInsuranceFile);
                }
                else
                {
                    SocialInsuranceFile.FileName = sinInformation.SocialInsuranceFile.FileName;
                    SocialInsuranceFile.Description = sinInformation.SocialInsuranceFile.Description;
                }
            }

            return Result.Ok();
        }

        public Result PatchProfileImage(ICovenantFile profileImage)
        {
            if (string.IsNullOrEmpty(profileImage?.FileName)) return Result.Ok();
            if (ProfileImage is null)
            {
                ProfileImage = new CovenantFile(profileImage.FileName, profileImage.Description);
                ProfileImageId = ProfileImage.Id;
                OnNewDocumentAdded?.Invoke(this, ProfileImage);
            }
            else
            {
                if (ProfileImage.FileName == profileImage.FileName) return Result.Ok();
                ProfileImage.Update(profileImage);
            }

            return Result.Ok();
        }

        public Result PatchDocuments<TTypeOfDocument, TFile>(IWorkerDocumentsInformation<TTypeOfDocument, TFile> documentsInformation)
            where TTypeOfDocument : ICatalog<Guid> where TFile : ICovenantFile
        {
            if (documentsInformation is null) throw new ArgumentNullException(nameof(documentsInformation));
            IdentificationNumber1 = documentsInformation.IdentificationNumber1;
            IdentificationType1Id = documentsInformation.IdentificationType1?.Id;
            if (string.IsNullOrEmpty(documentsInformation.IdentificationType1File?.FileName))
            {
                IdentificationType1File = null;
                IdentificationType1FileId = null;
            }
            else
            {
                if (IdentificationType1File is null)
                {
                    IdentificationType1File = new CovenantFile();
                    IdentificationType1File.Update(documentsInformation.IdentificationType1File);
                    IdentificationType1FileId = IdentificationType1File.Id;
                    OnNewDocumentAdded?.Invoke(this, IdentificationType1File);
                }
                else
                {
                    IdentificationType1File.Update(documentsInformation.IdentificationType1File);
                }
            }

            IdentificationNumber2 = documentsInformation.IdentificationNumber2;
            IdentificationType2Id = documentsInformation.IdentificationType2?.Id;
            if (string.IsNullOrEmpty(documentsInformation.IdentificationType2File?.FileName))
            {
                IdentificationType2File = null;
                IdentificationType2FileId = null;
            }
            else
            {
                if (IdentificationType2File is null)
                {
                    IdentificationType2File = new CovenantFile();
                    IdentificationType2File.Update(documentsInformation.IdentificationType2File);
                    IdentificationType2FileId = IdentificationType2File.Id;
                    OnNewDocumentAdded?.Invoke(this, IdentificationType2File);
                }
                else
                {
                    IdentificationType2File.Update(documentsInformation.IdentificationType2File);
                }
            }

            HavePoliceCheckBackground = documentsInformation.HavePoliceCheckBackground;
            if (string.IsNullOrEmpty(documentsInformation.PoliceCheckBackGround?.FileName))
            {
                PoliceCheckBackGround = null;
                PoliceCheckBackGroundId = null;
            }
            else
            {
                if (PoliceCheckBackGround is null)
                {
                    PoliceCheckBackGround = new CovenantFile();
                    PoliceCheckBackGround.Update(documentsInformation.PoliceCheckBackGround);
                    PoliceCheckBackGroundId = PoliceCheckBackGround.Id;
                    OnNewDocumentAdded?.Invoke(this, PoliceCheckBackGround);
                }
                else
                {
                    PoliceCheckBackGround.Update(documentsInformation.PoliceCheckBackGround);
                }
            }

            if (!string.IsNullOrEmpty(documentsInformation.Resume?.FileName))
            {
                if (Resume is null)
                {
                    Resume = new CovenantFile();
                    Resume.Update(documentsInformation.Resume);
                    ResumeId = Resume.Id;
                    OnNewDocumentAdded?.Invoke(this, Resume);
                }
                else
                {
                    Resume.Update(documentsInformation.Resume);
                }
            }
            return Result.Ok();
        }

        public Result PatchResume(CovenantFileModel resume)
        {
            if (!string.IsNullOrEmpty(resume?.FileName))
            {
                if (Resume is null)
                {
                    Resume = new CovenantFile();
                    Resume.Update(resume);
                    ResumeId = Resume.Id;
                    OnNewDocumentAdded?.Invoke(this, Resume);
                }
                else
                {
                    Resume.Update(resume);
                }
            }

            return Result.Ok();
        }

        public Result PatchEmergencyInformation(IEmergencyInformation emergencyInformation)
        {
            if (emergencyInformation is null) throw new ArgumentNullException(nameof(emergencyInformation));
            if (!string.IsNullOrEmpty(emergencyInformation.ContactEmergencyName))
            {
                if (!emergencyInformation.ContactEmergencyName.IsValidLength(CovenantConstants.Validation.ContactEmergencyMinLength, CovenantConstants.Validation.ContactEmergencyMaxLength))
                    return Result.Fail(ValidationMessages.LengthMsg(ApiResources.ContactEmergencyName, CovenantConstants.Validation.ContactEmergencyMinLength, CovenantConstants.Validation.ContactEmergencyMaxLength));
            }

            if (!string.IsNullOrEmpty(emergencyInformation.ContactEmergencyLastName))
            {
                if (!emergencyInformation.ContactEmergencyLastName.IsValidLength(CovenantConstants.Validation.ContactEmergencyMinLength, CovenantConstants.Validation.ContactEmergencyMaxLength))
                    return Result.Fail(ValidationMessages.LengthMsg(ApiResources.ContactEmergencyLastName, CovenantConstants.Validation.ContactEmergencyMinLength, CovenantConstants.Validation.ContactEmergencyMaxLength));
            }

            if (!CommonValidators.IsValidPhoneNumber(emergencyInformation.ContactEmergencyPhone))
                return Result.Fail(ValidationMessages.InvalidFormatMsg(ApiResources.ContactEmergencyPhone));

            if (!string.IsNullOrEmpty(emergencyInformation.HealthProblem))
            {
                int length = emergencyInformation.HealthProblem.Length;
                if (length > CovenantConstants.Validation.HealthProblemMaxLength) return Result.Fail(ValidationMessages.LengthMaxMsg(ApiResources.HealthProblem, CovenantConstants.Validation.HealthProblemMaxLength));
            }

            if (!string.IsNullOrEmpty(emergencyInformation.OtherHealthProblem))
            {
                int length = emergencyInformation.OtherHealthProblem.Length;
                if (length > CovenantConstants.Validation.HealthProblemMaxLength) return Result.Fail(ValidationMessages.LengthMaxMsg(ApiResources.OtherHealthProblem, CovenantConstants.Validation.HealthProblemMaxLength));
            }

            ContactEmergencyName = emergencyInformation.ContactEmergencyName;
            ContactEmergencyLastName = emergencyInformation.ContactEmergencyLastName;
            ContactEmergencyPhone = emergencyInformation.ContactEmergencyPhone;
            HaveAnyHealthProblem = emergencyInformation.HaveAnyHealthProblem;
            HealthProblem = emergencyInformation.HealthProblem;
            OtherHealthProblem = emergencyInformation.OtherHealthProblem;
            return Result.Ok();
        }

        public Result PatchAvailabilities(IReadOnlyCollection<ICatalog<Guid>> availabilities)
        {
            IfIsNotInRemove(Availabilities, a => availabilities.Any(i => i.Id == a.AvailabilityId));
            foreach (var availability in availabilities)
            {
                if (availability.Id == Guid.Empty) continue;
                if (Availabilities.Any(a => a.AvailabilityId == availability.Id)) continue;
                Availabilities.Add(new WorkerProfileAvailability { AvailabilityId = availability.Id, WorkerProfile = this });
            }

            return Result.Ok();
        }

        public Result PatchAvailabilityTimes(IReadOnlyCollection<ICatalog<Guid>> availabilityTimes)
        {
            IfIsNotInRemove(AvailabilityTimes, t => availabilityTimes.Any(c => c.Id == t.AvailabilityTimeId));
            foreach (var time in availabilityTimes)
            {
                if (time.Id == Guid.Empty) continue;
                if (AvailabilityTimes.Any(a => a.AvailabilityTimeId == time.Id)) continue;
                AvailabilityTimes.Add(new WorkerProfileAvailabilityTime { AvailabilityTimeId = time.Id, WorkerProfile = this });
            }

            return Result.Ok();
        }

        public Result PatchAvailabilityDays(IReadOnlyCollection<ICatalog<Guid>> days)
        {
            IfIsNotInRemove(AvailabilityDays, day => days.Any(i => i.Id == day.DayId));
            foreach (var day in days)
            {
                if (day.Id == Guid.Empty) continue;
                if (AvailabilityDays.Any(a => a.DayId == day.Id)) continue;
                AvailabilityDays.Add(new WorkerProfileAvailabilityDay { DayId = day.Id, WorkerProfile = this });
            }

            return Result.Ok();
        }

        public Result PatchLocationPreferences(IReadOnlyCollection<ICatalog<Guid>> cities)
        {
            IfIsNotInRemove(LocationPreferences, p => cities.Any(i => i.Id == p.CityId));
            foreach (var city in cities)
            {
                if (city.Id == Guid.Empty) continue;
                if (LocationPreferences.Any(l => l.CityId == city.Id)) continue;
                LocationPreferences.Add(new WorkerProfileLocationPreference { CityId = city.Id, WorkerProfile = this });
            }

            return Result.Ok();
        }

        public Result PatchLanguages(IReadOnlyCollection<ICatalog<Guid>> languages)
        {
            IfIsNotInRemove(Languages, l => languages.Any(ca => ca.Id == l.LanguageId));
            foreach (var language in languages)
            {
                if (language.Id == Guid.Empty) continue;
                if (Languages.Any(l => l.LanguageId == language.Id)) continue;
                Languages.Add(new WorkerProfileLanguage { LanguageId = language.Id, WorkerProfile = this });
            }

            return Result.Ok();
        }

        public Result PatchSkills(IReadOnlyCollection<string> skills)
        {
            Skills.Clear();
            foreach (string skill in skills)
            {
                Skills.Add(new WorkerProfileSkill { Skill = skill, WorkerProfile = this });
            }
            return Result.Ok();
        }

        public Result PatchLicenses<TFile>(IReadOnlyCollection<IWorkerProfileLicense<TFile>> licenses)
            where TFile : ICovenantFile
        {
            IfIsNotInRemove(Licenses, l => licenses.Any(m => m.License.FileName == l.License.FileName));
            foreach (IWorkerProfileLicense<TFile> i in licenses)
            {
                if (string.IsNullOrEmpty(i.License?.FileName)) return Result.Fail(ValidationMessages.RequiredMsg(ApiResources.LicenseFile));
                WorkerProfileLicense license = Licenses.FirstOrDefault(f => f.License.FileName == i.License.FileName);
                if (license is null)
                {
                    var file = new CovenantFile(i.License.FileName, i.License.Description);
                    Licenses.Add(new WorkerProfileLicense
                    {
                        License = file,
                        Expires = i.Expires,
                        Issued = i.Issued,
                        Number = i.Number,
                        LicenseId = file.Id,
                        WorkerProfile = this
                    });
                    continue;
                }

                license.Number = i.Number;
                license.Expires = i.Expires;
                license.Issued = i.Issued;
            }

            return Result.Ok();
        }

        public Result PatchCertificates(IReadOnlyCollection<ICovenantFile> certificates)
        {
            IfIsNotInRemove(Certificates, c => certificates.Any(m => m.FileName == c.Certificate.FileName));
            foreach (ICovenantFile i in certificates)
            {
                if (string.IsNullOrEmpty(i.FileName)) return Result.Fail(ValidationMessages.RequiredMsg(ApiResources.Certificates));
                if (Certificates.Any(c => c.Certificate.FileName == i.FileName)) continue;
                var file = new CovenantFile(i.FileName, i.Description);
                Certificates.Add(new WorkerProfileCertificate { Certificate = file, CertificateId = file.Id, WorkerProfile = this });
            }

            return Result.Ok();
        }

        public Result PatchOtherInformation<TLift>(IWorkerProfileOtherInformation<TLift> otherInformation)
            where TLift : ICatalog<Guid>
        {
            LiftId = otherInformation.Lift?.Id;
            return Result.Ok();
        }

        public Result<WorkerProfileJobExperience> AddJobExperience(IWorkerProfileJobExperience jobExperienceModel)
        {
            Result<WorkerProfileJobExperience> result = WorkerProfileJobExperience.Create(jobExperienceModel.Company,
                jobExperienceModel.Supervisor, jobExperienceModel.Duties,
                jobExperienceModel.StartDate, jobExperienceModel.EndDate, jobExperienceModel.IsCurrentJobPosition);
            if (!result) return result;
            WorkerProfileJobExperience jobExperience = result.Value;
            jobExperience.WorkerProfile = this;
            jobExperience.WorkerProfileId = this.Id;
            JobExperiences.Add(result.Value);
            return result;
        }

        public Result UpdateJobExperience(Guid id, IWorkerProfileJobExperience jobExperienceModel)
        {
            Result<WorkerProfileJobExperience> result = WorkerProfileJobExperience.Create(jobExperienceModel.Company,
                jobExperienceModel.Supervisor, jobExperienceModel.Duties, jobExperienceModel.StartDate,
                jobExperienceModel.EndDate, jobExperienceModel.IsCurrentJobPosition);
            if (!result) return result;
            WorkerProfileJobExperience current = JobExperiences.FirstOrDefault(e => e.Id == id);
            current?.Update(result.Value);
            return Result.Ok();
        }

        public Result AddOtherDocuments(IReadOnlyCollection<ICovenantFile> documents)
        {
            IfIsNotInRemove(OtherDocuments, d => documents.Any(c => c.FileName == d.Document.FileName));
            foreach (ICovenantFile document in documents)
            {
                if (OtherDocuments.Any(d => d.Document.FileName == document.FileName)) continue;
                var rDoc = CovenantFile.Create(document);
                if (!rDoc) return rDoc;
                var result = WorkerProfileOtherDocument.Create(Id, rDoc.Value);
                if (!result) return result;
                OtherDocuments.Add(result.Value);
            }
            return Result.Ok();
        }

        public Result DeleteJobExperience(Guid id)
        {
            WorkerProfileJobExperience toDelete = JobExperiences.FirstOrDefault(c => c.Id == id);
            if (toDelete is null) return Result.Fail();
            JobExperiences.Remove(toDelete);
            return Result.Ok();
        }

        public Result CanApply(DateTime now)
        {
            if (!ApprovedToWork) return Result.Fail("You aren't approved to work");

            if (string.IsNullOrEmpty(SocialInsurance))
                return Result.Fail("Please add your social insurance number");
            if (SocialInsuranceFileId is null)
                return Result.Fail("Please add your social insurance document file");

            if (string.IsNullOrEmpty(IdentificationNumber1))
                return Result.Fail("Please add the required documents (document number 1)");
            if (!IdentificationType1FileId.HasValue)
                return Result.Fail("Please add the required documents (document file 1)");

            if (string.IsNullOrEmpty(IdentificationNumber2))
                return Result.Fail("Please add the required documents (document number 2)");
            if (!IdentificationType2FileId.HasValue)
                return Result.Fail("Please add the required documents (document file 2)");

            if (!SocialInsuranceExpire) return Result.Ok();
            if (DueDate.GetValueOrDefault().Date < now.Date) return Result.Fail("Your social insurance has expired");
            return Result.Ok();
        }

        public Result CanBeBook(DateTime now)
        {
            if (IsSubcontractor) return Result.Ok(); //Every subcontractor can be book because the partner already validate the documents
            if (IsContractor) return Result.Ok(); //Every contractor can be book because they don't need to have documents
            if (!ApprovedToWork) return Result.Fail("Worker is not approved to work");
            return HasRequiredDocuments(now);
        }

        public Result UpdateApprovedToWork(DateTime now)
        {
            if (ApprovedToWork)
            {
                ApprovedToWork = false;
                return Result.Ok();
            }

            if (IsSubcontractor)
            {
                ApprovedToWork = true;
                return Result.Ok();
            }

            if (IsContractor)
            {
                ApprovedToWork = true;
                return Result.Ok();
            }
            if (string.IsNullOrEmpty(ProfileImage?.FileName)) return Result.Fail("Worker doesn't have profile image");
            var hasTaxCategory = HasTaxCategory();
            if (!hasTaxCategory) return hasTaxCategory;
            Result rHasRequiredDocuments = HasRequiredDocuments(now);
            if (!rHasRequiredDocuments) return rHasRequiredDocuments;
            ApprovedToWork = true;
            return Result.Ok();
        }

        private Result HasRequiredDocuments(DateTime now)
        {
            if (string.IsNullOrEmpty(SocialInsurance)) return Result.Fail("Worker doesn't have social insurance number");
            if (!SocialInsuranceExpire) return Result.Ok();
            if (DueDate.GetValueOrDefault().Date < now.Date) return Result.Fail("Social insurance has expired");
            return Result.Ok();
        }

        private Result HasTaxCategory()
        {
            if (!Location.IsUSA)
            {
                if (WorkerProfileTaxCategory?.FederalCategory == null && WorkerProfileTaxCategory?.ProvincialCategory == null)
                {
                    return Result.Fail("Taxes classes have not been configurated");
                }
            }
            return Result.Ok();
        }

        public string FullName =>
            FirstName +
            (string.IsNullOrWhiteSpace(MiddleName) ? string.Empty : $" {MiddleName}") +
            $" {LastName}" +
            (string.IsNullOrWhiteSpace(SecondLastName) ? string.Empty : $" {SecondLastName}");

        public static string MaskSINNumber(string sin)
        {
            if (string.IsNullOrEmpty(sin)) return sin;
            return sin.Length <= 6 ? $"{sin}-XXX" : $"{sin.Substring(0, 6)}-XXX";
        }

        private static void IfIsNotInRemove<T>(ICollection<T> list, Func<T, bool> filter)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                T elementAt = list.ElementAt(i);
                if (filter(elementAt)) continue;
                list.Remove(elementAt);
            }
        }

        public void UpdateDnu() => Dnu = !Dnu;

        public Result UpdateContractor(DateTime now)
        {
            if (IsContractor)
            {
                Result hasRequiredDocuments = HasRequiredDocuments(now);
                if (!hasRequiredDocuments) return hasRequiredDocuments;
                IsContractor = false;
                return Result.Ok();
            }

            IsContractor = true;
            return Result.Ok();
        }

        public Result UpdateSubcontractor(DateTime now, bool? isSubcontractor = null)
        {
            if (!isSubcontractor.HasValue) isSubcontractor = !IsSubcontractor;
            if (isSubcontractor.Value)
            {
                IsSubcontractor = true;
                return Result.Ok();
            }

            Result hasRequiredDocuments = HasRequiredDocuments(now);
            if (!hasRequiredDocuments) return hasRequiredDocuments;
            IsSubcontractor = false;
            return Result.Ok();
        }
    }
}