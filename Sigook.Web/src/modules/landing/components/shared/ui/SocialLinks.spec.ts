import { describe, expect, it } from 'vitest';
import SocialLinks from '@/modules/landing/components/shared/ui/SocialLinks.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('SocialLinks', () => {
  it('links to every social network in a new tab', () => {
    const links = mountWithPlugins(SocialLinks).findAll('a');

    expect(links.map((a) => a.attributes('aria-label'))).toEqual(['Sigook on Instagram', 'Sigook on Facebook', 'Sigook on LinkedIn']);
    expect(links.every((a) => a.attributes('target') === '_blank' && a.attributes('rel') === 'noopener noreferrer')).toBe(true);
    expect(links.every((a) => a.find('svg').exists())).toBe(true);
  });
});
