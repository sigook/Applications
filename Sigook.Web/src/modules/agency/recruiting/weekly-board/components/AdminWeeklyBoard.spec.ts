import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { flushPromises, RouterLinkStub } from '@vue/test-utils';
import { createPinia, setActivePinia, type Pinia } from 'pinia';
import AdminWeeklyBoard from '@/modules/agency/recruiting/weekly-board/components/AdminWeeklyBoard.vue';
import { assignRecruiters, getRequestRunners, getWeeklyBoard, moveAssignment, unassignRecruiter } from '@/modules/agency/recruiting/weekly-board/api';
import { RunnerStatus, RunnerType } from '@/modules/agency/recruiting/requests/runners/types';
import { RequestStatus } from '@/shared/constants/enums';
import { getDialog } from '@/shared/utils/buefyProgrammatic';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { WeeklyBoard, WeeklyBoardAssignment, WeeklyBoardRunner } from '@/modules/agency/recruiting/weekly-board/types';

vi.mock('@/modules/agency/recruiting/weekly-board/api', () => ({
  assignRecruiters: vi.fn(),
  getRequestRunners: vi.fn(),
  getWeeklyBoard: vi.fn(),
  moveAssignment: vi.fn(),
  unassignRecruiter: vi.fn(),
}));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));
vi.mock('@/shared/utils/buefyProgrammatic', () => {
  const dialog = { confirm: vi.fn() };
  return { getDialog: () => dialog };
});
vi.mock('@/modules/agency/recruiting/weekly-board/components/AssignRecruiterModal.vue', () => ({
  default: { name: 'AssignRecruiterModal', props: ['days', 'preset', 'saving'], emits: ['assign', 'close'], template: '<div class="assign-stub" />' },
}));
vi.mock('@/modules/agency/recruiting/requests/runners/RunnerHistoryModal.vue', () => ({
  default: { name: 'RunnerHistoryModal', props: ['requestId', 'runnerId'], emits: ['close'], template: '<div class="history-stub" />' },
}));

interface ConfirmOptions {
  title: string;
  message: string;
  onConfirm: () => Promise<void>;
}

function assignment(overrides: Partial<WeeklyBoardAssignment>): WeeklyBoardAssignment {
  return {
    agencyId: 'agency-1',
    recruiterId: 'r1',
    recruiterName: 'Rita Gomez',
    requestId: 'req-1',
    numberId: 501,
    companyName: 'Acme Foods',
    jobTitle: 'Packer',
    city: 'Toronto',
    provinceCode: 'ON',
    workDate: '2026-03-02T00:00:00',
    status: RequestStatus.Filled,
    isAsap: true,
    workerSalary: null,
    usesRunners: true,
    runnersSent: 2,
    runners: [],
    ...overrides,
  };
}

const monday = assignment({});
const wednesday = assignment({ requestId: 'req-2', numberId: 502, workDate: '2026-03-04T00:00:00', status: RequestStatus.Open, isAsap: false, workerSalary: 60000, city: null });

function board(): WeeklyBoard {
  return {
    weekStart: '2026-03-02',
    weekEnd: '2026-03-08',
    totalAssignments: 2,
    totalWorkersSent: 3,
    recruiters: [
      { recruiterId: 'r1', recruiterName: 'Rita Gomez', ordersCount: 2, workersSent: 3, assignments: [monday, wednesday] },
      { recruiterId: 'r2', recruiterName: 'Sam Lee', ordersCount: 0, workersSent: 0, assignments: [] },
    ],
  };
}

const runnerSent: WeeklyBoardRunner = { runnerId: 'run-1', workerProfileId: 'wp-1', fullName: 'John Smith', email: 'john@example.com', type: RunnerType.Active, status: RunnerStatus.Hired };

let pinia: Pinia;

async function setup(response: WeeklyBoard = board()) {
  vi.mocked(getWeeklyBoard).mockResolvedValue(response);
  const wrapper = mountWithPlugins(AdminWeeklyBoard, { global: { plugins: [pinia], stubs: { RouterLink: RouterLinkStub } } });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function cell(wrapper: Wrapper, row: number, day: number) {
  return wrapper.findAll('tbody tr')[row].findAll('td')[day + 1];
}

function card(wrapper: Wrapper, numberId: number) {
  const found = wrapper.findAll('.order-card').find((c) => c.find('.card-num').text() === `#${numberId}`);
  if (!found) throw new Error(`card ${numberId} not found`);
  return found;
}

function button(wrapper: Wrapper, text: string) {
  const found = wrapper.findAll('button').find((b) => b.text() === text);
  if (!found) throw new Error(`${text} not found`);
  return found;
}

function lastConfirm(): ConfirmOptions {
  const call = vi.mocked(getDialog().confirm).mock.calls.at(-1);
  if (!call) throw new Error('confirm not opened');
  return call[0] as ConfirmOptions;
}

beforeEach(() => {
  vi.useFakeTimers({ now: new Date('2026-03-04T10:00:00'), toFake: ['Date'] });
  pinia = createPinia();
  setActivePinia(pinia);
});

afterEach(() => {
  vi.useRealTimers();
});

describe('AdminWeeklyBoard', () => {
  it('loads the current week from monday to sunday', async () => {
    const wrapper = await setup();

    expect(getWeeklyBoard).toHaveBeenCalledWith({ from: '2026-03-02', to: '2026-03-08', agencyIds: undefined });
    expect(wrapper.findAll('thead th').map((th) => th.text())).toEqual([
      'Recruiter', 'MONMar 2', 'TUEMar 3', 'WEDMar 4', 'THUMar 5', 'FRIMar 6', 'SATMar 7', 'SUNMar 8',
    ]);
    expect(wrapper.find('thead th.is-today').text()).toBe('WEDMar 4');
    expect(wrapper.text()).toContain('Mar 2 – Mar 8, 2026');
    expect(wrapper.findAll('.stat-value').map((s) => s.text())).toEqual(['2', '3']);
  });

  it('places each order card on its recruiter and day', async () => {
    const wrapper = await setup();

    expect(wrapper.findAll('.recruiter-name').map((r) => r.text())).toEqual(['Rita Gomez', 'Sam Lee']);
    expect(wrapper.find('.recruiter-meta').text()).toBe('2 orders · 3 sent');
    expect(cell(wrapper, 0, 0).find('.order-card').exists()).toBe(true);
    expect(cell(wrapper, 0, 2).find('.order-card').exists()).toBe(true);
    expect(cell(wrapper, 0, 1).find('.order-card').exists()).toBe(false);

    const first = card(wrapper, 501);
    expect(first.classes()).toContain('is-filled');
    expect(first.find('.request-flag--asap').exists()).toBe(true);
    expect(first.find('.request-flag--dh').exists()).toBe(false);
    expect(first.find('.card-location').text()).toBe('Toronto, ON');
    expect(first.find('.card-sent').text()).toBe('2 sent');
    expect(first.findComponent(RouterLinkStub).props('to')).toEqual({ name: 'agency-request', params: { id: 'req-1' } });

    const second = card(wrapper, 502);
    expect(second.classes()).toContain('is-open');
    expect(second.find('.request-flag--dh').exists()).toBe(true);
    expect(second.find('.card-location').exists()).toBe(false);
  });

  it('shows the empty week', async () => {
    const wrapper = await setup({ ...board(), totalAssignments: 0, totalWorkersSent: 0, recruiters: [] });

    expect(wrapper.find('.empty-row').text()).toBe('No assignments this week');
    expect(wrapper.find('.empty-row').attributes('colspan')).toBe('8');
  });

  it('shows the error when the board fails to load', async () => {
    const error = new Error('board failed');
    vi.mocked(getWeeklyBoard).mockRejectedValue(error);
    const wrapper = mountWithPlugins(AdminWeeklyBoard, { global: { plugins: [pinia], stubs: { RouterLink: RouterLinkStub } } });
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(wrapper.find('.empty-row').exists()).toBe(true);
  });

  it('moves to the previous and next week and refreshes', async () => {
    const wrapper = await setup();
    const [previous, next, refresh] = wrapper.findAll('.range-nav > button');

    await previous.trigger('click');
    await flushPromises();
    expect(getWeeklyBoard).toHaveBeenLastCalledWith({ from: '2026-02-23', to: '2026-03-01', agencyIds: undefined });

    await next.trigger('click');
    await next.trigger('click');
    await flushPromises();
    expect(getWeeklyBoard).toHaveBeenLastCalledWith({ from: '2026-03-09', to: '2026-03-15', agencyIds: undefined });

    await refresh.trigger('click');
    await flushPromises();
    expect(getWeeklyBoard).toHaveBeenCalledTimes(5);
  });

  it('loads a custom range picked in the calendar', async () => {
    const wrapper = await setup();
    wrapper.findComponent({ name: 'BDatepicker' }).vm.$emit('update:modelValue', [new Date('2026-03-10T00:00:00'), new Date('2026-03-12T00:00:00')]);
    await flushPromises();

    expect(getWeeklyBoard).toHaveBeenLastCalledWith({ from: '2026-03-10', to: '2026-03-12', agencyIds: undefined });
    expect(wrapper.findAll('thead th')).toHaveLength(4);
  });

  it('opens the assign modal empty or preset from a cell', async () => {
    const wrapper = await setup();
    await button(wrapper, 'Assign recruiter').trigger('click');
    await flushPromises();
    const modal = wrapper.findComponent({ name: 'AssignRecruiterModal' });
    expect(modal.props('preset')).toBeUndefined();
    expect(modal.props('days')).toHaveLength(7);

    await cell(wrapper, 1, 3).find('.add-card').trigger('click');
    await flushPromises();
    expect(wrapper.findComponent({ name: 'AssignRecruiterModal' }).props('preset')).toEqual({ recruiterId: 'r2', workDate: '2026-03-05' });
  });

  it('assigns recruiters and reloads the board', async () => {
    vi.mocked(assignRecruiters).mockResolvedValue();
    const wrapper = await setup();
    await button(wrapper, 'Assign recruiter').trigger('click');
    await flushPromises();
    const payload = { requestId: 'req-3', workDates: ['2026-03-05'], recruiterIds: ['r2'] };
    wrapper.findComponent({ name: 'AssignRecruiterModal' }).vm.$emit('assign', payload);
    await flushPromises();

    expect(assignRecruiters).toHaveBeenCalledWith(payload);
    expect(showAlertSuccess).toHaveBeenCalledWith('Recruiter assigned');
    expect(getWeeklyBoard).toHaveBeenCalledTimes(2);
  });

  it('shows the error when the assignment fails', async () => {
    const error = new Error('assign failed');
    vi.mocked(assignRecruiters).mockRejectedValue(error);
    const wrapper = await setup();
    await button(wrapper, 'Assign recruiter').trigger('click');
    await flushPromises();
    wrapper.findComponent({ name: 'AssignRecruiterModal' }).vm.$emit('assign', { requestId: 'req-3', workDates: [], recruiterIds: [] });
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(getWeeklyBoard).toHaveBeenCalledOnce();
  });

  it('moves a card to another recruiter and day', async () => {
    vi.mocked(moveAssignment).mockResolvedValue();
    const wrapper = await setup();
    await card(wrapper, 501).trigger('dragstart');
    expect(card(wrapper, 501).classes()).toContain('is-dragging');
    await cell(wrapper, 1, 1).trigger('dragover');
    expect(cell(wrapper, 1, 1).classes()).toContain('is-drop-target');
    await cell(wrapper, 1, 1).trigger('drop');
    await flushPromises();

    expect(moveAssignment).toHaveBeenCalledWith({ requestId: 'req-1', fromRecruiterId: 'r1', fromWorkDate: '2026-03-02', toRecruiterId: 'r2', toWorkDate: '2026-03-03' });
    expect(showAlertSuccess).toHaveBeenCalledWith('Assignment moved');
    expect(getWeeklyBoard).toHaveBeenCalledTimes(2);
  });

  it('does nothing when a card is dropped where it was', async () => {
    const wrapper = await setup();
    await card(wrapper, 501).trigger('dragstart');
    await cell(wrapper, 0, 0).trigger('drop');
    await flushPromises();

    expect(moveAssignment).not.toHaveBeenCalled();
  });

  it('ignores drags that did not start on a card and clears a cancelled drag', async () => {
    const wrapper = await setup();
    await cell(wrapper, 1, 1).trigger('dragover');
    expect(cell(wrapper, 1, 1).classes()).not.toContain('is-drop-target');
    await cell(wrapper, 1, 1).trigger('drop');

    await card(wrapper, 501).trigger('dragstart');
    await cell(wrapper, 1, 1).trigger('dragover');
    await cell(wrapper, 1, 1).trigger('dragleave');
    expect(cell(wrapper, 1, 1).classes()).not.toContain('is-drop-target');
    await card(wrapper, 501).trigger('dragend');
    expect(card(wrapper, 501).classes()).not.toContain('is-dragging');
    await flushPromises();

    expect(moveAssignment).not.toHaveBeenCalled();
  });

  it('shows the error when the move fails', async () => {
    const error = new Error('move failed');
    vi.mocked(moveAssignment).mockRejectedValue(error);
    const wrapper = await setup();
    await card(wrapper, 502).trigger('dragstart');
    await cell(wrapper, 1, 2).trigger('drop');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('removes an assignment from its card once confirmed', async () => {
    vi.mocked(unassignRecruiter).mockResolvedValue();
    const wrapper = await setup();
    await card(wrapper, 501).find('.card-remove').trigger('click');

    const options = lastConfirm();
    expect(options.title).toBe('Remove assignment');
    expect(options.message).toBe('Remove <strong>Rita Gomez</strong> from order #501?');
    expect(getRequestRunners).not.toHaveBeenCalled();
    await options.onConfirm();
    await flushPromises();

    expect(unassignRecruiter).toHaveBeenCalledWith({ requestId: 'req-1', recruiterId: 'r1', workDate: '2026-03-02' });
    expect(showAlertSuccess).toHaveBeenCalledWith('Assignment removed');
    expect(getWeeklyBoard).toHaveBeenCalledTimes(2);
  });

  it('shows the error when the removal fails', async () => {
    const error = new Error('remove failed');
    vi.mocked(unassignRecruiter).mockRejectedValue(error);
    const wrapper = await setup();
    await card(wrapper, 501).find('.card-remove').trigger('click');
    await lastConfirm().onConfirm();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('opens the order detail with its runners', async () => {
    vi.mocked(getRequestRunners).mockResolvedValue([runnerSent]);
    const wrapper = await setup();
    await card(wrapper, 501).trigger('click');
    await flushPromises();

    expect(getRequestRunners).toHaveBeenCalledWith('req-1');
    const detail = wrapper.find('.detail-card');
    expect(detail.find('.modal-card-title').text()).toBe('Request #501');
    expect(detail.text()).toContain('Acme Foods · Packer');
    expect(detail.text()).toContain('Rita Gomez');
    expect(detail.text()).toContain('Mon Mar 2');
    expect(detail.text()).toContain('Toronto, ON');
    expect(detail.text()).toContain('Filled');
    expect(detail.text()).toContain('Runners (1)');
    expect(detail.text()).toContain('John Smith');
  });

  it('shows an open order without location or runners', async () => {
    vi.mocked(getRequestRunners).mockResolvedValue([]);
    const wrapper = await setup();
    await card(wrapper, 502).trigger('click');
    await flushPromises();

    const detail = wrapper.find('.detail-card');
    expect(detail.text()).toContain('Open');
    expect(detail.find('.detail-workers').exists()).toBe(false);
  });

  it('shows the error when the runners fail to load', async () => {
    const error = new Error('runners failed');
    vi.mocked(getRequestRunners).mockRejectedValue(error);
    const wrapper = await setup();
    await card(wrapper, 501).trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('opens the history of a runner from the detail', async () => {
    vi.mocked(getRequestRunners).mockResolvedValue([runnerSent]);
    const wrapper = await setup();
    await card(wrapper, 501).trigger('click');
    await flushPromises();

    const item = Array.from(document.body.querySelectorAll<HTMLElement>('.dropdown-item')).find((i) => i.textContent.trim() === 'View History');
    item?.click();
    await flushPromises();

    expect(wrapper.findComponent({ name: 'RunnerHistoryModal' }).props()).toEqual({ requestId: 'req-1', runnerId: 'run-1' });
  });

  it('removes the assignment from the detail once confirmed', async () => {
    vi.mocked(getRequestRunners).mockResolvedValue([]);
    vi.mocked(unassignRecruiter).mockResolvedValue();
    const wrapper = await setup();
    await card(wrapper, 502).trigger('click');
    await flushPromises();
    await button(wrapper, 'Remove assignment').trigger('click');
    await flushPromises();

    expect(unassignRecruiter).not.toHaveBeenCalled();
    expect(lastConfirm().title).toBe('Remove assignment');
    await lastConfirm().onConfirm();
    await flushPromises();

    expect(unassignRecruiter).toHaveBeenCalledWith({ requestId: 'req-2', recruiterId: 'r1', workDate: '2026-03-04' });
  });
});
