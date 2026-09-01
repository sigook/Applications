using Covenant.Common.Entities.Company;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using Covenant.Common.Models.Location;
using System.Linq.Expressions;

namespace Covenant.Common.Repositories.Company;

public interface ICompanyRepository
{
    Task Create<T>(T entity) where T : class;
    void Delete<T>(T entity) where T : class;
    void Update<T>(T entity) where T : class;
    Task<CompanyProfileJobPositionRate> GetJobPosition(Guid id);
    Task<IEnumerable<CompanyProfileJobPositionRateModel>> GetJobPositions(Expression<Func<CompanyProfileJobPositionRate, bool>> expression);
    Task<IEnumerable<CompanyProfileJobPositionRateModel>> GetJobPositions(Guid companyProfileId, GetJobPositionsFilter filter);
    Task<CompanyProfileJobPositionRateModel> GetJobPositionDetail(Guid id);
    Task<CompanyProfileIdsModel> GetCompanyProfileId(Expression<Func<CompanyProfile, bool>> condition);
    Task<CompanyProfile> GetCompanyProfile(Expression<Func<CompanyProfile, bool>> expression);
    Task<CompanyProfileDetailModel> GetCompanyProfileDetail(Expression<Func<CompanyProfile, bool>> expression);
    Task<PaginatedList<CompanyProfileListModel>> GetCompaniesProfileForAgency(Guid agencyId, GetCompanyForAgencyFilter filter);
    IQueryable<CompanyProfileListModel> GetAllCompaniesProfileForAgency(Guid agencyId, GetCompanyForAgencyFilter filter);
    Task<List<CompanyProfileWithDetailsModel>> GetCompaniesWithDetailsForAgency(Guid agencyId, GetCompanyForAgencyFilter filter);
    Task<string> GetCompanyProfileInvoiceNotes(Guid companyProfileId);
    Task UpdateCompanyProfileInvoiceNotes(Guid companyProfileId, string htmlNotes);
    Task<List<CompanyProfileInvoiceRecipientModel>> GetInvoiceRecipients(Guid companyProfileId);
    Task<CompanyProfileInvoiceRecipient> GetInvoiceRecipient(Guid id);
    Task UpdateInvoiceRecipient(Guid id, CompanyProfileInvoiceRecipientModel model);
    Task<Guid> CreateInvoiceRecipient(Guid companyProfileId, CompanyProfileInvoiceRecipientModel model);
    Task<IEnumerable<CompanyProfileContactPersonModel>> GetContactPeople(Expression<Func<CompanyProfileContactPerson, bool>> condition);
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
    Task<IEnumerable<CompanyUserModel>> GetAllCompanyUsers(Guid companyProfileId);
    Task<Guid> GetCompanyIdForUser(Guid userId);
    Task<CompanyUserModel> GetCompanyUserDetail(Guid id);
    Task<CompanyUser> GetCompanyUser(Guid id);
    Task BulkCompanies(IEnumerable<BulkCompany> bulk);
    Task<PaginatedList<DealListModel>> GetDeals(Guid agencyId, GetDealsFilter filter);
    Task<Deal> GetDeal(Expression<Func<Deal, bool>> expression);
    Task<PaginatedList<CompanyInteractionListModel>> GetInteractions(Guid agencyId, GetCompanyInteractionsFilter filter);
    Task<CompanyInteraction> GetInteraction(Expression<Func<CompanyInteraction, bool>> expression);
    Task<List<BaseModel<Guid>>> GetCompaniesList(Guid agencyId, string searchTerm);
}