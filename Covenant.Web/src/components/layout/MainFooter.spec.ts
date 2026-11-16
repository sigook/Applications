import { describe, expect, it } from 'vitest'
import { defineComponent } from 'vue'
import { mountAt } from '@/testing/mountWithPlugins'
import MainFooter from './MainFooter.vue'

const Blank = defineComponent({ render: () => null })
const routes = [
  { path: '/talents', name: 'talents', component: Blank },
  { path: '/open-positions', name: 'open_positions', component: Blank },
  { path: '/about', name: 'about', component: Blank },
]

describe('MainFooter', () => {
  it.each(['/talents', '/open-positions'])('uses the blue panel on %s', async (path) => {
    const { wrapper } = await mountAt(MainFooter, path, {}, routes)

    expect(wrapper.find('.footer__panel').classes()).toContain('footer__panel--blue')
  })

  it('uses the default panel elsewhere', async () => {
    const { wrapper } = await mountAt(MainFooter, '/about', {}, routes)

    expect(wrapper.find('.footer__panel').classes()).not.toContain('footer__panel--blue')
  })
})
