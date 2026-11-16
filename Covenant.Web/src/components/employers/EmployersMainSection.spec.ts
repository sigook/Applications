import { describe, expect, it } from 'vitest'
import { flushPromises } from '@vue/test-utils'
import { mountAt } from '@/testing/mountWithPlugins'
import { scrollTarget } from '@/testing/scrollTarget'
import EmployersMainSection from './EmployersMainSection.vue'

describe('EmployersMainSection', () => {
  it('starts on industrial and switches to professional', async () => {
    const { wrapper } = await mountAt(EmployersMainSection, '/employers')
    expect(wrapper.text()).toContain('TEMP TO PERM')

    await wrapper.findAll('.switch-btn')[0].trigger('click')

    expect(wrapper.text()).toContain('DIRECT HIRING')
  })

  it('opens the tab from the query and follows query changes', async () => {
    const { wrapper, router } = await mountAt(EmployersMainSection, '/employers?type=professional')
    expect(wrapper.text()).toContain('DIRECT HIRING')

    await router.push('/employers?type=industrial')
    await flushPromises()
    expect(wrapper.text()).toContain('TEMP TO PERM')

    await router.push('/employers?type=other')
    await flushPromises()
    expect(wrapper.text()).toContain('TEMP TO PERM')
  })

  it('shows and hides the benefits of each card', async () => {
    const { wrapper } = await mountAt(EmployersMainSection, '/employers')
    const green = wrapper.find('.card-green')
    const blue = wrapper.find('.card-blue')

    await green.find('.btn-pill').trigger('click')
    await blue.find('.btn-pill').trigger('click')
    expect(green.classes()).toContain('sharp-mobile')
    expect(blue.classes()).toContain('sharp-mobile')

    await green.find('.btn-back-circle').trigger('click')
    await blue.find('.btn-back-circle').trigger('click')
    expect(green.classes()).not.toContain('sharp-mobile')
    expect(blue.classes()).not.toContain('sharp-mobile')
  })

  it('scrolls to the contact section', async () => {
    const scroll = scrollTarget('contact-section')
    const { wrapper } = await mountAt(EmployersMainSection, '/employers')

    await wrapper.find('.btn-contact-outline').trigger('click')

    expect(scroll).toHaveBeenCalled()
  })

  it('does nothing without a contact section', async () => {
    const { wrapper } = await mountAt(EmployersMainSection, '/employers')

    await wrapper.find('.btn-contact-outline').trigger('click')

    expect(wrapper.find('.btn-contact-outline').exists()).toBe(true)
  })
})
