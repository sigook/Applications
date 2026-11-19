import { describe, expect, it } from 'vitest';
import NumbersGrid from '@/modules/landing/components/About/NumbersGrid.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('NumbersGrid', () => {
  it('renders a card per stat with its tone and delay', () => {
    const wrapper = mountWithPlugins(NumbersGrid, {
      props: { stats: [{ value: '10', label: 'Years', tone: 'blue' }, { value: '700', label: 'Clients', tone: 'red' }] },
    });
    const stats = wrapper.findAll('.numbers-grid__stat');

    expect(stats.map((s) => s.find('.numbers-grid__num').text())).toEqual(['+10', '+700']);
    expect(stats.map((s) => s.find('.numbers-grid__lbl').text())).toEqual(['Years', 'Clients']);
    expect(stats[1].classes()).toContain('secondary-card--red');
    expect(stats[1].attributes('style')).toContain('transition-delay: 140ms');
  });
});
