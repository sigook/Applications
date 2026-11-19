import { describe, expect, it } from 'vitest';
import IndustryIcon from '@/modules/landing/components/shared/icons/IndustryIcon.vue';
import LandingIcon from '@/modules/landing/components/shared/icons/LandingIcon.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('IndustryIcon', () => {
  it('renders the industry glyph', () => {
    const wrapper = mountWithPlugins(IndustryIcon, { props: { name: 'logistics' } });

    expect(wrapper.findComponent(LandingIcon).props('name')).toBe('logistics');
    expect(wrapper.classes()).toContain('industry-icon');
  });
});
