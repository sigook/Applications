import { resolve } from 'node:path';
import type { McpServer } from '@modelcontextprotocol/sdk/server/mcp.js';
import { z } from 'zod';
import { WorkbookReader } from '../api/workbookReader.js';
import type { CompanyProfileListModel, JobPositionModel } from '../models/apiModels.js';
import { readQuote } from '../rates/quoteSheet.js';
import { RATE_CATEGORIES, compareRole, missingRoles, type ComparisonStatus } from '../rates/rateComparison.js';
import { jsonResult, type Services } from './services.js';

const pickCompany = ({ id, numberId, fullName, locations, active, industry }: CompanyProfileListModel) =>
  ({ profileId: id, numberId, fullName, locations, active, industry });

const pickRole = ({ id, jobPosition, workerRate, rate }: JobPositionModel) => ({ id, jobPosition: jobPosition ?? '', workerRate, rate });

const sameCents = (a: number, b: number): boolean => Math.round(a * 100) === Math.round(b * 100);

export function registerCompanyRateTools(server: McpServer, { api }: Services): void {
  server.registerTool('find_companies', {
    description: 'Finds client companies of the current agency by name or location. Returns profileId (what the role tools need), number, name and locations. A company can have one profile per country: pick it by its locations.',
    inputSchema: { name: z.string().describe('Part of the company name or location, e.g. Epsilon.') }
  }, async ({ name }, { signal }) => jsonResult((await api.getCompanies(name, 50, signal)).items.map(pickCompany)));

  server.registerTool('list_company_roles', {
    description: 'Lists the roles (job positions) of a company profile with their rates: workerRate = pay rate of the worker, rate = bill rate charged to the company. Requests read their rates from the role.',
    inputSchema: {
      profileId: z.string().describe('Company profile id from find_companies.'),
      search: z.string().nullish().describe('Only roles whose name contains this text. Optional.')
    }
  }, async ({ profileId, search }, { signal }) => {
    const roles = (await api.getJobPositions(profileId, signal)).map(pickRole);
    const wanted = search?.trim().toLowerCase();
    return jsonResult(wanted ? roles.filter(role => role.jobPosition.toLowerCase().includes(wanted)) : roles);
  });

  server.registerTool('compare_role_rates', {
    description: 'Compares the roles of a company profile against a rate quote sheet of a local .xlsx (get_instructions rate-quote has the layout of each sheet). Each role is matched by category (general labor, skilled labor, forklift, supervisor) and US state + city parsed from its name. Status: equal, different, ambiguous (several quote rows with different rates), no-quote (category or location missing in the quote), unparsed (no category or no state). Read-only.',
    inputSchema: {
      profileId: z.string().describe('Company profile id from find_companies.'),
      path: z.string().describe('Local path of the quote workbook (download_teams_file).'),
      sheet: z.string().describe('Quote sheet, e.g. QUOTE EPSILON.'),
      locationColumn: z.string().default('A').describe('Column with "ST - City" locations. Default A.'),
      firstRow: z.number().int().min(1).describe('First data row of the sheet.'),
      categories: z.array(z.object({
        category: z.enum(RATE_CATEGORIES),
        payColumn: z.string().describe('Column letter of the pay rate.'),
        billColumn: z.string().describe('Column letter of the bill rate.')
      })).min(1).describe('Columns of each rate category in the sheet.'),
      onlyDifferences: z.boolean().default(true).describe('Leave out the roles whose rates are equal (they are still counted). Default true.'),
      includeMissing: z.boolean().default(false).describe('Also list quote city + category pairs with no role in Sigook. Default false.')
    }
  }, async ({ profileId, path, sheet, locationColumn, firstRow, categories, onlyDifferences, includeMissing }, { signal }) => {
    const reader = await WorkbookReader.open(resolve(path));
    const quote = readQuote(reader, { sheet, locationColumn, firstRow, categories });
    const roles = (await api.getJobPositions(profileId, signal)).map(pickRole);
    const comparisons = roles.map(role => compareRole(role, quote.rows));
    const counts: Partial<Record<ComparisonStatus, number>> = {};
    for (const { status } of comparisons) counts[status] = (counts[status] ?? 0) + 1;
    const missing = missingRoles(comparisons, quote.rows);
    return jsonResult({
      sheet,
      quoteRows: quote.rows.length,
      unreadableQuoteLocations: quote.skipped,
      roles: roles.length,
      counts,
      missingInSigook: missing.length,
      comparisons: onlyDifferences ? comparisons.filter(item => item.status !== 'equal') : comparisons,
      missing: includeMissing ? missing : undefined
    });
  });

  server.registerTool('update_role_rates', {
    description: 'Updates the pay rate (workerRate) and bill rate (rate) of roles of a company profile. Only the rates change; name, shift, description and overtime stay as they are. Every request of the role reads the new rates right away, including hours not yet paid or invoiced. Writes data: only call it for roles the user confirmed.',
    inputSchema: {
      profileId: z.string().describe('Company profile id from find_companies.'),
      updates: z.array(z.object({
        roleId: z.string().describe('Role id from list_company_roles or compare_role_rates.'),
        workerRate: z.number().positive().describe('New pay rate of the worker.'),
        rate: z.number().positive().describe('New bill rate charged to the company.')
      })).min(1).max(100)
    }
  }, async ({ profileId, updates }, { signal }) => {
    const results = [];
    for (const { roleId, workerRate, rate } of updates) {
      const current = await api.getJobPosition(profileId, roleId, signal);
      const before = { workerRate: current.workerRate, rate: current.rate };
      if (sameCents(current.workerRate, workerRate) && sameCents(current.rate, rate)) {
        results.push({ roleId, jobPosition: current.jobPosition, success: true, unchanged: true, before });
        continue;
      }
      const result = await api.updateJobPosition(profileId, roleId, { ...current, workerRate, rate }, signal);
      results.push(result.success
        ? { roleId, jobPosition: current.jobPosition, success: true, before, after: { workerRate, rate } }
        : { roleId, jobPosition: current.jobPosition, success: false, statusCode: result.statusCode, error: result.error, before });
    }
    return jsonResult({
      updated: results.filter(item => item.success && !('unchanged' in item)).length,
      failed: results.filter(item => !item.success).length,
      results
    });
  });
}
