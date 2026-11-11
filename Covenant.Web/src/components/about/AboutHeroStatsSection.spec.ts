import { describe, expect, it } from 'vitest'
import { mountWithPlugins } from '@/testing/mountWithPlugins'
import { scrollTarget } from '@/testing/scrollTarget'
import AboutHeroStatsSection from './AboutHeroStatsSection.vue'

describe('AboutHeroStatsSection', () => {
  it('scrolls to the contact section', async () => {
    const scroll = scrollTarget('contact-section')
    const wrapper = mountWithPlugins(AboutHeroStatsSection)

    await wrapper.find('.about__cta').trigger('click')

    expect(scroll).toHaveBeenCalledWith({ behavior: 'smooth' })
  })

  it('does nothing without a contact section', async () => {
    const wrapper = mountWithPlugins(AboutHeroStatsSection)

    await wrapper.find('.about__cta').trigger('click')

    expect(wrapper.find('.about__cta').exists()).toBe(true)
  })
})
