import { describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import AgencyWorkerRequestHistory from '@/modules/agency/recruiting/workers/components/AgencyWorkerRequestHistory.vue';
import { getAgencyWorkerProfileRequestHistory } from '@/modules/agency/recruiting/workers/api';
import { RequestStatus, WorkerRequestStatus } from '@/shared/constants/enums';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { WorkerRequestHistoryItem } from '@/modules/agency/recruiting/workers/types';
import type { PaginatedList } from '@/shared/types/common';

vi.mock('@/modules/agency/recruiting/workers/api', () => ({ getAgencyWorkerProfileRequestHistory: vi.fn() }));
vi.mock('@/shared/api/reportApi', () => ({ downloadAgencyReport: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

const row: WorkerRequestHistoryItem = {
  id: 'req-1',
  numberId: 321,
  jobTitle: 'Forklift operator',
  companyFullName: 'Acme Foods',
  companyProfileId: 'cp-1',
  location: '100 King St',
  entrance: 'Door 4',
  createdAt: '2026-02-01T12:00:00',
  requestStatus: RequestStatus.Filled,
  workerRequestStatus: WorkerRequestStatus.Booked,
  isAsap: true,
  startWorking: '2026-02-03T12:00:00',
  finishWorking: null,
};

function page(items: WorkerRequestHistoryItem[]): PaginatedList<WorkerRequestHistoryItem> {
  return { items, totalItems: items.length, pageIndex: 1, totalPages: 1 };
}

async function setup(items: WorkerRequestHistoryItem[] = [row]) {
  vi.mocked(getAgencyWorkerProfileRequestHistory).mockResolvedValue(page(items));
  const wrapper = mountWithPlugins(AgencyWorkerRequestHistory, { props: { workerId: 'wp-1' } });
  await flushPromises();
  return wrapper;
}

function cells(wrapper: Awaited<ReturnType<typeof setup>>) {
  return wrapper.findAll('tbody tr td');
}

describe('AgencyWorkerRequestHistory', () => {
  it('loads the requests of the worker', async () => {
    await setup();

    expect(getAgencyWorkerProfileRequestHistory).toHaveBeenCalledWith(expect.objectContaining({
      profileId: 'wp-1',
      sortBy: 0,
      isDescending: true,
    }));
  });

  it('renders the request with links, flags and statuses', async () => {
    const wrapper = await setup();
    const texts = cells(wrapper).map((c) => c.text());

    expect(texts[0]).toContain('321');
    expect(texts[0]).toContain('Asap');
    expect(texts[1]).toBe('Acme Foods');
    expect(texts[2]).toBe('Forklift operator');
    expect(texts[3].replace(/\s+/g, ' ')).toBe('100 King St - Door 4');
    expect(texts[5]).toBe('—');
    expect(texts[6]).toBe('Booked');
    expect(cells(wrapper)[6].find('.tag').classes()).toContain('is-success');
    expect(cells(wrapper)[7].find('.dot-status').classes()).toContain('status-filled');
    expect(cells(wrapper)[7].find('.request-check').exists()).toBe(true);
    const links = wrapper.findAll('tbody a').map((a) => a.attributes('href'));
    expect(links).toEqual(['/recruiting/requests/req-1', '/recruiting/companies/cp-1']);
  });

  it('renders an open request without flags, entrance or start date', async () => {
    const wrapper = await setup([{ ...row, isAsap: false, entrance: undefined, startWorking: null, requestStatus: RequestStatus.Open, finishWorking: '2026-02-10T12:00:00' }]);
    const texts = cells(wrapper).map((c) => c.text());

    expect(texts[0]).not.toContain('Asap');
    expect(texts[3]).toBe('100 King St');
    expect(texts[4]).toBe('—');
    expect(texts[5]).not.toBe('—');
    expect(cells(wrapper)[7].find('.request-check').exists()).toBe(false);
  });

  it('opens the request when a cell other than the client is clicked', async () => {
    const wrapper = await setup();

    await cells(wrapper)[1].trigger('click');
    await flushPromises();
    expect(wrapper.vm.$router.currentRoute.value.path).toBe('/');

    await cells(wrapper)[2].trigger('click');
    await flushPromises();
    expect(wrapper.vm.$router.currentRoute.value.path).toBe('/recruiting/requests/req-1');
  });

  it('filters by worker and request statuses', async () => {
    const wrapper = await setup();
    const [workerStatuses, requestStatuses] = wrapper.findAllComponents({ name: 'BTaginput' });

    workerStatuses.vm.$emit('update:modelValue', [{ id: WorkerRequestStatus.Finished, value: 'Finished' }]);
    await flushPromises();
    expect(getAgencyWorkerProfileRequestHistory).toHaveBeenLastCalledWith(expect.objectContaining({ workerRequestStatuses: [WorkerRequestStatus.Finished] }));

    requestStatuses.vm.$emit('update:modelValue', [{ id: RequestStatus.Cancelled, value: 'Cancelled' }]);
    await flushPromises();
    expect(getAgencyWorkerProfileRequestHistory).toHaveBeenLastCalledWith(expect.objectContaining({
      statuses: [RequestStatus.Cancelled],
      workerRequestStatuses: [WorkerRequestStatus.Finished],
    }));
  });

  it('filters by start date range and clears it', async () => {
    const wrapper = await setup();
    const from = new Date('2026-02-01T00:00:00');
    const to = new Date('2026-02-28T00:00:00');
    const picker = wrapper.findComponent({ name: 'BDatepicker' });

    picker.vm.$emit('update:modelValue', [from, to]);
    await flushPromises();
    expect(getAgencyWorkerProfileRequestHistory).toHaveBeenLastCalledWith(expect.objectContaining({
      startWorkingFrom: from.toISOString(),
      startWorkingTo: to.toISOString(),
    }));

    picker.vm.$emit('icon-right-click');
    await flushPromises();
    expect(getAgencyWorkerProfileRequestHistory).toHaveBeenLastCalledWith(expect.objectContaining({ startWorkingFrom: null, startWorkingTo: null }));
  });
});
