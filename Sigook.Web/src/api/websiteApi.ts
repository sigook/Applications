import { api } from '@/security/apiService';
import { buildMultipartFormData } from '@/utils/multipart';
import { generateFileName } from '@/utils/fileNaming';
import type {
  JobSearchFilter,
  JobViewModel,
  ContactForm,
  CandidateFormData,
  CandidateApplyPayload,
} from '@/types/website';

export function getJobs(filter: JobSearchFilter = {}): Promise<JobViewModel[]> {
  return api.get<JobViewModel[]>('/api/WebSite/jobs', {
    params: { ...filter, countries: filter.countries ?? ['USA', 'CA'] }
  });
}

export function submitContactForm(contact: ContactForm): Promise<void> {
  return api.post<void>('/api/WebSite/contact', contact);
}

export function submitCandidate(formData: FormData): Promise<void> {
  return api.post<void>('/api/WebSite/candidate', formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  });
}

export function submitCandidateApplication(data: CandidateFormData, requestId?: string): Promise<void> {
  const fileName = data.resume ? generateFileName('Resume', data.resume.name) : null;

  const model: CandidateApplyPayload = {
    fullName: data.fullName,
    email: data.email,
    phone: data.phone,
    skills: data.skills,
    status: data.status || '',
    countryId: data.countryId,
    address: data.address,
    fileName,
    hasVehicle: data.hasVehicle,
    sourceId: data.sourceId || null,
    requestId,
  };

  return submitCandidate(buildMultipartFormData(model, fileName ? { [fileName]: data.resume } : {}));
}
