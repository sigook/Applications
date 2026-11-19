import { describe, expect, it, vi } from 'vitest';
import CurrentJobCard from '@/modules/worker/requests/components/CurrentJobCard.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { WorkerAssignmentListItem } from '@/modules/worker/activity/types';

const { push } = vi.hoisted(() => ({ push: vi.fn() }));

vi.mock('vue-router', async (importOriginal) => ({
  ...await importOriginal<typeof import('vue-router')>(),
  useRouter: () => ({ push }),
}));

const assignment: WorkerAssignmentListItem = {
  id: 'r1',
  numberId: 300,
  jobTitle: 'Picker',
  clientName: 'Acme',
  location: 'Toronto',
  startAt: '2026-05-01T12:00:00',
  startWorking: '2026-05-04T12:00:00',
  status: 'Active',
  hoursWorked: 40,
};

function setup(overrides: Partial<WorkerAssignmentListItem> = {}) {
  return mountWithPlugins(CurrentJobCard, { props: { assignment: { ...assignment, ...overrides } } });
}

describe('CurrentJobCard', () => {
  it('shows the current assignment', () => {
    const wrapper = setup();
    expect(wrapper.find('.current-job__title').text()).toBe('Picker');
    expect(wrapper.findAll('.current-job__sub').map((s) => s.text())).toEqual(['Acme · Toronto', 'Since 04-May-2026 · #300']);
    expect(wrapper.find('.tag').classes()).toContain('is-info');
    expect(wrapper.find('a').attributes('href')).toBe('/worker-requests/applied/r1?tab=punch');
  });

  it('falls back to the request start and skips a missing client', () => {
    const wrapper = setup({ startWorking: null, clientName: undefined });
    expect(wrapper.findAll('.current-job__sub').map((s) => s.text())).toEqual(['Toronto', 'Since 01-May-2026 · #300']);
  });

  it('opens the assignment', async () => {
    await setup().find('.current-job').trigger('click');
    expect(push).toHaveBeenCalledWith('/worker-requests/applied/r1');
  });
});
