import http from '@/security/apiService';
import type { PaginatedList } from '@/types/common';
import type { NoteModel, NoteItem, NotePagination, CreateNoteResponse } from '@/types/agency';

// ---------------------------------------------------------------------------
// Worker notes (read + create only)
// ---------------------------------------------------------------------------

export function getWorkerProfileNotes(userId: string, pagination: NotePagination): Promise<PaginatedList<NoteItem>> {
  return http
    .get(`/api/AgencyWorkerProfile/${userId}/Note?PageSize=${pagination.size}&PageIndex=${pagination.page}`)
    .then(r => r.data);
}

export function createWorkerProfileNote(userId: string, model: NoteModel): Promise<CreateNoteResponse> {
  return http.post(`/api/AgencyWorkerProfile/${userId}/Note`, model).then(r => r.data);
}

// ---------------------------------------------------------------------------
// Candidate notes (read, create, delete)
// ---------------------------------------------------------------------------

export function getCandidateNotes(userId: string, pagination: NotePagination): Promise<PaginatedList<NoteItem>> {
  return http
    .get(`/api/AgencyCandidate/${userId}/Note?PageSize=${pagination.size}&PageIndex=${pagination.page}`)
    .then(r => r.data);
}

export function createCandidateNote(userId: string, model: NoteModel): Promise<CreateNoteResponse> {
  return http.post(`/api/AgencyCandidate/${userId}/Note`, model).then(r => r.data);
}

export function deleteCandidateNote(userId: string, id: string): Promise<void> {
  return http.delete(`/api/AgencyCandidate/${userId}/Note/${id}`).then(() => {});
}

// ---------------------------------------------------------------------------
// Company notes (full CRUD)
// ---------------------------------------------------------------------------

export function getAgencyCompanyNotes(userId: string, pagination: NotePagination): Promise<PaginatedList<NoteItem>> {
  return http
    .get(`/api/AgencyCompanyProfile/${userId}/Note?PageSize=${pagination.size}&PageIndex=${pagination.page}`)
    .then(r => r.data);
}

export function createAgencyCompanyNote(userId: string, model: NoteModel): Promise<CreateNoteResponse> {
  return http.post(`/api/AgencyCompanyProfile/${userId}/Note`, model).then(r => r.data);
}

export function updateAgencyCompanyNote(userId: string, id: string, model: NoteModel): Promise<void> {
  return http.put(`/api/AgencyCompanyProfile/${userId}/Note/${id}`, model).then(() => {});
}

export function deleteAgencyCompanyNote(userId: string, id: string): Promise<void> {
  return http.delete(`/api/AgencyCompanyProfile/${userId}/Note/${id}`).then(() => {});
}

// ---------------------------------------------------------------------------
// Request notes (full CRUD)
// ---------------------------------------------------------------------------

export function getAgencyRequestNotes(userId: string, pagination: NotePagination): Promise<PaginatedList<NoteItem>> {
  return http
    .get(`/api/AgencyRequest/${userId}/Note?PageSize=${pagination.size}&PageIndex=${pagination.page}`)
    .then(r => r.data);
}

export function createAgencyRequestNote(userId: string, model: NoteModel): Promise<CreateNoteResponse> {
  return http.post(`/api/AgencyRequest/${userId}/Note`, model).then(r => r.data);
}

export function updateAgencyRequestNote(userId: string, id: string, model: NoteModel): Promise<void> {
  return http.put(`/api/AgencyRequest/${userId}/Note/${id}`, model).then(() => {});
}

export function deleteAgencyRequestNote(userId: string, id: string): Promise<void> {
  return http.delete(`/api/AgencyRequest/${userId}/Note/${id}`).then(() => {});
}

// ---------------------------------------------------------------------------
// Request worker notes (full CRUD)
// ---------------------------------------------------------------------------

export function getAgencyRequestWorkerNotes(
  requestId: string,
  userId: string,
  pagination: NotePagination,
): Promise<PaginatedList<NoteItem>> {
  return http
    .get(
      `/api/AgencyRequest/${requestId}/Worker/${userId}/Note?PageSize=${pagination.size}&PageIndex=${pagination.page}`,
    )
    .then(r => r.data);
}

export function createAgencyRequestWorkerNote(
  requestId: string,
  userId: string,
  model: NoteModel,
): Promise<CreateNoteResponse> {
  return http.post(`/api/AgencyRequest/${requestId}/Worker/${userId}/Note`, model).then(r => r.data);
}

export function updateAgencyRequestWorkerNote(
  requestId: string,
  userId: string,
  id: string,
  model: NoteModel,
): Promise<void> {
  return http.put(`/api/AgencyRequest/${requestId}/Worker/${userId}/Note/${id}`, model).then(() => {});
}

export function deleteAgencyRequestWorkerNote(requestId: string, userId: string, id: string): Promise<void> {
  return http.delete(`/api/AgencyRequest/${requestId}/Worker/${userId}/Note/${id}`).then(() => {});
}
