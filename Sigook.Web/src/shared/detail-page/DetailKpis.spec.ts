import { beforeEach, describe, expect, it, vi } from 'vitest';
import { computed, ref } from 'vue';
import DetailKpis from '@/shared/detail-page/DetailKpis.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { DetailKpi } from '@/shared/detail-page/types';

const mobile = ref(false);
const width = ref(0);

vi.mock('@/shared/composables/useBreakpoint', () => ({
  useBreakpoint: () => ({ isMobile: computed(() => mobile.value) }),
}));
vi.mock('@/shared/composables/useElementSize', () => ({
  useElementSize: () => ({ el: ref<HTMLElement | null>(null), width, height: ref(0) }),
}));

function kpisOf(count: number): DetailKpi[] {
  return Array.from({ length: count }, (_, i) => ({ key: `k${i}`, label: `Label ${i}`, value: `${i}` }));
}

function setup(kpis: DetailKpi[], slots: Record<string, string> = {}) {
  return mountWithPlugins(DetailKpis, { props: { kpis }, slots });
}

function columns(wrapper: ReturnType<typeof setup>) {
  return (wrapper.find('dl').element as HTMLElement).style.getPropertyValue('--detail-kpi-columns');
}

beforeEach(() => {
  mobile.value = false;
  width.value = 0;
});

describe('DetailKpis', () => {
  it('renders label, value and hint', () => {
    const wrapper = setup([{ key: 'hours', label: 'Hours', value: '40', hint: 'this week' }]);

    expect(wrapper.find('dt').text()).toBe('Hours');
    expect(wrapper.find('.detail-kpi-value').text()).toBe('40');
    expect(wrapper.find('.detail-kpi-hint').text()).toBe('this week');
    expect(wrapper.find('progress').exists()).toBe(false);
    expect(wrapper.find('.detail-kpi-extra').exists()).toBe(false);
  });

  it('renders a progress bar', () => {
    const progress = setup([{ key: 'fill', label: 'Filled', value: '3/4', progress: 75 }]).find('progress');
    expect(progress.attributes('value')).toBe('75');
    expect(progress.text()).toBe('75%');
  });

  it('lets slots override the value and add extras', () => {
    const wrapper = setup([{ key: 'rate', label: 'Rate', value: '20' }], {
      'kpi-rate': '<template #kpi-rate="{ kpi }"><b class="custom">${{ kpi.value }}</b></template>',
      'kpi-rate-extra': '<i class="extra">CAD</i>',
    });

    expect(wrapper.find('.custom').text()).toBe('$20');
    expect(wrapper.find('.detail-kpi-extra .extra').text()).toBe('CAD');
  });

  it('fits all kpis on one row when wide enough', () => {
    width.value = 4 * 190;
    const wrapper = setup(kpisOf(4));

    expect(columns(wrapper)).toBe('4');
    expect(wrapper.findAll('.is-wrapped')).toHaveLength(0);
  });

  it('wraps into two rows when narrow', () => {
    width.value = 5 * 190 - 1;
    const wrapper = setup(kpisOf(5));
    const items = wrapper.findAll('.detail-kpi');

    expect(columns(wrapper)).toBe('3');
    expect(items.map((i) => i.classes().includes('is-row-start'))).toEqual([true, false, false, true, false]);
    expect(items.map((i) => i.classes().includes('is-wrapped'))).toEqual([false, false, false, true, true]);
  });

  it('uses two columns on mobile', () => {
    mobile.value = true;
    width.value = 5000;
    expect(columns(setup(kpisOf(5)))).toBe('2');
  });
});
