import { beforeEach, describe, expect, it } from 'vitest';
import AppVersionToast from '@/modules/landing/layout/AppVersionToast.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

function setup(modelValue?: boolean) {
  return mountWithPlugins(AppVersionToast, { props: modelValue === undefined ? {} : { modelValue } });
}

describe('AppVersionToast', () => {
  beforeEach(() => {
    sessionStorage.clear();
  });

  it('stays hidden without a new version', () => {
    expect(setup().find('.vtoast').exists()).toBe(false);
  });

  it('shows when a new version is available', () => {
    expect(setup(true).find('.vtoast__title').text()).toBe('New version available');
  });

  it('stays hidden once dismissed in this session', () => {
    sessionStorage.setItem('app-version-toast-dismissed', '1');

    expect(setup(true).find('.vtoast').exists()).toBe(false);
  });

  it('emits update when asked to reload', async () => {
    const wrapper = setup(true);
    await wrapper.find('.vtoast__update').trigger('click');

    expect(wrapper.emitted('update')).toHaveLength(1);
  });

  it('remembers the dismissal and closes', async () => {
    const wrapper = setup(true);
    await wrapper.find('.vtoast__dismiss').trigger('click');

    expect(wrapper.find('.vtoast').exists()).toBe(false);
    expect(sessionStorage.getItem('app-version-toast-dismissed')).toBe('1');
    expect(wrapper.emitted('update:modelValue')).toEqual([[false]]);
  });

  it('follows the model value', async () => {
    const wrapper = setup(false);
    await wrapper.setProps({ modelValue: true });
    expect(wrapper.find('.vtoast').exists()).toBe(true);

    await wrapper.setProps({ modelValue: false });
    expect(wrapper.find('.vtoast').exists()).toBe(false);
  });
});
