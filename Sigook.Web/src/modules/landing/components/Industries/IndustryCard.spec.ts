import { describe, expect, it } from 'vitest';
import IndustryCard, { type Industry } from '@/modules/landing/components/Industries/IndustryCard.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

const industry: Industry = {
  key: 'logistics',
  title: 'Logistics',
  tagline: 'Moving goods',
  body: 'Warehouses and distribution',
  positions: ['Pickers', 'Drivers'],
  photo: 'https://img.test/logistics.jpg',
  tone: 'red',
};

describe('IndustryCard', () => {
  it('renders the collapsed card', () => {
    const wrapper = mountWithPlugins(IndustryCard, { props: { industry, expanded: false, index: 2 } });

    expect(wrapper.classes()).toContain('industry-card--red');
    expect(wrapper.classes()).not.toContain('industry-card--expanded');
    expect(wrapper.find('.industry-card__index').text()).toBe('03');
    expect(wrapper.find('.industry-card__title').text()).toBe('Logistics');
    expect(wrapper.findAll('.industry-card__positions li').map((li) => li.text())).toEqual(['Pickers', 'Drivers']);
    expect(wrapper.find('.industry-card__photo').attributes('style')).toContain('https://img.test/logistics.jpg');
    expect(wrapper.find('.industry-card__toggle').text()).toBe('View details+');
    expect(wrapper.find('.industry-card__toggle').attributes('aria-expanded')).toBe('false');
  });

  it('renders the expanded card without photo', () => {
    const wrapper = mountWithPlugins(IndustryCard, { props: { industry: { ...industry, photo: undefined }, expanded: true, index: 11 } });

    expect(wrapper.classes()).toContain('industry-card--expanded');
    expect(wrapper.find('.industry-card__index').text()).toBe('12');
    expect(wrapper.find('.industry-card__photo').attributes('style')).toContain('background-image: none');
    expect(wrapper.find('.industry-card__toggle').text()).toBe('Show less−');
  });

  it('asks to toggle', async () => {
    const wrapper = mountWithPlugins(IndustryCard, { props: { industry, expanded: false, index: 0 } });
    await wrapper.find('.industry-card__toggle').trigger('click');

    expect(wrapper.emitted('toggle')).toHaveLength(1);
  });
});
