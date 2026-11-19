import { describe, expect, it } from 'vitest';
import SolutionsSection, { type SolutionOption } from '@/modules/landing/components/shared/sections/SolutionsSection.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

const options: SolutionOption[] = [
  { key: 'temporary', eyebrow: 'Option 1', title: 'Temporary', body: 'Short term help', benefits: ['Fast'], ctaLabel: 'Hire temp', ctaTo: '/employers', variant: 'cyan' },
  { key: 'temp-to-perm', eyebrow: 'Option 2', title: 'Temp to perm', benefits: ['Try first'], ctaLabel: 'Unused', ctaTo: '/unused', variant: 'blue' },
  { key: 'direct', eyebrow: 'Option 3', title: 'Direct hire', benefits: ['Permanent'], ctaLabel: 'Hire direct', ctaTo: '/partner', variant: 'red' },
];

function setup() {
  return mountWithPlugins(SolutionsSection, {
    props: { sectionId: 'solutions', eyebrow: 'Solutions', heading: 'Three ways', headingAccent: 'One fit.', subtitle: 'Pick one', options },
  });
}

describe('SolutionsSection', () => {
  it('renders one card per option with its call to action', () => {
    const wrapper = setup();

    expect(wrapper.attributes('id')).toBe('solutions');
    expect(wrapper.findAll('.secondary-card__title').map((t) => t.text())).toEqual(['Temporary', 'Temp to perm', 'Direct hire']);
    expect(wrapper.findAll('a.arrow-pill').map((a) => [a.find('.arrow-pill__label').text(), a.attributes('href')])).toEqual([['Hire temp', '/employers'], ['Hire direct', '/partner']]);
    expect(wrapper.find('a.arrow-pill.arrow-pill--hover-red').exists()).toBe(true);
    expect(wrapper.text()).toContain('Short term help');
  });

  it('expands the temp to perm details and dims the rest', async () => {
    const wrapper = setup();
    const toggle = wrapper.find('button.arrow-pill');
    expect(toggle.text()).toBe('View details');

    await toggle.trigger('click');
    const cards = wrapper.findAll('.solutions-section__card');
    expect(toggle.text()).toBe('Show less');
    expect(wrapper.find('.solutions-section__cards').classes()).toContain('solutions-section__cards--detail');
    expect(cards[1].classes()).toEqual(expect.arrayContaining(['secondary-card--red', 'secondary-card--expanded', 'solutions-section__card--popped']));
    expect(cards[0].classes()).toContain('solutions-section__card--dimmed');
    expect(cards[1].find('.ttp-detail__result').exists()).toBe(true);

    await toggle.trigger('click');
    expect(wrapper.findAll('.solutions-section__card')[1].classes()).toContain('secondary-card--blue');
  });
});
