import { describe, expect, it } from 'vitest';
import LandingSectionHeader from '@/modules/landing/components/shared/sections/LandingSectionHeader.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('LandingSectionHeader', () => {
  it('renders the eyebrow and heading with the default widths', () => {
    const wrapper = mountWithPlugins(LandingSectionHeader, { props: { eyebrow: 'About', heading: 'Who we are' } });

    expect(wrapper.find('.section-header__eyebrow').text()).toBe('About');
    expect(wrapper.find('.section-header__heading').text()).toBe('Who we are');
    expect(wrapper.find('.section-header__heading').attributes('style')).toContain('max-width: 780px');
    expect(wrapper.find('.section-header__accent').exists()).toBe(false);
    expect(wrapper.find('.section-header__subtitle').exists()).toBe(false);
  });

  it('renders the accent, subtitle and style overrides', () => {
    const wrapper = mountWithPlugins(LandingSectionHeader, {
      props: {
        eyebrow: 'About',
        heading: 'Who we are',
        headingAccent: 'Since 2008.',
        subtitle: 'A short story',
        eyebrowVariant: 'red',
        headingMaxWidth: '600px',
        subtitleMaxWidth: '400px',
        marginBottom: '20px',
        headingSize: '40px',
        subtitleColor: 'red',
      },
    });

    expect(wrapper.find('.section-header__accent').text()).toBe('Since 2008.');
    expect(wrapper.find('.section-header__subtitle').text()).toBe('A short story');
    expect(wrapper.attributes('style')).toContain('margin-bottom: 20px');
    expect(wrapper.find('.section-header__heading').attributes('style')).toContain('max-width: 600px');
    expect(wrapper.find('.section-header__heading').attributes('style')).toContain('font-size: 40px');
    expect(wrapper.find('.section-header__subtitle').attributes('style')).toContain('max-width: 400px');
    expect(wrapper.find('.section-header__subtitle').attributes('style')).toContain('color: red');
  });
});
