import http from '@/security/apiService';
import type { PaginatedList } from '@/types/common';
import type {
  AgencyCompanyFilter,
  AgencyCompanyListItem,
  AgencyCompanyContactPerson,
  AgencyCompanyLocationModel,
  AgencyCompanyJobPosition,
  VaccinationRequiredModel,
  InvoiceNotesModel,
  InvoiceRecipientModel,
  PetitionJobPositionPayload,
  UpdateIsAsapRequestsPayload,
} from '@/types/agency';
import type { CovenantFileModel } from '@/types/common';
import type {
  CompanyProfileDetail,
  CompanyProfileDocumentModel,
  CompanyProfileListItem,
  CompanyProfileSettingsUpdate,
  CompanyUserModel,
  CreateCompanyUserModel,
} from '@/types/company';

// ---------------------------------------------------------------------------
// Company CRUD (agency view)
// ---------------------------------------------------------------------------

export function createAgencyCompany(company: Partial<CompanyProfileDetail>): Promise<{ id: string }> {
  return http.post('/api/v2/AgencyCompanyProfile', company).then(r => r.data);
}

export function getAgencyCompanies(filter: AgencyCompanyFilter): Promise<PaginatedList<AgencyCompanyListItem>> {
  return http.get('api/v2/AgencyCompanyProfile', { params: { ...filter } }).then(r => r.data);
}

export function getAgencyCompany(companyId: string): Promise<CompanyProfileDetail> {
  return http.get(`/api/v2/AgencyCompanyProfile/${companyId}`).then(r => r.data);
}

export function updateAgencyCompany(companyId: string, company: Partial<CompanyProfileDetail>): Promise<void> {
  return http.put(`/api/v2/AgencyCompanyProfile/${companyId}`, company).then(() => {});
}

export function updateCompanyVaccinationRequired(companyProfileId: string, model: VaccinationRequiredModel): Promise<void> {
  return http.put(`/api/v2/AgencyCompanyProfile/${companyProfileId}/VaccinationRequired`, model).then(() => {});
}

export function updateAgencyCompanyEmail(companyProfileId: string, model: { newEmail: string }): Promise<void> {
  return http.put(`/api/v2/AgencyCompanyProfile/${companyProfileId}/Email`, model).then(() => {});
}

export function updateAgencyCompanyProfileLogo(profileId: string, model: Partial<CovenantFileModel>): Promise<void> {
  return http.put(`/api/AgencyCompanyProfile/${profileId}/Logo`, model).then(() => {});
}

export function getAgencyCompanyProfileWithRequests(): Promise<CompanyProfileListItem[]> {
  return http.get('/api/v2/AgencyCompanyProfile/company-with-requests').then(r => r.data);
}

export function bulkAgencyCompanies(agencyId: string, file: File): Promise<Blob> {
  const formData = new FormData();
  formData.append('file', file);
  return http
    .post(`/api/v2/AgencyCompanyProfile/bulk/${agencyId}`, formData, {
      responseType: 'blob',
      headers: { 'Content-Type': 'multipart/form-data' },
    })
    .then(r => r.data);
}

// ---------------------------------------------------------------------------
// Company contact persons
// ---------------------------------------------------------------------------

export function getAgencyCompanyContactPerson(profileId: string): Promise<AgencyCompanyContactPerson[]> {
  return http.get(`/api/AgencyCompanyProfile/${profileId}/ContactPerson`).then(r => r.data);
}

export function createAgencyCompanyContactPerson(profileId: string, model: AgencyCompanyContactPerson): Promise<{ id: string }> {
  return http.post(`/api/AgencyCompanyProfile/${profileId}/ContactPerson`, model).then(r => r.data);
}

export function updateAgencyCompanyContactPerson(profileId: string, personId: string, model: AgencyCompanyContactPerson): Promise<void> {
  return http.put(`/api/AgencyCompanyProfile/${profileId}/ContactPerson/${personId}`, model).then(() => {});
}

export function deleteAgencyCompanyContactPerson(profileId: string, personId: string): Promise<void> {
  return http.delete(`/api/AgencyCompanyProfile/${profileId}/ContactPerson/${personId}`).then(() => {});
}

// ---------------------------------------------------------------------------
// Company locations
// ---------------------------------------------------------------------------

export function getAgencyCompanyLocation(profileId: string): Promise<AgencyCompanyLocationModel[]> {
  return http.get(`/api/AgencyCompanyProfile/${profileId}/Location`).then(r => r.data);
}

export function createAgencyCompanyLocation(profileId: string, model: AgencyCompanyLocationModel): Promise<{ id: string }> {
  return http.post(`/api/AgencyCompanyProfile/${profileId}/Location`, model).then(r => r.data);
}

export function updateAgencyCompanyLocation(profileId: string, locationId: string, model: AgencyCompanyLocationModel): Promise<void> {
  return http.put(`/api/AgencyCompanyProfile/${profileId}/Location/${locationId}`, model).then(() => {});
}

export function deleteAgencyCompanyLocation(profileId: string, locationId: string): Promise<void> {
  return http.delete(`/api/AgencyCompanyProfile/${profileId}/Location/${locationId}`).then(() => {});
}

export function updateAgencyCompanyContactInformation(profileId: string, model: Partial<CompanyProfileDetail>): Promise<void> {
  return http.put(`/api/AgencyCompanyProfile/${profileId}/ContactInformation`, model).then(() => {});
}

// ---------------------------------------------------------------------------
// Company job positions
// ---------------------------------------------------------------------------

export function getAgencyCompanyJobPositions(companyProfileId: string): Promise<AgencyCompanyJobPosition[]> {
  return http.get(`/api/AgencyCompanyProfile/${companyProfileId}/JobPosition`).then(r => r.data);
}

export function getAgencyCompanyJobPositionById(profileId: string, id: string): Promise<AgencyCompanyJobPosition> {
  return http.get(`/api/AgencyCompanyProfile/${profileId}/JobPosition/${id}`).then(r => r.data);
}

export function createAgencyCompanyJobPosition(profileId: string, model: AgencyCompanyJobPosition): Promise<{ id: string }> {
  return http.post(`/api/AgencyCompanyProfile/${profileId}/JobPosition`, model).then(r => r.data);
}

export function updateAgencyCompanyJobPosition(profileId: string, id: string, model: AgencyCompanyJobPosition): Promise<void> {
  return http.put(`/api/AgencyCompanyProfile/${profileId}/JobPosition/${id}`, model).then(() => {});
}

export function deleteAgencyCompanyJobPosition(profileId: string, id: string): Promise<void> {
  return http.delete(`/api/AgencyCompanyProfile/${profileId}/JobPosition/${id}`).then(() => {});
}

export function petitionAgencyCompanyJobPosition(profileId: string, model: PetitionJobPositionPayload): Promise<void> {
  return http.post(`/api/AgencyCompanyProfile/${profileId}/JobPosition/Petition`, model).then(() => {});
}

// Legacy delete used by older code path
export function deleteAgencyJobPosition(companyProfileId: string, jobPositionRateId: string): Promise<void> {
  return http.delete(`/api/AgencyJobPosition/${companyProfileId}/${jobPositionRateId}`).then(() => {});
}

// ---------------------------------------------------------------------------
// Job position documents
// ---------------------------------------------------------------------------

export function getAgencyCompanyJobPositionDocuments(
  profileId: string,
  jobPositionId: string,
  pagination: { size: number; page: number },
): Promise<PaginatedList<CompanyProfileDocumentModel>> {
  return http
    .get(
      `/api/AgencyCompanyProfile/${profileId}/JobPosition/${jobPositionId}/Document?PageSize=${pagination.size}&PageIndex=${pagination.page}`,
    )
    .then(r => r.data);
}

export function createAgencyCompanyJobPositionDocuments(profileId: string, jobPositionId: string, model: CompanyProfileDocumentModel): Promise<{ id: string }> {
  return http.post(`/api/AgencyCompanyProfile/${profileId}/JobPosition/${jobPositionId}/Document`, model).then(r => r.data);
}

export function deleteAgencyCompanyJobPositionDocuments(profileId: string, jobPositionId: string, id: string): Promise<void> {
  return http.delete(`/api/AgencyCompanyProfile/${profileId}/JobPosition/${jobPositionId}/Document/${id}`).then(() => {});
}

// ---------------------------------------------------------------------------
// Company documents
// ---------------------------------------------------------------------------

export function getAgencyCompanyDocument(
  profileId: string,
  pagination: { size: number; page: number },
): Promise<PaginatedList<CompanyProfileDocumentModel>> {
  return http
    .get(`/api/AgencyCompanyProfile/${profileId}/Document?PageSize=${pagination.size}&PageIndex=${pagination.page}`)
    .then(r => r.data);
}

export function createAgencyCompanyDocument(profileId: string, model: CompanyProfileDocumentModel): Promise<{ id: string; pathFile: string }> {
  return http.post(`/api/AgencyCompanyProfile/${profileId}/Document`, model).then(r => r.data);
}

export function deleteAgencyCompanyDocument(profileId: string, id: string): Promise<void> {
  return http.delete(`/api/AgencyCompanyProfile/${profileId}/Document/${id}`).then(() => {});
}

// ---------------------------------------------------------------------------
// Invoice notes & recipients
// ---------------------------------------------------------------------------

export function getInvoiceNotes(id: string): Promise<InvoiceNotesModel> {
  return http.get(`/api/CompanyProfile/${id}/InvoiceNotes`).then(r => r.data);
}

export function postInvoiceNotes(id: string, model: InvoiceNotesModel): Promise<void> {
  return http.put(`/api/CompanyProfile/${id}/InvoiceNotes`, model).then(() => {});
}

export function getCompanyInvoiceRecipients(companyProfileId: string): Promise<InvoiceRecipientModel[]> {
  return http.get(`/api/CompanyProfile/${companyProfileId}/InvoiceRecipient`).then(r => r.data);
}

export function postCompanyInvoiceRecipient(companyProfileId: string, model: InvoiceRecipientModel): Promise<{ id: string }> {
  return http.post(`/api/CompanyProfile/${companyProfileId}/InvoiceRecipient`, model).then(r => r.data);
}

export function deleteCompanyInvoiceRecipient(companyProfileId: string, id: string): Promise<void> {
  return http.delete(`/api/CompanyProfile/${companyProfileId}/InvoiceRecipient/${id}`).then(() => {});
}

export function updateCompanyInvoiceRecipient(companyProfileId: string, id: string, model: InvoiceRecipientModel): Promise<void> {
  return http.put(`/api/CompanyProfile/${companyProfileId}/InvoiceRecipient/${id}`, model).then(() => {});
}

// ---------------------------------------------------------------------------
// Company settings
// ---------------------------------------------------------------------------

export function updatePermissionToSeeRequests(companyId: string, settings: CompanyProfileSettingsUpdate): Promise<void> {
  return http
    .patch(`/api/V2/AgencyCompanyProfile/${companyId}/RequiresPermissionToSeeRequests`, settings)
    .then(() => {});
}

export function updatePaidHolidays(companyId: string, settings: CompanyProfileSettingsUpdate): Promise<void> {
  return http.patch(`/api/V2/AgencyCompanyProfile/${companyId}/PaidHolidays`, settings).then(() => {});
}

export function updateOvertime(companyId: string, settings: CompanyProfileSettingsUpdate): Promise<void> {
  return http.patch(`/api/V2/AgencyCompanyProfile/${companyId}/Overtime`, settings).then(() => {});
}

// ---------------------------------------------------------------------------
// Company users (agency-managed users of a company profile)
// ---------------------------------------------------------------------------

export function getCompanyUsers(id: string): Promise<CompanyUserModel[]> {
  return http.get(`/api/V2/AgencyCompanyProfile/${id}/CompanyUsers`).then(r => r.data);
}

export function getCompanyProfileUsers(profileId: string): Promise<CompanyUserModel[]> {
  return http.get(`/api/agency-company-profile-user/${profileId}`).then(r => r.data);
}

export function createCompanyProfileUser(companyId: string, user: CreateCompanyUserModel): Promise<{ id: string }> {
  return http.post(`/api/agency-company-profile-user/${companyId}`, user).then(r => r.data);
}

export function deleteCompanyProfileUser(companyId: string, userId: string): Promise<void> {
  return http.delete(`/api/agency-company-profile-user/${companyId}/users/${userId}`).then(() => {});
}

export function updateIsAsapRequests(model: UpdateIsAsapRequestsPayload): Promise<void> {
  return http.put('/api/AgencyRequest/is-asap', model).then(() => {});
}
