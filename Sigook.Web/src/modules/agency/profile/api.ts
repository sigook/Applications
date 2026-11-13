import { api } from '@/app/security/apiService';
import type { AgencyDetail, AgencyLocationDetail, AgencyPersonnelCreateModel, AgencyPersonnelListItem, AgencyOption, UpdateUserAttendanceModel, UserAttendanceReport, UserAttendanceReportFilter, UserAttendanceToday } from '@/modules/agency/profile/types';

// Attendance (clock-in / clock-out of the agency users, in the time zone of the user's device)
function deviceTimeZone(): { timeZone: string } {
  return { timeZone: Intl.DateTimeFormat().resolvedOptions().timeZone };
}

function agencyParams(agencyId?: string): { agencyId?: string } {
  return agencyId ? { agencyId } : {};
}

function agenciesParams(agencyIds?: string): { agencyIds?: string } {
  return agencyIds === undefined ? {} : { agencyIds };
}

export function getAgencyProfile(agencyId?: string): Promise<AgencyDetail> {
  return api.get<AgencyDetail>('/api/agency/profile', { params: agencyParams(agencyId) });
}

export function updateAgencyProfile(agencyId: string, agency: AgencyDetail): Promise<void> {
  return api.put('/api/agency/profile', agency, { params: agencyParams(agencyId) });
}

// Agency Personnel (users of the agency back-office)
export function getAgencyPersonnel(agencyIds?: string): Promise<AgencyPersonnelListItem[]> {
  return api.get<AgencyPersonnelListItem[]>('/api/agency/profile/personnel', { params: agenciesParams(agencyIds) });
}

export function createAgencyPersonnel(model: AgencyPersonnelCreateModel): Promise<void> {
  return api.post('/api/agency/profile/personnel', model);
}

export function updateAgencyPersonnel(id: string, model: AgencyPersonnelCreateModel): Promise<void> {
  return api.put(`/api/agency/profile/personnel/${id}`, model);
}

// Roles the logged-in user is allowed to assign when creating personnel
export function getAssignableRoles(): Promise<string[]> {
  return api.get<string[]>('/api/agency/profile/personnel/Roles');
}

export function deleteAgencyPersonnel(id: string): Promise<void> {
  return api.del(`/api/agency/profile/personnel/${id}`);
}

// Agency Locations (billing addresses)
export function getAgencyLocations(agencyId: string): Promise<AgencyLocationDetail[]> {
  return api.get<AgencyLocationDetail[]>('/api/agency/profile/locations', { params: agencyParams(agencyId) });
}

export function createAgencyLocation(agencyId: string, model: AgencyLocationDetail): Promise<{ id: string }> {
  return api.post<{ id: string }>('/api/agency/profile/locations', model, { params: agencyParams(agencyId) });
}

export function updateAgencyLocation(agencyId: string, id: string, model: AgencyLocationDetail): Promise<void> {
  return api.put(`/api/agency/profile/locations/${id}`, model, { params: agencyParams(agencyId) });
}

export function deleteAgencyLocation(agencyId: string, id: string): Promise<void> {
  return api.del(`/api/agency/profile/locations/${id}`, { params: agencyParams(agencyId) });
}

export function getAccessibleAgencies(): Promise<AgencyOption[]> {
  return api.get<AgencyOption[]>('/api/agency/profile/agencies');
}

export function getAttendanceToday(): Promise<UserAttendanceToday> {
  return api.get<UserAttendanceToday>('/api/agency/profile/attendance/today', { params: deviceTimeZone() });
}

export function toggleAttendance(): Promise<UserAttendanceToday> {
  return api.post<UserAttendanceToday>('/api/agency/profile/attendance', null, { params: deviceTimeZone() });
}

export function getAttendancesTodayForUsers(agencyIds?: string): Promise<UserAttendanceToday[]> {
  return api.get<UserAttendanceToday[]>('/api/agency/profile/attendance/today/users', { params: { ...deviceTimeZone(), ...agenciesParams(agencyIds) } });
}

export function getAttendanceReport(agencyIds: string | undefined, filter: UserAttendanceReportFilter): Promise<UserAttendanceReport> {
  return api.get<UserAttendanceReport>('/api/agency/profile/attendance/report', { params: { ...filter, ...agenciesParams(agencyIds) } });
}

export function downloadAttendanceReport(agencyIds: string | undefined, filter: UserAttendanceReportFilter): Promise<Blob> {
  return api.get<Blob>('/api/agency/profile/attendance/report/file', { params: { ...filter, ...agenciesParams(agencyIds) }, responseType: 'blob' });
}

export function updateAttendance(id: string, model: UpdateUserAttendanceModel): Promise<void> {
  return api.put(`/api/agency/profile/attendance/${id}`, model);
}
