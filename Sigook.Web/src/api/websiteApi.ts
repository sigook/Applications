import { api } from '@/security/apiService';
import type { JobSearchFilter, JobViewModel, ContactForm } from '@/types/website';

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

export function candidateRequestApply(candidateId: string, requestId: string): Promise<void> {
  return api.post<void>(`/api/WebSite/candidate/${candidateId}/${requestId}/apply`);
}
