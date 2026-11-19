import { describe, expect, it } from 'vitest';
import Disclaimer from '@/modules/landing/pages/Legal/Disclaimer.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('Disclaimer', () => {
  it('renders the disclaimer sections', () => {
    const wrapper = mountWithPlugins(Disclaimer);

    expect(wrapper.find('.disclaimer__title').text()).toBe('Disclaimer');
    expect(wrapper.findAll('.disclaimer__heading').map((h) => h.text())).toContain('Use at Your Own Risk');
  });
});
