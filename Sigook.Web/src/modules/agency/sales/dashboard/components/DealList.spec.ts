import { describe, expect, it } from 'vitest';
import DealList from '@/modules/agency/sales/dashboard/components/DealList.vue';
import { DealStatus } from '@/modules/agency/sales/clients/types';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createDeal } from '@/testing/salesFixtures';
import type { Deal } from '@/modules/agency/sales/clients/types';

const plain = createDeal();
const withDocument = createDeal({
  id: 'deal-2',
  title: 'Night crew',
  status: DealStatus.Accepted,
  value: 1_250_000,
  documentPath: 'https://files/proposal.pdf',
  documentName: 'proposal.pdf',
});

function setup(items: Deal[] = [plain, withDocument], loading = false) {
  return mountWithPlugins(DealList, { props: { items, loading } });
}

describe('DealList', () => {
  it('lists deals with client, stage and compact value', () => {
    const rows = setup().findAll('.sd-deal-row');

    expect(rows.map((r) => r.find('.sd-row__title').text())).toEqual(['Warehouse staffing', 'Night crew']);
    expect(rows[0].find('.sd-deal-client').text()).toBe('Acme Logistics');
    expect(rows.map((r) => r.find('.sd-deal-stage').text())).toEqual(['Sent', 'Accepted']);
    expect(rows.map((r) => r.find('.sd-deal-value').text())).toEqual(['$25K', '$1.3M']);
  });

  it('links the document without editing the deal', async () => {
    const wrapper = setup();
    const link = wrapper.findAll('.sd-deal-row')[1].find('a.sd-deal-doc');

    expect(link.attributes('href')).toBe('https://files/proposal.pdf');
    expect(link.attributes('title')).toBe('proposal.pdf');
    expect(wrapper.findAll('.sd-deal-row')[0].find('a.sd-deal-doc').exists()).toBe(false);
    await link.trigger('click');
    expect(wrapper.emitted('edit')).toBeUndefined();
  });

  it('titles a document without name generically', () => {
    const wrapper = setup([{ ...withDocument, documentName: null }]);
    expect(wrapper.find('a.sd-deal-doc').attributes('title')).toBe('Open document');
  });

  it.each([
    ['click', {}],
    ['keydown', { key: 'Enter' }],
  ])('edits the deal on %s', async (event, options) => {
    const wrapper = setup();
    await wrapper.findAll('.sd-deal-row')[0].trigger(event, options);

    expect(wrapper.emitted('edit')).toEqual([[plain]]);
  });

  it('shows the spinner when loading with no deals', () => {
    expect(setup([], true).find('.sd-list__loading').exists()).toBe(true);
  });
});
