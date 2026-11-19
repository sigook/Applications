import { afterEach, describe, expect, it, vi } from 'vitest';
import { AxiosError, type AxiosResponse, type InternalAxiosRequestConfig } from 'axios';
import ForgotPassword from '@/app/pages/auth/ForgotPassword.vue';
import { requestPasswordResetCode, resetPasswordWithCode } from '@/app/security/authApi';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/app/security/authApi', () => ({ requestPasswordResetCode: vi.fn(), resetPasswordWithCode: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));

async function settle() {
  for (let i = 0; i < 10; i++) await new Promise((resolve) => { setTimeout(resolve, 0); });
}

function badRequest(data: unknown, status = 400): AxiosError {
  const config = { headers: {} } as InternalAxiosRequestConfig;
  const response: AxiosResponse = { data, status, statusText: 'ERR', headers: {}, config };
  return new AxiosError('failed', 'ERR_BAD_REQUEST', config, null, response);
}

function setup() {
  vi.useFakeTimers({ toFake: ['setInterval', 'clearInterval'] });
  const wrapper = mountWithPlugins(ForgotPassword);
  const push = vi.spyOn(wrapper.vm.$router, 'push');
  return { wrapper, push };
}

type Wrapper = ReturnType<typeof setup>['wrapper'];

function dangerMessages(wrapper: Wrapper) {
  return wrapper.findAll('.help.is-danger').map((h) => h.text());
}

async function requestCode(wrapper: Wrapper, email = 'ann@acme.com') {
  vi.mocked(requestPasswordResetCode).mockResolvedValue();
  await wrapper.find('input[type="email"]').setValue(email);
  await wrapper.find('form').trigger('submit');
  await settle();
}

async function reset(wrapper: Wrapper, code: string, password: string, confirm = password) {
  await wrapper.find('input[placeholder="123456"]').setValue(code);
  const passwords = wrapper.findAll('input[type="password"]');
  await passwords[0].setValue(password);
  await passwords[1].setValue(confirm);
  await wrapper.find('form').trigger('submit');
  await settle();
}

function resendLink(wrapper: Wrapper) {
  return wrapper.find('.auth-card__links a[href="#"]');
}

describe('ForgotPassword', () => {
  afterEach(() => {
    vi.useRealTimers();
  });

  it('requires a valid email to request a code', async () => {
    const { wrapper } = setup();
    await wrapper.find('form').trigger('submit');
    await settle();
    expect(dangerMessages(wrapper)).toEqual(['Email is required']);

    await wrapper.find('input[type="email"]').setValue('nope');
    await wrapper.find('form').trigger('submit');
    await settle();
    expect(dangerMessages(wrapper)).toEqual(['Invalid email']);
    expect(requestPasswordResetCode).not.toHaveBeenCalled();
  });

  it('requests a code and moves to the reset step', async () => {
    const { wrapper } = setup();
    await requestCode(wrapper);

    expect(requestPasswordResetCode).toHaveBeenCalledExactlyOnceWith('ann@acme.com');
    expect(wrapper.find('.auth-card__title').text()).toBe('Check your inbox');
    expect(wrapper.find('strong').text()).toBe('ann@acme.com');
    expect(dangerMessages(wrapper)).toEqual([]);
    expect(resendLink(wrapper).text()).toBe('Resend code (60s)');
  });

  it('stays on the first step when the code cannot be sent', async () => {
    const { wrapper } = setup();
    vi.mocked(requestPasswordResetCode).mockRejectedValue(new Error('down'));
    await wrapper.find('input[type="email"]').setValue('ann@acme.com');
    await wrapper.find('form').trigger('submit');
    await settle();

    expect(showAlertError).toHaveBeenCalledWith('Could not send the code. Try again later.');
    expect(wrapper.find('.auth-card__title').text()).toBe('Reset your password');
  });

  it('counts down and then allows resending the code', async () => {
    const { wrapper } = setup();
    await requestCode(wrapper);
    await resendLink(wrapper).trigger('click');
    await settle();
    expect(requestPasswordResetCode).toHaveBeenCalledTimes(1);

    vi.advanceTimersByTime(1000);
    await settle();
    expect(resendLink(wrapper).text()).toBe('Resend code (59s)');
    vi.advanceTimersByTime(59000);
    await settle();
    expect(resendLink(wrapper).text()).toBe('Resend code');
    expect(resendLink(wrapper).classes()).not.toContain('auth-card__link--disabled');

    await resendLink(wrapper).trigger('click');
    await settle();
    expect(requestPasswordResetCode).toHaveBeenCalledTimes(2);
    expect(showAlertSuccess).toHaveBeenCalledWith('A new code was sent. Check your inbox.');
    expect(resendLink(wrapper).text()).toBe('Resend code (60s)');
  });

  it('warns when resending fails', async () => {
    const { wrapper } = setup();
    await requestCode(wrapper);
    vi.advanceTimersByTime(60000);
    await settle();
    vi.mocked(requestPasswordResetCode).mockRejectedValue(new Error('down'));
    await resendLink(wrapper).trigger('click');
    await settle();

    expect(showAlertError).toHaveBeenCalledWith('Could not send the code. Try again later.');
  });

  it('validates the code and passwords', async () => {
    const { wrapper } = setup();
    await requestCode(wrapper);
    await wrapper.find('form').trigger('submit');
    await settle();
    expect(dangerMessages(wrapper)).toEqual(['Code is required', 'New password is required', 'Confirm password is required']);

    await reset(wrapper, '12ab', '123', '456');
    expect(dangerMessages(wrapper)).toEqual(['Code must be 6 digits', 'Min 6 characters', 'Passwords must match']);
    expect(resetPasswordWithCode).not.toHaveBeenCalled();
  });

  it('resets the password and goes to sign in', async () => {
    vi.mocked(resetPasswordWithCode).mockResolvedValue();
    const { wrapper, push } = setup();
    await requestCode(wrapper);
    await reset(wrapper, '123456', 'secret1');

    expect(resetPasswordWithCode).toHaveBeenCalledExactlyOnceWith({ email: 'ann@acme.com', code: '123456', newPassword: 'secret1' });
    expect(showAlertSuccess).toHaveBeenCalledWith('Password updated. Sign in with your new password.');
    expect(push).toHaveBeenCalledWith('/login');
  });

  it('lists the password policy messages', async () => {
    vi.mocked(resetPasswordWithCode).mockRejectedValue(badRequest({ error: 'password_policy', messages: ['Needs a digit', 'Needs a symbol'] }));
    const { wrapper, push } = setup();
    await requestCode(wrapper);
    await reset(wrapper, '123456', 'secret1');

    expect(wrapper.find('.auth-card__error p').text()).toBe('The new password was rejected.');
    expect(wrapper.findAll('.auth-card__error li').map((li) => li.text())).toEqual(['Needs a digit', 'Needs a symbol']);
    expect(push).not.toHaveBeenCalled();
  });

  it.each([
    [badRequest({ error: 'code_expired' }), 'The code has expired. Request a new one.'],
    [badRequest({ error: 'password_policy' }), 'The new password was rejected.'],
    [badRequest({}), 'Something went wrong. Please try again.'],
    [badRequest({ error: 'invalid_code' }, 500), 'Something went wrong. Please try again.'],
    [new Error('network'), 'Something went wrong. Please try again.'],
  ])('maps the reset failure %#', async (error, message) => {
    vi.mocked(resetPasswordWithCode).mockRejectedValue(error);
    const { wrapper } = setup();
    await requestCode(wrapper);
    await reset(wrapper, '123456', 'secret1');

    expect(wrapper.find('.auth-card__error p').text()).toBe(message);
    expect(wrapper.findAll('.auth-card__error li')).toHaveLength(0);
  });

  it('stops the countdown on unmount', async () => {
    const { wrapper } = setup();
    await requestCode(wrapper);
    wrapper.unmount();

    expect(vi.getTimerCount()).toBe(0);
  });
});
