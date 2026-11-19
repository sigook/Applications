import { describe, expect, it, vi } from 'vitest';
import { mount } from '@vue/test-utils';
import Buefy from 'buefy';
import { AxiosError, type AxiosResponse, type InternalAxiosRequestConfig } from 'axios';
import CreatePassword from '@/app/pages/auth/CreatePassword.vue';
import { createPassword } from '@/app/security/authApi';
import { createTestRouter } from '@/testing/mountWithPlugins';

vi.mock('@/app/security/authApi', () => ({ createPassword: vi.fn() }));

async function settle() {
  for (let i = 0; i < 10; i++) await new Promise((resolve) => { setTimeout(resolve, 0); });
}

function badRequest(data: unknown, status = 400): AxiosError {
  const config = { headers: {} } as InternalAxiosRequestConfig;
  const response: AxiosResponse = { data, status, statusText: 'ERR', headers: {}, config };
  return new AxiosError('failed', 'ERR_BAD_REQUEST', config, null, response);
}

async function setup(path = '/create-password?id=user-1&token=abc') {
  const router = createTestRouter();
  await router.push(path);
  await router.isReady();
  return mount(CreatePassword, { attachTo: document.body, global: { plugins: [Buefy, router] } });
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

async function save(wrapper: Wrapper, password: string, confirm = password) {
  const inputs = wrapper.findAll('input[type="password"]');
  await inputs[0].setValue(password);
  await inputs[1].setValue(confirm);
  await wrapper.find('form').trigger('submit');
  await settle();
}

function error(wrapper: Wrapper) {
  return wrapper.find('.auth-card__error p').text();
}

describe('CreatePassword', () => {
  it('flags a link without id or token', async () => {
    const wrapper = await setup('/create-password?id=user-1');

    expect(error(wrapper)).toBe('This link is invalid or has expired.');
    expect(wrapper.find('.auth-card__error a').attributes('href')).toBe('/forgot-password');
  });

  it('validates the passwords', async () => {
    const wrapper = await setup();
    await wrapper.find('form').trigger('submit');
    await settle();
    expect(wrapper.findAll('.help.is-danger').map((h) => h.text())).toEqual(['Password is required', 'Confirm password is required']);

    await save(wrapper, '123', '456');
    expect(wrapper.findAll('.help.is-danger').map((h) => h.text())).toEqual(['Min 6 characters', 'Passwords must match']);
    expect(createPassword).not.toHaveBeenCalled();
  });

  it('saves the password and offers to sign in', async () => {
    vi.mocked(createPassword).mockResolvedValue();
    const wrapper = await setup();
    await save(wrapper, 'secret1');

    expect(createPassword).toHaveBeenCalledExactlyOnceWith({ id: 'user-1', token: 'abc', password: 'secret1' });
    expect(wrapper.find('.auth-card__title').text()).toBe('You are all set');
    expect(wrapper.find('form').exists()).toBe(false);
    expect(wrapper.find('a.auth-btn').attributes('href')).toBe('/login');
  });

  it('lists the password policy messages', async () => {
    vi.mocked(createPassword).mockRejectedValue(badRequest({ error: 'password_policy', messages: ['Needs a digit'] }));
    const wrapper = await setup();
    await save(wrapper, 'secret1');

    expect(error(wrapper)).toBe('The new password was rejected.');
    expect(wrapper.findAll('.auth-card__error li').map((li) => li.text())).toEqual(['Needs a digit']);
    expect(wrapper.find('.auth-card__error a').exists()).toBe(false);
  });

  it.each([
    [badRequest({ error: 'password_policy' }), 'The new password was rejected.'],
    [badRequest({}), 'This link is invalid or has expired.'],
    [badRequest({ error: 'invalid_token' }, 500), 'Something went wrong. Please try again.'],
    [new Error('network'), 'Something went wrong. Please try again.'],
  ])('maps the failure %#', async (failure, message) => {
    vi.mocked(createPassword).mockRejectedValue(failure);
    const wrapper = await setup();
    await save(wrapper, 'secret1');

    expect(error(wrapper)).toBe(message);
    expect(wrapper.findAll('.auth-card__error li')).toHaveLength(0);
    expect(wrapper.find('.auth-card__title').text()).toBe('Create your password');
  });
});
