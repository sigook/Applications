import { describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import WorkLocationPreferencesForm from '@/shared/worker-profile/forms/WorkLocationPreferencesForm.vue';
import { getCities } from '@/shared/api/locationApi';
import { createWorkerLocationPreferences } from '@/shared/worker-profile/api';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createWorkerProfileDetail } from '@/testing/workerProfileFixtures';
import type { CatalogItem, City, LocationDetailModel } from '@/shared/types/common';

vi.mock('@/shared/api/locationApi', () => ({ getCities: vi.fn() }));
vi.mock('@/shared/worker-profile/api', () => ({ createWorkerLocationPreferences: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

const toronto: City = { id: 'city-1', value: 'Toronto' };
const ottawa: City = { id: 'city-2', value: 'Ottawa' };
const oakville: City = { id: 'city-3', value: 'Oakville' };

const ontarioLocation: LocationDetailModel = {
  address: '100 King St W',
  city: { id: 'city-1', value: 'Toronto', province: { id: 'province-on', value: 'Ontario' } },
};

async function setup(locationPreferences: CatalogItem[] = [], location: LocationDetailModel | null = ontarioLocation) {
  const wrapper = mountWithPlugins(WorkLocationPreferencesForm, {
    props: { data: createWorkerProfileDetail({ locationPreferences, location }) },
  });
  await Promise.allSettled(vi.mocked(getCities).mock.results.map((r) => r.value));
  await nextTick();
  return wrapper;
}

type Wrapper = Awaited<ReturnType<typeof setup>>;

function tags(wrapper: Wrapper) {
  return wrapper.findAll('.taginput .tag > span').map((t) => t.text());
}

function suggestions() {
  return Array.from(document.body.querySelectorAll<HTMLElement>('.dropdown-item')).map((i) => i.textContent.trim());
}

async function type(wrapper: Wrapper, text: string) {
  const input = wrapper.find('.taginput input');
  await input.trigger('focus');
  await input.setValue(text);
}

function saveButton(wrapper: Wrapper) {
  const button = wrapper.findAll('button').find((b) => b.text() === 'Save');
  if (!button) throw new Error('Save button not found');
  return button;
}

describe('WorkLocationPreferencesForm', () => {
  it('shows the worker preferences and loads the cities of the worker province', async () => {
    vi.mocked(getCities).mockResolvedValue([toronto, ottawa, oakville]);
    const wrapper = await setup([{ id: 'city-2', value: 'Ottawa' }]);

    expect(tags(wrapper)).toEqual(['Ottawa']);
    expect(getCities).toHaveBeenCalledExactlyOnceWith('province-on');
  });

  it('does not load cities when the worker has no province', async () => {
    await setup([], null);

    expect(getCities).not.toHaveBeenCalled();
  });

  it('shows the error when the cities fail to load', async () => {
    const error = new Error('cities failed');
    vi.mocked(getCities).mockRejectedValue(error);
    await setup();

    expect(showAlertError).toHaveBeenCalledWith(error);
  });

  it('filters the suggestions while typing', async () => {
    vi.mocked(getCities).mockResolvedValue([toronto, ottawa, oakville]);
    const wrapper = await setup();
    await type(wrapper, 'O');

    await vi.waitFor(() => { expect(suggestions()).toEqual(['Toronto', 'Ottawa', 'Oakville']); });
    await type(wrapper, 'OAK');
    await vi.waitFor(() => { expect(suggestions()).toEqual(['Oakville']); });
  });

  it('saves the picked cities and closes the modal', async () => {
    vi.mocked(getCities).mockResolvedValue([toronto, ottawa, oakville]);
    vi.mocked(createWorkerLocationPreferences).mockResolvedValue();
    const wrapper = await setup([ottawa]);
    await type(wrapper, 'tor');
    await vi.waitFor(() => { expect(suggestions()).toEqual(['Toronto']); });
    document.body.querySelector<HTMLElement>('.dropdown-item')?.click();
    await vi.waitFor(() => { expect(tags(wrapper)).toEqual(['Ottawa', 'Toronto']); });
    await saveButton(wrapper).trigger('click');

    await vi.waitFor(() => { expect(wrapper.emitted('closeModal')).toEqual([[true]]); });
    expect(createWorkerLocationPreferences).toHaveBeenCalledExactlyOnceWith('worker-profile-1', [ottawa, toronto]);
  });

  it('shows the error and keeps the modal open when saving fails', async () => {
    const error = new Error('save failed');
    vi.mocked(createWorkerLocationPreferences).mockRejectedValue(error);
    const wrapper = await setup([], null);
    await saveButton(wrapper).trigger('click');

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('closeModal')).toBeUndefined();
  });
});
