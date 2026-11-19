import { existsSync } from 'node:fs';
import type { McpServer } from '@modelcontextprotocol/sdk/server/mcp.js';
import { z } from 'zod';
import { isBlank } from '../api/lookupRules.js';
import type { WorkerDocumentSection } from '../api/sigookApi.js';
import type {
  WorkerDocumentsDetail,
  WorkerFileModel,
  WorkerIdentificationModel,
  WorkerLicenseModel,
  WorkerSinModel
} from '../models/apiModels.js';
import { generatedFileName } from './candidateTools.js';
import { jsonResult, type Services } from './services.js';

export const WORKER_DOCUMENT_KINDS = ['identification', 'sin', 'license', 'certificate', 'other', 'resume'] as const;

export type WorkerDocumentKind = typeof WORKER_DOCUMENT_KINDS[number];

export interface WorkerDocumentInput {
  kind: WorkerDocumentKind;
  replace: boolean;
  identificationTypeId?: string | null;
  slot?: number | null;
  number?: string | null;
  issued?: string | null;
  expires?: string | null;
  sinExpires?: boolean | null;
  dueDate?: string | null;
}

export type WorkerDocumentPayload =
  | { ok: true; section: WorkerDocumentSection; model: unknown; replaced?: WorkerFileModel }
  | { ok: false; error: string };

const MAX_DESCRIPTION_LENGTH = 20;

const KIND_SETTINGS: Record<WorkerDocumentKind, { prefix: string; description: string }> = {
  identification: { prefix: 'Document', description: 'Identification' },
  sin: { prefix: 'SIN-SSN', description: 'SIN' },
  license: { prefix: 'License', description: 'License' },
  certificate: { prefix: 'Certificate', description: 'Certificate' },
  other: { prefix: 'OtherDoc', description: 'Document' },
  resume: { prefix: 'Resume', description: 'Resume' }
};

export const workerDocumentPrefix = (kind: WorkerDocumentKind): string => KIND_SETTINGS[kind].prefix;

export function workerDocumentDescription(kind: WorkerDocumentKind, description: string | null | undefined): string {
  const cleaned = (description ?? '')
    .replace(/[^-_ a-zA-Z0-9]/g, ' ')
    .replace(/\s+/g, ' ')
    .trim()
    .slice(0, MAX_DESCRIPTION_LENGTH)
    .trim();
  return cleaned || KIND_SETTINGS[kind].description;
}

const hasFile = (file: WorkerFileModel | null | undefined): file is WorkerFileModel => !isBlank(file?.fileName);

const keepFile = (file: WorkerFileModel | null | undefined): WorkerFileModel | null =>
  hasFile(file) ? { fileName: file.fileName, description: file.description } : null;

const occupied = (label: string): WorkerDocumentPayload =>
  ({ ok: false, error: `${label} already has a file. Pass replace=true to overwrite it.` });

function identificationPayload(detail: WorkerDocumentsDetail, input: WorkerDocumentInput, file: WorkerFileModel): WorkerDocumentPayload {
  const slots = [
    { number: detail.identificationNumber1, type: detail.identificationType1, file: detail.identificationType1File },
    { number: detail.identificationNumber2, type: detail.identificationType2, file: detail.identificationType2File }
  ];
  const slot = input.slot ?? (hasFile(slots[0]!.file) ? (hasFile(slots[1]!.file) ? undefined : 2) : 1);
  if (slot === undefined) return { ok: false, error: 'Both identification slots already have a file. Pass slot (1 or 2) and replace=true to overwrite one.' };
  if (slot !== 1 && slot !== 2) return { ok: false, error: 'slot must be 1 or 2.' };
  const current = slots[slot - 1]!;
  if (hasFile(current.file) && !input.replace) return occupied(`Identification slot ${slot}`);
  const typeId = input.identificationTypeId ?? current.type?.id;
  if (isBlank(typeId)) return { ok: false, error: 'identificationTypeId is required. Use list_identification_types.' };
  const updated = slots.map((item, index) => index === slot - 1
    ? { number: input.number ?? item.number, typeId, file }
    : { number: item.number, typeId: item.type?.id, file: keepFile(item.file) });
  const model: WorkerIdentificationModel = {
    identificationNumber1: updated[0]!.number,
    identificationType1: updated[0]!.typeId ? { id: updated[0]!.typeId } : null,
    identificationType1File: updated[0]!.file,
    identificationNumber2: updated[1]!.number,
    identificationType2: updated[1]!.typeId ? { id: updated[1]!.typeId } : null,
    identificationType2File: updated[1]!.file,
    havePoliceCheckBackground: detail.havePoliceCheckBackground,
    policeCheckBackGround: keepFile(detail.policeCheckBackGround),
    resume: keepFile(detail.resume)
  };
  return { ok: true, section: 'Documents', model, replaced: keepFile(current.file) ?? undefined };
}

function sinPayload(detail: WorkerDocumentsDetail, input: WorkerDocumentInput, file: WorkerFileModel): WorkerDocumentPayload {
  if (hasFile(detail.socialInsuranceFile) && !input.replace) return occupied('SIN');
  const socialInsurance = input.number ?? detail.socialInsurance;
  if (isBlank(socialInsurance)) return { ok: false, error: 'The worker has no SIN number. Pass number.' };
  const socialInsuranceExpire = input.sinExpires ?? detail.socialInsuranceExpire;
  const dueDate = input.dueDate ?? detail.dueDate;
  if (socialInsuranceExpire && isBlank(dueDate)) return { ok: false, error: 'dueDate is required when the SIN expires.' };
  const model: WorkerSinModel = { socialInsurance: socialInsurance!, socialInsuranceExpire, dueDate, socialInsuranceFile: file };
  return { ok: true, section: 'SinInformation', model, replaced: keepFile(detail.socialInsuranceFile) ?? undefined };
}

function licensePayload(detail: WorkerDocumentsDetail, input: WorkerDocumentInput, file: WorkerFileModel): WorkerDocumentPayload {
  const existing: WorkerLicenseModel[] = (detail.licenses ?? [])
    .filter(item => hasFile(item.license))
    .map(item => ({ license: keepFile(item.license)!, number: item.number, issued: item.issued, expires: item.expires }));
  return { ok: true, section: 'Licenses', model: [...existing, { license: file, number: input.number, issued: input.issued, expires: input.expires }] };
}

function certificatePayload(detail: WorkerDocumentsDetail, file: WorkerFileModel): WorkerDocumentPayload {
  const existing = (detail.certificates ?? []).map(keepFile).filter(item => item !== null);
  return { ok: true, section: 'Certificates', model: [...existing, file] };
}

function resumePayload(detail: WorkerDocumentsDetail, input: WorkerDocumentInput, file: WorkerFileModel): WorkerDocumentPayload {
  if (hasFile(detail.resume) && !input.replace) return occupied('Resume');
  return { ok: true, section: 'Resume', model: file, replaced: keepFile(detail.resume) ?? undefined };
}

export function buildWorkerDocumentPayload(detail: WorkerDocumentsDetail, input: WorkerDocumentInput, file: WorkerFileModel): WorkerDocumentPayload {
  switch (input.kind) {
    case 'identification': return identificationPayload(detail, input, file);
    case 'sin': return sinPayload(detail, input, file);
    case 'license': return licensePayload(detail, input, file);
    case 'certificate': return certificatePayload(detail, file);
    case 'other': return { ok: true, section: 'OtherDocument', model: file };
    case 'resume': return resumePayload(detail, input, file);
  }
}

export function registerWorkerDocumentTools(server: McpServer, { api }: Services): void {
  server.registerTool('add_worker_document', {
    description: 'Uploads a document to an existing worker (not a candidate). kind picks the section: identification (2 slots, needs identificationTypeId from list_identification_types; a SIN/SSN type also fills the SIN), sin, license, certificate, other, resume. Existing documents of the worker are preserved. identification, sin and resume refuse to overwrite an existing file unless replace=true. Use find_workers to get workerProfileId.',
    inputSchema: {
      workerProfileId: z.guid().describe('workerProfileId from find_workers.'),
      kind: z.enum(WORKER_DOCUMENT_KINDS).describe('Document section: identification, sin, license, certificate, other or resume.'),
      filePath: z.string().describe('Local file path (pdf, doc, docx, png, jpg, gif, xls, xlsx).'),
      description: z.string().nullish().describe('Short label (letters, digits, spaces, - and _; max 20 chars), e.g. WHMIS, Forklift. Defaults by kind.'),
      replace: z.boolean().nullish().default(false).describe('Overwrite an existing identification slot, SIN file or resume. Default false.'),
      identificationTypeId: z.guid().nullish().describe('identification only: id from list_identification_types. Optional when the slot already has a type.'),
      slot: z.number().int().min(1).max(2).nullish().describe('identification only: slot 1 or 2. Default: first empty slot.'),
      number: z.string().nullish().describe('identification, sin or license: document number. Keeps the current number when omitted.'),
      issued: z.iso.date().nullish().describe('license only: issue date YYYY-MM-DD.'),
      expires: z.iso.date().nullish().describe('license only: expiry date YYYY-MM-DD.'),
      sinExpires: z.boolean().nullish().describe('sin only: whether the SIN expires (temporary SIN). Keeps the current value when omitted.'),
      dueDate: z.iso.date().nullish().describe('sin only: SIN expiry date YYYY-MM-DD, required when sinExpires is true.')
    }
  }, async (input, { signal }) => {
    const { workerProfileId, kind, filePath } = input;
    if (!existsSync(filePath)) return jsonResult({ success: false, error: `File not found: ${filePath}` });
    const detail = await api.getWorkerDetail(workerProfileId, signal);
    if (!detail.success) {
      return jsonResult({ success: false, statusCode: detail.statusCode, error: detail.statusCode === 404 ? `Worker ${workerProfileId} not found.` : detail.error });
    }
    const file: WorkerFileModel = {
      fileName: generatedFileName(workerDocumentPrefix(kind), filePath),
      description: workerDocumentDescription(kind, input.description)
    };
    const payload = buildWorkerDocumentPayload(detail.value, { ...input, replace: input.replace ?? false }, file);
    if (!payload.ok) return jsonResult({ success: false, error: payload.error });
    const result = await api.postWorkerDocumentSection(workerProfileId, payload.section, payload.model, { path: filePath, fileName: file.fileName }, signal);
    return jsonResult(result.success
      ? { success: true, kind, workerProfileId, fileName: file.fileName, description: file.description, replaced: payload.replaced }
      : { success: false, statusCode: result.statusCode, error: result.error });
  });
}
