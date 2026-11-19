import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import { nextTick } from 'vue';
import CreatePayStub from '@/modules/agency/accounting/paystubs/pages/CreatePayStub.vue';
import { createAgencyPayStub } from '@/modules/agency/accounting/paystubs/api';
import { getAgencyWorkersDropdown } from '@/modules/agency/recruiting/workers/api';
import { showAlertError, showAlertSuccess } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { AgencyWorkerDropdownItem } from '@/modules/agency/recruiting/workers/types';

vi.mock('@/modules/agency/accounting/paystubs/api', () => ({ createAgencyPayStub: vi.fn() }));
vi.mock('@/modules/agency/recruiting/workers/api', () => ({ getAgencyWorkersDropdown: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn(), showAlertSuccess: vi.fn() }));

const john: AgencyWorkerDropdownItem = { id: 'u-1', workerProfileId: 'wp-1', fullName: 'John Smith', email: 'john@test', approvedToWork: true };

function setup() {
  return mountWithPlugins(CreatePayStub);
}

type Wrapper = ReturnType<typeof setup>;

async function pickWorker(wrapper: Wrapper) {
  const autocomplete = wrapper.findComponent({ name: 'BAutocomplete' });
  autocomplete.vm.$emit('update:modelValue', 'John Smith');
  autocomplete.vm.$emit('select', john);
  await nextTick();
}

function header(wrapper: Wrapper, title: string) {
  const found = wrapper.findAll('.expandable-section-header').find((h) => h.text().includes(title));
  if (!found) throw new Error(`${title} not found`);
  return found;
}

function itemRows(wrapper: Wrapper) {
  return wrapper.findAll('.expandable-section-list > .columns').filter((r) => r.findComponent({ name: 'BSelect' }).exists());
}

async function addItem(wrapper: Wrapper, type: number, quantity: number, price: number, description = '') {
  await header(wrapper, 'Add Item').trigger('click');
  const row = itemRows(wrapper).at(-1);
  if (!row) throw new Error('row not found');
  row.findComponent({ name: 'BSelect' }).vm.$emit('update:modelValue', type);
  await nextTick();
  if (description) await row.findAll('input')[0].setValue(description);
  const [qty, unitPrice] = row.findAllComponents({ name: 'BNumberinput' });
  qty.vm.$emit('update:modelValue', quantity);
  await nextTick();
  unitPrice.vm.$emit('update:modelValue', price);
  await nextTick();
  return row;
}

function optionsOf(row: ReturnType<Wrapper['find']>) {
  return row.findAll('option').map((o) => o.text());
}

async function submit(wrapper: Wrapper) {
  await wrapper.findAll('button').find((b) => b.text() === 'Create PayStub')?.trigger('click');
}

beforeEach(() => {
  vi.useFakeTimers({ now: new Date('2026-11-19T10:00:00'), toFake: ['Date'] });
});

afterEach(() => {
  vi.useRealTimers();
});

describe('CreatePayStub', () => {
  it('defaults the dates to the current Sunday-Saturday week', () => {
    const wrapper = setup();

    expect(wrapper.findComponent({ name: 'BDatepicker' }).props('modelValue')).toEqual([new Date(2026, 10, 15), new Date(2026, 10, 21)]);
  });

  it('creates the pay stub with items, discount and computed totals', async () => {
    vi.mocked(createAgencyPayStub).mockResolvedValue();
    const wrapper = setup();
    const push = vi.spyOn(wrapper.vm.$router, 'push');
    await pickWorker(wrapper);
    await wrapper.find('input[name="position"]').setValue('Forklift');
    const regular = await addItem(wrapper, 0, 40, 18.5);
    await addItem(wrapper, 9, 1, 50, 'Referral bonus');
    await header(wrapper, 'Add Discount').trigger('click');
    await wrapper.find('input[name="discountDescription"]').setValue('Boots');
    wrapper.findAllComponents({ name: 'BNumberinput' }).at(-1)?.vm.$emit('update:modelValue', 25);
    await nextTick();

    expect((regular.findAll('input').at(-1)?.element as HTMLInputElement).value).toBe('740');
    await submit(wrapper);

    await vi.waitFor(() => { expect(createAgencyPayStub).toHaveBeenCalledOnce(); });
    expect(createAgencyPayStub).toHaveBeenCalledWith({
      workerProfileId: 'wp-1',
      position: 'Forklift',
      workBegins: '2026-11-15',
      workEnd: '2026-11-21',
      payVacations: false,
      items: [
        { type: 0, quantity: 40, unitPrice: 18.5, description: null },
        { type: 9, quantity: 1, unitPrice: 50, description: 'Referral bonus' },
      ],
      otherDeductions: 25,
      otherDeductionsDescription: 'Boots',
    });
    await flushPromises();
    expect(showAlertSuccess).toHaveBeenCalledWith('PayStub created successfully');
    expect(push).toHaveBeenCalledWith('/accounting/paystubs');
  });

  it('uses the picked date range', async () => {
    vi.mocked(createAgencyPayStub).mockResolvedValue();
    const wrapper = setup();
    await pickWorker(wrapper);
    await wrapper.find('input[name="position"]').setValue('Forklift');
    wrapper.findComponent({ name: 'BDatepicker' }).vm.$emit('update:modelValue', [new Date(2026, 10, 1), new Date(2026, 10, 7)]);
    await nextTick();
    await submit(wrapper);

    await vi.waitFor(() => { expect(createAgencyPayStub).toHaveBeenCalledOnce(); });
    expect(vi.mocked(createAgencyPayStub).mock.calls[0][0]).toMatchObject({ workBegins: '2026-11-01', workEnd: '2026-11-07', items: [], otherDeductions: 0, otherDeductionsDescription: null });
  });

  it('requires worker, position and dates', async () => {
    const wrapper = setup();
    wrapper.findComponent({ name: 'BDatepicker' }).vm.$emit('update:modelValue', []);
    await nextTick();
    await submit(wrapper);

    await vi.waitFor(() => { expect(wrapper.text()).toContain('Worker is required'); });
    expect(wrapper.text()).toContain('Position is required');
    expect(wrapper.text()).toContain('Dates are required');
    expect(createAgencyPayStub).not.toHaveBeenCalled();
  });

  it('flags incomplete items and discount', async () => {
    const wrapper = setup();
    await pickWorker(wrapper);
    await wrapper.find('input[name="position"]').setValue('Forklift');
    await header(wrapper, 'Add Item').trigger('click');
    await header(wrapper, 'Add Discount').trigger('click');
    await submit(wrapper);

    await vi.waitFor(() => { expect(wrapper.findAll('.help.is-danger').filter((h) => h.text() === 'Required')).toHaveLength(4); });
    expect(createAgencyPayStub).not.toHaveBeenCalled();
  });

  it('offers each single-use type once while bonus and reimbursement repeat', async () => {
    const wrapper = setup();
    await addItem(wrapper, 0, 1, 1);
    await addItem(wrapper, 9, 1, 1);
    await header(wrapper, 'Add Item').trigger('click');

    const options = optionsOf(itemRows(wrapper)[2]);
    expect(options).not.toContain('Regular Hours');
    expect(options).toEqual(expect.arrayContaining(['Overtime Hours', 'Bonus/Others', 'Reimbursement']));
    expect(optionsOf(itemRows(wrapper)[0])).toContain('Regular Hours');
  });

  it('removes vacation items and marks the type auto-calculated when vacations are paid', async () => {
    const wrapper = setup();
    await addItem(wrapper, 8, 1, 100);
    await addItem(wrapper, 0, 1, 1);
    wrapper.findComponent({ name: 'BSwitch' }).vm.$emit('update:modelValue', true);
    await nextTick();

    expect(itemRows(wrapper)).toHaveLength(1);
    expect(optionsOf(itemRows(wrapper)[0])).toContain('Vacations (auto-calculated)');

    await header(wrapper, 'Add Item').trigger('click');
    const select = itemRows(wrapper)[1].findComponent({ name: 'BSelect' });
    select.vm.$emit('update:modelValue', 8);
    await nextTick();
    expect(select.props('modelValue')).toBeNull();
  });

  it('removes items and the discount', async () => {
    vi.mocked(createAgencyPayStub).mockResolvedValue();
    const wrapper = setup();
    await pickWorker(wrapper);
    await wrapper.find('input[name="position"]').setValue('Forklift');
    const row = await addItem(wrapper, 0, 1, 1);
    await row.find('button').trigger('click');
    await header(wrapper, 'Add Discount').trigger('click');
    await wrapper.find('.expandable-section-list button').trigger('click');

    expect(wrapper.find('.expandable-section-list').exists()).toBe(false);
    await submit(wrapper);
    await vi.waitFor(() => { expect(createAgencyPayStub).toHaveBeenCalledOnce(); });
    expect(vi.mocked(createAgencyPayStub).mock.calls[0][0]).toMatchObject({ items: [], otherDeductions: 0 });
  });

  it('does not submit when the worker was typed but not selected', async () => {
    const wrapper = setup();
    await pickWorker(wrapper);
    wrapper.findComponent({ name: 'BAutocomplete' }).vm.$emit('select', null);
    await wrapper.find('input[name="position"]').setValue('Forklift');
    await submit(wrapper);
    await flushPromises();

    await vi.waitFor(() => { expect(wrapper.text()).toContain('Please select a worker from the list'); });
    expect(createAgencyPayStub).not.toHaveBeenCalled();
  });

  it('shows the error when creation fails', async () => {
    const error = new Error('duplicate');
    vi.mocked(createAgencyPayStub).mockRejectedValue(error);
    const wrapper = setup();
    await pickWorker(wrapper);
    await wrapper.find('input[name="position"]').setValue('Forklift');
    await submit(wrapper);

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
  });

  it('searches workers from three characters', async () => {
    vi.useFakeTimers({ now: new Date('2026-11-19T10:00:00'), toFake: ['Date', 'setTimeout', 'clearTimeout'] });
    vi.mocked(getAgencyWorkersDropdown).mockResolvedValue([john]);
    const wrapper = setup();
    wrapper.findComponent({ name: 'BAutocomplete' }).vm.$emit('typing', 'Joh');
    vi.advanceTimersByTime(300);
    await flushPromises();

    expect(getAgencyWorkersDropdown).toHaveBeenCalledWith({ searchTerm: 'Joh' });
    expect(wrapper.findComponent({ name: 'BAutocomplete' }).props('data')).toEqual([john]);
  });
});
