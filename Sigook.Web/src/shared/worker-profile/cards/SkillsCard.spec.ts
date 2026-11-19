import { describe, expect, it, vi } from 'vitest';
import { nextTick } from 'vue';
import SkillsCard from '@/shared/worker-profile/cards/SkillsCard.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import { createWorkerProfileDetail } from '@/testing/workerProfileFixtures';
import type { WorkerProfileDetail } from '@/shared/worker-profile/types';

vi.mock('@/shared/worker-profile/forms/WorkSkillsForm.vue', () => ({
  default: { name: 'SkillsForm', props: { data: Object }, emits: ['closeModal'], template: '<div class="skills-form" />' },
}));

function setup(overrides: Partial<WorkerProfileDetail> = {}) {
  return mountWithPlugins(SkillsCard, { props: { worker: createWorkerProfileDetail(overrides) } });
}

describe('SkillsCard', () => {
  it('shows the empty state', () => {
    expect(setup().find('.profile-empty').text()).toBe('No skills added');
  });

  it('lists the skills', () => {
    const wrapper = setup({ skills: [{ id: 's1', skill: 'Forklift' }, { id: null, skill: 'Welding' }] });

    expect(wrapper.findAll('.profile-tag').map((t) => t.text())).toEqual(['Forklift', 'Welding']);
    expect(wrapper.find('.profile-empty').exists()).toBe(false);
  });

  it('opens the skills form and emits updateProfile once it closes', async () => {
    const wrapper = setup();
    expect(wrapper.findComponent({ name: 'SkillsForm' }).exists()).toBe(false);

    await wrapper.find('button').trigger('click');
    await nextTick();

    const form = wrapper.findComponent({ name: 'SkillsForm' });
    expect(form.props('data')).toMatchObject({ id: 'worker-profile-1' });
    form.vm.$emit('closeModal');
    expect(wrapper.emitted('updateProfile')).toHaveLength(1);
  });
});
