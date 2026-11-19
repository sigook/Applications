import { describe, expect, it } from 'vitest';
import DashboardCard from '@/modules/agency/sales/dashboard/components/DashboardCard.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

type Props = {
  title: string;
  titleTo?: string;
  subtitle?: string;
  icon?: string;
  tone?: 'primary' | 'green' | 'accent';
  actionIcon?: string;
  actionLabel?: string;
};

function setup(props: Props, slots: Record<string, string> = {}) {
  return mountWithPlugins(DashboardCard, { props, slots: { default: '<p class="body">Body</p>', ...slots } });
}

describe('DashboardCard', () => {
  it('renders a plain title with only the body', () => {
    const wrapper = setup({ title: 'Deals by status' });

    expect(wrapper.find('p.sd-card__title').text()).toBe('Deals by status');
    expect(wrapper.find('.sd-card__subtitle').exists()).toBe(false);
    expect(wrapper.find('.sd-card__chip').exists()).toBe(false);
    expect(wrapper.find('.sd-card__actions').exists()).toBe(false);
    expect(wrapper.find('.sd-card__body .body').exists()).toBe(true);
  });

  it('links the title, shows icon and subtitle with the tone', () => {
    const wrapper = setup({ title: 'Clients', titleTo: '/sales/companies', subtitle: 'Last 10 contacted', icon: 'domain', tone: 'green' });

    expect(wrapper.find('a.sd-card__title').attributes('href')).toBe('/sales/companies');
    expect(wrapper.find('.sd-card__subtitle').text()).toBe('Last 10 contacted');
    expect(wrapper.find('.sd-card__chip').classes()).toContain('sd-tone-green');
  });

  it('emits action from the default action button', async () => {
    const wrapper = setup({ title: 'Deals', actionIcon: 'plus', actionLabel: 'Create deal' });
    const button = wrapper.find('button.sd-card__action');

    expect(button.attributes('aria-label')).toBe('Create deal');
    expect(button.classes()).toContain('sd-tone-primary');
    await button.trigger('click');
    expect(wrapper.emitted('action')).toHaveLength(1);
  });

  it('uses the subtitle and actions slots', () => {
    const wrapper = setup({ title: 'Deals' }, { subtitle: 'Total <b>3</b>', actions: '<span class="custom">Range</span>' });

    expect(wrapper.find('.sd-card__subtitle').text()).toBe('Total 3');
    expect(wrapper.find('.sd-card__actions .custom').exists()).toBe(true);
    expect(wrapper.find('button.sd-card__action').exists()).toBe(false);
  });
});
