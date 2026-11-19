import { describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import RunnerActionModals from '@/modules/agency/recruiting/requests/runners/RunnerActionModals.vue';
import { RunnerStatus } from '@/modules/agency/recruiting/requests/runners/types';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { RunnerActionTarget } from '@/modules/agency/recruiting/requests/runners/useRunnerActions';

const { modalStub } = vi.hoisted(() => ({
  modalStub: (name: string, extraProps: Record<string, unknown> = {}) => ({
    name,
    props: { requestId: String, runnerId: String, ...extraProps },
    emits: ['updated', 'close'],
    template: `<div class="${name}" />`,
  }),
}));

vi.mock('@/modules/agency/recruiting/requests/runners/RunnerStatusModal.vue', () => ({ default: modalStub('RunnerStatusModal', { currentStatus: Number }) }));
vi.mock('@/modules/agency/recruiting/requests/runners/RunnerInterviewModal.vue', () => ({ default: modalStub('RunnerInterviewModal') }));
vi.mock('@/modules/agency/recruiting/requests/runners/RunnerHistoryModal.vue', () => ({ default: modalStub('RunnerHistoryModal') }));

const target: RunnerActionTarget = { requestId: 'req-1', runnerId: 'run-1', name: 'Ana', status: RunnerStatus.InterviewScheduled };

interface Props {
  target?: RunnerActionTarget | null;
  statusOpen?: boolean;
  interviewOpen?: boolean;
  historyOpen?: boolean;
}

function setup({ target: t = target, statusOpen = false, interviewOpen = false, historyOpen = false }: Props = {}) {
  return mountWithPlugins(RunnerActionModals, { props: { target: t, statusOpen, interviewOpen, historyOpen } });
}

describe('RunnerActionModals', () => {
  it('renders the status modal for the target', async () => {
    const wrapper = setup({ statusOpen: true });
    await nextTick();

    const modal = wrapper.findComponent({ name: 'RunnerStatusModal' });
    expect(modal.props()).toEqual({ requestId: 'req-1', runnerId: 'run-1', currentStatus: RunnerStatus.InterviewScheduled });
    expect(wrapper.findComponent({ name: 'RunnerInterviewModal' }).exists()).toBe(false);
    expect(wrapper.findComponent({ name: 'RunnerHistoryModal' }).exists()).toBe(false);
  });

  it('forwards updates and closes the status modal', async () => {
    const wrapper = setup({ statusOpen: true });
    await nextTick();
    const modal = wrapper.findComponent({ name: 'RunnerStatusModal' });
    modal.vm.$emit('updated');
    modal.vm.$emit('close');

    expect(wrapper.emitted('updated')).toHaveLength(1);
    expect(wrapper.emitted('update:statusOpen')).toEqual([[false]]);
  });

  it('forwards updates and closes the interview modal', async () => {
    const wrapper = setup({ interviewOpen: true });
    await nextTick();
    const modal = wrapper.findComponent({ name: 'RunnerInterviewModal' });
    expect(modal.props()).toEqual({ requestId: 'req-1', runnerId: 'run-1' });
    modal.vm.$emit('updated');
    modal.vm.$emit('close');

    expect(wrapper.emitted('updated')).toHaveLength(1);
    expect(wrapper.emitted('update:interviewOpen')).toEqual([[false]]);
  });

  it('forwards updates and closes the history modal', async () => {
    const wrapper = setup({ historyOpen: true });
    await nextTick();
    const modal = wrapper.findComponent({ name: 'RunnerHistoryModal' });
    modal.vm.$emit('updated');
    modal.vm.$emit('close');

    expect(wrapper.emitted('updated')).toHaveLength(1);
    expect(wrapper.emitted('update:historyOpen')).toEqual([[false]]);
  });

  it('renders nothing inside the modals without a target', async () => {
    const wrapper = setup({ target: null, statusOpen: true, interviewOpen: true, historyOpen: true });
    await nextTick();

    expect(wrapper.findComponent({ name: 'RunnerStatusModal' }).exists()).toBe(false);
    expect(wrapper.findComponent({ name: 'RunnerInterviewModal' }).exists()).toBe(false);
    expect(wrapper.findComponent({ name: 'RunnerHistoryModal' }).exists()).toBe(false);
  });
});
