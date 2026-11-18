import { readFileSync, readdirSync } from 'node:fs';
import { join } from 'node:path';
import { instructionsDirectory } from '../config/packageInfo.js';

export interface InstructionTopic {
  name: string;
  title: string;
  description: string;
  markdown: string;
}

export interface SourceMapping extends InstructionTopic {
  detectHeaders: string[];
}

export const IMPORT_CANDIDATES_TOPIC = 'import-candidates';

function parseFrontMatter(content: string): { meta: Map<string, string>; topic: InstructionTopic } {
  const lines = content.split('\n').map(line => line.replace(/\r$/, ''));
  if (lines[0] !== '---') throw new Error('Instruction file must start with a front matter block');
  const end = lines.indexOf('---', 1);
  const meta = new Map<string, string>();
  for (const line of lines.slice(1, end)) {
    const separator = line.indexOf(':');
    if (separator > 0) meta.set(line.slice(0, separator).trim().toLowerCase(), line.slice(separator + 1).trim());
  }
  const name = meta.get('name');
  if (!name) throw new Error('Instruction front matter must have a name');
  return {
    meta,
    topic: {
      name,
      title: meta.get('title') ?? name,
      description: meta.get('description') ?? '',
      markdown: lines.slice(end + 1).join('\n').trim()
    }
  };
}

export const parseTopic = (content: string): InstructionTopic => parseFrontMatter(content).topic;

export function parseMapping(content: string): SourceMapping {
  const { meta, topic } = parseFrontMatter(content);
  return { ...topic, detectHeaders: (meta.get('detect') ?? '').split(',').map(header => header.trim()).filter(Boolean) };
}

const readMarkdown = (directory: string): string[] => readdirSync(directory)
  .filter(file => file.endsWith('.md'))
  .map(file => readFileSync(join(directory, file), 'utf8'));

const byName = (a: InstructionTopic, b: InstructionTopic) => a.name.localeCompare(b.name);

function findByName<T extends InstructionTopic>(items: T[], name: string, kind: string): T {
  const wanted = name.trim().toLowerCase();
  const item = items.find(candidate => candidate.name.toLowerCase() === wanted);
  if (!item) throw new Error(`Unknown ${kind} '${name}'. Known ${kind}s: ${items.map(candidate => candidate.name).join(', ')}.`);
  return item;
}

export class Instructions {
  readonly topics: InstructionTopic[];
  readonly mappings: SourceMapping[];

  constructor(directory: string = instructionsDirectory) {
    this.topics = readMarkdown(directory).map(parseTopic).sort(byName);
    this.mappings = readMarkdown(join(directory, 'sources')).map(parseMapping).sort(byName);
  }

  getTopic(topic: string): InstructionTopic {
    return findByName(this.topics, topic, 'topic');
  }

  getMapping(source: string): SourceMapping {
    return findByName(this.mappings, source, 'source');
  }

  detect(headers: string[]): SourceMapping | undefined {
    const set = new Set(headers.map(header => header.toLowerCase()));
    return this.mappings.find(item => item.detectHeaders.length > 0 && item.detectHeaders.every(header => set.has(header.toLowerCase())));
  }

  serverInstructions(): string {
    return [
      'Sigook staffing platform. Call sigook_whoami first to confirm the target environment and session.',
      'Before starting one of these workflows, call get_instructions with its topic and follow what it returns:',
      ...this.topics.map(topic => `- ${topic.name}: ${topic.description}`)
    ].join('\n');
  }
}
