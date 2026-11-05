import { readFileSync, readdirSync } from 'node:fs';
import { join } from 'node:path';
import { instructionsDirectory } from '../config/packageInfo.js';

export interface SourceMapping {
  name: string;
  title: string;
  description: string;
  detectHeaders: string[];
  markdown: string;
}

export function parseMapping(content: string): SourceMapping {
  const lines = content.split('\n').map(line => line.replace(/\r$/, ''));
  if (lines[0] !== '---') throw new Error('Mapping file must start with a front matter block');
  const end = lines.indexOf('---', 1);
  const meta = new Map<string, string>();
  for (const line of lines.slice(1, end)) {
    const separator = line.indexOf(':');
    if (separator > 0) meta.set(line.slice(0, separator).trim().toLowerCase(), line.slice(separator + 1).trim());
  }
  const name = meta.get('name');
  if (!name) throw new Error('Mapping front matter must have a name');
  return {
    name,
    title: meta.get('title') ?? name,
    description: meta.get('description') ?? '',
    detectHeaders: (meta.get('detect') ?? '').split(',').map(header => header.trim()).filter(Boolean),
    markdown: lines.slice(end + 1).join('\n').trim()
  };
}

export class ImportInstructions {
  readonly workflow: string;
  readonly mappings: SourceMapping[];

  constructor(directory: string = instructionsDirectory) {
    this.workflow = readFileSync(join(directory, 'workflow.md'), 'utf8');
    this.mappings = readdirSync(directory)
      .filter(file => file.endsWith('.md') && file !== 'workflow.md')
      .map(file => parseMapping(readFileSync(join(directory, file), 'utf8')))
      .sort((a, b) => a.name.localeCompare(b.name));
  }

  getMapping(source: string): SourceMapping {
    const wanted = source.trim().toLowerCase();
    const mapping = this.mappings.find(item => item.name.toLowerCase() === wanted);
    if (!mapping) throw new Error(`Unknown source '${source}'. Known sources: ${this.mappings.map(item => item.name).join(', ')}.`);
    return mapping;
  }

  detect(headers: string[]): SourceMapping | undefined {
    const set = new Set(headers.map(header => header.toLowerCase()));
    return this.mappings.find(item => item.detectHeaders.length > 0 && item.detectHeaders.every(header => set.has(header.toLowerCase())));
  }
}
