import { afterEach, describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import RequestGroupFilters from '@/modules/agency/shared/request-groups/RequestGroupFilters.vue';
import { getAgencyCompaniesList } from '@/modules/agency/recruiting/clients/api';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { RequestGroupFilter } from '@/modules/agency/shared/request-groups/types';

vi.mock('@/modules/agency/recruiting/clients/api', () => ({ getAgencyCompaniesList: vi.fn() }));

const statusOptions = [
  { id: 1, value: 'Pending' },
  { id: 2, value: 'Booked' },
];

function setup(filter: RequestGroupFilter = { pageIndex: 3 }) {
  const wrapper = mountWithPlugins(RequestGroupFilters, {
    props: {
      filter,
      statusOptions,
      'onUpdate:filter': (value: RequestGroupFilter) => { void wrapper.setProps({ filter: value }); },
    },
  });
  return wrapper;
}

function lastFilter(wrapper: ReturnType<typeof setup>) {
  return wrapper.emitted<[RequestGroupFilter]>('update:filter')?.at(-1)?.[0];
}

function input(wrapper: ReturnType<typeof setup>, placeholder: string) {
  return wrapper.find(`input[placeholder="${placeholder}"]`);
}

afterEach(() => {
  vi.useRealTimers();
});

describe('RequestGroupFilters', () => {
  it('patches the text filters without searching', async () => {
    const wrapper = setup();
    await input(wrapper, 'Search name or email').setValue('john');
    await input(wrapper, 'Position').setValue('Driver');
    expect(lastFilter(wrapper)).toEqual({ pageIndex: 3, name: 'john', jobTitle: 'Driver' });
    expect(wrapper.emitted('search')).toBeUndefined();
  });

  it('keeps the request number numeric or empty', async () => {
    const wrapper = setup();
    await input(wrapper, 'Request #').setValue('42');
    expect(lastFilter(wrapper)?.numberId).toBe(42);
    await input(wrapper, 'Request #').setValue('');
    expect(lastFilter(wrapper)?.numberId).toBeNull();
  });

  it('searches from the first page on Enter', async () => {
    const wrapper = setup();
    await input(wrapper, 'Position').trigger('keypress', { key: 'a' });
    expect(wrapper.emitted('search')).toBeUndefined();
    await input(wrapper, 'Position').trigger('keypress', { key: 'Enter' });
    expect(lastFilter(wrapper)).toEqual({ pageIndex: 1 });
    expect(wrapper.emitted('search')).toHaveLength(1);
  });

  it('searches clients and filters by the selected one', async () => {
    vi.useFakeTimers();
    vi.mocked(getAgencyCompaniesList).mockResolvedValue([{ id: 'c1', value: 'Acme' }]);
    const wrapper = setup();
    const autocomplete = wrapper.findComponent({ name: 'BAutocomplete' });
    autocomplete.vm.$emit('typing', 'Acm');
    await vi.runAllTimersAsync();
    expect(getAgencyCompaniesList).toHaveBeenCalledWith('Acm');
    expect(autocomplete.props('data')).toEqual([{ id: 'c1', value: 'Acme' }]);

    autocomplete.vm.$emit('select', { id: 'c1', value: 'Acme' });
    await nextTick();
    expect(lastFilter(wrapper)).toEqual({ pageIndex: 1, companyProfileId: 'c1' });
    autocomplete.vm.$emit('select', null);
    await nextTick();
    expect(lastFilter(wrapper)?.companyProfileId).toBeNull();
    expect(wrapper.emitted('search')).toHaveLength(2);
  });

  it('shows the selected statuses and searches when they change', async () => {
    const wrapper = setup({ pageIndex: 2, statuses: [2] });
    const taginput = wrapper.findComponent({ name: 'BTaginput' });
    expect(taginput.props('modelValue')).toEqual([{ id: 2, value: 'Booked' }]);

    taginput.vm.$emit('update:modelValue', statusOptions);
    await nextTick();
    expect(lastFilter(wrapper)).toEqual({ pageIndex: 1, statuses: [1, 2] });
    taginput.vm.$emit('update:modelValue', []);
    await nextTick();
    expect(lastFilter(wrapper)?.statuses).toBeUndefined();
    expect(wrapper.emitted('search')).toHaveLength(2);
  });

  it('filters and clears the start date range', async () => {
    const from = '2026-06-01T04:00:00.000Z';
    const to = '2026-06-05T04:00:00.000Z';
    const wrapper = setup({ pageIndex: 1, startAtFrom: from, startAtTo: to });
    const datepicker = wrapper.findComponent({ name: 'BDatepicker' });
    expect(datepicker.props('modelValue')).toEqual([new Date(from), new Date(to)]);

    datepicker.vm.$emit('update:modelValue', [new Date(to), new Date(to)]);
    await nextTick();
    expect(lastFilter(wrapper)).toMatchObject({ startAtFrom: to, startAtTo: to });

    datepicker.vm.$emit('icon-right-click');
    await nextTick();
    expect(lastFilter(wrapper)).toMatchObject({ startAtFrom: null, startAtTo: null });
  });

  it('shows an empty date range without both ends', () => {
    const wrapper = setup({ pageIndex: 1, startAtFrom: '2026-06-01T04:00:00.000Z' });
    expect(wrapper.findComponent({ name: 'BDatepicker' }).props('modelValue')).toEqual([]);
  });
});
