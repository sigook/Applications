import { describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import CompanyWorkers from '@/modules/agency/recruiting/clients/components/CompanyWorkers.vue';
import { getAgencyWorkers } from '@/modules/agency/recruiting/workers/api';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { AgencyWorkerListItem } from '@/modules/agency/recruiting/workers/types';

const { push } = vi.hoisted(() => ({ push: vi.fn() }));

vi.mock('vue-router', async (importOriginal) => ({
  ...await importOriginal<typeof import('vue-router')>(),
  useRouter: () => ({ push }),
}));
vi.mock('@/app/stores/security', () => ({ useSecurityStore: () => ({ userRoles: [] }) }));
vi.mock('@/modules/agency/recruiting/workers/api', () => ({ getAgencyWorkers: vi.fn() }));

const working = {
  id: 'w1',
  agencyId: 'agency-1',
  numberId: 77,
  fullName: 'John Smith',
  email: 'john@mail.com',
  mobileNumber: '416 555-0111',
  approvedToWork: true,
  dnu: false,
  isCurrentlyWorking: true,
  isSubcontractor: true,
  profileImage: 'https://files/john.png',
  createdAt: '2026-03-10T15:00:00Z',
  requests: [{ id: 'r1', value: 'R-100' }],
  skills: ['Forklift'],
};
const idle = {
  id: 'w2',
  agencyId: 'agency-1',
  numberId: 78,
  fullName: 'Mary Major',
  email: 'mary@mail.com',
  mobileNumber: '',
  approvedToWork: false,
  dnu: true,
  isCurrentlyWorking: false,
  isSubcontractor: false,
  profileImage: null,
  createdAt: '2026-03-11T15:00:00Z',
  requests: [],
  skills: [],
};

async function setup(items: AgencyWorkerListItem[] = [working, idle]) {
  vi.mocked(getAgencyWorkers).mockResolvedValue({ items, totalItems: items.length, pageIndex: 1, totalPages: 1 });
  const wrapper = mountWithPlugins(CompanyWorkers, { props: { company: { id: 'company-1' } } });
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function lastFilter() {
  return vi.mocked(getAgencyWorkers).mock.lastCall?.[0];
}

function rowTags(wrapper: Wrapper, index: number) {
  return wrapper.findAll('tbody tr')[index].findAll('td:last-child .tag').map((t) => t.text());
}

describe('CompanyWorkers', () => {
  it('loads the workers of the company sorted by name', async () => {
    await setup();

    expect(lastFilter()).toMatchObject({ companyProfileId: 'company-1', sortBy: 0, isDescending: false, pageIndex: 1 });
  });

  it('shows each worker with its details', async () => {
    const wrapper = await setup();
    const [first, second] = wrapper.findAll('tbody tr');

    expect(first.find('img').attributes('src')).toBe('https://files/john.png');
    expect(first.find('.Blue').text()).toBe('77');
    expect(first.text()).toContain('john@mail.com');
    expect(first.text()).toContain('R-100');
    expect(first.text()).toContain('Forklift');
    expect(rowTags(wrapper, 0)).toEqual(['Working', 'Approved To Work', 'Subcontractor']);
    expect(second.find('img').exists()).toBe(false);
    expect(second.find('.Blue').exists()).toBe(false);
    expect(second.text()).toContain('Skill');
    expect(rowTags(wrapper, 1)).toEqual(['DNU']);
  });

  it('opens the worker when a row is clicked', async () => {
    const wrapper = await setup();
    await wrapper.findAll('tbody tr')[1].find('td:nth-child(3)').trigger('click');

    expect(push).toHaveBeenCalledWith('/recruiting/workers/w2');
  });

  it('formats the phone filter and searches on enter', async () => {
    const wrapper = await setup();
    const phone = wrapper.findAll('thead input[placeholder="Search..."]')[2];
    await phone.setValue('4165550111');
    await phone.trigger('keyup.enter');
    await flushPromises();

    expect(lastFilter()).toMatchObject({ phone: '416 555-0111', pageIndex: 1 });
  });

  it('filters by details', async () => {
    const wrapper = await setup();
    wrapper.findComponent({ name: 'BTaginput' }).vm.$emit('update:modelValue', [{ id: 2, value: 'DNU' }, { id: 4, value: 'Subcontractor' }]);
    await flushPromises();

    expect(lastFilter()).toMatchObject({ features: [2, 4], pageIndex: 1 });
  });

  it('filters by creation date range and clears it', async () => {
    const wrapper = await setup();
    const picker = wrapper.findComponent({ name: 'BDatepicker' });
    picker.vm.$emit('update:modelValue', [new Date('2026-03-01T05:00:00Z'), new Date('2026-03-31T04:00:00Z')]);
    await flushPromises();

    expect(lastFilter()).toMatchObject({ createdAtFrom: '2026-03-01T05:00:00.000Z', createdAtTo: '2026-03-31T04:00:00.000Z' });

    picker.vm.$emit('icon-right-click');
    await flushPromises();

    expect(lastFilter()).toMatchObject({ createdAtFrom: null, createdAtTo: null });
  });
});
