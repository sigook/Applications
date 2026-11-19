import { describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import UserList from '@/modules/agency/recruiting/clients/components/UserList.vue';
import { createCompanyProfileUser, deleteCompanyProfileUser, getCompanyUsers, updateCompanyProfileUser, updateCompanyUserEmail } from '@/modules/agency/recruiting/clients/api';
import { showAlertConfirm, showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createAgencyCompany } from '@/testing/agencyCompanyFixtures';
import type { CompanyUserModel, CreateCompanyUserModel } from '@/shared/company-profile/types';

vi.mock('@/app/stores/security', () => ({ useSecurityStore: () => ({ userRoles: [] }) }));
vi.mock('@/modules/agency/recruiting/clients/api', () => ({
  getCompanyUsers: vi.fn(),
  deleteCompanyProfileUser: vi.fn(),
  createCompanyProfileUser: vi.fn(),
  updateCompanyProfileUser: vi.fn(),
  updateCompanyUserEmail: vi.fn(),
}));
vi.mock('@/shared/utils/toast', () => ({ showAlertConfirm: vi.fn(), showAlertError: vi.fn() }));
vi.mock('@/shared/company-profile/CompanyUserModal.vue', () => ({
  default: {
    name: 'CompanyUserModal',
    props: { user: Object, emailEditable: Boolean, save: Function },
    emits: ['updateUsers'],
    template: '<div class="company-user-modal-stub" />',
  },
}));

type Save = (user: CreateCompanyUserModel) => Promise<unknown>;

const ana: CompanyUserModel = {
  id: 'u1',
  companyId: 'company-user-1',
  createdAt: '2026-01-01T00:00:00Z',
  email: 'Ana@acme.com',
  name: 'Ana',
  lastname: 'Lopez',
  position: 'Supervisor',
  mobileNumber: '416 555-0103',
};

async function setup(users: CompanyUserModel[] = [ana]) {
  vi.mocked(getCompanyUsers).mockResolvedValue(users);
  const wrapper = mountWithPlugins(UserList, { props: { company: createAgencyCompany() } });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function rows(wrapper: Wrapper) {
  return wrapper.findAll('tbody tr').map((row) => row.findAll('td').slice(0, 5).map((cell) => cell.text()));
}

function userModals(wrapper: Wrapper) {
  return wrapper.findAllComponents({ name: 'CompanyUserModal' });
}

function edited(model: Partial<CreateCompanyUserModel> = {}): CreateCompanyUserModel {
  return { name: 'Ana', lastname: 'Lopez', email: 'ana@acme.com', position: null, mobileNumber: null, ...model };
}

async function openEdit(wrapper: Wrapper) {
  await wrapper.find('tbody button[title="Edit"]').trigger('click');
  await flushPromises();
  const modal = userModals(wrapper).find((m) => m.props('user'));
  if (!modal) throw new Error('edit modal not found');
  return modal;
}

describe('UserList', () => {
  it('lists the company users', async () => {
    const wrapper = await setup();

    expect(getCompanyUsers).toHaveBeenCalledExactlyOnceWith('company-profile-1');
    expect(rows(wrapper)).toEqual([['Ana@acme.com', 'Ana', 'Lopez', '416 555-0103', 'Supervisor']]);
    expect(wrapper.findComponent({ name: 'BLoading' }).props('modelValue')).toBe(false);
  });

  it('creates users through the company endpoint and reloads', async () => {
    vi.mocked(createCompanyProfileUser).mockResolvedValue({ id: 'u2' });
    const wrapper = await setup();
    await wrapper.findAll('button').find((b) => b.text() === 'Add')?.trigger('click');
    const modal = userModals(wrapper)[0];
    const user = edited({ email: 'new@acme.com' });

    await (modal.props('save') as Save)(user);
    expect(createCompanyProfileUser).toHaveBeenCalledExactlyOnceWith('company-profile-1', user);

    modal.vm.$emit('updateUsers');
    await flushPromises();

    expect(getCompanyUsers).toHaveBeenCalledTimes(2);
    expect(wrapper.findAllComponents({ name: 'BModal' })[0].props('modelValue')).toBe(false);
  });

  it('edits a user keeping the email when unchanged', async () => {
    vi.mocked(updateCompanyProfileUser).mockResolvedValue();
    const wrapper = await setup();
    const modal = await openEdit(wrapper);

    expect(modal.props('user')).toMatchObject({ id: 'u1' });
    expect(modal.props('emailEditable')).toBe(true);

    await (modal.props('save') as Save)(edited({ name: 'Anna', email: ' ANA@acme.com ' }));

    expect(updateCompanyProfileUser).toHaveBeenCalledExactlyOnceWith('company-profile-1', 'u1', {
      ...ana,
      actions: null,
      name: 'Anna',
      email: ' ANA@acme.com ',
      position: '',
      mobileNumber: '',
    });
    expect(updateCompanyUserEmail).not.toHaveBeenCalled();
  });

  it('changes the user email when it differs', async () => {
    vi.mocked(updateCompanyProfileUser).mockResolvedValue();
    vi.mocked(updateCompanyUserEmail).mockResolvedValue();
    const wrapper = await setup();
    const modal = await openEdit(wrapper);

    await (modal.props('save') as Save)(edited({ email: 'ana.lopez@acme.com', position: 'Lead', mobileNumber: '416 555-0199' }));

    expect(vi.mocked(updateCompanyProfileUser).mock.calls[0][2]).toMatchObject({ position: 'Lead', mobileNumber: '416 555-0199' });
    expect(updateCompanyUserEmail).toHaveBeenCalledExactlyOnceWith('company-profile-1', 'u1', { newEmail: 'ana.lopez@acme.com' });
  });

  it('closes the edit modal and reloads after editing', async () => {
    const wrapper = await setup();
    const modal = await openEdit(wrapper);
    modal.vm.$emit('updateUsers');
    await flushPromises();

    expect(getCompanyUsers).toHaveBeenCalledTimes(2);
    expect(userModals(wrapper).some((m) => m.props('user'))).toBe(false);
  });

  it('shows the error and hides the loader when loading the users fails', async () => {
    const error = new Error('load failed');
    vi.mocked(getCompanyUsers).mockRejectedValue(error);
    const wrapper = mountWithPlugins(UserList, { props: { company: createAgencyCompany() } });
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledExactlyOnceWith(error);
    expect(wrapper.findComponent({ name: 'BLoading' }).props('modelValue')).toBe(false);
  });

  it('shows the error when reloading the users fails', async () => {
    const wrapper = await setup();
    await wrapper.findAll('button').find((b) => b.text() === 'Add')?.trigger('click');
    vi.mocked(getCompanyUsers).mockRejectedValue({ data: 'Reload failed' });
    userModals(wrapper)[0].vm.$emit('updateUsers');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledExactlyOnceWith({ data: 'Reload failed' });
    expect(rows(wrapper)).toHaveLength(1);
    expect(wrapper.findComponent({ name: 'BLoading' }).props('modelValue')).toBe(false);
  });

  it('keeps the user when the delete is not confirmed', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(false);
    const wrapper = await setup();
    await wrapper.find('tbody button.is-danger').trigger('click');
    await flushPromises();

    expect(showAlertConfirm).toHaveBeenCalledWith('Are you sure', 'You want to delete this user');
    expect(deleteCompanyProfileUser).not.toHaveBeenCalled();
    expect(rows(wrapper)).toHaveLength(1);
  });

  it('deletes a user after confirming and reloads', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(deleteCompanyProfileUser).mockResolvedValue();
    const wrapper = await setup();
    vi.mocked(getCompanyUsers).mockResolvedValue([]);
    await wrapper.find('tbody button.is-danger').trigger('click');
    await flushPromises();

    expect(deleteCompanyProfileUser).toHaveBeenCalledExactlyOnceWith('company-profile-1', 'u1');
    expect(rows(wrapper)).toEqual([['No records available']]);
  });

  it('shows the server message when deleting fails', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(deleteCompanyProfileUser).mockRejectedValue({ data: 'User has requests' });
    const wrapper = await setup();
    await wrapper.find('tbody button.is-danger').trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith('User has requests');
    expect(getCompanyUsers).toHaveBeenCalledTimes(2);
  });
});
