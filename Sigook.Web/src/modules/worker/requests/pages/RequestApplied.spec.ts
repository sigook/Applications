import { beforeEach, describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import RequestApplied from '@/modules/worker/requests/pages/RequestApplied.vue';
import { getWorkerRequest } from '@/modules/worker/requests/api';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { WorkerRequestDetail } from '@/modules/worker/requests/types';

const { query } = vi.hoisted(() => ({ query: { value: {} as Record<string, string> } }));

vi.mock('vue-router', async (importOriginal) => ({
  ...await importOriginal<typeof import('vue-router')>(),
  useRoute: () => ({ params: { id: 'r1' }, query: query.value }),
}));
vi.mock('@/modules/worker/requests/api', () => ({ getWorkerRequest: vi.fn() }));
vi.mock('@/shared/ui/Breadcrumbs.vue', () => ({ default: { name: 'Breadcrumbs', props: ['crumbs', 'backTo'], template: '<nav />' } }));
vi.mock('@/shared/request-detail/RequestHeader.vue', () => ({
  default: {
    name: 'RequestHeader',
    props: ['numberId', 'title', 'logoIcon', 'kpis'],
    template: '<div><div class="subtitle"><slot name="subtitle" /></div><slot /></div>',
  },
}));
vi.mock('@/shared/request-detail/RequestDetailTab.vue', () => ({
  default: { name: 'RequestDetailTab', props: ['fields', 'description', 'responsibilities', 'requirements', 'incentive', 'incentiveDescription', 'skills'], template: '<div class="detail-tab"><slot name="rail" /></div>' },
}));
vi.mock('@/shared/request-detail/RequestLocationCard.vue', () => ({ default: { name: 'RequestLocationCard', props: ['location'], template: '<div />' } }));
vi.mock('@/modules/worker/requests/components/PunchCard.vue', () => ({
  default: { name: 'PunchCard', props: ['requestId', 'request'], emits: ['refreshRequest'], template: '<div class="punch-card" />' },
}));

const applied: WorkerRequestDetail = {
  id: 'r1',
  numberId: 300,
  jobTitle: 'Picker',
  location: '1 King St',
  punchCardOptionEnabled: true,
};

async function setup(request: Partial<WorkerRequestDetail> = {}) {
  vi.mocked(getWorkerRequest).mockResolvedValue({ ...applied, ...request });
  const wrapper = mountWithPlugins(RequestApplied);
  await flushPromises();
  return wrapper;
}

function tabs(wrapper: Awaited<ReturnType<typeof setup>>) {
  return wrapper.findComponent({ name: 'BTabs' });
}

beforeEach(() => {
  query.value = {};
});

describe('RequestApplied', () => {
  it('starts on the detail and loads the punch card on demand', async () => {
    const wrapper = await setup();
    expect(getWorkerRequest).toHaveBeenCalledWith('r1');
    expect(wrapper.find('.subtitle').text()).toBe('1 King St');
    expect(tabs(wrapper).props('modelValue')).toBe('Summary Request');
    expect(wrapper.findComponent({ name: 'RequestDetailTab' }).exists()).toBe(true);
    expect(wrapper.findComponent({ name: 'PunchCard' }).exists()).toBe(false);

    tabs(wrapper).vm.$emit('update:modelValue', 'Punch Card');
    await flushPromises();
    expect(wrapper.findComponent({ name: 'PunchCard' }).props()).toMatchObject({ requestId: 'r1' });
    tabs(wrapper).vm.$emit('update:modelValue', 'Punch Card');
    await flushPromises();
    expect(wrapper.findAllComponents({ name: 'PunchCard' })).toHaveLength(1);
  });

  it('opens the punch card from the query', async () => {
    query.value = { tab: 'punch' };
    const wrapper = await setup();
    expect(tabs(wrapper).props('modelValue')).toBe('Punch Card');
    expect(wrapper.findComponent({ name: 'PunchCard' }).exists()).toBe(true);
  });

  it('has no punch card when the option is off', async () => {
    query.value = { tab: 'punch' };
    const wrapper = await setup({ punchCardOptionEnabled: false });
    expect(tabs(wrapper).props('modelValue')).toBe('Summary Request');
    expect(wrapper.text()).not.toContain('Punch Card');
  });

  it('refreshes the request after a punch', async () => {
    query.value = { tab: 'punch' };
    const wrapper = await setup();
    vi.mocked(getWorkerRequest).mockResolvedValue({ ...applied, jobTitle: 'Senior picker' });
    wrapper.findComponent({ name: 'PunchCard' }).vm.$emit('refreshRequest');
    await flushPromises();
    expect(wrapper.findComponent({ name: 'RequestHeader' }).props('title')).toBe('Senior picker');
  });

  it('keeps the request when a refresh fails', async () => {
    query.value = { tab: 'punch' };
    const wrapper = await setup();
    vi.mocked(getWorkerRequest).mockRejectedValue('down');
    wrapper.findComponent({ name: 'PunchCard' }).vm.$emit('refreshRequest');
    await flushPromises();
    expect(wrapper.findComponent({ name: 'RequestHeader' }).props('title')).toBe('Picker');
  });

  it('stays empty when the request cannot be loaded', async () => {
    vi.mocked(getWorkerRequest).mockRejectedValue('gone');
    const wrapper = mountWithPlugins(RequestApplied);
    await flushPromises();
    expect(wrapper.findComponent({ name: 'RequestHeader' }).exists()).toBe(false);
  });
});
