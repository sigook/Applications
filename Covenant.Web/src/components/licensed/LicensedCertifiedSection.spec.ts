import { describe, expect, it } from 'vitest'
import { flushPromises } from '@vue/test-utils'
import { mountAt } from '@/testing/mountWithPlugins'
import { scrollTarget } from '@/testing/scrollTarget'
import LicensedCertifiedSection from './LicensedCertifiedSection.vue'

describe('LicensedCertifiedSection', () => {
  it('scrolls in place when already on the about page', async () => {
    const scroll = scrollTarget('contact-section')
    const { wrapper, router } = await mountAt(LicensedCertifiedSection, '/about')

    await wrapper.find('.licensed__cta').trigger('click')
    await flushPromises()

    expect(scroll).toHaveBeenCalledWith({ behavior: 'smooth' })
    expect(router.currentRoute.value.fullPath).toBe('/about')
  })

  it('ignores the click on the about page without a contact section', async () => {
    const { wrapper, router } = await mountAt(LicensedCertifiedSection, '/about')

    await wrapper.find('.licensed__cta').trigger('click')
    await flushPromises()

    expect(router.currentRoute.value.fullPath).toBe('/about')
  })

  it('navigates to the about contact section from other pages', async () => {
    const scroll = scrollTarget('contact-section')
    const { wrapper, router } = await mountAt(LicensedCertifiedSection, '/licensed-certified')

    await wrapper.find('.licensed__cta').trigger('click')
    await flushPromises()

    expect(router.currentRoute.value.fullPath).toBe('/about#contact-section')
    expect(scroll).toHaveBeenCalledWith({ behavior: 'smooth' })
  })
})
