import { describe, expect, it } from 'vitest'
import { mountWithPlugins } from '@/testing/mountWithPlugins'
import { scrollTarget } from '@/testing/scrollTarget'
import IndustriesCTA from './IndustriesCTA.vue'

describe('IndustriesCTA', () => {
  it('scrolls to the contact section', async () => {
    const scroll = scrollTarget('contact-section')
    const wrapper = mountWithPlugins(IndustriesCTA)

    await wrapper.find('.industries-cta__btn').trigger('click')

    expect(scroll).toHaveBeenCalledWith({ behavior: 'smooth' })
  })

  it('does nothing without a contact section', async () => {
    const wrapper = mountWithPlugins(IndustriesCTA)

    await wrapper.find('.industries-cta__btn').trigger('click')

    expect(wrapper.find('.industries-cta__btn').exists()).toBe(true)
  })
})
