import { beforeEach, describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import { flushPromises } from '@vue/test-utils';
import { createPinia, setActivePinia, type Pinia } from 'pinia';
import Request from '@/modules/worker/requests/pages/Request.vue';
import { getWorkerRequest, workerRequestApplySelf } from '@/modules/worker/requests/api';
import { useWorkerStore } from '@/modules/worker/store';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { WorkerRequestDetail } from '@/modules/worker/requests/types';

const { push } = vi.hoisted(() => ({ push: vi.fn() }));

vi.mock('vue-router', async (importOriginal) => ({
  ...await importOriginal<typeof import('vue-router')>(),
  useRoute: () => ({ params: { id: 'r1' }, query: {} }),
  useRouter: () => ({ push }),
}));
vi.mock('@/modules/worker/requests/api', () => ({ getWorkerRequest: vi.fn(), workerRequestApplySelf: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));
vi.mock('@/shared/ui/Breadcrumbs.vue', () => ({ default: { name: 'Breadcrumbs', props: ['crumbs', 'backTo'], template: '<nav />' } }));
vi.mock('@/shared/request-detail/RequestHeader.vue', () => ({
  default: {
    name: 'RequestHeader',
    props: ['numberId', 'title', 'logoIcon', 'kpis'],
    template: '<div><div class="subtitle"><slot name="subtitle" /></div><div class="alert"><slot name="alert" /></div><div class="actions"><slot name="actions" /></div><slot /></div>',
  },
}));
vi.mock('@/shared/request-detail/RequestDetailTab.vue', () => ({
  default: {
    name: 'RequestDetailTab',
    props: ['fields', 'description', 'responsibilities', 'requirements', 'incentive', 'incentiveDescription', 'skills'],
    template: '<div class="detail-tab"><slot name="rail" /></div>',
  },
}));
vi.mock('@/shared/request-detail/RequestLocationCard.vue', () => ({ default: { name: 'RequestLocationCard', props: ['location'], template: '<div />' } }));
vi.mock('@/shared/ui/EditTextarea.vue', () => ({
  default: { name: 'EditTextarea', props: ['title', 'minLength'], emits: ['updateContent'], template: '<div class="edit-textarea" />' },
}));

let pinia: Pinia;

const openRequest: WorkerRequestDetail = {
  id: 'r1',
  numberId: 300,
  jobTitle: 'Picker',
  location: '1 King St',
  description: 'Pick orders',
  skills: ['Forklift'],
  requestStatus: 'Open',
  status: 'None',
  isApplicant: false,
  jobLocation: null,
};

async function setup(request: Partial<WorkerRequestDetail> = {}, profile: { id?: string; approvedToWork?: boolean } = { id: 'wp1', approvedToWork: true }) {
  useWorkerStore().setWorkerProfile(profile);
  vi.mocked(getWorkerRequest).mockResolvedValue({ ...openRequest, ...request });
  const wrapper = mountWithPlugins(Request, { global: { plugins: [pinia] } });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function applyButton(wrapper: Wrapper) {
  return wrapper.findAll('.actions button').find((b) => b.text() === 'Apply');
}

beforeEach(() => {
  pinia = createPinia();
  setActivePinia(pinia);
});

describe('Request', () => {
  it('shows the request detail', async () => {
    const wrapper = await setup();
    expect(getWorkerRequest).toHaveBeenCalledWith('r1');
    expect(wrapper.findComponent({ name: 'RequestHeader' }).props()).toMatchObject({ numberId: 300, title: 'Picker' });
    expect(wrapper.find('.subtitle').text()).toBe('1 King St');
    const tab = wrapper.findComponent({ name: 'RequestDetailTab' });
    expect(tab.props()).toMatchObject({ description: 'Pick orders', skills: ['Forklift'] });
    expect(tab.props('fields')[0]).toEqual({ label: 'Role (position)', value: 'Picker' });
    expect(wrapper.findComponent({ name: 'Breadcrumbs' }).props()).toEqual({ crumbs: [{ label: 'Jobs', to: '/worker-requests' }], backTo: '/worker-requests' });
  });

  it('lets an approved worker apply to an open request', async () => {
    vi.mocked(workerRequestApplySelf).mockResolvedValue();
    const wrapper = await setup();
    expect(wrapper.find('.alert').text()).toBe('');
    await applyButton(wrapper)?.trigger('click');
    await nextTick();
    wrapper.findComponent({ name: 'EditTextarea' }).vm.$emit('updateContent', 'Available mornings');
    await flushPromises();
    expect(workerRequestApplySelf).toHaveBeenCalledWith('r1', { comments: 'Available mornings' });
    expect(push).toHaveBeenCalledWith({ path: '/worker-requests/applied/r1' });
  });

  it('reports a failed application', async () => {
    vi.mocked(workerRequestApplySelf).mockRejectedValue('full');
    const wrapper = await setup();
    await applyButton(wrapper)?.trigger('click');
    await nextTick();
    wrapper.findComponent({ name: 'EditTextarea' }).vm.$emit('updateContent', '');
    await flushPromises();
    expect(showAlertError).toHaveBeenCalledWith('full');
    expect(push).not.toHaveBeenCalled();
  });

  it('warns a worker who is not approved', async () => {
    const wrapper = await setup({}, { id: 'wp1', approvedToWork: false });
    expect(wrapper.find('.alert').text()).toBe('You are not approved to work');
    expect(applyButton(wrapper)).toBeUndefined();
  });

  it('shows no warning before the profile is loaded', async () => {
    const wrapper = await setup({}, {});
    expect(wrapper.find('.alert').text()).toBe('');
  });

  it.each([
    [{ requestStatus: 'Filled' }],
    [{ requestStatus: 'Cancelled' }],
    [{ requestStatus: 'Other' }],
    [{ status: 'Rejected' }],
    [{ status: 'InQueue' }],
    [{ status: 'Declined' }],
    [{ isApplicant: true }],
  ])('does not offer apply for %o', async (overrides) => {
    expect(applyButton(await setup(overrides))).toBeUndefined();
  });

  it('stays empty when the request cannot be loaded', async () => {
    useWorkerStore().setWorkerProfile({});
    vi.mocked(getWorkerRequest).mockRejectedValue('gone');
    const wrapper = mountWithPlugins(Request, { global: { plugins: [pinia] } });
    await flushPromises();
    expect(wrapper.findComponent({ name: 'RequestHeader' }).exists()).toBe(false);
  });
});
