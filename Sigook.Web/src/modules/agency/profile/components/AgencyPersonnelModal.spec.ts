import { beforeEach, describe, expect, it, vi } from 'vitest';
import { reactive } from 'vue';
import { flushPromises } from '@vue/test-utils';
import { createPinia, setActivePinia, type Pinia } from 'pinia';
import AgencyPersonnelModal from '@/modules/agency/profile/components/AgencyPersonnelModal.vue';
import { createAgencyPersonnel, getAssignableRoles, updateAgencyPersonnel } from '@/modules/agency/profile/api';
import { useAgencyStore } from '@/modules/agency/store';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { AgencyOption, AgencyPersonnelListItem } from '@/modules/agency/profile/types';

const securityStore = reactive<{ user: { profile: { sub: string } } | null }>({ user: null });

vi.mock('@/app/stores/security', () => ({ useSecurityStore: () => securityStore }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));
vi.mock('@/modules/agency/profile/api', () => ({
  createAgencyPersonnel: vi.fn(),
  updateAgencyPersonnel: vi.fn(),
  getAssignableRoles: vi.fn(),
}));

const covenant: AgencyOption = { id: 'a1', name: 'Covenant' };
const sigook: AgencyOption = { id: 'a2', name: 'Sigook' };

const personnel: AgencyPersonnelListItem = {
  id: 'p-1',
  userId: 'user-1',
  name: 'Ana Smith',
  email: 'ana@sigook.com',
  role: 'recruiting',
  agencyIds: ['a2'],
  agencies: ['Sigook'],
};

let pinia: Pinia;

async function setup(current: AgencyPersonnelListItem | null = null) {
  const wrapper = mountWithPlugins(AgencyPersonnelModal, { props: { personnel: current }, global: { plugins: [pinia] } });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function value(wrapper: Wrapper, name: string) {
  return (wrapper.find(`[name="${name}"]`).element as HTMLInputElement).value;
}

function errors(wrapper: Wrapper) {
  return wrapper.findAll('.help.is-danger').map((e) => e.text());
}

async function submit(wrapper: Wrapper) {
  await wrapper.find('button.is-primary').trigger('click');
}

async function fill(wrapper: Wrapper) {
  await wrapper.find('[name="name"]').setValue('John Doe');
  await wrapper.find('[name="email"]').setValue('john@sigook.com');
  await wrapper.find('select[name="role"]').setValue('admin');
}

beforeEach(() => {
  pinia = createPinia();
  setActivePinia(pinia);
  useAgencyStore().setAgencies([covenant]);
  securityStore.user = { profile: { sub: 'me' } };
  vi.mocked(getAssignableRoles).mockResolvedValue(['admin', 'recruiting', 'custom']);
});

describe('AgencyPersonnelModal', () => {
  it('explains the invitation email and lists the assignable roles when creating', async () => {
    const wrapper = await setup();

    expect(wrapper.find('.message.is-info').text()).toContain('The user will receive an email');
    expect(wrapper.findAll('select[name="role"] option').map((o) => o.text())).toEqual(['Admin', 'Recruiting', 'custom']);
    expect(wrapper.find('button.is-primary').text()).toBe('Create');
    expect(wrapper.findComponent({ name: 'BTaginput' }).exists()).toBe(false);
  });

  it('validates the required fields', async () => {
    const wrapper = await setup();
    await submit(wrapper);

    await vi.waitFor(() => { expect(errors(wrapper)).toEqual(['Name is required', 'Email is required', 'Role is required']); });
    expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly');
    expect(createAgencyPersonnel).not.toHaveBeenCalled();
  });

  it('rejects an invalid email', async () => {
    const wrapper = await setup();
    await fill(wrapper);
    await wrapper.find('[name="email"]').setValue('not-an-email');
    await submit(wrapper);

    await vi.waitFor(() => { expect(errors(wrapper)).toContain('Invalid email'); });
    expect(createAgencyPersonnel).not.toHaveBeenCalled();
  });

  it('creates the user in the only agency and notifies the list', async () => {
    vi.mocked(createAgencyPersonnel).mockResolvedValue();
    const wrapper = await setup();
    await fill(wrapper);
    await submit(wrapper);

    await vi.waitFor(() => { expect(wrapper.emitted('updateUsers')).toHaveLength(1); });
    expect(createAgencyPersonnel).toHaveBeenCalledExactlyOnceWith({ name: 'John Doe', email: 'john@sigook.com', role: 'admin', agencyIds: ['a1'] });
  });

  it('requires an agency when the user has several', async () => {
    useAgencyStore().setAgencies([covenant, sigook]);
    const wrapper = await setup();
    await fill(wrapper);
    await submit(wrapper);

    await vi.waitFor(() => { expect(errors(wrapper)).toContain('Select at least one agency'); });
    expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly');
    expect(createAgencyPersonnel).not.toHaveBeenCalled();
  });

  it('creates the user in the chosen agencies', async () => {
    useAgencyStore().setAgencies([covenant, sigook]);
    vi.mocked(createAgencyPersonnel).mockResolvedValue();
    const wrapper = await setup();
    const taginput = wrapper.findComponent({ name: 'BTaginput' });
    taginput.vm.$emit('typing', 'SIG');
    await flushPromises();

    expect(taginput.props('data')).toEqual([sigook]);

    taginput.vm.$emit('update:modelValue', [sigook]);
    await fill(wrapper);
    await submit(wrapper);

    await vi.waitFor(() => { expect(wrapper.emitted('updateUsers')).toHaveLength(1); });
    expect(createAgencyPersonnel).toHaveBeenCalledWith(expect.objectContaining({ agencyIds: ['a2'] }));
  });

  it('loads the user to edit and saves it with update', async () => {
    useAgencyStore().setAgencies([covenant, sigook]);
    vi.mocked(updateAgencyPersonnel).mockResolvedValue();
    const wrapper = await setup(personnel);

    expect(wrapper.find('.message.is-warning').text()).toContain('The role is global');
    expect(value(wrapper, 'name')).toBe('Ana Smith');
    expect(value(wrapper, 'email')).toBe('ana@sigook.com');
    expect(value(wrapper, 'role')).toBe('recruiting');
    expect(wrapper.findComponent({ name: 'BTaginput' }).props('modelValue')).toEqual([sigook]);
    expect(wrapper.find('button.is-primary').text()).toBe('Save');

    await wrapper.find('[name="name"]').setValue('Ana Brown');
    await submit(wrapper);

    await vi.waitFor(() => { expect(wrapper.emitted('updateUsers')).toHaveLength(1); });
    expect(createAgencyPersonnel).not.toHaveBeenCalled();
    expect(updateAgencyPersonnel).toHaveBeenCalledExactlyOnceWith('p-1', { name: 'Ana Brown', email: 'ana@sigook.com', role: 'recruiting', agencyIds: ['a2'] });
  });

  it('locks the role and agencies of the signed-in user', async () => {
    useAgencyStore().setAgencies([covenant, sigook]);
    securityStore.user = { profile: { sub: 'user-1' } };
    const wrapper = await setup(personnel);

    expect((wrapper.find('select[name="role"]').element as HTMLSelectElement).disabled).toBe(true);
    expect(wrapper.text()).toContain('You cannot change your own role');
    expect(wrapper.text()).toContain('You cannot change your own agencies');
    expect(wrapper.findComponent({ name: 'BTaginput' }).props('disabled')).toBe(true);
  });

  it('shows the error when the roles cannot be loaded', async () => {
    const error = new Error('roles failed');
    vi.mocked(getAssignableRoles).mockRejectedValue(error);
    await setup();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('shows the error when saving fails', async () => {
    const error = new Error('save failed');
    vi.mocked(createAgencyPersonnel).mockRejectedValue(error);
    const wrapper = await setup();
    await fill(wrapper);
    await submit(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('updateUsers')).toBeUndefined();
  });
});
