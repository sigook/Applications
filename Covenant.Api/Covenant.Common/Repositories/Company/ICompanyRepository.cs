using Covenant.Common.Entities.Company;
using Covenant.Common.Models;
using Covenant.Common.Models.Candidate;
using Covenant.Common.Models.Company;
using Covenant.Common.Models.Location;
using System.Linq.Expressions;

namespace Covenant.Common.Repositories.Company
{
    public interface ICompanyRepository
    {
        Task Create<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        void Update<T>(T entity) where T : class;
        Task<CompanyProfileJobPositionRate> GetJobPosition(Guid id);
        Task<IEnumerable<CompanyProfileJobPositionRateModel>> GetJobPositions(Expression<Func<CompanyProfileJobPositionRate, bool>> expression);
        Task<CompanyProfileJobPositionRateModel> GetJobPositionDetail(Guid id);
        Task<Guid> GetCompanyId(Guid companyProfileId);
        Task<List<CompanyProfileAgencyListModel>> GetCompanyProfiles(Guid companyId);
        Task<CompanyProfile> GetCompanyProfile(Expression<Func<CompanyProfile, bool>> expression);
        Task<CompanyProfileDetailModel> GetCompanyProfileDetail(Expression<Func<CompanyProfile, bool>> expression);
        Task<PaginatedList<CompanyProfileListModel>> GetCompaniesProfileForAgency(Guid agencyId, GetCompanyForAgencyFilter filter);
        IEnumerable<CompanyProfileListModel> GetAllCompaniesProfileForAgency(Guid agencyId, GetCompanyForAgencyFilter filter);
        Task<string> GetCompanyProfileInvoiceNotes(Guid companyProfileId);
        Task UpdateCompanyProfileInvoiceNotes(Guid companyProfileId, string htmlNotes);
        Task<List<CompanyProfileInvoiceRecipientModel>> GetInvoiceRecipients(Guid companyProfileId);
        Task<CompanyProfileInvoiceRecipient> GetInvoiceRecipient(Guid id);
        Task UpdateInvoiceRecipient(Guid id, CompanyProfileInvoiceRecipientModel model);
        Task<Guid> CreateInvoiceRecipient(Guid companyProfileId, CompanyProfileInvoiceRecipientModel model);
        Task<IEnumerable<CompanyProfileContactPersonModel>> GetContactPersons(Expression<Func<CompanyProfileContactPerson, bool>> condition);
        Task<CompanyProfileContactPersonModel> GetContactPersonDetail(Guid profileId, Guid id);
        Task<CompanyProfileContactPerson> GetContactPerson(Guid id);
        Task<CompanyProfileLocationDetailModel> GetLocationDetail(Guid id);
        Task<CompanyProfileLocation> GetLocation(Guid id);
        Task<IEnumerable<LocationDetailModel>> GetCompanyLocations(Expression<Func<CompanyProfileLocation, bool>> condition);
        Task<PaginatedList<CompanyProfileDocumentModel>> GetDocuments(Guid profileId, Pagination pagination);
        Task<CompanyProfileDocument> GetDocument(Guid id);
        Task<PaginatedList<NoteModel>> GetNotes(Guid profileId, Pagination pagination);
        Task<NoteModel> GetNoteDetail(Guid profileId, Guid id);
        Task<CompanyProfileNote> GetNote(Guid profileId, Guid id);
        Task SaveChangesAsync();
        Task<IEnumerable<CompanyUserModel>> GetAllCompanyUsers(Guid companyId);
        Task<Guid> GetCompanyIdForUser(Guid userId);
        Task<CompanyUserModel> GetCompanyUserDetail(Guid id);
        Task<CompanyUser> GetCompanyUser(Guid id);
        Task<IEnumerable<ProvinceModel>> GetCompanyProvincesWithTaxes(Guid id);
        Task BulkCompanies(IEnumerable<BulkCompany> bulk);
    }
}