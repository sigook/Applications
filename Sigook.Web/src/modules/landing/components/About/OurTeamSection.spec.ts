import { describe, expect, it } from 'vitest';
import OurTeamSection from '@/modules/landing/components/About/OurTeamSection.vue';
import members from '@/modules/landing/data/teamMembers.json';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('OurTeamSection', () => {
  it('renders a card per team member', () => {
    const wrapper = mountWithPlugins(OurTeamSection);

    expect(wrapper.findAll('.team__name').map((n) => n.text())).toEqual(members.map((m) => m.name));
  });

  it('shows initials when there is no photo', () => {
    const wrapper = mountWithPlugins(OurTeamSection);

    expect(wrapper.findAll('.team__initials')[0].text()).toBe('AG');
  });

  it('links only real LinkedIn profiles', () => {
    const wrapper = mountWithPlugins(OurTeamSection);
    const expected = members.filter((m) => m.linkedin && m.linkedin.trim() !== '#').map((m) => m.linkedin);

    expect(wrapper.findAll('.team__linkedin').map((a) => a.attributes('href'))).toEqual(expected);
    expect(wrapper.find('.team__linkedin').attributes('aria-label')).toBe(`${members[0].name} on LinkedIn`);
  });
});
