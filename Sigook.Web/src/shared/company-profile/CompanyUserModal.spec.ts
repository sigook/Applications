import { describe, expect, it, vi } from 'vitest';
import CompanyUserModal from '@/shared/company-profile/CompanyUserModal.vue';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { CompanyUserModel, CreateCompanyUserModel } from '@/shared/company-profile/types';

vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

type Save = (model: CreateCompanyUserModel) => Promise<unknown>;

const user: CompanyUserModel = {
  id: 'user-1',
  companyId: 'company-1',
  createdAt: '2026-01-01T00:00:00',
  email: 'ann@acme.com',
  name: 'Ann',
  lastname: 'Lee',
  position: 'Manager',
  mobileNumber: '4165550000',
};

function setup(save: Save, current?: CompanyUserModel | null, emailEditable?: boolean) {
  return mountWithPlugins(CompanyUserModal, { props: { save, user: current, emailEditable } });
}

function input(wrapper: ReturnType<typeof setup>, name: string) {
  return wrapper.find(`input[name="${name}"]`);
}

async function submit(wrapper: ReturnType<typeof setup>) {
  await wrapper.find('button.is-primary').trigger('click');
}

describe('CompanyUserModal', () => {
  it('explains the invitation email for a new user', () => {
    const wrapper = setup(vi.fn<Save>());

    expect(wrapper.find('.message.is-info').text()).toContain('The user will receive an email');
    expect(wrapper.find('button.is-primary').text()).toBe('Create');
    expect(input(wrapper, 'email').attributes('disabled')).toBeUndefined();
  });

  it('fills the form and locks the email when editing', () => {
    const wrapper = setup(vi.fn<Save>(), user);

    expect(wrapper.find('.message').exists()).toBe(false);
    expect(wrapper.find('button.is-primary').text()).toBe('Save');
    expect((input(wrapper, 'name').element as HTMLInputElement).value).toBe('Ann');
    expect((input(wrapper, 'position').element as HTMLInputElement).value).toBe('Manager');
    expect((wrapper.find('input[type="tel"]').element as HTMLInputElement).value).toBe('416 555-0000');
    expect(input(wrapper, 'email').attributes('disabled')).toBeDefined();
  });

  it('warns about changing the email when it is editable', () => {
    const wrapper = setup(vi.fn<Save>(), user, true);

    expect(wrapper.find('.message.is-warning').text()).toContain('If you change the email');
    expect(input(wrapper, 'email').attributes('disabled')).toBeUndefined();
  });

  it('creates the user and emits updateUsers', async () => {
    const save = vi.fn<Save>().mockResolvedValue(undefined);
    const wrapper = setup(save);
    await input(wrapper, 'name').setValue('Bob');
    await input(wrapper, 'lastname').setValue('Stone');
    await input(wrapper, 'email').setValue('bob@acme.com');
    await wrapper.find('input[type="tel"]').setValue('4165551234');
    await submit(wrapper);

    await vi.waitFor(() => { expect(wrapper.emitted('updateUsers')).toHaveLength(1); });
    expect(save).toHaveBeenCalledExactlyOnceWith({
      name: 'Bob',
      lastname: 'Stone',
      position: null,
      email: 'bob@acme.com',
      mobileNumber: '416 555-1234',
    });
  });

  it('saves the edited user', async () => {
    const save = vi.fn<Save>().mockResolvedValue(undefined);
    const wrapper = setup(save, user);
    await input(wrapper, 'position').setValue('Director');
    await submit(wrapper);

    await vi.waitFor(() => { expect(wrapper.emitted('updateUsers')).toHaveLength(1); });
    expect(save).toHaveBeenCalledExactlyOnceWith({
      name: 'Ann',
      lastname: 'Lee',
      position: 'Director',
      email: 'ann@acme.com',
      mobileNumber: '4165550000',
    });
  });

  it('shows the validation errors and does not save', async () => {
    const save = vi.fn<Save>();
    const wrapper = setup(save);
    await input(wrapper, 'name').setValue('B');
    await input(wrapper, 'email').setValue('not-an-email');
    await submit(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly'); });
    const errors = wrapper.findAll('.help.is-danger').map((e) => e.text());
    expect(errors).toEqual(expect.arrayContaining(['Min 2 characters', 'Last Name is required', 'Invalid email']));
    expect(save).not.toHaveBeenCalled();
  });

  it('shows the server error when saving fails', async () => {
    const save = vi.fn<Save>().mockRejectedValue({ data: 'Email already taken' });
    const wrapper = setup(save, user);
    await submit(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith('Email already taken'); });
    expect(wrapper.emitted('updateUsers')).toBeUndefined();
  });
});
