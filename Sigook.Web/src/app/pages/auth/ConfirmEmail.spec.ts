import { describe, expect, it, vi } from 'vitest';
import { flushPromises, mount } from '@vue/test-utils';
import Buefy from 'buefy';
import { AxiosError, type AxiosResponse, type InternalAxiosRequestConfig } from 'axios';
import ConfirmEmail from '@/app/pages/auth/ConfirmEmail.vue';
import { confirmEmail } from '@/app/security/authApi';
import { createTestRouter } from '@/testing/mountWithPlugins';

vi.mock('@/app/security/authApi', () => ({ confirmEmail: vi.fn() }));

function badRequest(data: unknown, status = 400): AxiosError {
  const config = { headers: {} } as InternalAxiosRequestConfig;
  const response: AxiosResponse = { data, status, statusText: 'ERR', headers: {}, config };
  return new AxiosError('failed', 'ERR_BAD_REQUEST', config, null, response);
}

async function setup(path = '/confirm-email?id=user-1&token=abc') {
  const router = createTestRouter();
  await router.push(path);
  await router.isReady();
  return mount(ConfirmEmail, { attachTo: document.body, global: { plugins: [Buefy, router] } });
}

describe('ConfirmEmail', () => {
  it('shows a loading state while confirming', async () => {
    vi.mocked(confirmEmail).mockReturnValue(new Promise(() => undefined));
    const wrapper = await setup();

    expect(wrapper.find('.auth-card__title').text()).toBe('Confirming your account');
  });

  it('confirms the account', async () => {
    vi.mocked(confirmEmail).mockResolvedValue();
    const wrapper = await setup();
    await flushPromises();

    expect(confirmEmail).toHaveBeenCalledExactlyOnceWith({ id: 'user-1', token: 'abc' });
    expect(wrapper.find('.auth-card__title').text()).toBe('Account confirmed');
    expect(wrapper.find('a.auth-btn').attributes('href')).toBe('/login');
  });

  it.each(['/confirm-email', '/confirm-email?id=user-1', '/confirm-email?id=&token=abc'])('rejects the incomplete link %s', async (path) => {
    const wrapper = await setup(path);
    await flushPromises();

    expect(confirmEmail).not.toHaveBeenCalled();
    expect(wrapper.find('.auth-card__title').text()).toBe('We could not confirm your account');
    expect(wrapper.find('.auth-card__subtitle').text()).toBe('This link is invalid or has expired.');
    expect(wrapper.find('.auth-card__links a').attributes('href')).toBe('/login');
  });

  it.each([
    [badRequest({ error: 'inactive_user' }), 'Your account is inactive. Please contact support.'],
    [badRequest({}), 'This link is invalid or has expired.'],
    [badRequest({ error: 'invalid_token' }, 500), 'Something went wrong. Please try again.'],
    [new Error('network'), 'Something went wrong. Please try again.'],
  ])('maps the failure %#', async (failure, message) => {
    vi.mocked(confirmEmail).mockRejectedValue(failure);
    const wrapper = await setup();
    await flushPromises();

    expect(wrapper.find('.auth-card__subtitle').text()).toBe(message);
  });
});
