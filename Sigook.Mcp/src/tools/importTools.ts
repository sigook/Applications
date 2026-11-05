import { ResourceTemplate, type McpServer } from '@modelcontextprotocol/sdk/server/mcp.js';
import { z } from 'zod';
import { isBlank } from '../api/lookupRules.js';
import { jsonResult, type Services } from './services.js';

const MARKDOWN = 'text/markdown';

export function registerImportTools(server: McpServer, { instructions }: Services): void {
  server.registerTool('get_import_instructions', {
    description: 'Returns the step-by-step workflow to import candidates from a job-board export into Sigook, plus the column mapping of a source (ZipRecruiter, ...). Call it with no source to list the known sources and the CSV headers that identify each one; call it with the headers of the CSV to auto-detect the source. Always read this before importing.',
    inputSchema: {
      source: z.string().nullish().describe('Source name, e.g. ziprecruiter. Optional.'),
      headers: z.array(z.string()).nullish().describe('CSV headers as returned by read_export, used to auto-detect the source. Optional.')
    }
  }, async ({ source, headers }) => {
    const mapping = source !== null && source !== undefined ? instructions.getMapping(source)
      : headers?.length ? instructions.detect(headers)
      : undefined;
    return jsonResult({
      workflow: instructions.workflow,
      detectedSource: mapping?.name,
      mapping: mapping?.markdown,
      knownSources: instructions.mappings.map(({ name, title, description, detectHeaders }) => ({ name, title, description, detectHeaders }))
    });
  });

  server.registerPrompt('import-candidates', {
    description: 'Import candidates from a job-board export (folder or zip with CSV + resumes) into Sigook.',
    argsSchema: {
      path: z.string().describe('Folder or zip path of the export.'),
      source: z.string().optional().describe('Source name (ziprecruiter, ...). Leave empty to auto-detect from the CSV headers.')
    }
  }, ({ path, source }) => {
    const mapping = isBlank(source) ? undefined : instructions.getMapping(source!);
    const mappingSection = mapping
      ? `# Source mapping: ${mapping.title}\n\n${mapping.markdown}`
      : `Detect the source: call \`get_import_instructions\` with the CSV headers from \`read_export\`. Known sources: ${instructions.mappings.map(item => item.name).join(', ')}.`;
    return {
      messages: [{
        role: 'user' as const,
        content: { type: 'text' as const, text: `Import the candidates found at \`${path}\` into Sigook.\n\n${instructions.workflow}\n\n${mappingSection}` }
      }]
    };
  });

  server.registerResource('import-candidates', 'sigook://instructions/import-candidates', {
    description: 'Generic workflow to import candidates into Sigook.',
    mimeType: MARKDOWN
  }, uri => ({ contents: [{ uri: uri.href, mimeType: MARKDOWN, text: instructions.workflow }] }));

  server.registerResource('source-mapping', new ResourceTemplate('sigook://mappings/{source}', { list: undefined }), {
    description: 'Column mapping of a job-board export (ziprecruiter, ...) to Sigook fields.',
    mimeType: MARKDOWN
  }, (uri, { source }) => ({
    contents: [{ uri: uri.href, mimeType: MARKDOWN, text: instructions.getMapping(String(source)).markdown }]
  }));
}
