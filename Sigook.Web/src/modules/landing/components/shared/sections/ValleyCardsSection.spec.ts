import { describe, expect, it } from 'vitest';
import ValleyCardsSection, { type ValleyReason } from '@/modules/landing/components/shared/sections/ValleyCardsSection.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

const reasons: ValleyReason[] = [
  { eyebrow: '01', title: 'Speed', variant: 'cyan', body: 'Shortlists in 48h' },
  { eyebrow: '02', title: 'Quality', variant: 'blue', bullets: ['Vetted', 'Trained'] },
  { eyebrow: '03', title: 'Care', variant: 'red', bullets: ['Support'] },
];

describe('ValleyCardsSection', () => {
  it('renders body and bullet cards with the middle one offset', () => {
    const wrapper = mountWithPlugins(ValleyCardsSection, {
      props: { eyebrow: 'Why us', heading: 'Reasons', headingAccent: 'Three of them.', subtitle: 'Read on', reasons },
    });
    const cards = wrapper.findAll('.secondary-card');

    expect(wrapper.find('.section-header__accent').text()).toBe('Three of them.');
    expect(cards.map((c) => c.find('.secondary-card__title').text())).toEqual(['Speed', 'Quality', 'Care']);
    expect(cards[0].text()).toContain('Shortlists in 48h');
    expect(cards[1].findAll('li').map((li) => li.text())).toEqual(['Vetted', 'Trained']);
    expect(cards.map((c) => c.classes().includes('valley-cards__card--offset'))).toEqual([false, true, false]);
  });
});
