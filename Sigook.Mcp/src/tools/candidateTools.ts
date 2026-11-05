import { randomUUID } from 'node:crypto';
import { existsSync } from 'node:fs';
import { extname } from 'node:path';
import type { McpServer } from '@modelcontextprotocol/sdk/server/mcp.js';
import { z } from 'zod';
import { blankToUndefined, isBlank } from '../api/lookupRules.js';
import { jsonResult, type Services } from './services.js';

export const generatedFileName = (prefix: string, path: string): string =>
  `${prefix}_${randomUUID().replaceAll('-', '')}${extname(path)}`;

export const toSkills = (skills: string[] | null | undefined) =>
  (skills ?? []).filter(skill => !isBlank(skill)).map(skill => ({ skill: skill.trim() }));

export function registerCandidateTools(server: McpServer, { api }: Services): void {
  server.registerTool('create_candidate', {
    description: 'Creates a candidate in Sigook for the current agency, optionally uploading a resume file in the same call. Phone must already be in ### ###-#### format (see normalize_phone). The API rejects duplicated emails and phones; the error text is returned as-is so you can report it. Returns the new candidate id.',
    inputSchema: {
      name: z.string().describe('Full name.'),
      phone: z.string().describe('Phone in ### ###-#### format.'),
      email: z.string().nullish().describe('Email. Optional but strongly recommended.'),
      address: z.string().nullish().describe('Street address. Optional.'),
      postalCode: z.string().nullish().describe('Postal/zip code. Optional.'),
      cityId: z.guid().nullish().describe('Sigook city id from resolve_city. Optional.'),
      cityName: z.string().nullish().describe('City name matching cityId. Optional.'),
      sourceId: z.guid().nullish().describe('Sigook source id from list_sources. Optional.'),
      genderId: z.guid().nullish().describe('Sigook gender id from list_genders. Optional.'),
      hasVehicle: z.boolean().nullish().default(false).describe('Whether the candidate has a vehicle. Default false.'),
      residencyStatus: z.string().nullish().describe("Residency/work status free text, e.g. 'Authorized to work in the U.S.'. Optional."),
      skills: z.array(z.string()).nullish().describe('Skills to attach. Optional.'),
      resumePath: z.string().nullish().describe('Local path of the resume (pdf, doc, docx) as returned by read_export. Optional.')
    }
  }, async (input, { signal }) => {
    const { resumePath } = input;
    const hasResume = !isBlank(resumePath);
    if (hasResume && !existsSync(resumePath!)) return jsonResult({ success: false, error: `Resume not found: ${resumePath}` });
    const fileName = hasResume ? generatedFileName('Resume', resumePath!) : undefined;
    const result = await api.createCandidate({
      name: input.name.trim(),
      email: blankToUndefined(input.email),
      address: blankToUndefined(input.address),
      postalCode: blankToUndefined(input.postalCode),
      city: input.cityId ? { id: input.cityId, value: input.cityName ?? '' } : undefined,
      gender: input.genderId ? { id: input.genderId, value: '' } : undefined,
      hasVehicle: input.hasVehicle ?? false,
      residencyStatus: blankToUndefined(input.residencyStatus),
      sourceId: input.sourceId,
      dnu: false,
      phoneNumbers: [{ phoneNumber: input.phone.trim() }],
      skills: toSkills(input.skills),
      fileName
    }, fileName ? { path: resumePath!, fileName } : undefined, signal);
    return jsonResult(result.success
      ? { success: true, candidateId: result.value.id, resumeUploaded: fileName !== undefined }
      : { success: false, statusCode: result.statusCode, error: result.error });
  });

  server.registerTool('add_candidate_document', {
    description: 'Uploads an extra document (resume, certificate...) to an existing candidate.',
    inputSchema: {
      candidateId: z.guid().describe('Candidate id.'),
      filePath: z.string().describe('Local file path (pdf, doc, docx, png, jpg).'),
      description: z.string().nullish().default('Resume').describe('Short description, e.g. Resume. Default Resume.')
    }
  }, async ({ candidateId, filePath, description }, { signal }) => {
    if (!existsSync(filePath)) return jsonResult({ success: false, error: `File not found: ${filePath}` });
    const label = description ?? 'Resume';
    const fileName = generatedFileName(label.replaceAll(' ', '_'), filePath);
    const result = await api.addCandidateDocument(candidateId, { fileName, description: label }, { path: filePath, fileName }, signal);
    return jsonResult(result.success
      ? { success: true, documentId: result.value }
      : { success: false, statusCode: result.statusCode, error: result.error });
  });

  server.registerTool('add_candidate_note', {
    description: 'Adds a note to a candidate. Use it for information that has no field in Sigook, like screening answers or the apply date on the job board.',
    inputSchema: {
      candidateId: z.guid().describe('Candidate id.'),
      note: z.string().describe('Note text.')
    }
  }, async ({ candidateId, note }, { signal }) => {
    const result = await api.addCandidateNote(candidateId, { note }, signal);
    return jsonResult(result.success
      ? { success: true, noteId: result.value.id }
      : { success: false, statusCode: result.statusCode, error: result.error });
  });

  server.registerTool('apply_candidate_to_request', {
    description: 'Registers an existing candidate as an applicant of a request (job order). Use find_request to get the request id. Comments are shown to recruiters on the applicant. The API records the signed-in person as creator and adds the request job title to the candidate skills.',
    inputSchema: {
      requestId: z.guid().describe('Request id (Guid) from find_request.'),
      candidateId: z.guid().describe('Candidate id from create_candidate or find_candidates.'),
      comments: z.string().nullish().describe('Optional comments, e.g. screening answers from the job board. Stored on the applicant even though the response does not echo them.')
    }
  }, async ({ requestId, candidateId, comments }, { signal }) => {
    const result = await api.addApplicant(requestId, { candidateId, comments: blankToUndefined(comments) }, signal);
    return jsonResult(result.success
      ? { success: true, applicant: result.value }
      : { success: false, statusCode: result.statusCode, error: result.error });
  });
}
