import { beforeEach, describe, expect, it, vi } from 'vitest';
import { computed, nextTick, ref } from 'vue';
import { flushPromises } from '@vue/test-utils';
import CompanyRequestWorkers from '@/modules/company/requests/components/CompanyRequestWorkers.vue';
import { finishCompanyRequestWorker, getCompanyRequestTimeSheetFile, getRequestWorkers, rejectCompanyRequestWorker } from '@/modules/company/requests/api';
import { DurationTerm, RequestStatus, WorkerRequestFinishReason, WorkerRequestStatus } from '@/shared/constants/enums';
import { downloadFile } from '@/shared/utils/downloadFile';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { CompanyRequestDetail, CompanyRequestWorker } from '@/modules/company/requests/types';

const touch = ref(false);

vi.mock('@/shared/composables/useBreakpoint', () => ({
  useBreakpoint: () => ({ isTouch: computed(() => touch.value), isMobile: computed(() => touch.value) }),
}));
vi.mock('@/modules/company/requests/api', () => ({
  getRequestWorkers: vi.fn(),
  finishCompanyRequestWorker: vi.fn(),
  rejectCompanyRequestWorker: vi.fn(),
  getCompanyRequestTimeSheetFile: vi.fn(),
}));
vi.mock('@/shared/api/reportApi', () => ({ downloadAgencyReport: vi.fn() }));
vi.mock('@/shared/utils/downloadFile', () => ({ downloadFile: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));
vi.mock('@/modules/company/requests/components/CompanyPunchCardWorkerContainer.vue', () => ({
  default: { name: 'TablePunchCard', props: { workerProfileId: String, requestId: String, request: Object, worker: Object }, template: '<div class="punch-card" />' },
}));
vi.mock('@/shared/punch-card/DataEntryTerms.vue', () => ({ default: { name: 'DataEntryTerms', template: '<div class="data-entry-terms" />' } }));
vi.mock('@/shared/ui/EditTextarea.vue', () => ({
  default: { name: 'EditTextarea', props: { title: String, minLength: Number }, emits: ['updateContent'], template: '<div class="edit-textarea" />' },
}));
vi.mock('@/shared/request-detail/FinishAssignmentModal.vue', () => ({
  default: { name: 'FinishAssignmentModal', props: { subject: String, startWorking: String }, emits: ['submit', 'close'], template: '<div class="finish-modal" />' },
}));
vi.mock('@/shared/ui/MobileFiltersPanel.vue', () => ({
  default: {
    name: 'MobileFiltersPanel',
    props: { modelValue: Boolean, activeCount: Number },
    emits: ['update:modelValue', 'apply', 'clear'],
    template: '<div class="mobile-filters"><slot /></div>',
  },
}));

const request: CompanyRequestDetail = {
  id: 'request-1', numberId: 10, jobTitle: 'Forklift operator', workersQuantity: 2, workersQuantityWorking: 1,
  durationBreak: '00:30:00', breakIsPaid: false, holidayIsPaid: false, isAsap: false, jobIsOnBranchOffice: false,
  status: RequestStatus.Open, durationTerm: DurationTerm.LongTerm, createdAt: '2026-11-01T00:00:00', startAt: '2026-11-02T00:00:00',
};

const booked: CompanyRequestWorker = {
  numberId: 101, requestId: 'request-1', id: 'wr-1', workerId: 'worker-1', workerProfileId: 'wp-1', name: 'Ann Lee',
  workerRequestStatus: WorkerRequestStatus.Booked, profileImage: 'https://files/ann.png', isSubcontractor: false,
  totalHoursApproved: 32, totalHoursWorker: 40, startWorking: '2026-11-02T00:00:00', rejectedAt: null,
};
const finished: CompanyRequestWorker = {
  ...booked, numberId: 102, id: 'wr-2', workerProfileId: 'wp-2', name: 'Bob Ray', profileImage: '', isSubcontractor: true,
  workerRequestStatus: WorkerRequestStatus.Finished,
};

async function setup(current: CompanyRequestDetail = request) {
  vi.mocked(getRequestWorkers).mockResolvedValue({ items: [booked, finished], totalItems: 2, pageIndex: 1, totalPages: 1 });
  const wrapper = mountWithPlugins(CompanyRequestWorkers, {
    props: { request: current },
    global: { components: { DefaultImage: { props: { name: String }, template: '<span class="default-image">{{ name }}</span>' } } },
  });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function lastFilter() {
  return vi.mocked(getRequestWorkers).mock.lastCall?.[0];
}

function bodyRows(wrapper: Wrapper) {
  return wrapper.findAll('tbody tr').filter((tr) => !tr.classes('detail'));
}

beforeEach(() => {
  touch.value = false;
});

describe('CompanyRequestWorkers', () => {
  it('loads the request workers with their status labels', async () => {
    const wrapper = await setup();

    expect(getRequestWorkers).toHaveBeenCalledExactlyOnceWith({ sortBy: 2, requestId: 'request-1', pageIndex: 1, pageSize: 30 });
    expect(wrapper.find('.data-entry-terms').exists()).toBe(true);
    const rows = bodyRows(wrapper);
    expect(rows).toHaveLength(2);
    expect(rows[0].text()).toContain('Ann Lee');
    expect(rows[0].text()).toContain('Booked');
    expect(rows[1].text()).toContain('Finished');
    expect(rows[1].find('.Blue').text()).toBe('102');
    expect(rows[0].find('img').attributes('src')).toBe('https://files/ann.png');
    expect(rows[1].find('.default-image').exists()).toBe(true);
    expect(wrapper.findAll('thead th').map((th) => th.text())).toEqual(expect.arrayContaining(['Hours', 'Status']));
  });

  it('hides hours, punch cards and export for direct hiring', async () => {
    const wrapper = await setup({ ...request, workerSalary: 60000 });

    expect(wrapper.find('.data-entry-terms').exists()).toBe(false);
    expect(wrapper.findAll('thead th').map((th) => th.text())).not.toContain('Hours');
    expect(wrapper.find('.sigook-grid-actions .dropdown').exists()).toBe(false);
  });

  it('offers end and reject only to booked workers', async () => {
    const wrapper = await setup();
    const rows = bodyRows(wrapper);

    expect(rows[0].findAll('button.is-rounded')).toHaveLength(2);
    expect(rows[1].findAll('button.is-rounded')).toHaveLength(0);
  });

  it('rejects a booked worker with the comment and reloads', async () => {
    vi.mocked(rejectCompanyRequestWorker).mockResolvedValue();
    const wrapper = await setup();
    await bodyRows(wrapper)[0].find('button.is-danger').trigger('click');
    const textarea = wrapper.findComponent({ name: 'EditTextarea' });
    expect(textarea.props()).toEqual({ title: 'Reject Worker', minLength: 10 });

    textarea.vm.$emit('updateContent', 'Did not show up twice');
    await flushPromises();

    expect(rejectCompanyRequestWorker).toHaveBeenCalledExactlyOnceWith('request-1', 'wp-1', { comments: 'Did not show up twice' });
    expect(getRequestWorkers).toHaveBeenCalledTimes(2);
  });

  it('shows the error when the rejection fails', async () => {
    vi.mocked(rejectCompanyRequestWorker).mockRejectedValue({ data: 'reject failed' });
    const wrapper = await setup();
    await bodyRows(wrapper)[0].find('button.is-danger').trigger('click');
    wrapper.findComponent({ name: 'EditTextarea' }).vm.$emit('updateContent', 'Did not show up twice');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledExactlyOnceWith('reject failed');
    expect(getRequestWorkers).toHaveBeenCalledTimes(1);
  });

  it('ends the assignment of a booked worker and reloads', async () => {
    vi.mocked(finishCompanyRequestWorker).mockResolvedValue();
    const wrapper = await setup();
    await bodyRows(wrapper)[0].find('button.is-primary').trigger('click');
    const modal = wrapper.findComponent({ name: 'FinishAssignmentModal' });
    expect(modal.props()).toEqual({ subject: 'Ann Lee', startWorking: '2026-11-02T00:00:00' });

    const model = { lastDay: '2026-11-18', reason: WorkerRequestFinishReason.ContractCompleted };
    modal.vm.$emit('submit', model);
    await flushPromises();

    expect(finishCompanyRequestWorker).toHaveBeenCalledExactlyOnceWith('request-1', 'wp-1', model);
    expect(getRequestWorkers).toHaveBeenCalledTimes(2);
  });

  it('shows the error when ending the assignment fails', async () => {
    vi.mocked(finishCompanyRequestWorker).mockRejectedValue({ data: 'finish failed' });
    const wrapper = await setup();
    await bodyRows(wrapper)[0].find('button.is-primary').trigger('click');
    wrapper.findComponent({ name: 'FinishAssignmentModal' }).vm.$emit('submit', { lastDay: '2026-11-18', reason: WorkerRequestFinishReason.Other });
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledExactlyOnceWith('finish failed');
  });

  it('exports the punch card', async () => {
    const blob = new Blob(['xlsx']);
    vi.mocked(getCompanyRequestTimeSheetFile).mockResolvedValue(blob);
    await setup();
    const item = Array.from(document.body.querySelectorAll<HTMLElement>('.dropdown-item')).find((i) => i.textContent.includes('Export Punch Card'));
    if (!item) throw new Error('export item not found');
    item.click();
    await flushPromises();

    expect(getCompanyRequestTimeSheetFile).toHaveBeenCalledExactlyOnceWith('request-1');
    expect(downloadFile).toHaveBeenCalledExactlyOnceWith(blob, 'TimeSheet_request-1');
  });

  it('shows the error when the export fails', async () => {
    const error = new Error('export failed');
    vi.mocked(getCompanyRequestTimeSheetFile).mockRejectedValue(error);
    await setup();
    const item = Array.from(document.body.querySelectorAll<HTMLElement>('.dropdown-item')).find((i) => i.textContent.includes('Export Punch Card'));
    item?.click();
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledExactlyOnceWith(error);
    expect(downloadFile).not.toHaveBeenCalled();
  });

  it('filters by status and start date range', async () => {
    const wrapper = await setup();
    const [taginput] = wrapper.findAllComponents({ name: 'BTaginput' });
    taginput.vm.$emit('update:modelValue', [{ id: WorkerRequestStatus.Finished, value: 'Finished' }]);
    await flushPromises();
    expect(lastFilter()).toMatchObject({ statuses: [WorkerRequestStatus.Finished], pageIndex: 1 });

    const from = new Date(2026, 10, 1);
    const to = new Date(2026, 10, 15);
    const [datepicker] = wrapper.findAllComponents({ name: 'BDatepicker' });
    datepicker.vm.$emit('update:modelValue', [from, to]);
    await flushPromises();
    expect(lastFilter()).toMatchObject({ startWorkingFrom: from, startWorkingTo: to });

    datepicker.vm.$emit('icon-right-click');
    await flushPromises();
    expect(lastFilter()).toMatchObject({ startWorkingFrom: undefined, startWorkingTo: undefined });
  });

  it('shows cards with an expandable punch card on touch screens', async () => {
    touch.value = true;
    const wrapper = await setup();

    expect(wrapper.findAll('.rcard__title').map((t) => t.text())).toEqual(['Ann Lee', 'Bob Ray']);
    expect(wrapper.find('.rcard .rcard__row').text()).toContain('02-Nov-2026');
    expect(wrapper.find('.punch-card').exists()).toBe(false);

    await wrapper.findAll('.rcard__head')[1].trigger('click');
    const punchCard = wrapper.findComponent({ name: 'TablePunchCard' });
    expect(punchCard.props()).toMatchObject({ workerProfileId: 'wp-2', requestId: 'request-1' });

    await wrapper.findAll('.rcard__head')[1].trigger('click');
    expect(wrapper.find('.punch-card').exists()).toBe(false);
  });

  it('does not expand cards for direct hiring', async () => {
    touch.value = true;
    const wrapper = await setup({ ...request, workerSalary: 60000 });
    await wrapper.find('.rcard__head').trigger('click');

    expect(wrapper.find('.punch-card').exists()).toBe(false);
  });

  it('acts on booked workers from their card', async () => {
    touch.value = true;
    const wrapper = await setup();
    await wrapper.find('.rcard__actions button.is-danger').trigger('click');
    expect(wrapper.findComponent({ name: 'EditTextarea' }).exists()).toBe(true);

    await wrapper.find('.rcard__actions button.is-primary').trigger('click');
    expect(wrapper.findComponent({ name: 'FinishAssignmentModal' }).props('subject')).toBe('Ann Lee');
  });

  it('searches by name and counts and clears the mobile filters', async () => {
    touch.value = true;
    const wrapper = await setup();
    await wrapper.find('.sigook-grid-actions input').setValue('ann');
    await wrapper.find('.sigook-grid-actions input').trigger('keyup.enter');
    await flushPromises();
    expect(lastFilter()).toMatchObject({ name: 'ann', pageIndex: 1 });

    const panel = wrapper.findComponent({ name: 'MobileFiltersPanel' });
    const [taginput] = panel.findAllComponents({ name: 'BTaginput' });
    taginput.vm.$emit('update:modelValue', [{ id: WorkerRequestStatus.Booked, value: 'Booked' }]);
    await flushPromises();
    expect(panel.props('activeCount')).toBe(2);

    panel.vm.$emit('clear');
    await flushPromises();
    expect(lastFilter()).toMatchObject({ name: undefined, numberId: undefined, statuses: [] });
    expect(panel.props('activeCount')).toBe(0);

    panel.vm.$emit('apply');
    await nextTick();
    expect(getRequestWorkers).toHaveBeenCalledTimes(5);
  });
});
