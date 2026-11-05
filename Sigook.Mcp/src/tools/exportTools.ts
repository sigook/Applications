import { readFileSync, readdirSync, statSync } from 'node:fs';
import { basename, extname, join, relative } from 'node:path';
import type { McpServer } from '@modelcontextprotocol/sdk/server/mcp.js';
import { parse } from 'csv-parse/sync';
import { z } from 'zod';
import { jsonResult, type Services } from './services.js';

const DOCUMENT_EXTENSIONS = new Set(['.pdf', '.doc', '.docx']);
const DELIMITERS = [',', ';', '\t', '|'];

const listFiles = (root: string): string[] =>
  readdirSync(root, { recursive: true, withFileTypes: true })
    .filter(entry => entry.isFile())
    .map(entry => join(entry.parentPath, entry.name))
    .sort();

const toRelative = (root: string, file: string): string => relative(root, file).replaceAll('\\', '/');

export function detectDelimiter(content: string): string {
  const firstLine = (content.split(/\r?\n/, 1)[0] ?? '').replace(/"[^"]*"/g, '');
  const count = (delimiter: string): number => firstLine.split(delimiter).length;
  return DELIMITERS.reduce((best, delimiter) => (count(delimiter) > count(best) ? delimiter : best));
}

export function parseCsv(content: string, skip: number, take: number) {
  const records = parse(content, {
    bom: true,
    delimiter: detectDelimiter(content.replace(/^﻿/, '')),
    relax_quotes: true,
    relax_column_count: true,
    skip_empty_lines: true
  }) as string[][];
  const [headers = [], ...data] = records;
  const rows = data.slice(skip, skip + take).map(record =>
    Object.fromEntries(headers.map((header, index) => [header, record[index] ?? ''])));
  return { headers, totalRows: data.length, skip, returned: rows.length, rows };
}

export function registerExportTools(server: McpServer, { exports: cache }: Services): void {
  server.registerTool('read_export', {
    description: 'Reads a job-board export (a folder or a zip containing CSV files and resume documents). Returns the CSV rows as raw header/value maps, untouched, plus the local path of every document found so it can be passed to create_candidate. Zips are extracted once into a local cache. Use skip/take to page through large files.',
    inputSchema: {
      path: z.string().describe('Folder or .zip path of the export. A folder may contain several zips; all of them are read.'),
      skip: z.number().int().nullish().default(0).describe('Rows to skip, per CSV file. Default 0.'),
      take: z.number().int().nullish().default(100).describe('Maximum rows to return, per CSV file. Default 100.')
    }
  }, async ({ path, skip, take }) => {
    const root = cache.materialize(path);
    const files = listFiles(root);
    const csvFiles = files
      .filter(file => extname(file).toLowerCase() === '.csv')
      .map(file => ({ fileName: toRelative(root, file), ...parseCsv(readFileSync(file, 'utf8'), skip ?? 0, take ?? 100) }));
    const documents = files
      .filter(file => DOCUMENT_EXTENSIONS.has(extname(file).toLowerCase()))
      .map(file => ({ fileName: basename(file), relativePath: toRelative(root, file), path: file, sizeBytes: statSync(file).size }));
    return jsonResult({ root, csvFiles, documents });
  });
}
