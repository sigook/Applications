import { beforeEach, describe, expect, it, vi } from 'vitest';
import { computed, nextTick, ref } from 'vue';
import { flushPromises } from '@vue/test-utils';
import { createPinia, setActivePinia, type Pinia } from 'pinia';
import Requests from '@/modules/worker/requests/pages/Requests.vue';
import { getJobs } from '@/modules/worker/requests/api';
import { getMyProfile } from '@/modules/worker/profile/api';
import { getWorkerHistory } from '@/modules/worker/activity/api';
import { useWorkerStore } from '@/modules/worker/store';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createWorkerProfileDetail } from '@/testing/workerProfileFixtures';
import { WorkerRequestSortBy, type WorkerRequestListItem } from '@/modules/worker/requests/types';
import type { WorkerAssignmentListItem } from '@/modules/worker/activity/types';

const touch = ref(false);
const { push } = vi.hoisted(() => ({ push: vi.fn() }));

vi.mock('vue-router', async (importOriginal) => ({
  ...await importOriginal<typeof import('vue-router')>(),
  useRouter: () => ({ push }),
}));
vi.mock('@/shared/composables/useBreakpoint', () => ({
  useBreakpoint: () => ({ isTouch: computed(() => touch.value) }),
}));
vi.mock('@/shared/api/reportApi', () => ({ downloadAgencyReport: vi.fn() }));
vi.mock('@/modules/worker/requests/api', () => ({ getJobs: vi.fn() }));
vi.mock('@/modules/worker/profile/api', () => ({ getMyProfile: vi.fn() }));
vi.mock('@/modules/worker/activity/api', () => ({ getWorkerHistory: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));
vi.mock('@/shared/ui/MobileFiltersPanel.vue', () => ({
  default: {
    name: 'MobileFiltersPanel',
    props: ['modelValue', 'activeCount'],
    emits: ['update:modelValue', 'apply', 'clear'],
    template: '<div class="mobile-filters"><slot /></div>',
  },
}));
vi.mock('@/modules/worker/requests/components/CurrentJobCard.vue', () => ({
  default: { name: 'CurrentJobCard', props: ['assignment'], template: '<div class="current-job" />' },
}));

let pinia: Pinia;

const openJob: WorkerRequestListItem = {
  id: 'r1',
  numberId: 300,
  jobTitle: 'Picker',
  location: '1 King St',
  entrance: 'Door 4',
  status: 'None',
  isAsap: true,
  workerRate: 21.5,
  startAt: '2026-06-01T12:00:00',
  finishAt: '2026-06-30T12:00:00',
  durationTerm: 'ShortTerm',
};

const appliedJob: WorkerRequestListItem = { ...openJob, id: 'r2', numberId: 301, status: 'Applied', isAsap: false, entrance: '', durationTerm: 'LongTerm' };
const filledJob: WorkerRequestListItem = { ...openJob, id: 'r3', numberId: 302, status: 'Filled', isAsap: false, durationTerm: 'LongTerm' };

const active: WorkerAssignmentListItem = { id: 'a1', jobTitle: 'Driver', hoursWorked: 10, status: 'Active' };

async function setup({ profileLoaded = true, activeItems = [] as WorkerAssignmentListItem[] } = {}) {
  if (profileLoaded) useWorkerStore().setWorkerProfile(createWorkerProfileDetail());
  vi.mocked(getMyProfile).mockResolvedValue(createWorkerProfileDetail({ approvedToWork: false, missingRequirements: ['Identification'] }));
  vi.mocked(getWorkerHistory).mockResolvedValue({ items: activeItems, totalItems: activeItems.length, pageIndex: 1, totalPages: 1 });
  vi.mocked(getJobs).mockResolvedValue({ items: [openJob, appliedJob, filledJob], totalItems: 3, pageIndex: 1, totalPages: 1 });
  const wrapper = mountWithPlugins(Requests, { global: { plugins: [pinia] } });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function grid(wrapper: Wrapper) {
  return wrapper.findComponent({ name: 'SigookGrid' });
}

beforeEach(() => {
  touch.value = false;
  pinia = createPinia();
  setActivePinia(pinia);
});

describe('Requests', () => {
  it('lists the open jobs newest first', async () => {
    const wrapper = await setup();
    expect(getWorkerHistory).toHaveBeenCalledWith({ status: 'Active', pageIndex: 1, pageSize: 10 });
    expect(getMyProfile).not.toHaveBeenCalled();
    expect(getJobs).toHaveBeenCalledWith(expect.objectContaining({ sortBy: WorkerRequestSortBy.NumberId, isDescending: true }));
    const rows = wrapper.findAll('tbody tr').map((r) => r.findAll('td').map((c) => c.text().replace(/\s+/g, ' ')));
    expect(rows[0]).toEqual(['300 Asap', 'Picker', '1 King St - Door 4', '01-Jun-2026 - 30-Jun-2026Short Term', '$21.50', '']);
    expect(rows[1]).toEqual(['301', 'Picker', '1 King St', '01-Jun-2026 Long Term', '$21.50', 'Applied']);
    expect(rows[2][3]).toBe('01-Jun-2026 - 30-Jun-2026Long Term');
    expect(wrapper.find('.b-loading-overlay.is-active').exists()).toBe(false);
  });

  it('loads the profile when the store is empty and warns about it', async () => {
    const wrapper = await setup({ profileLoaded: false });
    expect(getMyProfile).toHaveBeenCalledOnce();
    expect(wrapper.text()).toContain('You are not approved to work');
    expect(wrapper.text()).toContain('Your Profile is Incomplete');
    const link = wrapper.find('.normal-list a');
    expect(link.text()).toBe('Identification document');
    expect(link.attributes('href')).toBe('/worker-profile?section=documents');
  });

  it('reports a failed profile or assignments load', async () => {
    vi.mocked(getMyProfile).mockRejectedValue('profile down');
    vi.mocked(getWorkerHistory).mockRejectedValue('history down');
    vi.mocked(getJobs).mockResolvedValue({ items: [], totalItems: 0, pageIndex: 1, totalPages: 0 });
    mountWithPlugins(Requests, { global: { plugins: [pinia] } });
    await flushPromises();
    expect(showAlertError).toHaveBeenCalledWith('profile down');
    expect(showAlertError).toHaveBeenCalledWith('history down');
  });

  it('shows the current jobs instead of the list', async () => {
    const wrapper = await setup({ activeItems: [active] });
    expect(wrapper.findComponent({ name: 'CurrentJobCard' }).props('assignment')).toEqual(active);
    expect(grid(wrapper).exists()).toBe(false);
    expect(getJobs).not.toHaveBeenCalled();
  });

  it.each([
    [openJob, '/worker-requests/r1'],
    [appliedJob, '/worker-requests/applied/r2'],
    [{ ...openJob, status: 'Booked' }, '/worker-requests/applied/r1'],
  ])('opens the job by its status', async (row, path) => {
    const wrapper = await setup();
    grid(wrapper).vm.$emit('click', row);
    expect(push).toHaveBeenCalledWith({ path });
  });

  it('filters and sorts from the mobile panel', async () => {
    touch.value = true;
    const wrapper = await setup();
    expect(wrapper.findAll('.job-card')).toHaveLength(3);
    await wrapper.find('.sigook-grid-actions button').trigger('click');
    const panel = wrapper.findComponent({ name: 'MobileFiltersPanel' });
    expect(panel.props()).toMatchObject({ modelValue: true, activeCount: 0 });

    const inputs = panel.findAll('input');
    await inputs[0].setValue('301');
    await inputs[1].setValue('Driver');
    await panel.find('select').setValue('lowestRate');
    expect(panel.props('activeCount')).toBe(3);

    panel.vm.$emit('apply');
    await flushPromises();
    expect(getJobs).toHaveBeenLastCalledWith(expect.objectContaining({
      numberId: '301', jobTitle: 'Driver', sortBy: WorkerRequestSortBy.Rate, isDescending: false, pageIndex: 1,
    }));

    panel.vm.$emit('clear');
    await flushPromises();
    expect(getJobs).toHaveBeenLastCalledWith(expect.objectContaining({
      numberId: undefined, jobTitle: undefined, sortBy: WorkerRequestSortBy.NumberId, isDescending: true,
    }));
    expect(panel.props('activeCount')).toBe(0);
  });

  it('opens a job from its mobile card', async () => {
    touch.value = true;
    const wrapper = await setup();
    await wrapper.findAll('.job-card')[1].trigger('click');
    await nextTick();
    expect(push).toHaveBeenCalledWith({ path: '/worker-requests/applied/r2' });
  });
});
