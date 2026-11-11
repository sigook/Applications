import { describe, expect, it } from 'vitest'
import { mountWithPlugins } from '@/testing/mountWithPlugins'
import RolesTalents from './RolesTalents.vue'

describe('RolesTalents', () => {
  it('flips a card and toggles it back', async () => {
    const wrapper = mountWithPlugins(RolesTalents)
    const card = wrapper.findAll('.role-card')[0]

    await card.trigger('click')
    expect(card.classes()).toContain('is-active')

    await card.trigger('click')
    expect(card.classes()).not.toContain('is-active')
  })

  it('opens the detail face and closes everything from it', async () => {
    const wrapper = mountWithPlugins(RolesTalents)
    const card = wrapper.findAll('.role-card')[0]
    await card.trigger('click')

    await card.find('.plus-btn').trigger('click')
    expect(card.classes()).toEqual(expect.arrayContaining(['is-active', 'is-detailed']))

    await card.trigger('click')
    expect(card.classes()).not.toContain('is-active')
    expect(card.classes()).not.toContain('is-detailed')
  })
})
