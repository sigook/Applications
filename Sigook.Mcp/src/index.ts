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
import { Instructions } from './instructions/instructions.js';
import { registerCandidateTools } from './tools/candidateTools.js';
import { registerCompanyRateTools } from './tools/companyRateTools.js';
import { registerDocumentTools } from './tools/documentTools.js';
import { registerExportTools } from './tools/exportTools.js';
import { registerFileTools } from './tools/fileTools.js';
import { registerImportCandidateTools } from './tools/importCandidateTools.js';
import { registerInstructionTools } from './tools/instructionTools.js';
import { registerLookupTools } from './tools/lookupTools.js';
import { registerSessionTools } from './tools/sessionTools.js';
import type { Services } from './tools/services.js';

console.log = console.info = console.debug = console.warn = console.error;

const environment = environmentFromProcess();
if (environment.allowInvalidCertificates) process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';

const session = new SigookSession(new TokenStore(environment.name), new PkceLoginFlow(environment), environment);
const api = new SigookApi(session, environment);
const instructions = new Instructions();
const services: Services = {
  session,
  api,
  catalog: new CatalogCache(api),
  exports: new ExportCache(),
  instructions
};

const server = new McpServer({ name: 'sigook', version: packageVersion }, {
  instructions: instructions.serverInstructions()
});

registerSessionTools(server, services);
registerLookupTools(server, services);
registerExportTools(server, services);
registerDocumentTools(server);
registerInstructionTools(server, services);
registerImportCandidateTools(server, services);
registerCandidateTools(server, services);
registerFileTools(server, services);
registerCompanyRateTools(server, services);

await server.connect(new StdioServerTransport());
