import http from '@/security/apiService';
import type { PaginatedList } from '@/types/common';
import type {
  Candidate,
  CandidateDocument,
  AgencyCandidateFilter,
  CreateCandidateModel,
  CandidatePhoneNumberModel,
  CandidateSkillModel,
} from '@/types/candidate';

// ---------------------------------------------------------------------------
// Candidates CRUD
// ---------------------------------------------------------------------------

export function getAgencyCandidates(filter: AgencyCandidateFilter): Promise<PaginatedList<Candidate>> {
  return http.get('/api/AgencyCandidate', { params: { ...filter } }).then(r => r.data);
}

export function getAgencyCandidate(candidateId: string): Promise<Candidate> {
  return http.get(`/api/AgencyCandidate/${candidateId}`).then(r => r.data);
}

export function createAgencyCandidate(model: CreateCandidateModel): Promise<{ id: string }> {
  return http.post('/api/AgencyCandidate', model).then(r => r.data);
}

export function updateAgencyCandidate(candidateId: string, model: Partial<Candidate>): Promise<void> {
  return http.put(`/api/AgencyCandidate/${candidateId}`, model).then(() => {});
}

export function deleteAgencyCandidate(candidateId: string): Promise<void> {
  return http.delete(`/api/AgencyCandidate/${candidateId}`).then(() => {});
}

export function updateAgencyCandidateRecruiter(candidateId: string): Promise<void> {
  return http.put(`/api/AgencyCandidate/${candidateId}/Recruiter`, null).then(() => {});
}

export function convertCandidateToWorker(candidateId: string): Promise<{ id: string }> {
  return http.post(`/api/AgencyCandidate/${candidateId}/convert-to-worker`).then(r => r.data);
}

// ---------------------------------------------------------------------------
// Phone numbers
// ---------------------------------------------------------------------------

export function addCandidatePhoneNumber(candidateId: string, model: CandidatePhoneNumberModel): Promise<{ id: string }> {
  return http.post(`/api/AgencyCandidate/${candidateId}/PhoneNumber`, model).then(r => r.data);
}

export function deleteCandidatePhoneNumber(candidateId: string, numberId: string): Promise<void> {
  return http.delete(`/api/AgencyCandidate/${candidateId}/PhoneNumber/${numberId}`).then(() => {});
}

// ---------------------------------------------------------------------------
// Skills
// ---------------------------------------------------------------------------

export function addCandidateSkill(candidateId: string, model: CandidateSkillModel): Promise<{ id: string }> {
  return http.post(`/api/AgencyCandidate/${candidateId}/Skill`, model).then(r => r.data);
}

export function deleteCandidateSkill(candidateId: string, skillId: string): Promise<void> {
  return http.delete(`/api/AgencyCandidate/${candidateId}/Skill/${skillId}`).then(() => {});
}

// ---------------------------------------------------------------------------
// Documents
// ---------------------------------------------------------------------------

export function getCandidateDocuments(candidateId: string): Promise<CandidateDocument[]> {
  return http.get(`/api/AgencyCandidate/${candidateId}/Document`).then(r => r.data);
}

export function addCandidateDocument(candidateId: string, model: Record<string, unknown> | FormData): Promise<{ id: string }> {
  return http.post(`/api/AgencyCandidate/${candidateId}/Document`, model).then(r => r.data);
}

export function deleteCandidateDocument(candidateId: string, id: string): Promise<void> {
  return http.delete(`/api/AgencyCandidate/${candidateId}/Document/${id}`).then(() => {});
}

// ---------------------------------------------------------------------------
// Bulk upload
// ---------------------------------------------------------------------------

export function bulkAgencyCandidates(agencyId: string, file: File): Promise<Blob> {
  const formData = new FormData();
  formData.append('file', file);
  return http
    .post(`/api/AgencyCandidate/bulk/${agencyId}`, formData, {
      responseType: 'blob',
      headers: { 'Content-Type': 'multipart/form-data' },
    })
    .then(r => r.data);
}
