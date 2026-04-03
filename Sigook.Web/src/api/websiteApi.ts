import http from '@/security/apiService';
import axios from 'axios';

export interface JobSearchFilter {
  jobId?: string;
  jobTitle?: string;
  location?: string;
  countries?: string[];
}

export interface JobViewModel {
  id: string;
  requestId: string;
  numberId: string;
  title: string;
  salary: string;
  location: string;
  type: string;
  createdAt: string;
  description: string;
  requirements: string;
  responsibilities: string;
  shift: string;
  createdBy: string;
  agencyId: string | null;
}

export interface ContactForm {
  title: string;
  name: string;
  email: string;
  phone: string;
  message: string;
  subject: string;
  captchaResponse: string;
}

export function getJobs(filter: JobSearchFilter): Promise<JobViewModel[]> {
  return http.get('/api/WebSite/jobs', {
    params: { ...filter, countries: ['USA', 'CA'] }
  }).then(r => r.data);
}

export function getLandingJobPositions(): Promise<any[]> {
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
