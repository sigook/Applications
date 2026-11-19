import { describe, expect, it } from 'vitest';
import AboutMissionSection from '@/modules/landing/components/About/AboutMissionSection.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('AboutMissionSection', () => {
  it('renders the mission and vision cards', () => {
    const wrapper = mountWithPlugins(AboutMissionSection);

    expect(wrapper.find('.section-header__heading').text()).toContain('Built on purpose.');
    expect(wrapper.findAll('.about-mission__card').map((c) => c.text())).toEqual([
      expect.stringContaining('Our Mission'),
      expect.stringContaining('Our Vision'),
    ]);
  });
});
