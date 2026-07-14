import { api } from '@/security/apiService';
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
  return api.post<{ id: string }>('/api/v2/AgencyCompanyProfile', company);
}

export function getAgencyCompanies(filter: AgencyCompanyFilter): Promise<PaginatedList<AgencyCompanyListItem>> {
  return api.get<PaginatedList<AgencyCompanyListItem>>('api/v2/AgencyCompanyProfile', { params: { ...filter } });
}

export function getAgencyCompany(companyId: string): Promise<CompanyProfileDetail> {
  return api.get<CompanyProfileDetail>(`/api/v2/AgencyCompanyProfile/${companyId}`);
}

export function updateAgencyCompany(companyId: string, company: Partial<CompanyProfileDetail>): Promise<void> {
  return api.put(`/api/v2/AgencyCompanyProfile/${companyId}`, company);
}

export function updateCompanyVaccinationRequired(companyProfileId: string, model: VaccinationRequiredModel): Promise<void> {
  return api.put(`/api/v2/AgencyCompanyProfile/${companyProfileId}/VaccinationRequired`, model);
}

export function updateAgencyCompanyEmail(companyProfileId: string, model: { newEmail: string }): Promise<void> {
  return api.put(`/api/v2/AgencyCompanyProfile/${companyProfileId}/Email`, model);
}

export function updateAgencyCompanyProfileLogo(profileId: string, model: Partial<CovenantFileModel>): Promise<void> {
  return api.put(`/api/AgencyCompanyProfile/${profileId}/Logo`, model);
}

export function getAgencyCompanyProfileWithRequests(): Promise<CompanyProfileListItem[]> {
  return api.get<CompanyProfileListItem[]>('/api/v2/AgencyCompanyProfile/company-with-requests');
}

export function bulkAgencyCompanies(agencyId: string, file: File): Promise<Blob> {
  const formData = new FormData();
  formData.append('file', file);
  return api
    .post<Blob>(`/api/v2/AgencyCompanyProfile/bulk/${agencyId}`, formData, {
      responseType: 'blob',
      headers: { 'Content-Type': 'multipart/form-data' },
    });
}

// ---------------------------------------------------------------------------
// Company contact persons
// ---------------------------------------------------------------------------

export function getAgencyCompanyContactPerson(profileId: string): Promise<AgencyCompanyContactPerson[]> {
  return api.get<AgencyCompanyContactPerson[]>(`/api/AgencyCompanyProfile/${profileId}/ContactPerson`);
}

export function createAgencyCompanyContactPerson(profileId: string, model: AgencyCompanyContactPerson): Promise<{ id: string }> {
  return api.post<{ id: string }>(`/api/AgencyCompanyProfile/${profileId}/ContactPerson`, model);
}

export function updateAgencyCompanyContactPerson(profileId: string, personId: string, model: AgencyCompanyContactPerson): Promise<void> {
  return api.put(`/api/AgencyCompanyProfile/${profileId}/ContactPerson/${personId}`, model);
}

export function deleteAgencyCompanyContactPerson(profileId: string, personId: string): Promise<void> {
  return api.del(`/api/AgencyCompanyProfile/${profileId}/ContactPerson/${personId}`);
}

// ---------------------------------------------------------------------------
// Company locations
// ---------------------------------------------------------------------------

export function getAgencyCompanyLocation(profileId: string): Promise<AgencyCompanyLocationModel[]> {
  return api.get<AgencyCompanyLocationModel[]>(`/api/AgencyCompanyProfile/${profileId}/Location`);
}

export function createAgencyCompanyLocation(profileId: string, model: AgencyCompanyLocationModel): Promise<{ id: string }> {
  return api.post<{ id: string }>(`/api/AgencyCompanyProfile/${profileId}/Location`, model);
}

export function updateAgencyCompanyLocation(profileId: string, locationId: string, model: AgencyCompanyLocationModel): Promise<void> {
  return api.put(`/api/AgencyCompanyProfile/${profileId}/Location/${locationId}`, model);
}

export function deleteAgencyCompanyLocation(profileId: string, locationId: string): Promise<void> {
  return api.del(`/api/AgencyCompanyProfile/${profileId}/Location/${locationId}`);
}

export function updateAgencyCompanyContactInformation(profileId: string, model: Partial<CompanyProfileDetail>): Promise<void> {
  return api.put(`/api/AgencyCompanyProfile/${profileId}/ContactInformation`, model);
}

// ---------------------------------------------------------------------------
// Company job positions
// ---------------------------------------------------------------------------

export function getAgencyCompanyJobPositions(companyProfileId: string): Promise<AgencyCompanyJobPosition[]> {
  return api.get<AgencyCompanyJobPosition[]>(`/api/AgencyCompanyProfile/${companyProfileId}/JobPosition`);
}

export function getAgencyCompanyJobPositionById(profileId: string, id: string): Promise<AgencyCompanyJobPosition> {
  return api.get<AgencyCompanyJobPosition>(`/api/AgencyCompanyProfile/${profileId}/JobPosition/${id}`);
}

export function createAgencyCompanyJobPosition(profileId: string, model: AgencyCompanyJobPosition): Promise<{ id: string }> {
  return api.post<{ id: string }>(`/api/AgencyCompanyProfile/${profileId}/JobPosition`, model);
}

export function updateAgencyCompanyJobPosition(profileId: string, id: string, model: AgencyCompanyJobPosition): Promise<void> {
  return api.put(`/api/AgencyCompanyProfile/${profileId}/JobPosition/${id}`, model);
}

export function deleteAgencyCompanyJobPosition(profileId: string, id: string): Promise<void> {
  return api.del(`/api/AgencyCompanyProfile/${profileId}/JobPosition/${id}`);
}

export function petitionAgencyCompanyJobPosition(profileId: string, model: PetitionJobPositionPayload): Promise<void> {
  return api.post(`/api/AgencyCompanyProfile/${profileId}/JobPosition/Petition`, model);
}

// Legacy delete used by older code path
export function deleteAgencyJobPosition(companyProfileId: string, jobPositionRateId: string): Promise<void> {
  return api.del(`/api/AgencyJobPosition/${companyProfileId}/${jobPositionRateId}`);
}

// ---------------------------------------------------------------------------
// Job position documents
// ---------------------------------------------------------------------------

export function getAgencyCompanyJobPositionDocuments(
  profileId: string,
  jobPositionId: string,
  pagination: { size: number; page: number },
): Promise<PaginatedList<CompanyProfileDocumentModel>> {
  return api
    .get<PaginatedList<CompanyProfileDocumentModel>>(
      `/api/AgencyCompanyProfile/${profileId}/JobPosition/${jobPositionId}/Document?PageSize=${pagination.size}&PageIndex=${pagination.page}`,
    );
}

export function createAgencyCompanyJobPositionDocuments(profileId: string, jobPositionId: string, model: CompanyProfileDocumentModel): Promise<{ id: string }> {
  return api.post<{ id: string }>(`/api/AgencyCompanyProfile/${profileId}/JobPosition/${jobPositionId}/Document`, model);
}

export function deleteAgencyCompanyJobPositionDocuments(profileId: string, jobPositionId: string, id: string): Promise<void> {
  return api.del(`/api/AgencyCompanyProfile/${profileId}/JobPosition/${jobPositionId}/Document/${id}`);
}

// ---------------------------------------------------------------------------
// Company documents
// ---------------------------------------------------------------------------

export function getAgencyCompanyDocument(
  profileId: string,
  pagination: { size: number; page: number },
): Promise<PaginatedList<CompanyProfileDocumentModel>> {
  return api
    .get<PaginatedList<CompanyProfileDocumentModel>>(`/api/AgencyCompanyProfile/${profileId}/Document?PageSize=${pagination.size}&PageIndex=${pagination.page}`);
}

export function createAgencyCompanyDocument(profileId: string, model: CompanyProfileDocumentModel): Promise<{ id: string; pathFile: string }> {
  return api.post<{ id: string; pathFile: string }>(`/api/AgencyCompanyProfile/${profileId}/Document`, model);
}

export function deleteAgencyCompanyDocument(profileId: string, id: string): Promise<void> {
  return api.del(`/api/AgencyCompanyProfile/${profileId}/Document/${id}`);
}

// ---------------------------------------------------------------------------
// Invoice notes & recipients
// ---------------------------------------------------------------------------

export function getInvoiceNotes(id: string): Promise<InvoiceNotesModel> {
  return api.get<InvoiceNotesModel>(`/api/CompanyProfile/${id}/InvoiceNotes`);
}

export function postInvoiceNotes(id: string, model: InvoiceNotesModel): Promise<void> {
  return api.put(`/api/CompanyProfile/${id}/InvoiceNotes`, model);
}

export function getCompanyInvoiceRecipients(companyProfileId: string): Promise<InvoiceRecipientModel[]> {
  return api.get<InvoiceRecipientModel[]>(`/api/CompanyProfile/${companyProfileId}/InvoiceRecipient`);
}

export function postCompanyInvoiceRecipient(companyProfileId: string, model: InvoiceRecipientModel): Promise<{ id: string }> {
  return api.post<{ id: string }>(`/api/CompanyProfile/${companyProfileId}/InvoiceRecipient`, model);
}

export function deleteCompanyInvoiceRecipient(companyProfileId: string, id: string): Promise<void> {
  return api.del(`/api/CompanyProfile/${companyProfileId}/InvoiceRecipient/${id}`);
}

export function updateCompanyInvoiceRecipient(companyProfileId: string, id: string, model: InvoiceRecipientModel): Promise<void> {
  return api.put(`/api/CompanyProfile/${companyProfileId}/InvoiceRecipient/${id}`, model);
}

// ---------------------------------------------------------------------------
// Company settings
// ---------------------------------------------------------------------------

export function updatePermissionToSeeRequests(companyId: string, settings: CompanyProfileSettingsUpdate): Promise<void> {
  return api
    .patch(`/api/V2/AgencyCompanyProfile/${companyId}/RequiresPermissionToSeeRequests`, settings);
}

export function updatePaidHolidays(companyId: string, settings: CompanyProfileSettingsUpdate): Promise<void> {
  return api.patch(`/api/V2/AgencyCompanyProfile/${companyId}/PaidHolidays`, settings);
}

export function updateOvertime(companyId: string, settings: CompanyProfileSettingsUpdate): Promise<void> {
  return api.patch(`/api/V2/AgencyCompanyProfile/${companyId}/Overtime`, settings);
}

// ---------------------------------------------------------------------------
// Company users (agency-managed users of a company profile)
// ---------------------------------------------------------------------------

export function getCompanyUsers(id: string): Promise<CompanyUserModel[]> {
  return api.get<CompanyUserModel[]>(`/api/V2/AgencyCompanyProfile/${id}/CompanyUsers`);
}

export function getCompanyProfileUsers(profileId: string): Promise<CompanyUserModel[]> {
  return api.get<CompanyUserModel[]>(`/api/agency-company-profile-user/${profileId}`);
}

export function createCompanyProfileUser(companyId: string, user: CreateCompanyUserModel): Promise<{ id: string }> {
  return api.post<{ id: string }>(`/api/agency-company-profile-user/${companyId}`, user);
}

export function deleteCompanyProfileUser(companyId: string, userId: string): Promise<void> {
  return api.del(`/api/agency-company-profile-user/${companyId}/users/${userId}`);
}

export function updateIsAsapRequests(model: UpdateIsAsapRequestsPayload): Promise<void> {
  return api.put('/api/AgencyRequest/is-asap', model);
}
