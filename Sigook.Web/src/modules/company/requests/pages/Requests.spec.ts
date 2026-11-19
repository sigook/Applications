import { beforeEach, describe, expect, it, vi } from 'vitest';
import { computed, ref } from 'vue';
import { flushPromises } from '@vue/test-utils';
import Requests from '@/modules/company/requests/pages/Requests.vue';
import { getRequestShift, getRequests } from '@/modules/company/requests/api';
import { RequestStatus } from '@/shared/constants/enums';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { CompanyRequestFilter, CompanyRequestListItem } from '@/modules/company/requests/types';

const touch = ref(false);

const { push, store } = vi.hoisted(() => ({
  push: vi.fn(),
  store: {
    companyRequestFilter: null as CompanyRequestFilter | null,
    setCompanyRequestFilter: vi.fn(),
  },
}));

vi.mock('vue-router', async (importOriginal) => ({
  ...await importOriginal<typeof import('vue-router')>(),
  useRouter: () => ({ push }),
}));
vi.mock('@/modules/company/store', () => ({ useCompanyStore: () => store }));
vi.mock('@/shared/composables/useBreakpoint', () => ({
  useBreakpoint: () => ({ isTouch: computed(() => touch.value) }),
}));
vi.mock('@/modules/company/requests/api', () => ({ getRequests: vi.fn(), getRequestShift: vi.fn() }));
vi.mock('@/shared/api/reportApi', () => ({ downloadAgencyReport: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));
vi.mock('@/shared/ui/PageHeader.vue', () => ({
  default: { name: 'PageHeader', props: { title: String, count: Number }, template: '<h2 class="page-header">{{ title }} {{ count }}</h2>' },
}));
vi.mock('@/shared/request-detail/AgencyShiftDetail.vue', () => ({
  default: { name: 'AgencyShift', props: { requestId: String, displayShift: String, fetchShift: Function }, template: '<span class="shift">{{ displayShift }}</span>' },
}));
vi.mock('@/shared/ui/MobileFiltersPanel.vue', () => ({
  default: {
    name: 'MobileFiltersPanel',
    props: { modelValue: Boolean, activeCount: Number },
    emits: ['update:modelValue', 'apply', 'clear'],
    template: '<div class="mobile-filters"><slot /></div>',
  },
}));

function item(overrides: Partial<CompanyRequestListItem>): CompanyRequestListItem {
  return {
    id: 'request-1', numberId: 501, jobTitle: 'Forklift operator', location: '1 King St', entrance: 'Door 3',
    displayShift: 'Mon-Fri 08:00-16:00', workersQuantity: 4, workersQuantityWorking: 2, requestStatus: RequestStatus.Open,
    isAsap: true, isDirectHiring: false, createdAt: '2026-11-18T12:00:00', ...overrides,
  };
}

const inProgress = item({});
const filled = item({ id: 'request-2', numberId: 502, jobTitle: 'Packer', entrance: '', workersQuantity: 3, workersQuantityWorking: 3, requestStatus: RequestStatus.Filled, isAsap: false, isDirectHiring: true });
const open = item({ id: 'request-3', numberId: 503, workersQuantity: 1, workersQuantityWorking: 0, isAsap: false });

async function setup(items: CompanyRequestListItem[] = [inProgress, filled, open]) {
  vi.mocked(getRequests).mockResolvedValue({ items, totalItems: 12, pageIndex: 1, totalPages: 1 });
  const wrapper = mountWithPlugins(Requests);
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function lastFilter() {
  return vi.mocked(getRequests).mock.lastCall?.[0];
}

function rows(wrapper: Wrapper) {
  return wrapper.findAll('tbody tr');
}

beforeEach(() => {
  touch.value = false;
  store.companyRequestFilter = null;
  push.mockResolvedValue(undefined);
});

describe('Requests', () => {
  it('loads the requests newest first and remembers the filter', async () => {
    const wrapper = await setup();

    const params = { sortBy: 0, isDescending: true, pageIndex: 1, pageSize: 30 };
    expect(getRequests).toHaveBeenCalledExactlyOnceWith(params);
    expect(store.setCompanyRequestFilter).toHaveBeenCalledWith(params);
    expect(wrapper.find('.page-header').text()).toBe('Staff Requests 12');
    expect(rows(wrapper)).toHaveLength(3);
  });

  it('restores the stored filter', async () => {
    store.companyRequestFilter = { sortBy: 2, isDescending: false, pageIndex: 2, pageSize: 60, jobTitle: 'pack' };
    await setup();

    expect(getRequests).toHaveBeenNthCalledWith(1, { sortBy: 2, isDescending: false, pageIndex: 2, pageSize: 60, jobTitle: 'pack' });
  });

  it('shows the request columns, flags, status and worker totals', async () => {
    const wrapper = await setup();
    const [first, second, third] = rows(wrapper);

    expect(first.find('a').attributes('href')).toBe('/company-requests/request-1');
    expect(first.find('.request-flag--asap').exists()).toBe(true);
    expect(second.find('.request-flag--dh').exists()).toBe(true);
    expect(first.text().replace(/\s+/g, ' ')).toContain('1 King St - Door 3');
    expect(second.text()).not.toContain(' - ');
    expect(first.find('.shift').text()).toBe('Mon-Fri 08:00-16:00');
    expect(wrapper.findComponent({ name: 'AgencyShift' }).props('fetchShift')).toBe(getRequestShift);
    expect(first.find('.dot-status').classes()).toContain('status-inprogress');
    expect(second.find('.dot-status').classes()).toContain('status-filled');
    expect(second.find('.request-check').exists()).toBe(true);
    expect(third.find('.dot-status').classes()).toContain('status-open');
    expect(wrapper.find('thead').text()).toContain('(5 / 8)');
  });

  it('shows zero totals without requests', async () => {
    const wrapper = await setup([]);

    expect(wrapper.find('thead').text()).toContain('(0 / 0)');
  });

  it('opens a request from its cells and its workers from the workers cell', async () => {
    const wrapper = await setup();
    const cells = rows(wrapper)[0].findAll('td');

    await cells[1].trigger('click');
    expect(push).toHaveBeenLastCalledWith('/company-requests/request-1');

    await cells[4].trigger('click');
    expect(push).toHaveBeenLastCalledWith({ path: '/company-requests/request-1', query: { tab: 'Workers' } });

    push.mockClear();
    await cells[3].trigger('click');
    expect(push).not.toHaveBeenCalled();
  });

  it('filters from the column search on enter', async () => {
    const wrapper = await setup();
    const searchInputs = wrapper.findAll('thead input');
    await searchInputs[1].setValue('pack');
    await searchInputs[1].trigger('keyup', { key: 'Enter' });
    await flushPromises();

    expect(lastFilter()).toMatchObject({ jobTitle: 'pack', pageIndex: 1 });
  });

  describe('on touch screens', () => {
    beforeEach(() => {
      touch.value = true;
    });

    it('shows request cards that open the request or its workers', async () => {
      const wrapper = await setup();
      const cards = wrapper.findAll('.rcard');

      expect(cards).toHaveLength(3);
      expect(cards[0].text()).toContain('Open');
      expect(cards[0].text()).toContain('2 / 4');
      expect(cards[1].text()).toContain('Filled');
      expect(cards[1].find('.request-flag--dh').exists()).toBe(true);

      await cards[0].trigger('click');
      expect(push).toHaveBeenLastCalledWith('/company-requests/request-1');

      await cards[1].findAll('.rcard__row')[2].trigger('click');
      expect(push).toHaveBeenLastCalledWith({ path: '/company-requests/request-2', query: { tab: 'Workers' } });

      push.mockClear();
      await cards[1].findAll('.rcard__row')[1].trigger('click');
      expect(push).not.toHaveBeenCalled();
    });

    it('searches by position and counts and clears the filters', async () => {
      const wrapper = await setup();
      const search = wrapper.find('.sigook-grid-actions input');
      await search.setValue('fork');
      await search.trigger('keyup.enter');
      await flushPromises();
      expect(lastFilter()).toMatchObject({ jobTitle: 'fork', pageIndex: 1 });

      const panel = wrapper.findComponent({ name: 'MobileFiltersPanel' });
      await panel.findAll('input')[2].setValue('King');
      expect(panel.props('activeCount')).toBe(2);

      panel.vm.$emit('apply');
      await flushPromises();
      expect(lastFilter()).toMatchObject({ jobTitle: 'fork', location: 'King' });

      panel.vm.$emit('clear');
      await flushPromises();
      expect(lastFilter()).toMatchObject({ numberId: undefined, jobTitle: undefined, location: undefined });
      expect(panel.props('activeCount')).toBe(0);
    });
  });
});
