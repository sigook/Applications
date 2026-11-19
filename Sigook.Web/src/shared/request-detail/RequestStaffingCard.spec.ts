import { describe, expect, it } from 'vitest';
import RequestStaffingCard from '@/shared/request-detail/RequestStaffingCard.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { RequestStaffingItem } from '@/shared/request-detail/types';

const items: RequestStaffingItem[] = [
  { key: 'start', title: 'Starts tomorrow', detail: '2 spots open', variant: 'is-warning' },
  { key: 'applicants', title: '3 applicants', detail: 'Waiting review', variant: 'is-neutral', actionLabel: 'Review' },
  { key: 'other', title: 'Note', variant: 'is-neutral' },
];

function setup(list: RequestStaffingItem[] = items) {
  return mountWithPlugins(RequestStaffingCard, { props: { items: list } });
}

describe('RequestStaffingCard', () => {
  it('renders nothing without items', () => {
    expect(setup([]).find('.detail-card').exists()).toBe(false);
  });

  it('lists the items with their icon, variant and detail', () => {
    const rows = setup().findAll('.request-staffing-item');

    expect(rows).toHaveLength(3);
    expect(rows[0].classes()).toContain('is-warning');
    expect(rows[0].find('.request-staffing-title').text()).toBe('Starts tomorrow');
    expect(rows[0].find('.request-staffing-detail').text()).toBe('2 spots open');
    expect(rows[0].find('.mdi-clock-outline').exists()).toBe(true);
    expect(rows[1].find('.mdi-account-multiple-outline').exists()).toBe(true);
    expect(rows[2].find('.mdi-information-outline').exists()).toBe(true);
    expect(rows[2].find('.request-staffing-detail').exists()).toBe(false);
  });

  it('emits the key of the clicked action', async () => {
    const wrapper = setup();
    const actions = wrapper.findAll('.request-staffing-action');

    expect(actions.map((a) => a.text())).toEqual(['Review']);
    await actions[0].trigger('click');
    expect(wrapper.emitted('action')).toEqual([['applicants']]);
  });
});
