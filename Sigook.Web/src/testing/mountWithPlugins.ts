import { mount, type ComponentMountingOptions } from '@vue/test-utils';
import Buefy from 'buefy';
import { createMemoryHistory, createRouter, type Router } from 'vue-router';
import { defineComponent, type Component } from 'vue';

const Blank = defineComponent({ render: () => null });

export function createTestRouter(): Router {
  return createRouter({
    history: createMemoryHistory(),
    routes: [{ path: '/:pathMatch(.*)*', component: Blank }],
  });
}

export function mountWithPlugins<C extends Component>(
  component: C,
  options: ComponentMountingOptions<C> = {},
) {
  const router = createTestRouter();
  const global = options.global ?? {};
  return mount(component, {
    attachTo: document.body,
    ...options,
    global: {
      ...global,
      plugins: [Buefy, router, ...(global.plugins ?? [])],
    },
  });
}
