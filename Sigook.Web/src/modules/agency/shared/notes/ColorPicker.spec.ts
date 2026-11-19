import { describe, expect, it } from 'vitest';
import { nextTick } from 'vue';
import ColorPicker from '@/modules/agency/shared/notes/ColorPicker.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

function setup(modelValue: string) {
  return mountWithPlugins(ColorPicker, { props: { modelValue } });
}

async function open(wrapper: ReturnType<typeof setup>) {
  await wrapper.find('.color-picker-trigger').trigger('click');
  await nextTick();
  return Array.from(document.body.querySelectorAll<HTMLButtonElement>('.color-item'));
}

describe('ColorPicker', () => {
  it('shows the current color with a border for white', () => {
    const icon = setup('#fefefe').find('.note-color-icon');
    expect(icon.classes()).toContain('has-border');
    expect(setup('#f28c82').find('.note-color-icon').classes()).not.toContain('has-border');
  });

  it('marks the active color among the twelve options', async () => {
    const items = await open(setup('#fcbc05'));
    expect(items).toHaveLength(12);
    expect(items.filter((i) => i.classList.contains('active'))).toHaveLength(1);
    expect(items[2].classList.contains('active')).toBe(true);
  });

  it('emits the chosen color', async () => {
    const wrapper = setup('#fefefe');
    const items = await open(wrapper);
    items[1].click();
    expect(wrapper.emitted('update:modelValue')).toEqual([['#f28c82']]);
  });
});
