import { describe, expect, it, vi } from 'vitest';
import WorkAvailabilityTimesForm from '@/shared/worker-profile/forms/WorkAvailabilityTimesForm.vue';
import { getAvailabilityTimes } from '@/shared/api/catalogApi';
import { createWorkerAvailabilityTimes } from '@/shared/worker-profile/api';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createWorkerProfileDetail } from '@/testing/workerProfileFixtures';
import type { CatalogItem } from '@/shared/types/common';

vi.mock('@/shared/api/catalogApi', () => ({ getAvailabilityTimes: vi.fn() }));
vi.mock('@/shared/worker-profile/api', () => ({ createWorkerAvailabilityTimes: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

const morning: CatalogItem = { id: 'time-1', value: 'Morning' };
const afternoon: CatalogItem = { id: 'time-2', value: 'Afternoon' };
const night: CatalogItem = { id: 'time-3', value: 'Night' };

async function setup(availabilityTimes: CatalogItem[] = []) {
  vi.mocked(getAvailabilityTimes).mockResolvedValue([morning, afternoon, night]);
  const wrapper = mountWithPlugins(WorkAvailabilityTimesForm, {
    props: { data: createWorkerProfileDetail({ availabilityTimes }) },
  });
  await vi.waitFor(() => { expect(wrapper.findAll('input[type="checkbox"]')).toHaveLength(3); });
  return wrapper;
}

function checkbox(wrapper: Awaited<ReturnType<typeof setup>>, text: string) {
  const label = wrapper.findAll('label.checkbox').find((l) => l.text() === text);
  if (!label) throw new Error(`${text} checkbox not found`);
  return label.find('input');
}

function saveButton(wrapper: Awaited<ReturnType<typeof setup>>) {
  const button = wrapper.findAll('button').find((b) => b.text() === 'Save');
  if (!button) throw new Error('Save button not found');
  return button;
}

describe('WorkAvailabilityTimesForm', () => {
  it('lists the catalog times and checks the ones the worker has', async () => {
    const wrapper = await setup([{ id: 'time-2', value: 'Afternoon' }]);

    expect(wrapper.findAll('label.checkbox').map((l) => l.text())).toEqual(['Morning', 'Afternoon', 'Night']);
    await vi.waitFor(() => { expect((checkbox(wrapper, 'Afternoon').element as HTMLInputElement).checked).toBe(true); });
    expect((checkbox(wrapper, 'Morning').element as HTMLInputElement).checked).toBe(false);
  });

  it('saves the selected times and closes the modal', async () => {
    vi.mocked(createWorkerAvailabilityTimes).mockResolvedValue();
    const wrapper = await setup([morning]);
    await checkbox(wrapper, 'Night').setValue(true);
    await saveButton(wrapper).trigger('click');

    await vi.waitFor(() => { expect(wrapper.emitted('closeModal')).toEqual([[true]]); });
    expect(createWorkerAvailabilityTimes).toHaveBeenCalledExactlyOnceWith('worker-profile-1', [morning, night]);
  });

  it('shows the error and keeps the modal open when saving fails', async () => {
    const error = new Error('save failed');
    vi.mocked(createWorkerAvailabilityTimes).mockRejectedValue(error);
    const wrapper = await setup();
    await saveButton(wrapper).trigger('click');

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('closeModal')).toBeUndefined();
  });
});
