import { describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import { nextTick } from 'vue';
import CreateInvoice from '@/modules/agency/accounting/invoices/pages/CreateInvoice.vue';
import { createAgencyInvoice, previewAgencyInvoice } from '@/modules/agency/accounting/invoices/api';
import { getAgencyCompanyProfileWithRequests } from '@/modules/agency/recruiting/clients/api';
import { getAllAgencyRequests } from '@/modules/agency/recruiting/requests/api';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { CompanyProfileListItem } from '@/modules/agency/recruiting/clients/types';
import type { AgencyRequestListItem } from '@/modules/agency/recruiting/requests/types';
import type { InvoicePreviewModel } from '@/shared/types/invoice';

vi.mock('@/modules/agency/accounting/invoices/api', () => ({ createAgencyInvoice: vi.fn(), previewAgencyInvoice: vi.fn() }));
vi.mock('@/modules/agency/recruiting/clients/api', () => ({ getAgencyCompanyProfileWithRequests: vi.fn() }));
vi.mock('@/modules/agency/recruiting/requests/api', () => ({ getAllAgencyRequests: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

const acme = { id: 'company-1', fullName: 'Acme Logistics' } as CompanyProfileListItem;
const globex = { id: 'company-2', fullName: 'Globex' } as CompanyProfileListItem;
const forklift = { id: 'req-1', numberId: 101, jobTitle: 'Forklift' } as AgencyRequestListItem;
const packer = { id: 'req-2', numberId: 202, jobTitle: 'Packer' } as AgencyRequestListItem;

const preview: InvoicePreviewModel = {
  subTotal: 1000,
  tax: 130,
  taxName: 'HST',
  total: 1130,
  items: [{ description: 'Forklift - Regular', quantity: 40, unitPrice: 25, total: 1000 }],
  discounts: [],
};

async function setup() {
  vi.mocked(getAgencyCompanyProfileWithRequests).mockResolvedValue([acme, globex]);
  vi.mocked(getAllAgencyRequests).mockResolvedValue([forklift, packer]);
  const wrapper = mountWithPlugins(CreateInvoice);
  await flushPromises();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

async function pickCompany(wrapper: Wrapper, company: CompanyProfileListItem | null) {
  const autocomplete = wrapper.findComponent({ name: 'BAutocomplete' });
  autocomplete.vm.$emit('update:modelValue', company?.fullName ?? '');
  autocomplete.vm.$emit('select', company);
  await flushPromises();
}

function button(wrapper: Wrapper, text: string) {
  const found = wrapper.findAll('button').find((b) => b.text() === text);
  if (!found) throw new Error(`${text} not found`);
  return found;
}

async function addRow(wrapper: Wrapper, title: string, description: string, quantity: number, price: number) {
  const header = wrapper.findAll('.expandable-section-header').find((h) => h.text().includes(title));
  await header?.trigger('click');
  const container = header?.element.parentElement;
  const rows = wrapper.findAll('.expandable-section-list > .columns').filter((r) => container?.contains(r.element));
  const row = rows.at(-1);
  if (!row) throw new Error('row not found');
  await row.find('input').setValue(description);
  const [qty, unitPrice] = row.findAllComponents({ name: 'BNumberinput' });
  qty.vm.$emit('update:modelValue', quantity);
  await nextTick();
  unitPrice.vm.$emit('update:modelValue', price);
  await nextTick();
  return row;
}

async function previewInvoice(wrapper: Wrapper) {
  const calls = vi.mocked(previewAgencyInvoice).mock.calls.length;
  await button(wrapper, 'Preview Invoice').trigger('click');
  await vi.waitFor(() => { expect(previewAgencyInvoice).toHaveBeenCalledTimes(calls + 1); });
  await flushPromises();
}

async function submitInvalid(wrapper: Wrapper) {
  await button(wrapper, 'Preview Invoice').trigger('click');
  await flushPromises();
}

describe('CreateInvoice', () => {
  it('filters companies by name and loads open requests of the selected one', async () => {
    const wrapper = await setup();
    const autocomplete = wrapper.findComponent({ name: 'BAutocomplete' });
    autocomplete.vm.$emit('update:modelValue', 'glo');
    await nextTick();

    expect(autocomplete.props('data')).toEqual([globex]);
    expect(wrapper.findComponent({ name: 'BTaginput' }).props('disabled')).toBe(true);

    await pickCompany(wrapper, acme);

    expect(getAllAgencyRequests).toHaveBeenCalledExactlyOnceWith({ companyProfileId: 'company-1', statuses: [1, 3], pageSize: 1000, pageIndex: 0 });
    const taginput = wrapper.findComponent({ name: 'BTaginput' });
    expect(taginput.props('disabled')).toBe(false);
    expect(taginput.props('data')).toEqual([forklift, packer]);
  });

  it('filters requests by number or title and hides the selected ones', async () => {
    const wrapper = await setup();
    await pickCompany(wrapper, acme);
    const taginput = wrapper.findComponent({ name: 'BTaginput' });

    taginput.vm.$emit('typing', 'pack');
    await nextTick();
    expect(taginput.props('data')).toEqual([packer]);

    taginput.vm.$emit('typing', '10');
    await nextTick();
    expect(taginput.props('data')).toEqual([forklift]);

    taginput.vm.$emit('typing', '');
    taginput.vm.$emit('update:modelValue', [forklift]);
    await nextTick();
    expect(taginput.props('data')).toEqual([packer]);
  });

  it('requires a company before previewing', async () => {
    const wrapper = await setup();
    await submitInvalid(wrapper);

    await vi.waitFor(() => { expect(wrapper.text()).toContain('Company is required'); });
    expect(previewAgencyInvoice).not.toHaveBeenCalled();
  });

  it('rejects an invalid email', async () => {
    const wrapper = await setup();
    await pickCompany(wrapper, acme);
    await wrapper.find('input[name="email"]').setValue('nope');
    await submitInvalid(wrapper);

    await vi.waitFor(() => { expect(wrapper.text()).toContain('Invalid email'); });
    expect(previewAgencyInvoice).not.toHaveBeenCalled();
  });

  it('previews with requests, dates, email and computed item totals, then confirms', async () => {
    vi.mocked(previewAgencyInvoice).mockResolvedValue(preview);
    vi.mocked(createAgencyInvoice).mockResolvedValue();
    const wrapper = await setup();
    const push = vi.spyOn(wrapper.vm.$router, 'push');
    await pickCompany(wrapper, acme);
    wrapper.findComponent({ name: 'BTaginput' }).vm.$emit('update:modelValue', [forklift, packer]);
    await wrapper.find('input[name="email"]').setValue('ap@acme.test');
    const [datePicker, rangePicker] = wrapper.findAllComponents({ name: 'BDatepicker' });
    const invoiceDate = new Date(2026, 10, 15);
    datePicker.vm.$emit('update:modelValue', invoiceDate);
    rangePicker.vm.$emit('update:modelValue', [new Date(2026, 10, 1), new Date(2026, 10, 14)]);
    await nextTick();
    const itemRow = await addRow(wrapper, 'Add Additional Items', 'Safety boots', 2, 45.5);
    await addRow(wrapper, 'Add Discounts', 'Loyalty', 1, 20);

    expect(itemRow.findAll('input').map((i) => (i.element as HTMLInputElement).value).at(-1)).toBe('91');

    await previewInvoice(wrapper);

    expect(previewAgencyInvoice).toHaveBeenCalledExactlyOnceWith({
      companyProfileId: 'company-1',
      requestIds: ['req-1', 'req-2'],
      directHiring: false,
      email: 'ap@acme.test',
      invoiceDate: invoiceDate.toISOString(),
      from: '2026-11-01',
      to: '2026-11-14',
      additionalItems: [{ description: 'Safety boots', quantity: 2, unitPrice: 45.5, total: 91 }],
      discounts: [{ description: 'Loyalty', quantity: 1, unitPrice: 20, total: 20 }],
    });
    expect(wrapper.text()).toContain('Forklift - Regular');
    expect(wrapper.text()).toContain('Total:$1,130.00');

    await button(wrapper, 'Confirm').trigger('click');
    await flushPromises();

    expect(createAgencyInvoice).toHaveBeenCalledExactlyOnceWith(vi.mocked(previewAgencyInvoice).mock.calls[0][0]);
    expect(push).toHaveBeenCalledWith('/accounting/invoices');
  });

  it('flags incomplete items and discounts', async () => {
    const wrapper = await setup();
    await pickCompany(wrapper, acme);
    await wrapper.findAll('.expandable-section-header')[0].trigger('click');
    await wrapper.findAll('.expandable-section-header')[1].trigger('click');
    await submitInvalid(wrapper);

    await vi.waitFor(() => { expect(wrapper.findAll('.help.is-danger').filter((h) => h.text() === 'Required')).toHaveLength(6); });
    expect(previewAgencyInvoice).not.toHaveBeenCalled();
  });

  it('removes items and discounts', async () => {
    vi.mocked(previewAgencyInvoice).mockResolvedValue(preview);
    const wrapper = await setup();
    await pickCompany(wrapper, acme);
    const item = await addRow(wrapper, 'Add Additional Items', 'Boots', 1, 10);
    const discount = await addRow(wrapper, 'Add Discounts', 'Promo', 1, 5);
    await item.find('button').trigger('click');
    await discount.find('button').trigger('click');

    expect(wrapper.find('.expandable-section-list').exists()).toBe(false);
    await previewInvoice(wrapper);
    expect(vi.mocked(previewAgencyInvoice).mock.calls[0][0]).toMatchObject({ additionalItems: [], discounts: [] });
  });

  it('drops the preview when an item or discount is removed so confirm never sends stale lines', async () => {
    vi.mocked(previewAgencyInvoice).mockResolvedValue(preview);
    vi.mocked(createAgencyInvoice).mockResolvedValue();
    const wrapper = await setup();
    await pickCompany(wrapper, acme);
    const item = await addRow(wrapper, 'Add Additional Items', 'Boots', 1, 10);
    const discount = await addRow(wrapper, 'Add Discounts', 'Promo', 1, 5);
    await previewInvoice(wrapper);
    expect(wrapper.text()).toContain('Forklift - Regular');

    await item.find('button').trigger('click');
    expect(wrapper.findAll('button').some((b) => b.text() === 'Confirm')).toBe(false);

    await previewInvoice(wrapper);
    await discount.find('button').trigger('click');
    expect(wrapper.findAll('button').some((b) => b.text() === 'Confirm')).toBe(false);

    await previewInvoice(wrapper);
    await button(wrapper, 'Confirm').trigger('click');
    await flushPromises();

    expect(createAgencyInvoice).toHaveBeenCalledExactlyOnceWith(expect.objectContaining({ additionalItems: [], discounts: [] }));
  });

  it('drops the preview when an item is edited after previewing', async () => {
    vi.mocked(previewAgencyInvoice).mockResolvedValue(preview);
    const wrapper = await setup();
    await pickCompany(wrapper, acme);
    const item = await addRow(wrapper, 'Add Additional Items', 'Boots', 1, 10);
    await previewInvoice(wrapper);

    await item.find('input').setValue('Gloves');

    expect(wrapper.findAll('button').some((b) => b.text() === 'Confirm')).toBe(false);
  });

  it('asks for a tax percentage instead of requests on direct hiring', async () => {
    vi.mocked(previewAgencyInvoice).mockResolvedValue(preview);
    const wrapper = await setup();
    await pickCompany(wrapper, acme);
    wrapper.findComponent({ name: 'BTaginput' }).vm.$emit('update:modelValue', [forklift]);
    wrapper.findComponent({ name: 'BSwitch' }).vm.$emit('update:modelValue', true);
    await flushPromises();

    expect(wrapper.findComponent({ name: 'BTaginput' }).exists()).toBe(false);
    const tax = wrapper.findAllComponents({ name: 'BNumberinput' })[0];
    tax.vm.$emit('update:modelValue', 13);
    await nextTick();
    await previewInvoice(wrapper);

    expect(vi.mocked(previewAgencyInvoice).mock.calls[0][0]).toMatchObject({ directHiring: true, taxPercentage: 13, requestIds: [] });

    wrapper.findComponent({ name: 'BSwitch' }).vm.$emit('update:modelValue', false);
    await flushPromises();
    await previewInvoice(wrapper);
    expect(vi.mocked(previewAgencyInvoice).mock.calls[1][0]).toMatchObject({ directHiring: false, taxPercentage: undefined });
  });

  it('resets the invoice when the company is cleared', async () => {
    const wrapper = await setup();
    await pickCompany(wrapper, acme);
    await pickCompany(wrapper, null);

    expect(wrapper.findComponent({ name: 'BTaginput' }).props('disabled')).toBe(true);
    expect(getAllAgencyRequests).toHaveBeenCalledOnce();
  });

  it('closes the preview on cancel', async () => {
    vi.mocked(previewAgencyInvoice).mockResolvedValue(preview);
    const scrollTo = vi.fn();
    vi.stubGlobal('scrollTo', scrollTo);
    const wrapper = await setup();
    await pickCompany(wrapper, acme);
    await previewInvoice(wrapper);
    await button(wrapper, 'Cancel').trigger('click');

    expect(wrapper.text()).not.toContain('PREVIEW INVOICE');
    expect(scrollTo).toHaveBeenCalledWith({ top: 0, behavior: 'smooth' });
  });

  it('shows errors from preview and creation', async () => {
    const previewError = new Error('no timesheets');
    vi.mocked(previewAgencyInvoice).mockRejectedValueOnce(previewError).mockResolvedValueOnce(preview);
    const createError = new Error('duplicate');
    vi.mocked(createAgencyInvoice).mockRejectedValue(createError);
    const wrapper = await setup();
    await pickCompany(wrapper, acme);
    await previewInvoice(wrapper);

    expect(showAlertError).toHaveBeenCalledWith(previewError);
    expect(wrapper.text()).not.toContain('PREVIEW INVOICE');

    await previewInvoice(wrapper);
    await button(wrapper, 'Confirm').trigger('click');
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(createError);
  });
});
