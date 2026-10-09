import { api } from '@/app/security/apiService';
import { buildMultipartFormData } from '@/shared/utils/multipart';
import type { PaginatedList } from '@/shared/types/common';
import type {
  Candidate,
  CandidateDetail,
  CandidateDocument,
  CreateCandidateDocumentPayload,
  AgencyCandidateFilter,
  CreateCandidateModel,
  CandidatePhoneNumberModel,
  CandidateSkillModel,
} from '@/modules/agency/recruiting/candidates/types';

// ---------------------------------------------------------------------------
// Candidates CRUD
// ---------------------------------------------------------------------------

export function getAgencyCandidates(filter: AgencyCandidateFilter): Promise<PaginatedList<Candidate>> {
  return api.get<PaginatedList<Candidate>>('/api/agency/recruiting/candidates', { params: { ...filter } });
}

export function getAgencyCandidate(candidateId: string): Promise<CandidateDetail> {
  return api.get<CandidateDetail>(`/api/agency/recruiting/candidates/${candidateId}`);
}

export function createAgencyCandidate(model: CreateCandidateModel, resume?: File | null): Promise<{ id: string }> {
  return api.post<{ id: string }>(
    '/api/agency/recruiting/candidates',
    buildMultipartFormData(model, model.fileName ? { [model.fileName]: resume } : {}),
    { headers: { 'Content-Type': 'multipart/form-data' } },
  );
}

export function updateAgencyCandidate(candidateId: string, model: CreateCandidateModel): Promise<void> {
  return api.put(`/api/agency/recruiting/candidates/${candidateId}`, model);
}

export function deleteAgencyCandidate(candidateId: string): Promise<void> {
  return api.del(`/api/agency/recruiting/candidates/${candidateId}`);
}

export function updateAgencyCandidateRecruiter(candidateId: string): Promise<void> {
  return api.put(`/api/agency/recruiting/candidates/${candidateId}/Recruiter`, null);
}

export function convertCandidateToWorker(candidateId: string): Promise<{ id: string }> {
  return api.post<{ id: string }>(`/api/agency/recruiting/candidates/${candidateId}/convert-to-worker`);
}

// ---------------------------------------------------------------------------
// Phone numbers
// ---------------------------------------------------------------------------

export function addCandidatePhoneNumber(candidateId: string, model: CandidatePhoneNumberModel): Promise<{ id: string }> {
  return api.post<{ id: string }>(`/api/agency/recruiting/candidates/${candidateId}/PhoneNumbers`, model);
}

export function deleteCandidatePhoneNumber(candidateId: string, numberId: string): Promise<void> {
  return api.del(`/api/agency/recruiting/candidates/${candidateId}/PhoneNumbers/${numberId}`);
}

// ---------------------------------------------------------------------------
// Skills
// ---------------------------------------------------------------------------

export function addCandidateSkill(candidateId: string, model: CandidateSkillModel): Promise<{ id: string }> {
  return api.post<{ id: string }>(`/api/agency/recruiting/candidates/${candidateId}/Skills`, model);
}

export function deleteCandidateSkill(candidateId: string, skillId: string): Promise<void> {
  return api.del(`/api/agency/recruiting/candidates/${candidateId}/Skills/${skillId}`);
}

// ---------------------------------------------------------------------------
// Documents
// ---------------------------------------------------------------------------

export function getCandidateDocuments(candidateId: string): Promise<PaginatedList<CandidateDocument>> {
  return api.get<PaginatedList<CandidateDocument>>(`/api/agency/recruiting/candidates/${candidateId}/Documents`);
}

export function addCandidateDocument(candidateId: string, model: CreateCandidateDocumentPayload, file: File): Promise<string> {
  return api.post<string>(
    `/api/agency/recruiting/candidates/${candidateId}/Documents`,
    buildMultipartFormData(model, { [model.fileName]: file }),
    { headers: { 'Content-Type': 'multipart/form-data' } },
  );
}

export function deleteCandidateDocument(candidateId: string, id: string): Promise<void> {
  return api.del(`/api/agency/recruiting/candidates/${candidateId}/Documents/${id}`);
}

// ---------------------------------------------------------------------------
// Bulk upload
// ---------------------------------------------------------------------------

export function bulkAgencyCandidates(agencyId: string, file: File): Promise<Blob> {
  const formData = new FormData();
  formData.append('file', file);
  return api.post<Blob>(`/api/agency/recruiting/candidates/bulk/${agencyId}`, formData, {
    responseType: 'blob',
    headers: { 'Content-Type': 'multipart/form-data' },
  });
}
