import { spawn } from 'node:child_process';
import { createHash, randomBytes } from 'node:crypto';
import { createServer, type Server } from 'node:http';
import { CLIENT_ID, LOOPBACK_PORT, REDIRECT_URI, SCOPE, type SigookEnvironment } from '../config/environment.js';

const LOGIN_TIMEOUT_MS = 3 * 60 * 1000;

export interface TokenResponse {
  access_token?: string;
  refresh_token?: string;
  expires_in?: number;
  error?: string;
  error_description?: string;
}

interface CallbackQuery {
  code: string | null;
  state: string | null;
  error: string | null;
  errorDescription: string | null;
}

const escapeHtml = (text: string): string =>
  text.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;').replace(/'/g, '&#39;');

function browserCommand(url: string): { command: string; args: string[] } {
  if (process.platform === 'win32') return { command: 'rundll32', args: ['url.dll,FileProtocolHandler', url] };
  if (process.platform === 'darwin') return { command: 'open', args: [url] };
  return { command: 'xdg-open', args: [url] };
}

function openBrowser(url: string): void {
  const { command, args } = browserCommand(url);
  const child = spawn(command, args, { stdio: 'ignore', detached: true });
  child.on('error', error => console.error(`Could not open the browser: ${error.message}. Open this URL manually: ${url}`));
  child.unref();
}

function listen(server: Server): Promise<void> {
  return new Promise((resolve, reject) => {
    server.once('error', (error: NodeJS.ErrnoException) =>
      reject(error.code === 'EADDRINUSE'
        ? new Error(`Port ${LOOPBACK_PORT} is busy; close the process using it and retry. (${error.message})`)
        : error));
    server.listen(LOOPBACK_PORT, 'localhost', resolve);
  });
}

export class PkceLoginFlow {
  constructor(private readonly environment: SigookEnvironment) {}

  async login(signal?: AbortSignal): Promise<TokenResponse> {
    const verifier = randomBytes(32).toString('base64url');
    const challenge = createHash('sha256').update(verifier, 'ascii').digest('base64url');
    const state = randomBytes(16).toString('base64url');

    const query = await this.waitForCallback(this.authorizeUrl(challenge, state), signal);

    if (query.error) throw new Error(`Authorization failed: ${query.error} ${query.errorDescription ?? ''}`.trim());
    if (!query.code || query.state !== state) throw new Error('Authorization response is invalid.');

    return this.exchange({
      grant_type: 'authorization_code',
      code: query.code,
      redirect_uri: REDIRECT_URI,
      code_verifier: verifier
    }, signal);
  }

  refresh(refreshToken: string, signal?: AbortSignal): Promise<TokenResponse> {
    return this.exchange({ grant_type: 'refresh_token', refresh_token: refreshToken }, signal);
  }

  async revoke(refreshToken: string, signal?: AbortSignal): Promise<void> {
    try {
      await fetch(new URL('connect/revocation', this.environment.authority), {
        method: 'POST',
        body: new URLSearchParams({ client_id: CLIENT_ID, token: refreshToken, token_type_hint: 'refresh_token' }),
        signal
      });
    } catch (error) {
      console.error(`Refresh token revocation failed: ${(error as Error).message}`);
    }
  }

  private async waitForCallback(authorizeUrl: string, signal?: AbortSignal): Promise<CallbackQuery> {
    const { promise: received, resolve: settle } = Promise.withResolvers<CallbackQuery>();

    const server = createServer((request, response) => {
      const url = new URL(request.url ?? '/', REDIRECT_URI);
      if (url.pathname.replace(/\/$/, '') !== '/callback') {
        response.writeHead(404).end();
        return;
      }
      const error = url.searchParams.get('error');
      const message = error ? `Sign-in failed: ${error}` : 'Signed in to Sigook. You can close this window.';
      response.writeHead(200, { 'Content-Type': 'text/html; charset=utf-8', Connection: 'close' });
      response.end(`<!doctype html><html><body style="font-family:sans-serif;padding:2rem"><h2>${escapeHtml(message)}</h2></body></html>`);
      settle({
        code: url.searchParams.get('code'),
        state: url.searchParams.get('state'),
        error,
        errorDescription: url.searchParams.get('error_description')
      });
    });

    await listen(server);
    const combined = AbortSignal.any([AbortSignal.timeout(LOGIN_TIMEOUT_MS), ...(signal ? [signal] : [])]);
    try {
      console.error(`Opening browser for Sigook sign-in (${this.environment.name})`);
      openBrowser(authorizeUrl);
      return await new Promise<CallbackQuery>((resolve, reject) => {
        combined.addEventListener('abort', () => reject(new Error('Sign-in timed out or was cancelled.')), { once: true });
        void received.then(resolve);
      });
    } finally {
      server.close();
      server.closeAllConnections();
    }
  }

  private async exchange(form: Record<string, string>, signal?: AbortSignal): Promise<TokenResponse> {
    const response = await fetch(new URL('connect/token', this.environment.authority), {
      method: 'POST',
      body: new URLSearchParams({ ...form, client_id: CLIENT_ID }),
      signal
    });
    const body = await response.text();
    let tokens: TokenResponse;
    try {
      tokens = JSON.parse(body) as TokenResponse;
    } catch {
      tokens = { error: 'invalid_response' };
    }
    if (!response.ok || tokens.error)
      throw new Error(`Token request failed (${response.status}): ${tokens.error ?? ''} ${tokens.error_description ?? ''}`.trim());
    return tokens;
  }

  private authorizeUrl(challenge: string, state: string): string {
    const query = Object.entries({
      client_id: CLIENT_ID,
      response_type: 'code',
      redirect_uri: REDIRECT_URI,
      scope: SCOPE,
      code_challenge: challenge,
      code_challenge_method: 'S256',
      state
    }).map(([key, value]) => `${encodeURIComponent(key)}=${encodeURIComponent(value)}`).join('&');
    return new URL(`connect/authorize?${query}`, this.environment.authority).toString();
  }
}
