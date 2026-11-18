import { ResourceTemplate, type McpServer } from '@modelcontextprotocol/sdk/server/mcp.js';
import { z } from 'zod';
import { isBlank } from '../api/lookupRules.js';
import { IMPORT_CANDIDATES_TOPIC } from '../instructions/instructions.js';
import { jsonResult, type Services } from './services.js';

const MARKDOWN = 'text/markdown';

export function registerInstructionTools(server: McpServer, { instructions }: Services): void {
  const topicNames = instructions.topics.map(topic => topic.name).join(', ');

  server.registerTool('get_instructions', {
    description: `Returns the step-by-step instructions of a Sigook workflow. Always read them before starting the workflow. Topics: ${topicNames}. For ${IMPORT_CANDIDATES_TOPIC}, it also lists the known job-board sources; pass source, or the CSV headers from read_export to auto-detect it, to get its column mapping.`,
    inputSchema: {
      topic: z.string().describe(`Workflow topic: ${topicNames}.`),
      source: z.string().nullish().describe(`${IMPORT_CANDIDATES_TOPIC} only. Source name, e.g. ziprecruiter. Optional.`),
      headers: z.array(z.string()).nullish().describe(`${IMPORT_CANDIDATES_TOPIC} only. CSV headers as returned by read_export, used to auto-detect the source. Optional.`)
    }
  }, async ({ topic, source, headers }) => {
    const instructionTopic = instructions.getTopic(topic);
    if (instructionTopic.name !== IMPORT_CANDIDATES_TOPIC) return jsonResult({ topic: instructionTopic.name, instructions: instructionTopic.markdown });
    const mapping = !isBlank(source) ? instructions.getMapping(source!)
      : headers?.length ? instructions.detect(headers)
      : undefined;
    return jsonResult({
      topic: instructionTopic.name,
      instructions: instructionTopic.markdown,
      detectedSource: mapping?.name,
      mapping: mapping?.markdown,
      knownSources: instructions.mappings.map(({ name, title, description, detectHeaders }) => ({ name, title, description, detectHeaders }))
    });
  });

  server.registerPrompt(IMPORT_CANDIDATES_TOPIC, {
    description: 'Import candidates from a job-board export (folder or zip with CSV + resumes) into Sigook.',
    argsSchema: {
      path: z.string().describe('Folder or zip path of the export.'),
      source: z.string().optional().describe('Source name (ziprecruiter, ...). Leave empty to auto-detect from the CSV headers.')
    }
  }, ({ path, source }) => {
    const mapping = isBlank(source) ? undefined : instructions.getMapping(source!);
    const mappingSection = mapping
      ? `# Source mapping: ${mapping.title}\n\n${mapping.markdown}`
      : `Detect the source: call \`get_instructions\` with topic \`${IMPORT_CANDIDATES_TOPIC}\` and the CSV headers from \`read_export\`. Known sources: ${instructions.mappings.map(item => item.name).join(', ')}.`;
    return {
      messages: [{
        role: 'user' as const,
        content: { type: 'text' as const, text: `Import the candidates found at \`${path}\` into Sigook.\n\n${instructions.getTopic(IMPORT_CANDIDATES_TOPIC).markdown}\n\n${mappingSection}` }
      }]
    };
  });

  server.registerResource('instructions', new ResourceTemplate('sigook://instructions/{topic}', {
    list: () => ({
      resources: instructions.topics.map(topic => ({
        uri: `sigook://instructions/${topic.name}`,
        name: topic.name,
        title: topic.title,
        description: topic.description,
        mimeType: MARKDOWN
      }))
    })
  }), {
    description: 'Step-by-step instructions of a Sigook workflow.',
    mimeType: MARKDOWN
  }, (uri, { topic }) => ({
    contents: [{ uri: uri.href, mimeType: MARKDOWN, text: instructions.getTopic(String(topic)).markdown }]
  }));

  server.registerResource('source-mapping', new ResourceTemplate('sigook://mappings/{source}', { list: undefined }), {
    description: 'Column mapping of a job-board export (ziprecruiter, ...) to Sigook fields.',
    mimeType: MARKDOWN
  }, (uri, { source }) => ({
    contents: [{ uri: uri.href, mimeType: MARKDOWN, text: instructions.getMapping(String(source)).markdown }]
  }));
}
