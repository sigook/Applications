import type { SigookEnvironment } from '../config/environment.js';
import type { PkceLoginFlow, TokenResponse } from './pkceLoginFlow.js';
import type { StoredTokens, TokenStore } from './tokenStore.js';

const EXPIRY_MARGIN_MS = 60 * 1000;

export class SigookSession {
  private gate: Promise<unknown> = Promise.resolve();

  constructor(
    private readonly store: TokenStore,
    private readonly loginFlow: PkceLoginFlow,
    readonly environment: SigookEnvironment
  ) {}

  get isSignedIn(): boolean {
    return this.store.load() !== null;
  }

  async login(signal?: AbortSignal): Promise<StoredTokens> {
    const tokens = this.toStored(await this.loginFlow.login(signal));
    this.store.save(tokens);
    return tokens;
  }

  async logout(signal?: AbortSignal): Promise<void> {
    const tokens = this.store.load();
    if (tokens?.refreshToken) await this.loginFlow.revoke(tokens.refreshToken, signal);
    this.store.clear();
  }

  getAccessToken(signal?: AbortSignal): Promise<string> {
    const result = this.gate.then(() => this.currentToken(signal));
    this.gate = result.catch(() => undefined);
    return result;
  }

  private async currentToken(signal?: AbortSignal): Promise<string> {
    const tokens = this.store.load();
    if (!tokens) throw this.notSignedIn();
    if (new Date(tokens.expiresAt).getTime() - EXPIRY_MARGIN_MS > Date.now()) return tokens.accessToken;
    if (!tokens.refreshToken) {
      this.store.clear();
      throw this.notSignedIn();
    }
    let refreshed: StoredTokens;
    try {
      refreshed = this.toStored(await this.loginFlow.refresh(tokens.refreshToken, signal));
    } catch {
      this.store.clear();
      throw this.notSignedIn();
    }
    this.store.save(refreshed);
    return refreshed.accessToken;
  }

  private toStored(response: TokenResponse): StoredTokens {
    return {
      accessToken: response.access_token ?? '',
      refreshToken: response.refresh_token,
      expiresAt: new Date(Date.now() + (response.expires_in ?? 0) * 1000).toISOString(),
      environment: this.environment.name
    };
  }

  private notSignedIn(): Error {
    return new Error(`Not signed in to Sigook (${this.environment.name}). Call the sigook_login tool first.`);
  }
}
