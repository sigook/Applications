import { describe, expect, it } from 'vitest';
import Footer from '@/modules/landing/layout/Footer.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('Footer', () => {
  it('renders the navigation columns, contact and legal links', () => {
    const wrapper = mountWithPlugins(Footer);

    expect(wrapper.findAll('nav.footer__col').map((col) => col.attributes('aria-label'))).toEqual(['Company', 'Services']);
    expect(wrapper.findAll('nav.footer__col a').map((a) => a.attributes('href'))).toEqual([
      '/about', '/open-positions', '/industries', '/special-projects', '/partner',
      '/employers', '/payroll', '/talents', '/certified',
    ]);
    expect(wrapper.find('a[href="mailto:info@sigook.com"]').exists()).toBe(true);
    expect(wrapper.findAll('.footer__legal-link').map((a) => a.text())).toEqual(['Privacy Policy', 'Terms and Conditions', 'Disclaimer']);
    expect(wrapper.find('.social-links').exists()).toBe(true);
  });

  it('shows the current year', () => {
    expect(mountWithPlugins(Footer).find('.footer__copyright').text()).toContain(String(new Date().getFullYear()));
  });
});
