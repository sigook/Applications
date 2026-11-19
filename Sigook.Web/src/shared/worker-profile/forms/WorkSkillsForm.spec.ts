import { describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import WorkSkillsForm from '@/shared/worker-profile/forms/WorkSkillsForm.vue';
import { getSkills } from '@/shared/api/catalogApi';
import { createWorkerSkills } from '@/shared/worker-profile/api';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createWorkerProfileDetail } from '@/testing/workerProfileFixtures';
import type { WorkerProfileSkill } from '@/shared/worker-profile/types';

vi.mock('@/shared/api/catalogApi', () => ({ getSkills: vi.fn() }));
vi.mock('@/shared/worker-profile/api', () => ({ createWorkerSkills: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));

async function setup(skills: WorkerProfileSkill[] = []) {
  vi.mocked(getSkills).mockResolvedValue([{ skill: 'Forklift' }, { skill: 'Welding' }, { skill: 'Packing' }]);
  const wrapper = mountWithPlugins(WorkSkillsForm, { props: { data: createWorkerProfileDetail({ skills }) } });
  await Promise.allSettled(vi.mocked(getSkills).mock.results.map((r) => r.value));
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

describe('WorkSkillsForm', () => {
  it('shows the worker skills as tags', async () => {
    const wrapper = await setup([{ id: 'skill-1', skill: 'Forklift' }]);

    expect(tags(wrapper)).toEqual(['Forklift']);
  });

  it('filters the suggestions while typing', async () => {
    const wrapper = await setup();
    await type(wrapper, 'WEL');

    await vi.waitFor(() => { expect(suggestions()).toEqual(['Welding']); });
  });

  it('saves picked and new skills by name and closes the modal', async () => {
    vi.mocked(createWorkerSkills).mockResolvedValue();
    const wrapper = await setup([{ id: 'skill-1', skill: 'Forklift' }]);
    await type(wrapper, 'pack');
    await vi.waitFor(() => { expect(suggestions()).toEqual(['Packing']); });
    document.body.querySelector<HTMLElement>('.dropdown-item')?.click();
    await type(wrapper, 'Carpentry');
    await wrapper.find('.taginput input').trigger('keydown', { key: 'Enter' });
    await vi.waitFor(() => { expect(tags(wrapper)).toEqual(['Forklift', 'Packing', 'Carpentry']); });
    await saveButton(wrapper).trigger('click');

    await vi.waitFor(() => { expect(wrapper.emitted('closeModal')).toEqual([[true]]); });
    expect(createWorkerSkills).toHaveBeenCalledExactlyOnceWith('worker-profile-1', ['Forklift', 'Packing', 'Carpentry']);
  });

  it('shows the error and keeps the modal open when saving fails', async () => {
    const error = new Error('save failed');
    vi.mocked(createWorkerSkills).mockRejectedValue(error);
    const wrapper = await setup();
    await saveButton(wrapper).trigger('click');

    await vi.waitFor(() => { expect(showAlertError).toHaveBeenCalledWith(error); });
    expect(wrapper.emitted('closeModal')).toBeUndefined();
  });
});
