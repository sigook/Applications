import { beforeEach, describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import { createPinia, setActivePinia, type Pinia } from 'pinia';
import Workers from '@/modules/agency/recruiting/workers/pages/Workers.vue';
import { getAgencyWorkers, updateApprovedToWork } from '@/modules/agency/recruiting/workers/api';
import { useAgencyStore } from '@/modules/agency/store';
import { showAlertConfirm, showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { formatPhone } from '@/shared/utils/phoneFormat';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { AgencyWorkerFilter, AgencyWorkerListItem } from '@/modules/agency/recruiting/workers/types';
import type { PaginatedList } from '@/shared/types/common';

vi.mock('@/modules/agency/recruiting/workers/api', () => ({ getAgencyWorkers: vi.fn(), updateApprovedToWork: vi.fn() }));
vi.mock('@/shared/api/reportApi', () => ({ downloadAgencyReport: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertConfirm: vi.fn(), showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));

const approved = {
  id: 'wp-1',
  agencyId: 'agency-1',
  fullName: 'John Smith',
  email: 'john@example.com',
  approvedToWork: true,
  dnu: true,
  numberId: 1001,
  createdAt: '2026-01-15T12:00:00',
  profileImage: 'https://files/john.png',
  externalId: 'EXT-1',
  mobileNumber: '4165550000',
  address: 'Toronto, ON',
  isSubcontractor: true,
  isCurrentlyWorking: true,
  requests: [{ id: 'req-1', value: '501' }],
  skills: ['Forklift', 'Packing', 'Driving', 'Welding'],
};

const pending = {
  ...approved,
  id: 'wp-2',
  fullName: 'Ana Lopez',
  approvedToWork: false,
  dnu: false,
  profileImage: null,
  isSubcontractor: false,
  isCurrentlyWorking: false,
  requests: [],
  skills: [],
};

let pinia: Pinia;

function page(items: AgencyWorkerListItem[]): PaginatedList<AgencyWorkerListItem> {
  return { items, totalItems: 42, pageIndex: 1, totalPages: 2 };
}

async function setup(items: AgencyWorkerListItem[] = [approved, pending]) {
  vi.mocked(getAgencyWorkers).mockResolvedValue(page(items));
  const wrapper = mountWithPlugins(Workers, {
    global: { plugins: [pinia], components: { 'default-image': { props: ['name'], template: '<span class="default-image" />' } } },
  });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function rows(wrapper: Wrapper) {
  return wrapper.findAll('tbody tr');
}

async function openActions(wrapper: Wrapper, index: number) {
  await rows(wrapper)[index].find('td:last-child .dropdown').trigger('click');
  await flushPromises();
}

function menuItem(text: string) {
  const item = Array.from(document.body.querySelectorAll<HTMLElement>('.dropdown-item')).find((i) => i.textContent.trim() === text);
  if (!item) throw new Error(`${text} not found`);
  return item;
}

function hasMenuItem(text: string) {
  return Array.from(document.body.querySelectorAll<HTMLElement>('.dropdown-item')).some((i) => i.textContent.trim() === text);
}

function currentPath(wrapper: Wrapper) {
  return wrapper.vm.$router.currentRoute.value.fullPath;
}

beforeEach(() => {
  pinia = createPinia();
  setActivePinia(pinia);
});

describe('Workers', () => {
  it('loads the workers sorted by name and keeps the filter in the store', async () => {
    const wrapper = await setup();

    expect(getAgencyWorkers).toHaveBeenCalledWith(expect.objectContaining({ sortBy: 0, isDescending: false, pageIndex: 1 }));
    expect(useAgencyStore().agencyWorkerProfileFilter).toMatchObject({ sortBy: 0, isDescending: false });
    expect(wrapper.find('.page-header').text()).toContain('42');
  });

  it('renders the worker details', async () => {
    const wrapper = await setup();
    const first = rows(wrapper)[0];

    expect(first.find('img').attributes('src')).toBe('https://files/john.png');
    expect(first.text()).toContain('John Smith');
    expect(first.text()).toContain('EXT-1');
    expect(first.text()).toContain('Toronto, ON');
    expect(first.text()).toContain('See more...');
    expect(first.findAll('.tag').map((t) => t.text())).toEqual(expect.arrayContaining(['501', 'Forklift', 'Working', 'DNU', 'Approved To Work', 'Subcontractor']));
    expect(first.find('.Blue').text()).toBe('1001');

    const second = rows(wrapper)[1];
    expect(second.find('img').exists()).toBe(false);
    expect(second.text()).toContain('Request ID');
    expect(second.text()).not.toContain('Working');
  });

  it('opens the worker when a cell is clicked', async () => {
    const wrapper = await setup();
    await rows(wrapper)[0].findAll('td')[2].trigger('click');
    await flushPromises();

    expect(currentPath(wrapper)).toBe('/recruiting/workers/wp-1');
  });

  it('does not open the worker from the requests or actions cells', async () => {
    const wrapper = await setup();
    const cells = rows(wrapper)[1].findAll('td');
    await cells[6].trigger('click');
    await cells[cells.length - 1].trigger('click');
    await flushPromises();

    expect(currentPath(wrapper)).toBe('/');
  });

  it('opens the request applicants from a request tag', async () => {
    const wrapper = await setup();
    await rows(wrapper)[0].findAll('td')[6].find('.tag span').trigger('click');
    await flushPromises();

    expect(currentPath(wrapper)).toBe('/recruiting/requests/req-1?tab=Applicants');
  });

  it('edits the worker from the row menu', async () => {
    const wrapper = await setup();
    await openActions(wrapper, 0);
    menuItem('Edit').click();
    await flushPromises();

    expect(currentPath(wrapper)).toBe('/recruiting/workers/wp-1');
  });

  it('approves a worker from the row menu and reloads', async () => {
    vi.mocked(updateApprovedToWork).mockResolvedValue();
    const wrapper = await setup();
    await openActions(wrapper, 1);
    expect(hasMenuItem('Reject to work')).toBe(false);
    menuItem('Approve to work').click();
    await flushPromises();

    expect(updateApprovedToWork).toHaveBeenCalledWith('wp-2');
    expect(showAlertSuccess).toHaveBeenCalledWith('Updated');
    expect(getAgencyWorkers).toHaveBeenCalledTimes(2);
  });

  it('rejects an approved worker once confirmed', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(updateApprovedToWork).mockResolvedValue();
    const wrapper = await setup();
    await openActions(wrapper, 0);
    menuItem('Reject to work').click();
    await flushPromises();

    expect(showAlertConfirm).toHaveBeenCalledWith('Are you sure?', 'You want to disable the worker. This worker will not be able to apply to new requests');
    expect(updateApprovedToWork).toHaveBeenCalledWith('wp-1');
  });

  it('keeps the worker when the rejection is cancelled', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(false);
    const wrapper = await setup();
    await openActions(wrapper, 0);
    menuItem('Reject to work').click();
    await flushPromises();

    expect(updateApprovedToWork).not.toHaveBeenCalled();
  });

  it('shows the error when the approval fails and reloads', async () => {
    const error = new Error('approve failed');
    vi.mocked(updateApprovedToWork).mockRejectedValue(error);
    const wrapper = await setup();
    await openActions(wrapper, 1);
    menuItem('Approve to work').click();
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(getAgencyWorkers).toHaveBeenCalledTimes(2);
  });

  it('filters by features', async () => {
    const wrapper = await setup();
    wrapper.findComponent({ name: 'BTaginput' }).vm.$emit('update:modelValue', [{ id: 2, value: 'DNU' }, { id: 4, value: 'Subcontractor' }]);
    await flushPromises();

    expect(getAgencyWorkers).toHaveBeenLastCalledWith(expect.objectContaining({ features: [2, 4], pageIndex: 1 }));
  });

  it('filters by created date range and clears it', async () => {
    const wrapper = await setup();
    const from = new Date('2026-01-01T00:00:00');
    const to = new Date('2026-01-31T00:00:00');
    const picker = wrapper.findComponent({ name: 'BDatepicker' });

    picker.vm.$emit('update:modelValue', [from, to]);
    await flushPromises();
    expect(getAgencyWorkers).toHaveBeenLastCalledWith(expect.objectContaining({ createdAtFrom: from.toISOString(), createdAtTo: to.toISOString() }));

    picker.vm.$emit('icon-right-click');
    await flushPromises();
    expect(getAgencyWorkers).toHaveBeenLastCalledWith(expect.objectContaining({ createdAtFrom: null, createdAtTo: null }));
  });

  it('formats the phone search', async () => {
    const wrapper = await setup();
    await wrapper.findAll('thead input[placeholder="Search..."]')[3].setValue('4165551234');
    await wrapper.find('button[aria-label="Refresh"]').trigger('click');
    await flushPromises();

    expect(getAgencyWorkers).toHaveBeenLastCalledWith(expect.objectContaining({ phone: formatPhone('4165551234') }));
  });

  it('restores the saved filter', async () => {
    const saved: AgencyWorkerFilter = {
      sortBy: 3,
      isDescending: true,
      features: [3],
      createdAtFrom: '2026-01-01T05:00:00.000Z',
      createdAtTo: '2026-01-31T05:00:00.000Z',
    };
    useAgencyStore().updateAgencyWorkerProfileFilter(saved);
    const wrapper = await setup();

    expect(getAgencyWorkers).toHaveBeenCalledWith(expect.objectContaining({ sortBy: 3, isDescending: true, features: [3] }));
    expect(wrapper.findComponent({ name: 'BTaginput' }).props('modelValue')).toEqual([{ id: 3, value: 'Approved To Work' }]);
    expect(wrapper.findComponent({ name: 'BDatepicker' }).props('modelValue')).toEqual([
      new Date('2026-01-01T05:00:00.000Z'),
      new Date('2026-01-31T05:00:00.000Z'),
    ]);
  });
});
