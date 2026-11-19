import { describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import AgencyRequestSkills from '@/modules/agency/recruiting/requests/components/AgencyRequestSkills.vue';
import { deleteAgencyRequestSkill, postAgencyRequestSkill } from '@/modules/agency/recruiting/requests/api';
import { showAlertError } from '@/shared/utils/toast';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { AgencyRequestSkillModel } from '@/modules/agency/recruiting/requests/types';

vi.mock('@/modules/agency/recruiting/requests/api', () => ({ postAgencyRequestSkill: vi.fn(), deleteAgencyRequestSkill: vi.fn() }));
vi.mock('@/shared/utils/toast', () => ({ showAlertError: vi.fn() }));
vi.mock('@/shared/ui/FormSkillAdd.vue', () => ({
  default: {
    name: 'FormSkillAdd',
    props: { existingSkills: Array },
    emits: ['onPressAdd', 'onDelete'],
    template: '<div class="skills-form-stub" />',
  },
}));

const forklift: AgencyRequestSkillModel = { id: 's-1', skill: 'Forklift' };
const packing: AgencyRequestSkillModel = { id: 's-2', skill: 'Packing' };

function setup(modelValue: AgencyRequestSkillModel[], canEdit = true) {
  return mountWithPlugins(AgencyRequestSkills, {
    props: { requestId: 'req-1', canEdit, modelValue, 'onUpdate:modelValue': vi.fn() },
  });
}

type Wrapper = ReturnType<typeof setup>;

function form(wrapper: Wrapper) {
  return wrapper.findComponent({ name: 'FormSkillAdd' });
}

function lastModel(wrapper: Wrapper) {
  return wrapper.emitted<[AgencyRequestSkillModel[]]>('update:modelValue')?.at(-1)?.[0];
}

describe('AgencyRequestSkills', () => {
  it('shows the skills as tags when read-only', () => {
    const wrapper = setup([forklift, packing], false);

    expect(wrapper.findAll('.request-skills-tag').map((t) => t.text())).toEqual(['Forklift', 'Packing']);
    expect(form(wrapper).exists()).toBe(false);
  });

  it('shows an empty state when read-only without skills', () => {
    const wrapper = setup([], false);

    expect(wrapper.find('.request-skills-empty').text()).toBe('No skills');
  });

  it('passes the skills to the editor', () => {
    const wrapper = setup([forklift]);

    expect(form(wrapper).props('existingSkills')).toEqual([forklift]);
  });

  it('adds a skill with its new id', async () => {
    vi.mocked(postAgencyRequestSkill).mockResolvedValue({ id: 's-9' });
    const wrapper = setup([forklift]);
    form(wrapper).vm.$emit('onPressAdd', { skill: 'Welding' });
    await flushPromises();

    expect(postAgencyRequestSkill).toHaveBeenCalledExactlyOnceWith('req-1', { skill: 'Welding' });
    expect(lastModel(wrapper)).toEqual([forklift, { id: 's-9', skill: 'Welding' }]);
  });

  it('restores the skills when adding fails', async () => {
    const error = new Error('boom');
    vi.mocked(postAgencyRequestSkill).mockRejectedValue(error);
    const wrapper = setup([forklift]);
    form(wrapper).vm.$emit('onPressAdd', { skill: 'Welding' });
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(lastModel(wrapper)).toEqual([forklift]);
  });

  it('removes a skill', async () => {
    vi.mocked(deleteAgencyRequestSkill).mockResolvedValue();
    const wrapper = setup([forklift, packing]);
    form(wrapper).vm.$emit('onDelete', forklift);
    await flushPromises();

    expect(deleteAgencyRequestSkill).toHaveBeenCalledExactlyOnceWith('req-1', 's-1');
    expect(lastModel(wrapper)).toEqual([packing]);
  });

  it('ignores removing an unsaved skill', async () => {
    const wrapper = setup([forklift]);
    form(wrapper).vm.$emit('onDelete', { skill: 'Draft' });
    await flushPromises();

    expect(deleteAgencyRequestSkill).not.toHaveBeenCalled();
    expect(lastModel(wrapper)).toBeUndefined();
  });

  it('restores the skills when removing fails', async () => {
    const error = new Error('boom');
    vi.mocked(deleteAgencyRequestSkill).mockRejectedValue(error);
    const wrapper = setup([forklift]);
    form(wrapper).vm.$emit('onDelete', forklift);
    await flushPromises();

    expect(showAlertError).toHaveBeenCalledWith(error);
    expect(lastModel(wrapper)).toEqual([forklift]);
  });
});
