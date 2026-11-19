import { describe, expect, it } from 'vitest';
import LabeledChipList from '@/modules/landing/components/shared/ui/LabeledChipList.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('LabeledChipList', () => {
  it('renders the label and chips', () => {
    const wrapper = mountWithPlugins(LabeledChipList, { props: { label: 'Roles', items: ['Picker', 'Packer'] } });

    expect(wrapper.find('.labeled-chip-list__label').text()).toBe('Roles');
    expect(wrapper.findAll('.labeled-chip-list__chip').map((c) => c.text())).toEqual(['Picker', 'Packer']);
  });

  it('links the more chip when it has a target', () => {
    const wrapper = mountWithPlugins(LabeledChipList, { props: { label: 'Roles', items: [], moreLabel: '+5 more', moreTo: '/industries' } });

    expect(wrapper.find('a.labeled-chip-list__chip--more').attributes('href')).toBe('/industries');
  });

  it('shows a plain more chip without a target', () => {
    const wrapper = mountWithPlugins(LabeledChipList, { props: { label: 'Roles', items: [], moreLabel: '+5 more' } });

    expect(wrapper.find('span.labeled-chip-list__chip--more').text()).toBe('+5 more');
    expect(wrapper.find('a').exists()).toBe(false);
  });
});
