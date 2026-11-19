import { describe, expect, it } from 'vitest';
import MeterList from '@/modules/agency/sales/dashboard/components/MeterList.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { SalesMeter } from '@/modules/agency/sales/dashboard/types';

function setup(items: SalesMeter[]) {
  return mountWithPlugins(MeterList, { props: { title: 'This week', items } });
}

function widths(wrapper: ReturnType<typeof setup>) {
  return wrapper.findAll('.sd-meter__fill').map((f) => (f.element as HTMLElement).style.width);
}

describe('MeterList', () => {
  it('scales each meter against the largest count', () => {
    const wrapper = setup([
      { label: 'Call', count: 8, color: '#21b7ff' },
      { label: 'Email', count: 2, color: '#3eb800' },
      { label: 'SMS', count: 0, color: '#ff9932' },
    ]);

    expect(wrapper.find('.sd-meters__title').text()).toBe('This week');
    expect(widths(wrapper)).toEqual(['100%', '25%', '0%']);
    expect(wrapper.findAll('.sd-meter__count').map((c) => c.text())).toEqual(['8', '2', '0']);
    expect(wrapper.findAll('.sd-meter')[1].attributes('aria-label')).toBe('Email: 2');
    expect((wrapper.find('.sd-meter__fill').element as HTMLElement).style.backgroundColor).toBe('#21b7ff');
  });

  it('leaves every meter empty when all counts are zero', () => {
    const wrapper = setup([
      { label: 'Call', count: 0, color: '#21b7ff' },
      { label: 'Email', count: 0, color: '#3eb800' },
    ]);

    expect(widths(wrapper)).toEqual(['0%', '0%']);
  });

  it('renders nothing for no items', () => {
    expect(setup([]).findAll('.sd-meter')).toHaveLength(0);
  });
});
