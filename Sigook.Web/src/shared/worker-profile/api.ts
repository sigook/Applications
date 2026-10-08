import { api } from '@/app/security/apiService';
import type { WorkerCatalogItem, WorkerBasicInformationModel, WorkerContactInformationModel, WorkerEmergencyInformationModel, WorkerOtherInformationModel, WorkerJobExperienceModel } from '@/shared/worker-profile/types';

// Job Experience
export function createWorkerWorkExperience(profileId: string, model: WorkerJobExperienceModel): Promise<void> {
  return api.post(`/api/worker/profile/${profileId}/JobExperience`, model);
}

export function editWorkerWorkExperience(profileId: string, id: string, model: WorkerJobExperienceModel): Promise<void> {
  return api.put(`/api/worker/profile/${profileId}/JobExperience/${id}`, model);
}

export function deleteWorkerWorkExperience(profileId: string, id: string): Promise<void> {
  return api.del(`/api/worker/profile/${profileId}/JobExperience/${id}`);
}

// SIN Information
export function createWorkerSin(profileId: string, formData: FormData): Promise<void> {
  return api.post(`/api/worker/profile/${profileId}/SinInformation`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  });
}

// Basic Information
export function createWorkerBasicInformation(profileId: string, model: WorkerBasicInformationModel): Promise<void> {
  return api.post(`/api/worker/profile/${profileId}/BasicInformation`, model);
}

// Emergency Information
export function createWorkerEmergencyInformation(profileId: string, model: WorkerEmergencyInformationModel): Promise<void> {
  return api.post(`/api/worker/profile/${profileId}/EmergencyInformation`, model);
}

// Documents
export function createWorkerDocuments(profileId: string, formData: FormData): Promise<void> {
  return api.post(`/api/worker/profile/${profileId}/Documents`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  });
}

// Resume
export function createWorkerResume(profileId: string, formData: FormData): Promise<void> {
  return api.post(`/api/worker/profile/${profileId}/Resume`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  });
}

// Contact Information
export function createWorkerContactInformation(profileId: string, model: WorkerContactInformationModel): Promise<void> {
  return api.post(`/api/worker/profile/${profileId}/ContactInformation`, model);
}

// Availabilities
export function createWorkerAvailabilities(profileId: string, model: WorkerCatalogItem[]): Promise<void> {
  return api.post(`/api/worker/profile/${profileId}/Availabilities`, model);
}

export function createWorkerAvailabilityTimes(profileId: string, model: WorkerCatalogItem[]): Promise<void> {
  return api.post(`/api/worker/profile/${profileId}/AvailabilityTimes`, model);
}

export function createWorkerAvailabilityDays(profileId: string, model: WorkerCatalogItem[]): Promise<void> {
  return api.post(`/api/worker/profile/${profileId}/AvailabilityDays`, model);
}

// Location Preferences
export function createWorkerLocationPreferences(profileId: string, model: WorkerCatalogItem[]): Promise<void> {
  return api.post(`/api/worker/profile/${profileId}/LocationPreferences`, model);
}

// Languages
export function createWorkerLanguages(profileId: string, model: WorkerCatalogItem[]): Promise<void> {
  return api.post(`/api/worker/profile/${profileId}/Languages`, model);
}

// Other Information
export function createWorkerOther(profileId: string, model: WorkerOtherInformationModel): Promise<void> {
  return api.post(`/api/worker/profile/${profileId}/OtherInformation`, model);
}

// Skills
export function createWorkerSkills(profileId: string, model: string[]): Promise<void> {
  return api.post(`/api/worker/profile/${profileId}/Skills`, model);
}

// Licenses
export function createWorkerLicenses(profileId: string, formData: FormData): Promise<void> {
  return api.post(`/api/worker/profile/${profileId}/Licenses`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  });
}

export function deleteWorkerLicenses(profileId: string, licenseId: string): Promise<void> {
  return api.del(`/api/worker/profile/${profileId}/Licenses/${licenseId}`);
}

// Certificates
export function createWorkerCertificates(profileId: string, formData: FormData): Promise<void> {
  return api.post(`/api/worker/profile/${profileId}/Certificates`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  });
}

export function deleteWorkerCertificates(profileId: string, certificateId: string): Promise<void> {
  return api.del(`/api/worker/profile/${profileId}/Certificates/${certificateId}`);
}

// Other Documents
export function createWorkerOtherDocuments(profileId: string, formData: FormData): Promise<void> {
  return api.post(`/api/worker/profile/${profileId}/OtherDocument`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  });
}

export function deleteWorkerOtherDocuments(profileId: string, otherDocumentId: string): Promise<void> {
  return api.del(`/api/worker/profile/${profileId}/OtherDocument/${otherDocumentId}`);
}

// Profile Image
export function createWorkerImage(profileId: string, formData: FormData): Promise<void> {
  return api.post(`/api/worker/profile/${profileId}/ProfileImage`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  });
}
