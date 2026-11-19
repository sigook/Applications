import { describe, expect, it } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import DetailHeader from '@/shared/detail-page/DetailHeader.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { DetailKpi } from '@/shared/detail-page/types';

type Props = InstanceType<typeof DetailHeader>['$props'];

function setup(props: Partial<Props> = {}, slots: Record<string, string> = {}) {
  return mountWithPlugins(DetailHeader, { props: { title: 'Forklift Operator', kpis: [], ...props }, slots });
}

const kpis: DetailKpi[] = [{ key: 'hours', label: 'Hours', value: '40' }];

describe('DetailHeader', () => {
  it('renders the title and the body', () => {
    const wrapper = setup({}, { default: '<p class="body">Body</p>' });

    expect(wrapper.find('h1').text()).toBe('Forklift Operator');
    expect(wrapper.find('h1').attributes('title')).toBe('Forklift Operator');
    expect(wrapper.find('.detail-body .body').exists()).toBe(true);
    expect(wrapper.find('.detail-header-meta').exists()).toBe(false);
    expect(wrapper.find('.detail-kpis').exists()).toBe(false);
  });

  it('shows initials of the organization, or of the title without one', () => {
    expect(setup({ orgName: 'acme  logistics inc' }).find('.detail-header-logo').text()).toBe('AL');
    expect(setup().find('.detail-header-logo').text()).toBe('FO');
  });

  it('shows the logo and falls back to initials when it fails to load', async () => {
    const wrapper = setup({ logo: 'https://files/logo.png', orgName: 'Acme' });
    const img = wrapper.find('.detail-header-logo img');
    expect(img.attributes('src')).toBe('https://files/logo.png');

    await img.trigger('error');
    expect(wrapper.find('.detail-header-logo img').exists()).toBe(false);
    expect(wrapper.find('.detail-header-logo').text()).toBe('A');
  });

  it('shows the logo icon when there is no logo', () => {
    const wrapper = setup({ logoIcon: 'account' });
    expect(wrapper.find('.detail-header-logo .mdi-account').exists()).toBe(true);
  });

  it('links the logo and organization name to the organization', async () => {
    const wrapper = setup({ orgName: 'Acme', orgLink: '/clients/1' });
    await flushPromises();

    expect(wrapper.find('a.detail-header-logo').attributes('href')).toBe('/clients/1');
    expect(wrapper.find('a.detail-header-logo').attributes('aria-label')).toBe('Acme');
    expect(wrapper.find('a.detail-header-org').text()).toBe('Acme');
  });

  it('shows the organization as text without a link', () => {
    const wrapper = setup({ orgName: 'Acme' });
    expect(wrapper.find('span.detail-header-org').text()).toBe('Acme');
  });

  it('emits editLogo from an editable logo', async () => {
    const wrapper = setup({ logoEditable: true, logo: 'https://files/logo.png' });
    await wrapper.find('button[aria-label="Change logo"]').trigger('click');
    expect(wrapper.emitted('editLogo')).toHaveLength(1);
  });

  it.each([
    [{ numberId: 12 }, '#12'],
    [{ meta: 'Day shift' }, 'Day shift'],
    [{ numberId: 0, meta: 'Day shift' }, '#0 · Day shift'],
  ])('renders the meta line %j', (props, expected) => {
    expect(setup(props).find('.detail-header-meta span').text()).toBe(expected);
  });

  it('renders the status, text chips, icon chips and meta chips', () => {
    const wrapper = setup({
      statusLabel: 'Open',
      statusVariant: 'is-success',
      chips: [{ label: 'ASAP', variant: 'is-danger' }, { label: 'Direct hire', icon: 'handshake' }],
      metaChips: [{ label: 'Night', icon: 'weather-night', variant: 'is-info' }, { label: 'Plain' }],
    });
    const chips = wrapper.findAll('.detail-header-chips .detail-chip');

    expect(chips[0].text()).toBe('Open');
    expect(chips[0].classes()).toContain('is-success');
    expect(chips[1].text()).toBe('ASAP');
    expect(chips[1].classes()).toContain('is-danger');
    expect(chips[2].classes()).toContain('is-icon');
    expect(chips[2].attributes('aria-label')).toBe('Direct hire');
    const meta = wrapper.findAll('.detail-chip.is-meta');
    expect(meta.map((m) => m.text())).toEqual(['Night', 'Plain']);
    expect(meta[0].classes()).toContain('is-info');
  });

  it('renders the alert, subtitle and actions slots', () => {
    const wrapper = setup({}, {
      alert: 'Deleted',
      subtitle: '<p class="sub">Sub</p>',
      actions: '<button class="act">Edit</button>',
    });

    expect(wrapper.find('.detail-alert').text()).toBe('Deleted');
    expect(wrapper.find('.sub').exists()).toBe(true);
    expect(wrapper.find('.detail-header-actions .act').exists()).toBe(true);
  });

  it('forwards kpi slots to the kpi strip', () => {
    const wrapper = setup({ kpis }, { 'kpi-hours': '<b class="custom">42h</b>' });

    expect(wrapper.find('.detail-kpis dt').text()).toBe('Hours');
    expect(wrapper.find('.detail-kpis .custom').text()).toBe('42h');
  });

  it('hides the header but keeps the body', () => {
    const wrapper = setup({ headerHidden: true, kpis }, { default: '<p class="body">Body</p>' });

    expect(wrapper.classes()).toContain('is-header-hidden');
    expect(wrapper.find('.detail-header').exists()).toBe(false);
    expect(wrapper.find('.body').exists()).toBe(true);
  });
});
