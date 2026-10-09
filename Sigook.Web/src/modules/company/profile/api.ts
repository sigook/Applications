import { api } from '@/app/security/apiService';
import type { CompanyProfileDetail, CompanyProfileLocationDetail, CompanyProfileJobPositionRate, CompanyUserModel, CreateCompanyUserModel } from '@/shared/company-profile/types';
import type { CompanyContactPersonModel } from '@/modules/company/profile/types';

// Profile
export function getCompanyProfile(): Promise<CompanyProfileDetail> {
  return api.get<CompanyProfileDetail>('/api/company/profile');
}

export function updateProfile(id: string, company: CompanyProfileDetail): Promise<void> {
  return api.put(`/api/company/profile/${id}`, company);
}

// Profile Locations
export function getProfileLocations(): Promise<CompanyProfileLocationDetail[]> {
  return api.get<CompanyProfileLocationDetail[]>('/api/company/profile/locations');
}

export function createProfileLocation(model: CompanyProfileLocationDetail): Promise<void> {
  return api.post('/api/company/profile/locations', model);
}

export function updateProfileLocation(id: string, model: CompanyProfileLocationDetail): Promise<void> {
  return api.put(`/api/company/profile/locations/${id}`, model);
}

export function deleteProfileLocation(id: string): Promise<void> {
  return api.del(`/api/company/profile/locations/${id}`);
}

// Job Positions
export function getCompanyJobPositions(): Promise<CompanyProfileJobPositionRate[]> {
  return api.get<CompanyProfileJobPositionRate[]>('/api/company/profile/jobpositions');
}

// Company Users
export function getCompanyUser(): Promise<CompanyUserModel[]> {
  return api.get<CompanyUserModel[]>('/api/company/profile/users');
}

export function getCompanyUserDetail(): Promise<CompanyUserModel> {
  return api.get<CompanyUserModel>('/api/company/profile/users/detail');
}

export function createCompanyUser(model: CreateCompanyUserModel): Promise<void> {
  return api.post('/api/company/profile/users', model);
}

export function updateCompanyUser(id: string, user: CompanyUserModel): Promise<void> {
  return api.put(`/api/company/profile/users/${id}`, user);
}

export function deleteCompanyUser(id: string): Promise<void> {
  return api.del(`/api/company/profile/users/${id}`);
}

// Contact People
export function getContactPeople(): Promise<CompanyContactPersonModel[]> {
  return api.get<CompanyContactPersonModel[]>('/api/company/profile/contactpeople');
}

export function saveContactPerson(model: CompanyContactPersonModel): Promise<void> {
  return api.post('/api/company/profile/contactpeople', model);
}

export function deleteContactPerson(id: string): Promise<void> {
  return api.del(`/api/company/profile/contactpeople/${id}`);
}
