import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { AxiosError, AxiosHeaders, type AxiosResponse, type InternalAxiosRequestConfig } from 'axios';
import http, { api } from '@/app/security/apiService';
import type { UserProfile } from '@/shared/types/security';

const security = vi.hoisted(() => ({
  getUser: vi.fn<() => Promise<UserProfile | null>>(),
  silentSignin: vi.fn<() => Promise<UserProfile | null>>(),
  signOut: vi.fn<() => Promise<void>>(),
}));

vi.mock('@/app/stores', () => ({ default: {} }));
vi.mock('@/app/stores/security', () => ({ useSecurityStore: () => security }));

type Adapter = (config: InternalAxiosRequestConfig) => Promise<AxiosResponse>;

const adapter = vi.fn<Adapter>();
const originalAdapter = http.defaults.adapter;

function user(token: string): UserProfile {
  return { token_type: 'Bearer', access_token: token, profile: { email: 'a@b.com', role: 'admin', sub: 'u1', name: 'Ann' } };
}

function ok(config: InternalAxiosRequestConfig, data: unknown = { ok: true }): AxiosResponse {
  return { data, status: 200, statusText: 'OK', headers: {}, config };
}

function fail(config: InternalAxiosRequestConfig, status: number, data: unknown = 'error'): AxiosError {
  const response: AxiosResponse = { data, status, statusText: 'ERR', headers: {}, config };
  return new AxiosError(`Request failed with status code ${status}`, 'ERR_BAD_RESPONSE', config, null, response);
}

function sentHeaders(call: number): AxiosHeaders {
  const config = adapter.mock.calls.at(call)?.[0];
  if (!config) throw new Error(`request ${call} was not sent`);
  return AxiosHeaders.from(config.headers);
}

const assign = vi.fn<(url: string) => void>();
const alertMock = vi.fn<(message: string) => void>();

beforeEach(() => {
  vi.clearAllMocks();
  localStorage.clear();
  http.defaults.adapter = adapter;
  security.getUser.mockResolvedValue(null);
  security.signOut.mockResolvedValue();
  vi.stubGlobal('alert', alertMock);
  vi.spyOn(window, 'location', 'get').mockReturnValue({
    ...window.location,
    pathname: '/recruiting/requests',
    search: '?status=open',
    assign,
  });
});

afterEach(() => {
  http.defaults.adapter = originalAdapter;
});

describe('request interceptor', () => {
  it('sends the bearer token of the signed-in user', async () => {
    security.getUser.mockResolvedValue(user('abc'));
    adapter.mockImplementation((config) => Promise.resolve(ok(config)));

    await api.get('/api/x');

    expect(sentHeaders(0).get('Authorization')).toBe('Bearer abc');
  });

  it('sends no Authorization header when nobody is signed in', async () => {
    adapter.mockImplementation((config) => Promise.resolve(ok(config)));

    await api.get('/api/x');

    expect(sentHeaders(0).has('Authorization')).toBe(false);
  });

  it.each([
    [null, 'en-US'],
    ['en', 'en-US'],
    ['fr', 'fr'],
    ['es', 'es'],
  ])('language %j -> accept-language %s', async (language, expected) => {
    if (language) localStorage.setItem('language', language);
    adapter.mockImplementation((config) => Promise.resolve(ok(config)));

    await api.get('/api/x');

    expect(sentHeaders(0).get('accept-language')).toBe(expected);
  });

  it('serializes params with qs, encoding values only', () => {
    expect(http.getUri({ url: '/api/x', params: { ids: ['a b', 'c'], filter: { name: 'x&y' } } })).toContain(
      '/api/x?ids[0]=a%20b&ids[1]=c&filter[name]=x%26y',
    );
  });
});

describe('api helpers', () => {
  beforeEach(() => {
    adapter.mockImplementation((config) => Promise.resolve(ok(config, { method: config.method, body: config.data })));
  });

  it('get returns the response data', async () => {
    await expect(api.get('/api/x')).resolves.toEqual({ method: 'get', body: undefined });
    expect(adapter.mock.calls[0]?.[0].url).toBe('/api/x');
  });

  it.each([['post'], ['put'], ['patch']] as const)('%s sends the JSON body', async (method) => {
    const result = await api[method]<{ method: string; body: string }>('/api/x', { a: 1 });

    expect(result.method).toBe(method);
    expect(JSON.parse(result.body)).toEqual({ a: 1 });
  });

  it('del issues a DELETE', async () => {
    await expect(api.del('/api/x/1')).resolves.toEqual({ method: 'delete', body: undefined });
  });
});

describe('response interceptor', () => {
  it('retries once with a refreshed token after a 401', async () => {
    security.getUser.mockResolvedValueOnce(user('old')).mockResolvedValue(user('new'));
    security.silentSignin.mockResolvedValue(user('new'));
    localStorage.setItem('language', 'fr');
    const tokens: unknown[] = [];
    adapter
      .mockImplementationOnce((config) => {
        tokens.push(AxiosHeaders.from(config.headers).get('Authorization'));
        return Promise.reject(fail(config, 401));
      })
      .mockImplementation((config) => {
        tokens.push(AxiosHeaders.from(config.headers).get('Authorization'));
        return Promise.resolve(ok(config, 'second'));
      });

    await expect(api.get('/api/x')).resolves.toBe('second');

    expect(security.silentSignin).toHaveBeenCalledOnce();
    expect(adapter).toHaveBeenCalledTimes(2);
    expect(tokens).toEqual(['Bearer old', 'Bearer new']);
    expect(sentHeaders(1).get('accept-language')).toBe('fr');
    expect(assign).not.toHaveBeenCalled();
  });

  it('signs out and redirects to login when the retried request is also 401', async () => {
    security.getUser.mockResolvedValue(user('t'));
    security.silentSignin.mockResolvedValue(user('t'));
    adapter.mockImplementation((config) => Promise.reject(fail(config, 401, 'unauthorized')));

    await expect(api.get('/api/x')).rejects.toMatchObject({ status: 401 });

    expect(adapter).toHaveBeenCalledTimes(2);
    expect(security.signOut).toHaveBeenCalled();
    expect(assign).toHaveBeenCalledWith(`/login?returnUrl=${encodeURIComponent('/recruiting/requests?status=open')}`);
  });

  it('signs out and redirects when the silent sign-in fails', async () => {
    const renewError = new Error('login_required');
    security.silentSignin.mockRejectedValue(renewError);
    adapter.mockImplementation((config) => Promise.reject(fail(config, 401)));

    await expect(api.get('/api/x')).rejects.toBe(renewError);

    expect(adapter).toHaveBeenCalledOnce();
    expect(security.signOut).toHaveBeenCalledOnce();
    expect(assign).toHaveBeenCalledOnce();
  });

  it('still redirects when signing out fails', async () => {
    security.silentSignin.mockRejectedValue(new Error('login_required'));
    security.signOut.mockRejectedValue(new Error('signout failed'));
    adapter.mockImplementation((config) => Promise.reject(fail(config, 401)));

    await expect(api.get('/api/x')).rejects.toThrow('login_required');

    expect(assign).toHaveBeenCalledOnce();
  });

  it('alerts on 403 and rejects with the response', async () => {
    adapter.mockImplementation((config) => Promise.reject(fail(config, 403, 'forbidden')));

    await expect(api.get('/api/x')).rejects.toMatchObject({ status: 403, data: 'forbidden' });

    expect(alertMock).toHaveBeenCalledWith('You are not authorized to perform this action');
    expect(security.silentSignin).not.toHaveBeenCalled();
  });

  it('alerts on 500 and rejects with the response', async () => {
    adapter.mockImplementation((config) => Promise.reject(fail(config, 500, { message: 'crash' })));

    await expect(api.get('/api/x')).rejects.toMatchObject({ status: 500, data: { message: 'crash' } });

    expect(alertMock).toHaveBeenCalledWith('Oops something was wrong please try again later');
  });

  it('replaces a blob 500 body with a readable message', async () => {
    adapter.mockImplementation((config) => Promise.reject(fail(config, 500, new Blob(['x']))));

    await expect(api.get('/api/report', { responseType: 'blob' })).rejects.toEqual({
      data: 'An error occurred please try again, if the error persists try later one of our engineers will solve it soon',
    });
  });

  it('rejects other statuses with the response and no alert', async () => {
    adapter.mockImplementation((config) => Promise.reject(fail(config, 400, { email: 'Invalid' })));

    await expect(api.post('/api/x', {})).rejects.toMatchObject({ status: 400, data: { email: 'Invalid' } });

    expect(alertMock).not.toHaveBeenCalled();
  });

  it('wraps network errors without a response into { data: message }', async () => {
    adapter.mockImplementation((config) => Promise.reject(new AxiosError('Network Error', 'ERR_NETWORK', config)));

    await expect(api.get('/api/x')).rejects.toEqual({ data: 'Network Error' });

    expect(alertMock).not.toHaveBeenCalled();
  });
});
