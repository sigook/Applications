import http from '@/security/apiService';
import axios from 'axios';
import type { JobSearchFilter, JobViewModel, ContactForm, LandingJobPositions } from '@/types/website';

export function getJobs(filter: JobSearchFilter = {}): Promise<JobViewModel[]> {
  return http.get('/api/WebSite/jobs', {
    params: { ...filter, countries: filter.countries ?? ['USA'] }
  }).then(r => r.data);
}

export function getLandingJobPositions(): Promise<LandingJobPositions> {
  return axios.get('/data/job-positions.json').then(r => r.data);
}

export function submitContactForm(contact: ContactForm): Promise<void> {
  return http.post('/api/WebSite/contact', contact).then(r => r.data);
}

export function submitCandidate(formData: FormData): Promise<void> {
  return http.post('/api/WebSite/candidate', formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  }).then(r => r.data);
}
