import { beforeEach, describe, expect, it, vi } from 'vitest';
import { computed, nextTick, reactive, ref } from 'vue';
import { flushPromises } from '@vue/test-utils';
import TableRequests from '@/modules/agency/recruiting/requests/components/TableRequests.vue';
import { bulkCancelRequests, bulkUpdateRecruiters, getAgencyRequests } from '@/modules/agency/recruiting/requests/api';
import { updateIsAsapRequests } from '@/modules/agency/recruiting/clients/api';
import { getSalesRequests } from '@/modules/agency/sales/api';
import { getSourcesForRequests } from '@/shared/api/catalogApi';
import { createAgencyRequestNote, deleteAgencyRequestNote, getAgencyRequestNotes, updateAgencyRequestNote } from '@/modules/agency/shared/notes/api';
import { RequestStatus } from '@/shared/constants/enums';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { AgencyRequestFilter, AgencyRequestListItem, AgencyRequestsPagedResponse, TableRequestsConfig } from '@/modules/agency/recruiting/requests/types';

const { childStub } = vi.hoisted(() => ({
  childStub: (name: string, props: string[], emits: string[] = []) => ({ name, props, emits, template: `<div class="${name}" />` }),
}));

const admin = ref(true);
const route = reactive<{ path: string; query: Record<string, string>; params: Record<string, string> }>({ path: '/recruiting/requests', query: {}, params: {} });
const agencyStore = reactive<{ agencyRequestFilter: AgencyRequestFilter | null; updateAgencyRequestFilter: (f: AgencyRequestFilter | null) => void }>({
  agencyRequestFilter: null,
  updateAgencyRequestFilter: vi.fn(),
});

vi.mock('vue-router', async (importOriginal) => ({
  ...await importOriginal<typeof import('vue-router')>(),
  useRoute: () => route,
}));
vi.mock('@/modules/agency/store', () => ({ useAgencyStore: () => agencyStore }));
vi.mock('@/shared/composables/useAdmin', () => ({ useAdmin: () => ({ isAdmin: computed(() => admin.value) }) }));
vi.mock('@/modules/agency/recruiting/requests/api', () => ({
  getAgencyRequests: vi.fn(),
  bulkCancelRequests: vi.fn(),
  bulkUpdateRecruiters: vi.fn(),
  getAgencyRequestShift: vi.fn(),
}));
vi.mock('@/modules/agency/recruiting/clients/api', () => ({ updateIsAsapRequests: vi.fn() }));
vi.mock('@/modules/agency/sales/api', () => ({ getSalesRequests: vi.fn() }));
vi.mock('@/shared/api/catalogApi', () => ({ getSourcesForRequests: vi.fn() }));
vi.mock('@/shared/api/reportApi', () => ({ downloadAgencyReport: vi.fn() }));
vi.mock('@/modules/agency/shared/notes/api', () => ({
  getAgencyRequestNotes: vi.fn(),
  createAgencyRequestNote: vi.fn(),
  updateAgencyRequestNote: vi.fn(),
  deleteAgencyRequestNote: vi.fn(),
}));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));
vi.mock('@/modules/agency/shared/AgencyFilter.vue', () => ({ default: childStub('AgencyFilter', ['modelValue'], ['update:modelValue', 'change']) }));
vi.mock('@/modules/agency/shared/AgencyBadge.vue', () => ({ default: childStub('AgencyBadge', ['agencyId']) }));
vi.mock('@/modules/agency/shared/notes/NotesPopover.vue', () => ({
  default: childStub('NotesPopover', ['userId', 'notesCount', 'canCreate', 'onGet', 'onCreate', 'onUpdate', 'onDelete'], ['update:count']),
}));
vi.mock('@/shared/request-detail/AgencyShiftDetail.vue', () => ({ default: childStub('AgencyShiftDetail', ['requestId', 'displayShift', 'fetchShift']) }));
vi.mock('@/shared/request-detail/CompanyCancelList.vue', () => ({ default: childStub('CompanyCancelList', [], ['sendReason']) }));
vi.mock('@/modules/agency/recruiting/requests/components/JobBoardsModal.vue', () => ({
  default: childStub('JobBoardsModal', ['requestId', 'numberId', 'currentBoards'], ['saved', 'close']),
}));
vi.mock('@/modules/agency/recruiting/requests/components/BulkRecruiterModal.vue', () => ({
  default: childStub('BulkRecruiterModal', ['requestCount'], ['submit', 'cancel']),
}));

function item(overrides: Partial<AgencyRequestListItem> = {}): AgencyRequestListItem {
  return {
    id: 'req-1',
    agencyId: 'ag-1',
    numberId: 1001,
    jobTitle: 'Forklift operator',
    billingTitle: 'Operator II',
    createdAt: '2026-11-10T12:00:00',
    entrance: 'Door 4',
    companyFullName: 'Acme Foods',
    companyProfileId: 'cp-1',
    requestStatus: RequestStatus.Open,
    workersQuantity: 5,
    workersQuantityWorking: 2,
    isAsap: true,
    workerRate: 21.5,
    workerSalary: null,
    displayRecruiters: 'maria|john',
    displayReportTo: 'Bob Boss',
    displayShift: 'Mon-Fri',
    salesRepresentative: 'Sam Sales',
    notesCount: 3,
    vaccinationRequired: true,
    punchCardOptionEnabled: false,
    hasPermissionToSeeInternalRequests: true,
    location: '100 King St',
    locationAddress: '100 King St, Toronto',
    jobBoards: [{ sourceId: 'src-1', value: 'Indeed' }],
    ...overrides,
  };
}

const first = item();
const second = item({
  id: 'req-2',
  numberId: 1002,
  isAsap: false,
  workerRate: null,
  workerSalary: 55000,
  requestStatus: RequestStatus.Filled,
  workersQuantity: 3,
  workersQuantityWorking: 3,
  displayRecruiters: undefined,
  displayReportTo: undefined,
  billingTitle: undefined,
  entrance: undefined,
  salesRepresentative: undefined,
  vaccinationRequired: false,
  jobBoards: [],
});

function response(items: AgencyRequestListItem[]): AgencyRequestsPagedResponse {
  return { items, pageIndex: 1, totalPages: 1, totalItems: items.length, jobBoardsSummary: [{ sourceId: 'src-1', value: 'Indeed', count: 1 }] };
}

interface Props {
  companyProfileId?: string;
  agencyId?: string;
  config?: Partial<TableRequestsConfig>;
}

async function setup(props: Props = {}, items: AgencyRequestListItem[] = [first, second]) {
  vi.mocked(getAgencyRequests).mockResolvedValue(response(items.map((i) => ({ ...i }))));
  vi.mocked(getSalesRequests).mockResolvedValue(response(items.map((i) => ({ ...i }))));
  vi.mocked(getSourcesForRequests).mockResolvedValue([{ id: 'src-1', value: 'Indeed' }, { id: 'src-2', value: 'Kijiji' }]);
  const wrapper = mountWithPlugins(TableRequests, { props });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function rows(wrapper: Wrapper) {
  return wrapper.findAll('tbody tr');
}

function cell(wrapper: Wrapper, row: number, label: string) {
  const found = rows(wrapper)[row].find(`td[data-label="${label}"]`);
  if (!found.exists()) throw new Error(`${label} cell not found`);
  return found;
}

function rowActions() {
  return Array.from(document.body.querySelectorAll<HTMLElement>('.dropdown-item[role="listitem"]')).filter((i) => i.textContent.trim() !== 'Export');
}

function lastFilter() {
  return vi.mocked(getAgencyRequests).mock.lastCall?.[0];
}

async function check(wrapper: Wrapper, ...indexes: number[]) {
  for (const index of indexes) {
    await rows(wrapper)[index].find('td.checkbox-cell label').trigger('click');
  }
  await nextTick();
}

async function openQuickActions(wrapper: Wrapper) {
  const trigger = wrapper.findAll('.sigook-grid-actions button').find((b) => b.text() === 'Quick Actions');
  if (!trigger) throw new Error('Quick actions not found');
  await trigger.trigger('click');
  await nextTick();
}

function quickAction(text: string) {
  const found = Array.from(document.body.querySelectorAll<HTMLElement>('.dropdown-item[role="menuitem"]')).find((i) => i.textContent.trim() === text);
  if (!found) throw new Error(`${text} not found`);
  return found;
}

beforeEach(() => {
  admin.value = true;
  route.path = '/recruiting/requests';
  route.query = {};
  agencyStore.agencyRequestFilter = null;
});

describe('TableRequests', () => {
  it('loads the requests with the default filter and remembers it', async () => {
    const wrapper = await setup();

    expect(getAgencyRequests).toHaveBeenCalledWith(expect.objectContaining({ onlyMine: false, sortBy: 0, isDescending: true, pageIndex: 1 }));
    expect(agencyStore.updateAgencyRequestFilter).toHaveBeenCalledWith(lastFilter());
    expect(wrapper.emitted('update:totalItems')).toEqual([[2]]);
    expect(wrapper.emitted('onDataLoading')).toEqual([[true], [false]]);
    expect(wrapper.find('.job-boards-summary').text()).toBe('Posted in:Indeed (1)');
  });

  it('only shows my requests by default for non admins', async () => {
    admin.value = false;
    await setup();

    expect(lastFilter()?.onlyMine).toBe(true);
  });

  it('renders each request row', async () => {
    const wrapper = await setup();

    expect(cell(wrapper, 0, 'ID').text()).toBe('1001Asap');
    expect(cell(wrapper, 0, 'ID').find('a').attributes('href')).toBe('/recruiting/requests/req-1');
    expect(cell(wrapper, 0, 'ID').find('.mdi-needle').exists()).toBe(true);
    expect(cell(wrapper, 0, 'Client').find('a').attributes('href')).toBe('/recruiting/companies/cp-1');
    expect(cell(wrapper, 0, 'Client').text()).toContain('Bob Boss');
    expect(cell(wrapper, 0, 'Location').text().replace(/\s+/g, ' ')).toBe('100 King St - Door 4');
    expect(cell(wrapper, 0, 'Position').text()).toContain('Operator II');
    expect(cell(wrapper, 0, 'Created').text()).toContain('10-Nov-2026');
    expect(cell(wrapper, 0, 'Recruiter').text()).toBe('maria | john');
    expect(cell(wrapper, 0, 'Sales Rep').text()).toBe('Sam Sales');
    expect(cell(wrapper, 0, 'Rate / Salary').text()).toBe('$21.50');
    expect(cell(wrapper, 0, 'Job Boards').text()).toBe('Indeed');
    expect(cell(wrapper, 0, 'Status').find('.dot-status').classes()).toContain('status-inprogress');

    expect(cell(wrapper, 1, 'ID').text()).toContain('DH');
    expect(cell(wrapper, 1, 'ID').text()).not.toContain('Asap');
    expect(cell(wrapper, 1, 'Recruiter').text()).toBe('—');
    expect(cell(wrapper, 1, 'Rate / Salary').text()).toBe('$55,000.00');
    expect(cell(wrapper, 1, 'Job Boards').find('.job-boards-cell__add').exists()).toBe(true);
    expect(cell(wrapper, 1, 'Status').find('.dot-status').classes()).toContain('status-filled');
    expect(cell(wrapper, 1, 'Status').find('.request-check').exists()).toBe(true);
    expect(wrapper.find('thead').text()).toContain('(5 / 8)');
  });

  it('wires the notes popover to the request notes api', async () => {
    vi.mocked(getAgencyRequestNotes).mockResolvedValue({ items: [], pageIndex: 1, totalPages: 1, totalItems: 0 });
    vi.mocked(createAgencyRequestNote).mockResolvedValue({ id: 'n-1', note: 'New note' });
    vi.mocked(updateAgencyRequestNote).mockResolvedValue();
    vi.mocked(deleteAgencyRequestNote).mockResolvedValue();
    const wrapper = await setup();
    const popover = wrapper.findComponent({ name: 'NotesPopover' });
    type Callback = (payload: Record<string, unknown>) => Promise<unknown>;
    const model = { note: 'x', color: '#fff' };
    await (popover.props('onGet') as Callback)({ userId: 'req-1', pagination: { page: 1, size: 5 } });
    await (popover.props('onCreate') as Callback)({ userId: 'req-1', model });
    await (popover.props('onUpdate') as Callback)({ userId: 'req-1', id: 'n-1', model });
    await (popover.props('onDelete') as Callback)({ userId: 'req-1', id: 'n-1' });

    expect(getAgencyRequestNotes).toHaveBeenCalledWith('req-1', { page: 1, size: 5 });
    expect(createAgencyRequestNote).toHaveBeenCalledWith('req-1', model);
    expect(updateAgencyRequestNote).toHaveBeenCalledWith('req-1', 'n-1', model);
    expect(deleteAgencyRequestNote).toHaveBeenCalledWith('req-1', 'n-1');
    popover.vm.$emit('update:count', 7);
    await nextTick();
    expect(popover.props('notesCount')).toBe(7);
  });

  it('opens the request from a cell and the workers tab from the workers cell', async () => {
    const wrapper = await setup();
    await cell(wrapper, 0, 'Location').trigger('click');
    await flushPromises();
    expect(wrapper.vm.$router.currentRoute.value.fullPath).toBe('/recruiting/requests/req-1');

    const workers = rows(wrapper)[0].findAll('td').find((td) => td.text() === '2 / 5');
    if (!workers) throw new Error('workers cell not found');
    await workers.trigger('click');
    await flushPromises();
    expect(wrapper.vm.$router.currentRoute.value.fullPath).toBe('/recruiting/requests/req-1?tab=Workers');
  });

  it('does not navigate from recruiter, notes or actions cells', async () => {
    const wrapper = await setup();
    await cell(wrapper, 0, 'Recruiter').trigger('click');
    await cell(wrapper, 0, 'Notes').trigger('click');
    await flushPromises();

    expect(wrapper.vm.$router.currentRoute.value.fullPath).toBe('/');
  });

  it('manages the job boards of a request and reloads when saved', async () => {
    const wrapper = await setup();
    await cell(wrapper, 0, 'Job Boards').trigger('click');
    await nextTick();
    const modal = wrapper.findComponent({ name: 'JobBoardsModal' });
    expect(modal.props()).toEqual({ requestId: 'req-1', numberId: 1001, currentBoards: [{ sourceId: 'src-1', value: 'Indeed' }] });

    modal.vm.$emit('saved');
    await flushPromises();
    expect(getAgencyRequests).toHaveBeenCalledTimes(2);
    modal.vm.$emit('close');
    await nextTick();
    expect(wrapper.findAllComponents({ name: 'BModal' })[2].props('modelValue')).toBe(false);
  });

  it('offers edit and duplicate actions', async () => {
    const wrapper = await setup();
    await rows(wrapper)[0].find('td[data-label="actions"] .dropdown, td:last-child .dropdown').trigger('click');
    await nextTick();
    await nextTick();
    const items = rowActions();
    expect(items.map((i) => i.textContent.trim())).toEqual(['Edit Request', 'Duplicate Request']);

    items[0].click();
    await flushPromises();
    expect(wrapper.vm.$router.currentRoute.value.fullPath).toBe('/recruiting/requests/update/cp-1/req-1');
    items[1].click();
    await flushPromises();
    expect(wrapper.vm.$router.currentRoute.value.fullPath).toBe('/recruiting/requests/duplicate/cp-1/req-1');
  });

  it('hides edit for a cancelled request', async () => {
    const wrapper = await setup({}, [item({ requestStatus: RequestStatus.Cancelled })]);
    await rows(wrapper)[0].find('td:last-child .dropdown').trigger('click');
    await nextTick();
    await nextTick();

    expect(rowActions().map((i) => i.textContent.trim())).toEqual(['Duplicate Request']);
  });

  it('filters by status', async () => {
    const wrapper = await setup();
    const statusInput = wrapper.findAllComponents({ name: 'BTaginput' })[1];
    expect((statusInput.props('data') as { value: string }[]).map((s) => s.value)).toEqual(['Pending Review', 'Open', 'Filled', 'Cancelled']);
    statusInput.vm.$emit('update:modelValue', [{ id: RequestStatus.Open, value: 'Open' }]);
    await flushPromises();

    expect(lastFilter()).toEqual(expect.objectContaining({ statuses: [RequestStatus.Open], pageIndex: 1 }));
  });

  it('filters by job boards', async () => {
    const wrapper = await setup();
    const boards = wrapper.findAllComponents({ name: 'BTaginput' })[0];
    expect(boards.props('data')).toEqual([{ id: 'src-1', value: 'Indeed' }, { id: 'src-2', value: 'Kijiji' }]);
    boards.vm.$emit('update:modelValue', [{ id: 'src-2', value: 'Kijiji' }]);
    await flushPromises();

    expect(lastFilter()?.jobBoardIds).toEqual(['src-2']);
  });

  it('filters by creation dates and clears them', async () => {
    const wrapper = await setup();
    const picker = wrapper.findComponent({ name: 'BDatepicker' });
    const from = new Date(2026, 10, 1);
    const to = new Date(2026, 10, 15);
    picker.vm.$emit('update:modelValue', [from, to]);
    await flushPromises();
    expect(lastFilter()).toEqual(expect.objectContaining({ createdAtFrom: from.toISOString(), createdAtTo: to.toISOString() }));

    picker.vm.$emit('icon-right-click');
    await flushPromises();
    expect(lastFilter()).toEqual(expect.objectContaining({ createdAtFrom: null, createdAtTo: null }));
  });

  it('filters assigned to me and by agency', async () => {
    const wrapper = await setup();
    await wrapper.find('.sigook-grid input[type="checkbox"]').setValue(true);
    await flushPromises();
    expect(lastFilter()?.onlyMine).toBe(true);

    const agencyFilter = wrapper.findComponent({ name: 'AgencyFilter' });
    agencyFilter.vm.$emit('update:modelValue', 'ag-2');
    agencyFilter.vm.$emit('change');
    await flushPromises();
    expect(lastFilter()?.agencyIds).toBe('ag-2');
  });

  it('restores the remembered filter', async () => {
    agencyStore.agencyRequestFilter = {
      sortBy: 2,
      isDescending: false,
      statuses: [RequestStatus.Filled],
      createdAtFrom: '2026-11-01T04:00:00.000Z',
      createdAtTo: '2026-11-15T05:00:00.000Z',
      jobBoardIds: ['src-1'],
    };
    const wrapper = await setup();

    expect(lastFilter()).toEqual(expect.objectContaining({ sortBy: 2, isDescending: false, statuses: [RequestStatus.Filled] }));
    expect(wrapper.findAllComponents({ name: 'BTaginput' })[1].props('modelValue')).toEqual([{ id: RequestStatus.Filled, value: 'Filled' }]);
    expect(wrapper.findAllComponents({ name: 'BTaginput' })[0].props('modelValue')).toEqual([{ id: 'src-1', value: 'Indeed' }]);
    expect(wrapper.findComponent({ name: 'BDatepicker' }).props('modelValue')).toEqual([new Date('2026-11-01T04:00:00.000Z'), new Date('2026-11-15T05:00:00.000Z')]);
  });

  it('applies the pending review filter from the query', async () => {
    agencyStore.agencyRequestFilter = { sortBy: 3, isDescending: true, pageSize: 50, onlyMine: true, companyFullName: 'Acme' };
    route.query = { status: 'pending-review' };
    const wrapper = await setup();

    expect(lastFilter()).toEqual({ sortBy: 3, isDescending: true, pageSize: 50, onlyMine: false, statuses: [RequestStatus.PendingReview], pageIndex: 1 });
    expect(wrapper.findAllComponents({ name: 'BTaginput' })[1].props('modelValue')).toEqual([{ id: RequestStatus.PendingReview, value: 'Pending Review' }]);
  });

  it('switches to pending review when the query changes', async () => {
    await setup();
    route.query = { status: 'other' };
    await flushPromises();
    expect(getAgencyRequests).toHaveBeenCalledTimes(1);

    route.query = { status: 'pending-review' };
    await flushPromises();
    expect(lastFilter()?.statuses).toEqual([RequestStatus.PendingReview]);
  });

  it('scopes the list to a company without global filters', async () => {
    const wrapper = await setup({ companyProfileId: 'cp-1', config: { showAssignedToMe: false, showQuickActions: false, enableCheckable: false, showSalesRepColumn: false, showNotesColumn: false } });

    expect(lastFilter()).toEqual(expect.objectContaining({ companyProfileId: 'cp-1', onlyMine: false }));
    expect(agencyStore.updateAgencyRequestFilter).not.toHaveBeenCalled();
    expect(wrapper.findComponent({ name: 'AgencyFilter' }).exists()).toBe(false);
    expect(wrapper.find('thead').text()).not.toContain('Client');
    expect(wrapper.find('thead').text()).not.toContain('Sales Rep');
    expect(wrapper.find('td.checkbox-cell').exists()).toBe(false);
    expect(wrapper.text()).not.toContain('Quick Actions');
  });

  it('scopes the list to an agency', async () => {
    await setup({ agencyId: 'ag-9' });

    expect(lastFilter()).toEqual(expect.objectContaining({ agencyId: 'ag-9', onlyMine: false }));
  });

  it('uses the sales endpoints in the sales view', async () => {
    route.path = '/sales/requests';
    const wrapper = await setup();

    expect(getSalesRequests).toHaveBeenCalled();
    expect(getAgencyRequests).not.toHaveBeenCalled();
    expect(cell(wrapper, 0, 'ID').find('a').attributes('href')).toBe('/sales/requests/req-1');
    expect(cell(wrapper, 0, 'Client').find('a').attributes('href')).toBe('/sales/companies/cp-1');
  });

  it('disables the quick actions until rows are checked', async () => {
    const wrapper = await setup();
    await openQuickActions(wrapper);

    expect(quickAction('Cancel requests').classList).toContain('is-disabled');
    expect(quickAction('Assign / Unassign recruiters').classList).toContain('is-disabled');
  });

  it('hides recruiter assignment for non admins', async () => {
    admin.value = false;
    const wrapper = await setup();
    await openQuickActions(wrapper);

    expect(() => quickAction('Assign / Unassign recruiters')).toThrow();
  });

  it('marks the checked requests as asap', async () => {
    vi.mocked(updateIsAsapRequests).mockResolvedValue();
    const wrapper = await setup();
    await check(wrapper, 1);
    await openQuickActions(wrapper);
    const toggle = document.body.querySelector<HTMLInputElement>('.quick-action-item input');
    if (!toggle) throw new Error('asap switch not found');
    expect(toggle.checked).toBe(false);
    toggle.click();
    await flushPromises();

    expect(updateIsAsapRequests).toHaveBeenCalledExactlyOnceWith({ ids: ['req-2'], isAsap: true });
    expect(getAgencyRequests).toHaveBeenCalledTimes(2);
  });

  it('shows the error when the asap update fails', async () => {
    const error = new Error('boom');
    vi.mocked(updateIsAsapRequests).mockRejectedValue(error);
    const wrapper = await setup();
    await check(wrapper, 0);
    await openQuickActions(wrapper);
    document.body.querySelector<HTMLInputElement>('.quick-action-item input')?.click();
    await flushPromises();

    expect(updateIsAsapRequests).toHaveBeenCalledWith({ ids: ['req-1'], isAsap: false });
    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(wrapper.emitted('onDataLoading')?.at(-1)).toEqual([false]);
  });

  it('cancels the checked requests', async () => {
    vi.mocked(bulkCancelRequests).mockResolvedValue({ cancelled: 1, skipped: 1 });
    const wrapper = await setup();
    await check(wrapper, 0, 1);
    await openQuickActions(wrapper);
    quickAction('Cancel requests').click();
    await nextTick();
    wrapper.findComponent({ name: 'CompanyCancelList' }).vm.$emit('sendReason', { reasonId: 'r-1', otherMessage: 'Client closed' });
    await flushPromises();

    expect(bulkCancelRequests).toHaveBeenCalledExactlyOnceWith({ ids: ['req-1', 'req-2'], cancellationReasonId: 'r-1', otherCancellationReason: 'Client closed' });
    expect(showAlertSuccess).toHaveBeenCalledWith('Cancelled 1 order(s), skipped 1');
    expect(getAgencyRequests).toHaveBeenCalledTimes(2);
  });

  it('shows the error when cancelling fails', async () => {
    const error = new Error('boom');
    vi.mocked(bulkCancelRequests).mockRejectedValue(error);
    const wrapper = await setup();
    await check(wrapper, 0);
    await openQuickActions(wrapper);
    quickAction('Cancel requests').click();
    await nextTick();
    wrapper.findComponent({ name: 'CompanyCancelList' }).vm.$emit('sendReason', { reasonId: 'r-1', otherMessage: '' });
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(wrapper.findAllComponents({ name: 'BModal' })[0].props('modelValue')).toBe(false);
  });

  it('assigns recruiters to the checked requests', async () => {
    vi.mocked(bulkUpdateRecruiters).mockResolvedValue();
    const wrapper = await setup();
    await check(wrapper, 0);
    await openQuickActions(wrapper);
    quickAction('Assign / Unassign recruiters').click();
    await nextTick();
    const modal = wrapper.findComponent({ name: 'BulkRecruiterModal' });
    expect(modal.props('requestCount')).toBe(1);
    modal.vm.$emit('submit', ['p-1']);
    await flushPromises();

    expect(bulkUpdateRecruiters).toHaveBeenCalledExactlyOnceWith({ ids: ['req-1'], recruiterIds: ['p-1'] });
    expect(showAlertSuccess).toHaveBeenCalledWith('Recruiters assigned');
  });

  it('unassigns recruiters', async () => {
    vi.mocked(bulkUpdateRecruiters).mockResolvedValue();
    const wrapper = await setup();
    await check(wrapper, 0);
    await openQuickActions(wrapper);
    quickAction('Assign / Unassign recruiters').click();
    await nextTick();
    wrapper.findComponent({ name: 'BulkRecruiterModal' }).vm.$emit('submit', []);
    await flushPromises();

    expect(showAlertSuccess).toHaveBeenCalledWith('Recruiters unassigned');
  });

  it('shows the error when assigning fails and closes on cancel', async () => {
    const error = new Error('boom');
    vi.mocked(bulkUpdateRecruiters).mockRejectedValue(error);
    const wrapper = await setup();
    await check(wrapper, 0);
    await openQuickActions(wrapper);
    quickAction('Assign / Unassign recruiters').click();
    await nextTick();
    wrapper.findComponent({ name: 'BulkRecruiterModal' }).vm.$emit('submit', ['p-1']);
    await flushPromises();
    expect(showAlertError).toHaveBeenCalledWith(error);

    await openQuickActions(wrapper);
    quickAction('Assign / Unassign recruiters').click();
    await nextTick();
    wrapper.findComponent({ name: 'BulkRecruiterModal' }).vm.$emit('cancel');
    await nextTick();
    expect(wrapper.findAllComponents({ name: 'BModal' })[1].props('modelValue')).toBe(false);
  });
});
