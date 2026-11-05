import { existsSync, readFileSync } from 'node:fs';
import { extname } from 'node:path';
import type { McpServer } from '@modelcontextprotocol/sdk/server/mcp.js';
import mammoth from 'mammoth';
import { extractText, getDocumentProxy } from 'unpdf';
import { z } from 'zod';
import { jsonResult } from './services.js';

interface DocumentContent {
  text: string;
  pages: number;
}

async function readPdf(path: string): Promise<DocumentContent> {
  const pdf = await getDocumentProxy(new Uint8Array(readFileSync(path)), { verbosity: 0 });
  const { totalPages, text } = await extractText(pdf, { mergePages: false });
  return { text: text.join('\n\n'), pages: totalPages };
}

async function readDocx(path: string): Promise<DocumentContent> {
  const { value } = await mammoth.extractRawText({ path });
  return { text: value, pages: 1 };
}

const readPlain = (path: string): DocumentContent =>
  ({ text: readFileSync(path, 'utf8').replace(/^﻿/, ''), pages: 1 });

export function normalizeText(text: string): string {
  const output: string[] = [];
  let blankRun = 0;
  for (const raw of text.replaceAll('\r\n', '\n').split('\n')) {
    const line = raw.split(' ').filter(Boolean).join(' ');
    if (line.length === 0) {
      if (++blankRun > 1) continue;
    } else {
      blankRun = 0;
    }
    output.push(line);
  }
  return output.join('\n').trim();
}

export function registerDocumentTools(server: McpServer): void {
  server.registerTool('read_document', {
    description: 'Extracts the text of a local resume or document (pdf, docx, txt, md) so you can read it: skills, certifications, address, experience. Scanned PDFs have no text layer and return empty text. Use the paths returned by read_export.',
    inputSchema: {
      path: z.string().describe('Local file path.'),
      maxChars: z.number().int().nullish().default(20000).describe('Maximum characters to return. Default 20000.')
    }
  }, async ({ path, maxChars }) => {
    if (!existsSync(path)) throw new Error(`File not found: ${path}`);
    const extension = extname(path).toLowerCase();
    let content: DocumentContent;
    if (extension === '.pdf') content = await readPdf(path);
    else if (extension === '.docx') content = await readDocx(path);
    else if (['.txt', '.md', '.csv'].includes(extension)) content = readPlain(path);
    else throw new Error(`Unsupported format ${extension}. Supported: pdf, docx, txt, md.`);
    const limit = maxChars ?? 20000;
    const normalized = normalizeText(content.text);
    const truncated = normalized.length > limit;
    return jsonResult({
      path,
      format: extension.slice(1),
      pages: content.pages,
      chars: normalized.length,
      truncated,
      hasText: normalized.length > 0,
      text: truncated ? normalized.slice(0, limit) : normalized
    });
  });
}
