import { describe, expect, it } from 'vitest'
import { flushPromises } from '@vue/test-utils'
import { defineComponent } from 'vue'
import { mountAt } from '@/testing/mountWithPlugins'
import TalentsMainSection from './TalentsMainSection.vue'

const Blank = defineComponent({ render: () => null })
const routes = [{ path: '/open-positions', name: 'open_positions', component: Blank }]

describe('TalentsMainSection', () => {
  it('starts on professional and resets the cards when switching tabs', async () => {
    const { wrapper } = await mountAt(TalentsMainSection, '/talents', {}, routes)
    expect(wrapper.text()).toContain('Direct Hiring')
    await wrapper.find('.card-green .btn-pill').trigger('click')
    await wrapper.find('.card-blue .btn-pill').trigger('click')
    expect(wrapper.find('.card-green').classes()).toContain('sharp-mobile')

    await wrapper.findAll('.switch-btn')[1].trigger('click')

    expect(wrapper.text()).toContain('Temp to Perm')
    expect(wrapper.find('.card-green').classes()).not.toContain('sharp-mobile')
    expect(wrapper.find('.card-blue').classes()).not.toContain('sharp-mobile')
  })

  it('closes the benefits with the back buttons', async () => {
    const { wrapper } = await mountAt(TalentsMainSection, '/talents', {}, routes)
    await wrapper.find('.card-green .btn-pill').trigger('click')
    await wrapper.find('.card-blue .btn-pill').trigger('click')

    await wrapper.find('.card-green .btn-back-circle').trigger('click')
    await wrapper.find('.card-blue .btn-back-circle').trigger('click')

    expect(wrapper.find('.card-green').classes()).not.toContain('sharp-mobile')
    expect(wrapper.find('.card-blue').classes()).not.toContain('sharp-mobile')
  })

  it('opens the tab from the query and follows query changes', async () => {
    const { wrapper, router } = await mountAt(TalentsMainSection, '/talents?type=industrial', {}, routes)
    expect(wrapper.text()).toContain('Temp to Perm')

    await router.push('/talents?type=professional')
    await flushPromises()

    expect(wrapper.text()).toContain('Direct Hiring')
  })

  it('navigates to the open positions', async () => {
    const { wrapper, router } = await mountAt(TalentsMainSection, '/talents', {}, routes)

    await wrapper.find('.btn-contact-outline').trigger('click')
    await flushPromises()

    expect(router.currentRoute.value.name).toBe('open_positions')
  })
})
