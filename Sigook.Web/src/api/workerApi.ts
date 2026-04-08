import http from '@/security/apiService';
import type { PaginatedList } from '@/types/common';
import type {
  WorkerProfile,
  WorkerRequestFilter,
  WorkerRequestApplyModel,
  WorkerCommentFilter,
  WorkerCommentList,
  WageHistoryFilter,
  TimeSheetHistoryFilter,
  ClockTypeResult,
} from '@/types/worker';

// Requests
export function getJobs(filter: WorkerRequestFilter): Promise<PaginatedList<Record<string, unknown>>> {
  return http.get('/api/WorkerRequest', { params: { ...filter } }).then(r => r.data);
}

export function getWorkerRequest(id: string): Promise<Record<string, unknown>> {
  return http.get(`/api/WorkerRequest/${id}`).then(r => r.data);
}

export function workerRequestApplySelf(requestId: string, model: WorkerRequestApplyModel): Promise<void> {
  return http.post(`/api/WorkerRequest/${requestId}/Apply/`, model).then(() => {});
}

export function workerRequestApply(workerId: string, requestId: string, model: WorkerRequestApplyModel): Promise<void> {
  return http.post(`/api/WorkerRequest/${workerId}/${requestId}/Apply`, model).then(() => {});
}

export function workerRequestDecline(id: string): Promise<void> {
  return http.delete(`/api/WorkerRequest/Decline/${id}`).then(() => {});
}

// TimeSheet
export function workerRegisterTime(requestId: string, latitude: number, longitude: number): Promise<Record<string, unknown>> {
  return http.post(`/api/WorkerRequest/${requestId}/TimeSheet`, { latitude, longitude }).then(r => r.data);
}

export function workerGetTimeSheet(requestId: string): Promise<Record<string, unknown>[]> {
  return http.get(`/api/WorkerRequest/${requestId}/TimeSheet`).then(r => r.data);
}

export function getClockType(requestId: string, date: string): Promise<ClockTypeResult> {
  return http.get(`/api/WorkerRequest/${requestId}/TimeSheet/clock-type`, { params: { date } }).then(r => r.data);
}

// Comments
export function getCommentsWorker(filter: WorkerCommentFilter): Promise<WorkerCommentList> {
  return http.get(`/api/worker/${filter.workerId}/comment`, {
    params: { PageSize: filter.size, PageIndex: filter.pageIndex }
  }).then(r => r.data);
}

// Profile
export function getMyProfile(): Promise<WorkerProfile> {
  return http.get('/api/WorkerProfile/me').then(r => r.data);
}

export function registerWorker(payload: FormData): Promise<Record<string, unknown>> {
  return http.post('/api/WorkerProfile', payload, {
    headers: { 'Content-Type': 'multipart/form-data' }
  }).then(r => r.data);
}

export function uploadWorker(profileId: string, worker: Record<string, unknown>): Promise<Record<string, unknown>> {
  return http.put(`/api/WorkerProfile/${profileId}`, worker).then(r => r.data);
}

// Request History
export function getWorkerRequestHistory(filter: WorkerRequestFilter): Promise<PaginatedList<Record<string, unknown>>> {
  return http.get('/api/WorkerRequestHistory', { params: { ...filter } }).then(r => r.data);
}

export function getWorkerRequestHistoryDetail(id: string): Promise<Record<string, unknown>> {
  return http.get(`/api/WorkerRequestHistory/${id}`).then(r => r.data);
}

// Job Experience
export function createWorkerWorkExperience(profileId: string, model: Record<string, unknown>): Promise<Record<string, unknown>> {
  return http.post(`/api/WorkerProfile/${profileId}/JobExperience`, model).then(r => r.data);
}

export function editWorkerWorkExperience(profileId: string, id: string, model: Record<string, unknown>): Promise<Record<string, unknown>> {
  return http.put(`/api/WorkerProfile/${profileId}/JobExperience/${id}`, model).then(r => r.data);
}

export function deleteWorkerWorkExperience(profileId: string, id: string): Promise<Record<string, unknown>> {
  return http.delete(`/api/WorkerProfile/${profileId}/JobExperience/${id}`).then(r => r.data);
}

// SIN Information
export function createWorkerSin(profileId: string, formData: FormData): Promise<Record<string, unknown>> {
  return http.post(`/api/WorkerProfile/${profileId}/SinInformation`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  }).then(r => r.data);
}

// Basic Information
export function createWorkerBasicInformation(profileId: string, model: Record<string, unknown>): Promise<Record<string, unknown>> {
  return http.post(`/api/WorkerProfile/${profileId}/BasicInformation`, model).then(r => r.data);
}

// Emergency Information
export function createWorkerEmergencyInformation(profileId: string, model: Record<string, unknown>): Promise<Record<string, unknown>> {
  return http.post(`/api/WorkerProfile/${profileId}/EmergencyInformation`, model).then(r => r.data);
}

// Documents
export function createWorkerDocuments(profileId: string, formData: FormData): Promise<Record<string, unknown>> {
  return http.post(`/api/WorkerProfile/${profileId}/Documents`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  }).then(r => r.data);
}

// Resume
export function createWorkerResume(profileId: string, formData: FormData): Promise<Record<string, unknown>> {
  return http.post(`/api/WorkerProfile/${profileId}/Resume`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  }).then(r => r.data);
}

// Contact Information
export function createWorkerContactInformation(profileId: string, model: Record<string, unknown>): Promise<Record<string, unknown>> {
  return http.post(`/api/WorkerProfile/${profileId}/ContactInformation`, model).then(r => r.data);
}

// Availabilities
export function createWorkerAvailabilities(profileId: string, model: Record<string, unknown>[]): Promise<Record<string, unknown>> {
  return http.post(`/api/WorkerProfile/${profileId}/Availabilities`, model).then(r => r.data);
}

export function createWorkerAvailabilityTimes(profileId: string, model: Record<string, unknown>[]): Promise<Record<string, unknown>> {
  return http.post(`/api/WorkerProfile/${profileId}/AvailabilityTimes`, model).then(r => r.data);
}

export function createWorkerAvailabilityDays(profileId: string, model: Record<string, unknown>[]): Promise<Record<string, unknown>> {
  return http.post(`/api/WorkerProfile/${profileId}/AvailabilityDays`, model).then(r => r.data);
}

// Location Preferences
export function createWorkerLocationPreferences(profileId: string, model: Record<string, unknown>[]): Promise<Record<string, unknown>> {
  return http.post(`/api/WorkerProfile/${profileId}/LocationPreferences`, model).then(r => r.data);
}

// Languages
export function createWorkerLanguages(profileId: string, model: Record<string, unknown>[]): Promise<Record<string, unknown>> {
  return http.post(`/api/WorkerProfile/${profileId}/Languages`, model).then(r => r.data);
}

// Other Information
export function createWorkerOther(profileId: string, model: Record<string, unknown>): Promise<Record<string, unknown>> {
  return http.post(`/api/WorkerProfile/${profileId}/OtherInformation`, model).then(r => r.data);
}

// Skills
export function createWorkerSkills(profileId: string, model: Record<string, unknown>[]): Promise<Record<string, unknown>> {
  return http.post(`/api/WorkerProfile/${profileId}/Skills`, model).then(r => r.data);
}

// Licenses
export function createWorkerLicenses(profileId: string, formData: FormData): Promise<Record<string, unknown>> {
  return http.post(`/api/WorkerProfile/${profileId}/Licenses`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  }).then(r => r.data);
}

export function deleteWorkerLicenses(profileId: string, licenseId: string): Promise<Record<string, unknown>> {
  return http.delete(`/api/WorkerProfile/${profileId}/Licenses/${licenseId}`).then(r => r.data);
}

// Certificates
export function createWorkerCertificates(profileId: string, formData: FormData): Promise<Record<string, unknown>> {
  return http.post(`/api/WorkerProfile/${profileId}/Certificates`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  }).then(r => r.data);
}

export function deleteWorkerCertificates(profileId: string, certificateId: string): Promise<Record<string, unknown>> {
  return http.delete(`/api/WorkerProfile/${profileId}/Certificates/${certificateId}`).then(r => r.data);
}

// Other Documents
export function createWorkerOtherDocuments(profileId: string, formData: FormData): Promise<Record<string, unknown>> {
  return http.post(`/api/WorkerProfile/${profileId}/OtherDocument`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  }).then(r => r.data);
}

export function deleteWorkerOtherDocuments(profileId: string, otherDocumentId: string): Promise<Record<string, unknown>> {
  return http.delete(`/api/WorkerProfile/${profileId}/OtherDocument/${otherDocumentId}`).then(r => r.data);
}

// Profile Image
export function createWorkerImage(profileId: string, formData: FormData): Promise<Record<string, unknown>> {
  return http.post(`/api/WorkerProfile/${profileId}/ProfileImage`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  }).then(r => r.data);
}

// Wage History
export function getWorkerProfileWageHistory(filter: WageHistoryFilter): Promise<PaginatedList<Record<string, unknown>>> {
  return http.get(`/api/WorkerProfile/${filter.profileId}/WageHistory`, { params: { ...filter } }).then(r => r.data);
}

export function getWorkerProfileWageHistoryAccumulated(profileId: string, rowNumber: number): Promise<Record<string, unknown>> {
  return http.get(`/api/WorkerProfile/${profileId}/WageHistory/${rowNumber}`).then(r => r.data);
}

// TimeSheet History
export function getWorkerProfileTimeSheetHistory(filter: TimeSheetHistoryFilter): Promise<PaginatedList<Record<string, unknown>>> {
  return http.get(`/api/WorkerProfile/${filter.profileId}/TimeSheetHistory`, { params: { ...filter } }).then(r => r.data);
}

export function getWorkerProfileTimeSheetHistoryAccumulated(profileId: string, rowNumber: number): Promise<Record<string, unknown>> {
  return http.get(`/api/WorkerProfile/${profileId}/TimeSheetHistory/${rowNumber}`).then(r => r.data);
}
