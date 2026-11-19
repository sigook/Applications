import { describe, expect, it } from 'vitest';
import DecoMagnifier from '@/modules/landing/components/shared/hero/DecoMagnifier.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('DecoMagnifier', () => {
  it('renders a decorative image', () => {
    const img = mountWithPlugins(DecoMagnifier);

    expect(img.element.tagName).toBe('IMG');
    expect(img.attributes('aria-hidden')).toBe('true');
    expect(img.attributes('alt')).toBe('');
  });
});
