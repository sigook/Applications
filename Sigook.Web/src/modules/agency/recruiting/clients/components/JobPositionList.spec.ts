import { beforeEach, describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import { reactive } from 'vue';
import JobPositionList from '@/modules/agency/recruiting/clients/components/JobPositionList.vue';
import {
  createAgencyCompanyJobPosition,
  createAgencyCompanyJobPositionWithoutRates,
  deleteAgencyCompanyJobPosition,
  getAgencyCompanyJobPositionById,
  getAgencyCompanyJobPositions,
  updateAgencyCompanyJobPosition,
} from '@/modules/agency/recruiting/clients/api';
import { JobPositionSortBy } from '@/modules/agency/recruiting/clients/types';
import { showAlertConfirm, showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { AgencyCompanyJobPosition } from '@/modules/agency/recruiting/clients/types';

const securityStore = reactive<{ userRoles: string[] }>({ userRoles: [] });
const { route } = vi.hoisted(() => ({ route: { path: '/recruiting/companies/company-1', params: { id: 'company-1' }, query: {} } }));

vi.mock('vue-router', async (importOriginal) => ({
  ...await importOriginal<typeof import('vue-router')>(),
  useRoute: () => route,
}));
vi.mock('@/app/stores/security', () => ({ useSecurityStore: () => securityStore }));
vi.mock('@/modules/agency/recruiting/clients/api', () => ({
  getAgencyCompanyJobPositions: vi.fn(),
  deleteAgencyCompanyJobPosition: vi.fn(),
  createAgencyCompanyJobPosition: vi.fn(),
  createAgencyCompanyJobPositionWithoutRates: vi.fn(),
  updateAgencyCompanyJobPosition: vi.fn(),
  getAgencyCompanyJobPositionById: vi.fn(),
}));
vi.mock('@/shared/utils/toast', () => ({ showAlertConfirm: vi.fn(), showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));
vi.mock('@/shared/company-profile/JobPositionForm.vue', () => ({
  default: {
    name: 'JobPositionForm',
    props: { kind: String, create: Function, update: Function, load: Function, currentPosition: Object, isDuplicate: Boolean },
    emits: ['updateContent'],
    template: '<div class="job-position-form-stub" />',
  },
}));
vi.mock('@/modules/agency/recruiting/clients/components/RolesShiftDetail.vue', () => ({
  default: { name: 'RolesShiftDetail', props: { displayShift: String, roleId: String, companyProfileId: String }, template: '<span class="roles-shift-stub" />' },
}));

const packer: AgencyCompanyJobPosition = {
  id: 'jp-1',
  jobPosition: 'Packer',
  rate: 30,
  workerRate: 20,
  workerRateMax: 24,
  createdBy: 'jane.doe@sigook.com',
  createdAt: '2026-03-10T15:00:00Z',
  displayShift: 'Mon-Fri 8-4',
  description: 'Line packing',
};
const driver: AgencyCompanyJobPosition = { id: 'jp-2', jobPosition: 'Driver', rate: 0, workerRate: 0 };

async function setup(items: AgencyCompanyJobPosition[] = [packer, driver]) {
  vi.mocked(getAgencyCompanyJobPositions).mockResolvedValue({ items, totalItems: items.length, pageIndex: 1, totalPages: 1 });
  const wrapper = mountWithPlugins(JobPositionList);
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function headers(wrapper: Wrapper) {
  return wrapper.findAll('thead th').map((th) => th.text()).filter(Boolean);
}

function row(wrapper: Wrapper, index: number) {
  return wrapper.findAll('tbody tr:not(.detail)')[index];
}

async function openActions(wrapper: Wrapper, index: number) {
  await row(wrapper, index).find('td:last-child .dropdown').trigger('click');
  await flushPromises();
}

function menuItems() {
  return Array.from(document.body.querySelectorAll<HTMLElement>('.dropdown-item')).map((i) => i.textContent.trim());
}

function menuItem(text: string) {
  const item = Array.from(document.body.querySelectorAll<HTMLElement>('.dropdown-item')).find((i) => i.textContent.trim() === text);
  if (!item) throw new Error(`${text} not found`);
  return item;
}

function form(wrapper: Wrapper) {
  return wrapper.findComponent({ name: 'JobPositionForm' });
}

beforeEach(() => {
  securityStore.userRoles = ['recruiting'];
});

describe('JobPositionList', () => {
  it('loads the roles of the routed company sorted by role', async () => {
    await setup();

    expect(getAgencyCompanyJobPositions).toHaveBeenCalledExactlyOnceWith('company-1', expect.objectContaining({
      sortBy: JobPositionSortBy.Role,
      isDescending: false,
      role: '',
      pageIndex: 1,
    }));
  });

  it('hides the agency rate and duplication from non admins', async () => {
    const wrapper = await setup();

    expect(headers(wrapper)).not.toContain('Agency Rate');
    expect(row(wrapper, 0).text()).toContain('$20.00');
    expect(row(wrapper, 1).text()).toContain('Rates pending');
    await openActions(wrapper, 0);
    expect(menuItems()).toEqual(['Edit Role', 'Delete Role']);
  });

  it('shows the agency rate and duplication to admins', async () => {
    securityStore.userRoles = ['admin'];
    const wrapper = await setup();

    expect(headers(wrapper)).toContain('Agency Rate');
    expect(row(wrapper, 0).text()).toContain('$30.00');
    await openActions(wrapper, 0);
    expect(menuItems()).toEqual(['Edit Role', 'Duplicate Role', 'Delete Role']);
  });

  it('shows the shift of roles that have one', async () => {
    const wrapper = await setup();
    const shifts = wrapper.findAllComponents({ name: 'RolesShiftDetail' });

    expect(shifts).toHaveLength(1);
    expect(shifts[0].props()).toEqual({ displayShift: 'Mon-Fri 8-4', roleId: 'jp-1', companyProfileId: 'company-1' });
  });

  it('opens a basic form for non admins that creates roles without rates', async () => {
    vi.mocked(createAgencyCompanyJobPositionWithoutRates).mockResolvedValue({ id: 'jp-3' });
    const wrapper = await setup();
    await wrapper.findAll('button').find((b) => b.text() === 'Add')?.trigger('click');

    expect(form(wrapper).props('kind')).toBe('basic');
    expect(form(wrapper).props('isDuplicate')).toBe(false);
    await (form(wrapper).props('create') as (m: AgencyCompanyJobPosition) => Promise<unknown>)({ jobPosition: 'Picker' });

    expect(createAgencyCompanyJobPositionWithoutRates).toHaveBeenCalledExactlyOnceWith('company-1', { jobPosition: 'Picker' });
    expect(createAgencyCompanyJobPosition).not.toHaveBeenCalled();
  });

  it('opens a full form for admins wired to the company endpoints', async () => {
    securityStore.userRoles = ['admin'];
    vi.mocked(createAgencyCompanyJobPosition).mockResolvedValue({ id: 'jp-3' });
    vi.mocked(updateAgencyCompanyJobPosition).mockResolvedValue();
    vi.mocked(getAgencyCompanyJobPositionById).mockResolvedValue(packer);
    const wrapper = await setup();
    await wrapper.findAll('button').find((b) => b.text() === 'Add')?.trigger('click');
    const props = form(wrapper).props();

    expect(props.kind).toBe('full');
    await (props.create as (m: AgencyCompanyJobPosition) => Promise<unknown>)({ jobPosition: 'Picker', rate: 25 });
    await (props.update as (id: string, m: AgencyCompanyJobPosition) => Promise<unknown>)('jp-1', packer);
    await (props.load as (id: string) => Promise<unknown>)('jp-1');

    expect(createAgencyCompanyJobPosition).toHaveBeenCalledExactlyOnceWith('company-1', { jobPosition: 'Picker', rate: 25 });
    expect(updateAgencyCompanyJobPosition).toHaveBeenCalledExactlyOnceWith('company-1', 'jp-1', packer);
    expect(getAgencyCompanyJobPositionById).toHaveBeenCalledExactlyOnceWith('company-1', 'jp-1');
  });

  it('edits a role and reloads the grid once saved', async () => {
    const wrapper = await setup();
    await openActions(wrapper, 0);
    menuItem('Edit Role').click();
    await flushPromises();

    expect(form(wrapper).props('currentPosition')).toEqual(packer);
    expect(form(wrapper).props('isDuplicate')).toBe(false);

    form(wrapper).vm.$emit('updateContent', 'jp-1');
    await flushPromises();

    expect(getAgencyCompanyJobPositions).toHaveBeenCalledTimes(2);
    expect(showAlertSuccess).not.toHaveBeenCalled();
    expect(wrapper.findComponent({ name: 'BModal' }).props('modelValue')).toBe(false);
  });

  it('duplicates a role and confirms it', async () => {
    securityStore.userRoles = ['admin'];
    const wrapper = await setup();
    await openActions(wrapper, 0);
    menuItem('Duplicate Role').click();
    await flushPromises();

    expect(form(wrapper).props('isDuplicate')).toBe(true);
    form(wrapper).vm.$emit('updateContent', 'jp-9');
    await flushPromises();

    expect(showAlertSuccess).toHaveBeenCalledWith('Role duplicated');
    expect(getAgencyCompanyJobPositions).toHaveBeenCalledTimes(2);
  });

  it('deletes a role after confirming and reloads', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(deleteAgencyCompanyJobPosition).mockResolvedValue();
    const wrapper = await setup();
    await openActions(wrapper, 1);
    menuItem('Delete Role').click();
    await flushPromises();

    expect(showAlertConfirm).toHaveBeenCalledWith('Are you sure', 'You want to delete this position');
    expect(deleteAgencyCompanyJobPosition).toHaveBeenCalledExactlyOnceWith('company-1', 'jp-2');
    expect(showAlertSuccess).toHaveBeenCalledWith('Deleted');
    expect(getAgencyCompanyJobPositions).toHaveBeenCalledTimes(2);
  });

  it('keeps the role when deletion is cancelled and shows delete errors', async () => {
    const error = new Error('in use');
    vi.mocked(showAlertConfirm).mockResolvedValueOnce(false).mockResolvedValueOnce(true);
    vi.mocked(deleteAgencyCompanyJobPosition).mockRejectedValue(error);
    const wrapper = await setup();

    await openActions(wrapper, 0);
    menuItem('Delete Role').click();
    await flushPromises();
    expect(deleteAgencyCompanyJobPosition).not.toHaveBeenCalled();

    await openActions(wrapper, 0);
    menuItem('Delete Role').click();
    await flushPromises();
    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(wrapper.findComponent({ name: 'BLoading' }).props('modelValue')).toBe(false);
  });

  it('searches by role', async () => {
    const wrapper = await setup();
    await wrapper.find('thead input').setValue('pack');
    await wrapper.find('thead input').trigger('keyup.enter');
    await flushPromises();

    expect(getAgencyCompanyJobPositions).toHaveBeenLastCalledWith('company-1', expect.objectContaining({ role: 'pack', pageIndex: 1 }));
  });
});
