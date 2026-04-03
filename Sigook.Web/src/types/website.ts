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
