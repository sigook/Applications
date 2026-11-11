import { mount, type ComponentMountingOptions } from '@vue/test-utils'
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import { createMemoryHistory, createRouter, type Router } from 'vue-router'
import { defineComponent, type Component } from 'vue'

const Blank = defineComponent({ render: () => null })

export function createTestRouter(): Router {
  return createRouter({
    history: createMemoryHistory(),
    routes: [{ path: '/:pathMatch(.*)*', component: Blank }],
  })
}

export function mountWithPlugins<C extends Component>(
  component: C,
  options: ComponentMountingOptions<C> = {},
) {
  const vuetify = createVuetify({ components, directives })
  const router = createTestRouter()
  const global = options.global ?? {}
  return mount(component, {
    attachTo: document.body,
    ...options,
    global: {
      ...global,
      plugins: [vuetify, router, ...(global.plugins ?? [])],
    },
  })
}
