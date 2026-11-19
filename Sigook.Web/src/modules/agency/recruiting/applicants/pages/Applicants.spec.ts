import { beforeEach, describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import { createPinia, setActivePinia, type Pinia } from 'pinia';
import Applicants from '@/modules/agency/recruiting/applicants/pages/Applicants.vue';
import {
  changeAgencyApplicantsStatus,
  deleteAgencyRequestApplicant,
  getAgencyApplicants,
  postAgencyRequestApplicant,
  updateAgencyRequestApplicant,
} from '@/modules/agency/recruiting/requests/api';
import { convertCandidateToWorker } from '@/modules/agency/recruiting/candidates/api';
import { createAgencyRunner } from '@/modules/agency/recruiting/requests/runners/api';
import { RunnerType } from '@/modules/agency/recruiting/requests/runners/types';
import { reportBulkResult } from '@/modules/agency/shared/request-groups/bulkReport';
import { useAgencyStore } from '@/modules/agency/store';
import { AgencyApplicantSortBy } from '@/modules/agency/recruiting/applicants/types';
import { RequestApplicantStatus } from '@/shared/request-detail/requestApplicant';
import { showAlertConfirm, showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { AgencyApplicant, AgencyApplicantsPagedResponse, AgencyRequestApplicants } from '@/modules/agency/recruiting/applicants/types';

const { stub } = vi.hoisted(() => ({
  stub: (name: string, props: string[], emits: string[], template = `<div class="${name}" />`) => ({ name, props, emits, template }),
}));

vi.mock('@/modules/agency/recruiting/requests/api', () => ({
  changeAgencyApplicantsStatus: vi.fn(),
  deleteAgencyRequestApplicant: vi.fn(),
  getAgencyApplicants: vi.fn(),
  postAgencyRequestApplicant: vi.fn(),
  updateAgencyRequestApplicant: vi.fn(),
}));
vi.mock('@/modules/agency/recruiting/candidates/api', () => ({ convertCandidateToWorker: vi.fn() }));
vi.mock('@/modules/agency/recruiting/requests/runners/api', () => ({ createAgencyRunner: vi.fn() }));
vi.mock('@/modules/agency/shared/request-groups/bulkReport', () => ({ reportBulkResult: vi.fn() }));
vi.mock('@/shared/api/reportApi', () => ({ downloadAgencyReport: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertConfirm: vi.fn(), showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));
vi.mock('@/modules/agency/shared/request-groups/RequestGroup.vue', () => ({
  default: stub('RequestGroup', ['group', 'countLabel', 'fill', 'open'], ['update:open'],
    '<div class="RequestGroup"><b-dropdown><template #trigger><button class="group-menu" /></template><slot name="menu" /></b-dropdown><slot /></div>'),
}));
vi.mock('@/modules/agency/shared/request-groups/RequestGroupFilters.vue', () => ({ default: stub('RequestGroupFilters', ['filter', 'statusOptions'], ['search', 'update:filter']) }));
vi.mock('@/modules/agency/shared/request-groups/RequestGroupsToolbar.vue', () => ({
  default: stub('RequestGroupsToolbar', ['onlyMine', 'agencyIds', 'hasGroups', 'exportUrl', 'exportParams', 'exportFileName'], ['search', 'expandAll', 'collapseAll', 'loading', 'update:onlyMine', 'update:agencyIds']),
}));
vi.mock('@/modules/agency/shared/request-groups/RequestGroupsPagination.vue', () => ({ default: stub('RequestGroupsPagination', ['page', 'total', 'perPage'], ['change']) }));
vi.mock('@/modules/agency/recruiting/requests/components/ManageApplicantsModal.vue', () => ({ default: stub('ManageApplicantsModal', ['requestId'], ['updateApplicants']) }));
vi.mock('@/modules/agency/recruiting/applicants/components/ApplicantComplianceDetail.vue', () => ({
  default: stub('ApplicantComplianceDetail', ['requestId', 'applicantId', 'name', 'status', 'workerProfileId'], ['loaded', 'statusChanged']),
}));
vi.mock('@/shared/ui/EditTextarea.vue', () => ({ default: stub('EditTextarea', ['title', 'minLength', 'data'], ['updateContent']) }));
vi.mock('@/modules/agency/recruiting/candidates/components/DetailCandidate.vue', () => ({ default: stub('DetailCandidate', ['candidateId'], ['onUpdateWorker']) }));
vi.mock('@/modules/agency/recruiting/requests/runners/SelectRunnerTypeModal.vue', () => ({ default: stub('SelectRunnerTypeModal', ['name'], ['select', 'close']) }));

function applicant(overrides: Partial<AgencyApplicant>): AgencyApplicant {
  return {
    id: 'a1',
    candidateId: 'c1',
    workerProfileId: null,
    name: 'Ana Lopez',
    email: 'ana@example.com',
    comments: 'Call back',
    status: RequestApplicantStatus.Pending,
    createdBy: 'jane.doe@sigook.com',
    createdAt: '2026-03-01T12:00:00',
    requestId: 'req-1',
    requestNumberId: 501,
    companyFullName: 'Acme Foods',
    jobTitle: 'Packer',
    isAsap: false,
    isDirectHiring: false,
    complianceTotal: 4,
    complianceCompleted: 1,
    mandatoryPending: 2,
    ...overrides,
  };
}

const candidate = applicant({});
const worker = applicant({ id: 'a2', candidateId: null, workerProfileId: 'wp-2', name: 'John Smith', status: RequestApplicantStatus.InProgress, complianceTotal: 0, complianceCompleted: 0, mandatoryPending: 0 });

function group(overrides: Partial<AgencyRequestApplicants> = {}): AgencyRequestApplicants {
  return {
    requestId: 'req-1',
    agencyId: 'agency-1',
    numberId: 501,
    companyFullName: 'Acme Foods',
    jobTitle: 'Packer',
    isAsap: false,
    isDirectHiring: false,
    workersQuantity: 5,
    confirmedApplicants: 1,
    totalApplicants: 2,
    applicants: [candidate, worker],
    ...overrides,
  };
}

function response(groups: AgencyRequestApplicants[]): AgencyApplicantsPagedResponse {
  return { items: groups, totalItems: 31, pageIndex: 1, totalPages: 3, totalApplicants: 64 };
}

let pinia: Pinia;

async function setup(groups: AgencyRequestApplicants[] = [group()]) {
  vi.mocked(getAgencyApplicants).mockResolvedValue(response(groups.map((g) => ({ ...g, applicants: g.applicants.map((a) => ({ ...a })) }))));
  const wrapper = mountWithPlugins(Applicants, { global: { plugins: [pinia] } });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function grid(wrapper: Wrapper, index = 0) {
  return wrapper.findAllComponents({ name: 'SigookGrid' })[index];
}

function rows(wrapper: Wrapper) {
  return wrapper.findAll('tbody tr').filter((r) => !r.classes('detail'));
}

async function openRowActions(wrapper: Wrapper, index: number) {
  await rows(wrapper)[index].find('td:last-child .dropdown').trigger('click');
  await flushPromises();
}

function bodyItem(text: string) {
  const item = Array.from(document.body.querySelectorAll<HTMLElement>('.dropdown-item')).find((i) => i.textContent.trim() === text);
  if (!item) throw new Error(`${text} not found`);
  return item;
}

function bodyItems() {
  return Array.from(document.body.querySelectorAll<HTMLElement>('.dropdown-item')).map((i) => i.textContent.trim());
}

function bulkButton(wrapper: Wrapper, text: string) {
  const found = wrapper.findAll('.bulk-selection button').find((b) => b.text() === text);
  if (!found) throw new Error(`${text} not found`);
  return found;
}

async function select(wrapper: Wrapper, applicants: AgencyApplicant[], index = 0) {
  grid(wrapper, index).vm.$emit('update:checkedRows', applicants);
  await flushPromises();
}

function path(wrapper: Wrapper) {
  return wrapper.vm.$router.currentRoute.value.fullPath;
}

beforeEach(() => {
  pinia = createPinia();
  setActivePinia(pinia);
});

describe('Applicants', () => {
  it('loads the first page of requests with their applicants', async () => {
    const wrapper = await setup();

    expect(getAgencyApplicants).toHaveBeenCalledWith({ pageIndex: 1, pageSize: 15, sortBy: AgencyApplicantSortBy.Name, isDescending: true, onlyMine: false });
    expect(useAgencyStore().agencyApplicantsFilter).toMatchObject({ pageIndex: 1, pageSize: 15 });
    expect(wrapper.find('.page-header').text()).toContain('64');
    expect(wrapper.findComponent({ name: 'RequestGroupsPagination' }).props()).toEqual({ page: 1, total: 31, perPage: 15 });
    expect(wrapper.findComponent({ name: 'RequestGroupsToolbar' }).props('hasGroups')).toBe(true);
  });

  it('starts from the saved filter', async () => {
    useAgencyStore().updateAgencyApplicantsFilter({ pageIndex: 2, onlyMine: true, name: 'ana' });
    await setup();

    expect(getAgencyApplicants).toHaveBeenCalledWith(expect.objectContaining({ pageIndex: 2, onlyMine: true, name: 'ana', pageSize: 15 }));
  });

  it('shows the empty state when no request matches', async () => {
    const wrapper = await setup([]);

    expect(wrapper.text()).toContain('No records available');
    expect(wrapper.findComponent({ name: 'RequestGroup' }).exists()).toBe(false);
  });

  it('shows the error when the applicants fail to load', async () => {
    const error = new Error('load failed');
    vi.mocked(getAgencyApplicants).mockRejectedValue(error);
    mountWithPlugins(Applicants, { global: { plugins: [pinia] } });
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('labels each request with its applicants and confirmed fill', async () => {
    const wrapper = await setup([group(), group({ requestId: 'req-2', totalApplicants: 9 })]);
    const groups = wrapper.findAllComponents({ name: 'RequestGroup' });

    expect(groups[0].props('countLabel')).toBe('2 applicants');
    expect(groups[1].props('countLabel')).toBe('2 of 9 applicants');
    expect(groups[0].props('fill')).toEqual({ value: 1, total: 5, label: 'confirmed' });
    expect(groups[0].props('open')).toBe(false);
  });

  it('renders the applicant rows', async () => {
    const wrapper = await setup();
    const [first, second] = rows(wrapper);

    expect(first.text()).toContain('Ana Lopez');
    expect(first.text()).toContain('Pending');
    expect(first.text()).toContain('1/4');
    expect(first.text()).toContain('Call back');
    expect(first.find('.applicants-compliance-bar span').attributes('style')).toContain('width: 25%');
    expect(first.find('.applicants-compliance-bar span').classes()).not.toContain('is-complete');
    expect(second.text()).toContain('In progress');
    expect(second.find('.applicants-compliance-bar span').attributes('style')).toContain('width: 0%');
    expect(second.find('.applicants-compliance-bar span').classes()).toContain('is-complete');
  });

  it('expands, collapses and toggles request groups', async () => {
    const wrapper = await setup([group(), group({ requestId: 'req-2' })]);
    const toolbar = wrapper.findComponent({ name: 'RequestGroupsToolbar' });

    toolbar.vm.$emit('expandAll');
    await flushPromises();
    expect(wrapper.findAllComponents({ name: 'RequestGroup' }).map((g) => g.props('open'))).toEqual([true, true]);

    toolbar.vm.$emit('collapseAll');
    await flushPromises();
    expect(wrapper.findAllComponents({ name: 'RequestGroup' }).map((g) => g.props('open'))).toEqual([false, false]);

    wrapper.findAllComponents({ name: 'RequestGroup' })[1].vm.$emit('update:open', true);
    await flushPromises();
    expect(wrapper.findAllComponents({ name: 'RequestGroup' }).map((g) => g.props('open'))).toEqual([false, true]);
  });

  it('reloads from the filters, the toolbar and the pagination', async () => {
    const wrapper = await setup();
    wrapper.findComponent({ name: 'RequestGroupFilters' }).vm.$emit('search');
    wrapper.findComponent({ name: 'RequestGroupsToolbar' }).vm.$emit('search');
    await flushPromises();
    expect(getAgencyApplicants).toHaveBeenCalledTimes(3);

    wrapper.findComponent({ name: 'RequestGroupsPagination' }).vm.$emit('change', 3);
    await flushPromises();
    expect(getAgencyApplicants).toHaveBeenLastCalledWith(expect.objectContaining({ pageIndex: 3 }));
  });

  it('opens the worker from the name and toggles the checklist from status or compliance', async () => {
    const wrapper = await setup();
    const table = grid(wrapper);

    table.vm.$emit('cellclick', candidate, { field: 'name' });
    await flushPromises();
    expect(path(wrapper)).toBe('/');

    table.vm.$emit('cellclick', worker, { field: 'compliance' });
    await flushPromises();
    expect(grid(wrapper).props('openedDetailed')).toEqual(['a2']);

    table.vm.$emit('cellclick', worker, { field: 'status' });
    await flushPromises();
    expect(grid(wrapper).props('openedDetailed')).toEqual([]);

    table.vm.$emit('cellclick', worker, { field: 'comments' });
    table.vm.$emit('cellclick', worker, { field: 'name' });
    await flushPromises();
    expect(path(wrapper)).toBe('/recruiting/workers/wp-2');
  });

  it('updates the row progress when the checklist reports', async () => {
    const wrapper = await setup();
    grid(wrapper).vm.$emit('cellclick', candidate, { field: 'compliance' });
    await flushPromises();

    const detail = wrapper.findComponent({ name: 'ApplicantComplianceDetail' });
    expect(detail.props()).toEqual({ requestId: 'req-1', applicantId: 'a1', name: 'Ana Lopez', status: RequestApplicantStatus.Pending, workerProfileId: null });
    detail.vm.$emit('loaded', { mandatoryCompleted: true, itemsCount: 4, completedCount: 4 });
    await flushPromises();
    expect(rows(wrapper)[0].text()).toContain('4/4');
    expect(rows(wrapper)[0].find('.applicants-compliance-bar span').classes()).toContain('is-complete');

    detail.vm.$emit('loaded', { mandatoryCompleted: false, itemsCount: 4, completedCount: 3 });
    await flushPromises();
    expect(rows(wrapper)[0].find('.applicants-compliance-bar span').classes()).not.toContain('is-complete');

    detail.vm.$emit('statusChanged');
    await flushPromises();
    expect(getAgencyApplicants).toHaveBeenCalledTimes(2);
  });

  it('changes the status of the applicants selected across requests', async () => {
    vi.mocked(changeAgencyApplicantsStatus).mockResolvedValue({ updated: 1, skipped: [{ applicantId: 'a2', reason: 'Already confirmed' }] });
    const other = applicant({ id: 'a3', requestId: 'req-2', name: 'Lia Park' });
    const wrapper = await setup([group(), group({ requestId: 'req-2', applicants: [other] })]);
    await select(wrapper, [candidate, worker], 0);
    await select(wrapper, [other], 1);

    expect(wrapper.find('.bulk-selection-count').text()).toBe('3 selected');
    await bulkButton(wrapper, 'Confirm').trigger('click');
    await flushPromises();

    expect(changeAgencyApplicantsStatus).toHaveBeenCalledWith({ applicantIds: ['a1', 'a2', 'a3'], status: RequestApplicantStatus.Confirmed });
    expect(reportBulkResult).toHaveBeenCalledWith(1, [{ id: 'a2', reason: 'Already confirmed' }], new Map([['a1', 'Ana Lopez'], ['a2', 'John Smith'], ['a3', 'Lia Park']]), 'applicants');
    expect(getAgencyApplicants).toHaveBeenCalledTimes(2);
    expect(wrapper.find('.bulk-selection').exists()).toBe(false);
  });

  it('starts the selected applicants', async () => {
    vi.mocked(changeAgencyApplicantsStatus).mockResolvedValue({ updated: 1, skipped: [] });
    const wrapper = await setup();
    await select(wrapper, [candidate]);
    await bulkButton(wrapper, 'Start').trigger('click');
    await flushPromises();

    expect(changeAgencyApplicantsStatus).toHaveBeenCalledWith({ applicantIds: ['a1'], status: RequestApplicantStatus.InProgress });
    expect(showAlertConfirm).not.toHaveBeenCalled();
  });

  it('asks before cancelling the selected applicants', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValueOnce(false).mockResolvedValueOnce(true);
    vi.mocked(changeAgencyApplicantsStatus).mockResolvedValue({ updated: 2, skipped: [] });
    const wrapper = await setup();
    await select(wrapper, [candidate, worker]);

    await bulkButton(wrapper, 'Cancel').trigger('click');
    await flushPromises();
    expect(showAlertConfirm).toHaveBeenCalledWith('Are you sure', 'You want to cancel 2 applicants');
    expect(changeAgencyApplicantsStatus).not.toHaveBeenCalled();

    await bulkButton(wrapper, 'Cancel').trigger('click');
    await flushPromises();
    expect(changeAgencyApplicantsStatus).toHaveBeenCalledWith({ applicantIds: ['a1', 'a2'], status: RequestApplicantStatus.Cancelled });
  });

  it('shows the error when the bulk change fails', async () => {
    const error = new Error('bulk failed');
    vi.mocked(changeAgencyApplicantsStatus).mockRejectedValue(error);
    const wrapper = await setup();
    await select(wrapper, [candidate]);
    await bulkButton(wrapper, 'Confirm').trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(reportBulkResult).not.toHaveBeenCalled();
  });

  it('clears the selection', async () => {
    const wrapper = await setup();
    await select(wrapper, [candidate]);
    await bulkButton(wrapper, 'Clear').trigger('click');

    expect(wrapper.find('.bulk-selection').exists()).toBe(false);
  });

  it('adds an applicant to the request from its menu', async () => {
    vi.mocked(postAgencyRequestApplicant).mockResolvedValue(applicant({ id: 'a9' }));
    const wrapper = await setup();
    const item = wrapper.findAll('.RequestGroup .dropdown-item').find((i) => i.text() === 'Add Applicant');
    await item?.trigger('click');
    await flushPromises();

    const modal = wrapper.findComponent({ name: 'ManageApplicantsModal' });
    expect(modal.props('requestId')).toBe('req-1');
    modal.vm.$emit('updateApplicants', { model: { workerProfileId: 'wp-9' } });
    await flushPromises();

    expect(postAgencyRequestApplicant).toHaveBeenCalledWith('req-1', { workerProfileId: 'wp-9' });
    expect(getAgencyApplicants).toHaveBeenCalledTimes(2);
  });

  it('shows the error when adding an applicant fails', async () => {
    const error = new Error('add failed');
    vi.mocked(postAgencyRequestApplicant).mockRejectedValue(error);
    const wrapper = await setup();
    await wrapper.findAll('.RequestGroup .dropdown-item').find((i) => i.text() === 'Add Applicant')?.trigger('click');
    await flushPromises();
    wrapper.findComponent({ name: 'ManageApplicantsModal' }).vm.$emit('updateApplicants', { model: { workerProfileId: 'wp-9' } });
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('opens the request from its menu', async () => {
    const wrapper = await setup();
    await wrapper.findAll('.RequestGroup .dropdown-item').find((i) => i.text() === 'Open Request')?.trigger('click');
    await flushPromises();

    expect(path(wrapper)).toBe('/recruiting/requests/req-1');
  });

  it('edits the applicant comments', async () => {
    vi.mocked(updateAgencyRequestApplicant).mockResolvedValue();
    const wrapper = await setup();
    await rows(wrapper)[0].find('.applicants-comments button').trigger('click');
    await flushPromises();

    const editor = wrapper.findComponent({ name: 'EditTextarea' });
    expect(editor.props()).toEqual({ title: 'Comments', minLength: 0, data: 'Call back' });
    editor.vm.$emit('updateContent', 'Called, starts Monday');
    await flushPromises();

    expect(updateAgencyRequestApplicant).toHaveBeenCalledWith('req-1', 'a1', { comments: 'Called, starts Monday' });
    expect(rows(wrapper)[0].text()).toContain('Called, starts Monday');
  });

  it('shows the error when the comments fail to save', async () => {
    const error = new Error('comments failed');
    vi.mocked(updateAgencyRequestApplicant).mockRejectedValue(error);
    const wrapper = await setup();
    await rows(wrapper)[0].find('.applicants-comments button').trigger('click');
    await flushPromises();
    wrapper.findComponent({ name: 'EditTextarea' }).vm.$emit('updateContent', 'x');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(rows(wrapper)[0].text()).toContain('Call back');
  });

  it('offers candidate actions for a candidate and worker actions for a worker', async () => {
    const wrapper = await setup();
    await openRowActions(wrapper, 0);
    expect(bodyItems()).toEqual(expect.arrayContaining(['Edit Candidate', 'Convert to Worker', 'Delete']));
    expect(bodyItems()).not.toContain('Open Worker');

    await openRowActions(wrapper, 1);
    expect(bodyItems()).toEqual(expect.arrayContaining(['Open Worker', 'Add Runner']));
  });

  it('edits the candidate and reloads once it is updated', async () => {
    const wrapper = await setup();
    await openRowActions(wrapper, 0);
    bodyItem('Edit Candidate').click();
    await flushPromises();

    const detail = wrapper.findComponent({ name: 'DetailCandidate' });
    expect(detail.props('candidateId')).toBe('c1');
    detail.vm.$emit('onUpdateWorker', true);
    await flushPromises();
    expect(getAgencyApplicants).toHaveBeenCalledTimes(2);
  });

  it('converts a candidate to a worker', async () => {
    vi.mocked(convertCandidateToWorker).mockResolvedValue({ id: 'wp-9' });
    const wrapper = await setup();
    await openRowActions(wrapper, 0);
    bodyItem('Convert to Worker').click();
    await flushPromises();

    expect(convertCandidateToWorker).toHaveBeenCalledWith('c1');
    expect(getAgencyApplicants).toHaveBeenCalledTimes(2);
  });

  it('shows the error when the conversion fails', async () => {
    const error = new Error('convert failed');
    vi.mocked(convertCandidateToWorker).mockRejectedValue(error);
    const wrapper = await setup();
    await openRowActions(wrapper, 0);
    bodyItem('Convert to Worker').click();
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('opens the worker from the row menu', async () => {
    const wrapper = await setup();
    await openRowActions(wrapper, 1);
    bodyItem('Open Worker').click();
    await flushPromises();

    expect(path(wrapper)).toBe('/recruiting/workers/wp-2');
  });

  it('adds the worker as a runner of the request', async () => {
    vi.mocked(createAgencyRunner).mockResolvedValue('run-1');
    const wrapper = await setup();
    await openRowActions(wrapper, 1);
    bodyItem('Add Runner').click();
    await flushPromises();

    const modal = wrapper.findComponent({ name: 'SelectRunnerTypeModal' });
    expect(modal.props('name')).toBe('John Smith');
    modal.vm.$emit('select', RunnerType.Passive);
    await flushPromises();

    expect(createAgencyRunner).toHaveBeenCalledWith('req-1', { workerProfileId: 'wp-2', type: RunnerType.Passive });
    expect(showAlertSuccess).toHaveBeenCalledWith('Runner added');
  });

  it('shows the error when adding the runner fails', async () => {
    const error = new Error('runner failed');
    vi.mocked(createAgencyRunner).mockRejectedValue(error);
    const wrapper = await setup();
    await openRowActions(wrapper, 1);
    bodyItem('Add Runner').click();
    await flushPromises();
    wrapper.findComponent({ name: 'SelectRunnerTypeModal' }).vm.$emit('select', RunnerType.Active);
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('removes an applicant once confirmed', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(deleteAgencyRequestApplicant).mockResolvedValue();
    const wrapper = await setup([group({ applicants: [applicant({ name: '<b>Ana</b>' })] })]);
    await openRowActions(wrapper, 0);
    bodyItem('Delete').click();
    await flushPromises();

    expect(showAlertConfirm).toHaveBeenCalledWith('Are you sure', 'You want to remove &lt;b&gt;Ana&lt;/b&gt; from request 501');
    expect(deleteAgencyRequestApplicant).toHaveBeenCalledWith('req-1', 'a1');
    expect(getAgencyApplicants).toHaveBeenCalledTimes(2);
  });

  it('keeps the applicant when the removal is cancelled', async () => {
    vi.mocked(showAlertConfirm).mockResolvedValue(false);
    const wrapper = await setup();
    await openRowActions(wrapper, 0);
    bodyItem('Delete').click();
    await flushPromises();

    expect(deleteAgencyRequestApplicant).not.toHaveBeenCalled();
  });

  it('shows the error when the removal fails', async () => {
    const error = new Error('delete failed');
    vi.mocked(showAlertConfirm).mockResolvedValue(true);
    vi.mocked(deleteAgencyRequestApplicant).mockRejectedValue(error);
    const wrapper = await setup();
    await openRowActions(wrapper, 0);
    bodyItem('Delete').click();
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });
});
