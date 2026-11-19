import { describe, expect, it, vi } from 'vitest';
import WorkEmailForm from '@/shared/worker-profile/forms/WorkEmailForm.vue';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createWorkerProfileDetail } from '@/testing/workerProfileFixtures';

vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

function setup(save = vi.fn<(workerId: string, model: { newEmail: string }) => Promise<unknown>>()) {
  const wrapper = mountWithPlugins(WorkEmailForm, { props: { data: createWorkerProfileDetail(), save } });
  return { wrapper, save };
}

type Wrapper = ReturnType<typeof setup>['wrapper'];

function saveButton(wrapper: Wrapper) {
  const button = wrapper.findAll('button').find((b) => b.text() === 'Save');
  if (!button) throw new Error('Save button not found');
  return button;
}

async function fill(wrapper: Wrapper, newEmail: string, confirmEmail: string) {
  await wrapper.find('input[name="newEmail"]').setValue(newEmail);
  await wrapper.find('input[name="confirmEmail"]').setValue(confirmEmail);
}

function errorMessages(wrapper: Wrapper) {
  return wrapper.findAll('.help.is-danger').map((m) => m.text());
}

describe('WorkEmailForm', () => {
  it('warns that the previous email stops working', () => {
    expect(setup().wrapper.find('.message').text()).toContain('will not be able to login with the previous email anymore');
  });

  it('requires both emails', async () => {
    const { wrapper, save } = setup();
    await saveButton(wrapper).trigger('click');

    await vi.waitFor(() => { expect(errorMessages(wrapper)).toEqual(['Email is required', 'Confirm Email is required']); });
    expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly');
    expect(save).not.toHaveBeenCalled();
  });

  it('rejects an invalid email', async () => {
    const { wrapper, save } = setup();
    await fill(wrapper, 'not-an-email', 'not-an-email');
    await saveButton(wrapper).trigger('click');

    await vi.waitFor(() => { expect(errorMessages(wrapper)).toEqual(['Invalid email', 'Invalid email']); });
    expect(save).not.toHaveBeenCalled();
  });

  it('requires the confirmation to match', async () => {
    const { wrapper, save } = setup();
    await fill(wrapper, 'new@example.com', 'other@example.com');
    await saveButton(wrapper).trigger('click');

    await vi.waitFor(() => { expect(errorMessages(wrapper)).toEqual(['Emails must match']); });
    expect(save).not.toHaveBeenCalled();
  });

  it('blocks pasting into the confirmation', () => {
    const { wrapper } = setup();
    const event = new Event('paste', { cancelable: true });
    wrapper.find('input[name="confirmEmail"]').element.dispatchEvent(event);

    expect(event.defaultPrevented).toBe(true);
  });

  it('saves the new email and closes the modal', async () => {
    const { wrapper, save } = setup();
    save.mockResolvedValue(undefined);
    await fill(wrapper, 'new@example.com', 'new@example.com');
    await saveButton(wrapper).trigger('click');

    await vi.waitFor(() => { expect(wrapper.emitted('closeModal')).toEqual([[true]]); });
    expect(save).toHaveBeenCalledExactlyOnceWith('worker-profile-1', { newEmail: 'new@example.com' });
  });

  it('shows the error and keeps the modal open when saving fails', async () => {
    const error = new Error('email taken');
    const { wrapper, save } = setup();
    save.mockRejectedValue(error);
    await fill(wrapper, 'new@example.com', 'new@example.com');
    await saveButton(wrapper).trigger('click');

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('closeModal')).toBeUndefined();
  });
});
