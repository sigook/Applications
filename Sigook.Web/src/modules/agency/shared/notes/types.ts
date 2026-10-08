export interface NoteModel {
  id?: string;
  note: string;
  color?: string;
}

// Note item returned by any GET /api/.../Note endpoint.
// Mirrors backend NoteModel (Covenant.Common.Models.NoteModel).
export interface NoteItem {
  id: string;
  note: string;
  color?: string;
  createdAt?: string;
  createdBy?: string;
}

export interface NotePagination {
  page: number;
  size: number;
}

// Response returned by POST /api/.../Note. Same shape as NoteModel / NoteItem.
export type CreateNoteResponse = NoteItem;

export interface NoteFormModel extends NoteModel {
  createdAt?: string;
  createdBy?: string;
}

export interface NotesFetchPayload {
  userId: string;
  pagination: NotePagination;
}

export interface NotesCreatePayload {
  userId: string;
  model: NoteModel;
}

export interface NotesUpdatePayload {
  userId: string;
  id: string;
  model: NoteModel;
}

export interface NotesDeletePayload {
  userId: string;
  id: string;
}

export interface RequestNotesFetchPayload {
  requestId: string;
  userId: string;
  pagination: NotePagination;
}

export interface RequestNotesCreatePayload {
  requestId: string;
  userId: string;
  model: NoteModel;
}

export interface RequestNotesUpdatePayload {
  requestId: string;
  userId: string;
  id: string;
  model: NoteModel;
}

export interface RequestNotesDeletePayload {
  requestId: string;
  userId: string;
  id: string;
}
