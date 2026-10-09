import { api } from '@/app/security/apiService';
import { buildMultipartFormData } from '@/shared/utils/multipart';
import { generateFileName } from '@/shared/utils/fileNaming';
import type { CatalogItem, PaginatedList } from '@/shared/types/common';
import type { AgencyCompanyFilter, AgencyCompanyListItem, AgencyCompanyContactPerson, AgencyCompanyLocationModel, AgencyCompanyJobPosition, AgencyCompanyJobPositionFilter, VaccinationRequiredModel, InvoiceNotesModel, InvoiceRecipientModel, PetitionJobPositionPayload, UpdateIsAsapRequestsPayload, CompanyDeletionCheck } from '@/modules/agency/recruiting/clients/types';
import type { CovenantFileModel } from '@/shared/types/common';
import type { AgencyCompanyProfileDetail, CompanyProfileDocumentModel, CompanyProfileListItem, CompanyProfileSettingsUpdate } from '@/modules/agency/recruiting/clients/types';
import type { CompanyProfileDetail, CompanyUserModel, CreateCompanyUserModel } from '@/shared/company-profile/types';

const companyProfilesUrl = '/api/agency/recruiting/clients';

const recruitingCompanyProfilesUrl = '/api/agency/recruiting/clients';

export function createAgencyCompany(company: Partial<CompanyProfileDetail>): Promise<{ id: string }> {
  return api.post<{ id: string }>(companyProfilesUrl, company);
}

export function getAgencyCompanies(filter: AgencyCompanyFilter): Promise<PaginatedList<AgencyCompanyListItem>> {
  return api.get<PaginatedList<AgencyCompanyListItem>>(recruitingCompanyProfilesUrl, { params: { ...filter } });
}

export function getAgencyCompany(companyId: string): Promise<AgencyCompanyProfileDetail> {
  return api.get<AgencyCompanyProfileDetail>(`${companyProfilesUrl}/${companyId}`);
}

export function updateAgencyCompany(companyId: string, company: Partial<CompanyProfileDetail>): Promise<void> {
  return api.put(`${companyProfilesUrl}/${companyId}`, company);
}

export function updateCompanyVaccinationRequired(companyProfileId: string, model: VaccinationRequiredModel): Promise<void> {
  return api.put(`${companyProfilesUrl}/${companyProfileId}/VaccinationRequired`, model);
}

export function updateAgencyCompanyEmail(companyProfileId: string, model: { newEmail: string }): Promise<void> {
  return api.put(`${companyProfilesUrl}/${companyProfileId}/Email`, model);
}

export function updateAgencyCompanyProfileLogo(profileId: string, logo: File): Promise<void> {
  const fileName = generateFileName('Logo', logo.name);
  const formData = new FormData();
  formData.append(fileName, logo, fileName);
  return api.put(`${companyProfilesUrl}/${profileId}/Logo`, formData, { headers: { 'Content-Type': 'multipart/form-data' } });
}

export function getAgencyCompanyProfileWithRequests(): Promise<CompanyProfileListItem[]> {
  return api.get<CompanyProfileListItem[]>(`${companyProfilesUrl}/company-with-requests`);
}

export function getAgencyCompaniesList(searchTerm?: string): Promise<CatalogItem[]> {
  return api.get<CatalogItem[]>(`${companyProfilesUrl}/companies-list`, { params: { searchTerm } });
}

export function getCompanyDeletionCheck(companyProfileId: string): Promise<CompanyDeletionCheck> {
  return api.get<CompanyDeletionCheck>(`${companyProfilesUrl}/${companyProfileId}/deletion-check`);
}

export function deleteAgencyCompany(companyProfileId: string): Promise<void> {
  return api.del(`${companyProfilesUrl}/${companyProfileId}`);
}

export function bulkAgencyCompanies(agencyId: string, file: File): Promise<Blob> {
  const formData = new FormData();
  formData.append('file', file);
  return api
    .post<Blob>(`${companyProfilesUrl}/bulk/${agencyId}`, formData, {
      responseType: 'blob',
      headers: { 'Content-Type': 'multipart/form-data' },
    });
}

export function getAgencyCompanyContactPerson(profileId: string): Promise<AgencyCompanyContactPerson[]> {
  return api.get<AgencyCompanyContactPerson[]>(`${companyProfilesUrl}/${profileId}/ContactPeople`);
}

export function createAgencyCompanyContactPerson(profileId: string, model: AgencyCompanyContactPerson): Promise<{ id: string }> {
  return api.post<{ id: string }>(`${companyProfilesUrl}/${profileId}/ContactPeople`, model);
}

export function updateAgencyCompanyContactPerson(profileId: string, personId: string, model: AgencyCompanyContactPerson): Promise<void> {
  return api.put(`${companyProfilesUrl}/${profileId}/ContactPeople/${personId}`, model);
}

export function deleteAgencyCompanyContactPerson(profileId: string, personId: string): Promise<void> {
  return api.del(`${companyProfilesUrl}/${profileId}/ContactPeople/${personId}`);
}

export function getAgencyCompanyLocation(profileId: string): Promise<AgencyCompanyLocationModel[]> {
  return api.get<AgencyCompanyLocationModel[]>(`${companyProfilesUrl}/${profileId}/Locations`);
}

export function createAgencyCompanyLocation(profileId: string, model: AgencyCompanyLocationModel): Promise<{ id: string }> {
  return api.post<{ id: string }>(`${companyProfilesUrl}/${profileId}/Locations`, model);
}

export function updateAgencyCompanyLocation(profileId: string, locationId: string, model: AgencyCompanyLocationModel): Promise<void> {
  return api.put(`${companyProfilesUrl}/${profileId}/Locations/${locationId}`, model);
}

export function deleteAgencyCompanyLocation(profileId: string, locationId: string): Promise<void> {
  return api.del(`${companyProfilesUrl}/${profileId}/Locations/${locationId}`);
}

export function updateAgencyCompanyContactInformation(profileId: string, model: Partial<CompanyProfileDetail>): Promise<void> {
  return api.put(`${companyProfilesUrl}/${profileId}/ContactInformation`, model);
}

export function getAgencyCompanyJobPositions(companyProfileId: string, filter?: AgencyCompanyJobPositionFilter): Promise<AgencyCompanyJobPosition[]> {
  return api.get<AgencyCompanyJobPosition[]>(`${companyProfilesUrl}/${companyProfileId}/JobPositions`, { params: { ...filter } });
}

export function getAgencyCompanyJobPositionById(profileId: string, id: string): Promise<AgencyCompanyJobPosition> {
  return api.get<AgencyCompanyJobPosition>(`${companyProfilesUrl}/${profileId}/JobPositions/${id}`);
}

export function createAgencyCompanyJobPosition(profileId: string, model: AgencyCompanyJobPosition): Promise<{ id: string }> {
  return api.post<{ id: string }>(`${companyProfilesUrl}/${profileId}/JobPositions`, model);
}

export function updateAgencyCompanyJobPosition(profileId: string, id: string, model: AgencyCompanyJobPosition): Promise<void> {
  return api.put(`${companyProfilesUrl}/${profileId}/JobPositions/${id}`, model);
}

export function deleteAgencyCompanyJobPosition(profileId: string, id: string): Promise<void> {
  return api.del(`${companyProfilesUrl}/${profileId}/JobPositions/${id}`);
}

export function petitionAgencyCompanyJobPosition(profileId: string, model: PetitionJobPositionPayload): Promise<void> {
  return api.post(`${companyProfilesUrl}/${profileId}/JobPositions/Petition`, model);
}

export function getAgencyCompanyDocument(
  profileId: string,
  pagination: { size: number; page: number },
): Promise<PaginatedList<CompanyProfileDocumentModel>> {
  return api
    .get<PaginatedList<CompanyProfileDocumentModel>>(`${companyProfilesUrl}/${profileId}/Documents?PageSize=${pagination.size}&PageIndex=${pagination.page}`);
}

export function createAgencyCompanyDocument(profileId: string, model: CompanyProfileDocumentModel, file: File): Promise<string> {
  return api.post<string>(
    `${companyProfilesUrl}/${profileId}/Documents`,
    buildMultipartFormData(model, { [model.fileName]: file }),
    { headers: { 'Content-Type': 'multipart/form-data' } },
  );
}

export function deleteAgencyCompanyDocument(profileId: string, id: string): Promise<void> {
  return api.del(`${companyProfilesUrl}/${profileId}/Documents/${id}`);
}

export function getInvoiceNotes(id: string): Promise<InvoiceNotesModel> {
  return api.get<InvoiceNotesModel>(`${companyProfilesUrl}/${id}/InvoiceNotes`);
}

export function postInvoiceNotes(id: string, model: InvoiceNotesModel): Promise<void> {
  return api.put(`${companyProfilesUrl}/${id}/InvoiceNotes`, model);
}

export function getCompanyInvoiceRecipients(companyProfileId: string): Promise<InvoiceRecipientModel[]> {
  return api.get<InvoiceRecipientModel[]>(`${companyProfilesUrl}/${companyProfileId}/InvoiceRecipients`);
}

export function postCompanyInvoiceRecipient(companyProfileId: string, model: InvoiceRecipientModel): Promise<{ id: string }> {
  return api.post<{ id: string }>(`${companyProfilesUrl}/${companyProfileId}/InvoiceRecipients`, model);
}

export function deleteCompanyInvoiceRecipient(companyProfileId: string, id: string): Promise<void> {
  return api.del(`${companyProfilesUrl}/${companyProfileId}/InvoiceRecipients/${id}`);
}

export function updatePermissionToSeeRequests(companyId: string, settings: CompanyProfileSettingsUpdate): Promise<void> {
  return api
    .patch(`${companyProfilesUrl}/${companyId}/RequiresPermissionToSeeRequests`, settings);
}

export function updatePaidHolidays(companyId: string, settings: CompanyProfileSettingsUpdate): Promise<void> {
  return api.patch(`${companyProfilesUrl}/${companyId}/PaidHolidays`, settings);
}

export function updateOvertime(companyId: string, settings: CompanyProfileSettingsUpdate): Promise<void> {
  return api.patch(`${companyProfilesUrl}/${companyId}/Overtime`, settings);
}

export function getCompanyUsers(profileId: string): Promise<CompanyUserModel[]> {
  return api.get<CompanyUserModel[]>(`${companyProfilesUrl}/${profileId}/Users`);
}

export function createCompanyProfileUser(profileId: string, user: CreateCompanyUserModel): Promise<{ id: string }> {
  return api.post<{ id: string }>(`${companyProfilesUrl}/${profileId}/Users`, user);
}

export function deleteCompanyProfileUser(profileId: string, userId: string): Promise<void> {
  return api.del(`${companyProfilesUrl}/${profileId}/Users/${userId}`);
}

export function updateIsAsapRequests(model: UpdateIsAsapRequestsPayload): Promise<void> {
  return api.put('/api/agency/recruiting/requests/is-asap', model);
}
