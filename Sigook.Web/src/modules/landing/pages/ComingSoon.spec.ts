import { describe, expect, it } from 'vitest';
import { mount } from '@vue/test-utils';
import Buefy from 'buefy';
import { createMemoryHistory, createRouter } from 'vue-router';
import ComingSoon from '@/modules/landing/pages/ComingSoon.vue';

async function setup(path: string) {
  const router = createRouter({
    history: createMemoryHistory(),
    routes: [
      { path: '/payroll', component: ComingSoon, meta: { title: 'Payroll Solutions' } },
      { path: '/:pathMatch(.*)*', component: ComingSoon },
    ],
  });
  await router.push(path);
  await router.isReady();
  return mount(ComingSoon, { attachTo: document.body, global: { plugins: [Buefy, router] } });
}

describe('ComingSoon', () => {
  it('shows the route title and a link home', async () => {
    const wrapper = await setup('/payroll');

    expect(wrapper.find('.coming__label').text()).toBe('Payroll Solutions');
    expect(wrapper.find('a').attributes('href')).toBe('/');
  });

  it('leaves the label empty without a title', async () => {
    expect((await setup('/other')).find('.coming__label').text()).toBe('');
  });
});
