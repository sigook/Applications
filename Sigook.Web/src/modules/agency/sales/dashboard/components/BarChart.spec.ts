import { beforeEach, describe, expect, it, vi } from 'vitest';
import { nextTick, ref } from 'vue';
import BarChart from '@/modules/agency/sales/dashboard/components/BarChart.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { SalesBarPoint } from '@/modules/agency/sales/dashboard/types';

const width = ref(0);
const height = ref(0);

vi.mock('@/shared/composables/useElementSize', () => ({
  useElementSize: () => ({ el: ref(null), width, height }),
}));

const points: SalesBarPoint[] = [
  { key: '0', label: 'Sent', value: 4, color: '#21b7ff', caption: '$12K' },
  { key: '1', label: 'Accepted', value: 2, color: '#3eb800' },
  { key: '2', label: 'Closed', value: 0, color: '#8a94a6' },
];

function setup(props: { points?: SalesBarPoint[]; formatValue?: (value: number) => string } = {}) {
  return mountWithPlugins(BarChart, { props: { title: 'Deals by status', points, ...props } });
}

function bars(wrapper: ReturnType<typeof setup>) {
  return wrapper.findAll('g');
}

function barHeight(path: string): number {
  const numbers = path.match(/-?\d+(\.\d+)?/g)?.map(Number) ?? [];
  return numbers[1] - numbers[5];
}

beforeEach(() => {
  width.value = 600;
  height.value = 240;
});

describe('BarChart', () => {
  it('describes the data for screen readers', () => {
    expect(setup().attributes('aria-label')).toBe('Deals by status: Sent 4, Accepted 2, Closed 0');
    expect(setup({ points: [] }).attributes('aria-label')).toBe('Deals by status: no data');
  });

  it('draws one bar per point with amount, label, color and tooltip', () => {
    const wrapper = setup();

    expect(bars(wrapper)).toHaveLength(3);
    expect(bars(wrapper).map((b) => b.find('.sd-bar-amount').text())).toEqual(['4', '2', '0']);
    expect(bars(wrapper).map((b) => b.find('.sd-bar-label').text())).toEqual(['Sent', 'Accepted', 'Closed']);
    expect(bars(wrapper).map((b) => b.find('title').text())).toEqual(['Sent: 4 · $12K', 'Accepted: 2', 'Closed: 0']);
    expect((bars(wrapper)[1].find('path').element as SVGPathElement).style.fill).toBe('#3eb800');
  });

  it('scales bar heights to the highest value', () => {
    const paths = bars(setup()).map((b) => b.find('path').attributes('d') ?? '');
    const [sent, accepted, closed] = paths.map(barHeight);

    expect(sent).toBeGreaterThan(0);
    expect(accepted).toBeCloseTo(sent / 2, 5);
    expect(closed).toBe(0);
  });

  it('formats amounts with the given formatter', () => {
    const wrapper = setup({ formatValue: (value) => `${value} deals` });

    expect(bars(wrapper)[0].find('.sd-bar-amount').text()).toBe('4 deals');
    expect(wrapper.attributes('aria-label')).toContain('Sent 4 deals');
  });

  it('draws nothing before the element is measured', () => {
    width.value = 0;
    expect(setup().find('svg').exists()).toBe(false);
  });

  it('draws no bars when the height leaves no room', () => {
    height.value = 50;
    expect(bars(setup())).toHaveLength(0);
  });

  it('wraps two-word labels that do not fit and rotates when still too wide', async () => {
    const wrapper = setup({ points: [
      { key: '0', label: 'Under Review', value: 1, color: '#ffb020' },
      { key: '1', label: 'To Send', value: 1, color: '#9ad6ff' },
    ] });
    width.value = 150;
    await nextTick();

    expect(bars(wrapper)[0].findAll('.sd-bar-label').map((t) => t.text())).toEqual(['Under', 'Review']);
    expect(bars(wrapper)[0].find('.sd-bar-label').attributes('text-anchor')).toBe('middle');

    width.value = 70;
    await nextTick();

    const label = bars(wrapper)[0].find('.sd-bar-label');
    expect(bars(wrapper)[0].findAll('.sd-bar-label').map((t) => t.text())).toEqual(['Under Review']);
    expect(label.attributes('text-anchor')).toBe('end');
    expect(label.attributes('transform')).toMatch(/^rotate\(-45, /);
  });

  it('keeps all bars at the baseline when every value is zero', () => {
    const wrapper = setup({ points: points.map((p) => ({ ...p, value: 0 })) });

    expect(bars(wrapper).map((b) => barHeight(b.find('path').attributes('d') ?? ''))).toEqual([0, 0, 0]);
  });
});
