import { mount, type ComponentMountingOptions } from '@vue/test-utils'
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import { createHead } from '@unhead/vue/client'
import { createMemoryHistory, createRouter, type Router, type RouteRecordRaw } from 'vue-router'
import { defineComponent, type Component } from 'vue'

const Blank = defineComponent({ render: () => null })

export function createTestRouter(routes: RouteRecordRaw[] = []): Router {
  return createRouter({
    history: createMemoryHistory(),
    routes: [...routes, { path: '/:pathMatch(.*)*', component: Blank }],
  })
}

export function mountWithPlugins<C extends Component>(
  component: C,
  options: ComponentMountingOptions<C> = {},
  router: Router = createTestRouter(),
) {
  const vuetify = createVuetify({ components, directives })
  const head = createHead()
  const global = options.global ?? {}
  return mount(component, {
    attachTo: document.body,
    ...options,
    global: {
      ...global,
      plugins: [vuetify, router, head, ...(global.plugins ?? [])],
    },
  })
}

export async function mountAt<C extends Component>(
  component: C,
  path: string,
  options: ComponentMountingOptions<C> = {},
  routes: RouteRecordRaw[] = [],
) {
  const router = createTestRouter(routes)
  await router.push(path)
  await router.isReady()
  const wrapper = mountWithPlugins(component, options, router)
  return { wrapper, router }
}
