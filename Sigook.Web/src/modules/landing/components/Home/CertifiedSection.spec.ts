import { describe, expect, it } from 'vitest';
import CertifiedSection from '@/modules/landing/components/Home/CertifiedSection.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('CertifiedSection', () => {
  it('renders the promise and links to the contact form', () => {
    const wrapper = mountWithPlugins(CertifiedSection);

    expect(wrapper.find('.certified__eyebrow').text()).toBe('Our Promise');
    expect(wrapper.find('a.arrow-pill').attributes('href')).toBe('/#home-contact');
  });
});
