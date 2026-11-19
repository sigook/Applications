import { describe, expect, it } from 'vitest';
import InteractionList from '@/modules/agency/sales/dashboard/components/InteractionList.vue';
import { InteractionType } from '@/modules/agency/sales/clients/types';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createInteraction } from '@/testing/salesFixtures';
import type { CompanyInteraction } from '@/modules/agency/sales/clients/types';

const call = createInteraction();
const linkedIn = createInteraction({ id: 'int-2', description: 'Connected with HR', interactionType: InteractionType.LinkedIn, createdAt: '2026-11-19T08:30:00' });

function setup(items: CompanyInteraction[] = [call, linkedIn]) {
  return mountWithPlugins(InteractionList, { props: { items, asOf: '2026-11-19T09:00:00' } });
}

describe('InteractionList', () => {
  it('lists interactions with type icon, meta and elapsed time', () => {
    const rows = setup().findAll('.sd-interaction-row');

    expect(rows.map((r) => r.find('.sd-row__title').text())).toEqual(['Called the plant manager', 'Connected with HR']);
    expect(rows.map((r) => r.find('.sd-row__meta').text())).toEqual(['Call · Acme Logistics', 'LinkedIn · Acme Logistics']);
    expect(rows.map((r) => r.find('.sd-interaction-time').text())).toEqual(['Now', '30m']);
    expect(rows[1].find('.sd-interaction-avatar i').classes()).toContain('mdi-linkedin');
  });

  it('shows the default empty text', () => {
    expect(setup([]).find('.sd-list__empty').text()).toBe('Nothing yet');
  });

  it.each([
    ['click', {}],
    ['keydown', { key: 'Enter' }],
  ])('edits the interaction on %s', async (event, options) => {
    const wrapper = setup();
    await wrapper.findAll('.sd-interaction-row')[1].trigger(event, options);

    expect(wrapper.emitted('edit')).toEqual([[linkedIn]]);
  });
});
