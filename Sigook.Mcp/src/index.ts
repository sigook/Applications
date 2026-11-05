#!/usr/bin/env node
import { McpServer } from '@modelcontextprotocol/sdk/server/mcp.js';
import { StdioServerTransport } from '@modelcontextprotocol/sdk/server/stdio.js';
import { CatalogCache } from './api/catalogCache.js';
import { ExportCache } from './api/exportCache.js';
import { SigookApi } from './api/sigookApi.js';
import { PkceLoginFlow } from './auth/pkceLoginFlow.js';
import { SigookSession } from './auth/sigookSession.js';
import { TokenStore } from './auth/tokenStore.js';
import { environmentFromProcess } from './config/environment.js';
import { packageVersion } from './config/packageInfo.js';
import { ImportInstructions } from './instructions/importInstructions.js';
import { registerCandidateTools } from './tools/candidateTools.js';
import { registerDocumentTools } from './tools/documentTools.js';
import { registerExportTools } from './tools/exportTools.js';
import { registerImportCandidateTools } from './tools/importCandidateTools.js';
import { registerImportTools } from './tools/importTools.js';
import { registerLookupTools } from './tools/lookupTools.js';
import { registerSessionTools } from './tools/sessionTools.js';
import type { Services } from './tools/services.js';

console.log = console.info = console.debug = console.warn = console.error;

const environment = environmentFromProcess();
if (environment.allowInvalidCertificates) process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';

const session = new SigookSession(new TokenStore(environment.name), new PkceLoginFlow(environment), environment);
const api = new SigookApi(session, environment);
const services: Services = {
  session,
  api,
  catalog: new CatalogCache(api),
  exports: new ExportCache(),
  instructions: new ImportInstructions()
};

const server = new McpServer({ name: 'sigook', version: packageVersion }, {
  instructions: 'Sigook staffing platform. Call sigook_whoami first to confirm the target environment and session. To import candidates from a job-board export, call get_import_instructions before anything else: it returns the workflow and the column mapping of the source.'
});

registerSessionTools(server, services);
registerLookupTools(server, services);
registerExportTools(server, services);
registerDocumentTools(server);
registerImportTools(server, services);
registerImportCandidateTools(server, services);
registerCandidateTools(server, services);

await server.connect(new StdioServerTransport());
