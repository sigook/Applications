import { api } from '@/security/apiService';
import type { PaginatedList } from '@/types/common';
import type {
  AgencyDetail,
  AgencyListFilter,
  AgencyListItem,
  AgencyLocationDetail,
  AgencyPersonnelCreateModel,
  AgencyPersonnelListItem,
  CreateAgencyModel,
  PersonnelAgencyItem,
  UpdateUserAttendanceModel,
  UserAttendanceReport,
  UserAttendanceReportFilter,
  UserAttendanceToday,
} from '@/types/agency';

// Profile (current logged-in agency)
export function getAgencyProfile(): Promise<AgencyDetail> {
  return api.get<AgencyDetail>('/api/Agency/Profile');
}

// Agency CRUD
export function getAgency(id: string): Promise<AgencyDetail> {
  return api.get<AgencyDetail>(`/api/Agency/${id}`);
}

export function getAgenciesList(filter: AgencyListFilter): Promise<PaginatedList<AgencyListItem>> {
  return api.get<PaginatedList<AgencyListItem>>('/api/Agency', { params: { ...filter } });
}

export function createAgency(model: CreateAgencyModel): Promise<{ id: string }> {
  return api.post<{ id: string }>('/api/Agency', model);
}

export function updateAgency(agency: AgencyDetail): Promise<void> {
  return api.put('/api/Agency', agency);
}

// Agency Personnel (users of the agency back-office)
export function getAgencyPersonnel(): Promise<AgencyPersonnelListItem[]> {
  return api.get<AgencyPersonnelListItem[]>('/api/agency/personnel');
}

export function createAgencyPersonnel(model: AgencyPersonnelCreateModel): Promise<void> {
  return api.post('/api/agency/personnel', model);
}

export function updateAgencyPersonnel(id: string, model: AgencyPersonnelCreateModel): Promise<void> {
  return api.put(`/api/agency/personnel/${id}`, model);
}

// Roles the logged-in user is allowed to assign when creating personnel
export function getAssignableRoles(): Promise<string[]> {
  return api.get<string[]>('/api/agency/personnel/Roles');
}

export function deleteAgencyPersonnel(id: string): Promise<void> {
  return api.del(`/api/agency/personnel/${id}`);
}

// Agency Locations (billing addresses)
export function getAgencyLocations(): Promise<AgencyLocationDetail[]> {
  return api.get<AgencyLocationDetail[]>('/api/Agency/Location');
}

export function createAgencyLocation(model: AgencyLocationDetail): Promise<{ id: string }> {
  return api.post<{ id: string }>('/api/Agency/Location', model);
}

export function updateAgencyLocation(id: string, model: AgencyLocationDetail): Promise<void> {
  return api.put(`/api/Agency/Location/${id}`, model);
}

export function deleteAgencyLocation(id: string): Promise<void> {
  return api.del(`/api/Agency/Location/${id}`);
}

// Personnel Agencies (agencies a user has access to + switching)
export function getPersonnelAgencies(): Promise<PersonnelAgencyItem[]> {
  return api.get<PersonnelAgencyItem[]>('/api/agency/personnel/Agencies');
}

export function switchPersonnelAgency(id: string): Promise<void> {
  return api.put(`/api/agency/personnel/Agencies/${id}`);
}

// Attendance (clock-in / clock-out of the agency users, in the time zone of the user's device)
function deviceTimeZone(): { timeZone: string } {
  return { timeZone: Intl.DateTimeFormat().resolvedOptions().timeZone };
}

export function getAttendanceToday(): Promise<UserAttendanceToday> {
  return api.get<UserAttendanceToday>('/api/agency/attendance/today', { params: deviceTimeZone() });
}

export function toggleAttendance(): Promise<UserAttendanceToday> {
  return api.post<UserAttendanceToday>('/api/agency/attendance', null, { params: deviceTimeZone() });
}

export function getAttendancesTodayForUsers(): Promise<UserAttendanceToday[]> {
  return api.get<UserAttendanceToday[]>('/api/agency/attendance/today/users', { params: deviceTimeZone() });
}

export function getAttendanceReport(filter: UserAttendanceReportFilter): Promise<UserAttendanceReport> {
  return api.get<UserAttendanceReport>('/api/agency/attendance/report', { params: { ...filter } });
}

export function downloadAttendanceReport(filter: UserAttendanceReportFilter): Promise<Blob> {
  return api.get<Blob>('/api/agency/attendance/report/file', { params: { ...filter }, responseType: 'blob' });
}

export function updateAttendance(id: string, model: UpdateUserAttendanceModel): Promise<void> {
  return api.put(`/api/agency/attendance/${id}`, model);
}
