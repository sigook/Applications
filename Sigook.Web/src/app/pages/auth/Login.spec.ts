import { beforeEach, describe, expect, it, vi } from 'vitest';
import { mount } from '@vue/test-utils';
import Buefy from 'buefy';
import { AxiosError, type AxiosResponse, type InternalAxiosRequestConfig } from 'axios';
import Login from '@/app/pages/auth/Login.vue';
import { resendConfirmationLink } from '@/app/security/authApi';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { createTestRouter } from '@/testing/mountWithPlugins';
import type { UserProfile } from '@/shared/types/security';

const security = vi.hoisted(() => ({
  user: null as UserProfile | null,
  userRoles: [] as string[],
  signInWithPassword: vi.fn<(email: string, password: string) => Promise<void>>(),
  signInWithMicrosoft: vi.fn<() => Promise<void>>(),
}));

vi.mock('@/app/stores/security', () => ({ useSecurityStore: () => security }));
vi.mock('@/app/security/authApi', () => ({ resendConfirmationLink: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));

async function settle() {
  for (let i = 0; i < 10; i++) await new Promise((resolve) => { setTimeout(resolve, 0); });
}

function badRequest(data: unknown, status = 400): AxiosError {
  const config = { headers: {} } as InternalAxiosRequestConfig;
  const response: AxiosResponse = { data, status, statusText: 'ERR', headers: {}, config };
  return new AxiosError('failed', 'ERR_BAD_REQUEST', config, null, response);
}

async function setup(path = '/login') {
  const router = createTestRouter();
  await router.push(path);
  await router.isReady();
  const push = vi.spyOn(router, 'push');
  const replace = vi.spyOn(router, 'replace');
  const wrapper = mount(Login, { attachTo: document.body, global: { plugins: [Buefy, router] } });
  return { wrapper, push, replace };
}

type Wrapper = Awaited<ReturnType<typeof setup>>['wrapper'];

async function signIn(wrapper: Wrapper, email = 'ann@acme.com', password = 'secret') {
  await wrapper.find('input[type="email"]').setValue(email);
  await wrapper.find('input[type="password"]').setValue(password);
  await wrapper.find('form').trigger('submit');
  await settle();
}

function loginError(wrapper: Wrapper) {
  return wrapper.find('.auth-card__error p').text();
}

describe('Login', () => {
  beforeEach(() => {
    security.user = null;
    security.userRoles = [];
  });

  it('validates the email and password', async () => {
    const { wrapper } = await setup();
    await wrapper.find('form').trigger('submit');
    await settle();

    expect(wrapper.findAll('.help.is-danger').map((h) => h.text())).toEqual(['Email is required', 'Password is required']);
    await signIn(wrapper, 'bad', 'x');
    expect(wrapper.find('.help.is-danger').text()).toBe('Invalid email');
    expect(security.signInWithPassword).not.toHaveBeenCalled();
  });

  it('signs in and goes to the role home page', async () => {
    security.signInWithPassword.mockImplementation(() => {
      security.userRoles = ['recruiting'];
      return Promise.resolve();
    });
    const { wrapper, push } = await setup();
    await signIn(wrapper);

    expect(security.signInWithPassword).toHaveBeenCalledExactlyOnceWith('ann@acme.com', 'secret');
    expect(push).toHaveBeenCalledWith('/recruiting/weekly-board');
  });

  it('honours a relative return url', async () => {
    security.signInWithPassword.mockResolvedValue();
    const { wrapper, push } = await setup('/login?returnUrl=/worker-profile');
    await signIn(wrapper);

    expect(push).toHaveBeenCalledWith('/worker-profile');
  });

  it('ignores an absolute return url', async () => {
    security.signInWithPassword.mockResolvedValue();
    const { wrapper, push } = await setup('/login?returnUrl=https://evil.com');
    await signIn(wrapper);

    expect(push).toHaveBeenCalledWith('/unauthorized');
  });

  it('redirects an already signed-in user', async () => {
    security.user = { token_type: 'Bearer', access_token: 't', profile: { email: 'a@b.com', role: 'admin', sub: 'u1', name: 'Ann' } };
    security.userRoles = ['admin'];
    const { replace } = await setup();

    expect(replace).toHaveBeenCalledWith('/recruiting/requests');
  });

  it('shows the error code from the query string', async () => {
    const { wrapper } = await setup('/login?error=invalid_user');

    expect(loginError(wrapper)).toBe('Your Microsoft 365 account is not allowed to sign in.');
    expect(wrapper.find('.auth-card__error a').exists()).toBe(false);
  });

  it.each([
    [{ error: 'invalid_grant', error_description: 'invalid_credentials' }, 'Invalid email or password.'],
    [{ error: 'locked_out' }, 'Too many failed attempts. Try again in a few minutes.'],
    [{}, 'Something went wrong. Please try again.'],
  ])('maps the token error %o', async (data, message) => {
    security.signInWithPassword.mockRejectedValue(badRequest(data));
    const { wrapper, push } = await setup();
    await signIn(wrapper);

    expect(loginError(wrapper)).toBe(message);
    expect(push).not.toHaveBeenCalled();
  });

  it('shows the generic error for non 400 failures', async () => {
    security.signInWithPassword.mockRejectedValue(badRequest({ error: 'invalid_credentials' }, 500));
    const { wrapper } = await setup();
    await signIn(wrapper);

    expect(loginError(wrapper)).toBe('Something went wrong. Please try again.');
  });

  it('shows the generic error for unexpected failures', async () => {
    security.signInWithPassword.mockRejectedValue(new Error('network'));
    const { wrapper } = await setup();
    await signIn(wrapper);

    expect(loginError(wrapper)).toBe('Something went wrong. Please try again.');
  });

  it('resends the confirmation email when the email is not confirmed', async () => {
    security.signInWithPassword.mockRejectedValue(badRequest({ error_description: 'email_not_confirmed' }));
    vi.mocked(resendConfirmationLink).mockResolvedValue();
    const { wrapper } = await setup();
    await signIn(wrapper);

    expect(loginError(wrapper)).toBe('Your email address has not been confirmed yet.');
    await wrapper.find('.auth-card__error a').trigger('click');
    await settle();
    expect(resendConfirmationLink).toHaveBeenCalledWith('ann@acme.com');
    expect(showAlertSuccess).toHaveBeenCalledWith('Confirmation email sent. Check your inbox.');
  });

  it('warns when the confirmation email cannot be sent', async () => {
    vi.mocked(resendConfirmationLink).mockRejectedValue(new Error('down'));
    const { wrapper } = await setup('/login?error=email_not_confirmed');
    await wrapper.find('.auth-card__error a').trigger('click');
    await settle();

    expect(showAlertError).toHaveBeenCalledWith('Could not send the confirmation email. Try again later.');
  });

  it('clears the previous error on a new attempt', async () => {
    security.signInWithPassword.mockResolvedValue();
    const { wrapper } = await setup('/login?error=locked_out');
    await signIn(wrapper);

    expect(wrapper.find('.auth-card__error').exists()).toBe(false);
  });

  it('signs in with Microsoft', async () => {
    security.signInWithMicrosoft.mockResolvedValue();
    const { wrapper } = await setup();
    const button = wrapper.findAll('button').find((b) => b.text().includes('Continue with Microsoft 365'));
    await button?.trigger('click');

    expect(security.signInWithMicrosoft).toHaveBeenCalledOnce();
  });
});
