import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import WorkerAccountSecurity from '@/modules/worker/profile/components/WorkerAccountSecurity.vue';
import { changeEmail, deactivateAccount, getAccount } from '@/shared/api/accountApi';
import { getUserNotifications, updateUserNotification } from '@/shared/api/userNotificationApi';
import { getDialog } from '@/shared/utils/buefyProgrammatic';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { UserNotificationItem } from '@/shared/types/common';

interface ConfirmConfig {
  title: string;
  onConfirm: () => Promise<void>;
}

const { signOut, confirm } = vi.hoisted(() => ({
  signOut: vi.fn<() => Promise<void>>(),
  confirm: vi.fn<(config: ConfirmConfig) => void>(),
}));

vi.mock('@/app/stores/security', () => ({ useSecurityStore: () => ({ signOut }) }));
vi.mock('@/shared/api/accountApi', () => ({ changeEmail: vi.fn(), getAccount: vi.fn(), deactivateAccount: vi.fn() }));
vi.mock('@/shared/api/userNotificationApi', () => ({ getUserNotifications: vi.fn(), updateUserNotification: vi.fn() }));
vi.mock('@/shared/utils/buefyProgrammatic', () => ({ getDialog: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));

const notification: UserNotificationItem = { id: 1, title: 'New jobs', description: 'Email me new jobs', emailNotification: false, pushNotification: true };

async function setup() {
  const wrapper = mountWithPlugins(WorkerAccountSecurity);
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function button(wrapper: Wrapper, text: string) {
  const found = wrapper.findAll('button').find((b) => b.text() === text);
  if (!found) throw new Error(`${text} not found`);
  return found;
}

async function openEmailModal(wrapper: Wrapper) {
  await button(wrapper, 'Change').trigger('click');
  await flushPromises();
}

function emailInput(name: string) {
  const input = document.body.querySelector<HTMLInputElement>(`input[name="${name}"]`);
  if (!input) throw new Error(`${name} not found`);
  return input;
}

async function type(wrapper: Wrapper, name: string, value: string) {
  await wrapper.find(`input[name="${name}"]`).setValue(value);
}

async function save(wrapper: Wrapper) {
  await button(wrapper, 'Save').trigger('click');
  await flushPromises();
  await new Promise((resolve) => { setTimeout(resolve, 10); });
  await flushPromises();
}

function confirmConfig(): ConfirmConfig {
  const config = confirm.mock.lastCall?.[0];
  if (!config) throw new Error('confirm not opened');
  return config;
}

beforeEach(() => {
  vi.mocked(getAccount).mockResolvedValue({ email: 'john@x.com' } as Awaited<ReturnType<typeof getAccount>>);
  vi.mocked(getUserNotifications).mockResolvedValue([{ ...notification }]);
  vi.mocked(getDialog).mockReturnValue({ confirm } as unknown as ReturnType<typeof getDialog>);
});

afterEach(() => {
  vi.useRealTimers();
});

describe('WorkerAccountSecurity', () => {
  it('shows the login email and the notifications', async () => {
    const wrapper = await setup();
    expect(wrapper.find('.profile-fields dd').text()).toBe('john@x.com');
    const item = wrapper.find('.account-notification');
    expect(item.text()).toContain('New jobs');
    expect(item.text()).toContain('No');
  });

  it('reports failed loads', async () => {
    vi.mocked(getAccount).mockRejectedValue('account down');
    vi.mocked(getUserNotifications).mockRejectedValue('notifications down');
    const wrapper = await setup();
    expect(showAlertError).toHaveBeenCalledWith('account down');
    expect(showAlertError).toHaveBeenCalledWith('notifications down');
    expect(wrapper.find('.profile-fields dd').text()).toBe('—');
    expect(wrapper.find('.account-notification').exists()).toBe(false);
  });

  it('saves a notification preference', async () => {
    vi.mocked(updateUserNotification).mockResolvedValue();
    const wrapper = await setup();
    await wrapper.find('.account-notification input[type="checkbox"]').setValue(true);
    await flushPromises();
    expect(updateUserNotification).toHaveBeenCalledWith(expect.objectContaining({ id: 1, emailNotification: true }));
    expect(wrapper.find('.account-notification').text()).toContain('Yes');
  });

  it('reports a failed notification save', async () => {
    vi.mocked(updateUserNotification).mockRejectedValue('nope');
    const wrapper = await setup();
    await wrapper.find('.account-notification input[type="checkbox"]').setValue(true);
    await flushPromises();
    expect(showAlertError).toHaveBeenCalledWith('nope');
  });

  it('starts the change with the current email', async () => {
    const wrapper = await setup();
    await openEmailModal(wrapper);
    expect(emailInput('email').value).toBe('john@x.com');
    expect(emailInput('confirmNewEmail').value).toBe('');
  });

  it('requires matching emails', async () => {
    const wrapper = await setup();
    await openEmailModal(wrapper);
    await type(wrapper, 'email', 'new@x.com');
    await type(wrapper, 'confirmNewEmail', 'other@x.com');
    await save(wrapper);
    await vi.waitFor(() => { expect(document.body.textContent).toContain('Emails must match'); });
    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly'); });
    expect(changeEmail).not.toHaveBeenCalled();
  });

  it('requires a valid email', async () => {
    const wrapper = await setup();
    await openEmailModal(wrapper);
    await type(wrapper, 'email', 'not-an-email');
    await save(wrapper);
    await vi.waitFor(() => { expect(document.body.textContent).toContain('Invalid email'); });
    expect(document.body.textContent).toContain('Emails must match');
    expect(changeEmail).not.toHaveBeenCalled();
  });

  it('changes the login email', async () => {
    vi.mocked(changeEmail).mockResolvedValue();
    const wrapper = await setup();
    await openEmailModal(wrapper);
    await type(wrapper, 'email', 'new@x.com');
    await type(wrapper, 'confirmNewEmail', 'new@x.com');
    await save(wrapper);
    await vi.waitFor(() => { expect(changeEmail).toHaveBeenCalledWith({ newEmail: 'new@x.com', confirmNewEmail: 'new@x.com' }); });
    expect(showAlertSuccess).toHaveBeenCalledWith('Updated');
    expect(wrapper.find('.profile-fields dd').text()).toBe('new@x.com');
  });

  it('reports a failed email change', async () => {
    vi.mocked(changeEmail).mockRejectedValue('taken');
    const wrapper = await setup();
    await openEmailModal(wrapper);
    await type(wrapper, 'email', 'new@x.com');
    await type(wrapper, 'confirmNewEmail', 'new@x.com');
    await save(wrapper);
    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith('taken'); });
    expect(wrapper.find('.profile-fields dd').text()).toBe('john@x.com');
  });

  it('deactivates the account and signs out after confirming', async () => {
    vi.mocked(deactivateAccount).mockResolvedValue();
    signOut.mockResolvedValue();
    const assign = vi.fn();
    vi.stubGlobal('location', { ...window.location, assign });
    vi.useFakeTimers();
    const wrapper = await setup();
    await button(wrapper, 'Deactivate my account').trigger('click');
    expect(confirmConfig().title).toBe('Are you sure?');
    await confirmConfig().onConfirm();

    expect(deactivateAccount).toHaveBeenCalledOnce();
    expect(showAlertSuccess).toHaveBeenCalledWith('Your account has been deactivated. You will be signed out shortly.');
    expect(signOut).not.toHaveBeenCalled();
    await vi.advanceTimersByTimeAsync(2000);
    expect(signOut).toHaveBeenCalledOnce();
    expect(assign).toHaveBeenCalledWith('/');
  });

  it('reports a failed deactivation', async () => {
    vi.mocked(deactivateAccount).mockRejectedValue('denied');
    const wrapper = await setup();
    await button(wrapper, 'Deactivate my account').trigger('click');
    await confirmConfig().onConfirm();
    expect(showAlertError).toHaveBeenCalledWith('denied');
  });
});
