import { readFileSync } from 'node:fs';
import { extname } from 'node:path';
import type { SigookSession } from '../auth/sigookSession.js';
import type { SigookEnvironment } from '../config/environment.js';
import type {
  AgencyRequestListModel,
  CandidateCreateModel,
  CandidateListModel,
  CatalogItem,
  CityModel,
  CompanyProfileListModel,
  CovenantFileModel,
  IdentificationTypeModel,
  IdResponse,
  JobPositionModel,
  NoteModel,
  PaginatedList,
  ProvinceModel,
  RequestApplicantModel,
  SkillModel,
  TeamsFileModel,
  UserInfo,
  WorkerDocumentsDetail,
  WorkerListModel
} from '../models/apiModels.js';

const REQUEST_TIMEOUT_MS = 100 * 1000;
const MAX_ERROR_BODY = 2000;

export type ApiResult<T> =
  | { success: true; value: T; statusCode: number }
  | { success: false; statusCode: number; error: string };

export interface FilePart {
  path: string;
  fileName: string;
}

export type WorkerDocumentSection = 'Documents' | 'SinInformation' | 'Licenses' | 'Certificates' | 'OtherDocument' | 'Resume';

export interface WorkerSearch {
  fullName?: string | null;
  phone?: string | null;
  numberId?: number | null;
}

interface CallOptions {
  body?: FormData;
  json?: unknown;
  signal?: AbortSignal;
}

const CONTENT_TYPES: Record<string, string> = {
  '.pdf': 'application/pdf',
  '.doc': 'application/msword',
  '.docx': 'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
  '.png': 'image/png',
  '.jpg': 'image/jpeg',
  '.jpeg': 'image/jpeg',
  '.gif': 'image/gif',
  '.xls': 'application/vnd.ms-excel',
  '.xlsx': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
};

export const toJson = (model: unknown): string =>
  JSON.stringify(model, (_key, value: unknown) => (value === null ? undefined : value));

const truncate = (body: string): string =>
  body.length <= MAX_ERROR_BODY ? body : `${body.slice(0, MAX_ERROR_BODY)}…`;

function networkMessage(error: unknown): string {
  const failure = error as Error & { cause?: Error };
  return failure.cause?.message ? `${failure.message}: ${failure.cause.message}` : failure.message;
}

function parseBody<T>(text: string): T {
  return (text ? JSON.parse(text) : null) as T;
}

function multipart(data: unknown, file?: FilePart): FormData {
  const form = new FormData();
  form.append('data', toJson(data));
  if (file) {
    const type = CONTENT_TYPES[extname(file.path).toLowerCase()] ?? 'application/octet-stream';
    form.append('file', new Blob([readFileSync(file.path)], { type }), file.fileName);
  }
  return form;
}

function query(values: Record<string, string | number | null | undefined>): string {
  const entries = Object.entries(values).filter(([, value]) => value !== null && value !== undefined);
  return new URLSearchParams(entries.map(([key, value]) => [key, String(value)])).toString();
}

export class SigookApi {
  constructor(
    private readonly session: SigookSession,
    private readonly environment: SigookEnvironment
  ) {}

  getSources = (signal?: AbortSignal) => this.get<CatalogItem[]>('api/catalog/source', signal);

  getGenders = (signal?: AbortSignal) => this.get<CatalogItem[]>('api/catalog/gender', signal);

  getSkills = (signal?: AbortSignal) => this.get<SkillModel[]>('api/catalog/skills', signal);

  getProvinces = (countryCode: string, signal?: AbortSignal) =>
    this.get<ProvinceModel[]>(`api/location/province/${encodeURIComponent(countryCode)}`, signal);

  getCities = (provinceId: string, signal?: AbortSignal) =>
    this.get<CityModel[]>(`api/location/city/${provinceId}`, signal);

  getRequests = (numberId: number, pageSize: number, signal?: AbortSignal) =>
    this.get<PaginatedList<AgencyRequestListModel>>(`api/agency/recruiting/requests?${query({ numberId, pageSize })}`, signal);

  getCandidates = (name: string | null | undefined, phone: string | null | undefined, pageSize: number, signal?: AbortSignal) =>
    this.get<PaginatedList<CandidateListModel>>(`api/agency/recruiting/candidates?${query({ name, phone, pageSize })}`, signal);

  createCandidate = (model: CandidateCreateModel, file: FilePart | undefined, signal?: AbortSignal) =>
    this.asResult<IdResponse>('POST', 'api/agency/recruiting/candidates', { body: multipart(model, file), signal });

  addCandidateDocument = (candidateId: string, model: CovenantFileModel, file: FilePart, signal?: AbortSignal) =>
    this.asResult<string>('POST', `api/agency/recruiting/candidates/${candidateId}/documents`, { body: multipart(model, file), signal });

  getIdentificationTypes = (signal?: AbortSignal) =>
    this.get<IdentificationTypeModel[]>('api/catalog/identificationType', signal);

  getWorkers = (filter: WorkerSearch, pageSize: number, signal?: AbortSignal) =>
    this.get<PaginatedList<WorkerListModel>>(`api/agency/recruiting/workers?${query({ ...filter, pageSize })}`, signal);

  getWorkerDetail = (workerProfileId: string, signal?: AbortSignal) =>
    this.asResult<WorkerDocumentsDetail>('GET', `api/agency/recruiting/workers/${workerProfileId}`, { signal });

  postWorkerDocumentSection = (workerProfileId: string, section: WorkerDocumentSection, model: unknown, file: FilePart, signal?: AbortSignal) =>
    this.asResult<unknown>('POST', `api/worker/profile/${workerProfileId}/${section}`, { body: multipart(model, file), signal });

  addCandidateNote =(candidateId: string, model: NoteModel, signal?: AbortSignal) =>
    this.asResult<IdResponse>('POST', `api/agency/recruiting/candidates/${candidateId}/notes`, { json: model, signal });

  addApplicant = (requestId: string, model: RequestApplicantModel, signal?: AbortSignal) =>
    this.asResult<unknown>('POST', `api/agency/recruiting/requests/${requestId}/applicants`, { json: model, signal });

  getCompanies = (search: string, pageSize: number, signal?: AbortSignal) =>
    this.get<PaginatedList<CompanyProfileListModel>>(`api/agency/recruiting/clients?${query({ businessInfo: search, pageSize })}`, signal);

  getJobPositions = (profileId: string, signal?: AbortSignal) =>
    this.get<JobPositionModel[]>(`api/agency/recruiting/clients/${profileId}/jobpositions/Dropdown`, signal);

  getJobPosition = (profileId: string, id: string, signal?: AbortSignal) =>
    this.get<JobPositionModel>(`api/agency/recruiting/clients/${profileId}/jobpositions/${id}`, signal);

  updateJobPosition = (profileId: string, id: string, model: JobPositionModel, signal?: AbortSignal) =>
    this.asResult<unknown>('PUT', `api/agency/recruiting/clients/${profileId}/jobpositions/${id}`, { json: model, signal });

  findTeamsFiles = (team: string, channel: string, name: string, signal?: AbortSignal) =>
    this.get<TeamsFileModel[]>(`api/agency/teams/files?${query({ team, channel, name })}`, signal);

  async downloadTeamsFile(team: string, channel: string, name: string, signal?: AbortSignal): Promise<Buffer> {
    const url = new URL(`api/agency/teams/files/content?${query({ team, channel, name })}`, this.environment.apiUrl);
    const token = await this.session.getAccessToken(signal);
    let response: Response;
    try {
      response = await this.send(url, 'GET', token, {}, signal);
    } catch (error) {
      throw new Error(`Sigook API unreachable: ${networkMessage(error)}`);
    }
    if (response.status === 404) throw new Error(`File '${name}' not found in channel '${channel}' of team '${team}'. Use find_teams_files to see the exact names.`);
    if (!response.ok) throw new Error(`GET ${url.pathname}${url.search} failed (${response.status}): ${truncate(await response.text())}`);
    return Buffer.from(await response.arrayBuffer());
  }

  async getUserInfo(signal?: AbortSignal): Promise<UserInfo> {
    const user = await this.orThrow<UserInfo>(new URL('connect/userinfo', this.environment.authority), signal);
    return { sub: user.sub, name: user.name, email: user.email, nickname: user.nickname, role: user.role };
  }

  private get<T>(path: string, signal?: AbortSignal): Promise<T> {
    return this.orThrow<T>(new URL(path, this.environment.apiUrl), signal);
  }

  private async orThrow<T>(url: URL, signal?: AbortSignal): Promise<T> {
    const token = await this.session.getAccessToken(signal);
    let response: Response;
    let text: string;
    try {
      response = await this.send(url, 'GET', token, {}, signal);
      text = await response.text();
    } catch (error) {
      throw new Error(`Sigook API unreachable: ${networkMessage(error)}`);
    }
    if (!response.ok) throw new Error(`GET ${url.pathname}${url.search} failed (${response.status}): ${truncate(text)}`);
    return parseBody<T>(text);
  }

  private async asResult<T>(method: string, path: string, options: CallOptions): Promise<ApiResult<T>> {
    const token = await this.session.getAccessToken(options.signal);
    try {
      const response = await this.send(new URL(path, this.environment.apiUrl), method, token, options, options.signal);
      const text = await response.text();
      return response.ok
        ? { success: true, value: parseBody<T>(text), statusCode: response.status }
        : { success: false, statusCode: response.status, error: truncate(text) };
    } catch (error) {
      return { success: false, statusCode: 0, error: `Sigook API unreachable: ${networkMessage(error)}` };
    }
  }

  private send(url: URL, method: string, token: string, options: CallOptions, signal?: AbortSignal): Promise<Response> {
    const headers: Record<string, string> = { Authorization: `Bearer ${token}` };
    if (options.json !== undefined) headers['Content-Type'] = 'application/json';
    return fetch(url, {
      method,
      headers,
      body: options.json !== undefined ? toJson(options.json) : options.body,
      signal: AbortSignal.any([AbortSignal.timeout(REQUEST_TIMEOUT_MS), ...(signal ? [signal] : [])])
    });
  }
}
