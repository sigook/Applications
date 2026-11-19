import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import { flushPromises } from '@vue/test-utils';
import AgencyWorkers from '@/modules/agency/shared/AgencyWorkers.vue';
import {
  finishAgencyRequestWorker,
  finishAllAgencyRequestWorkers,
  getAgencyRequestsWorkers,
  getWorkersReportDocument,
  rejectAgencyRequestWorker,
  updateAgencyRequestWorkerStartDate,
} from '@/modules/agency/recruiting/requests/api';
import {
  createAgencyRequestWorkerNote,
  deleteAgencyRequestWorkerNote,
  getAgencyRequestWorkerNotes,
  updateAgencyRequestWorkerNote,
} from '@/modules/agency/shared/notes/api';
import { downloadAgencyReport } from '@/shared/api/reportApi';
import { downloadFile } from '@/shared/utils/downloadFile';
import { showAlertError } from '@/shared/utils/toast';
import { WorkerRequestFinishReason, WorkerRequestStatus } from '@/shared/constants/enums';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { AgencyRequestDetail, AgencyRequestWorker } from '@/modules/agency/recruiting/requests/types';
import type { FinishAssignmentModel } from '@/shared/request-detail/types';

const { push, stub } = vi.hoisted(() => ({
  push: vi.fn(),
  stub: (name: string, props: string[], emits: string[] = []) => ({ name, props, emits, template: `<div class="${name}" />` }),
}));

vi.mock('vue-router', async (importOriginal) => ({
  ...await importOriginal<typeof import('vue-router')>(),
  useRouter: () => ({ push }),
}));
vi.mock('@/modules/agency/recruiting/requests/api', () => ({
  finishAgencyRequestWorker: vi.fn(),
  finishAllAgencyRequestWorkers: vi.fn(),
  getAgencyRequestsWorkers: vi.fn(),
  rejectAgencyRequestWorker: vi.fn(),
  updateAgencyRequestWorkerStartDate: vi.fn(),
  getWorkersReportDocument: vi.fn(),
}));
vi.mock('@/modules/agency/shared/notes/api', () => ({
  getAgencyRequestWorkerNotes: vi.fn(),
  createAgencyRequestWorkerNote: vi.fn(),
  updateAgencyRequestWorkerNote: vi.fn(),
  deleteAgencyRequestWorkerNote: vi.fn(),
}));
vi.mock('@/shared/api/reportApi', () => ({ downloadAgencyReport: vi.fn() }));
vi.mock('@/shared/utils/downloadFile', () => ({ downloadFile: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));
vi.mock('@/modules/agency/shared/AgencyWorkersList.vue', () => ({ default: stub('WorkersList', ['requestId'], ['workerBooked']) }));
vi.mock('@/modules/agency/shared/notes/NotesPopover.vue', () => ({
  default: stub('NotesPopover', ['canCreate', 'userId', 'requestId', 'notesCount', 'onGet', 'onCreate', 'onUpdate', 'onDelete'], ['update:count']),
}));
vi.mock('@/shared/ui/EditTextarea.vue', () => ({ default: stub('EditTextarea', ['title', 'subtitle', 'minLength'], ['updateContent']) }));
vi.mock('@/shared/request-detail/FinishAssignmentModal.vue', () => ({ default: stub('FinishAssignmentModal', ['subject', 'startWorking'], ['submit', 'close']) }));
vi.mock('@/modules/agency/recruiting/requests/components/AgencyPunchCardWorkerContainer.vue', () => ({
  default: stub('PunchCard', ['workerProfileId', 'worker', 'requestId', 'request']),
}));
vi.mock('@/modules/agency/recruiting/requests/components/DatepickerModal.vue', () => ({
  default: stub('DatepickerModal', ['startWorking'], ['onSelectCalendar', 'update:startWorking']),
}));

function worker(overrides: Partial<AgencyRequestWorker> = {}): AgencyRequestWorker {
  return {
    id: 'rw1',
    numberId: 501,
    requestId: 'r1',
    workerId: 'w1',
    workerProfileId: 'wp1',
    name: 'Jane Doe',
    workerRequestStatus: WorkerRequestStatus.Booked,
    approvedToWork: true,
    isSubcontractor: false,
    socialInsuranceExpire: false,
    notesCount: 2,
    startWorking: '2026-05-04T12:00:00',
    createdAt: '2026-05-01T12:00:00',
    totalHoursApproved: 30,
    totalHoursWorker: 32.5,
    ...overrides,
  };
}

const finished = worker({
  id: 'rw2',
  workerProfileId: 'wp2',
  name: 'Tom Lee',
  workerRequestStatus: WorkerRequestStatus.Finished,
  finishReason: WorkerRequestFinishReason.WorkerResigned,
  finishComments: 'Moved away',
  isSubcontractor: true,
  profileImage: 'https://img/tom.png',
});

const model: FinishAssignmentModel = { lastDay: '2026-06-01', reason: WorkerRequestFinishReason.WorkerResigned };

async function setup(workerSalary: number | null = null) {
  vi.mocked(getAgencyRequestsWorkers).mockResolvedValue({ items: [worker(), finished], totalItems: 2, pageIndex: 1, totalPages: 1 });
  const request = { id: 'r1', workerSalary } as unknown as AgencyRequestDetail;
  const wrapper = mountWithPlugins(AgencyWorkers, { props: { request }, global: { stubs: { DefaultImage: true } } });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function rows(wrapper: Wrapper) {
  return wrapper.findAll('tbody tr').filter((r) => !r.classes('detail'));
}

function child(wrapper: Wrapper, name: string) {
  return wrapper.findComponent({ name });
}

async function dropdownItem(wrapper: Wrapper, text: string) {
  await wrapper.findAll('.sigook-grid-toolbar button').find((b) => b.text() === 'Actions')?.trigger('click');
  await nextTick();
  const item = Array.from(document.body.querySelectorAll<HTMLElement>('.dropdown-item')).find((i) => i.textContent.trim() === text);
  if (!item) throw new Error(`${text} not found`);
  return item;
}

function dropdownTexts() {
  return Array.from(document.body.querySelectorAll<HTMLElement>('.dropdown-item')).map((i) => i.textContent.trim());
}

beforeEach(() => {
  vi.useFakeTimers({ now: new Date('2026-06-01T12:00:00'), toFake: ['Date'] });
});

afterEach(() => {
  vi.useRealTimers();
});

describe('AgencyWorkers', () => {
  it('loads the request workers sorted by status', async () => {
    const wrapper = await setup();
    expect(getAgencyRequestsWorkers).toHaveBeenCalledWith(expect.objectContaining({ requestId: 'r1', sortBy: 2, isDescending: false, pageIndex: 1 }));
    expect(rows(wrapper)).toHaveLength(2);
    const first = rows(wrapper)[0].text();
    expect(first).toContain('501');
    expect(first).toContain('Jane Doe');
    expect(first).toContain('Booked');
    const second = rows(wrapper)[1];
    expect(second.text()).toContain('Finished');
    expect(second.text()).toContain('Moved away');
    expect(second.text()).toContain('Worker resigned');
    expect(second.find('img').attributes('src')).toBe('https://img/tom.png');
    expect(second.find('.Blue').exists()).toBe(true);
  });

  it('shows hours and offers the punch card export for regular requests', async () => {
    const wrapper = await setup();
    expect(wrapper.find('thead').text()).toContain('Hours');
    await dropdownItem(wrapper, 'Export');
    expect(dropdownTexts()).toContain('Export Punch Card');
  });

  it('hides hours and the punch card export for direct hiring', async () => {
    const wrapper = await setup(25);
    expect(wrapper.find('thead').text()).not.toContain('Hours');
    await dropdownItem(wrapper, 'Export');
    expect(dropdownTexts()).toContain('End all assignments');
    expect(dropdownTexts()).not.toContain('Export Punch Card');
  });

  it('offers end and reject only for booked workers', async () => {
    const wrapper = await setup();
    expect(rows(wrapper)[0].findAll('.button.is-rounded')).toHaveLength(2);
    expect(rows(wrapper)[1].findAll('.button.is-rounded')).toHaveLength(0);
  });

  it('opens the worker profile from a cell', async () => {
    const wrapper = await setup();
    child(wrapper, 'SigookGrid').vm.$emit('cellclick', worker(), { field: 'name' });
    expect(push).toHaveBeenCalledWith('/recruiting/workers/wp1');
    child(wrapper, 'SigookGrid').vm.$emit('cellclick', worker(), { field: 'actions' });
    expect(push).toHaveBeenCalledOnce();
  });

  it('changes the start date', async () => {
    vi.mocked(updateAgencyRequestWorkerStartDate).mockResolvedValue();
    const wrapper = await setup();
    child(wrapper, 'SigookGrid').vm.$emit('cellclick', worker(), { field: 'startWorking' });
    await nextTick();
    const date = new Date('2026-05-10T04:00:00.000Z');
    child(wrapper, 'DatepickerModal').vm.$emit('onSelectCalendar', date);
    await flushPromises();

    expect(updateAgencyRequestWorkerStartDate).toHaveBeenCalledWith('r1', 'rw1', { startWorking: date.toISOString() });
    expect(getAgencyRequestsWorkers).toHaveBeenCalledTimes(2);
  });

  it('opens the start date modal from the start date button', async () => {
    const wrapper = await setup();
    await rows(wrapper)[0].find('.button.is-ghost').trigger('click');
    await nextTick();
    expect(child(wrapper, 'DatepickerModal').props('startWorking')).toBe('2026-05-04T12:00:00');
  });

  it('reports a failed start date change', async () => {
    vi.mocked(updateAgencyRequestWorkerStartDate).mockRejectedValue({ data: 'Invalid date' });
    const wrapper = await setup();
    child(wrapper, 'SigookGrid').vm.$emit('cellclick', worker(), { field: 'startWorking' });
    await nextTick();
    child(wrapper, 'DatepickerModal').vm.$emit('onSelectCalendar', new Date());
    await flushPromises();
    expect(showAlertError).toHaveBeenCalledWith('Invalid date');
  });

  it('rejects a worker with the reason', async () => {
    vi.mocked(rejectAgencyRequestWorker).mockResolvedValue();
    const wrapper = await setup();
    await rows(wrapper)[0].findAll('.button.is-rounded')[1].trigger('click');
    await nextTick();
    expect(child(wrapper, 'EditTextarea').props()).toMatchObject({ title: 'Reject Worker', minLength: 10 });
    child(wrapper, 'EditTextarea').vm.$emit('updateContent', 'No show on day one');
    await flushPromises();

    expect(rejectAgencyRequestWorker).toHaveBeenCalledWith('r1', 'wp1', { comments: 'No show on day one' });
    expect(wrapper.emitted('refreshRequest')).toHaveLength(1);
    expect(getAgencyRequestsWorkers).toHaveBeenCalledTimes(2);
  });

  it('reports a failed rejection', async () => {
    vi.mocked(rejectAgencyRequestWorker).mockRejectedValue({ data: 'Not allowed' });
    const wrapper = await setup();
    await rows(wrapper)[0].findAll('.button.is-rounded')[1].trigger('click');
    await nextTick();
    child(wrapper, 'EditTextarea').vm.$emit('updateContent', 'No show on day one');
    await flushPromises();
    expect(showAlertError).toHaveBeenCalledWith('Not allowed');
    expect(wrapper.emitted('refreshRequest')).toBeUndefined();
  });

  it('ends one assignment', async () => {
    vi.mocked(finishAgencyRequestWorker).mockResolvedValue();
    const wrapper = await setup();
    await rows(wrapper)[0].findAll('.button.is-rounded')[0].trigger('click');
    await nextTick();
    expect(child(wrapper, 'FinishAssignmentModal').props()).toEqual({ subject: 'Jane Doe', startWorking: '2026-05-04T12:00:00' });
    child(wrapper, 'FinishAssignmentModal').vm.$emit('submit', model);
    await flushPromises();

    expect(finishAgencyRequestWorker).toHaveBeenCalledWith('r1', 'wp1', model);
    expect(finishAllAgencyRequestWorkers).not.toHaveBeenCalled();
    expect(wrapper.emitted('refreshRequest')).toHaveLength(1);
  });

  it('ends every assignment', async () => {
    vi.mocked(finishAllAgencyRequestWorkers).mockResolvedValue();
    const wrapper = await setup();
    (await dropdownItem(wrapper, 'End all assignments')).click();
    await nextTick();
    expect(child(wrapper, 'FinishAssignmentModal').props()).toEqual({ subject: 'Every booked worker of this request', startWorking: null });
    child(wrapper, 'FinishAssignmentModal').vm.$emit('submit', model);
    await flushPromises();

    expect(finishAllAgencyRequestWorkers).toHaveBeenCalledWith('r1', model);
    expect(wrapper.emitted('refreshRequest')).toHaveLength(1);
  });

  it('reports a failed end of assignment', async () => {
    vi.mocked(finishAllAgencyRequestWorkers).mockRejectedValue({ data: 'Has open shifts' });
    const wrapper = await setup();
    (await dropdownItem(wrapper, 'End all assignments')).click();
    await nextTick();
    child(wrapper, 'FinishAssignmentModal').vm.$emit('submit', model);
    await flushPromises();
    expect(showAlertError).toHaveBeenCalledWith('Has open shifts');
  });

  it('exports the workers report', async () => {
    const blob = new Blob(['x']);
    vi.mocked(getWorkersReportDocument).mockResolvedValue(blob);
    const wrapper = await setup();
    (await dropdownItem(wrapper, 'Export')).click();
    await flushPromises();
    expect(getWorkersReportDocument).toHaveBeenCalledWith('r1');
    expect(downloadFile).toHaveBeenCalledWith(blob, 'WorkersReport_r1');
  });

  it('reports a failed workers export', async () => {
    vi.mocked(getWorkersReportDocument).mockRejectedValue('fail');
    const wrapper = await setup();
    (await dropdownItem(wrapper, 'Export')).click();
    await flushPromises();
    expect(showAlertError).toHaveBeenCalledWith('fail');
  });

  it('exports the punch card with the current filters', async () => {
    const blob = new Blob(['x']);
    vi.mocked(downloadAgencyReport).mockResolvedValue(blob);
    const wrapper = await setup();
    (await dropdownItem(wrapper, 'Export Punch Card')).click();
    await flushPromises();
    expect(downloadAgencyReport).toHaveBeenCalledWith('/api/agency/recruiting/requests/r1/TimeSheets/File', expect.objectContaining({ requestId: 'r1' }));
    expect(downloadFile).toHaveBeenCalledWith(blob, `Timesheet_${new Date().toLocaleDateString()}`);
  });

  it('reports a failed punch card export', async () => {
    vi.mocked(downloadAgencyReport).mockRejectedValue('fail');
    const wrapper = await setup();
    (await dropdownItem(wrapper, 'Export Punch Card')).click();
    await flushPromises();
    expect(showAlertError).toHaveBeenCalledWith('fail');
  });

  it('reloads after booking from manage workers', async () => {
    const wrapper = await setup();
    await wrapper.findAll('button').find((b) => b.text() === 'Manage Workers')?.trigger('click');
    await nextTick();
    expect(child(wrapper, 'WorkersList').props('requestId')).toBe('r1');
    child(wrapper, 'WorkersList').vm.$emit('workerBooked');
    await flushPromises();
    expect(getAgencyRequestsWorkers).toHaveBeenCalledTimes(2);
    expect(wrapper.emitted('refreshRequest')).toHaveLength(1);
  });

  it('filters by status and start date range', async () => {
    const wrapper = await setup();
    wrapper.findComponent({ name: 'BTaginput' }).vm.$emit('update:modelValue', [{ id: WorkerRequestStatus.Booked, value: 'Booked' }]);
    await flushPromises();
    expect(getAgencyRequestsWorkers).toHaveBeenLastCalledWith(expect.objectContaining({ statuses: [WorkerRequestStatus.Booked], pageIndex: 1 }));

    const from = new Date('2026-05-01T04:00:00.000Z');
    const to = new Date('2026-05-31T04:00:00.000Z');
    wrapper.findComponent({ name: 'BDatepicker' }).vm.$emit('update:modelValue', [from, to]);
    await flushPromises();
    expect(getAgencyRequestsWorkers).toHaveBeenLastCalledWith(expect.objectContaining({ startWorkingFrom: from.toISOString(), startWorkingTo: to.toISOString() }));

    wrapper.findComponent({ name: 'BDatepicker' }).vm.$emit('icon-right-click');
    await flushPromises();
    expect(getAgencyRequestsWorkers).toHaveBeenLastCalledWith(expect.objectContaining({ startWorkingFrom: null, startWorkingTo: null }));
  });

  it('routes notes to the request worker endpoints and updates the count', async () => {
    const wrapper = await setup();
    const popover = wrapper.findComponent({ name: 'NotesPopover' });
    expect(popover.props()).toMatchObject({ userId: 'rw1', requestId: 'r1', notesCount: 2, canCreate: false });
    const pagination = { page: 1, size: 20 };
    const props = popover.props() as {
      onGet: (p: object) => unknown;
      onCreate: (p: object) => unknown;
      onUpdate: (p: object) => unknown;
      onDelete: (p: object) => unknown;
    };
    void props.onGet({ requestId: 'r1', userId: 'rw1', pagination });
    void props.onCreate({ requestId: 'r1', userId: 'rw1', model: { note: 'a' } });
    void props.onUpdate({ requestId: 'r1', userId: 'rw1', id: 'n1', model: { note: 'b' } });
    void props.onDelete({ requestId: 'r1', userId: 'rw1', id: 'n1' });
    expect(getAgencyRequestWorkerNotes).toHaveBeenCalledWith('r1', 'rw1', pagination);
    expect(createAgencyRequestWorkerNote).toHaveBeenCalledWith('r1', 'rw1', { note: 'a' });
    expect(updateAgencyRequestWorkerNote).toHaveBeenCalledWith('r1', 'rw1', 'n1', { note: 'b' });
    expect(deleteAgencyRequestWorkerNote).toHaveBeenCalledWith('r1', 'rw1', 'n1');

    popover.vm.$emit('update:count', 3);
    await nextTick();
    expect(popover.props('notesCount')).toBe(3);
  });

  it('shows the punch card in the row detail for regular requests', async () => {
    const wrapper = await setup();
    await rows(wrapper)[0].find('td.chevron-cell a').trigger('click');
    await nextTick();
    expect(child(wrapper, 'PunchCard').props()).toMatchObject({ workerProfileId: 'wp1', requestId: 'r1' });
  });
});
