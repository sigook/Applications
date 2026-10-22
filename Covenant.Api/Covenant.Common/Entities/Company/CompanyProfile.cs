using Covenant.Common.Entities.Agency;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Resources;

namespace Covenant.Common.Entities.Company
{
    public class CompanyProfile
    {
        private const int CommentMaximum = 5000;

        public const string DefaultImageLogo = "company.png";

        public static readonly TimeSpan DefaultOvertimeStarts = TimeSpan.FromHours(44);
        public static readonly TimeSpan MinimumOvertimeStarts = TimeSpan.FromHours(40);
        public const int DefaultPaymentTermDays = 30;

        public CompanyProfile()
        {
        }

        public CompanyProfile(User company, Agency.Agency agency, string fullName, string phone, CompanyProfileIndustry industry)
        {
            Company = company ?? throw new ArgumentNullException(nameof(company));
            CompanyId = company.Id;
            Agency = agency;
            FullName = fullName;
            Phone = phone;
            Industry = industry;
        }

        public Guid Id { get; internal set; } = Guid.NewGuid();
        public User Company { get; internal set; }
        public Guid CompanyId { get; private set; }
        public Agency.Agency Agency { get; internal set; }
        public Guid AgencyId { get; internal set; }
        public int NumberId { get; private set; }
        public Guid? LogoId { get; private set; }
        public CovenantFile Logo { get; internal set; } = new CovenantFile(DefaultImageLogo);
        public string FullName { get; internal set; }
        public string Phone { get; private set; }
        public int? PhoneExt { get; private set; }
        public string Fax { get; private set; }
        public int? FaxExt { get; private set; }
        public string Website { get; private set; }
        public string About { get; private set; }
        public string InternalInfo { get; private set; }
        public string ContactRole { get; private set; }
        public string ContactName { get; private set; }
        public CompanyStatus CompanyStatus { get; private set; }
        public bool RequiresPermissionToSeeRequests { get; private set; }
        public ICollection<CompanyProfileLocation> Locations { get; internal set; } = new List<CompanyProfileLocation>();
        public ICollection<CompanyProfileContactPerson> ContactPeople { get; private set; } = new List<CompanyProfileContactPerson>();
        public ICollection<CompanyProfileJobPositionRate> JobPositionRates { get; private set; } = new List<CompanyProfileJobPositionRate>();
        public ICollection<CompanyProfileDocument> Documents { get; private set; } = new List<CompanyProfileDocument>();
        public ICollection<CompanyProfileNote> Notes { get; private set; } = new List<CompanyProfileNote>();
        public CompanyProfileIndustry Industry { get; internal set; }
        public Guid? SalesRepresentativeId { get; private set; }
        public AgencyPersonnel SalesRepresentative { get; private set; }
        public Guid IndustryId { get; private set; }
        public string CreatedBy { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.Now;
        public string UpdatedBy { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public bool Active { get; internal set; }
        public bool PaidHolidays { get; private set; }
        public bool RequiredPaymentMethod { get; private set; }
        public bool? VaccinationRequired { get; private set; }
        public string VaccinationRequiredComments { get; private set; }
        public TimeSpan OvertimeStartsAfter { get; private set; } = DefaultOvertimeStarts;
        public int PaymentTermDays { get; private set; } = DefaultPaymentTermDays;
        public Location BillingAddress => Locations
            .FirstOrDefault(l => l.IsBilling)?.Location ?? Locations.FirstOrDefault()?.Location;
        public string FormattedPhone => $"{Phone} {(PhoneExt.HasValue ? $"Ext:{PhoneExt.Value}" : string.Empty)}";
        public string FormattedFax => $"{Fax} {(FaxExt.HasValue ? $"Ext:{FaxExt.Value}" : string.Empty)}";

        public Result UpdateOvertimeStartsAfter(TimeSpan start)
        {
            if (start < MinimumOvertimeStarts) return Result.Fail($"Start must be greater than {MinimumOvertimeStarts.TotalHours}");
            OvertimeStartsAfter = start;
            return Result.Ok();
        }

        public void AddLocation(Location location, bool isBilling)
        {
            Locations.Add(new CompanyProfileLocation
            {
                CompanyProfile = this,
                CompanyProfileId = this.Id,
                IsBilling = isBilling,
                Location = location,
                LocationId = location.Id
            });
        }

        public void AddJobPositionRate(CompanyProfileJobPositionRate jobPositionRate) => JobPositionRates.Add(jobPositionRate);

        public void UpdateLogo(string fileName, string description)
        {
            if (Logo is null)
            {
                Logo = new CovenantFile(fileName, description);
                LogoId = Logo.Id;
            }
            else
            {
                Logo.FileName = fileName;
                Logo.Description = description;
            }
        }

        public Result UpdateName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return Result.Fail(ValidationMessages.RequiredMsg(ApiResources.BusinessName));
            FullName = name;
            return Result.Ok();
        }

        public Result UpdateVaccinationInfo(bool? required, string comments)
        {
            VaccinationRequired = required;
            if (!string.IsNullOrEmpty(comments) && comments.Length > CommentMaximum)
            {
                return Result.Fail(ValidationMessages.LengthMaxMsg("Comments", CommentMaximum));
            }
            VaccinationRequiredComments = comments;
            return Result.Ok();
        }

        public void UpdatePermissionToSeeRequests(bool value)
        {
            RequiresPermissionToSeeRequests = value;
        }

        public void UpdatePhones(string phone, int? phoneExt, string fax, int? faxExt)
        {
            Phone = phone;
            PhoneExt = phoneExt;
            Fax = fax;
            FaxExt = faxExt;
        }

        public void UpdateWebsite(string website) => Website = website;

        public void ReplaceLogo(Guid logoId) => LogoId = logoId;

        public void UpdatePaidHolidays(bool paidHolidays) => PaidHolidays = paidHolidays;

        public void UpdatePaymentTermDays(int paymentTermDays) => PaymentTermDays = paymentTermDays;

        public static CompanyProfile CompanyRegisterByItself(
            User user,
            Guid agencyId,
            string name,
            string phone,
            int? phoneExt,
            Location location,
            bool isBilling,
            ICollection<string> jobPositions,
            string logoName)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            var companyProfile = new CompanyProfile
            {
                Company = user ?? throw new ArgumentNullException(nameof(user)),
                CompanyId = user.Id,
                AgencyId = agencyId,
                FullName = name,
                Logo = new CovenantFile(string.IsNullOrEmpty(logoName) ? DefaultImageLogo : logoName),
                Industry = new CompanyProfileIndustry("Other."),
                Phone = phone,
                PhoneExt = phoneExt,
                RequiredPaymentMethod = true
            };
            companyProfile.AddLocation(location, isBilling);
            foreach (var jobPosition in jobPositions)
            {
                Result<CompanyProfileJobPositionRate> rRate = CompanyProfileJobPositionRate.CompanyCreate(companyProfile.Id, jobPosition);
                if (rRate) companyProfile.AddJobPositionRate(rRate.Value);
            }
            return companyProfile;
        }

        public static Result<CompanyProfile> AgencyCreateCompany(
            User user,
            Guid agencyId,
            CompanyName fullName,
            string phone,
            int? phoneExt,
            string fax,
            int? faxExt,
            string webSite,
            CompanyProfileIndustry industry,
            CovenantFile logo,
            string about,
            string internalInfo,
            bool requiresPermissionToSeeRequests,
            string createdBy,
            CompanyStatus companyStatus,
            Guid? salesRepresentativeId)
        {
            var profile = new CompanyProfile
            {
                Company = user ?? throw new ArgumentNullException(nameof(user)),
                CompanyId = user.Id,
                AgencyId = agencyId,
                FullName = fullName,
                Phone = phone,
                PhoneExt = phoneExt,
                Fax = fax,
                FaxExt = faxExt,
                Website = webSite,
                Industry = industry ?? throw new ArgumentNullException(nameof(industry)),
                IndustryId = industry.Id,
                About = about,
                InternalInfo = internalInfo,
                Active = true,
                RequiresPermissionToSeeRequests = requiresPermissionToSeeRequests,
                CreatedBy = createdBy,
                CompanyStatus = companyStatus,
                SalesRepresentativeId = salesRepresentativeId
            };
            if (logo == null) return Result.Ok(profile);
            profile.Logo = logo;
            profile.LogoId = logo.Id;
            return Result.Ok(profile);
        }

        public void ChangeFullName(string fullName) => FullName = fullName;

        public void UpdateDetails(string about, string internalInfo, Guid? salesRepresentativeId)
        {
            About = about;
            InternalInfo = internalInfo;
            SalesRepresentativeId = salesRepresentativeId;
        }

        public void UpdateStatus(CompanyStatus companyStatus, string updatedBy, DateTime updatedAt)
        {
            CompanyStatus = companyStatus;
            UpdatedBy = updatedBy;
            UpdatedAt = updatedAt;
        }

        public void ReplaceJobPositionRates(ICollection<CompanyProfileJobPositionRate> jobPositionRates) => JobPositionRates = jobPositionRates;
    }
}
