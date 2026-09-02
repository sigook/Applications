import { api } from '@/security/apiService';
import { ClockType } from '@/constants/enums';
import type { PaginatedList } from '@/types/common';
import type {
  WorkerProfile,
  WorkerRequestFilter,
  WorkerRequestApplyModel,
  WorkerCommentFilter,
  WorkerCommentList,
  WageHistoryFilter,
  TimeSheetHistoryFilter,
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
  return api.get<PaginatedList<WorkerRequestListItem>>('/api/WorkerRequest', { params: { ...filter } });
}

export function getWorkerRequest(id: string): Promise<WorkerRequestDetail> {
  return api.get<WorkerRequestDetail>(`/api/WorkerRequest/${id}`);
}

export function workerRequestApplySelf(requestId: string, model: WorkerRequestApplyModel): Promise<void> {
  return api.post(`/api/WorkerRequest/${requestId}/Apply/`, model);
}

export function requestApplyByEmail(numberId: number, email: string): Promise<void> {
  return api.post<void>('/api/WorkerRequest/Apply', { numberId, email });
}

export function workerRequestDecline(id: string): Promise<void> {
  return api.del(`/api/WorkerRequest/Decline/${id}`);
}

// TimeSheet
export function workerRegisterTime(requestId: string, latitude: number, longitude: number): Promise<void> {
  return api.post(`/api/WorkerRequest/${requestId}/TimeSheet`, { latitude, longitude });
}

export function workerGetTimeSheet(requestId: string): Promise<WorkerTimeSheetItem[]> {
  return api.get<WorkerTimeSheetItem[]>(`/api/WorkerRequest/${requestId}/TimeSheet`);
}

export function getClockType(
  requestId: string,
  latitude: number,
  longitude: number,
  date: string
): Promise<ClockType> {
  return api.get<ClockType>(`/api/WorkerRequest/${requestId}/TimeSheet/clock-type/${latitude}/${longitude}`, {
    params: { date }
  });
}

// Comments
export function getCommentsWorker(filter: WorkerCommentFilter): Promise<WorkerCommentList> {
  return api.get<WorkerCommentList>(`/api/worker/${filter.workerId}/comment`, {
    params: { PageSize: filter.size, PageIndex: filter.pageIndex }
  });
}

// Profile
export function getMyProfile(): Promise<WorkerProfile> {
  return api.get<WorkerProfile>('/api/WorkerProfile/me');
}

export function registerWorker(payload: FormData): Promise<string> {
  return api.post<string>('/api/WorkerProfile', payload, {
    headers: { 'Content-Type': 'multipart/form-data' }
  });
}

export function uploadWorker(profileId: string, worker: WorkerProfile): Promise<void> {
  return api.put(`/api/WorkerProfile/${profileId}`, worker);
}

// Request History
export function getWorkerRequestHistory(filter: WorkerRequestFilter): Promise<PaginatedList<WorkerRequestListItem>> {
  return api.get<PaginatedList<WorkerRequestListItem>>('/api/WorkerRequestHistory', { params: { ...filter } });
}

export function getWorkerRequestHistoryDetail(id: string): Promise<WorkerRequestDetail> {
  return api.get<WorkerRequestDetail>(`/api/WorkerRequestHistory/${id}`);
}

// Job Experience
export function createWorkerWorkExperience(profileId: string, model: WorkerJobExperienceModel): Promise<void> {
  return api.post(`/api/WorkerProfile/${profileId}/JobExperience`, model);
}

export function editWorkerWorkExperience(profileId: string, id: string, model: WorkerJobExperienceModel): Promise<void> {
  return api.put(`/api/WorkerProfile/${profileId}/JobExperience/${id}`, model);
}

export function deleteWorkerWorkExperience(profileId: string, id: string): Promise<void> {
  return api.del(`/api/WorkerProfile/${profileId}/JobExperience/${id}`);
}

// SIN Information
export function createWorkerSin(profileId: string, formData: FormData): Promise<void> {
  return api.post(`/api/WorkerProfile/${profileId}/SinInformation`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  });
}

// Basic Information
export function createWorkerBasicInformation(profileId: string, model: WorkerBasicInformationModel): Promise<void> {
  return api.post(`/api/WorkerProfile/${profileId}/BasicInformation`, model);
}

// Emergency Information
export function createWorkerEmergencyInformation(profileId: string, model: WorkerEmergencyInformationModel): Promise<void> {
  return api.post(`/api/WorkerProfile/${profileId}/EmergencyInformation`, model);
}

// Documents
export function createWorkerDocuments(profileId: string, formData: FormData): Promise<void> {
  return api.post(`/api/WorkerProfile/${profileId}/Documents`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  });
}

// Resume
export function createWorkerResume(profileId: string, formData: FormData): Promise<void> {
  return api.post(`/api/WorkerProfile/${profileId}/Resume`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  });
}

// Contact Information
export function createWorkerContactInformation(profileId: string, model: WorkerContactInformationModel): Promise<void> {
  return api.post(`/api/WorkerProfile/${profileId}/ContactInformation`, model);
}

// Availabilities
export function createWorkerAvailabilities(profileId: string, model: WorkerCatalogItem[]): Promise<void> {
  return api.post(`/api/WorkerProfile/${profileId}/Availabilities`, model);
}

export function createWorkerAvailabilityTimes(profileId: string, model: WorkerCatalogItem[]): Promise<void> {
  return api.post(`/api/WorkerProfile/${profileId}/AvailabilityTimes`, model);
}

export function createWorkerAvailabilityDays(profileId: string, model: WorkerCatalogItem[]): Promise<void> {
  return api.post(`/api/WorkerProfile/${profileId}/AvailabilityDays`, model);
}

// Location Preferences
export function createWorkerLocationPreferences(profileId: string, model: WorkerCatalogItem[]): Promise<void> {
  return api.post(`/api/WorkerProfile/${profileId}/LocationPreferences`, model);
}

// Languages
export function createWorkerLanguages(profileId: string, model: WorkerCatalogItem[]): Promise<void> {
  return api.post(`/api/WorkerProfile/${profileId}/Languages`, model);
}

// Other Information
export function createWorkerOther(profileId: string, model: WorkerOtherInformationModel): Promise<void> {
  return api.post(`/api/WorkerProfile/${profileId}/OtherInformation`, model);
}

// Skills
export function createWorkerSkills(profileId: string, model: string[]): Promise<void> {
  return api.post(`/api/WorkerProfile/${profileId}/Skills`, model);
}

// Licenses
export function createWorkerLicenses(profileId: string, formData: FormData): Promise<void> {
  return api.post(`/api/WorkerProfile/${profileId}/Licenses`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  });
}

export function deleteWorkerLicenses(profileId: string, licenseId: string): Promise<void> {
  return api.del(`/api/WorkerProfile/${profileId}/Licenses/${licenseId}`);
}

// Certificates
export function createWorkerCertificates(profileId: string, formData: FormData): Promise<void> {
  return api.post(`/api/WorkerProfile/${profileId}/Certificates`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  });
}

export function deleteWorkerCertificates(profileId: string, certificateId: string): Promise<void> {
  return api.del(`/api/WorkerProfile/${profileId}/Certificates/${certificateId}`);
}

// Other Documents
export function createWorkerOtherDocuments(profileId: string, formData: FormData): Promise<void> {
  return api.post(`/api/WorkerProfile/${profileId}/OtherDocument`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  });
}

export function deleteWorkerOtherDocuments(profileId: string, otherDocumentId: string): Promise<void> {
  return api.del(`/api/WorkerProfile/${profileId}/OtherDocument/${otherDocumentId}`);
}

// Profile Image
export function createWorkerImage(profileId: string, formData: FormData): Promise<void> {
  return api.post(`/api/WorkerProfile/${profileId}/ProfileImage`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  });
}

// Wage History
export function getWorkerProfileWageHistory(filter: WageHistoryFilter): Promise<PaginatedList<WorkerWageHistoryItem>> {
  return api.get<PaginatedList<WorkerWageHistoryItem>>(`/api/WorkerProfile/${filter.profileId}/WageHistory`, { params: { ...filter } });
}

export function getWorkerProfileWageHistoryAccumulated(profileId: string, rowNumber: number): Promise<WorkerWageHistoryItem> {
  return api.get<WorkerWageHistoryItem>(`/api/WorkerProfile/${profileId}/WageHistory/${rowNumber}`);
}

// TimeSheet History
export function getWorkerProfileTimeSheetHistory(filter: TimeSheetHistoryFilter): Promise<PaginatedList<WorkerTimeSheetHistoryItem>> {
  return api.get<PaginatedList<WorkerTimeSheetHistoryItem>>(`/api/WorkerProfile/${filter.profileId}/TimeSheetHistory`, { params: { ...filter } });
}

export function getWorkerProfileTimeSheetHistoryAccumulated(profileId: string, rowNumber: number): Promise<WorkerTimeSheetHistoryItem> {
  return api.get<WorkerTimeSheetHistoryItem>(`/api/WorkerProfile/${profileId}/TimeSheetHistory/${rowNumber}`);
}
