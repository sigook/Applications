import { describe, expect, it, vi } from 'vitest';
import Invoices from '@/modules/company/invoices/pages/Invoices.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/modules/company/invoices/components/CompanyInvoices.vue', () => ({
  default: { name: 'CompanyInvoices', template: '<div class="company-invoices" />' },
}));

describe('Invoices', () => {
  it('renders the company invoices grid', () => {
    const wrapper = mountWithPlugins(Invoices);

    expect(wrapper.find('.company-invoices').exists()).toBe(true);
  });
});
