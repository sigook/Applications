import { describe, expect, it } from 'vitest';
import LandingHero from '@/modules/landing/components/shared/hero/LandingHero.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('LandingHero', () => {
  it('renders the background, copy, slots and scroll link', () => {
    const wrapper = mountWithPlugins(LandingHero, {
      props: {
        image: '/hero.webp',
        imageSm: '/hero-960.webp',
        eyebrowText: 'Talents',
        subtitle: 'Find your next job',
        scrollHref: '#next',
        magnifierVars: { '--mag-top': '10%' },
      },
      slots: { heading: 'Work <em>better</em>', default: '<a class="extra">Apply</a>' },
    });

    expect(wrapper.find('.hero-bg__img').attributes('src')).toBe('/hero.webp');
    expect(wrapper.find('.landing-hero__eyebrow').text()).toBe('Talents');
    expect(wrapper.find('.landing-hero__heading').html()).toContain('<em>better</em>');
    expect(wrapper.find('.landing-hero__subtitle').text()).toBe('Find your next job');
    expect(wrapper.find('.extra').exists()).toBe(true);
    expect(wrapper.find('.scroll-indicator').attributes('href')).toBe('#next');
    expect(wrapper.attributes('style')).toContain('--mag-top: 10%');
  });
});
