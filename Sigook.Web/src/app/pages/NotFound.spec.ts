import { describe, expect, it } from 'vitest';
import NotFound from '@/app/pages/NotFound.vue';
import { mountWithPlugins } from '@/testing/mountWithPlugins';

describe('NotFound', () => {
  it('shows the 404 message', () => {
    const wrapper = mountWithPlugins(NotFound);

    expect(wrapper.find('h2').text()).toBe('404');
    expect(wrapper.find('p').text()).toBe('The page you’re looking for can’t be found.');
  });
});
