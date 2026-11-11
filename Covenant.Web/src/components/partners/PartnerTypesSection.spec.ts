import { describe, expect, it } from 'vitest'
import { mountWithPlugins } from '@/testing/mountWithPlugins'
import { scrollTarget } from '@/testing/scrollTarget'
import PartnerTypesSection from './PartnerTypesSection.vue'

describe('PartnerTypesSection', () => {
  it('starts on business and switches to recruiter', async () => {
    const wrapper = mountWithPlugins(PartnerTypesSection)
    const [recruiter, business] = wrapper.findAll('.partner-types__btn')
    expect(business.classes()).toContain('partner-types__btn--active')

    await recruiter.trigger('click')

    expect(recruiter.classes()).toContain('partner-types__btn--active')
    expect(business.classes()).not.toContain('partner-types__btn--active')
  })

  it('scrolls to the partner contact section', async () => {
    const scroll = scrollTarget('partner-contact-section')
    const wrapper = mountWithPlugins(PartnerTypesSection)

    await wrapper.find('.partner-types__cta').trigger('click')

    expect(scroll).toHaveBeenCalledWith({ behavior: 'smooth' })
  })

  it('does nothing without a partner contact section', async () => {
    const wrapper = mountWithPlugins(PartnerTypesSection)

    await wrapper.find('.partner-types__cta').trigger('click')

    expect(wrapper.find('.partner-types__cta').exists()).toBe(true)
  })
})
