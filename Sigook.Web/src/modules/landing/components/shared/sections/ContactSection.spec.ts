import { describe, expect, it, vi } from 'vitest';
import ContactSection from '@/modules/landing/components/shared/sections/ContactSection.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

vi.mock('@/modules/landing/components/shared/forms/GetInTouchForm.vue', () => ({
  default: { name: 'GetInTouchForm', template: '<form class="get-in-touch-stub" />' },
}));

describe('ContactSection', () => {
  it('renders the title and the contact form', () => {
    const wrapper = mountWithPlugins(ContactSection);

    expect(wrapper.find('.contact__title').text()).toBe('Get in Touch!');
    expect(wrapper.find('.get-in-touch-stub').exists()).toBe(true);
  });
});
