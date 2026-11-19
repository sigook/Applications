import { describe, expect, it, vi } from 'vitest';
import DialogCompanyUpdateEmail from '@/modules/agency/recruiting/clients/components/DialogCompanyUpdateEmail.vue';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

type Save = (email: string) => Promise<void>;

function setup(save: Save = vi.fn<Save>().mockResolvedValue()) {
  return mountWithPlugins(DialogCompanyUpdateEmail, { props: { save } });
}

function errors(wrapper: ReturnType<typeof setup>) {
  return wrapper.findAll('.help.is-danger').map((e) => e.text());
}

async function fill(wrapper: ReturnType<typeof setup>, newEmail: string, confirmEmail: string) {
  await wrapper.find('input[name="newEmail"]').setValue(newEmail);
  await wrapper.find('input[name="confirmEmail"]').setValue(confirmEmail);
}

describe('DialogCompanyUpdateEmail', () => {
  it('warns that the previous email stops working', () => {
    expect(setup().find('.message').text()).toContain('will not be able to login with the previous email');
  });

  it('requires both emails', async () => {
    const save = vi.fn<Save>();
    const wrapper = setup(save);
    await wrapper.find('button.is-primary').trigger('click');

    await vi.waitFor(() => { expect(errors(wrapper)).toEqual(['Email is required', 'Confirm email is required']); });
    expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly');
    expect(save).not.toHaveBeenCalled();
  });

  it('rejects an invalid email and a mismatching confirmation', async () => {
    const save = vi.fn<Save>();
    const wrapper = setup(save);
    await fill(wrapper, 'not-an-email', 'other');
    await wrapper.find('button.is-primary').trigger('click');

    await vi.waitFor(() => { expect(errors(wrapper)).toEqual(['Invalid email', 'Emails must match']); });
    expect(save).not.toHaveBeenCalled();
  });

  it('saves the new email and closes with it', async () => {
    const save = vi.fn<Save>().mockResolvedValue();
    const wrapper = setup(save);
    await fill(wrapper, 'new@acme.com', 'new@acme.com');
    await wrapper.find('button.is-primary').trigger('click');

    await vi.waitFor(() => { expect(wrapper.emitted('closeModal')).toEqual([[true, 'new@acme.com']]); });
    expect(save).toHaveBeenCalledExactlyOnceWith('new@acme.com');
  });

  it('shows the error and stays open when saving fails', async () => {
    const error = new Error('taken');
    const wrapper = setup(vi.fn<Save>().mockRejectedValue(error));
    await fill(wrapper, 'new@acme.com', 'new@acme.com');
    await wrapper.find('button.is-primary').trigger('click');

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('closeModal')).toBeUndefined();
  });
});
