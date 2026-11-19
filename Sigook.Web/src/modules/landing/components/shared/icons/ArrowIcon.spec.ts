import { describe, expect, it } from 'vitest';
import ArrowIcon from '@/modules/landing/components/shared/icons/ArrowIcon.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('ArrowIcon', () => {
  it('renders with the default size and color', () => {
    const svg = mountWithPlugins(ArrowIcon);

    expect(svg.attributes('width')).toBe('52');
    expect(svg.attributes('height')).toBe('20');
    expect(svg.html()).toContain('white');
  });

  it('accepts a custom size, color and stroke', () => {
    const svg = mountWithPlugins(ArrowIcon, { props: { width: 32, height: 11, color: '#fff', strokeWidth: 1.5 } });

    expect(svg.attributes('width')).toBe('32');
    expect(svg.attributes('height')).toBe('11');
    expect(svg.html()).toContain('#fff');
    expect(svg.html()).toContain('1.5');
  });
});
