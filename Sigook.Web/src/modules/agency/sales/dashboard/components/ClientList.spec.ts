import { describe, expect, it } from 'vitest';
import ClientList from '@/modules/agency/sales/dashboard/components/ClientList.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createRecentClient } from '@/testing/salesFixtures';
import type { SalesRecentClient } from '@/modules/agency/sales/dashboard/types';

const acme = createRecentClient();
const globex = createRecentClient({ id: 'company-2', fullName: 'globex corp inc', email: null, lastInteractionAt: '2026-11-17T09:00:00' });

function setup(items: SalesRecentClient[] = [acme, globex], loading = false) {
  return mountWithPlugins(ClientList, { props: { items, asOf: '2026-11-19T09:00:00', loading } });
}

describe('ClientList', () => {
  it('lists clients with initials, email and time since the last interaction', () => {
    const rows = setup().findAll('.sd-client-row');

    expect(rows.map((r) => r.find('.sd-client-avatar').text())).toEqual(['AL', 'GC']);
    expect(rows[0].find('.sd-row__title').text()).toBe('Acme Logistics');
    expect(rows[0].find('.sd-client-email').text()).toBe('ap@acme.test');
    expect(rows.map((r) => r.find('.sd-client-time').text())).toEqual(['2h', '2d']);
    expect(rows[0].attributes('aria-label')).toBe('View interactions for Acme Logistics');
  });

  it('shows the empty text', () => {
    expect(setup([]).find('.sd-list__empty').text()).toBe('No interactions yet');
  });

  it.each([
    ['click', {}],
    ['keydown', { key: 'Enter' }],
    ['keydown', { key: ' ' }],
  ])('selects the client on %s %o', async (event, options) => {
    const wrapper = setup();
    await wrapper.findAll('.sd-client-row')[1].trigger(event, options);

    expect(wrapper.emitted('select')).toEqual([[globex]]);
  });
});
