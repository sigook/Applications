import { describe, expect, it } from 'vitest';
import LandingIcon from '@/modules/landing/components/shared/icons/LandingIcon.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('LandingIcon', () => {
  it('renders a stroke icon with its own stroke width', () => {
    const svg = mountWithPlugins(LandingIcon, { props: { name: 'close' } });

    expect(svg.attributes()).toMatchObject({ viewBox: '0 0 24 24', fill: 'none', stroke: 'currentColor', 'stroke-width': '2', 'aria-hidden': 'true' });
    expect(svg.find('path').attributes('d')).toBe('M18 6 6 18M6 6l12 12');
  });

  it('uses the default stroke width or an override', () => {
    expect(mountWithPlugins(LandingIcon, { props: { name: 'automotive' } }).attributes('stroke-width')).toBe('1.5');
    expect(mountWithPlugins(LandingIcon, { props: { name: 'close', strokeWidth: 3 } }).attributes('stroke-width')).toBe('3');
  });

  it('renders a filled icon without stroke', () => {
    const svg = mountWithPlugins(LandingIcon, { props: { name: 'facebook' } });

    expect(svg.attributes('fill')).toBe('currentColor');
    expect(svg.attributes('stroke')).toBe('none');
  });
});
