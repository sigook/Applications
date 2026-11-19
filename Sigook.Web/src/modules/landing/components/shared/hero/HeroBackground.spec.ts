import { describe, expect, it } from 'vitest';
import HeroBackground from '@/modules/landing/components/shared/hero/HeroBackground.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('HeroBackground', () => {
  it('renders a responsive image with the focal point', () => {
    const img = mountWithPlugins(HeroBackground, { props: { image: '/a.webp', imageSm: '/a-960.webp', focal: 'center 35%' } }).find('img');

    expect(img.attributes('srcset')).toBe('/a-960.webp 960w, /a.webp 1920w');
    expect(img.attributes('style')).toContain('object-position: center 35%');
  });

  it('skips the srcset without a small image', () => {
    const img = mountWithPlugins(HeroBackground, { props: { image: '/a.webp' } }).find('img');

    expect(img.attributes('srcset')).toBeUndefined();
    expect(img.attributes('style')).toContain('object-position: center');
  });
});
