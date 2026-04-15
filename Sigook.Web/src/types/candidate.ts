import { Skill } from './common';

// Candidate item returned by GET /api/AgencyCandidate.
// Mirrors backend CandidateListModel (Covenant.Common.Models.Candidate).
export interface Candidate {
  id: string;
  agencyId: string;
  name: string;
  email?: string;
  address?: string;
  postalCode?: string;
  residencyStatus?: string;
  phoneNumbers: CandidatePhoneNumberModel[];
  skills: Skill[];
  requests?: { id: string; value: string }[];
  recruiter?: string;
  createdAt: string;
  hasVehicle: boolean;
  hasDocuments: boolean;
  notesCount: number;
  dnu: boolean;
  source?: string;
}

// Document attached to a candidate. Mirrors backend CovenantFileModel.
export interface CandidateDocument {
  id: string;
  pathFile?: string;
  fileName: string;
  description?: string;
  canDownload: boolean;
}

// Payload for POST /api/AgencyCandidate/{id}/Document. The id/pathFile/canDownload
// fields are populated server-side after upload, so only fileName/description are sent.
export interface CreateCandidateDocumentPayload {
  fileName: string;
  description?: string;
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

// Filter for GET /api/AgencyCandidate. Mirrors backend GetCandidatesFilter.
export interface AgencyCandidateFilter {
  pageIndex?: number;
  pageSize?: number;
  isDescending?: boolean;
  sortBy?: number;
  name?: string;
  phone?: string;
  address?: string;
  skills?: string;
  numberId?: number | string;
  createdAtFrom?: string | null;
  createdAtTo?: string | null;
  recruiter?: string;
  statuses?: string[];
  source?: string;
  resumeOnly?: boolean;
}

// Body for POST/PUT /api/AgencyCandidate. Mirrors backend CandidateCreateModel.
export interface CreateCandidateModel {
  name: string;
  email?: string;
  address?: string;
  postalCode?: string;
  gender?: { id: string; value?: string } | null;
  hasVehicle: boolean;
  residencyStatus?: string;
  source?: string;
  dnu?: boolean;
  phoneNumbers?: CandidatePhoneNumberModel[];
  skills?: CandidateSkillModel[];
  fileName?: string;
}

// Phone number payload for POST /api/AgencyCandidate/{id}/PhoneNumber.
// Mirrors backend PhoneNumberModel.
export interface CandidatePhoneNumberModel {
  id?: string;
  phoneNumber: string;
}

// Skill payload for POST /api/AgencyCandidate/{id}/Skill.
// Mirrors backend SkillModel.
export interface CandidateSkillModel {
  id?: string;
  skill: string;
}
