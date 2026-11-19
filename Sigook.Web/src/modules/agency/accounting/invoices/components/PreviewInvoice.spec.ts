import { afterEach, describe, expect, it, vi } from 'vitest';
import PreviewInvoice from '@/modules/agency/accounting/invoices/components/PreviewInvoice.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { InvoicePreviewModel } from '@/shared/types/invoice';

const preview: InvoicePreviewModel = {
  subTotal: 1250,
  tax: 162.5,
  taxName: 'HST 13%',
  total: 1412.5,
  items: [
    { description: 'General labour - Regular', quantity: 40, unitPrice: 25, total: 1000 },
    { description: 'General labour - Overtime', quantity: 6.5, unitPrice: 37.5, total: 243.75 },
  ],
  discounts: [{ description: 'Volume discount', quantity: 1, unitPrice: 50, total: 50 }],
};

function setup(overrides: Partial<InvoicePreviewModel> = {}) {
  return mountWithPlugins(PreviewInvoice, { props: { preview: { ...preview, ...overrides } } });
}

function lines(wrapper: ReturnType<typeof setup>, selector: string) {
  return wrapper.findAll(selector).map((row) => row.findAll('.column').map((c) => c.text()));
}

afterEach(() => {
  vi.useRealTimers();
});

describe('PreviewInvoice', () => {
  it('shows today in long format', () => {
    vi.useFakeTimers({ now: new Date('2026-11-19T10:00:00'), toFake: ['Date'] });
    expect(setup().find('.color-gray').text()).toBe('November 19, 2026');
  });

  it('lists items and discounts with quantities and prices', () => {
    const wrapper = setup();

    expect(lines(wrapper, '.columns.mb-2:not(.has-text-weight-semibold):not(.color-red)').slice(0, 2)).toEqual([
      ['General labour - Regular', '40.00', '$25.00', '$1,000.00'],
      ['General labour - Overtime', '6.50', '$37.50', '$243.75'],
    ]);
    expect(lines(wrapper, '.columns.color-red')).toEqual([['Volume discount', '1.00', '$50.00', '-$50.00']]);
  });

  it('shows subtotal, named tax and total', () => {
    const text = setup().text();

    expect(text).toContain('Subtotal:$1,250.00');
    expect(text).toContain('HST 13%:$162.50');
    expect(text).toContain('Total:$1,412.50');
  });

  it('falls back to Tax and hides empty sections', () => {
    const wrapper = setup({ taxName: undefined, items: [], discounts: [] });

    expect(wrapper.text()).toContain('Tax:$162.50');
    expect(wrapper.text()).not.toContain('Items & Services');
    expect(wrapper.text()).not.toContain('Discounts');
  });
});
