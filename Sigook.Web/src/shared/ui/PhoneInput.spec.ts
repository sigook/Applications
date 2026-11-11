import { describe, expect, it } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import PhoneInput from '@/shared/ui/PhoneInput.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

function setup(props: Record<string, unknown> = {}) {
  return mountWithPlugins(PhoneInput, { props });
}

describe('PhoneInput', () => {
  it('formats the default value', () => {
    const wrapper = setup({ defaultValue: '4165550199' });
    expect((wrapper.find('input').element as HTMLInputElement).value).toBe('416 555-0199');
  });

  it('shows the label with a required mark', () => {
    const wrapper = setup({ label: 'Phone', required: true });
    expect(wrapper.find('label').text()).toBe('Phone *');
  });

  it.each([
    ['416', '416'],
    ['41655', '416 55'],
    ['(416) 555-0199 ext', '416 555-0199'],
  ])('formats typed %s as %s and emits it', async (typed, expected) => {
    const wrapper = setup();
    await wrapper.find('input').setValue(typed);
    await flushPromises();

    expect((wrapper.find('input').element as HTMLInputElement).value).toBe(expected);
    expect(wrapper.emitted('formattedPhone')?.at(-1)).toEqual([expected]);
  });

  it('emits null when cleared', async () => {
    const wrapper = setup({ defaultValue: '4165550199' });
    await wrapper.find('input').setValue('');
    await flushPromises();
    expect(wrapper.emitted('formattedPhone')?.at(-1)).toEqual([null]);
  });

  it('follows changes to the default value', async () => {
    const wrapper = setup({ defaultValue: '' });
    await wrapper.setProps({ defaultValue: '2125550123' });
    await flushPromises();
    expect((wrapper.find('input').element as HTMLInputElement).value).toBe('212 555-0123');
  });

  it('hides errors until validated, then reports an invalid number', async () => {
    const wrapper = setup({ required: true });
    await flushPromises();
    expect(wrapper.find('.help.is-danger').exists()).toBe(false);

    const valid = await (wrapper.vm as unknown as { validatePhone: () => Promise<boolean> }).validatePhone();
    await flushPromises();

    expect(valid).toBe(false);
    expect(wrapper.find('.help.is-danger').text()).toBe('Phone is required');
  });

  it('validates a correct number', async () => {
    const wrapper = setup({ required: true, defaultValue: '4165550199' });
    const valid = await (wrapper.vm as unknown as { validatePhone: () => Promise<boolean> }).validatePhone();
    expect(valid).toBe(true);
  });
});
