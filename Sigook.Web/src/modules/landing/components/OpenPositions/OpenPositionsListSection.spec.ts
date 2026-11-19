import { beforeEach, describe, expect, it, vi } from 'vitest';
import { flushPromises, mount } from '@vue/test-utils';
import Buefy from 'buefy';
import { createMemoryHistory, createRouter, type Router } from 'vue-router';
import OpenPositionsListSection from '@/modules/landing/components/OpenPositions/OpenPositionsListSection.vue';
import { getJobs } from '@/modules/landing/api';
import { useJobPostingSchema } from '@/modules/landing/composables/useJobPostingSchema';
import type { JobViewModel } from '@/modules/landing/types';

vi.mock('@/modules/landing/api', () => ({ getJobs: vi.fn() }));
vi.mock('@/modules/landing/composables/useJobPostingSchema', () => ({ useJobPostingSchema: vi.fn() }));
vi.mock('@/modules/landing/components/OpenPositions/JobDetail.vue', () => ({
  default: { name: 'JobDetail', props: { job: { type: Object, required: true } }, template: '<div class="job-detail-stub">{{ job.numberId }}</div>' },
}));

class FakeMediaQueryList {
  readonly listeners: (() => void)[] = [];
  readonly removeEventListener = vi.fn();

  constructor(public matches: boolean) {}

  addEventListener(_type: string, listener: () => void): void {
    this.listeners.push(listener);
  }
}

let media: FakeMediaQueryList;

function job(numberId: string, overrides: Partial<JobViewModel> = {}): JobViewModel {
  return {
    id: `job-${numberId}`,
    requestId: `request-${numberId}`,
    numberId,
    title: `Job ${numberId}`,
    salary: '$20.00',
    location: 'Miami, FL',
    type: 'Full time',
    createdAt: '2026-11-01T00:00:00Z',
    description: '',
    requirements: '',
    responsibilities: '',
    shift: '',
    createdBy: 'agency',
    agencyId: null,
    ...overrides,
  };
}

const jobs = [job('101', { title: 'Forklift Operator' }), job('102', { title: 'Welder', salary: '$0.00' }), job('103', { title: 'Driver', salary: '' })];

function createAppRouter(): Router {
  const Blank = { render: () => null };
  return createRouter({
    history: createMemoryHistory(),
    routes: [{ path: '/open-positions/:numberId?', name: 'open-positions', component: Blank }],
  });
}

async function setup(path = '/open-positions', mobile = false) {
  media = new FakeMediaQueryList(mobile);
  vi.stubGlobal('matchMedia', vi.fn(() => media));
  const router = createAppRouter();
  await router.push(path);
  await router.isReady();
  const replace = vi.spyOn(router, 'replace');
  const wrapper = mount(OpenPositionsListSection, { attachTo: document.body, global: { plugins: [Buefy, router] } });
  await flushPromises();
  return { wrapper, router, replace };
}

type Wrapper = Awaited<ReturnType<typeof setup>>['wrapper'];

function items(wrapper: Wrapper) {
  return wrapper.findAll('.op-list__item');
}

function asideJob(wrapper: Wrapper) {
  return wrapper.find('.op-list__detail .job-detail-stub').text();
}

describe('OpenPositionsListSection', () => {
  beforeEach(() => {
    vi.spyOn(Element.prototype, 'scrollIntoView').mockImplementation(() => undefined);
    vi.mocked(getJobs).mockResolvedValue(jobs);
  });

  it('shows a loading message while fetching', async () => {
    vi.mocked(getJobs).mockReturnValue(new Promise(() => undefined));
    media = new FakeMediaQueryList(false);
    vi.stubGlobal('matchMedia', vi.fn(() => media));
    const router = createAppRouter();
    await router.push('/open-positions');
    const wrapper = mount(OpenPositionsListSection, { global: { plugins: [Buefy, router] } });
    await flushPromises();

    expect(wrapper.find('.op-list__empty').text()).toBe('Loading open roles…');
  });

  it('fetches the jobs with the route filters', async () => {
    await setup('/open-positions/102?jobTitle=weld&location=Miami');

    expect(getJobs).toHaveBeenCalledWith({ jobId: '102', jobTitle: 'weld', location: 'Miami' });
  });

  it('lists the jobs and selects the first one on desktop', async () => {
    const { wrapper } = await setup();

    expect(items(wrapper).map((i) => i.find('.op-list__item-title').text())).toEqual(['Forklift Operator', 'Welder', 'Driver']);
    expect(items(wrapper)[0].attributes('href')).toBe('/open-positions/101');
    expect(items(wrapper)[0].classes()).toContain('op-list__item--active');
    expect(asideJob(wrapper)).toBe('101');
  });

  it('hides empty or zero salaries', async () => {
    const { wrapper } = await setup();

    expect(items(wrapper).map((i) => i.find('.op-list__item-salary').exists())).toEqual([true, false, false]);
  });

  it('selects the job from the url and scrolls to it', async () => {
    const { wrapper } = await setup('/open-positions/103');

    expect(asideJob(wrapper)).toBe('103');
    expect(Element.prototype.scrollIntoView).toHaveBeenCalledWith({ behavior: 'smooth', block: 'center' });
  });

  it('falls back to the first job for an unknown id', async () => {
    const { wrapper } = await setup('/open-positions/999');

    expect(asideJob(wrapper)).toBe('101');
    expect(Element.prototype.scrollIntoView).not.toHaveBeenCalled();
  });

  it('selects a job and syncs the url on desktop', async () => {
    const { wrapper, replace } = await setup('/open-positions?location=Miami');
    await items(wrapper)[1].trigger('click');
    await flushPromises();

    expect(asideJob(wrapper)).toBe('102');
    expect(replace).toHaveBeenCalledWith({ name: 'open-positions', params: { numberId: '102' }, query: { location: 'Miami' } });
    expect(wrapper.find('.op-list__inline-detail').exists()).toBe(false);
  });

  it('does not rewrite the url for the job already in it', async () => {
    const { wrapper, replace } = await setup('/open-positions/101');
    await items(wrapper)[0].trigger('click');

    expect(replace).not.toHaveBeenCalled();
  });

  it('follows url changes to another listed job', async () => {
    const { wrapper, router } = await setup();
    await router.push('/open-positions/103');
    await flushPromises();
    expect(asideJob(wrapper)).toBe('103');

    await router.push('/open-positions/999');
    await flushPromises();
    expect(asideJob(wrapper)).toBe('103');
  });

  it('expands the selected job inline on mobile', async () => {
    const { wrapper } = await setup('/open-positions', true);

    expect(wrapper.find('.op-list__detail').exists()).toBe(true);
    expect(wrapper.findAll('.op-list__inline-detail').map((d) => d.text())).toEqual(['101']);
    expect(items(wrapper)[0].attributes('aria-expanded')).toBe('true');
  });

  it('toggles inline details on mobile', async () => {
    const { wrapper, replace } = await setup('/open-positions', true);
    await items(wrapper)[2].trigger('click');
    await flushPromises();

    expect(wrapper.findAll('.op-list__inline-detail').map((d) => d.text())).toEqual(['101', '103']);
    expect(replace).toHaveBeenCalledWith({ name: 'open-positions', params: { numberId: '103' }, query: {} });
    expect(items(wrapper)[2].classes()).toContain('op-list__item--active');

    await items(wrapper)[0].trigger('click');
    expect(wrapper.findAll('.op-list__inline-detail').map((d) => d.text())).toEqual(['103']);
    expect(items(wrapper)[0].classes()).not.toContain('op-list__item--active');
  });

  it('switches layout when the viewport changes', async () => {
    const { wrapper } = await setup();
    media.matches = true;
    media.listeners.forEach((listener) => { listener(); });
    await items(wrapper)[1].trigger('click');

    expect(wrapper.findAll('.op-list__inline-detail').map((d) => d.text())).toEqual(['102']);
  });

  it('stops listening to the viewport on unmount', async () => {
    const { wrapper } = await setup();
    wrapper.unmount();

    expect(media.removeEventListener).toHaveBeenCalledWith('change', expect.any(Function));
  });

  it('shows the error with a retry', async () => {
    vi.mocked(getJobs).mockRejectedValueOnce(new Error('down'));
    const { wrapper } = await setup();

    expect(wrapper.find('.op-list__empty').text()).toContain('We could not load the open positions. Please try again.');
    await wrapper.find('.op-list__empty-reset').trigger('click');
    await flushPromises();
    expect(getJobs).toHaveBeenCalledTimes(2);
    expect(items(wrapper)).toHaveLength(3);
  });

  it('shows the empty state', async () => {
    vi.mocked(getJobs).mockResolvedValue([]);
    const { wrapper } = await setup();

    expect(wrapper.find('.op-list__empty').text()).toContain('No openings match your search.');
    expect(wrapper.find('.op-list__body').classes()).toContain('op-list__body--empty');
    expect(wrapper.find('.op-list__empty-reset').exists()).toBe(false);
    expect(wrapper.find('.op-list__detail').exists()).toBe(false);
  });

  it('feeds the job in the url to the posting schema', async () => {
    await setup('/open-positions/102');
    const getter = vi.mocked(useJobPostingSchema).mock.calls.at(-1)?.[0];

    expect(getter?.()).toEqual(jobs[1]);
  });

  it('feeds nothing to the posting schema without a job in the url', async () => {
    await setup();
    const getter = vi.mocked(useJobPostingSchema).mock.calls.at(-1)?.[0];

    expect(getter?.()).toBeUndefined();
  });
});
