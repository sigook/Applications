import { describe, expect, it } from 'vitest';
import SecondaryCard from '@/modules/landing/components/shared/cards/SecondaryCard.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('SecondaryCard', () => {
  it('renders the default variant', () => {
    const wrapper = mountWithPlugins(SecondaryCard);

    expect(wrapper.classes()).toContain('secondary-card--cyan');
    expect(wrapper.classes()).not.toContain('secondary-card--expanded');
    expect(wrapper.find('.secondary-card__title').exists()).toBe(false);
    expect(wrapper.find('.secondary-card__red-veil').exists()).toBe(false);
  });

  it('renders all parts and the expanded state', () => {
    const wrapper = mountWithPlugins(SecondaryCard, {
      props: { variant: 'red', eyebrow: 'Option 1', title: 'Temp to perm', list: ['Flexible'], delay: 160, expanded: true },
      slots: { icon: '<i class="icon-slot" />', default: 'Body', expanded: '<p class="more">More</p>', button: '<button class="cta">Go</button>' },
    });

    expect(wrapper.classes()).toEqual(expect.arrayContaining(['secondary-card--red', 'secondary-card--expanded']));
    expect(wrapper.attributes('style')).toContain('transition-delay: 160ms');
    expect(wrapper.text()).toContain('Option 1');
    expect(wrapper.find('.secondary-card__title').text()).toBe('Temp to perm');
    expect(wrapper.findAll('.secondary-card__list li').map((li) => li.text())).toEqual(['Flexible']);
    expect(wrapper.find('.secondary-card__red-veil').exists()).toBe(true);
    expect(wrapper.find('.more').exists()).toBe(true);
    expect(wrapper.find('.cta').exists()).toBe(true);
  });
});
