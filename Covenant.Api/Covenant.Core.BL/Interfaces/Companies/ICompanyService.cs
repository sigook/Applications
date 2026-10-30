using Covenant.Common.Functionals;
using Covenant.Common.Models.Accounting.Invoices.Companies;
using Covenant.Common.Models.Companies.Agencies;
using Covenant.Common.Models.Website;
using Covenant.Common.Models;
using Covenant.Common.Models.Accounting.Invoices;
using Covenant.Common.Models.Companies;

namespace Covenant.Core.BL.Interfaces.Companies;

public interface ICompanyService
{
    Task<Result<Guid>> CreateCompanyProfile(CompanyRegisterByItselfModel model);
    Task<Result> UpdateProfile(Guid profileId, CompanyProfileDetailModel model);
    Task<Result> UpdateLogo(Guid profileId);
    Task<Result<Guid>> CreateCompanyLocation(CompanyProfileLocationDetailModel model, Guid? profileId = null);
    Task<Result> UpdateCompanyLocation(Guid id, CompanyProfileLocationDetailModel model);
    Task<Result> DeleteCompanyLocation(Guid id);
    Task<Result> DeleteContactPerson(Guid id);
    Task<Result> UpdateCompanyUser(Guid id, CompanyUserModel model);
    Task<Result> UpdateContactInformation(Guid profileId, CompanyProfileDetailModel model);
    Task<Result> UpdateVaccinationInfo(Guid profileId, VaccinationRequiredModel model);
    Task<Result> UpdateRequiresPermissionToSeeRequests(Guid profileId, bool requiresPermission);
    Task<Result> UpdatePaidHolidays(Guid profileId, bool paidHolidays);
    Task<Result> UpdateOvertime(Guid profileId, double overtimeStartsAfterHours);
    Task<Result> UpdatePaymentTerms(Guid profileId, CompanyProfilePaymentTermsUpdateModel model);
    Task<Guid> CreateInvoiceRecipient(Guid profileId, CompanyProfileInvoiceRecipientModel model);
    Task UpdateInvoiceRecipient(Guid id, CompanyProfileInvoiceRecipientModel model);
    Task<Result> DeleteInvoiceRecipient(Guid id);
    Task UpdateInvoiceNotes(Guid profileId, string htmlNotes);
    Task<Result<NoteModel>> CreateNote(Guid profileId, NoteModel model);
    Task<Result> UpdateNote(Guid profileId, Guid noteId, NoteModel model);
    Task<Result> DeleteNote(Guid profileId, Guid noteId);
    Task<Result<Guid>> CreateJobPositionWithoutRates(Guid profileId, CreateJobPositionModel model);
    Task<Result<Guid>> CompanyCreateJobPosition(CreateJobPositionModel model);
    Task<Result> RequestNewJobPosition(ContactDto contact);
    Task<Result> RequestNewWorker(Guid requestId, CommentsModel model);
    Task<InvoiceSummaryModel> GetCompanyInvoice(Guid invoiceId);
    Task<Result> CreateCompanyUser(CompanyUserModel model, Guid? companyProfileId = null);
    Task<Result> CreateContact(CompanyProfileContactPersonModel model);
    Task<Result> DeleteCompanyUser(Guid userId, Guid? companyProfileId = null);
    Task<Result<ResultGenerateDocument<byte[]>>> BulkCompany(Guid agencyId, Stream file);
    Task<CompanyDeletionCheckModel> CheckCompanyDeletion(Guid companyProfileId);
    Task<Result> DeleteCompanyProfile(Guid companyProfileId);
}