import { describe, expect, it } from 'vitest';
import PageHeader from '@/shared/ui/PageHeader.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('PageHeader', () => {
  it('renders the title without count or actions', () => {
    const wrapper = mountWithPlugins(PageHeader, { props: { title: 'Requests' } });

    expect(wrapper.find('.page-header-title').text()).toBe('Requests');
    expect(wrapper.find('.page-header-count').exists()).toBe(false);
    expect(wrapper.find('.page-header-actions').exists()).toBe(false);
    expect(wrapper.classes()).not.toContain('has-actions');
  });

  it('shows the count, including zero', () => {
    const wrapper = mountWithPlugins(PageHeader, { props: { title: 'Requests', count: 0 } });
    expect(wrapper.find('.page-header-count').text()).toBe('0');
  });

  it('renders crumbs before the title', () => {
    const wrapper = mountWithPlugins(PageHeader, {
      props: { title: 'Invoice 12', crumbs: [{ label: 'Invoices', to: '/accounting/invoices' }] },
    });

    expect(wrapper.find('.page-header-breadcrumb a').text()).toBe('Invoices');
    expect(wrapper.find('.page-header-heading > .page-header-separator').exists()).toBe(true);
  });

  it('renders actions from the default slot', () => {
    const wrapper = mountWithPlugins(PageHeader, {
      props: { title: 'Requests' },
      slots: { default: '<button>New</button>' },
    });

    expect(wrapper.classes()).toContain('has-actions');
    expect(wrapper.find('.page-header-actions button').text()).toBe('New');
  });
});
