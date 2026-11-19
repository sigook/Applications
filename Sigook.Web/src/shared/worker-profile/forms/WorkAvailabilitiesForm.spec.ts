import { describe, expect, it, vi } from 'vitest';
import WorkAvailabilitiesForm from '@/shared/worker-profile/forms/WorkAvailabilitiesForm.vue';
import { getAvailability } from '@/shared/api/catalogApi';
import { createWorkerAvailabilities } from '@/shared/worker-profile/api';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createWorkerProfileDetail } from '@/testing/workerProfileFixtures';
import type { CatalogItem } from '@/shared/types/common';

vi.mock('@/shared/api/catalogApi', () => ({ getAvailability: vi.fn() }));
vi.mock('@/shared/worker-profile/api', () => ({ createWorkerAvailabilities: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

const fullTime: CatalogItem = { id: 'availability-1', value: 'Full Time' };
const partTime: CatalogItem = { id: 'availability-2', value: 'Part Time' };
const weekends: CatalogItem = { id: 'availability-3', value: 'Weekends' };

async function setup(availabilities: CatalogItem[] = []) {
  vi.mocked(getAvailability).mockResolvedValue([fullTime, partTime, weekends]);
  const wrapper = mountWithPlugins(WorkAvailabilitiesForm, {
    props: { data: createWorkerProfileDetail({ availabilities }) },
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

describe('WorkAvailabilitiesForm', () => {
  it('lists the catalog availabilities and checks the ones the worker has', async () => {
    const wrapper = await setup([{ id: 'availability-2', value: 'Part Time' }]);

    expect(wrapper.findAll('label.checkbox').map((l) => l.text())).toEqual(['Full Time', 'Part Time', 'Weekends']);
    await vi.waitFor(() => { expect((checkbox(wrapper, 'Part Time').element as HTMLInputElement).checked).toBe(true); });
    expect((checkbox(wrapper, 'Full Time').element as HTMLInputElement).checked).toBe(false);
  });

  it('saves the selected availabilities and closes the modal', async () => {
    vi.mocked(createWorkerAvailabilities).mockResolvedValue();
    const wrapper = await setup([fullTime]);
    await checkbox(wrapper, 'Weekends').setValue(true);
    await saveButton(wrapper).trigger('click');

    await vi.waitFor(() => { expect(wrapper.emitted('closeModal')).toEqual([[true]]); });
    expect(createWorkerAvailabilities).toHaveBeenCalledExactlyOnceWith('worker-profile-1', [fullTime, weekends]);
  });

  it('shows the error and keeps the modal open when saving fails', async () => {
    const error = new Error('save failed');
    vi.mocked(createWorkerAvailabilities).mockRejectedValue(error);
    const wrapper = await setup();
    await saveButton(wrapper).trigger('click');

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('closeModal')).toBeUndefined();
  });
});
