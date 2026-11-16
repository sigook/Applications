import { describe, expect, it } from 'vitest';
import Breadcrumbs from '@/shared/ui/Breadcrumbs.vue';
import { useMobileBack } from '@/shared/composables/useMobileBack';
import { invoicesCrumbs } from '@/shared/ui/breadcrumbs';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('Breadcrumbs', () => {
  it('renders nothing without crumbs or back link', () => {
    const wrapper = mountWithPlugins(Breadcrumbs);
    expect(wrapper.find('nav').exists()).toBe(false);
  });

  it('renders links for crumbs with a route and text for the rest', () => {
    const wrapper = mountWithPlugins(Breadcrumbs, { props: { crumbs: invoicesCrumbs } });

    expect(wrapper.find('span').text()).toBe('Accounting');
    const link = wrapper.find('a');
    expect(link.text()).toBe('Invoices');
    expect(link.attributes('href')).toBe('/accounting/invoices');
    expect(wrapper.findAll('.page-header-separator')).toHaveLength(1);
  });

  it('shows a back link and registers it for the mobile top bar', () => {
    const back = useMobileBack();
    const wrapper = mountWithPlugins(Breadcrumbs, { props: { backTo: '/requests' } });

    expect(wrapper.find('a.page-header-back').attributes('href')).toBe('/requests');
    expect(back.value).toBe('/requests');

    wrapper.unmount();
    expect(back.value).toBeNull();
  });
});
