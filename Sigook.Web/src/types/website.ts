import type { ResidencyStatus } from './common';

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

// Values bound to the public candidate application form (landing).
export interface CandidateFormData {
  fullName: string;
  email: string;
  phone: string;
  countryId: string;
  address: string;
  status: ResidencyStatus | '' | null | undefined;
  sourceId: string | null;
  skills: string[];
  hasVehicle: boolean;
  resume: File | null;
  termsAccepted: boolean;
}

// JSON part sent in the multipart payload of POST /api/WebSite/candidate.
export interface CandidateApplyPayload {
  fullName: string;
  email: string;
  phone: string;
  skills: string[];
  status: string;
  countryId: string;
  address: string;
  fileName: string | null;
  hasVehicle: boolean;
  sourceId: string | null;
  requestId?: string;
}

// Job context carried into the candidate apply modal when opened from a posting.
export interface CandidateApplyModalContext {
  readonly jobTitle?: string;
  readonly jobNumber?: string;
  readonly requestId?: string;
}

export interface ContactForm {
  title: string;
  name: string;
  email: string;
  phone: string;
  company?: string;
  location?: string;
  message: string;
  subject: string;
  captchaResponse: string;
}
