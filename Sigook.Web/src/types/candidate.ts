import { Skill } from './common';

export interface Candidate {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  mobileNumber: string;
  notes: string;
  skills: Skill[];
  documents: CandidateDocument[];
  phoneNumbers: string[];
  createdAt: string;
  recruiterId: string | null;
  recruiterName: string;
  status: string;
  lastRequest: string | null;
}

export interface CandidateDocument {
  id: string;
  candidateId: string;
  fileName: string;
  fileId: string;
}

export interface CandidateFilter {
  page: number;
  pageSize: number;
  searchTerm?: string;
  sortBy?: string;
  sortDesc?: boolean;
  recruiterId?: string | null;
  status?: string | null;
}
