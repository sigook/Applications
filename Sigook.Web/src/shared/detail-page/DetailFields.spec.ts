import { describe, expect, it, vi } from 'vitest';
import { flushPromises } from '@vue/test-utils';
import DetailFields from '@/shared/detail-page/DetailFields.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';
import type { DetailFact } from '@/shared/detail-page/types';

function setup(fields: DetailFact[], columns?: 2 | 3) {
  return mountWithPlugins(DetailFields, { props: { fields, columns } });
}

describe('DetailFields', () => {
  it('renders plain label/value pairs', () => {
    const wrapper = setup([{ label: 'Name', value: 'Acme' }, { label: 'City', value: 'Toronto' }]);

    expect(wrapper.findAll('dt').map((d) => d.text())).toEqual(['Name', 'City']);
    expect(wrapper.findAll('dd').map((d) => d.text())).toEqual(['Acme', 'Toronto']);
    expect(wrapper.find('a').exists()).toBe(false);
    expect(wrapper.find('button').exists()).toBe(false);
  });

  it('uses two columns only when asked', () => {
    expect(setup([], 2).find('dl').classes()).toContain('is-two');
    expect(setup([], 3).find('dl').classes()).not.toContain('is-two');
  });

  it('opens external links in a new tab', () => {
    const link = setup([{ label: 'Web', value: 'acme.com', href: 'https://acme.com' }]).find('a');

    expect(link.text()).toBe('acme.com');
    expect(link.attributes('href')).toBe('https://acme.com');
    expect(link.attributes('target')).toBe('_blank');
    expect(link.attributes('rel')).toBe('noopener noreferrer');
  });

  it('renders a copy link for contact values', () => {
    const wrapper = setup([{ label: 'Phone', value: '416', copy: { kind: 'phone', value: '4165550199', ext: 12 } }]);

    expect(wrapper.find('.copy-link-text').attributes('href')).toBe('tel:4165550199');
    expect(wrapper.find('.copy-link-text').text()).toBe('4165550199 Ext. 12');
  });

  it('falls back to the value when the copy value is empty', () => {
    const wrapper = setup([{ label: 'Email', value: '-', copy: { kind: 'email', value: null } }]);

    expect(wrapper.find('.copy-link').exists()).toBe(false);
    expect(wrapper.find('dd').text()).toBe('-');
  });

  it('renders a router link with the default label', async () => {
    const wrapper = setup([{ label: 'Owner', value: 'Ann', to: '/owners/1' }]);
    await flushPromises();
    const link = wrapper.find('a.detail-field-link');

    expect(link.attributes('href')).toBe('/owners/1');
    expect(link.text()).toBe('Edit');
  });

  it('runs the action with a custom label', async () => {
    const onAction = vi.fn();
    const wrapper = setup([{ label: 'Rate', value: '$20', onAction, linkLabel: 'Change' }]);
    const button = wrapper.find('button.detail-field-link');

    expect(button.text()).toBe('Change');
    await button.trigger('click');
    expect(onAction).toHaveBeenCalledOnce();
  });
});
