import { api } from '@/app/security/apiService';
import type { AgencyDetail, AgencyLocationDetail, AgencyPersonnelCreateModel, AgencyPersonnelListItem, PersonnelAgencyItem, UpdateUserAttendanceModel, UserAttendanceReport, UserAttendanceReportFilter, UserAttendanceToday } from '@/modules/agency/profile/types';

// Attendance (clock-in / clock-out of the agency users, in the time zone of the user's device)
function deviceTimeZone(): { timeZone: string } {
  return { timeZone: Intl.DateTimeFormat().resolvedOptions().timeZone };
}

// Profile (current logged-in agency)
export function getAgencyProfile(): Promise<AgencyDetail> {
  return api.get<AgencyDetail>('/api/agency/profile');
}

// Agency Personnel (users of the agency back-office)
export function getAgencyPersonnel(): Promise<AgencyPersonnelListItem[]> {
  return api.get<AgencyPersonnelListItem[]>('/api/agency/profile/personnel');
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
export function getAgencyLocations(): Promise<AgencyLocationDetail[]> {
  return api.get<AgencyLocationDetail[]>('/api/agency/profile/locations');
}

export function createAgencyLocation(model: AgencyLocationDetail): Promise<{ id: string }> {
  return api.post<{ id: string }>('/api/agency/profile/locations', model);
}

export function updateAgencyLocation(id: string, model: AgencyLocationDetail): Promise<void> {
  return api.put(`/api/agency/profile/locations/${id}`, model);
}

export function deleteAgencyLocation(id: string): Promise<void> {
  return api.del(`/api/agency/profile/locations/${id}`);
}

// Personnel Agencies (agencies a user has access to + switching)
export function getPersonnelAgencies(): Promise<PersonnelAgencyItem[]> {
  return api.get<PersonnelAgencyItem[]>('/api/agency/profile/personnel/agencies');
}

export function switchPersonnelAgency(id: string): Promise<void> {
  return api.put(`/api/agency/profile/personnel/agencies/${id}`);
}

export function getAttendanceToday(): Promise<UserAttendanceToday> {
  return api.get<UserAttendanceToday>('/api/agency/profile/attendance/today', { params: deviceTimeZone() });
}

export function toggleAttendance(): Promise<UserAttendanceToday> {
  return api.post<UserAttendanceToday>('/api/agency/profile/attendance', null, { params: deviceTimeZone() });
}

export function getAttendancesTodayForUsers(): Promise<UserAttendanceToday[]> {
  return api.get<UserAttendanceToday[]>('/api/agency/profile/attendance/today/users', { params: deviceTimeZone() });
}

export function getAttendanceReport(filter: UserAttendanceReportFilter): Promise<UserAttendanceReport> {
  return api.get<UserAttendanceReport>('/api/agency/profile/attendance/report', { params: { ...filter } });
}

export function downloadAttendanceReport(filter: UserAttendanceReportFilter): Promise<Blob> {
  return api.get<Blob>('/api/agency/profile/attendance/report/file', { params: { ...filter }, responseType: 'blob' });
}

export function updateAttendance(id: string, model: UpdateUserAttendanceModel): Promise<void> {
  return api.put(`/api/agency/profile/attendance/${id}`, model);
}
