import { describe, expect, it } from 'vitest'
import { mountWithPlugins } from '@/testing/mountWithPlugins'
import RolesWeRecruit from './RolesWeRecruit.vue'

describe('RolesWeRecruit', () => {
  it('flips one card at a time and closes it on a second click', async () => {
    const wrapper = mountWithPlugins(RolesWeRecruit)
    const cards = wrapper.findAll('.role-card')

    await cards[0].trigger('click')
    expect(cards[0].classes()).toContain('is-active')

    await cards[1].trigger('click')
    expect(cards[0].classes()).not.toContain('is-active')
    expect(cards[1].classes()).toContain('is-active')

    await cards[1].trigger('click')
    expect(wrapper.findAll('.role-card.is-active')).toHaveLength(0)
  })
})
