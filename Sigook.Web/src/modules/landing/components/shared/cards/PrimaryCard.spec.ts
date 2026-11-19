import { describe, expect, it } from 'vitest';
import PrimaryCard from '@/modules/landing/components/shared/cards/PrimaryCard.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('PrimaryCard', () => {
  it('renders the default variant with only the provided parts', () => {
    const wrapper = mountWithPlugins(PrimaryCard);

    expect(wrapper.classes()).toContain('primary-card--navy');
    expect(wrapper.attributes('style')).toContain('transition-delay: 0ms');
    expect(wrapper.find('.primary-card__title').exists()).toBe(false);
    expect(wrapper.find('.primary-card__list').exists()).toBe(false);
  });

  it('renders the title, eyebrow, list, divider and slots', () => {
    const wrapper = mountWithPlugins(PrimaryCard, {
      props: { variant: 'red', eyebrow: 'Talent', title: 'Find work', list: ['Weekly pay', 'Benefits'], showDivider: true, delay: 200 },
      slots: { icon: '<i class="icon-slot" />', default: 'Body text', button: '<button class="cta">Go</button>' },
    });

    expect(wrapper.classes()).toContain('primary-card--red');
    expect(wrapper.attributes('style')).toContain('transition-delay: 200ms');
    expect(wrapper.text()).toContain('Talent');
    expect(wrapper.find('.primary-card__title').text()).toBe('Find work');
    expect(wrapper.findAll('.primary-card__list li').map((li) => li.text())).toEqual(['Weekly pay', 'Benefits']);
    expect(wrapper.find('.icon-slot').exists()).toBe(true);
    expect(wrapper.text()).toContain('Body text');
    expect(wrapper.find('.cta').exists()).toBe(true);
  });
});
