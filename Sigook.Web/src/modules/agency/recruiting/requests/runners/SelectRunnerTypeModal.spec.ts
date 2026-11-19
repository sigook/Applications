import { describe, expect, it } from 'vitest';
import SelectRunnerTypeModal from '@/modules/agency/recruiting/requests/runners/SelectRunnerTypeModal.vue';
import { RunnerType } from '@/modules/agency/recruiting/requests/runners/types';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

function setup(name?: string) {
  return mountWithPlugins(SelectRunnerTypeModal, { props: { name } });
}

describe('SelectRunnerTypeModal', () => {
  it('shows the worker name and selects Active by default', async () => {
    const wrapper = setup('Ana Lopez');

    expect(wrapper.find('section p').text()).toBe('Ana Lopez');
    await wrapper.find('form').trigger('submit');

    expect(wrapper.emitted('select')).toEqual([[RunnerType.Active]]);
  });

  it('hides the name when none is given', () => {
    const wrapper = setup();

    expect(wrapper.find('section p').exists()).toBe(false);
  });

  it('emits the passive type when chosen', async () => {
    const wrapper = setup();
    await wrapper.findAll('input[type="radio"]')[1].setValue(true);
    await wrapper.find('form').trigger('submit');

    expect(wrapper.emitted('select')).toEqual([[RunnerType.Passive]]);
  });

  it('closes on cancel', async () => {
    const wrapper = setup();
    await wrapper.find('footer button').trigger('click');

    expect(wrapper.emitted('close')).toHaveLength(1);
    expect(wrapper.emitted('select')).toBeUndefined();
  });
});
