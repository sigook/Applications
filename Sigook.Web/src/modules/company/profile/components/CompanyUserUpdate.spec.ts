import { describe, expect, it, vi } from 'vitest';
import CompanyUserUpdate from '@/modules/company/profile/components/CompanyUserUpdate.vue';
import { updateCompanyUser } from '@/modules/company/profile/api';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { CompanyUserModel } from '@/shared/company-profile/types';

vi.mock('@/modules/company/profile/api', () => ({ updateCompanyUser: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));

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

function setup(current: CompanyUserModel = user) {
  return mountWithPlugins(CompanyUserUpdate, { props: { user: current } });
}

function input(wrapper: ReturnType<typeof setup>, name: string) {
  return wrapper.find(`input[name="${name}"]`);
}

function value(wrapper: ReturnType<typeof setup>, name: string) {
  return (input(wrapper, name).element as HTMLInputElement).value;
}

async function submit(wrapper: ReturnType<typeof setup>) {
  await wrapper.find('button.is-primary').trigger('click');
}

function errors(wrapper: ReturnType<typeof setup>) {
  return wrapper.findAll('.help.is-danger').map((e) => e.text());
}

describe('CompanyUserUpdate', () => {
  it('fills the form with the user', () => {
    const wrapper = setup();

    expect(value(wrapper, 'name')).toBe('Ann');
    expect(value(wrapper, 'lastname')).toBe('Lee');
    expect(value(wrapper, 'position')).toBe('Manager');
    expect((wrapper.find('input[type="tel"]').element as HTMLInputElement).value).toBe('416 555-0000');
  });

  it('saves the edited user and emits it', async () => {
    vi.mocked(updateCompanyUser).mockResolvedValue();
    const wrapper = setup();
    await input(wrapper, 'name').setValue('Anna');
    await input(wrapper, 'position').setValue('');
    await wrapper.find('input[type="tel"]').setValue('6475551234');
    await submit(wrapper);

    const expected = { ...user, name: 'Anna', position: '', mobileNumber: '647 555-1234' };
    await vi.waitFor(() => { expect(wrapper.emitted('saved')).toHaveLength(1); });
    expect(updateCompanyUser).toHaveBeenCalledExactlyOnceWith('user-1', expected);
    expect(wrapper.emitted('update:user')).toEqual([[expected]]);
    expect(showAlertSuccess).toHaveBeenCalledWith('Updated');
  });

  it('requires name and last name', async () => {
    const wrapper = setup();
    await input(wrapper, 'name').setValue('');
    await input(wrapper, 'lastname').setValue('');
    await submit(wrapper);

    await vi.waitFor(() => { expect(errors(wrapper)).toEqual(expect.arrayContaining(['Name is required', 'Last Name is required'])); });
    expect(updateCompanyUser).not.toHaveBeenCalled();
  });

  it('rejects a one-letter position', async () => {
    const wrapper = setup();
    await input(wrapper, 'position').setValue('A');
    await submit(wrapper);

    await vi.waitFor(() => { expect(errors(wrapper)).toContain('Min 2 characters'); });
    expect(updateCompanyUser).not.toHaveBeenCalled();
  });

  it('refills the form when the user changes', async () => {
    const wrapper = setup();
    await wrapper.setProps({ user: { ...user, name: 'Bob', lastname: 'Ray', position: '', mobileNumber: '' } });

    expect(value(wrapper, 'name')).toBe('Bob');
    expect(value(wrapper, 'lastname')).toBe('Ray');
    expect(value(wrapper, 'position')).toBe('');
  });

  it('shows the error and does not emit when saving fails', async () => {
    const error = new Error('save failed');
    vi.mocked(updateCompanyUser).mockRejectedValue(error);
    const wrapper = setup();
    await submit(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('saved')).toBeUndefined();
    expect(showAlertSuccess).not.toHaveBeenCalled();
  });
});
