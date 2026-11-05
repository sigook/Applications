import type { McpServer } from '@modelcontextprotocol/sdk/server/mcp.js';
import { z } from 'zod';
import { digitsOnly, isBlank, tryNormalizePhone } from '../api/lookupRules.js';
import type { AgencyRequestListModel, CatalogItem, ProvinceModel } from '../models/apiModels.js';
import { jsonResult, type Services } from './services.js';

const pickCatalogItem = ({ id, value }: CatalogItem): CatalogItem => ({ id, value });

const pickProvince = ({ id, value, code }: ProvinceModel): ProvinceModel => ({ id, value, code });

const pickRequest = (request: AgencyRequestListModel): AgencyRequestListModel => ({
  id: request.id,
  numberId: request.numberId,
  jobTitle: request.jobTitle,
  companyFullName: request.companyFullName,
  address: request.address,
  city: request.city,
  provinceName: request.provinceName,
  postalCode: request.postalCode,
  requestStatus: request.requestStatus,
  workersQuantity: request.workersQuantity,
  workersQuantityWorking: request.workersQuantityWorking
});

export function registerLookupTools(server: McpServer, { api, catalog }: Services): void {
  server.registerTool('find_request', {
    description: 'Finds a Sigook request (job order) of the current agency by its number, e.g. 2549. Returns id, job title, company, location and status. The id is what apply_candidate_to_request needs.',
    inputSchema: { numberId: z.number().int().describe('Request number as shown in Sigook (NumberId).') }
  }, async ({ numberId }, { signal }) => {
    const page = await api.getRequests(numberId, 5, signal);
    const match = page.items.find(request => request.numberId === numberId);
    return jsonResult(match ? { found: true, numberId, request: pickRequest(match) } : { found: false, numberId });
  });

  server.registerTool('resolve_city', {
    description: 'Resolves a city name plus province/state into a Sigook city. Country is CA or USA. The state can be a code (PA) or a name (Pennsylvania). Returns the best match and, when ambiguous, the closest alternatives.',
    inputSchema: {
      city: z.string().describe('City name, e.g. Philadelphia.'),
      provinceOrState: z.string().describe('Province or state code or name, e.g. PA, ON, Ontario.'),
      countryCode: z.string().nullish().default('USA').describe('CA or USA. Default USA.')
    }
  }, async ({ city, provinceOrState, countryCode }) => {
    const resolution = await catalog.resolveCity(city, provinceOrState, countryCode ?? 'USA');
    if (resolution.city) {
      const { id, value } = resolution.city;
      return jsonResult({ found: true, city: { id, value, province: pickProvince(resolution.province!) } });
    }
    return jsonResult({
      found: false,
      reason: resolution.error,
      province: resolution.province && pickProvince(resolution.province),
      provinces: resolution.provinces,
      alternatives: resolution.alternatives
    });
  });

  server.registerTool('list_sources', {
    description: 'Lists the candidate sources configured in Sigook (ZipRecruiter, Indeed, Other...). Use the id as sourceId in create_candidate.'
  }, async ({ signal }) => jsonResult((await api.getSources(signal)).map(pickCatalogItem)));

  server.registerTool('list_genders', {
    description: 'Lists the gender catalog of Sigook. Use the id as genderId in create_candidate.'
  }, async ({ signal }) => jsonResult((await api.getGenders(signal)).map(pickCatalogItem)));

  server.registerTool('list_skills', {
    description: 'Lists the skill vocabulary Sigook already uses (worker skill catalog). Prefer these names when extracting skills from a resume so candidates stay searchable; the API still accepts new skills.'
  }, async ({ signal }) => {
    const skills = await api.getSkills(signal);
    return jsonResult(skills.map(item => item.skill).filter(skill => !isBlank(skill)).sort());
  });

  server.registerTool('find_candidates', {
    description: 'Searches existing candidates of the current agency by name and/or phone. Use it before create_candidate to detect duplicates. Phone must be in Sigook format (### ###-####); use normalize_phone first.',
    inputSchema: {
      name: z.string().nullish().describe('Full or partial name.'),
      phone: z.string().nullish().describe('Phone in Sigook format ### ###-####.')
    }
  }, async ({ name, phone }, { signal }) => {
    if (isBlank(name) && isBlank(phone)) return jsonResult({ error: 'Provide name or phone.' });
    const page = await api.getCandidates(name, phone, 10, signal);
    return jsonResult({
      totalItems: page.totalItems,
      candidates: page.items.map(candidate => ({
        id: candidate.id,
        name: candidate.name,
        email: candidate.email,
        phones: candidate.phoneNumbers?.map(item => item.phoneNumber),
        city: candidate.city,
        source: candidate.source,
        recruiter: candidate.recruiter,
        createdAt: candidate.createdAt,
        dnu: candidate.dnu,
        requests: candidate.requests?.map(item => item.value)
      }))
    });
  });

  server.registerTool('normalize_phone', {
    description: "Converts any North American phone string (e.g. '+1 4452510680', '(445) 251-0680') into the format Sigook requires: ### ###-####. Returns an error when the number does not have 10 digits.",
    inputSchema: { phone: z.string().describe('Raw phone number.') }
  }, async ({ phone }) => {
    const normalized = tryNormalizePhone(phone);
    return jsonResult(normalized
      ? { ok: true, input: phone, phone: normalized }
      : { ok: false, input: phone, error: `Expected 10 digits, got ${digitsOnly(phone).length}.` });
  });
}
