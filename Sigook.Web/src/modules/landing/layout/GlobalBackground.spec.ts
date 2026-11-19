import { describe, expect, it } from 'vitest';
import GlobalBackground from '@/modules/landing/layout/GlobalBackground.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('GlobalBackground', () => {
  it('renders the decorative layer hidden from assistive tech', () => {
    const wrapper = mountWithPlugins(GlobalBackground);

    expect(wrapper.attributes('aria-hidden')).toBe('true');
    expect(wrapper.findAll('.global-bg__deco')).toHaveLength(8);
  });
});
