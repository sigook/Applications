import { describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import RunnerHistoryModal from '@/modules/agency/recruiting/requests/runners/RunnerHistoryModal.vue';
import { RunnerStatus, RunnerType, type RunnerDetail } from '@/modules/agency/recruiting/requests/runners/types';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/modules/agency/recruiting/runners/components/RunnerDetailPanel.vue', () => ({
  default: {
    name: 'RunnerDetailPanel',
    props: { requestId: String, runnerId: String },
    emits: ['updated', 'loaded'],
    template: '<div class="panel-stub" />',
  },
}));

const detail: RunnerDetail = {
  id: 'run-1',
  numberId: 7,
  requestId: 'req-1',
  workerProfileId: 'wp-1',
  name: 'Ana Lopez',
  email: 'ana@mail.com',
  type: RunnerType.Active,
  status: RunnerStatus.SentToClient,
  createdAt: '2026-11-01T12:00:00',
  statusHistory: [],
  interviews: [],
};

function setup() {
  return mountWithPlugins(RunnerHistoryModal, { props: { requestId: 'req-1', runnerId: 'run-1' } });
}

describe('RunnerHistoryModal', () => {
  it('shows a generic title until the detail loads', () => {
    const wrapper = setup();

    expect(wrapper.find('.modal-card-title').text()).toBe('History');
    expect(wrapper.find('.copy-link').exists()).toBe(false);
    expect(wrapper.findComponent({ name: 'RunnerDetailPanel' }).props()).toEqual({ requestId: 'req-1', runnerId: 'run-1' });
  });

  it('shows the runner name and email once loaded', async () => {
    const wrapper = setup();
    wrapper.findComponent({ name: 'RunnerDetailPanel' }).vm.$emit('loaded', detail);
    await nextTick();

    expect(wrapper.find('.modal-card-title').text()).toBe('Ana Lopez');
    expect(wrapper.find('.copy-link a').attributes('href')).toBe('mailto:ana@mail.com');
  });

  it('omits the email when the runner has none', async () => {
    const wrapper = setup();
    wrapper.findComponent({ name: 'RunnerDetailPanel' }).vm.$emit('loaded', { ...detail, email: undefined });
    await nextTick();

    expect(wrapper.find('.copy-link').exists()).toBe(false);
  });

  it('forwards updates and closes', async () => {
    const wrapper = setup();
    wrapper.findComponent({ name: 'RunnerDetailPanel' }).vm.$emit('updated');
    await wrapper.find('footer button').trigger('click');

    expect(wrapper.emitted('updated')).toHaveLength(1);
    expect(wrapper.emitted('close')).toHaveLength(1);
  });
});
