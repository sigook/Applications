import { describe, expect, it } from 'vitest';
import ProfileCard from '@/shared/worker-profile/cards/ProfileCard.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('ProfileCard', () => {
  it('renders the title and the body slot', () => {
    const wrapper = mountWithPlugins(ProfileCard, { props: { title: 'Skills' }, slots: { default: '<p class="body">Body</p>' } });

    expect(wrapper.find('.profile-card-title').text()).toBe('Skills');
    expect(wrapper.find('.body').exists()).toBe(true);
    expect(wrapper.find('.profile-card-actions').exists()).toBe(false);
  });

  it('renders the actions slot when given', () => {
    const wrapper = mountWithPlugins(ProfileCard, { props: { title: 'Skills' }, slots: { actions: '<button>Edit</button>' } });

    expect(wrapper.find('.profile-card-actions button').text()).toBe('Edit');
  });
});
