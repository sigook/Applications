import { describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import FormSkillAdd from '@/shared/ui/FormSkillAdd.vue';
import { getSkills } from '@/shared/api/catalogApi';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { Skill } from '@/shared/types/common';

vi.mock('@/shared/api/catalogApi', () => ({ getSkills: vi.fn() }));

const catalog: Skill[] = [{ skill: 'Forklift' }, { skill: 'Welding' }, { skill: 'Reach Truck' }];

async function setup(existingSkills?: Skill[]) {
  vi.mocked(getSkills).mockResolvedValue(catalog);
  const wrapper = mountWithPlugins(FormSkillAdd, { props: { existingSkills } });
  await flushPromises();
  return wrapper;
}

function taginput(wrapper: Awaited<ReturnType<typeof setup>>) {
  return wrapper.findComponent({ name: 'BTaginput' });
}

describe('FormSkillAdd', () => {
  it('shows the existing skills as tags', async () => {
    const wrapper = await setup([{ skill: 'Forklift' }, { skill: 'Driving' }]);
    expect(wrapper.findAll('.tag').map((t) => t.text())).toEqual(['Forklift', 'Driving']);
  });

  it('offers the whole catalog, then filters it while typing', async () => {
    const wrapper = await setup();
    expect(taginput(wrapper).props('data')).toEqual(catalog);

    taginput(wrapper).vm.$emit('typing', 'TRUCK');
    await flushPromises();
    expect(taginput(wrapper).props('data')).toEqual([{ skill: 'Reach Truck' }]);
  });

  it('emits a picked skill and the new list', async () => {
    const wrapper = await setup([{ skill: 'Driving' }]);

    taginput(wrapper).vm.$emit('update:modelValue', [{ skill: 'Driving' }, { skill: 'Welding' }]);
    taginput(wrapper).vm.$emit('add', { skill: 'Welding' });
    await flushPromises();

    expect(wrapper.emitted('onPressAdd')).toEqual([[{ skill: 'Welding' }]]);
    expect(wrapper.emitted('update:existingSkills')?.at(-1)).toEqual([[{ skill: 'Driving' }, { skill: 'Welding' }]]);
  });

  it('wraps a typed new skill', async () => {
    const wrapper = await setup();
    taginput(wrapper).vm.$emit('add', 'Painting');
    expect(wrapper.emitted('onPressAdd')).toEqual([[{ skill: 'Painting' }]]);
  });

  it('emits onDelete when a tag is removed', async () => {
    const wrapper = await setup([{ skill: 'Forklift' }]);
    await wrapper.find('.tag .delete').trigger('click');

    expect(wrapper.emitted('onDelete')).toEqual([[{ skill: 'Forklift' }]]);
    expect(wrapper.emitted('update:existingSkills')?.at(-1)).toEqual([[]]);
  });

  it('follows changes to the existing skills', async () => {
    const wrapper = await setup([]);
    await wrapper.setProps({ existingSkills: [{ skill: 'Welding' }] });
    expect(wrapper.findAll('.tag').map((t) => t.text())).toEqual(['Welding']);
  });

  it('closes on escape without emitting', async () => {
    const wrapper = await setup();
    await wrapper.find('input').trigger('keydown', { key: 'Escape' });
    expect(wrapper.emitted('onPressAdd')).toBeUndefined();
  });
});
