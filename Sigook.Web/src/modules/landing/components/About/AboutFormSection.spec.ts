import { describe, expect, it, vi } from 'vitest';
import AboutFormSection from '@/modules/landing/components/About/AboutFormSection.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/modules/landing/components/shared/sections/ContactSection.vue', () => ({
  default: { name: 'ContactSection', template: '<section class="contact-stub" />' },
}));

describe('AboutFormSection', () => {
  it('renders the contact section and the social links', () => {
    const wrapper = mountWithPlugins(AboutFormSection);

    expect(wrapper.attributes('id')).toBe('about-form');
    expect(wrapper.find('.contact-stub').exists()).toBe(true);
    expect(wrapper.find('.about-form__connect-label').text()).toBe('Connect with us');
    expect(wrapper.findAll('.social-links a')).toHaveLength(3);
  });
});
