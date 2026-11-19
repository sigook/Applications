import { describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import RunnerDetailPanel from '@/modules/agency/recruiting/runners/components/RunnerDetailPanel.vue';
import { getAgencyRunner, rescheduleRunnerInterview } from '@/modules/agency/recruiting/requests/runners/api';
import { InterviewStatus, InterviewType, RunnerStatus, RunnerType } from '@/modules/agency/recruiting/requests/runners/types';
import { getDialog } from '@/shared/utils/buefyProgrammatic';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { RunnerDetail } from '@/modules/agency/recruiting/requests/runners/types';

vi.mock('@/modules/agency/recruiting/requests/runners/api', () => ({ getAgencyRunner: vi.fn(), rescheduleRunnerInterview: vi.fn() }));
vi.mock('@/shared/api/reportApi', () => ({ downloadAgencyReport: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));
vi.mock('@/shared/utils/buefyProgrammatic', () => {
  const dialog = { prompt: vi.fn() };
  return { getDialog: () => dialog };
});
vi.mock('@/modules/agency/recruiting/requests/runners/RunnerInterviewModal.vue', () => ({
  default: { name: 'RunnerInterviewModal', props: ['requestId', 'runnerId'], emits: ['updated', 'close'], template: '<div class="interview-stub" />' },
}));

interface PromptOptions {
  title: string;
  inputAttrs: Record<string, unknown>;
  confirmText: string;
  onConfirm: (value: string, dialog: { close: () => void }) => Promise<void>;
}

function detail(overrides: Partial<RunnerDetail> = {}): RunnerDetail {
  return {
    id: 'run-1',
    numberId: 9,
    requestId: 'req-1',
    workerProfileId: 'wp-1',
    name: 'John Smith',
    type: RunnerType.Active,
    status: RunnerStatus.InterviewScheduled,
    createdAt: '2026-03-01T12:00:00',
    statusHistory: [
      { id: 'h1', newStatus: RunnerStatus.SentToClient, changedByEmail: 'jane@sigook.com', changedAt: '2026-03-01T12:00:00', comments: 'Sent' },
      { id: 'h2', newStatus: RunnerStatus.InterviewScheduled, changedByEmail: 'jane@sigook.com', changedAt: '2026-03-02T12:00:00' },
    ],
    interviews: [
      { id: 'i1', scheduledDate: '2026-03-05T14:00:00', type: InterviewType.Video, interviewer: 'Mark', status: InterviewStatus.Rescheduled, notes: 'Bring ID', rescheduleCount: 1, createdAt: '2026-03-02T12:00:00' },
    ],
    ...overrides,
  };
}

async function setup(response: RunnerDetail = detail(), canEdit?: boolean) {
  vi.mocked(getAgencyRunner).mockResolvedValue(response);
  const wrapper = mountWithPlugins(RunnerDetailPanel, { props: { requestId: 'req-1', runnerId: 'run-1', canEdit } });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function button(wrapper: Wrapper, text: string) {
  return wrapper.findAll('button').find((b) => b.text() === text);
}

function lastPrompt(): PromptOptions {
  const call = vi.mocked(getDialog().prompt).mock.calls.at(-1);
  if (!call) throw new Error('prompt not opened');
  return call[0] as PromptOptions;
}

describe('RunnerDetailPanel', () => {
  it('loads the runner and emits it', async () => {
    const wrapper = await setup();

    expect(getAgencyRunner).toHaveBeenCalledWith('req-1', 'run-1');
    expect(wrapper.emitted('loaded')).toEqual([[detail()]]);
  });

  it('renders the status history and the interviews', async () => {
    const wrapper = await setup();

    const history = wrapper.findAll('.status-timeline-item');
    expect(history).toHaveLength(2);
    expect(history[0].text()).toContain('jane@sigook.com');
    expect(history[0].text()).toContain('Sent');
    const interview = wrapper.find('tbody tr').text();
    expect(interview).toContain('Rescheduled');
    expect(interview).toContain('Mark');
    expect(interview).toContain('Bring ID');
    expect(wrapper.find('tbody .tag').classes()).toContain('is-warning');
  });

  it('shows the empty states', async () => {
    const wrapper = await setup(detail({ statusHistory: [], interviews: [] }));

    expect(wrapper.text()).toContain('No history yet');
    expect(wrapper.text()).toContain('No interviews yet');
  });

  it('shows the error when the runner fails to load', async () => {
    const error = new Error('load failed');
    vi.mocked(getAgencyRunner).mockRejectedValue(error);
    const wrapper = mountWithPlugins(RunnerDetailPanel, { props: { requestId: 'req-1', runnerId: 'run-1' } });
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(wrapper.find('.runner-detail-card').exists()).toBe(false);
  });

  it('offers interview actions only while an interview is scheduled', async () => {
    const scheduled = await setup();
    expect(button(scheduled, 'Add interview')).toBeDefined();
    expect(button(scheduled, 'Reschedule')).toBeDefined();

    const hired = await setup(detail({ status: RunnerStatus.Hired }));
    expect(button(hired, 'Add interview')).toBeUndefined();
    expect(button(hired, 'Reschedule')).toBeUndefined();
  });

  it('hides the interview actions when the runner cannot be edited', async () => {
    const wrapper = await setup(detail(), false);

    expect(button(wrapper, 'Add interview')).toBeUndefined();
    expect(button(wrapper, 'Reschedule')).toBeUndefined();
  });

  it('adds an interview and reloads', async () => {
    const wrapper = await setup();
    await button(wrapper, 'Add interview')?.trigger('click');
    await flushPromises();

    const modal = wrapper.findComponent({ name: 'RunnerInterviewModal' });
    expect(modal.props()).toEqual({ requestId: 'req-1', runnerId: 'run-1' });
    modal.vm.$emit('updated');
    await flushPromises();

    expect(wrapper.emitted('updated')).toHaveLength(1);
    expect(getAgencyRunner).toHaveBeenCalledTimes(2);
  });

  it('reschedules an interview from the date prompt', async () => {
    vi.mocked(rescheduleRunnerInterview).mockResolvedValue();
    const wrapper = await setup();
    await button(wrapper, 'Reschedule')?.trigger('click');

    const options = lastPrompt();
    expect(options.title).toBe('Reschedule interview');
    expect(options.inputAttrs).toEqual({ type: 'datetime-local', required: true });
    const dialog = { close: vi.fn() };
    await options.onConfirm('2026-03-10T09:30', dialog);

    expect(rescheduleRunnerInterview).toHaveBeenCalledWith('req-1', 'run-1', 'i1', { newDate: new Date('2026-03-10T09:30').toISOString() });
    expect(dialog.close).toHaveBeenCalledOnce();
    expect(showAlertSuccess).toHaveBeenCalledWith('Interview rescheduled');
    expect(wrapper.emitted('updated')).toHaveLength(1);
    expect(getAgencyRunner).toHaveBeenCalledTimes(2);
  });

  it('keeps the prompt open and shows the error when rescheduling fails', async () => {
    const error = new Error('reschedule failed');
    vi.mocked(rescheduleRunnerInterview).mockRejectedValue(error);
    const wrapper = await setup();
    await button(wrapper, 'Reschedule')?.trigger('click');
    const dialog = { close: vi.fn() };
    await lastPrompt().onConfirm('2026-03-10T09:30', dialog);

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(dialog.close).not.toHaveBeenCalled();
    expect(getAgencyRunner).toHaveBeenCalledOnce();
  });

  it('exposes a reload', async () => {
    const wrapper = await setup();
    await (wrapper.vm as unknown as { reload: () => Promise<void> }).reload();
    await flushPromises();

    expect(getAgencyRunner).toHaveBeenCalledTimes(2);
  });
});
