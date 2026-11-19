import { beforeEach, describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import { createPinia, setActivePinia, type Pinia } from 'pinia';
import Runners from '@/modules/agency/recruiting/runners/pages/Runners.vue';
import { changeAgencyRunnersStatus, getAgencyRunners } from '@/modules/agency/recruiting/runners/api';
import { createAgencyRunner } from '@/modules/agency/recruiting/requests/runners/api';
import { RunnerStatus, RunnerType } from '@/modules/agency/recruiting/requests/runners/types';
import { AgencyRunnerSortBy } from '@/modules/agency/recruiting/runners/types';
import { reportBulkResult } from '@/modules/agency/shared/request-groups/bulkReport';
import { useAgencyStore } from '@/modules/agency/store';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { AgencyRequestRunners, AgencyRunner, AgencyRunnersPagedResponse } from '@/modules/agency/recruiting/runners/types';

const { stub } = vi.hoisted(() => ({
  stub: (name: string, props: string[], emits: string[], template = `<div class="${name}" />`) => ({ name, props, emits, template }),
}));

vi.mock('@/modules/agency/recruiting/runners/api', () => ({ changeAgencyRunnersStatus: vi.fn(), getAgencyRunners: vi.fn() }));
vi.mock('@/modules/agency/recruiting/requests/runners/api', () => ({ createAgencyRunner: vi.fn() }));
vi.mock('@/modules/agency/shared/request-groups/bulkReport', () => ({ reportBulkResult: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));
vi.mock('@/modules/agency/shared/request-groups/RequestGroup.vue', () => ({
  default: stub('RequestGroup', ['group', 'countLabel', 'fill', 'open'], ['update:open'],
    '<div class="RequestGroup"><div class="group-tags"><slot name="tags" /></div><b-dropdown><template #trigger><button class="group-menu" /></template><slot name="menu" /></b-dropdown><slot /></div>'),
}));
vi.mock('@/modules/agency/shared/request-groups/RequestGroupFilters.vue', () => ({
  default: stub('RequestGroupFilters', ['filter', 'statusOptions'], ['search', 'update:filter'], '<div class="RequestGroupFilters"><slot /></div>'),
}));
vi.mock('@/modules/agency/shared/request-groups/RequestGroupsToolbar.vue', () => ({
  default: stub('RequestGroupsToolbar', ['onlyMine', 'agencyIds', 'hasGroups', 'exportUrl', 'exportParams', 'exportFileName'], ['search', 'expandAll', 'collapseAll', 'loading', 'update:onlyMine', 'update:agencyIds']),
}));
vi.mock('@/modules/agency/shared/request-groups/RequestGroupsPagination.vue', () => ({ default: stub('RequestGroupsPagination', ['page', 'total', 'perPage'], ['change']) }));
vi.mock('@/modules/agency/recruiting/runners/components/RunnersGrid.vue', () => ({
  default: stub('RunnersGrid', ['data', 'canEdit', 'checkable', 'checkedRows'], ['update:checkedRows', 'updated']),
}));
vi.mock('@/modules/agency/recruiting/runners/components/BulkRunnerStatusModal.vue', () => ({ default: stub('BulkRunnerStatusModal', ['count', 'isSaving'], ['submit', 'close']) }));
vi.mock('@/modules/agency/recruiting/requests/runners/CreateRunner.vue', () => ({ default: stub('CreateRunner', ['requestId', 'isSaving'], ['create', 'close']) }));

function runner(overrides: Partial<AgencyRunner>): AgencyRunner {
  return {
    id: 'run-1',
    requestId: 'req-1',
    workerProfileId: 'wp-1',
    name: 'John Smith',
    type: RunnerType.Active,
    status: RunnerStatus.SentToClient,
    interviewsCount: 0,
    createdAt: '2026-03-01T12:00:00',
    numberId: 1,
    createdBy: 'u1',
    requestNumberId: 501,
    companyFullName: 'Acme Foods',
    jobTitle: 'Packer',
    isAsap: false,
    isDirectHiring: false,
    usesRunners: true,
    ...overrides,
  };
}

const john = runner({});
const ana = runner({ id: 'run-2', name: 'Ana Lopez' });

function group(overrides: Partial<AgencyRequestRunners> = {}): AgencyRequestRunners {
  return {
    requestId: 'req-1',
    agencyId: 'agency-1',
    numberId: 501,
    companyFullName: 'Acme Foods',
    jobTitle: 'Packer',
    isAsap: false,
    isDirectHiring: false,
    usesRunners: true,
    workersQuantity: 3,
    hiredRunners: 1,
    totalRunners: 2,
    runners: [john, ana],
    ...overrides,
  };
}

function response(groups: AgencyRequestRunners[]): AgencyRunnersPagedResponse {
  return { items: groups, totalItems: 20, pageIndex: 1, totalPages: 2, totalRunners: 37 };
}

let pinia: Pinia;

async function setup(groups: AgencyRequestRunners[] = [group()]) {
  vi.mocked(getAgencyRunners).mockResolvedValue(response(groups));
  const wrapper = mountWithPlugins(Runners, { global: { plugins: [pinia] } });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function groups(wrapper: Wrapper) {
  return wrapper.findAllComponents({ name: 'RequestGroup' });
}

function menuItem(wrapper: Wrapper, text: string, index = 0) {
  return groups(wrapper)[index].findAll('.dropdown-item').find((i) => i.text() === text);
}

async function select(wrapper: Wrapper, rows: AgencyRunner[], index = 0) {
  wrapper.findAllComponents({ name: 'RunnersGrid' })[index].vm.$emit('update:checkedRows', rows);
  await flushPromises();
}

function modals(wrapper: Wrapper) {
  return wrapper.findAllComponents({ name: 'BModal' }).map((m) => m.props('modelValue') as boolean);
}

async function openBulk(wrapper: Wrapper) {
  await wrapper.findAll('.bulk-selection button').find((b) => b.text() === 'Change status')?.trigger('click');
  await flushPromises();
  return wrapper.findComponent({ name: 'BulkRunnerStatusModal' });
}

beforeEach(() => {
  pinia = createPinia();
  setActivePinia(pinia);
});

describe('Runners', () => {
  it('loads the first page of requests with their runners', async () => {
    const wrapper = await setup();

    expect(getAgencyRunners).toHaveBeenCalledWith({ pageIndex: 1, pageSize: 15, sortBy: AgencyRunnerSortBy.Name, isDescending: true, onlyMine: false });
    expect(useAgencyStore().agencyRunnersFilter).toMatchObject({ pageIndex: 1, pageSize: 15 });
    expect(wrapper.find('.page-header').text()).toContain('37');
    expect(wrapper.findComponent({ name: 'RequestGroupsPagination' }).props()).toEqual({ page: 1, total: 20, perPage: 15 });
  });

  it('starts from the saved filter', async () => {
    useAgencyStore().updateAgencyRunnersFilter({ pageIndex: 3, type: RunnerType.Passive });
    await setup();

    expect(getAgencyRunners).toHaveBeenCalledWith(expect.objectContaining({ pageIndex: 3, type: RunnerType.Passive }));
  });

  it('shows the empty state', async () => {
    const wrapper = await setup([]);

    expect(wrapper.text()).toContain('No records available');
    expect(wrapper.findComponent({ name: 'RequestGroupsToolbar' }).props('hasGroups')).toBe(false);
  });

  it('shows the error when the runners fail to load', async () => {
    const error = new Error('load failed');
    vi.mocked(getAgencyRunners).mockRejectedValue(error);
    mountWithPlugins(Runners, { global: { plugins: [pinia] } });
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('labels each request and passes its runners to the grid', async () => {
    const wrapper = await setup([group(), group({ requestId: 'req-2', totalRunners: 5, usesRunners: false })]);

    expect(groups(wrapper).map((g) => g.props('countLabel'))).toEqual(['2 runners', '2 of 5 runners']);
    expect(groups(wrapper)[0].props('fill')).toEqual({ value: 1, total: 3, label: 'hired' });
    const grids = wrapper.findAllComponents({ name: 'RunnersGrid' });
    expect(grids[0].props()).toEqual({ data: [john, ana], canEdit: true, checkable: true, checkedRows: [] });
    expect(grids[1].props('canEdit')).toBe(false);
  });

  it('marks requests without runners as read only and hides their add action', async () => {
    const wrapper = await setup([group(), group({ requestId: 'req-2', usesRunners: false })]);

    expect(groups(wrapper)[0].find('.group-tags').text()).toBe('');
    expect(groups(wrapper)[1].find('.group-tags').text()).toBe('Read-only');
    expect(menuItem(wrapper, 'Add Runner', 0)).toBeDefined();
    expect(menuItem(wrapper, 'Add Runner', 1)).toBeUndefined();
  });

  it('expands, collapses and toggles request groups', async () => {
    const wrapper = await setup([group(), group({ requestId: 'req-2' })]);
    const toolbar = wrapper.findComponent({ name: 'RequestGroupsToolbar' });

    toolbar.vm.$emit('expandAll');
    await flushPromises();
    expect(groups(wrapper).map((g) => g.props('open'))).toEqual([true, true]);

    toolbar.vm.$emit('collapseAll');
    await flushPromises();
    expect(groups(wrapper).map((g) => g.props('open'))).toEqual([false, false]);

    groups(wrapper)[0].vm.$emit('update:open', true);
    await flushPromises();
    expect(groups(wrapper).map((g) => g.props('open'))).toEqual([true, false]);
  });

  it('filters by type from the first page', async () => {
    const wrapper = await setup();
    wrapper.findComponent({ name: 'RequestGroupsPagination' }).vm.$emit('change', 2);
    await flushPromises();
    expect(getAgencyRunners).toHaveBeenLastCalledWith(expect.objectContaining({ pageIndex: 2 }));

    await wrapper.find('.RequestGroupFilters select').setValue(String(RunnerType.Active));
    await flushPromises();
    expect(getAgencyRunners).toHaveBeenLastCalledWith(expect.objectContaining({ type: RunnerType.Active, pageIndex: 1 }));
  });

  it('reloads from the filters, the toolbar and the grids', async () => {
    const wrapper = await setup();
    wrapper.findComponent({ name: 'RequestGroupFilters' }).vm.$emit('search');
    wrapper.findComponent({ name: 'RequestGroupsToolbar' }).vm.$emit('search');
    wrapper.findComponent({ name: 'RunnersGrid' }).vm.$emit('updated');
    await flushPromises();

    expect(getAgencyRunners).toHaveBeenCalledTimes(4);
  });

  it('changes the status of the runners selected across requests', async () => {
    vi.mocked(changeAgencyRunnersStatus).mockResolvedValue({ updated: 2, skipped: [{ runnerId: 'run-3', reason: 'Already hired' }] });
    const lia = runner({ id: 'run-3', requestId: 'req-2', name: 'Lia Park' });
    const wrapper = await setup([group(), group({ requestId: 'req-2', runners: [lia] })]);
    await select(wrapper, [john, ana], 0);
    await select(wrapper, [lia], 1);

    expect(wrapper.find('.bulk-selection-count').text()).toBe('3 selected');
    expect(wrapper.findAllComponents({ name: 'RunnersGrid' })[1].props('checkedRows')).toEqual([lia]);
    const modal = await openBulk(wrapper);
    expect(modal.props()).toEqual({ count: 3, isSaving: false });
    modal.vm.$emit('submit', { status: RunnerStatus.Hired, startDate: '2026-04-06T04:00:00.000Z', comments: 'Welcome' });
    await flushPromises();

    expect(changeAgencyRunnersStatus).toHaveBeenCalledWith({
      runnerIds: ['run-1', 'run-2', 'run-3'],
      status: RunnerStatus.Hired,
      startDate: '2026-04-06T04:00:00.000Z',
      comments: 'Welcome',
    });
    expect(reportBulkResult).toHaveBeenCalledWith(2, [{ id: 'run-3', reason: 'Already hired' }], new Map([['run-1', 'John Smith'], ['run-2', 'Ana Lopez'], ['run-3', 'Lia Park']]), 'runners');
    expect(getAgencyRunners).toHaveBeenCalledTimes(2);
    expect(modals(wrapper)).toEqual([false, false]);
    expect(wrapper.find('.bulk-selection').exists()).toBe(false);
  });

  it('keeps the modal open and shows the error when the bulk change fails', async () => {
    const error = new Error('bulk failed');
    vi.mocked(changeAgencyRunnersStatus).mockRejectedValue(error);
    const wrapper = await setup();
    await select(wrapper, [john]);
    const modal = await openBulk(wrapper);
    modal.vm.$emit('submit', { status: RunnerStatus.NoShow });
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(modals(wrapper)).toEqual([false, true]);
  });

  it('ignores a bulk change without selection', async () => {
    const wrapper = await setup();
    await select(wrapper, [john]);
    const modal = await openBulk(wrapper);
    await select(wrapper, []);
    modal.vm.$emit('submit', { status: RunnerStatus.NoShow });
    await flushPromises();

    expect(changeAgencyRunnersStatus).not.toHaveBeenCalled();
  });

  it('clears the selection', async () => {
    const wrapper = await setup();
    await select(wrapper, [john]);
    await wrapper.findAll('.bulk-selection button').find((b) => b.text() === 'Clear')?.trigger('click');

    expect(wrapper.find('.bulk-selection').exists()).toBe(false);
  });

  it('adds a runner to the request', async () => {
    vi.mocked(createAgencyRunner).mockResolvedValue('run-9');
    const wrapper = await setup();
    await menuItem(wrapper, 'Add Runner')?.trigger('click');
    await flushPromises();

    const create = wrapper.findComponent({ name: 'CreateRunner' });
    expect(create.props()).toEqual({ requestId: 'req-1', isSaving: false });
    create.vm.$emit('create', { workerProfileId: 'wp-9', type: RunnerType.Passive });
    await flushPromises();

    expect(createAgencyRunner).toHaveBeenCalledWith('req-1', { workerProfileId: 'wp-9', type: RunnerType.Passive });
    expect(modals(wrapper)).toEqual([false, false]);
    expect(getAgencyRunners).toHaveBeenCalledTimes(2);
  });

  it('keeps the form open and shows the error when adding fails', async () => {
    const error = new Error('create failed');
    vi.mocked(createAgencyRunner).mockRejectedValue(error);
    const wrapper = await setup();
    await menuItem(wrapper, 'Add Runner')?.trigger('click');
    await flushPromises();
    wrapper.findComponent({ name: 'CreateRunner' }).vm.$emit('create', { workerProfileId: 'wp-9', type: RunnerType.Active });
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(wrapper.findComponent({ name: 'CreateRunner' }).props('isSaving')).toBe(false);
    expect(modals(wrapper)).toEqual([true, false]);
  });

  it('opens the request from its menu', async () => {
    const wrapper = await setup();
    await menuItem(wrapper, 'Open Request')?.trigger('click');
    await flushPromises();

    expect(wrapper.vm.$router.currentRoute.value.path).toBe('/recruiting/requests/req-1');
  });
});
