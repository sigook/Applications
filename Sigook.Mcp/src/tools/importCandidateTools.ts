import { existsSync } from 'node:fs';
import type { McpServer } from '@modelcontextprotocol/sdk/server/mcp.js';
import { z } from 'zod';
import { blankToUndefined, isBlank, tryNormalizePhone } from '../api/lookupRules.js';
import type { CityModel } from '../models/apiModels.js';
import { runGrouped } from './batchRunner.js';
import { generatedFileName, toSkills } from './candidateTools.js';
import { jsonResult, type Services } from './services.js';

const MAX_BATCH = 25;
const BATCH_CONCURRENCY = 4;

const candidateRowShape = {
  name: z.string().describe('Full name.'),
  phone: z.string().describe('Phone as it comes from the export, any North American format.'),
  email: z.string().nullish().describe('Email. Optional.'),
  city: z.string().nullish().describe('City name. Optional; needs provinceOrState.'),
  provinceOrState: z.string().nullish().describe('Province or state code or name, e.g. PA, ON, Ontario.'),
  countryCode: z.string().nullish().default('USA').describe('CA or USA. Default USA.'),
  sourceName: z.string().nullish().describe("Source name as listed by list_sources, e.g. 'Zip Recruiter'. Unknown names fall back to Other."),
  residencyStatus: z.string().nullish().describe("Residency/work status free text, e.g. 'Authorized to work in the U.S.'. Optional."),
  hasVehicle: z.boolean().nullish().default(false).describe('Whether the candidate has a vehicle. Default false.'),
  address: z.string().nullish().describe('Street address. Optional.'),
  postalCode: z.string().nullish().describe('Postal/zip code. Optional.'),
  skills: z.array(z.string()).nullish().describe('Skills to attach. Optional.'),
  resumePath: z.string().nullish().describe('Local path of the resume (pdf, doc, docx) as returned by read_export. Optional.'),
  requestNumberId: z.number().int().nullish().describe('Sigook request number to apply the candidate to. Optional.'),
  applyComments: z.string().nullish().describe('Comments stored on the applicant, e.g. screening answers. Optional.'),
  note: z.string().nullish().describe('Note added to the candidate, e.g. apply date, status and labels on the job board. Optional.')
};

const candidateRow = z.object(candidateRowShape);
type CandidateRow = z.infer<typeof candidateRow>;
interface ImportOutcome {
  success: boolean;
  candidateId?: string;
  created?: boolean;
  [key: string]: unknown;
}

const fail = (step: string, error: string): ImportOutcome => ({ success: false, step, error });

const errorMessage = (error: unknown) => error instanceof Error ? error.message : String(error);

async function importCandidate(input: CandidateRow, { api, catalog }: Services, signal: AbortSignal): Promise<ImportOutcome> {
  const warnings: string[] = [];
  const phone = tryNormalizePhone(input.phone);
  if (!phone) return fail('phone', `Phone '${input.phone}' does not have 10 digits.`);
  const hasResume = !isBlank(input.resumePath);
  if (hasResume && !existsSync(input.resumePath!)) return fail('resume', `Resume not found: ${input.resumePath}`);

  let city: CityModel | undefined;
  if (!isBlank(input.city)) {
    if (isBlank(input.provinceOrState)) return fail('city', 'provinceOrState is required when city is given.');
    const resolution = await catalog.resolveCity(input.city!, input.provinceOrState!, input.countryCode ?? 'USA');
    if (!resolution.city)
      return { success: false, step: 'city', error: resolution.error, alternatives: resolution.alternatives, provinces: resolution.provinces };
    city = resolution.city;
  }

  const { requestNumberId } = input;
  const hasRequestNumber = requestNumberId !== null && requestNumberId !== undefined;
  const request = hasRequestNumber ? await catalog.request(requestNumberId) : undefined;
  if (hasRequestNumber && !request) warnings.push(`Request ${requestNumberId} not found; candidate not applied.`);

  const existing = await api.getCandidates(null, phone, 5, signal);
  const duplicate = existing.items.find(candidate => candidate.phoneNumbers?.some(item => item.phoneNumber === phone)) ?? existing.items[0];

  let candidateId: string;
  let created = false;
  let resumeUploaded = false;
  if (duplicate) {
    candidateId = duplicate.id;
    warnings.push(`Phone ${phone} already belongs to '${duplicate.name}' (${duplicate.id}); candidate not created.`);
  } else {
    const sourceId = await catalog.sourceId(input.sourceName);
    if (!sourceId) warnings.push(`Source '${input.sourceName}' not found and no 'Other' source; left empty.`);
    const fileName = hasResume ? generatedFileName('Resume', input.resumePath!) : undefined;
    const create = await api.createCandidate({
      name: input.name.trim(),
      email: blankToUndefined(input.email)?.toLowerCase(),
      address: blankToUndefined(input.address),
      postalCode: blankToUndefined(input.postalCode),
      city,
      hasVehicle: input.hasVehicle ?? false,
      residencyStatus: blankToUndefined(input.residencyStatus),
      sourceId,
      dnu: false,
      phoneNumbers: [{ phoneNumber: phone }],
      skills: toSkills(input.skills),
      fileName
    }, fileName ? { path: input.resumePath!, fileName } : undefined, signal);
    if (!create.success)
      return { success: false, step: 'create', statusCode: create.statusCode, error: create.error, warnings };
    candidateId = create.value.id;
    created = true;
    resumeUploaded = fileName !== undefined;
  }

  let applied: boolean | undefined;
  let applyError: string | undefined;
  if (request) {
    const apply = await api.addApplicant(request.id, { candidateId, comments: blankToUndefined(input.applyComments) }, signal);
    const alreadyApplicant = !apply.success && apply.error.toLowerCase().includes('already in the request');
    applied = apply.success || alreadyApplicant;
    if (alreadyApplicant) warnings.push(`Candidate was already an applicant of request ${request.numberId}.`);
    else if (!apply.success) applyError = `(${apply.statusCode}) ${apply.error}`;
  }

  let noteAdded: boolean | undefined;
  let noteError: string | undefined;
  if (!isBlank(input.note)) {
    const added = await api.addCandidateNote(candidateId, { note: input.note!.trim() }, signal);
    noteAdded = added.success;
    if (!added.success) noteError = `(${added.statusCode}) ${added.error}`;
  }

  return {
    success: applied !== false && noteAdded !== false,
    candidateId,
    created,
    duplicate: duplicate !== undefined,
    phone,
    resumeUploaded,
    requestId: request?.id,
    requestNumberId: request?.numberId,
    applied,
    applyError,
    noteAdded,
    noteError,
    warnings
  };
}

function outcomeKind(outcome: ImportOutcome): 'created' | 'existed' | 'failed' {
  if (outcome.candidateId === undefined) return 'failed';
  return outcome.created ? 'created' : 'existed';
}

export function registerImportCandidateTools(server: McpServer, services: Services): void {
  server.registerTool('import_candidate', {
    description: 'Imports one candidate in a single call: normalizes the phone, checks duplicates by phone, resolves the city, looks up the source and the request by number, creates the candidate with the resume, applies it to the request and adds a note. Nothing is written when the phone is invalid, the city is unknown or the resume path does not exist. When the phone already belongs to a candidate it is not created again; the request application and the note go to the existing one. The result reports each step (created, applied, noteAdded) so you retry only the failed step with apply_candidate_to_request or add_candidate_note; never call import_candidate or create_candidate again once a candidateId was returned. Prefer import_candidates for more than one row.',
    inputSchema: candidateRowShape
  }, async (input, { signal }) => jsonResult(await importCandidate(input, services, signal)));

  server.registerTool('import_candidates', {
    description: `Imports up to ${MAX_BATCH} candidates in one call. Each row behaves exactly like import_candidate and gets its own result, in the same order, with its zero-based index as 'row'. Rows run ${BATCH_CONCURRENCY} at a time; rows with the same phone run one after another, so the second one finds the candidate created by the first. One failing row never stops the others. Retry only the failed rows or steps: rows with a candidateId must never be imported again.`,
    inputSchema: {
      rows: z.array(candidateRow).min(1).max(MAX_BATCH).describe(`Candidates to import, at most ${MAX_BATCH}.`)
    }
  }, async ({ rows }, { signal }) => {
    const results = await runGrouped(
      rows,
      (row, index) => tryNormalizePhone(row.phone) ?? `invalid:${index}`,
      BATCH_CONCURRENCY,
      async (row, index) => {
        const outcome = await importCandidate(row, services, signal).catch((error: unknown) => fail('error', errorMessage(error)));
        return { row: index, name: row.name, ...outcome };
      });
    const kinds = results.map(outcomeKind);
    return jsonResult({
      total: rows.length,
      created: kinds.filter(kind => kind === 'created').length,
      existed: kinds.filter(kind => kind === 'existed').length,
      failed: kinds.filter(kind => kind === 'failed').length,
      incomplete: results.filter(result => result.success === false && result.candidateId !== undefined).length,
      results
    });
  });
}
