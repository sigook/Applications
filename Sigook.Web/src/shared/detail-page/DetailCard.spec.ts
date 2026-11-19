import { describe, expect, it } from 'vitest';
import DetailCard from '@/shared/detail-page/DetailCard.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('DetailCard', () => {
  it('renders the title and the body', () => {
    const wrapper = mountWithPlugins(DetailCard, { props: { title: 'Contact' }, slots: { default: '<p class="body">Body</p>' } });

    expect(wrapper.find('.detail-card-title').text()).toBe('Contact');
    expect(wrapper.find('.body').text()).toBe('Body');
    expect(wrapper.find('.detail-card-actions').exists()).toBe(false);
  });

  it('omits the header without title, actions or badge', () => {
    const wrapper = mountWithPlugins(DetailCard, { slots: { default: 'Body' } });
    expect(wrapper.find('.detail-card-header').exists()).toBe(false);
  });

  it('renders actions and badge without a title', () => {
    const wrapper = mountWithPlugins(DetailCard, {
      slots: { actions: '<button class="act">Edit</button>', badge: '<span class="badge">3</span>' },
    });

    expect(wrapper.find('.detail-card-title').exists()).toBe(false);
    expect(wrapper.find('.detail-card-actions .act').text()).toBe('Edit');
    expect(wrapper.find('.detail-card-header .badge').text()).toBe('3');
  });

  it('marks internal cards', () => {
    expect(mountWithPlugins(DetailCard, { props: { internal: true } }).classes()).toContain('is-internal');
    expect(mountWithPlugins(DetailCard).classes()).not.toContain('is-internal');
  });
});
