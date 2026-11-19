import { describe, expect, it, vi } from 'vitest';
import WorkAvailabilityDaysForm from '@/shared/worker-profile/forms/WorkAvailabilityDaysForm.vue';
import { getDays } from '@/shared/api/catalogApi';
import { createWorkerAvailabilityDays } from '@/shared/worker-profile/api';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createWorkerProfileDetail } from '@/testing/workerProfileFixtures';
import type { CatalogItem } from '@/shared/types/common';

vi.mock('@/shared/api/catalogApi', () => ({ getDays: vi.fn() }));
vi.mock('@/shared/worker-profile/api', () => ({ createWorkerAvailabilityDays: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

const monday: CatalogItem = { id: 'day-1', value: 'Monday' };
const tuesday: CatalogItem = { id: 'day-2', value: 'Tuesday' };
const wednesday: CatalogItem = { id: 'day-3', value: 'Wednesday' };
const days = [monday, tuesday, wednesday];

async function setup(availabilityDays: CatalogItem[] = []) {
  vi.mocked(getDays).mockResolvedValue(days);
  const wrapper = mountWithPlugins(WorkAvailabilityDaysForm, {
    props: { data: createWorkerProfileDetail({ availabilityDays }) },
  });
  await vi.waitFor(() => { expect(wrapper.findAll('input[type="checkbox"]')).toHaveLength(4); });
  return wrapper;
}

function checkbox(wrapper: Awaited<ReturnType<typeof setup>>, text: string) {
  const label = wrapper.findAll('label.checkbox').find((l) => l.text() === text);
  if (!label) throw new Error(`${text} checkbox not found`);
  return label.find('input');
}

function isChecked(wrapper: Awaited<ReturnType<typeof setup>>, text: string) {
  return (checkbox(wrapper, text).element as HTMLInputElement).checked;
}

function saveButton(wrapper: Awaited<ReturnType<typeof setup>>) {
  const button = wrapper.findAll('button').find((b) => b.text() === 'Save');
  if (!button) throw new Error('Save button not found');
  return button;
}

describe('WorkAvailabilityDaysForm', () => {
  it('checks the days the worker already has', async () => {
    const wrapper = await setup([{ id: 'day-2', value: 'Tuesday' }]);

    expect(wrapper.findAll('label.checkbox').map((l) => l.text())).toEqual(['All Days', 'Monday', 'Tuesday', 'Wednesday']);
    await vi.waitFor(() => { expect(isChecked(wrapper, 'Tuesday')).toBe(true); });
    expect(isChecked(wrapper, 'Monday')).toBe(false);
    expect(isChecked(wrapper, 'All Days')).toBe(false);
  });

  it('checks All Days when the worker has every day', async () => {
    const wrapper = await setup([...days]);

    await vi.waitFor(() => { expect(isChecked(wrapper, 'All Days')).toBe(true); });
  });

  it('selects every day with All Days and saves them', async () => {
    vi.mocked(createWorkerAvailabilityDays).mockResolvedValue();
    const wrapper = await setup();
    await checkbox(wrapper, 'All Days').setValue(true);

    expect(days.every((day) => isChecked(wrapper, day.value))).toBe(true);
    await saveButton(wrapper).trigger('click');

    await vi.waitFor(() => { expect(wrapper.emitted('closeModal')).toEqual([[true]]); });
    expect(createWorkerAvailabilityDays).toHaveBeenCalledExactlyOnceWith('worker-profile-1', days);
  });

  it('clears every day when All Days is unchecked', async () => {
    const wrapper = await setup([...days]);
    await vi.waitFor(() => { expect(isChecked(wrapper, 'All Days')).toBe(true); });
    await checkbox(wrapper, 'All Days').setValue(false);

    expect(days.some((day) => isChecked(wrapper, day.value))).toBe(false);
  });

  it('syncs All Days with the individual days', async () => {
    const wrapper = await setup([monday, tuesday]);
    await checkbox(wrapper, 'Wednesday').setValue(true);
    expect(isChecked(wrapper, 'All Days')).toBe(true);

    await checkbox(wrapper, 'Monday').setValue(false);
    expect(isChecked(wrapper, 'All Days')).toBe(false);
  });

  it('shows the error and keeps the modal open when saving fails', async () => {
    const error = new Error('save failed');
    vi.mocked(createWorkerAvailabilityDays).mockRejectedValue(error);
    const wrapper = await setup();
    await saveButton(wrapper).trigger('click');

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('closeModal')).toBeUndefined();
  });
});
