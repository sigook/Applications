import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { flushPromises, RouterLinkStub } from '@vue/test-utils';
import { createPinia, setActivePinia, type Pinia } from 'pinia';
import RecruiterWeeklyBoard from '@/modules/agency/recruiting/weekly-board/components/RecruiterWeeklyBoard.vue';
import { addRunner, getRecruiterWeeklyBoard } from '@/modules/agency/recruiting/weekly-board/api';
import { deleteAgencyRunner } from '@/modules/agency/recruiting/requests/runners/api';
import { RunnerStatus, RunnerType } from '@/modules/agency/recruiting/requests/runners/types';
import { RequestStatus } from '@/shared/constants/enums';
import { getDialog } from '@/shared/utils/buefyProgrammatic';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { RecruiterWeeklyBoard as Board, WeeklyBoardAssignment } from '@/modules/agency/recruiting/weekly-board/types';

vi.mock('@/modules/agency/recruiting/weekly-board/api', () => ({ addRunner: vi.fn(), getRecruiterWeeklyBoard: vi.fn() }));
vi.mock('@/modules/agency/recruiting/requests/runners/api', () => ({ deleteAgencyRunner: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));
vi.mock('@/shared/utils/buefyProgrammatic', () => {
  const dialog = { confirm: vi.fn() };
  return { getDialog: () => dialog };
});
vi.mock('@/modules/agency/recruiting/requests/runners/CreateRunner.vue', () => ({
  default: { name: 'CreateRunner', props: ['requestId'], emits: ['create', 'close'], template: '<div class="create-stub" />' },
}));
vi.mock('@/modules/agency/recruiting/requests/runners/RunnerActionsDropdown.vue', () => ({
  default: { name: 'RunnerActionsDropdown', props: ['status', 'canEdit'], emits: ['open', 'delete'], template: '<div class="actions-stub"><slot name="trigger" /></div>' },
}));
vi.mock('@/modules/agency/recruiting/requests/runners/RunnerActionModals.vue', () => ({
  default: {
    name: 'RunnerActionModals',
    props: ['target', 'statusOpen', 'interviewOpen', 'historyOpen'],
    emits: ['updated', 'update:statusOpen', 'update:interviewOpen', 'update:historyOpen'],
    template: '<div class="modals-stub" />',
  },
}));

interface ConfirmOptions {
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
    runnersSent: 1,
    runners: [{ runnerId: 'run-1', workerProfileId: 'wp-1', fullName: 'john smith', email: 'john@example.com', type: RunnerType.Active, status: RunnerStatus.SentToClient }],
    ...overrides,
  };
}

const past = assignment({});
const upcoming = assignment({ requestId: 'req-2', numberId: 502, workDate: '2026-03-05T00:00:00', status: RequestStatus.Open, isAsap: false, workerSalary: 55000, city: null, runners: [] });
const readOnly = assignment({ requestId: 'req-3', numberId: 503, workDate: '2026-03-06T00:00:00', status: RequestStatus.Cancelled, isAsap: false, usesRunners: false, runners: [] });

function board(assignments: WeeklyBoardAssignment[] = [past, upcoming, readOnly]): Board {
  return { recruiterName: 'Rita Gomez', weekStart: '2026-03-02', weekEnd: '2026-03-08', ordersCount: 3, workersSent: 1, assignments };
}

let pinia: Pinia;

async function setup(response: Board = board()) {
  vi.mocked(getRecruiterWeeklyBoard).mockResolvedValue(response);
  const wrapper = mountWithPlugins(RecruiterWeeklyBoard, { global: { plugins: [pinia], stubs: { RouterLink: RouterLinkStub } } });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function card(wrapper: Wrapper, numberId: number) {
  const found = wrapper.findAll('.order-card').find((c) => c.find('.card-num').text() === `# ${numberId}`);
  if (!found) throw new Error(`card ${numberId} not found`);
  return found;
}

function headings(wrapper: Wrapper) {
  return wrapper.findAll('.day-heading').map((h) => h.text());
}

function tab(wrapper: Wrapper, name: string) {
  const found = wrapper.findAll('.day-tab').find((t) => t.find('.tab-name').text() === name);
  if (!found) throw new Error(`${name} tab not found`);
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

describe('RecruiterWeeklyBoard', () => {
  it('loads the recruiter week and shows only the days with orders', async () => {
    const wrapper = await setup();

    expect(getRecruiterWeeklyBoard).toHaveBeenCalledWith({ from: '2026-03-02', to: '2026-03-08', agencyIds: undefined });
    expect(wrapper.findAll('.stat-value').map((s) => s.text())).toEqual(['3', '1']);
    expect(headings(wrapper)).toEqual(['MON MAR 2', 'THU MAR 5', 'FRI MAR 6']);
    expect(wrapper.findAll('.tab-dot')).toHaveLength(3);
    expect(tab(wrapper, 'ALL').classes()).toContain('is-active');
  });

  it('renders each order card', async () => {
    const wrapper = await setup();

    const first = card(wrapper, 501);
    expect(first.classes()).toContain('is-filled');
    expect(first.find('.request-flag--asap').exists()).toBe(true);
    expect(first.text()).toContain('Filled');
    expect(first.text()).toContain('Toronto, ON');
    expect(first.text()).toContain('RUNNERS (1)');
    expect(first.find('.sent-worker-avatar').text()).toBe('JS');

    const second = card(wrapper, 502);
    expect(second.classes()).toContain('is-open');
    expect(second.find('.request-flag--dh').exists()).toBe(true);
    expect(second.find('.sent-workers').exists()).toBe(false);

    expect(card(wrapper, 503).classes()).not.toContain('is-open');
    expect(card(wrapper, 503).text()).toContain('Cancelled');
  });

  it('offers to add runners only on upcoming orders that use them', async () => {
    const wrapper = await setup();

    expect(card(wrapper, 501).find('.add-worker-btn').exists()).toBe(false);
    expect(card(wrapper, 502).find('.add-worker-btn').exists()).toBe(true);
    expect(card(wrapper, 503).find('.add-worker-btn').exists()).toBe(false);
  });

  it('filters by day', async () => {
    const wrapper = await setup();
    await tab(wrapper, 'THU').trigger('click');
    expect(headings(wrapper)).toEqual(['THU MAR 5']);

    expect(wrapper.find('.empty-board').exists()).toBe(false);

    await tab(wrapper, 'TUE').trigger('click');
    expect(headings(wrapper)).toEqual([]);
    expect(wrapper.find('.order-card').exists()).toBe(false);
    expect(wrapper.find('.empty-board').text()).toBe('No assigned orders for this day.');

    await tab(wrapper, 'ALL').trigger('click');
    expect(headings(wrapper)).toHaveLength(3);
  });

  it('says when the week has no orders', async () => {
    const wrapper = await setup(board([]));

    expect(wrapper.find('.empty-board').text()).toBe('No assigned orders for this week.');
  });

  it('shows the error when the board fails to load', async () => {
    const error = new Error('board failed');
    vi.mocked(getRecruiterWeeklyBoard).mockRejectedValue(error);
    mountWithPlugins(RecruiterWeeklyBoard, { global: { plugins: [pinia], stubs: { RouterLink: RouterLinkStub } } });
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('moves between weeks and resets the day filter', async () => {
    const wrapper = await setup();
    await tab(wrapper, 'THU').trigger('click');
    const [previous, next, refresh] = wrapper.findAll('.range-nav > button');

    await next.trigger('click');
    await flushPromises();
    expect(getRecruiterWeeklyBoard).toHaveBeenLastCalledWith({ from: '2026-03-09', to: '2026-03-15', agencyIds: undefined });
    expect(tab(wrapper, 'ALL').classes()).toContain('is-active');

    await previous.trigger('click');
    await flushPromises();
    expect(getRecruiterWeeklyBoard).toHaveBeenLastCalledWith({ from: '2026-03-02', to: '2026-03-08', agencyIds: undefined });

    await refresh.trigger('click');
    await flushPromises();
    expect(getRecruiterWeeklyBoard).toHaveBeenCalledTimes(4);
  });

  it('loads a custom range', async () => {
    const wrapper = await setup();
    wrapper.findComponent({ name: 'BDatepicker' }).vm.$emit('update:modelValue', [new Date('2026-03-03T00:00:00'), new Date('2026-03-04T00:00:00')]);
    await flushPromises();

    expect(getRecruiterWeeklyBoard).toHaveBeenLastCalledWith({ from: '2026-03-03', to: '2026-03-04', agencyIds: undefined });
    expect(wrapper.text()).toContain('Mar 3 – Mar 4, 2026');
  });

  it('sends a runner to an upcoming order', async () => {
    vi.mocked(addRunner).mockResolvedValue();
    const wrapper = await setup();
    await card(wrapper, 502).find('.add-worker-btn').trigger('click');
    await flushPromises();

    const create = wrapper.findComponent({ name: 'CreateRunner' });
    expect(create.props('requestId')).toBe('req-2');
    create.vm.$emit('create', { workerProfileId: 'wp-9', type: RunnerType.Passive });
    await flushPromises();

    expect(addRunner).toHaveBeenCalledWith({ requestId: 'req-2', workDate: '2026-03-05', workerProfileId: 'wp-9', type: RunnerType.Passive });
    expect(showAlertSuccess).toHaveBeenCalledWith('Runner sent');
    expect(getRecruiterWeeklyBoard).toHaveBeenCalledTimes(2);
  });

  it('shows the error when sending the runner fails', async () => {
    const error = new Error('send failed');
    vi.mocked(addRunner).mockRejectedValue(error);
    const wrapper = await setup();
    await card(wrapper, 502).find('.add-worker-btn').trigger('click');
    await flushPromises();
    wrapper.findComponent({ name: 'CreateRunner' }).vm.$emit('create', { workerProfileId: 'wp-9', type: RunnerType.Active });
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(getRecruiterWeeklyBoard).toHaveBeenCalledOnce();
  });

  it('opens the runner actions for a sent runner', async () => {
    const wrapper = await setup();
    const actions = card(wrapper, 501).findComponent({ name: 'RunnerActionsDropdown' });
    expect(actions.props()).toEqual({ status: RunnerStatus.SentToClient, canEdit: true });

    actions.vm.$emit('open', 'status');
    await flushPromises();
    expect(wrapper.findComponent({ name: 'RunnerActionModals' }).props()).toEqual({
      target: { requestId: 'req-1', runnerId: 'run-1', name: 'john smith', status: RunnerStatus.SentToClient },
      statusOpen: true,
      interviewOpen: false,
      historyOpen: false,
    });

    wrapper.findComponent({ name: 'RunnerActionModals' }).vm.$emit('updated');
    await flushPromises();
    expect(getRecruiterWeeklyBoard).toHaveBeenCalledTimes(2);
  });

  it('deletes a runner once confirmed and reloads', async () => {
    vi.mocked(deleteAgencyRunner).mockResolvedValue();
    const wrapper = await setup();
    card(wrapper, 501).findComponent({ name: 'RunnerActionsDropdown' }).vm.$emit('delete');
    await lastConfirm().onConfirm();
    await flushPromises();

    expect(deleteAgencyRunner).toHaveBeenCalledWith('req-1', 'run-1');
    expect(getRecruiterWeeklyBoard).toHaveBeenCalledTimes(2);
  });
});
