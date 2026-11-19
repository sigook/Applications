import { beforeEach, describe, expect, it, vi } from 'vitest';
import { computed, nextTick, ref } from 'vue';
import { flushPromises } from '@vue/test-utils';
import CompanyUsers from '@/modules/company/profile/components/CompanyUsers.vue';
import { createCompanyUser, deleteCompanyUser, getCompanyUser, updateCompanyUser, updateCompanyUserEmail } from '@/modules/company/profile/api';
import { showAlertConfirm, showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { CompanyUserModel, CreateCompanyUserModel } from '@/shared/company-profile/types';

const touch = ref(false);

vi.mock('@/shared/composables/useBreakpoint', () => ({
  useBreakpoint: () => ({ isTouch: computed(() => touch.value) }),
}));
vi.mock('@/modules/company/profile/api', () => ({
  getCompanyUser: vi.fn(),
  deleteCompanyUser: vi.fn(),
  createCompanyUser: vi.fn(),
  updateCompanyUser: vi.fn(),
  updateCompanyUserEmail: vi.fn(),
}));
vi.mock('@/shared/api/reportApi', () => ({ downloadAgencyReport: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertConfirm: vi.fn(), showAlertError: vi.fn() }));
vi.mock('@/shared/company-profile/CompanyUserModal.vue', () => ({
  default: {
    name: 'CompanyUserModal',
    props: { save: Function, user: Object, emailEditable: Boolean },
    emits: ['updateUsers'],
    template: '<div class="company-user-modal" />',
  },
}));

type Save = (model: CreateCompanyUserModel) => Promise<void>;

const ann: CompanyUserModel = {
  id: 'user-1', companyId: 'company-1', createdAt: '2026-01-01T00:00:00', email: 'ann@acme.com',
  name: 'Ann', lastname: 'Lee', position: 'Manager', mobileNumber: '416 555-0000',
};
const bob: CompanyUserModel = {
  id: 'user-2', companyId: 'company-1', createdAt: '2026-01-01T00:00:00', email: 'bob@acme.com',
  name: 'Bob', lastname: 'Ray', position: '', mobileNumber: '',
};

async function setup(users: CompanyUserModel[] = [ann, bob]) {
  vi.mocked(getCompanyUser).mockResolvedValue(users);
  const wrapper = mountWithPlugins(CompanyUsers);
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function rows(wrapper: Wrapper) {
  return wrapper.findAll('tbody tr').map((row) => row.findAll('td').slice(0, 2).map((cell) => cell.text()));
}

function editModal(wrapper: Wrapper) {
  return wrapper.findAllComponents({ name: 'CompanyUserModal' }).find((m) => m.props('user'));
}

function editSave(wrapper: Wrapper): Save {
  const save = editModal(wrapper)?.props('save');
  if (typeof save !== 'function') throw new Error('edit modal not found');
  return save as Save;
}

beforeEach(() => {
  touch.value = false;
});

describe('CompanyUsers', () => {
  it('lists the users and reports how many there are', async () => {
    const wrapper = await setup();

    expect(rows(wrapper)).toEqual([['Ann', 'Lee'], ['Bob', 'Ray']]);
    expect(wrapper.emitted('loaded')).toEqual([[2]]);
  });

  it('deletes a user once confirmed and reloads', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(deleteCompanyUser).mockResolvedValue();
    const wrapper = await setup();
    vi.mocked(getCompanyUser).mockResolvedValue([bob]);
    await wrapper.findAll('tbody tr')[0].find('button.is-danger').trigger('click');
    await flushPromises();

    expect(showAlertConfirm).toHaveBeenCalledWith('Are you sure?', 'You want to delete user.');
    expect(deleteCompanyUser).toHaveBeenCalledExactlyOnceWith('user-1');
    expect(rows(wrapper)).toEqual([['Bob', 'Ray']]);
    expect(wrapper.emitted('loaded')).toEqual([[2], [1]]);
  });

  it('keeps the user when the deletion is not confirmed', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(false);
    const wrapper = await setup();
    await wrapper.findAll('tbody tr')[0].find('button.is-danger').trigger('click');
    await flushPromises();

    expect(deleteCompanyUser).not.toHaveBeenCalled();
  });

  it('shows the error when the deletion fails', async () => {
    const error = new Error('delete failed');
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(deleteCompanyUser).mockRejectedValue(error);
    const wrapper = await setup();
    await wrapper.findAll('tbody tr')[0].find('button.is-danger').trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledExactlyOnceWith(error);
  });

  it('creates users through the add modal and reloads once done', async () => {
    const wrapper = await setup();
    await wrapper.find('.sigook-grid-actions button').trigger('click');
    const createModal = wrapper.findAllComponents({ name: 'CompanyUserModal' })[0];
    expect(createModal.props('save')).toBe(createCompanyUser);

    createModal.vm.$emit('updateUsers');
    await flushPromises();

    expect(getCompanyUser).toHaveBeenCalledTimes(2);
  });

  it('updates the edited user without changing the email when it is the same', async () => {
    vi.mocked(updateCompanyUser).mockResolvedValue();
    const wrapper = await setup();
    await wrapper.findAll('tbody tr')[1].find('button[title="Edit"]').trigger('click');
    expect(editModal(wrapper)?.props('user')).toEqual(bob);
    expect(editModal(wrapper)?.props('emailEditable')).toBe(true);

    await editSave(wrapper)({ name: 'Bobby', lastname: 'Ray', position: null, mobileNumber: null, email: ' BOB@acme.com ' });

    expect(updateCompanyUser).toHaveBeenCalledExactlyOnceWith('user-2', { ...bob, name: 'Bobby', email: 'bob@acme.com', position: '', mobileNumber: '' });
    expect(updateCompanyUserEmail).not.toHaveBeenCalled();

    editModal(wrapper)?.vm.$emit('updateUsers');
    await flushPromises();
    expect(editModal(wrapper)).toBeUndefined();
    expect(getCompanyUser).toHaveBeenCalledTimes(2);
  });

  it('changes the email when the edited one differs', async () => {
    vi.mocked(updateCompanyUser).mockResolvedValue();
    vi.mocked(updateCompanyUserEmail).mockResolvedValue();
    const wrapper = await setup();
    await wrapper.findAll('tbody tr')[0].find('button[title="Edit"]').trigger('click');
    await editSave(wrapper)({ name: 'Ann', lastname: 'Lee', position: 'Manager', mobileNumber: '416 555-0000', email: 'ann.lee@acme.com' });

    expect(updateCompanyUser).toHaveBeenCalledExactlyOnceWith('user-1', ann);
    expect(updateCompanyUserEmail).toHaveBeenCalledExactlyOnceWith('user-1', { newEmail: 'ann.lee@acme.com' });
  });

  it('shows cards and filters them by search on touch screens', async () => {
    touch.value = true;
    const wrapper = await setup();

    expect(wrapper.findAll('.rcard__title').map((t) => t.text())).toEqual(['Ann Lee', 'Bob Ray']);
    expect(wrapper.find('.rcard__row').text()).toContain('Manager');

    await wrapper.find('.sigook-grid-actions input').setValue('  MANAG ');
    await nextTick();
    expect(wrapper.findAll('.rcard__title').map((t) => t.text())).toEqual(['Ann Lee']);

    await wrapper.find('.rcard button[title="Edit"]').trigger('click');
    expect(editModal(wrapper)?.props('user')).toEqual(ann);
  });

  it('deletes from a card', async () => {
    touch.value = true;
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(deleteCompanyUser).mockResolvedValue();
    const wrapper = await setup();
    await wrapper.findAll('.rcard')[1].find('button.is-danger').trigger('click');
    await flushPromises();

    expect(deleteCompanyUser).toHaveBeenCalledExactlyOnceWith('user-2');
  });
});
