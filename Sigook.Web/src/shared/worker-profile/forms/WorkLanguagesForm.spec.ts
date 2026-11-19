import { describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import WorkLanguagesForm from '@/shared/worker-profile/forms/WorkLanguagesForm.vue';
import { fetchLanguages } from '@/shared/api/catalogApi';
import { createWorkerLanguages } from '@/shared/worker-profile/api';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createWorkerProfileDetail } from '@/testing/workerProfileFixtures';
import type { CatalogItem } from '@/shared/types/common';

vi.mock('@/shared/api/catalogApi', () => ({ fetchLanguages: vi.fn() }));
vi.mock('@/shared/worker-profile/api', () => ({ createWorkerLanguages: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

const english: CatalogItem = { id: 'language-1', value: 'English' };
const french: CatalogItem = { id: 'language-2', value: 'French' };
const spanish: CatalogItem = { id: 'language-3', value: 'Spanish' };

async function setup(languages: CatalogItem[] = []) {
  vi.mocked(fetchLanguages).mockResolvedValue([english, french, spanish]);
  const wrapper = mountWithPlugins(WorkLanguagesForm, { props: { data: createWorkerProfileDetail({ languages }) } });
  await Promise.allSettled(vi.mocked(fetchLanguages).mock.results.map((r) => r.value));
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

describe('WorkLanguagesForm', () => {
  it('shows the worker languages as tags', async () => {
    const wrapper = await setup([english, french]);

    expect(tags(wrapper)).toEqual(['English', 'French']);
  });

  it('filters the suggestions while typing', async () => {
    const wrapper = await setup();
    await type(wrapper, 'SPA');

    await vi.waitFor(() => { expect(suggestions()).toEqual(['Spanish']); });
  });

  it('saves the picked languages and closes the modal', async () => {
    vi.mocked(createWorkerLanguages).mockResolvedValue();
    const wrapper = await setup([english]);
    await type(wrapper, 'fre');
    await vi.waitFor(() => { expect(suggestions()).toEqual(['French']); });
    document.body.querySelector<HTMLElement>('.dropdown-item')?.click();
    await vi.waitFor(() => { expect(tags(wrapper)).toEqual(['English', 'French']); });
    await saveButton(wrapper).trigger('click');

    await vi.waitFor(() => { expect(wrapper.emitted('closeModal')).toEqual([[true]]); });
    expect(createWorkerLanguages).toHaveBeenCalledExactlyOnceWith('worker-profile-1', [english, french]);
  });

  it('removes a language', async () => {
    vi.mocked(createWorkerLanguages).mockResolvedValue();
    const wrapper = await setup([english, french]);
    await wrapper.findAll('.taginput .tag .delete')[0].trigger('click');
    await saveButton(wrapper).trigger('click');

    await vi.waitFor(() => { expect(createWorkerLanguages).toHaveBeenCalledExactlyOnceWith('worker-profile-1', [french]); });
  });

  it('shows the error and keeps the modal open when saving fails', async () => {
    const error = new Error('save failed');
    vi.mocked(createWorkerLanguages).mockRejectedValue(error);
    const wrapper = await setup();
    await saveButton(wrapper).trigger('click');

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('closeModal')).toBeUndefined();
  });
});
