import { describe, expect, it, vi } from 'vitest';
import WorkLiftForm from '@/shared/worker-profile/forms/WorkLiftForm.vue';
import { fetchLifts } from '@/shared/api/catalogApi';
import { createWorkerOther } from '@/shared/worker-profile/api';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createWorkerProfileDetail } from '@/testing/workerProfileFixtures';
import type { CatalogItem } from '@/shared/types/common';

vi.mock('@/shared/api/catalogApi', () => ({ fetchLifts: vi.fn() }));
vi.mock('@/shared/worker-profile/api', () => ({ createWorkerOther: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

const lifts: CatalogItem[] = [
  { id: 'lift-1', value: '25 lbs' },
  { id: 'lift-2', value: '50 lbs' },
];

async function setup(lift: CatalogItem | null = null) {
  vi.mocked(fetchLifts).mockResolvedValue(lifts);
  const wrapper = mountWithPlugins(WorkLiftForm, { props: { data: createWorkerProfileDetail({ lift }) } });
  await vi.waitFor(() => { expect(wrapper.findAll('select option:not([disabled])')).toHaveLength(2); });
  return wrapper;
}

function saveButton(wrapper: Awaited<ReturnType<typeof setup>>) {
  const button = wrapper.findAll('button').find((b) => b.text() === 'Save');
  if (!button) throw new Error('Save button not found');
  return button;
}

describe('WorkLiftForm', () => {
  it('lists the lift options and selects the worker lift', async () => {
    const wrapper = await setup({ id: 'lift-2', value: '50 lbs' });

    expect(wrapper.findAll('select option:not([disabled])').map((o) => o.text())).toEqual(['25 lbs', '50 lbs']);
    await vi.waitFor(() => { expect((wrapper.find('select').element as HTMLSelectElement).value).toBe('lift-2'); });
  });

  it('requires a lift', async () => {
    const wrapper = await setup();
    await saveButton(wrapper).trigger('click');

    await vi.waitFor(() => { expect(wrapper.find('.help.is-danger').text()).toBe('Lift is required'); });
    expect(showAlertError).toHaveBeenCalledWith('Please make sure all required fields are filled out correctly');
    expect(createWorkerOther).not.toHaveBeenCalled();
  });

  it('saves the selected lift and closes the modal', async () => {
    vi.mocked(createWorkerOther).mockResolvedValue();
    const wrapper = await setup();
    await wrapper.find('select').setValue('lift-1');
    await saveButton(wrapper).trigger('click');

    await vi.waitFor(() => { expect(wrapper.emitted('closeModal')).toEqual([[true]]); });
    expect(createWorkerOther).toHaveBeenCalledExactlyOnceWith('worker-profile-1', { lift: { id: 'lift-1' } });
  });

  it('shows the error and keeps the modal open when saving fails', async () => {
    const error = new Error('save failed');
    vi.mocked(createWorkerOther).mockRejectedValue(error);
    const wrapper = await setup({ id: 'lift-2', value: '50 lbs' });
    await saveButton(wrapper).trigger('click');

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(createWorkerOther).toHaveBeenCalledWith('worker-profile-1', { lift: { id: 'lift-2' } });
    expect(wrapper.emitted('closeModal')).toBeUndefined();
  });
});
