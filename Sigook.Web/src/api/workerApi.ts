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
  WorkerCatalogItem,
  WorkerBasicInformationModel,
  WorkerContactInformationModel,
  WorkerEmergencyInformationModel,
  WorkerOtherInformationModel,
  WorkerJobExperienceModel,
  WorkerRequestListItem,
  WorkerRequestDetail,
  WorkerTimeSheetItem,
  WorkerWageHistoryItem,
  WorkerTimeSheetHistoryItem,
} from '@/types/worker';

// Requests
export function getJobs(filter: WorkerRequestFilter): Promise<PaginatedList<WorkerRequestListItem>> {
  return http.get('/api/WorkerRequest', { params: { ...filter } }).then(r => r.data);
}

export function getWorkerRequest(id: string): Promise<WorkerRequestDetail> {
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
export function workerRegisterTime(requestId: string, latitude: number, longitude: number): Promise<void> {
  return http.post(`/api/WorkerRequest/${requestId}/TimeSheet`, { latitude, longitude }).then(() => {});
}

export function workerGetTimeSheet(requestId: string): Promise<WorkerTimeSheetItem[]> {
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

export function registerWorker(payload: FormData): Promise<string> {
  return http.post('/api/WorkerProfile', payload, {
    headers: { 'Content-Type': 'multipart/form-data' }
  }).then(r => r.data);
}

export function uploadWorker(profileId: string, worker: WorkerProfile): Promise<void> {
  return http.put(`/api/WorkerProfile/${profileId}`, worker).then(() => {});
}

// Request History
export function getWorkerRequestHistory(filter: WorkerRequestFilter): Promise<PaginatedList<WorkerRequestListItem>> {
  return http.get('/api/WorkerRequestHistory', { params: { ...filter } }).then(r => r.data);
}

export function getWorkerRequestHistoryDetail(id: string): Promise<WorkerRequestDetail> {
  return http.get(`/api/WorkerRequestHistory/${id}`).then(r => r.data);
}

// Job Experience
export function createWorkerWorkExperience(profileId: string, model: WorkerJobExperienceModel): Promise<void> {
  return http.post(`/api/WorkerProfile/${profileId}/JobExperience`, model).then(() => {});
}

export function editWorkerWorkExperience(profileId: string, id: string, model: WorkerJobExperienceModel): Promise<void> {
  return http.put(`/api/WorkerProfile/${profileId}/JobExperience/${id}`, model).then(() => {});
}

export function deleteWorkerWorkExperience(profileId: string, id: string): Promise<void> {
  return http.delete(`/api/WorkerProfile/${profileId}/JobExperience/${id}`).then(() => {});
}

// SIN Information
export function createWorkerSin(profileId: string, formData: FormData): Promise<void> {
  return http.post(`/api/WorkerProfile/${profileId}/SinInformation`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  }).then(() => {});
}

// Basic Information
export function createWorkerBasicInformation(profileId: string, model: WorkerBasicInformationModel): Promise<void> {
  return http.post(`/api/WorkerProfile/${profileId}/BasicInformation`, model).then(() => {});
}

// Emergency Information
export function createWorkerEmergencyInformation(profileId: string, model: WorkerEmergencyInformationModel): Promise<void> {
  return http.post(`/api/WorkerProfile/${profileId}/EmergencyInformation`, model).then(() => {});
}

// Documents
export function createWorkerDocuments(profileId: string, formData: FormData): Promise<void> {
  return http.post(`/api/WorkerProfile/${profileId}/Documents`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  }).then(() => {});
}

// Resume
export function createWorkerResume(profileId: string, formData: FormData): Promise<void> {
  return http.post(`/api/WorkerProfile/${profileId}/Resume`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  }).then(() => {});
}

// Contact Information
export function createWorkerContactInformation(profileId: string, model: WorkerContactInformationModel): Promise<void> {
  return http.post(`/api/WorkerProfile/${profileId}/ContactInformation`, model).then(() => {});
}

// Availabilities
export function createWorkerAvailabilities(profileId: string, model: WorkerCatalogItem[]): Promise<void> {
  return http.post(`/api/WorkerProfile/${profileId}/Availabilities`, model).then(() => {});
}

export function createWorkerAvailabilityTimes(profileId: string, model: WorkerCatalogItem[]): Promise<void> {
  return http.post(`/api/WorkerProfile/${profileId}/AvailabilityTimes`, model).then(() => {});
}

export function createWorkerAvailabilityDays(profileId: string, model: WorkerCatalogItem[]): Promise<void> {
  return http.post(`/api/WorkerProfile/${profileId}/AvailabilityDays`, model).then(() => {});
}

// Location Preferences
export function createWorkerLocationPreferences(profileId: string, model: WorkerCatalogItem[]): Promise<void> {
  return http.post(`/api/WorkerProfile/${profileId}/LocationPreferences`, model).then(() => {});
}

// Languages
export function createWorkerLanguages(profileId: string, model: WorkerCatalogItem[]): Promise<void> {
  return http.post(`/api/WorkerProfile/${profileId}/Languages`, model).then(() => {});
}

// Other Information
export function createWorkerOther(profileId: string, model: WorkerOtherInformationModel): Promise<void> {
  return http.post(`/api/WorkerProfile/${profileId}/OtherInformation`, model).then(() => {});
}

// Skills
export function createWorkerSkills(profileId: string, model: string[]): Promise<void> {
  return http.post(`/api/WorkerProfile/${profileId}/Skills`, model).then(() => {});
}

// Licenses
export function createWorkerLicenses(profileId: string, formData: FormData): Promise<void> {
  return http.post(`/api/WorkerProfile/${profileId}/Licenses`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  }).then(() => {});
}

export function deleteWorkerLicenses(profileId: string, licenseId: string): Promise<void> {
  return http.delete(`/api/WorkerProfile/${profileId}/Licenses/${licenseId}`).then(() => {});
}

// Certificates
export function createWorkerCertificates(profileId: string, formData: FormData): Promise<void> {
  return http.post(`/api/WorkerProfile/${profileId}/Certificates`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  }).then(() => {});
}

export function deleteWorkerCertificates(profileId: string, certificateId: string): Promise<void> {
  return http.delete(`/api/WorkerProfile/${profileId}/Certificates/${certificateId}`).then(() => {});
}

// Other Documents
export function createWorkerOtherDocuments(profileId: string, formData: FormData): Promise<void> {
  return http.post(`/api/WorkerProfile/${profileId}/OtherDocument`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  }).then(() => {});
}

export function deleteWorkerOtherDocuments(profileId: string, otherDocumentId: string): Promise<void> {
  return http.delete(`/api/WorkerProfile/${profileId}/OtherDocument/${otherDocumentId}`).then(() => {});
}

// Profile Image
export function createWorkerImage(profileId: string, formData: FormData): Promise<void> {
  return http.post(`/api/WorkerProfile/${profileId}/ProfileImage`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  }).then(() => {});
}

// Wage History
export function getWorkerProfileWageHistory(filter: WageHistoryFilter): Promise<PaginatedList<WorkerWageHistoryItem>> {
  return http.get(`/api/WorkerProfile/${filter.profileId}/WageHistory`, { params: { ...filter } }).then(r => r.data);
}

export function getWorkerProfileWageHistoryAccumulated(profileId: string, rowNumber: number): Promise<WorkerWageHistoryItem> {
  return http.get(`/api/WorkerProfile/${profileId}/WageHistory/${rowNumber}`).then(r => r.data);
}

// TimeSheet History
export function getWorkerProfileTimeSheetHistory(filter: TimeSheetHistoryFilter): Promise<PaginatedList<WorkerTimeSheetHistoryItem>> {
  return http.get(`/api/WorkerProfile/${filter.profileId}/TimeSheetHistory`, { params: { ...filter } }).then(r => r.data);
}

export function getWorkerProfileTimeSheetHistoryAccumulated(profileId: string, rowNumber: number): Promise<WorkerTimeSheetHistoryItem> {
  return http.get(`/api/WorkerProfile/${profileId}/TimeSheetHistory/${rowNumber}`).then(r => r.data);
}
