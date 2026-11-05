import type { McpServer } from '@modelcontextprotocol/sdk/server/mcp.js';
import { jsonResult, type Services } from './services.js';

export function registerSessionTools(server: McpServer, { session, api }: Services): void {
  const { name: environment, apiUrl } = session.environment;

  server.registerTool('sigook_login', {
    description: "Signs in to Sigook with the Microsoft 365 account of the person running this harness. Opens the browser once; the session is cached for 30 days. Call this when any other tool reports 'Not signed in'."
  }, async ({ signal }) => {
    await session.login(signal);
    const user = await api.getUserInfo(signal);
    return jsonResult({ environment, apiUrl, user });
  });

  server.registerTool('sigook_logout', {
    description: 'Signs out of Sigook and forgets the cached session for the current environment.'
  }, async ({ signal }) => {
    await session.logout(signal);
    return jsonResult({ environment, signedIn: false });
  });

  server.registerTool('sigook_whoami', {
    description: 'Shows the target Sigook environment (local, staging or production) and who is signed in. Use it before writing anything to confirm you are pointing at the right environment.'
  }, async ({ signal }) => {
    if (!session.isSignedIn) return jsonResult({ environment, apiUrl, signedIn: false });
    const user = await api.getUserInfo(signal);
    return jsonResult({ environment, apiUrl, signedIn: true, user });
  });
}
