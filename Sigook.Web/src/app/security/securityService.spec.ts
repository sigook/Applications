import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { Log, UserManager, WebStorageStateStore, type User } from 'oidc-client-ts';
import mgr, { signInWithMicrosoft, signInWithPassword } from '@/app/security/securityService';
import { fetchUserInfo, requestPasswordToken } from '@/app/security/authApi';

const oidc = vi.hoisted(() => {
  const instance = {
    storeUser: vi.fn(),
    signinRedirect: vi.fn(),
    events: { load: vi.fn() },
  };
  return { instance };
});

vi.mock('oidc-client-ts', () => ({
  UserManager: vi.fn(function () {
    return oidc.instance;
  }),
  WebStorageStateStore: vi.fn(function (this: { args: unknown }, args: unknown) {
    this.args = args;
  }),
  User: vi.fn(function (this: Record<string, unknown>, settings: Record<string, unknown>) {
    Object.assign(this, settings);
  }),
  Log: { setLogger: vi.fn(), setLevel: vi.fn(), ERROR: 1 },
}));

vi.mock('@/app/security/authApi', () => ({ fetchUserInfo: vi.fn(), requestPasswordToken: vi.fn() }));

interface ManagerSettings {
  authority: string;
  client_id: string;
  redirect_uri: string;
  response_type: string;
  scope: string;
  post_logout_redirect_uri: string;
  automaticSilentRenew: boolean;
  silent_redirect_uri: string;
  loadUserInfo: boolean;
  userStore: unknown;
}

const constructorSettings = vi.mocked(UserManager).mock.calls[0]?.[0] as unknown as ManagerSettings;
const storeArgs = vi.mocked(WebStorageStateStore).mock.calls[0]?.[0];
const logSetLoggerCalls = vi.mocked(Log.setLogger).mock.calls.length;
const logSetLevelArgs = vi.mocked(Log.setLevel).mock.calls[0];

describe('securityService manager', () => {
  it('exports the configured UserManager', () => {
    expect(mgr).toBe(oidc.instance);
  });

  it('uses the authorization code flow with the app callbacks', () => {
    const origin = window.location.origin;

    expect(constructorSettings).toMatchObject({
      response_type: 'code',
      scope: 'openid profile api1 roles offline_access',
      redirect_uri: `${origin}/callback`,
      post_logout_redirect_uri: `${origin}/`,
      silent_redirect_uri: `${origin}/silent-refresh`,
      automaticSilentRenew: true,
      loadUserInfo: true,
    });
  });

  it('persists the user in localStorage', () => {
    expect(storeArgs).toEqual({ store: window.localStorage });
    expect(constructorSettings.userStore).toBeInstanceOf(WebStorageStateStore);
  });

  it('logs only errors outside production', () => {
    expect(logSetLoggerCalls).toBe(1);
    expect(logSetLevelArgs).toEqual([Log.ERROR]);
  });
});

describe('signInWithPassword', () => {
  beforeEach(() => {
    vi.useFakeTimers();
    vi.setSystemTime(new Date('2026-05-01T12:00:00.500Z'));
    vi.clearAllMocks();
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it('builds a user from the password tokens and loads it', async () => {
    vi.mocked(requestPasswordToken).mockResolvedValue({
      access_token: 'access',
      refresh_token: 'refresh',
      token_type: 'Bearer',
      scope: 'openid',
      expires_in: 3600,
    });
    const profile = { sub: 'u1', email: 'a@b.com', name: 'Ann', role: 'worker' };
    vi.mocked(fetchUserInfo).mockResolvedValue(profile);

    await signInWithPassword('a@b.com', 'secret');

    expect(requestPasswordToken).toHaveBeenCalledWith('a@b.com', 'secret');
    expect(fetchUserInfo).toHaveBeenCalledWith('Bearer', 'access');
    const stored = oidc.instance.storeUser.mock.calls[0]?.[0] as User;
    expect(stored).toMatchObject({
      access_token: 'access',
      refresh_token: 'refresh',
      token_type: 'Bearer',
      scope: 'openid',
      expires_at: Math.floor(Date.parse('2026-05-01T12:00:00.500Z') / 1000) + 3600,
      profile,
    });
    expect(oidc.instance.events.load).toHaveBeenCalledWith(stored);
  });

  it('does not store anything when the token request fails', async () => {
    vi.mocked(requestPasswordToken).mockRejectedValue(new Error('invalid_grant'));

    await expect(signInWithPassword('a@b.com', 'bad')).rejects.toThrow('invalid_grant');
    expect(fetchUserInfo).not.toHaveBeenCalled();
    expect(oidc.instance.storeUser).not.toHaveBeenCalled();
    expect(oidc.instance.events.load).not.toHaveBeenCalled();
  });

  it('does not store anything when the profile cannot be loaded', async () => {
    vi.mocked(requestPasswordToken).mockResolvedValue({ access_token: 'a', token_type: 'Bearer', expires_in: 60 });
    vi.mocked(fetchUserInfo).mockRejectedValue(new Error('userinfo failed'));

    await expect(signInWithPassword('a@b.com', 'pw')).rejects.toThrow('userinfo failed');
    expect(oidc.instance.storeUser).not.toHaveBeenCalled();
  });
});

describe('signInWithMicrosoft', () => {
  it('redirects to the external identity provider', async () => {
    oidc.instance.signinRedirect.mockResolvedValue(undefined);

    await signInWithMicrosoft();

    expect(oidc.instance.signinRedirect).toHaveBeenCalledWith({ acr_values: 'idp:oidc' });
  });
});
