import { describe, expect, it } from 'vitest';
import ScrollIndicator from '@/modules/landing/components/shared/hero/ScrollIndicator.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('ScrollIndicator', () => {
  it('uses the default target and label', () => {
    const link = mountWithPlugins(ScrollIndicator);

    expect(link.attributes('href')).toBe('#');
    expect(link.attributes('aria-label')).toBe('Scroll to next section');
    expect(link.find('.scroll-indicator__lbl').text()).toBe('Scroll');
  });

  it('accepts a target, label and custom text', () => {
    const link = mountWithPlugins(ScrollIndicator, { props: { href: '#jobs', ariaLabel: 'See jobs' }, slots: { default: 'Jobs' } });

    expect(link.attributes('href')).toBe('#jobs');
    expect(link.attributes('aria-label')).toBe('See jobs');
    expect(link.find('.scroll-indicator__lbl').text()).toBe('Jobs');
  });
});
