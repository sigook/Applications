using Covenant.Common.Constants;
using Covenant.Common.Entities.Agency;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Models;
using Covenant.Common.Resources;

namespace Covenant.Common.Entities.Company
{
    public class CompanyProfile
    {
        public const string DefaultImageLogo = "company.png";

        public static readonly TimeSpan DefaultOvertimeStarts = TimeSpan.FromHours(44);
        public static readonly TimeSpan MinimumOvertimeStarts = TimeSpan.FromHours(40);

        public CompanyProfile()
        {
        }

        public CompanyProfile(User company, Agency.Agency agency, string fullName, string businessName, string phone, CompanyProfileIndustry industry)
        {
            Company = company ?? throw new ArgumentNullException(nameof(company));
            CompanyId = company.Id;
            Agency = agency;
            FullName = fullName;
            BusinessName = businessName;
            Phone = phone;
            Industry = industry;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public User Company { get; set; }
        public Guid CompanyId { get; set; }
        public Agency.Agency Agency { get; set; }
        public Guid AgencyId { get; set; }
        public int NumberId { get; private set; }
        public Guid? LogoId { get; set; }
        public CovenantFile Logo { get; set; } = new CovenantFile(DefaultImageLogo);
        public string FullName { get; set; }
        public string BusinessName { get; set; }
        public string Phone { get; set; }
        public int? PhoneExt { get; set; }
        public string Fax { get; set; }
        public int? FaxExt { get; set; }
        public string Website { get; set; }
        public string About { get; set; }
        public string InternalInfo { get; set; }
        public string ContactRole { get; set; }
        public string ContactName { get; set; }
        public CompanyStatus CompanyStatus { get; set; }
        public bool RequiresPermissionToSeeOrders { get; set; }
        public ICollection<CompanyProfileLocation> Locations { get; set; } = new List<CompanyProfileLocation>();
        public ICollection<CompanyProfileContactPerson> ContactPersons { get; set; } = new List<CompanyProfileContactPerson>();
        public ICollection<CompanyProfileJobPositionRate> JobPositionRates { get; set; } = new List<CompanyProfileJobPositionRate>();
        public ICollection<CompanyProfileDocument> Documents { get; set; } = new List<CompanyProfileDocument>();
        public ICollection<CompanyProfileNote> Notes { get; set; } = new List<CompanyProfileNote>();
        public CompanyProfileIndustry Industry { get; set; }
        public Guid? SalesRepresentativeId { get; set; }
        public AgencyPersonnel SalesRepresentative { get; set; }
        public Guid IndustryId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool Active { get; set; }
        public bool PaidHolidays { get; set; }
        public bool RequiredPaymentMethod { get; set; }
        public bool? VaccinationRequired { get; set; }
        public string VaccinationRequiredComments { get; set; }
        public TimeSpan OvertimeStartsAfter { get; set; } = DefaultOvertimeStarts;
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
            BusinessName = name;
            FullName = name;
            return Result.Ok();
        }

        public Result UpdateVaccinationInfo(bool? required, string comments)
        {
            VaccinationRequired = required;
            if (!string.IsNullOrEmpty(comments) && comments.Length > CovenantConstants.Validation.CommentMaximum)
            {
                return Result.Fail(ValidationMessages.LengthMaxMsg("Comments", CovenantConstants.Validation.CommentMaximum));
            }
            VaccinationRequiredComments = comments;
            return Result.Ok();
        }

        public void UpdatePermissionToSeeOrders(bool value)
        {
            RequiresPermissionToSeeOrders = value;
        }

        public static CompanyProfile CompanyRegisterByItself(
            User user,
            Guid agencyId,
            string name,
            string phone,
            int? phoneExt,
            Location location,
            bool isBilling,
            ICollection<(Guid? jobPositionId, string otherJobPosition)> jonPositionRates,
            string logoName)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            var companyProfile = new CompanyProfile
            {
                Company = user ?? throw new ArgumentNullException(nameof(user)),
                CompanyId = user.Id,
                AgencyId = agencyId,
                BusinessName = name,
                FullName = name,
                Logo = new CovenantFile(logoName),
                Industry = new CompanyProfileIndustry("Other."),
                Phone = phone,
                PhoneExt = phoneExt,
                RequiredPaymentMethod = true
            };
            companyProfile.AddLocation(location, isBilling);
            foreach ((Guid? jobPositionId, string otherJobPosition) in jonPositionRates)
            {
                Result<CompanyProfileJobPositionRate> rRate = CompanyProfileJobPositionRate.CompanyCreate(companyProfile.Id, jobPositionId, otherJobPosition);
                if (rRate) companyProfile.AddJobPositionRate(rRate.Value);
            }
            return companyProfile;
        }

        public static Result<CompanyProfile> AgencyCreateCompany(
            User user,
            Guid agencyId,
            CompanyName fullName,
            CompanyName businessName,
            string phone,
            int? phoneExt,
            string fax,
            int? faxExt,
            string webSite,
            CompanyProfileIndustry industry,
            CovenantFile logo,
            string about,
            string internalInfo,
            bool requiresPermissionToSeeOrders,
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
                BusinessName = businessName,
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
                RequiresPermissionToSeeOrders = requiresPermissionToSeeOrders,
                CreatedBy = createdBy,
                CompanyStatus = companyStatus,
                SalesRepresentativeId = salesRepresentativeId
            };
            if (logo == null) return Result.Ok(profile);
            profile.Logo = logo;
            profile.LogoId = logo.Id;
            return Result.Ok(profile);
        }
    }
}