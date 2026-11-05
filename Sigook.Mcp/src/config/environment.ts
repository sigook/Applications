import { mkdirSync } from 'node:fs';
import { homedir } from 'node:os';
import { join } from 'node:path';

export const CLIENT_ID = 'sigook.mcp';
export const SCOPE = 'openid profile email roles api1 offline_access';
export const LOOPBACK_PORT = 53421;
export const REDIRECT_URI = `http://localhost:${LOOPBACK_PORT}/callback`;

const KNOWN: Record<string, { api: string; authority: string }> = {
  local: { api: 'https://localhost:44307', authority: 'https://localhost:44307' },
  staging: { api: 'https://staging.api.sigook.ca', authority: 'https://staging.accounts.sigook.ca' },
  production: { api: 'https://api.sigook.com', authority: 'https://accounts.sigook.com' }
};

export interface SigookEnvironment {
  name: string;
  apiUrl: string;
  authority: string;
  allowInvalidCertificates: boolean;
}

const withTrailingSlash = (url: string): string => `${url.replace(/\/+$/, '')}/`;

export function environmentFromProcess(env: NodeJS.ProcessEnv = process.env): SigookEnvironment {
  const name = (env.SIGOOK_ENV ?? 'staging').toLowerCase();
  const urls = KNOWN[name];
  if (!urls) throw new Error(`Unknown SIGOOK_ENV '${env.SIGOOK_ENV}'. Use one of: ${Object.keys(KNOWN).join(', ')}.`);
  return {
    name,
    apiUrl: withTrailingSlash(env.SIGOOK_API_URL ?? urls.api),
    authority: withTrailingSlash(env.SIGOOK_AUTHORITY ?? urls.authority),
    allowInvalidCertificates: env.SIGOOK_INSECURE === '1' || name === 'local'
  };
}

function localAppData(): string {
  if (process.env.LOCALAPPDATA) return process.env.LOCALAPPDATA;
  if (process.platform === 'darwin') return join(homedir(), 'Library', 'Application Support');
  return process.env.XDG_DATA_HOME ?? join(homedir(), '.local', 'share');
}

export function dataDirectory(): string {
  const directory = join(localAppData(), 'Sigook.Mcp');
  mkdirSync(directory, { recursive: true });
  return directory;
}
