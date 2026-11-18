import { mkdirSync, writeFileSync } from 'node:fs';
import { basename, join, resolve } from 'node:path';
import type { McpServer } from '@modelcontextprotocol/sdk/server/mcp.js';
import { z } from 'zod';
import { WorkbookReader } from '../api/workbookReader.js';
import { dataDirectory } from '../config/environment.js';
import { jsonResult, type Services } from './services.js';

const safeSegment = (value: string): string => value.trim().replace(/[<>:"/\\|?*]+/g, '_');

const teamsLocation = {
  team: z.string().describe('Teams team name, e.g. Covenant.'),
  channel: z.string().describe('Private or shared channel name, e.g. Management.')
};

export function registerFileTools(server: McpServer, { api }: Services): void {
  server.registerTool('find_teams_files', {
    description: 'Searches the files of a private or shared Microsoft Teams channel by name, most recently modified first. Admin only. Use it to get the exact file name that download_teams_file needs.',
    inputSchema: { ...teamsLocation, name: z.string().describe('Part of the file name, e.g. Quote Detail.') }
  }, async ({ team, channel, name }, { signal }) => jsonResult(await api.findTeamsFiles(team, channel, name, signal)));

  server.registerTool('download_teams_file', {
    description: 'Downloads a file of a private or shared Microsoft Teams channel to the local cache and returns its local path (for read_workbook, read_document, ...). Admin only. The name must be exact; find it with find_teams_files.',
    inputSchema: { ...teamsLocation, name: z.string().describe('Exact file name, e.g. Final - Epsilon Quote Detail 2026.xlsx.') }
  }, async ({ team, channel, name }, { signal }) => {
    const content = await api.downloadTeamsFile(team, channel, name, signal);
    const directory = join(dataDirectory(), 'teams', safeSegment(team), safeSegment(channel));
    mkdirSync(directory, { recursive: true });
    const path = join(directory, safeSegment(basename(name)));
    writeFileSync(path, content);
    return jsonResult({ path, size: content.length, downloadedAt: new Date().toISOString() });
  });

  server.registerTool('read_workbook', {
    description: 'Reads an Excel workbook (.xlsx) from a local path: the list of sheets plus a page of rows of one sheet. Formula cells return their last calculated value. Each row is { row, cells } where cells[0] is column A. Empty rows are skipped.',
    inputSchema: {
      path: z.string().describe('Local path of the .xlsx file.'),
      sheet: z.string().nullish().describe('Sheet name. Default: the first sheet.'),
      firstRow: z.number().int().min(1).nullish().default(1).describe('First row number to read (1-based). Default 1.'),
      take: z.number().int().min(1).max(500).nullish().default(50).describe('Maximum number of non-empty rows to return. Default 50.')
    }
  }, async ({ path, sheet, firstRow, take }) => {
    const reader = await WorkbookReader.open(resolve(path));
    return jsonResult(reader.rows(sheet, firstRow ?? 1, take ?? 50));
  });
}
