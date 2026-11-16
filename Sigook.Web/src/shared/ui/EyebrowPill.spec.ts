import { describe, expect, it } from 'vitest';
import { mount } from '@vue/test-utils';
import EyebrowPill from '@/shared/ui/EyebrowPill.vue';

describe('EyebrowPill', () => {
  it('uses the cyan variant by default', () => {
    const wrapper = mount(EyebrowPill, { slots: { default: 'Hiring' } });
    expect(wrapper.text()).toBe('Hiring');
    expect(wrapper.classes()).toContain('eyebrow-pill--cyan');
  });

  it('applies the requested variant', () => {
    expect(mount(EyebrowPill, { props: { variant: 'red' } }).classes()).toContain('eyebrow-pill--red');
  });
});
